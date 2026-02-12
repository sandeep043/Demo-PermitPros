using AutomationPermitPros.Config;
using AutomationPermitPros.Flows;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Tests
{
    public class ModuleExecutor
    {
        private readonly IPage _page;

        public ModuleExecutor(IPage page)
        {
            _page = page;
        }

        public async Task ExecuteBusinessLicenseAsync(string operation, string sheetName)
        {
            var flow = new BusinessLicensesFlow(_page);
            var testData = ExcelDataProvider.GetData(TestDataConfig.TestDataExcel, sheetName);

            foreach (var row in testData)
            {
                if (!ExcelHelper.IsTrue(row, "Run"))
                    continue;

                try
                {
                    switch (operation)
                    {
                        case "Create":
                            await flow.BUSLIC_CreateOnlyAsync();
                            break;
                        case "Search":
                            await flow.BUSLIC_SearchOnlyAsync(row);
                            break;
                        case "Edit":
                            await flow.BUSLIC_EditOnlyAsync(row);
                            break;
                        case "Delete":
                            await flow.BUSLIC_DeleteOnlyAsync(row);
                            break;
                        default:
                            Console.WriteLine($"Unknown operation '{operation}' for BusinessLicense - skipping row");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TestCaseID {row.GetValueOrDefault("TestCaseID", "<no-id>")} FAILED: {ex.Message}");
                }
                finally
                {
                    await _page.ReloadAsync();
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }
        }

        public async Task ExecuteLocationAsync(string operation, string sheetName)
        {
            var flow = new LocationFlow(_page);
            var testData = ExcelDataProvider.GetData(TestDataConfig.TestDataExcel, sheetName);

            foreach (var row in testData)
            {
                if (!ExcelHelper.IsTrue(row, "Run"))
                    continue;

                try
                {
                    switch (operation)
                    {
                        //case "Create":
                        //    await flow.CreateOnlyAsync(row);
                        //    break;
                        //case "Edit":
                        //    await flow.EditOnlyAsync(row);
                        //    break;
                        //case "Delete":
                        //    await flow.DeleteOnlyAsync(row);
                        //    break;
                        //case "Search":
                        //    await flow.SearchOnlyAsync(row);
                        //    break;
                        //default:
                        //    Console.WriteLine($"Unknown operation '{operation}' for Location - skipping row");
                        //    break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TestCaseID {row.GetValueOrDefault("TestCaseID", "<no-id>")} FAILED: {ex.Message}");
                }
                finally
                {
                    await _page.ReloadAsync();
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }

        }
    }
}
