using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace tests.E2E.Fixtures;

public class WebServer : IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TRUNCATE_DATA_QUERY = @"
                SET QUOTED_IDENTIFIER ON;
                DELETE FROM [dbo].[AspNetUsers];
                DELETE FROM [dbo].[Artists];
                DELETE FROM [dbo].[ArtPieces];
                DELETE FROM [dbo].[Reviews];
                DELETE FROM [dbo].[Reviewers];
                DELETE FROM [dbo].[Likes];
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
        }

        public void ClearData()
        {
                IContainerService db = Server.Containers.Single(c => c.Name == "db");
                db.Execute($"/opt/mssql-tools18/bin/sqlcmd -U sa -S localhost -P {DB_TEST_PASSWORD} -No -b -Q \"{TRUNCATE_DATA_QUERY}\"");
        }

        public void Dispose()
        {
                Server.Stop();
                Server.Remove();
                Server.Dispose();
        }
}
