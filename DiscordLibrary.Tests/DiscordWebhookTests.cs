using MVPCustomCheckerLibrary.DAL.Entities;

namespace DiscordLibrary.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            var previousAvailableMolds = new List<AvailableMolds>
            {
                new() { Id = 6, Plastic = "Hybrid Catch Discs", Mold = "Glitch (Soft) Neutron", Weight = "150-154" },
                new() { Id = 7, Plastic = "MVP Neutron", Mold = "Watt", Weight = "170-175" },
                new() { Id = 22, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "155-159" },
                new() { Id = 23, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "160-164" },
                new() { Id = 24, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "165-169" },
                new() { Id = 25, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "170-175" },
                new() { Id = 8, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "165-169" },
                new() { Id = 9, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "170-175" },
                new() { Id = 12, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "165-169" },
                new() { Id = 13, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "170-175" },
                new() { Id = 14, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "165-169" },
                new() { Id = 15, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "170-175" },
                new() { Id = 16, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "165-169" },
                new() { Id = 17, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "170-175" },
                new() { Id = 18, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "165-169" },
                new() { Id = 19, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "170-175" },
            };

            var currentAvailableMolds = new List<AvailableMolds>
            {
                new() { Id = 1, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "150-154" },
                new() { Id = 2, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "155-159" },
                new() { Id = 3, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "160-164" },
                new() { Id = 4, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "165-169" },
                new() { Id = 5, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "170-175" },
                new() { Id = 6, Plastic = "Hybrid Catch Discs", Mold = "Glitch (Soft) Neutron", Weight = "150-154" },
                new() { Id = 7, Plastic = "MVP Neutron", Mold = "Watt", Weight = "170-175" },
                new() { Id = 8, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "165-169" },
                new() { Id = 9, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "170-175" },
                new() { Id = 10, Plastic = "Streamline Neutron", Mold = "Echo", Weight = "165-169" },
                new() { Id = 11, Plastic = "Streamline Neutron", Mold = "Echo", Weight = "170-175" },
                new() { Id = 12, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "165-169" },
                new() { Id = 13, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "170-175" },
                new() { Id = 14, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "165-169" },
                new() { Id = 15, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "170-175" },
                new() { Id = 16, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "165-169" },
                new() { Id = 17, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "170-175" },
                new() { Id = 18, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "165-169" },
                new() { Id = 19, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "170-175" },
                new() { Id = 20, Plastic = "Axiom Eclipse", Mold = "Crave", Weight = "165-169" },
                new() { Id = 21, Plastic = "Axiom Eclipse", Mold = "Crave", Weight = "170-175" },
            };

            var webhooks = new[] { "https://discord.com/api/v10/webhooks/1224549511750488194/1H6mFgF5Qtk_VEy751lbblv8yMs_74stPIaAKQMvzAfjIclE4hYdwIA2Snx7CGJX3-ib?wait=true" };
            var addedMolds = currentAvailableMolds.Where(c => !previousAvailableMolds.Contains(c));
            var removedMolds = previousAvailableMolds.Where(c => !currentAvailableMolds.Contains(c));

            var number = await DiscordWebhook.SendMessageToWebhook(webhooks, currentAvailableMolds, addedMolds, removedMolds);
            Assert.That(number, Is.GreaterThan(0));
        }

        [Test]
        public async Task Test2()
        {
            var previousAvailableMolds = new List<AvailableMolds>
            {
                new() { Id = 6, Plastic = "Hybrid Catch Discs", Mold = "Glitch (Soft) Neutron", Weight = "150-154" },
                new() { Id = 7, Plastic = "MVP Neutron", Mold = "Watt", Weight = "170-175" },
                new() { Id = 22, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "155-159" },
                new() { Id = 23, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "160-164" },
                new() { Id = 24, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "165-169" },
                new() { Id = 25, Plastic = "Axiom Neutron", Mold = "Crave", Weight = "170-175" },
                new() { Id = 8, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "165-169" },
                new() { Id = 9, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "170-175" },
                new() { Id = 12, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "165-169" },
                new() { Id = 13, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "170-175" },
                new() { Id = 14, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "165-169" },
                new() { Id = 15, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "170-175" },
                new() { Id = 16, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "165-169" },
                new() { Id = 17, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "170-175" },
                new() { Id = 18, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "165-169" },
                new() { Id = 19, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "170-175" },
            };

            var currentAvailableMolds = new List<AvailableMolds>
            {
                new() { Id = 1, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "150-154" },
                new() { Id = 2, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "155-159" },
                new() { Id = 3, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "160-164" },
                new() { Id = 4, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "165-169" },
                new() { Id = 5, Plastic = "Axiom Fission", Mold = "Rhythm", Weight = "170-175" },
                new() { Id = 6, Plastic = "Hybrid Catch Discs", Mold = "Glitch (Soft) Neutron", Weight = "150-154" },
                new() { Id = 7, Plastic = "MVP Neutron", Mold = "Watt", Weight = "170-175" },
                new() { Id = 8, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "165-169" },
                new() { Id = 9, Plastic = "Streamline Neutron", Mold = "Ascend", Weight = "170-175" },
                new() { Id = 10, Plastic = "Streamline Neutron", Mold = "Echo", Weight = "165-169" },
                new() { Id = 11, Plastic = "Streamline Neutron", Mold = "Echo", Weight = "170-175" },
                new() { Id = 12, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "165-169" },
                new() { Id = 13, Plastic = "Streamline Neutron", Mold = "Jet", Weight = "170-175" },
                new() { Id = 14, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "165-169" },
                new() { Id = 15, Plastic = "Axiom Electron", Mold = "Pixel SimonLine", Weight = "170-175" },
                new() { Id = 16, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "165-169" },
                new() { Id = 17, Plastic = "Axiom Electron", Mold = "Pixel (Soft) SimonLine", Weight = "170-175" },
                new() { Id = 18, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "165-169" },
                new() { Id = 19, Plastic = "Axiom Electron", Mold = "Pixel (Firm) SimonLine", Weight = "170-175" },
                new() { Id = 20, Plastic = "Axiom Eclipse", Mold = "Crave", Weight = "165-169" },
                new() { Id = 21, Plastic = "Axiom Eclipse", Mold = "Crave", Weight = "170-175" },
            };

            var excelUrl = "https://mvpdiscsports.com/documents/MVP-Custom.xls";
            using var httpClient = new HttpClient();
            var downloadResponse = await httpClient.GetAsync(excelUrl);
            using var stream = await downloadResponse.Content.ReadAsStreamAsync();

            var webhooks = new[] { "https://discord.com/api/v10/webhooks/1224549511750488194/1H6mFgF5Qtk_VEy751lbblv8yMs_74stPIaAKQMvzAfjIclE4hYdwIA2Snx7CGJX3-ib?wait=true" };
            var addedMolds = currentAvailableMolds.Where(c => !previousAvailableMolds.Contains(c));
            var removedMolds = previousAvailableMolds.Where(c => !currentAvailableMolds.Contains(c));

            var number = await DiscordWebhook.SendMessageToWebhookWithAttachment(webhooks, stream, "MVP-Custom.xls", currentAvailableMolds, addedMolds, removedMolds);
            Assert.That(number, Is.GreaterThan(0));
        }
    }
}