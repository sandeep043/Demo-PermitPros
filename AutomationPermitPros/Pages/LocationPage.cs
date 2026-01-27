using AutomationPermitPros.Config;
using AutomationPermitPros.Pages.Base;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomationPermitPros.Pages
{

    public class LocationPage
    {
        private readonly BaseListPage _baseListpage;

        private readonly IPage _page;
        public LocationPage(IPage page)
        {
            _baseListpage = new BaseListPage(page);
            _page = page;
        }

        private ILocator SearchLocationNumberInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Number" });

        private ILocator SearchLocationNameInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Name" });

        private ILocator SearchStateDropdown => _page.GetByRole(AriaRole.Combobox).First;


        //create ILocators 

        private ILocator CreateStateDropDown => _page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Select state$") }).Nth(2);

        private ILocator CreateCategories => _page.GetByRole(AriaRole.Textbox, new() { Name = "categories" }).Nth(1);

        private ILocator CreateManagementEntity => _page.GetByRole(AriaRole.Textbox, new() { Name = "categories" }).First;

        private ILocator CreateParentEntity => _page.Locator("select");

        private ILocator DeleteModalTitle => _page.GetByText("Delete Location");

        private ILocator EditStateDropDownInput =>_page.Locator(".css-1xc3v61-indicatorContainer");

        private ILocator EditManagementEntity => _page.Locator(".arrow").First;

        private ILocator EditCategories => _page.Locator("div:nth-child(11) > .custom-multi-select > .select-box > .arrow");


        //Search Location Methods 

        public async Task SearchFillLocationNumberAsync(string value)
        {
            await SearchLocationNumberInput.FillAsync(value);
        }

        public async Task SearchFillLocationNameAsync(string value)
        {
            await SearchLocationNameInput.FillAsync(value);
        }

        public async Task SearchSelectStateAsync(string stateLabel)
        {
            var select = SearchStateDropdown;

            if (await select.CountAsync() == 0)
                throw new Exception("License type combobox not found.");

            // 1️⃣ Open dropdown
            await select.ClickAsync();

            await select.WaitForAsync(new() { Timeout = 2000 });

            // 2️⃣ Type filter text
            await select.SelectOptionAsync(new[] { stateLabel });

            await select.WaitForAsync(new() { Timeout = 2000 });

            //delete
                 
        }

        //Create Location Form fill Methods

        public async Task fillLegalNameAsync(string legalName)
        {
            await _page.GetByPlaceholder("Enter Legal Name").FillAsync(legalName);
        }

        public async Task fillLocationNumber(string locationNumber)
        {
            await _page.GetByPlaceholder("Enter Location Number").FillAsync(locationNumber);
        }

        public async Task fillLocationName(string locationName)
        {
            await _page.GetByPlaceholder("Enter Location Name").FillAsync(locationName);
        }

        public async Task fillOwnerShipAsync(string ownerShip)
        {
            await _page.GetByPlaceholder("Ownership %").FillAsync(ownerShip);
        }
        public async Task EditOwnerShipAsync(string ownerShip)
        {
            var ownerShipInput = _page.GetByPlaceholder("Ownership %");
            await ownerShipInput.ClickAsync();
            await ownerShipInput.FillAsync(ownerShip);
            await ownerShipInput.PressAsync("Enter");
        }

        public async Task StateDropDown(string state)
        {
            // Click the third "Select state" div (nth starts at 0)
            var select = CreateStateDropDown;
            await select.ClickAsync();
            //await _page.Locator("#react-select-2-input").FillAsync(state);

            await _page.Keyboard.TypeAsync(state);
            await Task.Delay(200);

            // Select the exact matching item
            await _page.GetByText(state, new() { Exact = true }).ClickAsync();
        }

        public async Task EditStateDropDown(string state)
        {
            var select = EditStateDropDownInput;
            await select.ClickAsync();
            await _page.Keyboard.TypeAsync(state);
            await Task.Delay(200);

            // Select the exact matching item
            await _page.GetByText(state, new() { Exact = true }).ClickAsync();

        }

        public async Task SelectManagementEntityDropDown(string managementEntity)
        {
            var select = CreateManagementEntity;
            await select.ClickAsync();

            await _page.GetByLabel(managementEntity).ClickAsync();



            //will see how to select value from dropdown
        }

        public async Task EditManagementEntityDropDown(string managementEntity)
        {
            var select = EditManagementEntity;
            await select.ClickAsync();
            await _page.GetByLabel(managementEntity).ClickAsync();
        }

        public async Task SelectCategoriesDropDown(string categories)
        {
            // Click the second categories textbox (nth starts at 0)
            var select = CreateCategories;
            await select.ClickAsync();
            await _page.GetByText(categories, new() { Exact = true }).ClickAsync();
        }

        public async Task EditCategoriesDropDown(string categories)
        {
            var select = EditCategories;
            await select.ClickAsync();
            await _page.GetByText(categories, new() { Exact = true }).ClickAsync();
        }
        //
        public async Task SelectParentEntityDropdown(string parentEntity)
        {
            var select = CreateParentEntity;
            await select.ClickAsync();
            await select.SelectOptionAsync(new SelectOptionValue { Label = parentEntity });
            //press enter 
            //await _page.Keyboard.PressAsync("Enter");
        } 







        public async Task SelectDateOpenedDateFromCalendarAsync(
      string year,
      string day)
        {
            await _baseListpage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 0,
                year: year,
                day: day
            );
        }


        public async Task SelectDateClosedDateFromCalendarAsync(
        string year,
            string day)
        {
            await _baseListpage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 1,
                year: year,
                day: day
            );
        }


        public async Task SelectLocationDescriptionFillAsync(string locationDescription)
        {
            await _page.Locator("div:nth-child(12) > div > .form-control").FillAsync(locationDescription);
        }

        public async Task contactEmailFillAsync(string contactEmail)
        {
            await _page.GetByPlaceholder("Enter Contact Email").FillAsync(contactEmail);
        }

        public async Task contactPhoneFillAsync(string contactPhone)
        {
            await _page.GetByPlaceholder("XXX-XXX-XXXX").FillAsync(contactPhone);
        }


        public async Task AccountingNumberFillAsync(string accountingNumber)
        {
            await _page.GetByPlaceholder("Enter Accounting Number").FillAsync(accountingNumber);
        }


        public async Task NotesFillsync(string notes)
        {
            await _page.Locator("textarea").FillAsync(notes);
        }

        public async Task IsActiveCheckBoxAsync(bool isActive)
        {
            var isChecked = await _page.Locator("#autoSizingCheck2").IsCheckedAsync();
            if (isActive != isChecked)
            {
                await _page.Locator("#autoSizingCheck2").ClickAsync();
            }
        }

        public async Task ClickCreateNew()
        {
            await _baseListpage.ClickCreateNew();
        }

        public async Task ClickCreate()
        {
            await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        private (string Year, string Day) SplitExcelDate(string excelDate)
        {
            var date = DateTime.ParseExact(
                excelDate,
                "MM/dd/yyyy",
                CultureInfo.InvariantCulture);

            return (date.Year.ToString(), date.Day.ToString());
        }

        //Delete Location Method 

        public async Task<bool> IsDeleteModalVisible()
        {
            try
            {
                await DeleteModalTitle.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                return await DeleteModalTitle.IsVisibleAsync();
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> BUSLIC_Click_DeleteIcon()
        {
            return await _baseListpage.Click_DeleteIcon();
        }

        public async Task<bool> BUSLIC_Adv_Delete()
        {
            return await _baseListpage.Adv_Delete();
        }
        public async Task<bool> location_IsDeleteModelVisible()
        {
            return await IsDeleteModalVisible();
        }
        public async Task<bool> BUSLIC_ConfirmDelete()
        {
            return await _baseListpage.ConfirmDelete();
        }






        public async Task<bool> LOC_Click_EditIcon()
        {
            return await _baseListpage.Click_EditIcon();
        }

        public async Task<bool> LOC_Adv_Save()
        {
            return await _baseListpage.Adv_Save();
        }




        public async Task<bool> BUSLIC_Click_ViewIcon()
        {
            return await _baseListpage.Click_ViewIcon();
        }

        public async Task SearchBusinessLicenseAsync(
            string? locationNumber = null,
            string? locationName = null,
            string? state = null
            )
        {
            if (!string.IsNullOrWhiteSpace(locationNumber))
                await SearchFillLocationNumberAsync(locationNumber);
            if (!string.IsNullOrWhiteSpace(locationName))
                await SearchFillLocationNameAsync(locationName);
            if (!string.IsNullOrWhiteSpace(state))
                await SearchSelectStateAsync(state);
            await _baseListpage.ClickSearch();
        }


        public async Task CreateLocationAsync(
            string? testId = null,
            string? legalName = null,
            string? locationNumber = null,
            string? locationName = null,
            string? ownerShip = null,
            string? ParentEntity = null,
            string? dateOpened = null,
            string? AccountingNumber = null,
            string? DateClosed = null,
            string? State = null,
            string? contactPhone = null,
            string? contactEmail = null,
            string? ManagementEntity = null,
            string? categories = null,
            string? LocationDescription = null,
            string? notes = null,
            string? locationDescription = null,
           string? isActive = null
            )
        {
            if (!string.IsNullOrWhiteSpace(legalName))
                await fillLegalNameAsync(legalName);
            if (!string.IsNullOrWhiteSpace(locationNumber))
                await fillLocationNumber(locationNumber);
            if (!string.IsNullOrWhiteSpace(locationName))
                await fillLocationName(locationName);
            if (!string.IsNullOrWhiteSpace(ownerShip))
                await fillOwnerShipAsync(ownerShip);
            if (!string.IsNullOrWhiteSpace(dateOpened))
            {
                var (year, day) = SplitExcelDate(dateOpened);

                await SelectDateOpenedDateFromCalendarAsync(year, day);
            }

            if (!string.IsNullOrWhiteSpace(DateClosed))
            {
                var (year, day) = SplitExcelDate(DateClosed);
                await SelectDateClosedDateFromCalendarAsync(year, day);
            }
            if (!string.IsNullOrWhiteSpace(State))
                await StateDropDown(State);

            if (!string.IsNullOrWhiteSpace(contactPhone))
                await contactPhoneFillAsync(contactPhone);
            if (!string.IsNullOrWhiteSpace(contactEmail))
                await contactEmailFillAsync(contactEmail);



            if (!string.IsNullOrWhiteSpace(ParentEntity))
                await SelectParentEntityDropdown(ParentEntity);


            if (!string.IsNullOrWhiteSpace(ManagementEntity))
                await SelectManagementEntityDropDown(ManagementEntity);

            if (!string.IsNullOrWhiteSpace(categories))
                await SelectCategoriesDropDown(categories);
            
            if (!string.IsNullOrWhiteSpace(LocationDescription))
                await SelectLocationDescriptionFillAsync(LocationDescription);
            if (!string.IsNullOrWhiteSpace(AccountingNumber))
                await AccountingNumberFillAsync(AccountingNumber);
            if (!string.IsNullOrWhiteSpace(notes))
                await NotesFillsync(notes);
            if(!string.IsNullOrEmpty(isActive))
                await IsActiveCheckBoxAsync(Convert.ToBoolean(isActive));

            await ClickCreate();
            await Task.Delay(2000);
            var screenShorts = new ScreenShorts(_page);
            await screenShorts.CaptureScreenshotAsync($"{testId}_afterCreate");
            await Task.Delay(2000);
        }


        public async Task EditLocationAsync(
           string? legalName = null,
            string? locationNumber = null,
            string? locationName = null,
            string? ownerShip = null,
            string? ParentEntity = null,
            string? dateOpened = null,
            string? AccountingNumber = null,
            string? DateClosed = null,
            string? State = null,
            string? contactPhone = null,
            string? contactEmail = null,
            string? ManagementEntity = null,
            string? categories = null,
            string? LocationDescription = null,
            string? notes = null,
           string? isActive = null
           )
        {  
            if (!string.IsNullOrWhiteSpace(legalName))
                await fillLegalNameAsync(legalName);
            if (!string.IsNullOrWhiteSpace(locationNumber))
                await fillLocationNumber(locationNumber);
            if (!string.IsNullOrWhiteSpace(locationName))
                await fillLocationName(locationName);
            if (!string.IsNullOrWhiteSpace(ownerShip))
                await EditOwnerShipAsync(ownerShip);
            if (!string.IsNullOrWhiteSpace(dateOpened))
            {
                var (year, day) = SplitExcelDate(dateOpened);
                await SelectDateOpenedDateFromCalendarAsync(year, day);
            }
            if (!string.IsNullOrWhiteSpace(DateClosed))
            {
                var (year, day) = SplitExcelDate(DateClosed);
                await SelectDateClosedDateFromCalendarAsync(year, day);
            }
            if (!string.IsNullOrWhiteSpace(State))
                await EditStateDropDown(State);

            if (!string.IsNullOrWhiteSpace(contactPhone))
                await contactPhoneFillAsync(contactPhone);

            if (!string.IsNullOrWhiteSpace(contactEmail))
                await contactEmailFillAsync(contactEmail);  

            if(  !string.IsNullOrWhiteSpace(ParentEntity))
                await SelectParentEntityDropdown(ParentEntity);

            if (!string.IsNullOrWhiteSpace(ManagementEntity))
                await EditManagementEntityDropDown(ManagementEntity);

            if (!string.IsNullOrWhiteSpace(categories))
                await EditCategoriesDropDown(categories);


            if (!string.IsNullOrWhiteSpace(LocationDescription))
                await SelectLocationDescriptionFillAsync(LocationDescription);


            if (!string.IsNullOrWhiteSpace(AccountingNumber))
                await AccountingNumberFillAsync(AccountingNumber);

            if (!string.IsNullOrWhiteSpace(notes))
                await NotesFillsync(notes);

            if(!string.IsNullOrWhiteSpace(isActive))
                await IsActiveCheckBoxAsync(Convert.ToBoolean(isActive));
           


        }
    }
}
