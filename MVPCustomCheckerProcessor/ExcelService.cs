using MVPCustomCheckerLibrary.DAL.Entities;
using MVPCustomCheckerProcessor.Models;
using NPOI.SS.UserModel;

namespace MVPCustomCheckerProcessor
{
    public static class ExcelService
    {
        public static IEnumerable<AvailableMolds> GetAvailableCustomDiscs(IWorkbook workbook)
        {
            var worksheet = workbook.GetSheetAt(0);
            var worksheetRows = new List<List<ExcelCell>>();
            for (var i = 52; i <= worksheet.LastRowNum; i++)
            {
                IRow row;
                try
                {
                    row = worksheet.GetRow(i);
                    var rowCells = row.Cells.Where(c => c.ColumnIndex < 40).Select((c, index) => new ExcelCell
                    {
                        //Index = index,
                        ColumnIndex = c.ColumnIndex,
						Address = c.Address.ToString(),
                        ForegroundRGB = c.CellStyle.FillForegroundColorColor.RGB,
                        IsMergedCell = c.IsMergedCell,
                        LeftBorderColor = c.CellStyle.LeftBorderColor,
                        IsFirstMergedCell = c.IsMergedCell && c.CellStyle.LeftBorderColor != 0,
                        Type = c.CellType.ToString(),
                        Text = GetCellContents(c)
                    }).ToList();
                    worksheetRows.Add(rowCells);
                }
                catch (NullReferenceException)
                {
                    continue;
                }
            }

            return ProcessExcelCells(worksheetRows);
        }

