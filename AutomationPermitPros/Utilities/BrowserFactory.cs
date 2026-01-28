using AutomationPermitPros.Config;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace AutomationPermitPros.Utilities
{
    internal static class BrowserFactory
    {
        public static async Task<IBrowser> LaunchAsync(IPlaywright playwright, AppSettings settings)
        {
            if (playwright == null) throw new ArgumentNullException(nameof(playwright));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

          
            var browserType = (Environment.GetEnvironmentVariable("BROWSER")?.Trim()
                               ?? settings.BrowserType ?? "chromium")
                              .Trim().ToLowerInvariant();

            var options = new BrowserTypeLaunchOptions
            {
                Headless = settings.Headless,
                SlowMo = settings.SlowMo,
            };

                    
            if (browserType == "firefox")
            {
                return await playwright.Firefox.LaunchAsync(options);
            }

            if (browserType == "webkit")
            {
                return await playwright.Webkit.LaunchAsync(options);
            }

            if (browserType == "edge" || browserType == "msedge")
            {
                options.Channel = "msedge";
                return await playwright.Chromium.LaunchAsync(options);
            }

            if (browserType == "chrome")
            {
                options.Channel = "chrome";
                return await playwright.Chromium.LaunchAsync(options);
            }

            return await playwright.Chromium.LaunchAsync(options);
        }
    }
}
