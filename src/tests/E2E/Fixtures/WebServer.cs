using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace tests.E2E.Fixtures;

public class WebServer : IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TRUNCATE_DATA_QUERY = @"
                SET QUOTED_IDENTIFIER ON;
                DELETE FROM [dbo].[Likes];
                DELETE FROM [dbo].[Boosts];
                DELETE FROM [dbo].[Reviews];
                DELETE FROM [dbo].[Reviewers];
                DELETE FROM [dbo].[ArtPieces];
                DELETE FROM [dbo].[Artists];
                DELETE FROM [dbo].[AspNetUserClaims];
                DELETE FROM [dbo].[AspNetUserLogins];
                DELETE FROM [dbo].[AspNetUserTokens];
                DELETE FROM [dbo].[AspNetUserRoles];
                DELETE FROM [dbo].[AspNetUsers];
                ";
        public ICompositeService Server { get; }

        public WebServer()
        {
                string dockerComposePath = Path.GetFullPath("../../../../../docker-compose.yaml");
                string dockerComposeOverridePath = Path.GetFullPath("../../../../../docker-compose.override.yaml");
                Server = new Builder()
                        .UseContainer()
                        .UseCompose()
                        .ServiceName("artwebsite-development")
                        .FromFile(dockerComposePath, dockerComposeOverridePath)
                        .WithEnvironment($"MSSQL_SA_PASSWORD={DB_TEST_PASSWORD}")
                        .RemoveOrphans()
                        .ForceBuild()
                        .ForceRecreate()
                        .WaitForHttp("web", "http://localhost:8080")
                        .Build()
                        .Start();
                WaitUntilWebServerReady();
        }

        public void ClearData()
        {
                IContainerService db = Server.Containers.Single(c => c.Name.Contains("db"));
                db.Execute($"/opt/mssql-tools18/bin/sqlcmd -U sa -S localhost -P {DB_TEST_PASSWORD} -No -b -Q \"{TRUNCATE_DATA_QUERY}\"");
        }

        public void Dispose()
        {
                Server.Stop();
                Server.Remove();
                Server.Dispose();
        }

        /// <summary>
        /// Waits until a successful status code is available at localhost.
        /// Ran synchronously in order to block test execution until ready.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void WaitUntilWebServerReady()
        {
                const int maxIterations = 25;
                // http instead of https to avoid SSL issues
                const string url = "http://localhost:8080";
                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(30);

                for (int i = 0; i < maxIterations; ++i)
                {
                        try
                        {
                                HttpResponseMessage response = client.GetAsync(url).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                        break;
                                }
                        }
                        catch (Exception e)
                        {
                                if (i >= 24)
                                {
                                        throw new InvalidOperationException("Attempting to connect to web server failed after 25 attempts.", e);
                                }
                        }
                        Thread.Sleep(2500);
                }
        }
}
