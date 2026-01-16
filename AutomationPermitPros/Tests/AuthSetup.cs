using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
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
            var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = false }
            );


            var context = await browser.NewContextAsync();
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
