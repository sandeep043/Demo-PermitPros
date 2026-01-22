using AutomationPermitPros.Pages.Base;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public async Task  fillOwnerShipAsync(string ownerShip)
        {
            await _page.GetByPlaceholder("Ownership %").FillAsync(ownerShip);
        } 

        public async Task StateDropDown(string state)
        {
            // Click the third "Select state" div (nth starts at 0)
            await _page.GetByText("Select state").ClickAsync();

            await _page.Keyboard.TypeAsync(state);
            await Task.Delay(200);

            // Select the exact matching item
            await _page.GetByText(state, new() { Exact = true }).ClickAsync();
        }

        public async Task SelectManagementEntityDropDown(string managementEntity)
        {
            await _page.GetByText("Select Management Entity").ClickAsync();
            //will see how to select value from dropdown
            await _page.GetByText(managementEntity, new() { Exact = true }).ClickAsync();
        } 

        public async Task SelectCategoriesDropDown(string categories)
        {
            // Click the second categories textbox (nth starts at 0)
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "categories" }).Nth(1).ClickAsync();
         
            await _page.GetByText(categories, new() { Exact = true }).ClickAsync();
        }

        public async Task SelectParentEntityDropdown(string parentEntity)
        {
            await _page.GetByText("Select parent Corporation").ClickAsync();
            //will see how to select value from dropdown
            await _page.GetByText(parentEntity, new() { Exact = true }).ClickAsync();
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


        public  async Task NotesFillsync(string notes)
        {
            await _page.Locator("textarea").FillAsync(notes);   
        }  

        public async Task IsActiveCheckBoxAsync(bool isActive)
        {
           var isChecked= await _page.Locator("#autoSizingCheck2").IsCheckedAsync();
            if (isActive != isChecked)
            {
                await  _page.Locator("#autoSizingCheck2").ClickAsync();
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
        public async Task CreateLocationAsync(
            string ? legalName = null,
            string ? locationNumber = null,
            string ? locationName = null,
            string ? ownerShip = null,
            string? ParentEntity=null,
            string ? dateOpened = null,
            string? AccountingNumber = null,
            string? DateClosed = null,
            string? State=null,
            string? contactPhone=null,
            string? contactEmail= null,
            string? ManagementEntity= null,
            string? categories= null,
            string? LocationDescription = null,
            string? notes= null ,
            string? locationDescription = null,
            bool? isActive = false
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

            if(!string.IsNullOrWhiteSpace(DateClosed))
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
            if (isActive.HasValue)
                await IsActiveCheckBoxAsync(isActive.Value);


            await ClickCreate();
        }
    }
}
