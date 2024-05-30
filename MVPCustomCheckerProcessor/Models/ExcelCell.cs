namespace MVPCustomCheckerProcessor.Models
{
    public class ExcelCell
    {
        public int ColumnIndex { get; set; }
        public string? Address { get; set; }
        public byte[]? ForegroundRGB { get; set; }
        public bool IsMergedCell { get; set; }
        public short LeftBorderColor { get; set; }
        public bool IsFirstMergedCell { get; set; }
        public string? Type { get; set; }
        public string? Text { get; set; }

        public CellStatus Status
        {
            get
            {
                return ForegroundRGB[0] switch
                {
                    0 => CellStatus.InStock,
                    255 => CellStatus.InStock,

                    51 => CellStatus.NotOffered,

                    150 => CellStatus.OutOfStock,

                    _ => CellStatus.Unknown
                };
            }
        }
        public override string ToString() => $"{Address} {Text ?? "Blank"} ({ForegroundRGB[0]},{ForegroundRGB[1]},{ForegroundRGB[2]}) {Status}";
    }

    public enum CellStatus
    {
        InStock,
        NotOffered,
        OutOfStock,
        UnlimitedStock,
        WholesaleLimit,
        Unknown
    }
}
