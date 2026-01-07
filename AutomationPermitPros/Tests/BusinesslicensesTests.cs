using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Config;
using AutomationPermitPros.Pages;
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


        [Test]
        public async Task BusinessLicenses_SearchWithLocationNameLicenseNumber()
        {


            //select Business Licenses from left menu 
            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");

            //Enter Location Name and License Number and click on search button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("uganda");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("LIC12345678");
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
            //var enterLocationNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNUMBER("001");
            var selectLocationResult = await BusinesslicensesBLock.BUSLIC_SELECT_LOCATION("uganda");
            //var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("Test Location");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("BBC18");
            var selectLicenseTypeResult = await BusinesslicensesBLock.BUSLIC_SELECT_LICENSETYPE("Beer");
            var selectRenewalDateResult =
             await BusinesslicensesBLock
            .BUSLIC_SELECT_RENEWALDATE_CALENDAR(
                //month: "January",
                year: "2002",
                day: "5"
        );
            var selectAgencyResult = await BusinesslicensesBLock.BUSLIC_SELECT_AGENCY("ABC");
            var selectExperationDateResult = await BusinesslicensesBLock.BUSLIC_SELECT_EXPERATIONDATE_CALENDAR(year: "2002",
                day: "5");

            //var selectStateResult = await BusinesslicensesBLock.BUSLIC_SELECT_STATE("California");
            var saveButtonResult = await BusinesslicensesBLock.BUSLIC_CREATE_BTN();

            // set Network Idle timeout to 10 seconds       
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(3000);
        }


        [Test]
        public async Task BusinessLicenses_EditIconFunctionality_EditLicenses_Type()
        {
            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");
            //Click on Create New Business License button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);


            //Enter Location Name and License Number and click on search button 

            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("uganda");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("BBC18");
            await _page.WaitForTimeoutAsync(2000);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();
            await _page.WaitForTimeoutAsync(2000);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var EditIconResult = await BusinesslicensesBLock.BUSLIC_EDIT_ICON();
            var EditSelectLicenseType = await BusinesslicensesBLock.BUSLIC_EditSELECT_LICENSETYPE("Elevator");
            var EditSaveButton = await BusinesslicensesBLock.BUSLIC_ADV_SAVE_BUTTON();
            await _page.WaitForTimeoutAsync(2000);



        }

        [Test]
        public async Task BusinessLicenses_ViewIconFunctionality()
        {
            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");
            //Click on Create New Business License button 
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);


            //Enter Location Name and License Number and click on search button 

            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME("uganda");
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER("LIC12345678");
            await _page.WaitForTimeoutAsync(2000);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();

            await _page.WaitForTimeoutAsync(2000);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var EditIconResult = await BusinesslicensesBLock.BUSLIC_VIEW_ICON();
            await _page.WaitForTimeoutAsync(10000);

        }


        [Test]
        [Category("Delete")]
        [Description("Delete business license withentering reason")]
        public async Task DeleteBusinessLicense_WithtReason_ShouldDeleteSuccessfully()
        {
            var BusinesslicensesBLock = new BusinesslicensesBLocks(_page);
            //Arrange
            string deletionReason = "Testing deletion functionality";
            string locationName = "uganda";
            string licenseNumber = "BBC18";


            var sideBar = new SidebarNavigationBlock(_page);
            var navigationResult = await sideBar.NavigateToAsync("Business Licenses");

            //Enter Location Name and License Number and click on search button 
            var enterLocationNameResult = await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME(locationName);
            var enterLicenseNumberResult = await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER(licenseNumber);
            await _page.WaitForTimeoutAsync(2000);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var searchButtonResult = await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();

            // set Network Idle timeout to 10 seconds    
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);

            // Act
            var result = await BusinesslicensesBLock.BUSLIC_Block_DeleteWithReason(deletionReason) ;

            // Assert
            Assert.IsTrue(result, "Failed to delete business license without reason");

            // Wait for deletion to complete
            await _page.WaitForTimeoutAsync(2000);

            // Verify deletion by searching again
            await BusinesslicensesBLock.BUSLIC_ENTER_LOCATIONNAME(locationName);
            await BusinesslicensesBLock.BUSLIC_ENTER_LICENSENUMBER(licenseNumber);
            await BusinesslicensesBLock.BUSLIC_SEARCHBUTTON();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForTimeoutAsync(2000);

            // Verify the record no longer exists
            var recordExistsAfterDelete = await BusinesslicensesBLock.BUSLIC_VerifySearchResultExists(licenseNumber);
            Assert.IsFalse(recordExistsAfterDelete, $"License {licenseNumber} should NOT exist after deletion");
            Console.WriteLine($"Test Passed: License {licenseNumber} successfully deleted and verified");
        }
    }
}
