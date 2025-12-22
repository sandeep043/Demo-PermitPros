using AutomationPermitPros.AutomationBlocks;
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



        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = false });
        }

        [TearDown]
        public async Task Teardown()
        {
            if (_browser != null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        [Test]
        public async Task LoginTest()
        {
            var context = await _browser.NewContextAsync();
            var page = await context.NewPageAsync();
            var loginBlock = new LoginBlock(page);
            var Result = await loginBlock.LOGIN();

        }

        [Test]
        public async Task BusinessLicenses_SearchWithLocationNameLicenseNumber()
        {
            var context = await _browser.NewContextAsync();
            var page = await context.NewPageAsync();
            var loginBlock = new LoginBlock(page);
            var loginResult = await loginBlock.LOGIN();

            //select Business Licenses from left menu 
            var sideBar = new SidebarNavigationBlock(page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");

            //Enter Location Name and License Number and click on search button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(page);
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("uganda");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("12345");
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();

            // set Network Idle timeout to 10 seconds    
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForTimeoutAsync(2000);
        }

       

    }
}
