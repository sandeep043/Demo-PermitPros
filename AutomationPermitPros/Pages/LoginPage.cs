using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Pages
{
    internal class LoginPage
    {
        private readonly IPage _page;


        //Locators 

        private ILocator EmailInput => _page.GetByLabel("email");
        private ILocator PasswordInput => _page.GetByLabel("password");
        private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Log in" });


        public LoginPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToLogin(string url)
        {
            await _page.GotoAsync(url);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        public async Task EnterEmail(string email)
        {

            await EmailInput.FillAsync(email);
        }
        public async Task EnterPassword(string password)
        {
            await PasswordInput.FillAsync(password);
        }


        public async Task ClickLogin()
        {
            await LoginButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }



    }
}
