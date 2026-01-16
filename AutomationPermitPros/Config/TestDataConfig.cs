using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Config
{
    public static class TestDataConfig
    {
        private static string TestDataRoot =>
            Path.Combine(AppContext.BaseDirectory, "TestData");

        private const string DefaultSheet = "BusinessLicense_TestData";

        // Business Licenses
        public static string BusinessLicensesExcel =>
            GetExcelPath("BusinessLicenses.xlsx");
        public static string BusinessLicensesSheet => DefaultSheet;

        // Locations
        public static string LocationsExcel =>
            GetExcelPath("Locations.xlsx");
        public static string LocationsSheet => DefaultSheet;

        private static string GetExcelPath(string fileName)
        {
            var path = Path.Combine(TestDataRoot, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Excel not found: {path}");

            return path;
        }


    }
}
