namespace DiscordLibrary.Models
{
    internal class DiscordWebhookMessage
    {
        public string Content { get; set; }
        public IEnumerable<DiscordEmbed> Embeds { get; set; }
    }
}
