using System.Diagnostics;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using web.Data;

namespace tests.Integration.Fixtures;

public sealed class DatabaseTestContext : WebApplicationFactory<Program>, IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TEST_CONNECTION_STRING = $"Data Source=localhost,14332;User ID=SA;Password={DB_TEST_PASSWORD};Encrypt=False";
        private readonly IContainerImageService _databaseImage;
        public IContainerService Database { get; }
        public IContainerService Ai { get; }

        public DatabaseTestContext()
        {
                Environment.SetEnvironmentVariable("AI_CONTAINER_URL", "http://localhost:8081/");

                _databaseImage = new Builder()
                        .DefineImage("art-website-db-test")
                        .BuildArguments($"password={DB_TEST_PASSWORD}")
                        .FromFile("../../../../db/Dockerfile")
                        .Build();

                using Process process = new()
                {
                        StartInfo = new()
                        {
                                FileName = "docker",
                                Arguments = "build -t art-website-ai-test ../../../../ai/",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                        }
                };
                process.Start();
                process.WaitForExit();

                Database = new Builder()
                        .UseContainer()
                        .DeleteIfExists(force: true)
                        .UseImage("art-website-db-test")
                        .WithName("art-website-db-test")
                        .ExposePort(14332, 1433)
                        .WaitForHealthy()
                        .Build()
                        .Start();
                Ai = new Builder()
                        .UseContainer()
                        .DeleteIfExists(force: true)
                        .UseImage("art-website-ai-test")
                        .WithName("art-website-ai-test")
                        .ExposePort(8081, 8081)
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
                Database.Remove(force: true);
                Database.Dispose();
                _databaseImage.Remove(force: true);
                _databaseImage.Dispose();

                Ai.Stop();
                Ai.Remove(force: true);
                Ai.Dispose();

                base.Dispose();
        }
}
