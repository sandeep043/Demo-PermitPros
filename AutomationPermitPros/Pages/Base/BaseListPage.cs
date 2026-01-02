using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Pages.Base
{
    public class BaseListPage
    {
        protected readonly IPage _page;

        public BaseListPage(IPage page)
        {
            _page = page;

        }
        protected ILocator CreateNewButton =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Create New" });

        protected ILocator SearchButton =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Search" });

        protected ILocator ExportToExcelButton =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Export to Excel" });


        protected ILocator DeleteIconButton =>
            _page.Locator("button[aria-label='Delete']");



        protected ILocator confirmButton =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Delete" });





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


        private async Task<ILocator> FindInputByLabelAsync(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("label must be provided", nameof(label));


            var byLabel = _page.GetByLabel(label);
            if (await byLabel.CountAsync() > 0)
                return byLabel;

            var labelLocator = _page.Locator($"xpath=//label[contains(normalize-space(.), \"{label}\")] ");
            if (await labelLocator.CountAsync() > 0)
            {
                var input = labelLocator.Locator("following::input[1]");
                if (await input.CountAsync() > 0)
                    return input;

                input = labelLocator.Locator("../input");
                if (await input.CountAsync() > 0)
                    return input;
            }


            var textElem = _page.Locator($"text=\"{label}\"");
            if (await textElem.CountAsync() > 0)
            {
                var nearbyInput = textElem.Locator("following::input[1]");
                if (await nearbyInput.CountAsync() > 0)
                    return nearbyInput;
            }

            throw new Exception($"Input for label '{label}' not found on the page.");
        }


        public async Task FillDateFieldAsync(string label, string dateValue)
        {
            var input = await FindInputByLabelAsync(label);
            await input.FillAsync(dateValue);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }


        public async Task<bool> SetDateByLabelAsync(string label, string dateValue)
        {
            try
            {
                await FillDateFieldAsync(label, dateValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SortByLabelAsync(string columnLabel, bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(columnLabel))
                throw new ArgumentException("columnLabel must be provided", nameof(columnLabel));


            var header = _page.Locator($"th:has-text(\"{columnLabel}\")");
            if (await header.CountAsync() == 0)
            {

                header = _page.Locator($":text(" + "\"" + columnLabel + "\"" + ")");
                if (await header.CountAsync() == 0)
                    throw new Exception($"Column header '{columnLabel}' not found on the page.");
            }

            var desired = ascending ? "ascending" : "descending";


            async Task<string?> ReadAriaSortAsync()
            {
                var v = await header.GetAttributeAsync("aria-sort");
                return v;
            }

            var current = await ReadAriaSortAsync();
            if (string.Equals(current, desired, StringComparison.OrdinalIgnoreCase))
                return true;


            ILocator sortControl = header.Locator("button");
            if (await sortControl.CountAsync() == 0)
                sortControl = header.Locator("svg");

            if (await sortControl.CountAsync() == 0)
                sortControl = header; // click the header as a last resort

            for (int i = 0; i < 4; i++)
            {
                await sortControl.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                current = await ReadAriaSortAsync();
                if (string.Equals(current, desired, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Final best-effort: try clicking any icon that explicitly indicates asc/desc if present
            var iconSelector = ascending ? "svg[aria-label='sort-asc'], .sort-asc" : "svg[aria-label='sort-desc'], .sort-desc";
            var icon = header.Locator(iconSelector);
            if (await icon.CountAsync() > 0)
            {
                await icon.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }

            return false;
        }



        public async Task<bool> Click_DeleteIcon()
        {

            try
            {
                await DeleteIconButton.ClickAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> Adv_Delete()
        {
            try
            {

                await confirmButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    }

