using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tests.E2E.Utils;

namespace tests.E2E.Fixtures;

public class WebDriverInitializer : IDisposable
{
        public IWebDriver Driver { get; }
        public WebServer WebServer { get; }

        public WebDriverInitializer()
        {
                ChromeOptions options = new();
                // Headless if variable is not set or set to true. The Selenium browser window opens otherwise.
                DotEnv.Load("../../../../../.env"); // Load variables from .env file
                string? headlessVariable = Environment.GetEnvironmentVariable("SELENIUM_HEADLESS");
                bool isParsed = bool.TryParse(headlessVariable, out bool isHeadless);
                if (headlessVariable is null || isParsed == false || isHeadless)
                {
                        options.AddArguments("--headless=new");
                }
                options.AddArguments("--no-sandbox");
                options.AddArguments("--ozone-platform=x11");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--window-size=1920,1080");
                options.AddArguments("--ignore-certificate-errors");
                options.AddArguments("--disable-web-security");
                options.AddArguments("--allow-running-insecure-content");
                Driver = new ChromeDriver(options);
                WebServer = new WebServer();
        }

        public void Dispose()
        {
                Driver.Quit();
                Driver.Dispose();
                WebServer.Server.Stop();
                WebServer.Server.Dispose();
        }
}
