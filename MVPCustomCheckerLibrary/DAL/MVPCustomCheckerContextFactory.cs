using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MVPCustomCheckerLibrary.DAL
{
    public class MVPCustomCheckerContextFactory : IDesignTimeDbContextFactory<MVPCustomCheckerContext>
    {
        public MVPCustomCheckerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables() // Make sure this is included
                .AddUserSecrets<MVPCustomCheckerContext>() // Use this if your DbContext is in the same project as your user secrets
                .Build();

            var connectionString = configuration.GetConnectionString("Database");

            var builder = new DbContextOptionsBuilder<MVPCustomCheckerContext>();
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)); // Adjust for your database provider

            return new MVPCustomCheckerContext(builder.Options);
        }
    }
}
