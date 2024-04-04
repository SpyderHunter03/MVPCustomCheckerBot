namespace DiscordLibrary.Models
{
    internal class AvailableCustomDisc
    {
        public string Plastic { get; set; }
        public string Weight { get; set; }
        public string Mold { get; set; }

        public override string ToString() => $"{Plastic} {Mold} {Weight}";

        public override bool Equals(object obj)
        {
            if (obj is AvailableCustomDisc acd)
            {
                if (Plastic.Equals(acd.Plastic) && Weight.Equals(acd.Weight) && Mold.Equals(acd.Mold))
                    return true;
            }

            return false;
        }
    }
}
