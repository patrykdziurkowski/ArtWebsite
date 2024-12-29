using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

[Collection("Web server collection")]
public class ArtistTests : IDisposable
{
        private readonly WebServer _webServer;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ArtistTests(WebServer webServer)
        {
                _webServer = webServer;
                ChromeOptions options = new();
                options.AddArguments("--headless");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--window-size=1920,1080");
                options.AddArguments("--ignore-certificate-errors");
                options.AddArguments("--disable-web-security");
                options.AddArguments("--allow-running-insecure-content");
                _driver = new ChromeDriver(options);
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
                _driver.Quit();
                _driver.Dispose();
        }

        [Fact, Order(0)]
        public async Task Setup_RedirectsToIndex_WhenSuccess()
        {
                _webServer.ClearData();
                await _driver.Navigate().GoToUrlAsync("http://localhost/Artist/Setup");

                _driver.FindElement(By.Id("Name")).SendKeys("SomeArtist");
                _driver.FindElement(By.Id("Summary")).SendKeys("My description!");
                _driver.FindElement(By.TagName("form")).FindElement(By.TagName("button")).Click();

                _wait.Until(d => d.PageSource.Contains("Index")).Should().BeTrue();
        }

}
