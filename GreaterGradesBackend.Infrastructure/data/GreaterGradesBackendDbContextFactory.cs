using GreaterGradesBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GreaterGradesBackend.Infrastructure
{
    public class GreaterGradesBackendDbContextFactory : IDesignTimeDbContextFactory<GreaterGradesBackendDbContext>
    {
        public GreaterGradesBackendDbContext CreateDbContext(string[] args = null)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\GreaterGradesBackend.Api\appsettings.json"))
                .Build();


            // Create options for DbContext
            var optionsBuilder = new DbContextOptionsBuilder<GreaterGradesBackendDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));


            var passwordHasher = new PasswordHasher<User>();
            return new GreaterGradesBackendDbContext(optionsBuilder.Options, passwordHasher);
        }
    }
}
