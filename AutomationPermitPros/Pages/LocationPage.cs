using AutomationPermitPros.Pages.Base;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task FillOpenedDateAsync(string dateValue)
           => await _baseListpage.FillDateFieldAsync("Date Opened", dateValue);


        public async Task ClickCreateNew()
        {
            await _baseListpage.ClickCreateNew();
        }


    }
}
