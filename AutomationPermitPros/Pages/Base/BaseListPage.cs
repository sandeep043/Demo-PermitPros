using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Pages.Base
{
    public abstract class BaseListPage
    {
        protected readonly IPage _page;

        protected BaseListPage(IPage page) { 
            _page = page; 

        }
        protected ILocator CreateNewButton =>
            _page.GetByRole(AriaRole.Button, new() { Name= "Create New" }); 
        
            protected ILocator SearchButton =>
                _page.GetByRole(AriaRole.Button, new() { Name = "Search" });

        protected ILocator ExportToExcelButton =>
            _page.GetByRole(AriaRole.Button,new() { Name = "Export to Excel" });

        public async Task ClickCreateNew()
        {
            await CreateNewButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task ClickSearch()
        {
            await SearchButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }


        public async Task<string> ExportToExcel()
        {
            var download = await _page.RunAndWaitForDownloadAsync(async () =>
            {
                await ExportToExcelButton.ClickAsync();
            });

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Downloads",
                $"Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            );

            await download.SaveAsAsync(filePath);
            return filePath;
        }
    }
}

