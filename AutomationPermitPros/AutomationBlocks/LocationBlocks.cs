using AutomationPermitPros.Pages;
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
        private readonly LocationPage locationPage;
        public LocationBlocks(IPage page)
        {
            _page = page;
            locationPage = new LocationPage(_page);

        }
    }
}
