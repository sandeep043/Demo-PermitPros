using AutomationPermitPros.Pages;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
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


        public async Task<bool> Location_Block_DeleteWithReason(string deletionReason)
        {
            try
            {
                // Step 1: Click delete icon
                await Task.Delay(2000);
                var deleteIconClicked = await _locationPage.BUSLIC_Click_DeleteIcon();


                await Task.Delay(2000);


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
            await _locationPage.CreateLocationAsync(
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
                isActive: ExcelHelper.IsTrue(data, "Active")
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


        public async Task DeleteAsync(Dictionary<string, string> data)
        {
            Console.WriteLine("Block: Delete Location r");


            var reason = data.GetValueOrDefault("Delete_Reason") ?? "Automation Delete";
            await Location_Block_DeleteWithReason(reason);
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
