namespace MVPCustomCheckerLibrary.DAL.Entities
{
    public class AvailableMolds
    {
        public int Id { get; set; }
        public string Plastic { get; set; }
        public string Mold { get; set; }
        public string Weight { get; set; }
        public DateTime DateAvailable { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is AvailableMolds am)
            {
                if (Plastic.Equals(am.Plastic) && Weight.Equals(am.Weight) && Mold.Equals(am.Mold))
                    return true;
            }

            return false;
        }

        public override string ToString() => $"{Plastic} {Mold} ({Weight})";
	}
}
