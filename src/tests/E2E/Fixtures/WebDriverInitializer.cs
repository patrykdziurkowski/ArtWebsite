using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tests.e2e.fixtures;

namespace tests.E2E.Fixtures;

public class WebDriverInitializer : IDisposable
{
        public IWebDriver Driver { get; }
        public WebDriverWait Wait { get; }
        public WebServer WebServer { get; }

        public WebDriverInitializer()
        {
                ChromeOptions options = new();
                options.AddArguments("--headless=new");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--window-size=1920,1080");
                options.AddArguments("--ignore-certificate-errors");
                options.AddArguments("--disable-web-security");
                options.AddArguments("--allow-running-insecure-content");
                Driver = new ChromeDriver(options);
                Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
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
