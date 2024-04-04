using DiscordLibrary.Models;
using MVPCustomCheckerLibrary.DAL.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace DiscordLibrary
{
    public static class DiscordWebhook
    {
        public static async Task<int> SendMessageToWebhook(
            IEnumerable<string> webhooks,
            IEnumerable<AvailableMolds> totalAvailableMolds,
            IEnumerable<AvailableMolds>? addedMolds = null, 
            IEnumerable<AvailableMolds>? removedMolds = null
        )
        {
            var payload = GetDiscordWebhookMessage(totalAvailableMolds, addedMolds, removedMolds);

            var retval = 0;
            using var client = new HttpClient();
            foreach (var webhook in webhooks)
            {
                var response = await client.PostAsJsonAsync(webhook, payload);
                if (response.IsSuccessStatusCode)
                    retval++;
            }
            

            return retval;
        }

        public static async Task<int> SendMessageToWebhookWithAttachment(
            IEnumerable<string> webhooks,
            Stream? attachmentStream, // Stream for the file you want to attach
            string? attachmentFileName, // Filename for the attachment
            IEnumerable<AvailableMolds> totalAvailableMolds,
            IEnumerable<AvailableMolds>? addedMolds = null,
            IEnumerable<AvailableMolds>? removedMolds = null
        )
        {
            var payload = GetDiscordWebhookMessage(totalAvailableMolds, addedMolds, removedMolds);

            var retval = 0;
            using var client = new HttpClient();

            foreach (var webhook in webhooks)
            {
                var content = JsonContent.Create(payload);
                using var formData = new MultipartFormDataContent("boundary")
                {
                    // Add the JSON part
                    { content , "payload_json" }
                };

                // Add the file attachment if a stream and filename are provided
                if (attachmentStream != null && attachmentFileName != null)
                {
                    var fileContent = new StreamContent(attachmentStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel"); // Adjust the MIME type accordingly
                    formData.Add(fileContent, "files[0]", attachmentFileName);
                }

                // Send the request
                var response = await client.PostAsync(webhook, formData);
                if (response.IsSuccessStatusCode)
                    retval++;
            }

            return retval;
        }

        private static DiscordWebhookMessage GetDiscordWebhookMessage(
            IEnumerable<AvailableMolds> totalAvailableMolds,
            IEnumerable<AvailableMolds>? addedMolds = null,
            IEnumerable<AvailableMolds>? removedMolds = null
        )
        {
            var embeds = new List<DiscordEmbed>(); // Populate this list as before

            // Conditionally add "Added Molds" embed if addedMolds is not null or empty
            if (addedMolds != null && addedMolds.Any())
            {
                embeds.Add(new DiscordEmbed
                {
                    Title = "Added Molds",
                    Description = GetGroupedDiscsDescription(
                        addedMolds.Select(m => new AvailableCustomDisc
                        {
                            Mold = m.Mold,
                            Plastic = m.Plastic,
                            Weight = m.Weight,
                        })),
                    Color = 65280
                });
            }

            // Conditionally add "Removed Molds" embed if removedMolds is not null or empty
            if (removedMolds != null && removedMolds.Any())
            {
                embeds.Add(new DiscordEmbed
                {
                    Title = "Removed Molds",
                    Description = GetGroupedDiscsDescription(
                        removedMolds.Select(m => new AvailableCustomDisc
                        {
                            Mold = m.Mold,
                            Plastic = m.Plastic,
                            Weight = m.Weight,
                        })),
                    Color = 16711680
                });
            }

            // Add "Total Available Molds" last
            embeds.Add(new DiscordEmbed
            {
                Title = "Total Available Molds",
                Description = GetGroupedDiscsDescription(
                    totalAvailableMolds.Select(m => new AvailableCustomDisc
                    {
                        Mold = m.Mold,
                        Plastic = m.Plastic,
                        Weight = m.Weight,
                    })),
                Color = 255
            });

            // Construct the JSON part of the payload
            var payload = new DiscordWebhookMessage
            {
                Content = "The MVP Custom File has been updated.",
                Embeds = embeds
            };

            return payload;
        }

        private static string GetGroupedDiscsDescription(IEnumerable<AvailableCustomDisc> availableCustomDiscs)
        {
            var groupedNewMolds = availableCustomDiscs.GroupBy(
                acd => acd.Plastic,
                acd => new { acd.Mold, acd.Weight },
                (key, g) => new GroupedAvailableCustomDisc
                {
                    Plastic = key,
                    Molds = g.GroupBy(
                        m => m.Mold,
                        m => m.Weight,
                        (key, gr) => new GroupedAvailableCustomDiscByMold
                        {
                            Mold = key,
                            Weight = gr.ToList()
                        }
                    )
                }
            );

            var description = new StringBuilder();
            foreach ((var group, var index) in groupedNewMolds.Select((g, i) => (group: g, index: i)))
            {
                description.AppendLine($"{group.Plastic}");
                description.AppendLine($"--------------------");
                foreach (var mold in group.Molds)
                {
                    description.AppendLine($" {mold.Mold} ({string.Join(", ", mold.Weight)})");
                }
                if (index < (groupedNewMolds.Count() - 1))
                    description.AppendLine();
            }

            return description.ToString();
        }
    }
}
