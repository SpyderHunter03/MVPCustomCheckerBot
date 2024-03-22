namespace MVPCustomCheckerProcessor.Models
{
    public class WeightRow
    {
        public string Plastic { get; set; }
        public int PlasticIndex { get; set; }
        public IEnumerable<WeightClass> WeightClasses { get; set; }

        public override string ToString() => $"{Plastic} ({string.Join(',', WeightClasses.Select(wc => wc.ToString()))})";
    }
}
