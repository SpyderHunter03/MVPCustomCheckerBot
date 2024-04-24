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

app.MapGet("/filelocations", async (MVPCustomCheckerContext dbContext) =>
{
  var locations = await dbContext.CustomFileLocations.ToListAsync();
  return locations;
})
.WithName("GetFileLocations")
.WithOpenApi();

app.MapGet("/availablemolds", async (MVPCustomCheckerContext dbContext) =>
{
  var locations = await dbContext.AvailableMolds.ToListAsync();
  return locations;
})
.WithName("GetAvailableMolds")
.WithOpenApi();

app.MapGet("/download/{fileId}", async (int fileId, MVPCustomCheckerContext dbContext) =>
{
  // Example: Assume files are stored in a directory named "Files" in wwwroot
  var fileRecord = await dbContext.CustomFileLocations
								  .FirstOrDefaultAsync(f => f.Id == fileId);

  if (fileRecord == null || !File.Exists(fileRecord.FileLocation))
  {
	return Results.NotFound("File not found.");
  }

  var stream = File.OpenRead(fileRecord.FileLocation);
  var contentType = "APPLICATION/octet-stream"; // Set appropriate content type
  return Results.File(stream, contentType, fileRecord.FileName);
})
.WithName("DownloadFile")
.WithOpenApi();

app.MapFallbackToFile("/index.html");

app.Run();
