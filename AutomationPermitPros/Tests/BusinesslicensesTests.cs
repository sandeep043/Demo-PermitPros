using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Flows;
using AutomationPermitPros.Pages;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationPermitPros.Tests
{
    [TestFixture]
    internal class BusinesslicensesTests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private string _screenshotDirectory;
        //private string _videoRecordDirectory;
        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            //_browser = await _playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions
            //{
            //    Headless = false,
            //    Args = new[] { "--window-size=1920,1080" }
            //});
            var settings = TestConfiguration.Instance.AppSettings;
            _browser = await BrowserFactory.LaunchAsync(_playwright, settings);

            _screenshotDirectory = Path.Combine(AppContext.BaseDirectory, "Screenshots");
            //_videoRecordDirectory = Path.Combine(AppContext.BaseDirectory, "Videos");
            if (!Directory.Exists(_screenshotDirectory))
            {
                Directory.CreateDirectory(_screenshotDirectory);
            }
            //if (!Directory.Exists(_videoRecordDirectory))
            //{
            //    Directory.CreateDirectory(_videoRecordDirectory);
            //}


            var storagePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "auth.json");
            Assert.IsTrue(File.Exists(storagePath), $"Authentication storage file not found at '{storagePath}'. Run AuthSetup first.");
            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                StorageStatePath = storagePath,
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });

            _page = await context.NewPageAsync();

            // Important: storage state doesn't navigate the page — go to app root so UI loads.
            var baseUrl = TestConfiguration.Instance.AppSettings.BaseUrl;
            await _page.GotoAsync(baseUrl);
            // Wait for network and visible sidebar/root element
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForSelectorAsync("nav, .sidebar, [data-testid=\"sidebar\"]", new() { Timeout = 15000 });
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }


        [TearDown]
        public async Task Teardown()
        {
            if (_browser != null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        [Test]
        public async Task Execute_Modules_Based_On_TestNames_Sheet()
        {
            var sideBar = new SidebarNavigationBlock(_page);

            // 1️⃣ Read controller sheet
            var controllerData = ExcelDataProvider.GetData(
                TestDataConfig.TestDataExcel,
                TestDataConfig.ControllerSheet
            );

            foreach (var controllerRow in controllerData)
            {
                if (!ExcelHelper.IsTrue(controllerRow, "Run"))
                    continue;

                string sheetName = controllerRow["SheetName"].Trim();

                Console.WriteLine($"=== Running Module Sheet: {sheetName} ===");

                // 2️⃣ Navigate based on module
                switch (sheetName)
                {
                    case "BusinessLicense_TestData":
                        await sideBar.NavigateToAsync("Business Licenses");
                        await ExecuteModuleSheet(
                            sheetName,
                            new BusinessLicensesFlow(_page)
                        );
                        break;

                    case "Locations_TestData":
                        await sideBar.NavigateToAsync("Locations");
                        await ExecuteModuleSheet(
                            sheetName,
                            new LocationFlow(_page)
                        );
                        break;

                    default:
                        throw new Exception($"Unknown SheetName '{sheetName}' in TestNames");
                }
            }
        }
 private async Task ExecuteModuleSheet(
    string sheetName,
    dynamic flow)
        {
            var testData = ExcelDataProvider.GetData(
                TestDataConfig.TestDataExcel,
                sheetName
            );

            //int excelRow = 2; // header is row 1

            foreach (var row in testData)
            {
                if (!ExcelHelper.IsTrue(row, "Run"))
                {
                    //excelRow++; 
                    continue;
                }

                Console.WriteLine($"Executing {sheetName} | TestCaseID = {row["TestCaseID"]}");

                try
                {
                    await flow.ExecuteAsync(row);

                    //ExcelResultWriter.WriteResult(
                    //    TestDataConfig.TestDataExcel, b     
                    //    sheetName,
                    //    excelRow,
                    //    "PASS"
                    //);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TestCaseID {row["TestCaseID"]} FAILED: {ex.Message}");
                    //ExcelResultWriter.WriteResult(
                    //    TestDataConfig.TestDataExcel,
                    //    sheetName,
                    //    excelRow,
                    //    "FAIL",
                    //    ex.Message
                    //);
                }
                finally
                {
                    await _page.ReloadAsync();
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    //excelRow++;
                }
            }
        }
    }
}

