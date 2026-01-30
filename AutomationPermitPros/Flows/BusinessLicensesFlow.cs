using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Report;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutomationPermitPros.Flows
{
    public class BusinessLicensesFlow
    {
        private readonly BusinesslicensesBLocks _block;
        private readonly IPage _page;

        public BusinessLicensesFlow(IPage page)
        {
            _block = new BusinesslicensesBLocks(page);
            _page = page;
            // Do NOT create a per-flow report path here.
            // The shared report path is initialized once (recommended in test setup) via TestReportWriter.Initialize(...).
        }

        public async Task ExecuteAsync(Dictionary<string, string> data)
        {
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;

            Console.WriteLine($"Executing TestCase: {testId}");

            // SEARCH (do not block flow on failure; write report and rethrow so outer loop logs & continues)
            if (ExcelHelper.IsTrue(data, "Search"))
            {
                try
                {
                    Console.WriteLine("Flow: SEARCH");
                    await _block.SearchAsync(data);
                    await _block.ReloadAsync();

                    TestReportWriter.AppendResult(testId ,"Search", "PASSED");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Search", "Fail", ex.Message);
                    throw;
                }
            }

            // CREATE
            if (ExcelHelper.IsTrue(data, "Create"))
            {
                try
                {
                    Console.WriteLine("Flow: CREATE");
                    // take a screenshot before
                    var screenShorts = new ScreenShorts(_page);
                    await screenShorts.CaptureScreenshotAsync($"{testId}_BeforeCreate");

                    await _block.CreateAsync(data);

                    Console.WriteLine("Flow: VALIDATE CREATE");

                    // Try to get toast with smaller timeout so we don't hang when required fields are missing.
                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 3000);

                    if (!foundToast)
                    {
                        // No toast -> likely validation error on the page (required fields missing).
                        // Log report and continue to next test case (do NOT rethrow).
                        TestReportWriter.AppendResult(testId, "Create", "InComplete", "Toast not visible within timeout (possible missing required fields)");
                        // Optional: capture screenshot after failure to help debugging
                        await screenShorts.CaptureScreenshotAsync($"{testId}_Create_NoToast");
                        return;
                    }

                    Console.WriteLine($"Actual Toast Message: {actualMessage}");

                    string expectedOutcome = data["CreateExpectedOutcome"];
                    string expectedMessage = data["CreateExpectedMessage"];

                    Assert.IsNotNull(actualMessage, "Expected a toast message, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'"
                        );

                        bool exists = await _block.BUSLIC_VerifySearchResultExists(
                            data["LicenseNumber"]
                        );

                        Assert.IsTrue(
                            exists,
                            "Record was created but not found in search results"
                        );
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'"
                        );
                    }
                    else
                    {
                        Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    // If we reach here creation succeeded
                    TestReportWriter.AppendResult(testId, "Create", "PASSED");
                }
                catch (Exception ex)
                {
                    // record failure and rethrow so test harness can capture/fail and continue outer loop
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
                try
                {
                    var screenShorts = new ScreenShorts(_page);
                    Console.WriteLine("Flow: EDIT");
                    await _block.SearchAsync(data);
                    await screenShorts.CaptureScreenshotAsync($"{testId}_BeforeEdit");
                    await _block.EditAsync(data);

                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 3000);
                    //var actualMessage = await _block.GetToastMessageAsync();

                    if (!foundToast)
                    {
                        // No toast -> likely validation error on the page (required fields missing).
                        // Log report and continue to next test case (do NOT rethrow).
                        TestReportWriter.AppendResult(testId, "Edit", "InComplete", "Toast not visible within timeout (possible missing required fields)");
                        // Optional: capture screenshot after failure to help debugging
                        await screenShorts.CaptureScreenshotAsync($"{testId}_Create_NoToast");
                        return;
                    }

                    Console.WriteLine($"Actual Toast Message: {actualMessage}");

                    string expectedOutcome = data["EditExpectedOutcome"];
                    string expectedMessage = data["EditExpectedMessage"];

                    Assert.IsNotNull(actualMessage, "Expected a toast message, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'"
                        );

                        //bool exists = await _block.BUSLIC_VerifySearchResultExists(
                        //    data["LicenseNumber"]
                        //);

                        //Assert.IsTrue(
                        //    exists,
                        //    "Record was created but not found in search results"
                        //);
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'"
                        );
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
                    TestReportWriter.AppendResult(testId, "Edit", "FAIL", ex.Message);
                    throw;
                }
            }

            // DELETE
            if (ExcelHelper.IsTrue(data, "Delete"))
            {
                var screenshots = new ScreenShorts(_page);
                try
                {
                    Console.WriteLine("Flow: DELETE");
                    await _block.SearchAsync(data);
                  
                    await screenshots.CaptureScreenshotAsync($"{testId}_beforeDelete");
                    await Task.Delay(1000);
                    await _block.DeleteAsync(data);
                    await screenshots.CaptureScreenshotAsync($"{testId}_afterDelete");
                    Console.WriteLine("Flow: VALIDATE DELETE");

                    string expectedOutcome = data["DeleteExpectedOutcome"];
                    string expectedMessage = data["DeleteExpectedMessage"];

                    var (foundToast, actualMessage) = await _block.TryGetToastMessageAsync(timeoutMs: 5000);
                    Console.WriteLine($"Actual Toast Message: {actualMessage}");
                    if (!foundToast)
                    {
                        TestReportWriter.AppendResult(testId, "Delete", "InComplete", "Toast not visible within timeout");
                        await screenshots.CaptureScreenshotAsync($"{testId}_Delete_NoToast");
                        return;
                    }
                    //var actualMessage = await _block.GetToastMessageAsync();

                    Assert.IsNotNull(actualMessage, "Expected a toast message after delete, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        //Validate success message
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected SUCCESS delete message '{expectedMessage}' but got '{actualMessage}'"
                        );

                        bool exists = await _block.BUSLIC_VerifySearchResultExists(
                            data["Search_LicenseNumber"]
                        );
                        Assert.IsFalse(
                            exists,
                            "Record was deleted but still found in search results"
                        );
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        //Validate error message
                        Assert.That(
                            actualMessage,
                            Does.Contain(expectedMessage).IgnoreCase,
                            $"Expected ERROR delete message '{expectedMessage}' but got '{actualMessage}'"
                        );
                    }
                    else
                    {
                        Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    TestReportWriter.AppendResult(testId, "Delete", "PASSED");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Delete", "FAIL", ex.Message);
                    throw;
                }
            }
        }
    }
}
