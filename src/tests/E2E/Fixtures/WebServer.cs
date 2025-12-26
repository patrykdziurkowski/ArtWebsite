using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace tests.E2E.Fixtures;

public class WebServer : IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TRUNCATE_DATA_QUERY = @"
                SET QUOTED_IDENTIFIER ON;
                DELETE FROM [dbo].[ArtPiecesServed];
                DELETE FROM [dbo].[MissionProgresses];
                DELETE FROM [dbo].[ReviewerPointAwards];
                DELETE FROM [dbo].[ArtistPointAwards];
                DELETE FROM [dbo].[Tags];
                DELETE FROM [dbo].[ArtPieceTags];
                DELETE FROM [dbo].[Likes];
                DELETE FROM [dbo].[Boosts];
                DELETE FROM [dbo].[Reviews];
                DELETE FROM [dbo].[Reviewers];
                DELETE FROM [dbo].[ArtPieces];
                DELETE FROM [dbo].[Artists];
                DELETE FROM [dbo].[AspNetUserClaims];
                DELETE FROM [dbo].[AspNetUserLogins];
                DELETE FROM [dbo].[AspNetUserTokens];
                -- Delete AspNetUserRoles for users who are not admins
                DELETE FROM [dbo].[AspNetUserRoles] WHERE UserId NOT IN (
                        SELECT UserId FROM [dbo].[AspNetUserRoles] ur
                        JOIN [dbo].[AspNetRoles] r ON ur.RoleId = r.Id
                        WHERE r.Name = 'Admin');
                -- Delete AspNetUsers for users who are not admins
                DELETE FROM [dbo].[AspNetUsers] WHERE Id NOT IN (
                        SELECT UserId FROM [dbo].[AspNetUserRoles] ur
                        JOIN [dbo].[AspNetRoles] r ON ur.RoleId = r.Id
                        WHERE r.Name = 'Admin');
                ";
        public ICompositeService Server { get; }

        public WebServer()
        {
                // FluentDocker doesnt seem to work well with docker compose v2+ so this is
                // a workaround for that.
                Environment.SetEnvironmentVariable("DOCKER_BUILDKIT", "0");
                Environment.SetEnvironmentVariable("COMPOSE_DOCKER_CLI_BUILD", "0");

                string dockerComposePath = Path.GetFullPath("../../../../../docker-compose.yaml");
                if (!File.Exists(dockerComposePath))
                {
                        throw new InvalidOperationException($"Could not start E2E test webserver due to a missing docker compose file in path {dockerComposePath}");
                }

                string dockerComposeOverridePath = Path.GetFullPath("../../../../../docker-compose.override.yaml");
                if (!File.Exists(dockerComposeOverridePath))
                {
                        throw new InvalidOperationException($"Could not start E2E test webserver due to a missing docker compose override file in path {dockerComposeOverridePath}");
                }

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
                ExecuteSql(TRUNCATE_DATA_QUERY);
        }

        public void ExecuteSql(string query)
        {
                IContainerService db = Server.Containers.Single(c => c.Name.Contains("db"));
                db.Execute($"/opt/mssql-tools18/bin/sqlcmd -U sa -S localhost -P {DB_TEST_PASSWORD} -No -b -Q \"{query}\"");
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
        private static void WaitUntilWebServerReady()
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