        public static IEnumerable<AvailableMolds> ProcessExcelCells(IEnumerable<IEnumerable<ExcelCell>> worksheetRows)
        {
            var availableCustomDiscs = new List<AvailableMolds>();
            WeightRow? currentWeightRowLeft = null;
            WeightRow? currentWeightRowRight = null;

            foreach (var rowCells in worksheetRows)
            {
                bool justSetRowLeftCurrentWeight = false;
                bool justSetRowRightCurrentWeight = false;
                //Check if row contains weights
                if (rowCells.Any(rc => rc.Text != null && rc.Text.Contains('-') && int.TryParse(rc.Text.Split('-')[0], out _)))
                {
                    //Row contains weights
                    //Get first plastic
                    var firstHyphenColumnIndex = rowCells.FirstOrDefault(rc => rc.Text != null && rc.Text.Contains('-'))?.ColumnIndex;
                    var plasticCell = rowCells.Where(rc => rc.ColumnIndex < firstHyphenColumnIndex).LastOrDefault(rc => !string.IsNullOrWhiteSpace(rc.Text));
                    //Get weight class index starts
                    var firstTotalRowIndex = rowCells.FirstOrDefault(rc => rc.Text != null && rc.Text.Equals("Total"))?.ColumnIndex;
                    var weightClasses = rowCells.Where(rc => rc.Text != null && rc.Text.Contains('-') && rc.ColumnIndex < firstTotalRowIndex)
                            .Select(rc => new WeightClass { 
                                ColumnIndex = rc.ColumnIndex, 
                                Weight = rc.Text 
                            }).ToList();

                    if (plasticCell.ColumnIndex == 0)
                    {
                        currentWeightRowLeft = new WeightRow { 
                            Plastic = plasticCell.Text, 
                            PlasticIndex = plasticCell.ColumnIndex, 
                            WeightClasses = weightClasses 
                        };
                        justSetRowLeftCurrentWeight = true;
                    }
                    else
                    {
                        currentWeightRowRight = new WeightRow { 
                            Plastic = plasticCell.Text, 
                            PlasticIndex = plasticCell.ColumnIndex, 
                            WeightClasses = weightClasses
                        };
                        justSetRowRightCurrentWeight = true;
                    }


                    //Check if it is a double weight row
                    var remainingRowCells = rowCells.Where(rc => rc.ColumnIndex > weightClasses.Max(wc => wc.ColumnIndex));
                    if (remainingRowCells.Any(rrc => rrc.Text != null && rrc.Text.Contains('-')))
                    {
                        var nextPlasticIndex = remainingRowCells.FirstOrDefault(rrc => rrc.Text != null && !rrc.Text.Equals("Total"))?.ColumnIndex;

                        remainingRowCells = remainingRowCells.Where(rrc => rrc.ColumnIndex >= nextPlasticIndex);
                        var secondPlastic = remainingRowCells.First();
                        var secondWeightClasses = remainingRowCells.Where(rc => rc.Text != null && rc.Text.Contains('-'))
                                .Select(rc => new WeightClass {
                                    ColumnIndex = rc.ColumnIndex,
                                    Weight = rc.Text 
                                }).ToList();
                        currentWeightRowRight = new WeightRow { 
                            Plastic = secondPlastic.Text,
							PlasticIndex = secondPlastic.ColumnIndex,
							WeightClasses = secondWeightClasses
                        };
                        justSetRowRightCurrentWeight = true;
                    }
                }

                if (currentWeightRowLeft != null && !justSetRowLeftCurrentWeight)
                {
                    if (string.IsNullOrWhiteSpace(rowCells.FirstOrDefault(rc => rc.ColumnIndex == currentWeightRowLeft.PlasticIndex)?.Text))
                    {
                        currentWeightRowLeft = null;
                    }
                    else
                    {
                        var leftRowCells = rowCells.Where(rc => currentWeightRowLeft.WeightClasses.Select(wc => wc.ColumnIndex).Contains(rc.ColumnIndex) || rc.ColumnIndex == currentWeightRowLeft.PlasticIndex);
                        var availWeights = ProcessExcelCells(currentWeightRowLeft, leftRowCells);
                        availableCustomDiscs.AddRange(availWeights);
                    }
                }

                if (currentWeightRowRight != null && !justSetRowRightCurrentWeight)
                {
                    if (string.IsNullOrWhiteSpace(rowCells.FirstOrDefault(rc => rc.ColumnIndex == currentWeightRowRight.PlasticIndex)?.Text))
                    {
                        currentWeightRowRight = null;
                    }
                    else
                    {
                        var leftRowCells = rowCells.Where(rc => currentWeightRowRight.WeightClasses.Select(wc => wc.ColumnIndex).Contains(rc.ColumnIndex) || rc.ColumnIndex == currentWeightRowRight.PlasticIndex);
                        var availWeights = ProcessExcelCells(currentWeightRowRight, leftRowCells);
                        availableCustomDiscs.AddRange(availWeights);
                    }
                }
            }
            return availableCustomDiscs;
        }

        public static List<AvailableMolds> ProcessExcelCells(WeightRow currentWeightRow, IEnumerable<ExcelCell> rowCells)
        {
            var availableDiscs = new List<AvailableMolds>();

            var cells = rowCells.Where(rc => currentWeightRow.WeightClasses.Select(wc => wc.ColumnIndex).Contains(rc.ColumnIndex));
            if (!cells.Any(c => c.Status == CellStatus.InStock)) return availableDiscs;

            var mold = rowCells.First().Text!;

            foreach (var weightClass in currentWeightRow.WeightClasses)
            {
                var cell = cells.FirstOrDefault(c => c.ColumnIndex == weightClass.ColumnIndex);
                if (cell != null && cell.Status == CellStatus.InStock)
                {
                    var avail = new AvailableMolds
                    {
                        DateAvailable = DateTime.UtcNow.Date,
                        Plastic = currentWeightRow.Plastic,
                        Mold = mold,
                        Weight = weightClass.ToString(),
                    };
                    availableDiscs.Add(avail);
                }
            }
            return availableDiscs;
        }

        public static string? GetCellContents(ICell cell) =>
        cell.CellType switch
        {
            CellType.Numeric => cell.NumericCellValue.ToString(),
            CellType.String => cell.StringCellValue,
            CellType.Boolean => cell.BooleanCellValue.ToString(),
            CellType.Formula => null,
            CellType.Error => null,
            CellType.Unknown => null,
            CellType.Blank => null,
            _ => null
        };
    }
}
