using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using premium.Api.Data;
using premium.Api.Models;
using Microsoft.EntityFrameworkCore.InMemory;


namespace Premium.Tests.Integration
{
    
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<premiumDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Add EF InMemory for tests
                services.AddDbContext<premiumDbContext>(static options =>
                {
                    object value = options.UseInMemoryDatabase("InsuranceTestDb");
                });

                // Build the service provider to create the DB and seed data
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<premiumDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Seed occupations (same as real app)
                db.Occupations.AddRange(
                    new premium.Api.Models.Occupation { Code = "Cleaner", DisplayName = "Cleaner", Rating = "Light Manual", Factor = 11.50m },
                    new premium.Api.Models.Occupation { Code = "Doctor", DisplayName = "Doctor", Rating = "Professional", Factor = 1.50m },
                    new premium.Api.Models.Occupation { Code = "Author", DisplayName = "Author", Rating = "White Collar", Factor = 2.25m },
                    new premium.Api.Models.Occupation { Code = "Farmer", DisplayName = "Farmer", Rating = "Heavy Manual", Factor = 31.75m },
                    new premium.Api.Models.Occupation { Code = "Mechanic", DisplayName = "Mechanic", Rating = "Heavy Manual", Factor = 31.75m },
                    new premium.Api.Models.Occupation { Code = "Florist", DisplayName = "Florist", Rating = "Light Manual", Factor = 11.50m },
                    new premium.Api.Models.Occupation { Code = "Other", DisplayName = "Other", Rating = "Heavy Manual", Factor = 31.75m }
                );
                db.SaveChanges();
            });
        }
    }

}
    