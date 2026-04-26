using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AdminPanelBack.DB
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var cwd = Directory.GetCurrentDirectory();
            var envPath = FindEnvPath(cwd);
            if (envPath != null)
            {
                Env.Load(envPath);
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection") ??
                configuration["DefaultConnection"] ??
                configuration["ConnectionStrings__DefaultConnection"] ??
                configuration["DEFAULT_CONNECTION"] ??
                configuration["DEFAULTCONNECTION"] ??
                configuration["DATABASE_URL"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("DefaultConnection is not set for design-time DbContextFactory");

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }

        private static string? FindEnvPath(string startDirectory)
        {
            var dir = new DirectoryInfo(startDirectory);
            for (var i = 0; i < 6 && dir != null; i++, dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, ".env");
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }
    }
}