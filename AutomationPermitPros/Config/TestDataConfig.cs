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

        private const string ExcelFileName = "TestData.xlsx";
        public static string TestDataExcel => GetExcelPath();

        public static string ControllerSheet => "TestNames";


        public const string BusinessLicensesSheet = "BusinessLicense_TestData";
        public const string LocationsSheet = "Locations";
        public const string StaffLicensesSheet = "StaffLicenses";

        private static string GetExcelPath()
        {
            var path = Path.Combine(TestDataRoot, ExcelFileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Test data Excel not found: {path}");

            return path;
        }

    }
}
