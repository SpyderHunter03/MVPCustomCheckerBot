using Microsoft.EntityFrameworkCore;
using MVPCustomCheckerLibrary.DAL;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MVPCustomCheckerProcessor
{
    public static class FileProcessor
    {
        private static readonly string ExcelUrl = "https://mvpdiscsports.com/documents/MVP-Custom.xls";
        private static readonly string LocalStoragePath = "/opt/MVPCustomCheckerProcessor/excelfiles";

        public static async Task ProcessFile(MVPCustomCheckerContext context)
        {
            try
            {
                var lastProcessedDateString = await context.Settings.FirstOrDefaultAsync(s => s.Name.Equals("LastRead", StringComparison.InvariantCultureIgnoreCase));
                var lastProcessedDate = lastProcessedDateString != null ?
                        DateTime.Parse(lastProcessedDateString.Setting).Date :
                        DateTime.MinValue.Date;

                var workbook = await GetWorkbook();

                Console.WriteLine($"Got workbook.");

                if (!ExcelUpdatedAfterLastReadDate(workbook, lastProcessedDate))
                {
                    Console.WriteLine($"No updates to file since last time ran: {lastProcessedDate}");
                    return;
                }

                Console.WriteLine($"Commencing file processing.");
                var availableCustomDiscs = ExcelService.GetAvailableCustomDiscs(workbook);

                context.AvailableMolds.AddRange(availableCustomDiscs);
                Console.WriteLine($"Saved available custom discs.");

                lastProcessedDateString.Setting = DateTime.UtcNow.ToString();
                Console.WriteLine($"Saved Last Read Setting.");
                context.SaveChangesAsync();

                // Save the file only after confirming it has updates.
                string localFilePath = Path.Combine(LocalStoragePath, $"MVP-Custom_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
                SaveWorkbookToFile(workbook, localFilePath);
                Console.WriteLine($"File saved at {localFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown: {ex.Message}");
                throw;
            }
        }

        public static async Task<IWorkbook> GetWorkbook()
        {
            using var httpClient = new HttpClient();
            var downloadResponse = await httpClient.GetAsync(ExcelUrl);

            if (!downloadResponse.IsSuccessStatusCode)
                throw new ApplicationException("Unable to download file.");

            using var stream = await downloadResponse.Content.ReadAsStreamAsync();
            IWorkbook workbook = ExcelUrl.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ?
                new HSSFWorkbook(stream) :
                new XSSFWorkbook(stream);

            return workbook;
        }

        private static void SaveWorkbookToFile(IWorkbook workbook, string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);
        }

        public static bool ExcelUpdatedAfterLastReadDate(IWorkbook workbook, DateTime lastReadDate)
        {
            var worksheet = workbook.GetSheetAt(0);
            var dateCellValue = worksheet.GetRow(0).GetCell(38).DateCellValue;

            Console.WriteLine($"Cell Date: {dateCellValue.Date}");
            Console.WriteLine($"LastRead Date: {lastReadDate.Date}");

            return dateCellValue.Date >= lastReadDate.Date;
        }
    }
}
