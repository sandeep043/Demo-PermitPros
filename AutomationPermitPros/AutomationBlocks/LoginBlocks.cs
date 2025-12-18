using AutomationPermitPros.Config;
using AutomationPermitPros.Pages;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.AutomationBlocks
{
    internal class LoginBlock
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly LoginPage _loginPage;
        public LoginBlock(IPage page)
        {
            _page = page;
            _config=TestConfiguration.Instance;
            _loginPage = new LoginPage(page);
        }

        public async Task <bool> LOGIN()
        {
            try
            {
                await _loginPage.NavigateToLogin(_config.AppSettings.BaseUrl);
                await _loginPage.EnterEmail(_config.Credentials.Username);
                await _loginPage.EnterPassword(_config.Credentials.Password); 
                await _loginPage.ClickLogin();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOGIN block Failed:{ex.Message}");
                return false;
            }

        }


    }
}
