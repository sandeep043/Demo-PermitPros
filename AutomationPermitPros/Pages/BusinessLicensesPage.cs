using AutomationPermitPros.Pages.Base;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Pages
{
    public class BusinessLicensesPage:BaseListPage
    {
        public BusinessLicensesPage(IPage page) : base(page) { }

        private ILocator PageHeader =>
        _page.Locator("h5:has-text('Business Licenses')");


        private ILocator CreatePageHeader =>
            _page.Locator("h4:has-text('Create Business License')");

        public async Task<bool> IsListPageLoaded()
        {
            return await PageHeader.IsVisibleAsync();
        }

        public async Task<bool> IsCreatePageLoaded()
        {
            return await CreatePageHeader.IsVisibleAsync();
        }

    }
}
