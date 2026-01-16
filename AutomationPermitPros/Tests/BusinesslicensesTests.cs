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
            _browser = await _playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = false });


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
                StorageStatePath = storagePath
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
        public async Task BusinessLicenses_Execute_From_Excel()
        {
            // 1️⃣ Navigate once
            var sideBar = new SidebarNavigationBlock(_page);
            Assert.IsTrue(
                await sideBar.NavigateToAsync("Business Licenses"),
                "Navigation to Business Licenses failed");

            // 2️⃣ Read Excel
            var testData = ExcelDataProvider.GetData(
                TestDataConfig.BusinessLicensesExcel,
                TestDataConfig.BusinessLicensesSheet);

            // 3️⃣ Create Flow
            var flow = new BusinessLicensesFlow(_page);

            // 4️⃣ Execute ALL rows
            foreach (var row in testData)
            {
                // 1️⃣ Skip header-like or invalid rows
                if (!row.ContainsKey("TestCaseID") ||
                    row["TestCaseID"].Equals("Unique ID", StringComparison.OrdinalIgnoreCase))
                    continue;

                // 2️⃣ Skip rows where Run != TRUE
                if (!ExcelHelper.IsTrue(row, "Run"))
                    continue;

                Console.WriteLine($"Executing TestCaseID = {row["TestCaseID"]}");

                await flow.ExecuteAsync(row);

                await _page.ReloadAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }
    }
}
