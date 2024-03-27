using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVPCustomCheckerLibrary.DAL;
using MVPCustomCheckerProcessor;

Console.WriteLine("[info] Starting the checker!");
var _cts = new CancellationTokenSource();

// Load the config file(we'll create this shortly)
Console.WriteLine("[info] Loading config file..");
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
    s.Name.Equals("NextRun", StringComparison.InvariantCultureIgnoreCase));
if (nextRun is null || DateTime.Parse(nextRun.Setting) > DateTime.UtcNow)
    await FileProcessor.ProcessFile(context);