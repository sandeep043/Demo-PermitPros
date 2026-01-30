using Microsoft.Extensions.Configuration;
using AutomationPermitPros.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutomationPermitPros.Config
{
    internal class TestConfiguration
    {
        private static TestConfiguration _instance;
        private readonly IConfiguration _configuration;

        public AppSettings AppSettings { get; }
        public Credentials Credentials { get; }

        private TestConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Bind configuration sections to classes (appsettings.json will now only provide non-sensitive defaults)
            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
            Credentials = _configuration.GetSection("Credentials").Get<Credentials>() ?? new Credentials();

            // Environment variables may still override, but Excel (EnvUsers) is required and authoritative.
            // Attempt to load required runtime values from the TestData Excel (sheet: "EnvUsers").
            // If the sheet, RUN row, or required columns are missing, throw and stop execution.
            TryLoadEnvironmentFromExcel();
        }

        public static TestConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestConfiguration();
                return _instance;
            }
        }

        // Helper method to get any configuration value
        public string GetValue(string key)
        {
            return _configuration[key];
        }

        private void TryLoadEnvironmentFromExcel()
        {
            var excelPath = TestDataConfig.TestDataExcel;
            const string sheetName = "EnvUsers";

            List<Dictionary<string, string>> rows;
            try
            {
                // This will throw if the sheet does not exist or file missing; we want to fail fast.
                rows = ExcelDataProvider.GetData(excelPath, sheetName).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Required sheet '{sheetName}' not found or unreadable in the test data Excel ('{excelPath}'). Aborting test run. Inner: {ex.Message}", ex);
            }

            // Find first RUN row
            var firstRunRow = rows.FirstOrDefault(r => ExcelHelper.IsTrue(r, "RUN") || ExcelHelper.IsTrue(r, "Run"));
            if (firstRunRow == null)
                throw new InvalidOperationException($"No row with RUN = TRUE found in sheet '{sheetName}'. Add a row with RUN=TRUE to provide URL, credentials and browser type.");

            // Required columns: URL, Username (or UserEmail), Password, Browser Type
            if (!TryGetValueCaseInsensitive(firstRunRow, "URL", out var url) || string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException($"EnvUsers RUN row must contain a non-empty 'URL' value.");

            if (!TryGetValueCaseInsensitive(firstRunRow, "UserEmail", out var user)  || string.IsNullOrWhiteSpace(user))
            {
                throw new InvalidOperationException($"EnvUsers RUN row must contain a non-empty 'UserEmail' or 'Username' value.");
            }

            if (!TryGetValueCaseInsensitive(firstRunRow, "Password", out var pwd) || string.IsNullOrWhiteSpace(pwd))
                throw new InvalidOperationException($"EnvUsers RUN row must contain a non-empty 'Password' value.");

            if (!TryGetValueCaseInsensitive(firstRunRow, "Browser Type", out var browser) &&
                !TryGetValueCaseInsensitive(firstRunRow, "BrowserType", out browser) &&
                !TryGetValueCaseInsensitive(firstRunRow, "Browser", out browser) || string.IsNullOrWhiteSpace(browser))
            {
                throw new InvalidOperationException($"EnvUsers RUN row must contain a non-empty 'Browser Type' (e.g. 'msedge' or 'chrome').");
            }

            // Assign values (Excel is authoritative)
            AppSettings.BaseUrl = url.Trim();
            Credentials.Username = user.Trim();
            Credentials.Password = pwd.Trim();
            AppSettings.BrowserType = browser.Trim();

            // Optional overrides if present
            //if (TryGetValueCaseInsensitive(firstRunRow, "Headless", out var headlessVal) &&
            //    bool.TryParse(headlessVal?.Trim(), out var h))
            //{
            //    AppSettings.Headless = h;
            //}

            //if (TryGetValueCaseInsensitive(firstRunRow, "Timeout", out var timeoutVal) &&
            //    int.TryParse(timeoutVal?.Trim(), out var t))
            //{
            //    AppSettings.Timeout = t;
            //}

            // Successful load — log small summary
            Console.WriteLine($"TestConfiguration: loaded runtime settings from Excel sheet '{sheetName}' (URL={AppSettings.BaseUrl}, User={Credentials.Username}, Browser={AppSettings.BrowserType})");
        }

        private static bool TryGetValueCaseInsensitive(Dictionary<string, string> row, string key, out string? value)
        {
            if (row.TryGetValue(key, out var val))
            {
                value = val;
                return true;
            }

            var match = row.Keys.FirstOrDefault(k => string.Equals(k?.Trim(), key, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                value = row[match];
                return true;
            }

            var compactKey = key.Replace(" ", "", StringComparison.OrdinalIgnoreCase);
            match = row.Keys.FirstOrDefault(k => string.Equals(k?.Replace(" ", "", StringComparison.OrdinalIgnoreCase), compactKey, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                value = row[match];
                return true;
            }

            value = null;
            return false;
        }
    }
}



