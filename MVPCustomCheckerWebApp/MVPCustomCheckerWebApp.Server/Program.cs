using Microsoft.EntityFrameworkCore;
using MVPCustomCheckerLibrary.DAL;

var builder = WebApplication.CreateBuilder(args);

// Determine the environment
var environmentName = builder.Environment.EnvironmentName;

// Configuration
builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
	.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("Database");
Console.WriteLine($"Connection String: {connectionString}");
builder.Services.AddDbContext<MVPCustomCheckerContext>(options =>
	options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)),
	mySqlOptions => mySqlOptions.EnableRetryOnFailure(
		maxRetryCount: 5, // The maximum number of retry attempts.
		maxRetryDelay: TimeSpan.FromSeconds(30), // The maximum delay between retries.
		errorNumbersToAdd: null)));


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/filelocations", async (HttpContext httpContext) =>
{
  var dbContext = httpContext.RequestServices.GetRequiredService<MVPCustomCheckerContext>();
  var locations = await dbContext.CustomFileLocations.ToListAsync();
  return locations;
})
.WithName("GetFileLocations")
.WithOpenApi();

app.MapFallbackToFile("/index.html");

app.Run();
