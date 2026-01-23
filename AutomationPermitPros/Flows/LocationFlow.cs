using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Report;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutomationPermitPros.Flows
{
    public class LocationFlow
    {

        private readonly LocationBlocks _block;

        public LocationFlow(IPage page)
        {
            _block = new LocationBlocks(page);
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

                    TestReportWriter.AppendResult(testId, "Search", "Completed");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Search", "InComplete", ex.Message);
                    throw;
                }
            }

            // CREATE
            if (ExcelHelper.IsTrue(data, "Create"))
            {
                try
                {
                    Console.WriteLine("Flow: CREATE");
                    await _block.CreateAsync(data);

                    Console.WriteLine("Flow: VALIDATE CREATE");

                    string expectedOutcome = data["ExpectedOutcome"];
                    string expectedMessage = data["ExpectedMessage"];

                    string actualMessage = await _block.GetToastMessageAsync();
                    Console.WriteLine($"Actual Toast Message: {actualMessage}");

                    if (actualMessage is null)
                        throw new Exception("Expected a toast message, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!actualMessage.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase))
                            throw new Exception($"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!actualMessage.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase))
                            throw new Exception($"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'");
                    }
                    else
                    {
                        throw new Exception($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    TestReportWriter.AppendResult(testId, "Create", "Completed");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Create", "InComplete", ex.Message);
                    throw;
                }
            }

            // EDIT
            if (ExcelHelper.IsTrue(data, "Edit"))
            {
                try
                {
                    Console.WriteLine("Flow: EDIT");
                    await _block.SearchAsync(data);
                    await _block.EditAsync(data);

                    TestReportWriter.AppendResult(testId, "Edit", "Completed");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Edit", "InComplete", ex.Message);
                    throw;
                }
            }

            // DELETE
            if (ExcelHelper.IsTrue(data, "Delete"))
            {
                try
                {
                    Console.WriteLine("Flow: DELETE");
                    await _block.SearchAsync(data);

                    await _block.DeleteAsync(data);

                    Console.WriteLine("Flow: VALIDATE DELETE");

                    string expectedOutcome = data["ExpectedOutcome"];
                    string expectedMessage = data["ExpectedMessage"];

                    string actualMessage = await _block.GetToastMessageAsync();

                    if (actualMessage is null)
                        throw new Exception("Expected a toast message after delete, but none appeared");

                    if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!actualMessage.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase))
                            throw new Exception($"Expected SUCCESS delete message '{expectedMessage}' but got '{actualMessage}'");

                        bool exists = await _block.LOCATION_VerifySearchResultExists(
                            data.GetValueOrDefault("Search_LocationNumber")
                        );

                        if (exists)
                            throw new Exception("Record still exists in search results after successful delete");
                    }
                    else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!actualMessage.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase))
                            throw new Exception($"Expected ERROR delete message '{expectedMessage}' but got '{actualMessage}'");

                        bool exists = await _block.LOCATION_VerifySearchResultExists(
                            data.GetValueOrDefault("Search_LocationNumber")
                        );

                        if (!exists)
                            throw new Exception("Record does NOT exist even though delete failed");
                    }
                    else
                    {
                        throw new Exception($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                    }

                    TestReportWriter.AppendResult(testId, "Delete", "Completed");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "Delete", "InComplete", ex.Message);
                    throw;
                }
            }

            // VIEW
            if (ExcelHelper.IsTrue(data, "View"))
            {
                try
                {
                    await _block.SearchAsync(data);

                    Console.WriteLine("Flow: VIEW");
                    await _block.ViewAsync();

                    TestReportWriter.AppendResult(testId, "View", "Completed");
                }
                catch (Exception ex)
                {
                    TestReportWriter.AppendResult(testId, "View", "InComplete", ex.Message);
                    throw;
                }
            }
        }
    }
}
