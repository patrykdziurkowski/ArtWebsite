using System;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace tests.e2e.fixtures;

public class WebServer : IDisposable
{
        private const string DB_TEST_PASSWORD = "exampleP@ssword123";
        private const string TRUNCATE_DATA_QUERY = @"EXEC sp_MSForEachTable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?'
";
        public ICompositeService Server { get; }

        public WebServer()
        {
                string dockerComposePath = Path.GetFullPath("../../../../../docker-compose.yaml");
                Server = new Builder()
                        .UseContainer()
                        .UseCompose()
                        .FromFile(dockerComposePath)
                        .WithEnvironment($"MSSQL_SA_PASSWORD={DB_TEST_PASSWORD}")
                        .WithEnvironment("ASPNETCORE_ENVIRONMENT=Development")
                        .ForceBuild()
                        .ForceRecreate()
                        .WaitForHttp("web", "http://localhost")
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
