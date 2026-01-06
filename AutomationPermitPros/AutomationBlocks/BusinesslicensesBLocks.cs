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

        public async Task<bool> BUSLIC_EDIT_ICON()
        {
            try
            {
                await _businessPage.BUSLIC_Click_EditIcon();
                return await _businessPage.IsCreatePageLoaded();
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
                return await _businessPage.IsCreatePageLoaded();
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
                return await _businessPage.IsCreatePageLoaded();
            }
            catch
            {
                return false;
            }
        }


    }
}

