namespace DiscordLibrary.Models
{
    internal class GroupedAvailableCustomDisc
    {
        public string Plastic { get; set; }
        public IEnumerable<GroupedAvailableCustomDiscByMold> Molds { get; set; }
    }
}
