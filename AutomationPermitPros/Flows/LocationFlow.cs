using AutomationPermitPros.AutomationBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Flows
{
    public class LocationFlow
    {

        private readonly LocationBlocks _block;

        public LocationFlow(Microsoft.Playwright.IPage page)
        {
            _block = new LocationBlocks(page);
        }

    }
}
