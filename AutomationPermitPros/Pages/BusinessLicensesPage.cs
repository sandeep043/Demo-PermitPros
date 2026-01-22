using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Pages.Base;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationPermitPros.Pages
{
    public class BusinessLicensesPage
    {

        private readonly IPage _page;
        private readonly BaseListPage _baseListPage;

        public BusinessLicensesPage(IPage page)
        {
            _page = page;
            _baseListPage = new BaseListPage(page);
        }

        private ILocator PageHeader => _page.GetByRole(AriaRole.Heading, new() { Name = "Business Licenses" });

        private ILocator CreatePageHeader => _page.GetByRole(AriaRole.Heading, new() { Name = "Create Business License" });

        // List page inputs
        private ILocator LocationNumberInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Number" });
        private ILocator LocationNameInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Name" });
        private ILocator LicenseNumberInput => _page.GetByPlaceholder("Enter License Number");

        // License Type controls
        private ILocator LicenseTypeDropdown => _page.GetByRole(AriaRole.Combobox, new() { Name = "License Type" });
        // react-select style comboboxes share aria-label 'Default select example' in the DOM; use Nth() to pick the correct one
        private ILocator LocationSelectInput => _page.GetByRole(AriaRole.Combobox, new() { Name = "Default select example" }).Nth(0);
        private ILocator AgencySelectInput => _page.GetByRole(AriaRole.Combobox, new() { Name = "Default select example" }).Nth(1);
        private ILocator LicenseTypeSelectInput => _page.GetByRole(AriaRole.Combobox, new() { Name = "Default select example" }).Nth(2);

        private ILocator EditLicenseTypeSelectInput => _page.Locator("#react-select-4-input");

        // State <select> as combobox
        private ILocator StateDropdown => _page.GetByRole(AriaRole.Combobox, new() { Name = "State" });

        // Other form controls
        private ILocator DescriptionInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Description" });
        private ILocator NotesTextArea => _page.GetByLabel("Notes");
        private ILocator CreateButton => _page.GetByRole(AriaRole.Button, new() { Name = "Create" });
        private ILocator BackToListButton => _page.GetByRole(AriaRole.Button, new() { Name = "Back To List" });
        private ILocator FileInput => _page.GetByLabel("Upload Document");
        private ILocator LicenseTypeCodeInput => _page.GetByPlaceholder("License Type Code");

        private ILocator EscrowStatusIdInput => _page.GetByPlaceholder("Enter Escrow Status ID");
        private ILocator PrevEscrowStatusIdInput => _page.GetByPlaceholder("Enter Prev Escrow Status ID");

      

        public async Task<bool> IsListPageLoaded()
        {
            return await PageHeader.IsVisibleAsync();
        }

        public async Task<bool> IsCreatePageLoaded()
        {
            return await CreatePageHeader.IsVisibleAsync();
        }

        // Async helpers (list page)
        public async Task FillLocationNumberAsync(string value)
        {
            await LocationNumberInput.FillAsync(value);
        }

        public async Task FillLocationNameAsync(string value)
        {
            await LocationNameInput.FillAsync(value);
        }

        public async Task FillLicenseNumberAsync(string value)
        {
            await LicenseNumberInput.FillAsync(value);
        }

        public async Task SelectLicenseTypeAsync(string optionLabel)
        {
            var select = LicenseTypeSelectInput;

            if (await select.CountAsync() == 0)
                throw new Exception("License type combobox not found.");

            // 1️⃣ Open dropdown
            await select.ClickAsync();

            await select.WaitForAsync(new() { Timeout = 2000 });

            // 2️⃣ Type filter text
            await select.FillAsync(optionLabel);

            // 3️⃣ Wait for option to appear (CRITICAL)
            var option = _page.GetByRole(
                AriaRole.Option,
                new() { Name = optionLabel }
            );

            await option.WaitForAsync(new() { Timeout = 2000 });

            // 4️⃣ Click the option explicitly
            await option.ClickAsync();

            // 5️⃣ Verify selection actually happened
            //var selectedValue = await select.InputValueAsync();
            //if (!selectedValue.Contains(optionLabel, StringComparison.OrdinalIgnoreCase))
            //    throw new Exception($"License Type '{optionLabel}' was not selected.");
        }


        public async Task SelectStateAsync(string stateLabel)
        {
            await StateDropdown.SelectOptionAsync(new SelectOptionValue { Label = stateLabel });
        }

        // Create-form specific methods
        public async Task SelectLocationAsync(string locationLabel)
        {
            var input = LocationSelectInput;

            if (await input.CountAsync() == 0)
                input = _page.GetByRole(AriaRole.Combobox).First;

            if (await input.CountAsync() == 0)
                throw new Exception("Location select input not found.");

            // 1️⃣ Open dropdown
            await input.ClickAsync();

            // 2️⃣ Type filter text
            await input.FillAsync(locationLabel);

            // 3️⃣ WAIT for at least one option to appear (CRITICAL)
            var firstOption = _page.GetByRole(AriaRole.Option).First;

            await firstOption.WaitForAsync(new() { Timeout = 5000 });

            // 4️⃣ Click first option explicitly
            await firstOption.ClickAsync();

            // 5️⃣ Verify selection actually happened
            //var selectedValue = await input.InputValueAsync();
            //if (string.IsNullOrWhiteSpace(selectedValue))
            //    throw new Exception("Location was not selected.");
        }

        public async Task EditSelectLocationAsync(string locationLabel)
        {
            var input = EditLicenseTypeSelectInput;


            // 1️⃣ Open dropdown
            await input.ClickAsync();

            // 2️⃣ Type filter text
            await input.FillAsync(locationLabel);

            // 3️⃣ WAIT for at least one option to appear (CRITICAL)
            var firstOption = _page.GetByRole(AriaRole.Option).First;

            await firstOption.WaitForAsync(new() { Timeout = 5000 });

            // 4️⃣ Click first option explicitly
            await firstOption.ClickAsync();

            // 5️⃣ Verify selection actually happened
            //var selectedValue = await input.InputValueAsync();
            //if (string.IsNullOrWhiteSpace(selectedValue))
            //    throw new Exception("Location was not selected.");
        }

        public async Task SelectAgencyAsync(string agencyLabel)
        {
            var input = AgencySelectInput;
            if (await input.CountAsync() == 0)
            {
                input = _page.GetByRole(AriaRole.Combobox).Nth(1);
            }

            if (await input.CountAsync() == 0)
                throw new Exception("Agency select input not found on create page.");

            await input.ClickAsync();
            await input.FillAsync(agencyLabel);
            await input.PressAsync("Enter");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task FillDescriptionAsync(string description)
        {
            var desc = DescriptionInput;
            if (await desc.CountAsync() > 0)
            {
                await desc.FillAsync(description);
                return;
            }

            var fallback = _page.GetByPlaceholder("Description");
            if (await fallback.CountAsync() > 0)
            {
                await fallback.First.FillAsync(description);
                return;
            }

            throw new Exception("Description input not found on create page.");
        }

        public async Task FillNotesAsync(string notes)
        {
            var ta = NotesTextArea;
            if (await ta.CountAsync() > 0)
            {
                await ta.FillAsync(notes);
                return;
            }

            var fallback = _page.GetByRole(AriaRole.Textbox).Last;
            if (await fallback.CountAsync() > 0)
            {
                await fallback.FillAsync(notes);
                return;
            }

            throw new Exception("Notes textarea not found on create page.");
        }

        public async Task UploadDocumentAsync(string filePath)
        {
            var file = FileInput;
            if (await file.CountAsync() > 0)
            {
                await file.SetInputFilesAsync(filePath);
                return;
            }

            // fallback to any input[type=file] accessed via GetByPlaceholder or role is not available; try GetByLabel for 'Choose File'
            var choose = _page.GetByRole(AriaRole.Button, new() { Name = "Choose File" });
            if (await choose.CountAsync() > 0)
            {
                var input = choose.Locator("xpath=following::input[@type='file'][1]");
                if (await input.CountAsync() > 0)
                {
                    await input.SetInputFilesAsync(filePath);
                    return;
                }
            }

            var anyFile = _page.Locator("input[type='file']");
            if (await anyFile.CountAsync() > 0)
            {
                await anyFile.First.SetInputFilesAsync(filePath);
                return;
            }

            throw new Exception("File input not found on create page.");
        }

        public async Task<string?> GetLicenseTypeCodeAsync()
        {
            var code = LicenseTypeCodeInput;
            if (await code.CountAsync() > 0)
                return await code.First.GetAttributeAsync("value");

            return null;
        }

        // Escrow status fields
        public async Task FillEscrowStatusIdAsync(string value)
        {
            var input = EscrowStatusIdInput;
            if (await input.CountAsync() > 0)
            {
                await input.FillAsync(value);
                return;
            }

            var fallback = _page.GetByPlaceholder("Enter Escrow Status ID");
            if (await fallback.CountAsync() > 0)
            {
                await fallback.First.FillAsync(value);
                return;
            }

            throw new Exception("Escrow Status ID input not found on create page.");
        }

        public async Task FillPrevEscrowStatusIdAsync(string value)
        {
            var input = PrevEscrowStatusIdInput;
            if (await input.CountAsync() > 0)
            {
                await input.FillAsync(value);
                return;
            }

            var fallback = _page.GetByPlaceholder("Enter Prev Escrow Status ID");
            if (await fallback.CountAsync() > 0)
            {
                await fallback.First.FillAsync(value);
                return;
            }

            throw new Exception("Prev Escrow Status ID input not found on create page.");
        }

        // Date fields - delegate to BaseListPage's single reusable date helper
        public async Task FillLicenseReceivedDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("License Received Date", dateValue);

        public async Task FillRenewalDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Renewal Date", dateValue);

        public async Task SelectRenewalDateFromCalendarAsync(
      string year,
      string day)
        {
            await _baseListPage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 1,
                year: year,
                day: day
            );
        }


        public async Task SelectExperitionDateFromCalendarAsync(
      string year,
      string day)
        {
            await _baseListPage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 2,
                year: year,
                day: day
            );
        }

        public async Task SelectDateIssuedFromCalendaryAsync(
     string year,string day)
        {
            await _baseListPage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 3,
                year: year,
                day: day
            );
        }

        public async Task SelectEffectiveDateFromCalendarAsync(
            string year, string day)
        {
            await _baseListPage.SelectMuiDateFromCalendarAsync(
                calendarIndex: 4,
                year: year,
                day: day
            );
        }



        private (string Year, string Day) SplitExcelDate(string excelDate)
        {
            var date = DateTime.ParseExact(
                excelDate,
                "MM/dd/yyyy",
                CultureInfo.InvariantCulture);

            return (date.Year.ToString(), date.Day.ToString());
        }




        public async Task FillExpirationDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Expiration Date", dateValue);

        public async Task FillDateIssuedAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Date Issued", dateValue);

        public async Task FillEffectiveDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Effective Date", dateValue);

        public async Task FillRenewalAppReceivedDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Renewal App Received Date", dateValue);

        public async Task FillApplicationRenewalSentDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Application Renewal Sent Date", dateValue);

        public async Task FillPreviousEscrowStatusDateAsync(string dateValue)
            => await _baseListPage.FillDateFieldAsync("Previous Escrow Status Date", dateValue);

        // New: fill entire create form (only fills values provided - skip null/empty)
        public async Task CreateBusinessLicenseAsync(
            string? location = null,
            string? licenseReceivedDate = null,
            string? agency = null,
            string? licenseNumber = null,
            string? renewalDate = null,
            string? description = null,
            string? licenseType = null,
            string? expirationDate = null,
            string? dateIssued = null,
            string? effectiveDate = null,
            string? renewalAppReceivedDate = null,
            string? applicationRenewalSentDate = null,
            string? escrowStatusId = null,
            string? prevEscrowStatusId = null,
            string? previousEscrowStatusDate = null,
            string? uploadFilePath = null,
            string? notes = null)
        {
            if (!string.IsNullOrWhiteSpace(location))
                await SelectLocationAsync(location);

            if (!string.IsNullOrWhiteSpace(licenseReceivedDate))
                await FillLicenseReceivedDateAsync(licenseReceivedDate);

            if (!string.IsNullOrWhiteSpace(agency))
                await SelectAgencyAsync(agency);

            if (!string.IsNullOrWhiteSpace(licenseNumber))
                await FillLicenseNumberAsync(licenseNumber);

            if (!string.IsNullOrWhiteSpace(renewalDate))
            {
                var (year, day) = SplitExcelDate(renewalDate);

                await SelectRenewalDateFromCalendarAsync(year, day);
            }

            if (!string.IsNullOrWhiteSpace(description))
                await FillDescriptionAsync(description);

            if (!string.IsNullOrWhiteSpace(licenseType))
                await SelectLicenseTypeAsync(licenseType);

            if (!string.IsNullOrWhiteSpace(expirationDate))
            {
                var (year, day) = SplitExcelDate(expirationDate);
                await SelectExperitionDateFromCalendarAsync(year, day);
            }
                

            if (!string.IsNullOrWhiteSpace(dateIssued))
            {
                var (year, day) = SplitExcelDate(dateIssued);
                await SelectDateIssuedFromCalendaryAsync(year, day);
            }


            if (!string.IsNullOrWhiteSpace(effectiveDate))
            { 
                var (year, day) = SplitExcelDate(effectiveDate);
                await FillEffectiveDateAsync(effectiveDate);
            }
            if (!string.IsNullOrWhiteSpace(renewalAppReceivedDate))
                await FillRenewalAppReceivedDateAsync(renewalAppReceivedDate);

            if (!string.IsNullOrWhiteSpace(applicationRenewalSentDate))
                await FillApplicationRenewalSentDateAsync(applicationRenewalSentDate);

            if (!string.IsNullOrWhiteSpace(escrowStatusId))
                await FillEscrowStatusIdAsync(escrowStatusId);

            if (!string.IsNullOrWhiteSpace(prevEscrowStatusId))
                await FillPrevEscrowStatusIdAsync(prevEscrowStatusId);

            if (!string.IsNullOrWhiteSpace(previousEscrowStatusDate))
                await FillPreviousEscrowStatusDateAsync(previousEscrowStatusDate);

            if (!string.IsNullOrWhiteSpace(uploadFilePath))
                await UploadDocumentAsync(uploadFilePath);

            if (!string.IsNullOrWhiteSpace(notes))
                await FillNotesAsync(notes);

            // finally click create
            await ClickCreateButtonAsync();
        }
        public async Task SearchBusinessLicenseAsync(
    string? locationNumber = null,
    string? locationName = null,
    string? licenseNumber = null,
    string? licenseType = null,
    string? state = null)
        {
            if (!string.IsNullOrWhiteSpace(locationNumber))
                await FillLocationNumberAsync(locationNumber);
            if (!string.IsNullOrWhiteSpace(locationName))
                await FillLocationNameAsync(locationName);
            if (!string.IsNullOrWhiteSpace(licenseNumber))
                await FillLicenseNumberAsync(licenseNumber);
            if (!string.IsNullOrWhiteSpace(licenseType))
                await SelectLicenseTypeAsync(licenseType);
            if (!string.IsNullOrWhiteSpace(state))
                await SelectStateAsync(state);
            await ClickSearch();
        }



  public async Task EditBusinessLicenseAsync(
   string? location = null,
    string? licenseReceivedDate = null,
    string? agency = null,
    string? licenseNumber = null,
    string? renewalDate = null,
    string? description = null,
    string? licenseType = null,
    string? expirationDate = null,
    string? dateIssued = null,
    string? effectiveDate = null,
    string? renewalAppReceivedDate = null,
    string? applicationRenewalSentDate = null,
    string? escrowStatusId = null,
    string? prevEscrowStatusId = null,
    string? previousEscrowStatusDate = null,

    string? notes = null)
        {
            if (!string.IsNullOrWhiteSpace(location))
                await EditSelectLocationAsync(location);
            if (!string.IsNullOrWhiteSpace(licenseType))
                await SelectLicenseTypeAsync(licenseType);
            if (!string.IsNullOrWhiteSpace(expirationDate))
                await FillExpirationDateAsync(expirationDate);
            if (!string.IsNullOrWhiteSpace(agency))
                await SelectAgencyAsync(agency);
            if (!string.IsNullOrWhiteSpace(licenseNumber))
                await FillLicenseNumberAsync(licenseNumber);
            if (!string.IsNullOrWhiteSpace(description))
                await FillDescriptionAsync(description);
            if (!string.IsNullOrWhiteSpace(escrowStatusId))
                await FillEscrowStatusIdAsync(escrowStatusId);
            if (!string.IsNullOrWhiteSpace(prevEscrowStatusId))
                await FillPrevEscrowStatusIdAsync(prevEscrowStatusId);
            if (!string.IsNullOrWhiteSpace(notes))
                await FillNotesAsync(notes);
            if (!string.IsNullOrWhiteSpace(licenseReceivedDate))
                await FillLicenseReceivedDateAsync(licenseReceivedDate);
            if (!string.IsNullOrWhiteSpace(renewalDate))
                await FillRenewalDateAsync(renewalDate);
            if (!string.IsNullOrWhiteSpace(dateIssued))
                await FillDateIssuedAsync(dateIssued);


        }

        // Submit/back
        public async Task ClickCreateButtonAsync()
        {
            if (await CreateButton.CountAsync() == 0)
                throw new Exception("Create button not found on the page.");

            await CreateButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task ClickBackToListAsync()
        {
            if (await BackToListButton.CountAsync() == 0)
                throw new Exception("Back To List button not found on the page.");

            await BackToListButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Forward base list page behaviors via composition
        public async Task ClickCreateNew()
        {
            await _baseListPage.ClickCreateNew();
        }

        public async Task ClickSearch()
        {
            await _baseListPage.ClickSearch();
        }

        public async Task<bool> BUSLIC_Click_DeleteIcon()
        {
            return await _baseListPage.Click_DeleteIcon();
        }

        public async Task<bool> BUSLIC_Adv_Delete()
        {
            return await _baseListPage.Adv_Delete();
        }
        public async Task<bool> BUSLIC_Adv_Save()
        {
            return await _baseListPage.Adv_Save();
        }

        public async Task<bool> BUSLIC_Click_EditIcon()
        {
            return await _baseListPage.Click_EditIcon();
        }

        public async Task<bool> BUSLIC_Click_ViewIcon()
        {
            return await _baseListPage.Click_ViewIcon();
        }

        public async Task<string> ExportToExcel()
        {
            return await _baseListPage.ExportToExcel();
        }

        // Expose generic sort from BaseListPage for this page
        public async Task<bool> SortColumnAsync(string columnLabel, bool ascending = true)
        {
            return await _baseListPage.SortByLabelAsync(columnLabel, ascending);
        }

        public async Task<bool> BUSLIC_IsDeleteModelVisible()
        {
            return await _baseListPage.IsDeleteModalVisible();
        }

        public async Task<bool> BUSLIC_EnterDeletionReason(string reason)
        {
            return await _baseListPage.EnterDeletionReason(reason);
        }

        public async Task<bool> BUSLIC_ConfirmDelete()
        {
                  return await _baseListPage.ConfirmDelete();
        }


        public async Task<bool> BUSLIC_CancelDelete()
        {
            return await _baseListPage.CancelDelete();
        }


        //Success toast message:
        public async Task<string> GetToastMessageAsync()
        {
            var toast = _page.GetByText("Created Successfully");
            await toast.WaitForAsync(new() { Timeout = 5000 });
            return (await toast.InnerTextAsync()).Trim();
        }

        public async Task<bool> IsToastVisibleAsync()
        {
            return await _page.GetByRole(AriaRole.Alert).IsVisibleAsync();
        }
    }
}
