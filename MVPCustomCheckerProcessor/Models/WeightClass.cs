namespace MVPCustomCheckerProcessor.Models
{
    public class WeightClass
    {
        public int ColumnIndex { get; set; }
        public string Weight { get; set; }

        public string BottomWeight
        {
            get
            {
                var firstWeight = Weight.Split('-')[0];
                return firstWeight.StartsWith('1') ? firstWeight : $"1{firstWeight}";
            }
        }

        public string TopWeight
        {
            get
            {
                var firstWeight = Weight.Split('-')[1];
                return firstWeight.StartsWith('1') ? firstWeight : $"1{firstWeight}";
            }
        }

        public override string ToString() => $"{BottomWeight}-{TopWeight}";
    }
}
