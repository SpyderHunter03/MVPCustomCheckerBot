using Microsoft.EntityFrameworkCore;
using MVPCustomCheckerLibrary.DAL;
using MVPCustomCheckerLibrary.DAL.Entities;
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
                var settings = await context.Settings.ToListAsync() ?? 
                    throw new NullReferenceException(nameof(context));

                var lastProcessedDateString = settings.FirstOrDefault(s =>
                    s.Name.Equals("NextRun", StringComparison.InvariantCultureIgnoreCase));
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

                //Save the last processing date
                if (lastProcessedDateString is null)
                {
                    lastProcessedDateString = new Settings
                    {
                        Name = "LastRead",
                        Setting = DateTime.UtcNow.ToString()
                    };
                    await context.Settings.AddAsync(lastProcessedDateString);
                }
                else
                {
                    lastProcessedDateString.Setting = DateTime.UtcNow.ToString();
                }

                //Get the next iteration time
                var nextIteration = settings.FirstOrDefault(s =>
                    s.Name.Equals("NextRunInterval", StringComparison.InvariantCultureIgnoreCase));
                if (nextIteration is null)
                {
                    nextIteration = new Settings
                    {
                        Name = "NextRunInterval",
                        Setting = TimeSpan.FromHours(3).ToString()
                    };
                    await context.Settings.AddAsync(nextIteration);
                }

                var nextRun = settings.FirstOrDefault(s =>
                    s.Name.Equals("NextRun", StringComparison.InvariantCultureIgnoreCase))
                    ?? new Settings
                    {
                        Name = "NextRun"
                    }; ;

                nextRun.Setting = DateTime.UtcNow.Add(TimeSpan.Parse(nextIteration.Setting)).ToString();

                await context.SaveChangesAsync();
                Console.WriteLine($"Saved Settings.");

                // Save the file only after confirming it has updates.
                SaveWorkbookToFile(workbook);
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

        private static void SaveWorkbookToFile(IWorkbook workbook)
        {
            var localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"MVP-Custom_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");

            // Assuming 'filePath' contains the full path to the file, including the directory and file name
            string directoryPath = Path.GetDirectoryName(localFilePath);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath, 
                UnixFileMode.UserWrite | 
                UnixFileMode.UserRead | 
                UnixFileMode.UserExecute | 
                UnixFileMode.GroupRead | 
                UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead);

            // Now, safely create and write to the file
            using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fileStream);

            Console.WriteLine($"File saved at {localFilePath}");
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
