using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> BUSLIC_SELECT_AGENCY()
        {
            try
            {
                await _businessPage.SelectAgencyAsync("ABC"); return true;
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


        public async Task<bool> BUSLIC_SELECT_EXPERATIONDATE_CALENDAR()
        {
            try
            {
                await _businessPage.SelectExperitionDateFromCalendarAsync("2025", "15");
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
                return await _businessPage.IsCreatePageLoaded();
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
                return await _businessPage.IsCreatePageLoaded();
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
                return await _businessPage.IsCreatePageLoaded();
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
                return await _businessPage.IsCreatePageLoaded();
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
                var deleteIconClicked = await _businessPage.BUSLIC_Click_DeleteIcon();
                if (!deleteIconClicked)
                {
                    Console.WriteLine("Block Failed: Could not click delete icon");
                    return false;
                }

                // Step 2: Wait for modal to appear
                await Task.Delay(1000); // Wait for modal animation
                var isModalVisible = await _businessPage.BUSLIC_IsDeleteModelVisible();
                if (!isModalVisible)
                {
                    Console.WriteLine("Block Failed: Delete modal did not appear");
                    return false;
                }

                // Step 3: Enter deletion reason
                var reasonEntered = await _businessPage.BUSLIC_EnterDeletionReason(deletionReason);
                if (!reasonEntered)
                {
                    Console.WriteLine("Block Failed: Could not enter deletion reason");
                    return false;
                }

                // Step 4: Confirm deletion
                var deleteConfirmed = await _businessPage.BUSLIC_ConfirmDelete();
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
                // Look for a table row containing the license number
                var row = _page.Locator($"//tr[contains(.,'{licenseNumber}')]");

                // Check if the row is visible
                bool isVisible = await row.IsVisibleAsync();

                Console.WriteLine($"License {licenseNumber} exists in search results: {isVisible}");
                return isVisible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying search result: {ex.Message}");
                return false;
            }
        }
    }
}


