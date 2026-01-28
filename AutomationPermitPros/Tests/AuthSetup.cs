using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationPermitPros.Tests
{
    [SetUpFixture]
    public class AuthSetup
    {
        

        [OneTimeSetUp]
        public async Task GlobalLoginSetup()
        {
            using var playwright = await Playwright.CreateAsync();
            //var browser = await playwright.Chromium.LaunchAsync(
            //    new BrowserTypeLaunchOptions
            //    {
            //        Headless = false,
            //        Args = new[] { "--window-size=1920,1080" }
            //    }
            //); 
            var settings = TestConfiguration.Instance.AppSettings;
            var browser = await BrowserFactory.LaunchAsync(playwright, settings);

            // Create the context with a fixed viewport that matches the browser window size.
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });

            var page = await context.NewPageAsync();
            var screenShorts = new ScreenShorts(page);
            // Login once
            var loginBlock = new LoginBlock(page);
            var loginResult = await loginBlock.LOGIN();

            //Assert.IsTrue(loginResult, "Login failed in setup");

            await screenShorts.CaptureScreenshotAsync("BusinessLicenses_afterLogin");
            var storagePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "auth.json");
            await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = storagePath });

            await browser.CloseAsync();
        }


    }
}
