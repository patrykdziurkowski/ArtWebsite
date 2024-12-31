using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using web.data;

namespace tests.integration.fixtures;

public sealed class DatabaseTestContext : WebApplicationFactory<Program>, IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TEST_CONNECTION_STRING = $"Data Source=localhost,14332;User ID=SA;Password={DB_TEST_PASSWORD};Encrypt=False";
        private readonly IContainerImageService _image;
        public IContainerService Database { get; }

        public DatabaseTestContext()
        {
                _image = new Builder()
                        .DefineImage("art-website-db-test")
                        .BuildArguments($"password={DB_TEST_PASSWORD}")
                        .FromFile("../../../../db/Dockerfile")
                        .Build();
                Database = new Builder()
                        .UseContainer()
                        .DeleteIfExists(force: true)
                        .UseImage("art-website-db-test")
                        .WithName("art-website-db-test")
                        .ExposePort(14332, 1433)
                        .WaitForHealthy()
                        .Build()
                        .Start();

                ApplyMigrations();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
                builder.ConfigureHostConfiguration((config) =>
                {
                        config.AddInMemoryCollection([
                                new KeyValuePair<string, string?>(
                                        "CONNECTION_STRING",
                                        TEST_CONNECTION_STRING)
                        ]);
                });
                return base.CreateHost(builder);
        }


        private void ApplyMigrations()
        {
                IServiceScope scope = Services.CreateScope();
                ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
        }

        public new void Dispose()
        {
                Database.Stop();
                Database.Remove();
                Database.Dispose();
                _image.Remove();
                _image.Dispose();
                base.Dispose();
        }
}
