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
            // Try to find an accessible link with the given name
            var menuItem = _page.GetByRole(AriaRole.Link, new() { Name = menuName });

            if (await menuItem.CountAsync() == 0)
            {
                // Fallback to a button role
                menuItem = _page.GetByRole(AriaRole.Button, new() { Name = menuName });
            }

            if (await menuItem.CountAsync() == 0)
            {
                // Final fallback: match by visible text anywhere in the sidebar
                menuItem = _page.GetByText(menuName);
            }

            var target = menuItem.First;
            await target.WaitForAsync(new() { Timeout = 10000 });
            await target.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
