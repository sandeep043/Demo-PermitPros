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
        public async Task ReloadAsync()
        {
            await _page.ReloadAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

    }
}
