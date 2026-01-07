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


        protected ILocator Model_DeleteIconButton =>
            _page.Locator("button[aria-label='Delete']");



        protected ILocator Adv_DeleteButton =>
            _page.GetByRole(AriaRole.Button, new() { Name = "Delete" });

        protected ILocator Adv_SaveButton =>
            _page.GetByRole(AriaRole.Button, new () { Name = "Save", Exact = true });
        protected ILocator CancelButton => _page.GetByRole(AriaRole.Button, new() { Name = "Cancel" });

        protected ILocator EditIconButton =>
            _page.Locator("button[aria-label='Edit']"); 

        protected ILocator ViewIconButton =>
            _page.Locator("button[aria-label='View Details']");
        protected ILocator DeleteReasonTextarea => _page.GetByRole(AriaRole.Textbox);

        protected ILocator DeleteModalMessage => _page.GetByText("There is no associated information");

        private ILocator DeleteConfirmButton => _page.GetByRole(AriaRole.Button, new() { Name = "Delete" });
        private ILocator DeleteModalTitle => _page.GetByText("Delete Business License");


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
            // 🔑 Locate the REAL MUI input
            var input = _page.Locator("input[placeholder='MM/DD/YYYY']").First;

            await input.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            // Focus
            await input.ClickAsync();

            // Clear mask
            await input.PressAsync("Control+A");
            await input.PressAsync("Backspace");

            // Type slowly (MUI requires this)
            await input.TypeAsync(dateValue, new LocatorTypeOptions
            {
                Delay = 100
            });

            // Commit value
            await input.PressAsync("Enter");

            // Force blur so React commits state
            await _page.Mouse.ClickAsync(5, 5);

            // ✅ VERIFY value actually changed
            var value = await input.InputValueAsync();
            if (value != dateValue)
                throw new Exception($"Date not set correctly. Expected: {dateValue}, Actual: {value}");
        }

        public async Task SelectMuiDateFromCalendarAsync(
        int calendarIndex,
        string year,
        string day)
        {
            // 1️⃣ Open calendar
            await _page
                .GetByRole(AriaRole.Button, new() { Name = "Choose date" })
                .Nth(calendarIndex)
                .ClickAsync();

            await _page.WaitForTimeoutAsync(500); // Small wait for animation

            // 2️⃣ Click header to open year selection
            await _page
                .GetByRole(AriaRole.Button, new() { Name = "calendar view is open, switch" })
                .ClickAsync();

            await _page.WaitForTimeoutAsync(500); // Small wait for year view

            // 3️⃣ Select year (it's a radio button, not gridcell)
            var yearCell = _page.GetByRole(
                AriaRole.Radio,
                new() { Name = year, Exact = true }
            );
            await yearCell.ScrollIntoViewIfNeededAsync();
            await yearCell.ClickAsync();

            await _page.WaitForTimeoutAsync(500); // Small wait after year selection

            // 4️⃣ Select day
            var dayCell = _page.GetByRole(
                AriaRole.Gridcell,
                new() { Name = day, Exact = true }
            );
            await dayCell.ClickAsync();

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
                await Model_DeleteIconButton.ClickAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public async Task<bool> Click_EditIcon()
        {

            try
            {
                await EditIconButton.ClickAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public async Task<bool> Click_ViewIcon()
        {

            try
            {
                await ViewIconButton.ClickAsync();
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

                await Adv_DeleteButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task EnterDeletionReason(string reason)
        //{
        //    await DeleteReasonTextarea.FillAsync(reason); 
        //}


        public async Task<bool> IsDeleteModalVisible()
        {
            try
            {
                await DeleteModalTitle.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                return await DeleteModalTitle.IsVisibleAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnterDeletionReason(string reason)
        {
            try
            {
                await DeleteReasonTextarea.FillAsync(reason);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error entering deletion reason: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> Adv_Save()
        {
            try
            {

                await Adv_SaveButton.ClickAsync();
                //await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Click confirm delete button in modal...
        public async Task<bool> ConfirmDelete()
        {
            try
            {
                await DeleteConfirmButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming delete: {ex.Message}");
                return false;
            }
        }

        // Click cancel button in modal
        public async Task<bool> CancelDelete()
        {
            try
            {
                await CancelButton.ClickAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling delete: {ex.Message}");
                return false;
            }
        }








    }
}

