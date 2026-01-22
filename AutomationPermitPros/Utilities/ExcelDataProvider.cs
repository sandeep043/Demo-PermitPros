using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Utilities
{
    public static class ExcelDataProvider
    {
        public static IEnumerable<Dictionary<string, string>> GetData(
            string filePath,
            string sheetName)
        {
            using var workbook = new XLWorkbook(filePath);
            var sheet = workbook.Worksheet(sheetName);

            var headers = sheet.Row(1).Cells()
                .Select(c => c.GetString()).ToList();

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var data = new Dictionary<string, string>();

                for (int i = 0; i < headers.Count; i++)
                    data[headers[i]] = row.Cell(i + 1).GetString();

                yield return data;
            }
        }

    }
}
