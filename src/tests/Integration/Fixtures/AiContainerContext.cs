using System.Diagnostics;
using Azure;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace tests.Integration.Fixtures;

public class AiContainerContext : IDisposable
{
        public IContainerService AiContainer { get; }

        public AiContainerContext()
        {
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
                var test1 = process.StandardOutput.ReadToEnd();
                var test2 = process.StandardError.ReadToEnd();

                AiContainer = new Builder()
                        .UseContainer()
                        .DeleteIfExists(force: true)
                        .UseImage("art-website-ai-test")
                        .WithName("art-website-ai-test")
                        .ExposePort(8081, 8081)
                        .Build()
                        .Start();

                WaitUntilReadyAsync().GetAwaiter().GetResult();
        }

        private static async Task WaitUntilReadyAsync()
        {
                while (true)
                {
                        using HttpClient client = new()
                        {
                                Timeout = TimeSpan.FromSeconds(30)
                        };

                        try
                        {
                                using HttpResponseMessage response = await client
                                        .SendAsync(new HttpRequestMessage(HttpMethod.Head, "http://localhost:8081/tag"));
                                break;
                        }
                        catch (TaskCanceledException)
                        {
                                // not ready yet
                        }
                        catch (HttpRequestException)
                        {
                                // not ready yet
                        }
                }
        }

        public void Dispose()
        {
                AiContainer.Stop();
                AiContainer.Remove(force: true);
                AiContainer.Dispose();
        }
}
