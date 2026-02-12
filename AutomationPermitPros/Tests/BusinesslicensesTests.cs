using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Flows;
using AutomationPermitPros.Pages;
using AutomationPermitPros.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
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
            // Only create Playwright once. Browser/context/page are created per EnvUsers row inside the test.
            _playwright = await Playwright.CreateAsync();

            _screenshotDirectory = Path.Combine(AppContext.BaseDirectory, "Screenshots");
            if (!Directory.Exists(_screenshotDirectory))
            {
                Directory.CreateDirectory(_screenshotDirectory);
            }
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
            // Read EnvUsers sheet and run the whole test-suite for each row where RUN = TRUE
            var envRows = ExcelDataProvider.GetData(
                TestDataConfig.TestDataExcel,
                "EnvUsers"
            ).Where(r => ExcelHelper.IsTrue(r, "RUN") || ExcelHelper.IsTrue(r, "Run")).ToList();

            if (envRows.Count == 0)
                throw new InvalidOperationException("No RUN=true row found in 'EnvUsers' sheet. Add at least one row with RUN=TRUE.");

            foreach (var envRow in envRows)
            {
                // Extract required runtime values from the env row (case-insensitive keys)
                static string? GetFirst(Dictionary<string, string> row, params string[] keys)
                {
                    foreach (var k in keys)
                    {
                        var match = row.Keys.FirstOrDefault(x => string.Equals(x?.Trim(), k, StringComparison.OrdinalIgnoreCase));
                        if (match != null)
                        {
                            var v = row[match];
                            if (!string.IsNullOrWhiteSpace(v))
                                return v.Trim();
                        }
                    }
                    return null;
                }

                var url = GetFirst(envRow, "URL") ?? throw new InvalidOperationException("EnvUsers row missing URL");
                var user = GetFirst(envRow, "UserEmail", "Username", "User") ?? throw new InvalidOperationException("EnvUsers row missing Username/UserEmail");
                var pwd = GetFirst(envRow, "Password") ?? throw new InvalidOperationException("EnvUsers row missing Password");
                var browserType = GetFirst(envRow, "Browser Type", "BrowserType", "Browser") ?? throw new InvalidOperationException("EnvUsers row missing Browser Type");

                // Override singleton TestConfiguration runtime values for this iteration
                TestConfiguration.Instance.AppSettings.BaseUrl = url;
                TestConfiguration.Instance.AppSettings.BrowserType = browserType;
                TestConfiguration.Instance.Credentials.Username = user;
                TestConfiguration.Instance.Credentials.Password = pwd;

                Console.WriteLine($"=== Running tests for Env row: URL={url}, User={user}, Browser={browserType} ===");

                // Launch browser for this env row
                var settings = TestConfiguration.Instance.AppSettings;
                _browser = await BrowserFactory.LaunchAsync(_playwright, settings);

                // Create new context and page for this env
                var context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height =  1080 }
                });

                _page = await context.NewPageAsync();

                // Perform login (AuthSetup was previously one-time; we login per env row here)
                var loginBlock = new LoginBlock(_page);
                var loginResult = await loginBlock.LOGIN();
                if (!loginResult)
                {
                    Console.WriteLine($"Login failed for user {user} — skipping this env row.");
                    await _browser.CloseAsync();
                    _browser = null;
                    continue; // move to next env row
                }

                // Wait for app root to load before running module sheets
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await _page.WaitForSelectorAsync("nav, .sidebar, [data-testid=\"sidebar\"]", new() { Timeout = 15000 });
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var sideBar = new SidebarNavigationBlock(_page);

                // 1️⃣ Read controller sheet
                var controllerData = ExcelDataProvider.GetData(
                    TestDataConfig.TestDataExcel,
                    TestDataConfig.ControllerSheet
                );
                var executor = new ModuleExecutor(_page);



                foreach (var controllerRow in controllerData)
                {
                    if (!ExcelHelper.IsTrue(controllerRow, "Run"))
                        continue;

                    string module = controllerRow["Module"].Trim();
                    string operation = controllerRow["Operation"].Trim();
                    string sheetName = controllerRow.GetValueOrDefault("SheetName")?.Trim() ?? $"{module}_{operation}";

                    Console.WriteLine($"=== Running Module '{module}' Operation '{operation}', sheet='{sheetName}' ===");

                    // dispatch per module (explicit navigation then per-operation execution)
                    switch (module)
                    {
                        case "BusinessLicense":
                        case "BUSLIC":
                            await sideBar.NavigateToAsync("Business Licenses");
                            await executor.ExecuteBusinessLicenseAsync(operation,sheetName);
                            break;

                        case "Location":
                        case "LOC":
                            await sideBar.NavigateToAsync("Locations");
                            await executor.ExecuteLocationAsync(operation,sheetName);
                            break;

                        default:
                            Console.WriteLine($"Unknown module '{module}' in controller. Skipping.");
                            break;
                    }
                }

                // Close browser for this env row and continue to next
                await _browser.CloseAsync();
                _browser = null;
                _page = null;
            }
        }

        
        }
    }


