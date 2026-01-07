using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Config
{
    internal class ScreenShorts
    {

        private readonly IPage _page;
        public string ScreenshotPath { get; set; }
        public ScreenShorts(IPage page, string? screenshotPath = null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            // default to a Screenshots folder under the current working directory when none provided
            ScreenshotPath = string.IsNullOrWhiteSpace(screenshotPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "Screenshots")
                : screenshotPath!;

            try
            {
                Directory.CreateDirectory(ScreenshotPath);
            }
            catch
            {
                // ignore directory creation failures; will surface when attempting to save
            }
        }


        public async Task CaptureScreenshotAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ScreenshotPath))
                {
                    ScreenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                    Directory.CreateDirectory(ScreenshotPath);
                }

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fullFileName = $"{fileName}_{timestamp}.png";
                var filePath = Path.Combine(ScreenshotPath, fullFileName);
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = filePath,
                    FullPage = true
                });
                Console.WriteLine($"Screenshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screenshot: {ex.Message}");
            }
        }


    }
}
