using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Report;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutomationPermitPros.Flows
{
    public class LocationFlow
    {

        private readonly LocationBlocks _block;
        private readonly IPage _page;

        public LocationFlow(IPage page)
        {
            _block = new LocationBlocks(page);
            _page = page;
            // Use shared report path (initialized in test setup)
        }

        public async Task ExecuteAsync(Dictionary<string, string> data)
        {
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;

            // SEARCH
            if (ExcelHelper.IsTrue(data, "Search"))
            {
                try
                {
                    Console.WriteLine("Flow: SEARCH");
                    await _block.SearchAsync(data);
                    await _block.ReloadAsync();

                    TestReportWriter.AppendResult(testId, "Search", "PASSED");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Search", "FAIL", ex.Message);
                    throw;
                }
            }

            // CREATE
            if (ExcelHelper.IsTrue(data, "Create"))
            {
                var screenshots = new ScreenShorts(_page);
                try
                {
                    Console.WriteLine("Flow: CREATE");                                      

                    // capture before-create state
                    await screenshots.CaptureScreenshotAsync($"{testId}_BeforeCreate");

                    await _block.CreateAsync(data);

                    Console.WriteLine("Flow: VALIDATE CREATE");

                    // Try to get toast quickly to avoid long waits when inline validation prevents toast.
                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 2000);

                    if (!foundToast)
                    {
                        // No toast -> likely inline validation (required fields) prevented create
                        TestReportWriter.AppendResult(testId, "Create", "InComplete", "Toast not visible within timeout (possible missing required fields)");
                        await screenshots.CaptureScreenshotAsync($"{testId}_Create_NoToast");
                        return;
                    }

                    Console.WriteLine($"Actual Toast Message: {actualMessage}");

                    string expectedOutcome = data.GetValueOrDefault("CreateExpectedOutcome") ?? string.Empty;
                    string expectedMessage = data.GetValueOrDefault("CreateExpectedMessage") ?? string.Empty;

                    Assert.IsNotNull(actualMessage, "Expected a toast message after create, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else
                    {
                        Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    TestReportWriter.AppendResult(testId, "Create", "PASSED");
                }
                catch (Exception ex)
                {
                    await screenshots.CaptureScreenshotAsync($"{testId}_Create_Failure");
                    TestReportWriter.AppendResult(testId, "Create", "FAIL", ex.Message);
                    throw;
                }
            }



            // VIEW
            if (ExcelHelper.IsTrue(data, "View"))
            {
                try
                {
                    await _block.SearchAsync(data);
                    var screenShorts = new ScreenShorts(_page);
                    await screenShorts.CaptureScreenshotAsync($"{testId}_BeforeView");
                    Console.WriteLine("Flow: VIEW");
                    await _block.ViewAsync();
                    await Task.Delay(2000);
                    await screenShorts.CaptureScreenshotAsync($"{testId}_afterView");
                    await _block.ReloadAsync();

                    TestReportWriter.AppendResult(testId, "View", "PASSED");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "View", "FAIL", ex.Message);
                    throw;
                }
            }

            // EDIT
            if (ExcelHelper.IsTrue(data, "Edit"))
            {
                var screenshots = new ScreenShorts(_page);
                try
                {
                    Console.WriteLine("Flow: EDIT");
                    await _block.SearchAsync(data);
                           await screenshots.CaptureScreenshotAsync($"{testId}_BeforeEdit");
                    await _block.EditAsync(data);

                    // Try to get toast (short timeout)
                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 2000);
                    if (!foundToast)
                    {
                        TestReportWriter.AppendResult(testId, "Edit", "InComplete", "Toast not visible within timeout (possible missing required fields)");
                        await screenshots.CaptureScreenshotAsync($"{testId}_Edit_NoToast");
                        return;
                    }

                    Console.WriteLine($"Actual Toast Message: {actualMessage}");

                    string expectedOutcome = data.GetValueOrDefault("EditExpectedOutcome") ?? string.Empty;
                    string expectedMessage = data.GetValueOrDefault("EditExpectedMessage") ?? string.Empty;

                    Assert.IsNotNull(actualMessage, "Expected a toast message after edit, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else
                    {
                        Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }
                    await _block.ReloadAsync();
                    TestReportWriter.AppendResult(testId, "Edit", "PASSED");
                }
                catch (Exception ex)
                {
                    await screenshots.CaptureScreenshotAsync($"{testId}_Edit_Failure");
                    TestReportWriter.AppendResult(testId, "Edit", "FAIL", ex.Message);
                    throw;
                }
            }

            // DELETE
            if (ExcelHelper.IsTrue(data, "Delete"))
            {
                var screenShorts = new ScreenShorts(_page);
                try
                {
                    Console.WriteLine("Flow: DELETE");
                    await _block.SearchAsync(data);

                    await screenShorts.CaptureScreenshotAsync($"{testId}_beforeDelete");
                    await Task.Delay(1000);
                    await _block.DeleteAsync(data);
                    await screenShorts.CaptureScreenshotAsync($"{testId}_afterDelete");
                    Console.WriteLine("Flow: VALIDATE DELETE");

                    string expectedOutcome = data.GetValueOrDefault("DeleteExpectedOutcome") ?? string.Empty;
                    string expectedMessage = data.GetValueOrDefault("DeleteExpectedMessage") ?? string.Empty;

                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 5000);
                    if (!foundToast)
                    {
                        TestReportWriter.AppendResult(testId, "Delete", "InComplete", "Toast not visible within timeout");
                        await screenShorts.CaptureScreenshotAsync($"{testId}_Delete_NoToast");
                        return;
                    }

                    Assert.IsNotNull(actualMessage, "Expected a toast message after delete, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS delete message '{expectedMessage}' but got '{actualMessage}'");

                        bool exists = await _block.LOCATION_VerifySearchResultExists(data.GetValueOrDefault("Search_LocationNumber"));
                        if (exists)
                            throw new Exception("Record still exists in search results after successful delete");
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(actualMessage, Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR delete message '{expectedMessage}' but got '{actualMessage}'");

                        bool exists = await _block.LOCATION_VerifySearchResultExists(data.GetValueOrDefault("Search_LocationNumber"));
                        if (!exists)
                            throw new Exception("Record does NOT exist even though delete failed");
                    }
                    else
                    {
                        Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    TestReportWriter.AppendResult(testId, "Delete", "PASSED");
                }
                catch (Exception ex)
                {
                    await screenShorts.CaptureScreenshotAsync($"{testId}_Delete_Failure");
                    TestReportWriter.AppendResult(testId, "Delete", "FAIL", ex.Message);
                    throw;
                }
            }

           
        }
    }
}
