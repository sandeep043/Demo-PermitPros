using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
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
        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = false });

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

        //[Test]
        //public async Task LoginTest()
        //{
        //    var context = await _browser.NewContextAsync();
        //    var page = await context.NewPageAsync();
        //    var loginBlock = new LoginBlock(page);
        //    var Result = await loginBlock.LOGIN();

        //}

        [Test]
        public async Task BusinessLicenses_SearchWithLocationNameLicenseNumber()
        {


            //select Business Licenses from left menu 
            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");

            //Enter Location Name and License Number and click on search button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("uganda");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("12345");
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();

            // set Network Idle timeout to 10 seconds    
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);
        }



        [Test]
        public async Task BusinessLicenses_CreateNewBusinessLicense_withRequiredFields()
        {

            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");
            //Click on Create New Business License button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);

            var createNewButtonResult = await BusinesslicensesBLock.BUSLIC_CREATE_NEW();

            //Enter Location Number, Location Name , License Number , select License Type , State from dropdowns and click on Save button
            var enterLocationNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNUMBER("001");
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("Test Location");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("LIC123456");
            var selectLicenseTypeResult = await BusinesslicensesBLock.BUSLIC_SELECT_LICENSETYPE("Business");
            var selectStateResult = await BusinesslicensesBLock.BUSLIC_SELECT_STATE("California");
            var saveButtonResult = await BusinesslicensesBLock.BUSLIC_CREATE_BTN();
            // set Network Idle timeout to 10 seconds    
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);
        }


        [Test]
        public async Task BusinessLicenses_CreateNewBussinessLicenseAndDeleteLicenseBySearch_()
        {
            
            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");
            //Click on Create New Business License button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);
            var createNewButtonResult = await BusinesslicensesBLock.BUSLIC_CREATE_NEW();
            //Enter Location Number, Location Name , License Number , select License Type , State from dropdowns and click on Save button
            var enterLocationNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNUMBER("002");
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("Delete Test Location");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("123444");
            var selectLicenseTypeResult = await BusinesslicensesBLock.BUSLIC_SELECT_LICENSETYPE("Business");
            var selectStateResult = await BusinesslicensesBLock.BUSLIC_SELECT_STATE("California");
            var saveButtonResult = await BusinesslicensesBLock.BUSLIC_CREATE_BTN();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);


            //Search the created license by License Number and delete it 
            var enterLicenseNumberSearchResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("123444");
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();
            var deleteLicenseResult = await BusinesslicensesBLock.BUSLIC_DELETE_ICON();
            var confirmDeleteResult = await BusinesslicensesBLock.BUSLIC_DELETE_ICON();

            // set Network Idle timeout to 10 seconds    
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);

        }

    }
}
