using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace AutomationPermitPros.Utilities
{
    public static class ExcelResultWriter
    {
        public static void WriteResult(
            string filePath,
            string sheetName,
            int rowNumber,
            string status,
            string errorMessage = "")
        {
            using var workbook = new XLWorkbook(filePath);
            var sheet = workbook.Worksheet(sheetName);

            sheet.Cell(rowNumber, "AG").Value = status;        // ExecutionStatus
            sheet.Cell(rowNumber, "AI").Value = errorMessage;  // ErrorMessage

            workbook.Save();
        }
    }
}