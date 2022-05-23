using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TestWebApp.Infrastructure
{
    public class DesignTimeDbContext : IDesignTimeDbContextFactory<TestWebAppDbContext>
    {
        public TestWebAppDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<TestWebAppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(TestWebAppDbContext)));

            return new TestWebAppDbContext(optionsBuilder.Options);
        }
    }
}
