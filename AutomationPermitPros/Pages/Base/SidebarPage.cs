using Microsoft.Playwright;

namespace AutomationPermitPros.Pages
{
    /// <summary>
    /// Page Object for Left Sidebar navigation
    /// Handles ONLY UI interactions
    /// </summary>
    public class SidebarPage
    {
        private readonly IPage _page;

        public SidebarPage(IPage page)
        {
            //dumi
            _page = page;
        }

        /// <summary>
        /// Clicks sidebar menu item using Playwright get-by helpers (role or text)
        /// </summary>
        public async Task ClickMenuAsync(string menuName)
        {
            // Ensure sidebar exists
            var sidebar = _page.Locator("nav, .sidebar, [data-testid=\"sidebar\"]");
            if (await sidebar.CountAsync() == 0)
            {
                // allow global fallback (some layouts do not use <nav>)
                sidebar = _page.Locator("body");
            }

            // Try role link/button inside sidebar first
            var item = sidebar.GetByRole(AriaRole.Link, new() { Name = menuName });
            if (await item.CountAsync() == 0)
                item = sidebar.GetByRole(AriaRole.Button, new() { Name = menuName });
            if (await item.CountAsync() == 0)
                item = sidebar.GetByText(menuName);

            if (await item.CountAsync() == 0)
                throw new InvalidOperationException($"Sidebar menu item '{menuName}' not found on the current page.");

            var target = item.First;
            await target.WaitForAsync(new() { Timeout = 20_000 });
            await target.ScrollIntoViewIfNeededAsync();
            await target.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

    }
    }
