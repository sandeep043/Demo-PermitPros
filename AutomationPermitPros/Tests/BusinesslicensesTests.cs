using AutomationPermitPros.AutomationBlocks;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Tests
{
    [TestFixture]
    internal class BusinesslicensesTests
    {
            private IPlaywright _playwright;
            private IBrowser _browser;
            
        

            [SetUp]
            public async Task Setup()
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = false });
            }

            [TearDown]
            public async Task Teardown()
            {
                if (_browser != null)
                    await _browser.CloseAsync();
                _playwright?.Dispose();
            }

            [Test]
            public async Task LoginTest()
            {  
                var context = await _browser.NewContextAsync();
                var page = await context.NewPageAsync();
                var loginBlock = new LoginBlock(page);
                var Result = await loginBlock.LOGIN();

            }

        }
    }
