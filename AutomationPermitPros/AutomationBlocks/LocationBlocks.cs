using AutomationPermitPros.Config;
using AutomationPermitPros.Pages;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.AutomationBlocks
{
    public class LocationBlocks
    {
        private readonly IPage _page;
        private readonly LocationPage _locationPage;
        public LocationBlocks(IPage page)
        {
            _page = page;
            _locationPage = new LocationPage(_page);

        }

        public async Task<string> GetToastMessageAsync()
        {
            var toast = _page.GetByRole(AriaRole.Alert);

            //Wait until toast appears
            await toast.WaitForAsync(new() { Timeout = 5000 });

            return (await toast.InnerTextAsync()).Trim();
        }

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


        public async Task<bool> Location_Block_DeleteWithReason(string deletionReason,string testId)
        {
            try
            {
                // Step 1: Click delete icon
                await Task.Delay(2000);
                var deleteIconClicked = await _locationPage.BUSLIC_Click_DeleteIcon();
             

                if (!deleteIconClicked)
                {
                    Console.WriteLine("Block Failed: Could not click delete icon");
                    return false;
                }

                await _locationPage.BUSLIC_Adv_Delete();


                // Step 2: Wait for modal to appear
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var isModalVisible = await _locationPage.location_IsDeleteModelVisible();

                if (!isModalVisible)
                {
                    Console.WriteLine("Block Failed: Delete modal did not appear");
                    return false;
                }
                await Task.Delay(2000);
                var screenShorts = new ScreenShorts(_page);
                await screenShorts.CaptureScreenshotAsync($"{testId}_afterEditModal");
                //// Step 3: Enter deletion reason
                //var reasonEntered = await _businessPage.BUSLIC_EnterDeletionReason(deletionReason);
                //if (!reasonEntered)
                //{
                //    Console.WriteLine("Block Failed: Could not enter deletion reason");
                //    return false;
                //}

                // Step 4: Confirm deletion
                var deleteConfirmed = await _locationPage.BUSLIC_ConfirmDelete();
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


        public async Task<bool> LOCATION_VerifySearchResultExists(string licenseNumber)
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



        public async Task CreateAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("-----------------Creating Location------------------------");

            await _locationPage.ClickCreateNew();
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;


            await _locationPage.CreateLocationAsync(
                testId: testId,
                legalName: data.GetValueOrDefault("Legal Name"),
                locationNumber: data.GetValueOrDefault("Location Number"),
                locationName: data.GetValueOrDefault("Location Name"),
                ownerShip: data.GetValueOrDefault("Ownership%"),
                ParentEntity: data.GetValueOrDefault("Parent Entity"),
                dateOpened: data.GetValueOrDefault("Date Opened"),
                AccountingNumber: data.GetValueOrDefault("Accounting Number"),
                DateClosed: data.GetValueOrDefault("Date Closed"),
                contactPhone: data.GetValueOrDefault("Contact Phone"),
                contactEmail: data.GetValueOrDefault("Contact Email"),
                State: data.GetValueOrDefault("State"),
                ManagementEntity: data.GetValueOrDefault("Management Entity"),
                categories: data.GetValueOrDefault("Categories"),
                LocationDescription: data.GetValueOrDefault("Location Description"),
                notes: data.GetValueOrDefault("Notes"),
                isActive: data.GetValueOrDefault("")
                );
        }


        public async Task SearchAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Search Business License");


            await _locationPage.SearchBusinessLicenseAsync(
                locationName: data.GetValueOrDefault("Search_LocationName"),
                locationNumber: data.GetValueOrDefault("Search_LocationNumber"),
                state: data.GetValueOrDefault("Search_State")
                );
        }

        public async Task EditAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Edit Location record");
            await Task.Delay(2000);
            await _locationPage.LOC_Click_EditIcon();
            await Task.Delay(2000);
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;


            await _locationPage.EditLocationAsync(
                legalName: data.GetValueOrDefault("EditLegalName"),
                locationNumber: data.GetValueOrDefault("EditLocationNumber"),
                locationName: data.GetValueOrDefault("EditLocationName"),
                ownerShip: data.GetValueOrDefault("EditOwnerShip%"),
                ParentEntity: data.GetValueOrDefault("EditParentEntity"),
                dateOpened: data.GetValueOrDefault("EditDateOpened"),
                AccountingNumber: data.GetValueOrDefault("EditAccountingNumber"),
                DateClosed: data.GetValueOrDefault("EditDateClosed"),
                contactPhone: data.GetValueOrDefault("EditContactPhone"),
                contactEmail: data.GetValueOrDefault("EditContactEmail"),
                State: data.GetValueOrDefault("EditState"),
                ManagementEntity: data.GetValueOrDefault("EditManagementEntity"),
                categories: data.GetValueOrDefault("EditCategories"),
                LocationDescription: data.GetValueOrDefault("EditLocationDescription"),
                notes: data.GetValueOrDefault("EditNotes"),
                isActive: data.GetValueOrDefault("Active")
                );

            await _locationPage.LOC_Adv_Save();
            await Task.Delay(2000);
            var screenShorts = new ScreenShorts(_page);
            await screenShorts.CaptureScreenshotAsync($"{testId}_afterEdit");
            await Task.Delay(2000);
        }


        public async Task DeleteAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Delete Location r");
            string testId = data.GetValueOrDefault("TestCaseID") ?? data.GetValueOrDefault("TestID") ?? string.Empty;

            var reason = data.GetValueOrDefault("Delete_Reason") ?? "Automation Delete";
            await Location_Block_DeleteWithReason(reason, testId);
        }


        //view

        public async Task ViewAsync()
        {
            Console.WriteLine("Block: View Business License");
            await Task.Delay(2000);
            await _locationPage.BUSLIC_Click_ViewIcon();
        }



        public async Task ReloadAsync()
        {
            await _page.ReloadAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

    }
}
