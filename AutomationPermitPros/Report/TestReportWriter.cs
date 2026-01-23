using ClosedXML.Excel;
using System;
using System.IO;
using System.Threading;

namespace AutomationPermitPros.Report
{
    internal static class TestReportWriter
    {
        private static readonly object _fileLock = new();
        private static string? _reportPath;

        /// <summary>
        /// Lazily initialized shared report path for all flows/tests.
        /// Call Initialize(...) to override default path before any AppendResult calls.
        /// </summary>
        public static string ReportPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_reportPath))
                    Initialize(null);
                return _reportPath!;
            }
            private set => _reportPath = value;
        }

        /// <summary>
        /// Initialize shared report file path. If <paramref name="reportPath"/> is null or empty
        /// a timestamped file will be created under {AppContext.BaseDirectory}/Reports.
        /// Safe to call multiple times; first non-empty value wins.
        /// </summary>
        public static void Initialize(string? reportPath)
        {
            if (!string.IsNullOrWhiteSpace(_reportPath))
                return; // already initialized

            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                var dir = Path.GetDirectoryName(reportPath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                ReportPath = reportPath!;
                return;
            }

            var reportsDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            ReportPath = Path.Combine(reportsDir, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        /// <summary>
        /// Append a single row to the shared report Excel. If file doesn't exist it will be created with headers.
        /// Columns: TestID, Operation, Status, Msg if Status is Incomplete, SS Before, SS After
        /// This method is resilient to transient file locks (retries).
        /// </summary>
        public static void AppendResult(
            string testId,
            string operation,
            string status,
            string message = "",
            string ssBefore = "",
            string ssAfter = "",
            string? overrideReportPath = null)
        {
            if (string.IsNullOrWhiteSpace(testId))
                testId = string.Empty;
            if (string.IsNullOrWhiteSpace(operation))
                operation = string.Empty;
            if (string.IsNullOrWhiteSpace(status))
                status = string.Empty;

            var targetPath = overrideReportPath ?? ReportPath;

            const int maxAttempts = 5;
            int attempt = 0;
            Exception? lastEx = null;

            // Ensure directory exists
            var dir = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            while (attempt++ < maxAttempts)
            {
                try
                {
                    lock (_fileLock)
                    {
                        if (!File.Exists(targetPath))
                        {
                            using var wb = new XLWorkbook();
                            var ws = wb.AddWorksheet("TestReport");
                            ws.Cell(1, 1).Value = "TestID";
                            ws.Cell(1, 2).Value = "Operation";
                            ws.Cell(1, 3).Value = "Status";
                            ws.Cell(1, 4).Value = "Msg if Status is Incomplete";
                            ws.Cell(1, 5).Value = "SS Before";
                            ws.Cell(1, 6).Value = "SS After";

                            ws.Row(1).Style.Font.SetBold(true);
                            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#dff0d8");
                            wb.SaveAs(targetPath);
                        }

                        using var wb2 = new XLWorkbook(targetPath);
                        var ws2 = wb2.Worksheet("TestReport") ?? wb2.AddWorksheet("TestReport");

                        // find first empty row
                        var lastRow = ws2.LastRowUsed();
                        int nextRow = lastRow == null ? 2 : lastRow.RowNumber() + 1;

                        ws2.Cell(nextRow, 1).Value = testId;
                        ws2.Cell(nextRow, 2).Value = operation;
                        ws2.Cell(nextRow, 3).Value = status;

                        // store message and screenshots (message typically only meaningful for InComplete)
                        ws2.Cell(nextRow, 4).Value = message ?? string.Empty;
                        ws2.Cell(nextRow, 5).Value = ssBefore ?? string.Empty;
                        ws2.Cell(nextRow, 6).Value = ssAfter ?? string.Empty;

                        ws2.Columns().AdjustToContents();
                        wb2.SaveAs(targetPath);
                    }

                    // success
                    return;
                }
                catch (IOException ioEx)
                {
                    lastEx = ioEx;
                    // file locked - small backoff
                    Thread.Sleep(200 * attempt);
                    continue;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    // other error - no retry
                    break;
                }
            }

            throw new Exception($"Unable to write report row after {maxAttempts} attempts", lastEx);
        }
    }
}