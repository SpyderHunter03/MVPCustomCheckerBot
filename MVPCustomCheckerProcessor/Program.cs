using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVPCustomCheckerLibrary.DAL;
using MVPCustomCheckerProcessor;

var _cts = new CancellationTokenSource();

// Load the config file(we'll create this shortly)
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var basePath = Directory.GetCurrentDirectory();
var devSettingsPath = Path.Combine(basePath, $"appsettings.{environmentName}.json");

var builder = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(basePath, $"appsettings.{environmentName}.json"), optional: true)
    .AddEnvironmentVariables();

var _config = builder.Build();
var connectionString = _config.GetConnectionString("Database");
var optionsBuilder = new DbContextOptionsBuilder<MVPCustomCheckerContext>();
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

using var context = new MVPCustomCheckerContext(optionsBuilder.Options);

var nextRun = await context.Settings.FirstOrDefaultAsync(s =>
    EF.Functions.Like(s.Name, "NextRun"));

if (nextRun is not null && DateTime.UtcNow <= DateTime.Parse(nextRun.Setting))
{
    Console.WriteLine($"Not running yet. Current Time: {DateTime.UtcNow} Next run time: {nextRun?.Setting}");
    return;
}

await FileProcessor.ProcessFile(context);
    