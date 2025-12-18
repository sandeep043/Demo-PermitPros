using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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

            // Bind configuration sections to classes

            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

            Credentials = _configuration.GetSection("Credentials").Get<Credentials>();

            // Override with environment variables if present

            AppSettings.BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? AppSettings.BaseUrl;

            Credentials.Username = Environment.GetEnvironmentVariable("TEST_USERNAME") ?? Credentials.Username;

            Credentials.Password = Environment.GetEnvironmentVariable("TEST_PASSWORD") ?? Credentials.Password;

        }

        public static TestConfiguration Instance

        {

            get

            {

                if (_instance == null)

                {

                    _instance = new TestConfiguration();

                }

                return _instance;

            }

        }

        // Helper method to get any configuration value

        public string GetValue(string key)

        {

            return _configuration[key];

        }

    }

}



