using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutomationPermitPros.Pages;
using Microsoft.Playwright;

namespace AutomationPermitPros.AutomationBlocks
{
    public class BusinesslicensesBLocks
    {
        private readonly IPage _page;
        private readonly BusinessLicensesPage _businessPage;

        public BusinesslicensesBLocks(IPage page)
        {
            _page = page;
            _businessPage = new BusinessLicensesPage(page);
        }

        public async Task<bool> BUSLIC_ENTER_LOCATIONNUMBER(string locationNumber)
        {
            try
            {
                await _businessPage.FillLocationNumberAsync(locationNumber);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_ENTER_LOCATIONNUMBER] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BUSLIC_SELECT_AGENCY(string Select_agency)
        {
            try
            {
                await _businessPage.SelectAgencyAsync(Select_agency); return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BUSLIC SELECT AGENCY ERROR ", ex.Message);
                return false;
            }
        }

        public async Task<bool> BUSLIC_ENTER_LOCATIONNAME(string locationName)
        {
            try
            {
                await _businessPage.FillLocationNameAsync(locationName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_ENTER_LOCATIONNAME] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BUSLIC_ENTER_LICENSENUMBER(string licenseNumber)
        {
            try
            {
                await _businessPage.FillLicenseNumberAsync(licenseNumber);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_ENTER_LICENSENUMBER] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BUSLIC_SELECT_LOCATION(string location)
        {
            try
            {
                await _businessPage.SelectLocationAsync(location);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_LOCATION] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BUSLIC_SELECT_RENEWALDATE_CALENDAR(
          string year,
          string day)
        {
            try
            {
                await _businessPage.SelectRenewalDateFromCalendarAsync(year, day);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_RENEWALDATE_CALENDAR] {ex.Message}");
                return false;
            }
        }


        public async Task<bool> BUSLIC_SELECT_EXPERATIONDATE_CALENDAR(string year, string day)
        {
            try
            {
                await _businessPage.SelectExperitionDateFromCalendarAsync(year, day);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_EXPERATIONDATE_CALENDAR] {ex.Message}");
                return false;
            }
        }



        public async Task<bool> BUSLIC_SELECT_LICENSETYPE(string licenseType)
        {
            try
            {
                await _businessPage.SelectLicenseTypeAsync(licenseType);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_LICENSETYPE] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        public async Task<bool> BUSLIC_EditSELECT_LICENSETYPE(string licenseType)
        {
            try
            {
                await _businessPage.EditSelectLocationAsync(licenseType);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_LICENSETYPE] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BUSLIC_SELECT_STATE(string state)
        {
            try
            {
                await _businessPage.SelectStateAsync(state);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BUSLIC_SELECT_STATE] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return false;
            }
        }

        public async Task<bool> BUSLIC_CREATE_NEW()
        {
            try
            {
                await _businessPage.ClickCreateNew();
                return await _businessPage.IsCreatePageLoaded();
            }
            catch
            {
                return false;
            }
        }
        //Search Button
        public async Task<bool> BUSLIC_SEARCHBUTTON()
        {
            try
            {
                await _businessPage.ClickSearch();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //Export to Excel
        public async Task<bool> EXPORT_EXCEL()
        {
            try
            {
                var filePath = await _businessPage.ExportToExcel();
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_CREATE_BTN()
        {
            try
            {
                await _businessPage.ClickCreateButtonAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_DELETE_ICON()
        {
            try
            {
                await _businessPage.BUSLIC_Click_DeleteIcon();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_EDIT_ICON()
        {
            try
            {
                await _businessPage.BUSLIC_Click_EditIcon();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_VIEW_ICON()
        {
            try
            {
                await _businessPage.BUSLIC_Click_ViewIcon();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_ADV_SAVE_BUTTON()
        {
            try
            {
                await _businessPage.BUSLIC_Adv_Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BUSLIC_ADV_DeleteButton()
        {
            try
            {
                await _businessPage.BUSLIC_Adv_Delete();
                return true;
            }
            catch
            {
                return false;
            }


        }

        public async Task<bool> BUSLIC_Block_DeleteWithReason(string deletionReason)
        {
            try
            {


                // Step 1: Click delete icon
                await Task.Delay(2000);
                var deleteIconClicked = await _businessPage.BUSLIC_Click_DeleteIcon();


                await Task.Delay(2000);


                if (!deleteIconClicked)
                {
                    Console.WriteLine("Block Failed: Could not click delete icon");
                    return false;
                }

                await _businessPage.BUSLIC_Adv_Delete();


                // Step 2: Wait for modal to appear
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var isModalVisible = await _businessPage.BUSLIC_IsDeleteModelVisible();
                if (!isModalVisible)
                {
                    Console.WriteLine("Block Failed: Delete modal did not appear");
                    return false;
                }

                //// Step 3: Enter deletion reason
                //var reasonEntered = await _businessPage.BUSLIC_EnterDeletionReason(deletionReason);
                //if (!reasonEntered)
                //{
                //    Console.WriteLine("Block Failed: Could not enter deletion reason");
                //    return false;
                //}

                // Step 4: Confirm deletion
                var deleteConfirmed = await _businessPage.BUSLIC_ConfirmDelete();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                if (!deleteConfirmed)
                {
                    Console.WriteLine("Block Failed: Could not confirm deletion");
                    return false;
                }

                // Step 5: Wait for deletion to complete
                await Task.Delay(2000);

                Console.WriteLine("Block Success: License deleted with reason");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Block Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> BUSLIC_VerifySearchResultExists(string licenseNumber)
        {
            try
            {
                // Wait for table or results area to load
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Target only table BODY rows
                var rows = _page.Locator("table tbody tr");

                int rowCount = await rows.CountAsync();
                if (rowCount == 0)
                {
                    Console.WriteLine("No rows found in search results");
                    return false;
                }

                // Check each row for the license number
                for (int i = 0; i < rowCount; i++)
                {
                    string rowText = await rows.Nth(i).InnerTextAsync();

                    if (rowText.Contains(licenseNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"License {licenseNumber} exists in search results");
                        return true;
                    }
                }

                Console.WriteLine($"License {licenseNumber} NOT found in search results");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying search result: {ex.Message}");
                return false;
            }
        }

        // New: try-get toast message with configurable timeout.
        // Returns (found, message). Does NOT throw on timeout — caller can decide how to handle.
        public async Task<(bool Found, string Message)> TryGetToastMessageAsync(int timeoutMs = 2000)
        {
            try
            {
                var toast = _page.GetByRole(AriaRole.Alert);
                await toast.WaitForAsync(new() { Timeout = timeoutMs });
                var text = (await toast.InnerTextAsync())?.Trim() ?? string.Empty;
                return (true, text);
            }
            catch (PlaywrightException)
            {
                // includes timeout and other Playwright-specific errors
                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TryGetToastMessageAsync error: {ex.Message}");
                return (false, string.Empty);
            }
        }

        // Existing synchronous GetToastMessageAsync remains for callers that expect the original behavior.
        //public async Task<string> GetToastMessageAsync()
        //{
        //    var toast = _page.GetByRole(AriaRole.Alert);

        //    //Wait until toast appears
        //    await toast.WaitForAsync(new() { Timeout = 5000 });

        //    return (await toast.InnerTextAsync()).Trim();
        //}

        //Business License Action Methods-------------------

        public async Task FillRenewalDateFromExcelAsync(Dictionary<string, string> data)
        {
            if (!data.TryGetValue("RenewalYear", out var year) ||
                !data.TryGetValue("RenewalDay", out var day) ||
                string.IsNullOrWhiteSpace(year) ||
                string.IsNullOrWhiteSpace(day))
            {
                Console.WriteLine("Renewal Date skipped (no Excel data)");
                return;
            }

            await _businessPage.SelectRenewalDateFromCalendarAsync(year, day);
        }

        public async Task FillExpirationDateFromExcelAsync(Dictionary<string, string> data)
        {
            if (!data.TryGetValue("ExpirationYear", out var year) ||
                !data.TryGetValue("ExpirationDay", out var day) ||
                string.IsNullOrWhiteSpace(year) ||
                string.IsNullOrWhiteSpace(day))
            {
                Console.WriteLine("Expiration Date skipped (no Excel data)");
                return;
            }

            await _businessPage.SelectExperitionDateFromCalendarAsync(year, day);
        }

        public async Task SearchAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Search Business License");

            //if (data.TryGetValue("Search_LocationName", out var location) &&
            //    !string.IsNullOrWhiteSpace(location))
            //{
            //    await _businessPage.FillLocationNameAsync(location);
            //}

            //if (data.TryGetValue("Search_LicenseNumber", out var licenseNumber) &&
            //    !string.IsNullOrWhiteSpace(licenseNumber))
            //{
            //    await _businessPage.FillLicenseNumberAsync(licenseNumber);
            //}

            //if (data.TryGetValue("Search_LicenseType", out var licenseType) && !string.IsNullOrWhiteSpace(licenseNumber))


            //    await _businessPage.ClickSearch();
            await _businessPage.SearchBusinessLicenseAsync(
                locationName: data.GetValueOrDefault("Search_LocationName"),
                licenseNumber: data.GetValueOrDefault("Search_LicenseNumber"),
                licenseType: data.GetValueOrDefault("Search_LicenseType"),
                locationNumber: data.GetValueOrDefault("Search_LocationNumber"),
                state: data.GetValueOrDefault("Search_State")
                );
        }

        public async Task CreateAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Create Business License");

            // Go to create page
            await _businessPage.ClickCreateNew();
            await _businessPage.IsCreatePageLoaded();
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;

            // Fill form (Excel-driven)
            await _businessPage.CreateBusinessLicenseAsync(
                testId: testId,
                location: data.GetValueOrDefault("Location"),
                agency: data.GetValueOrDefault("Agency"),
                licenseNumber: data.GetValueOrDefault("LicenseNumber"),
                licenseType: data.GetValueOrDefault("LicenseType"),
                expirationDate: data.GetValueOrDefault("ExpirationDate"),
                renewalDate: data.GetValueOrDefault("RenewalDate"),
                description: data.GetValueOrDefault("Description"),
                notes: data.GetValueOrDefault("Notes"),
                licenseReceivedDate: data.GetValueOrDefault("LicenseReceivedDate"),
                 dateIssued: data.GetValueOrDefault("DateIssued"),
                 effectiveDate: data.GetValueOrDefault("EffectiveDate"),
                 applicationRenewalSentDate: data.GetValueOrDefault("ApplicationRenewalSentDate"),
                 renewalAppReceivedDate: data.GetValueOrDefault("RenewalAppReceivedDate"),
                 escrowStatusId: data.GetValueOrDefault("EscrowStatusID"),
                 prevEscrowStatusId: data.GetValueOrDefault("PrevEscrowStatusID"),
                 previousEscrowStatusDate: data.GetValueOrDefault("PreviousEscrowStatusDate")
            );
        }


        public async Task EditAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Edit Business License");
            await Task.Delay(2000);

            await _businessPage.BUSLIC_Click_EditIcon();
            await Task.Delay(2000);

            await _businessPage.EditBusinessLicenseAsync(
                location: data.GetValueOrDefault("EditLocation"),
                agency: data.GetValueOrDefault("EditAgency"),
                licenseNumber: data.GetValueOrDefault("EditLicenseNumber"),
                licenseType: data.GetValueOrDefault("EditLicenseType"),
                expirationDate: data.GetValueOrDefault("EditExpirationDate"),
                renewalDate: data.GetValueOrDefault("EditRenewalDate"),
                description: data.GetValueOrDefault("EditDescription"),
                notes: data.GetValueOrDefault("EditNotes"),
                licenseReceivedDate: data.GetValueOrDefault("EditLicenseReceivedDate"),
                 dateIssued: data.GetValueOrDefault("EditDateIssued"),
                 effectiveDate: data.GetValueOrDefault("EditEffectiveDate"),
                 applicationRenewalSentDate: data.GetValueOrDefault("EditApplicationRenewalSentDate"),
                 renewalAppReceivedDate: data.GetValueOrDefault("EditRenewalAppReceivedDate"),
                 escrowStatusId: data.GetValueOrDefault("EditEscrowStatusID"),
                 prevEscrowStatusId: data.GetValueOrDefault("EditPrevEscrowStatusID"),
                 previousEscrowStatusDate: data.GetValueOrDefault("EditPreviousEscrowStatusDate"));

            await _businessPage.BUSLIC_Adv_Save();
        }


        public async Task DeleteAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Delete Business License");


            var reason = data.GetValueOrDefault("Delete_Reason") ?? "Automation Delete";
            await BUSLIC_Block_DeleteWithReason(reason);
        }

        public async Task ViewAsync()
        {
            Console.WriteLine("Block: View Business License");
            await Task.Delay(2000);
            await _businessPage.BUSLIC_Click_ViewIcon();
        }

        //validate Create Success Message

        public async Task<string> GetToastMessageAsync()
        {
            var toast = _page.GetByRole(AriaRole.Alert);

            //Wait until toast appears
            await toast.WaitForAsync(new() { Timeout = 5000 });

            return (await toast.InnerTextAsync()).Trim();
        }


        public async Task ReloadAsync()
        {
            await _page.ReloadAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}

















