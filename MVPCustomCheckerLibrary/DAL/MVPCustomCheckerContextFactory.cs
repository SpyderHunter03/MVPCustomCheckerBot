using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MVPCustomCheckerLibrary.DAL
{
    public class MVPCustomCheckerContextFactory : IDesignTimeDbContextFactory<MVPCustomCheckerContext>
    {
        public MVPCustomCheckerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
		        // Set the base path to the project directory
		        .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
		        // Include appsettings.json, environment variables, and user secrets
		        .AddJsonFile("appsettings.json", optional: true)
				.AddEnvironmentVariables() // Make sure this is included
                .AddUserSecrets<MVPCustomCheckerContext>() // Use this if your DbContext is in the same project as your user secrets
                .Build();

            var connectionString = configuration.GetConnectionString("Database");

            var builder = new DbContextOptionsBuilder<MVPCustomCheckerContext>();
            builder.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,36))); // Adjust for your database provider

            return new MVPCustomCheckerContext(builder.Options);
        }
    }
}
