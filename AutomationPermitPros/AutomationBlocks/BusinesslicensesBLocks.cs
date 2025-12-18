using AutomationPermitPros.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.AutomationBlocks
{
    public class BusinesslicensesBLocks
    {
        private readonly BusinessLicensesPage _page;

        public BusinesslicensesBLocks(BusinessLicensesPage page)
        {
            _page = page;
        }

        //Create New Button
        public async Task<bool> BUSLIC_CREATE_NEW()
        {
            try
            {
                await _page.ClickCreateNew();
                return await _page.IsCreatePageLoaded();
            }
            catch
            {
                return false;
            }
        }
        //Search Button
        public async Task<bool> BUSLIC_SearchButton()
        {
            try
            {
                await _page.ClickSearch();
                return await _page.IsCreatePageLoaded();
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
                var filePath = await _page.ExportToExcel();
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }



    }
}

