 using AutomationPermitPros.Pages;
using Microsoft.Playwright;


namespace AutomationPermitPros.AutomationBlocks
{

    public class SidebarNavigationBlock
    {
        private readonly SidebarPage _sidebarPage;

        public SidebarNavigationBlock(IPage page)
        {
            _sidebarPage = new SidebarPage(page);
        }


        public async Task<bool> NavigateToAsync(string menuName)
        {
            try
            {
                await _sidebarPage.ClickMenuAsync(menuName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigateToAsync] Error navigating to '{menuName}': {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
