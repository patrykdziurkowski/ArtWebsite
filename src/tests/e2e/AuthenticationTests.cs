using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tests.e2e.fixtures;
using tests.e2e.utils;

namespace tests.e2e;

[TestCaseOrderer("PriorityOrderer", "Tests")]
[Collection("Web server collection")]
public class AuthenticationTests : IDisposable
{
        private readonly WebServer _webServer;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public AuthenticationTests(WebServer webServer)
        {
                _webServer = webServer;
                ChromeOptions options = new();
                options.AddArguments("--headless");
                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--window-size=1920x1080");
                _driver = new ChromeDriver(options);
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
                _driver.Quit();
                _driver.Dispose();
        }

        [Fact, TestPriority(0)]
        public async Task RegisterPage_Loads()
        {
                _webServer.ClearData();
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");

                _driver.Title.Should().Contain("Register");
        }

        [Fact, TestPriority(1)]
        public async Task Register_RedirectsToEmailConfirmation_WhenSuccessful()
        {
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");
                _driver.FindElement(By.Id("Input_Email")).SendKeys("john@smith.com");
                _driver.FindElement(By.Id("Input_Password")).SendKeys("Ex@mpl3");
                _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("Ex@mpl3");

                _driver.FindElement(By.Id("registerSubmit")).Click();

                _wait.Until(d => d.FindElement(By.Id("confirm-link"))).Click();
                _wait.Until(d => d.PageSource.Contains("Thank you for confirming your email.")).Should().BeTrue();
        }

        [Fact, TestPriority(2)]
        public async Task Login_ShouldWork()
        {
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Login");
                _driver.FindElement(By.Id("Input_Email")).SendKeys("john@smith.com");
                _driver.FindElement(By.Id("Input_Password")).SendKeys("Ex@mpl3");

                _driver.FindElement(By.Id("login-submit")).Click();

                _wait.Until(d => d.PageSource.Contains("john@smith.com")).Should().BeTrue();
        }
}

