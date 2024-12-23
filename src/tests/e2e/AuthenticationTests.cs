using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

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
        public async Task RegisterPage_Loads()
        {
                _webServer.ClearData();
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");

                _driver.Title.Should().Contain("Register");
        }

        [Fact, Order(1)]
        public async Task Register_RedirectsToEmailConfirmation_WhenSuccessful()
        {
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");
                _driver.FindElement(By.Id("Input_Email")).SendKeys("john@smith.com");
                _driver.FindElement(By.Id("Input_Password")).SendKeys("Ex@mpl3");
                _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("Ex@mpl3");

                _driver.FindElement(By.Id("registerSubmit")).Click();

                _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("confirm-link"))).Click();
                _wait.Until(d => d.PageSource.Contains("Thank you for confirming your email.")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Login_ShouldWork()
        {
                await _driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Login");
                _driver.FindElement(By.Id("Input_Email")).SendKeys("john@smith.com");
                _driver.FindElement(By.Id("Input_Password")).SendKeys("Ex@mpl3");

                _driver.FindElement(By.Id("login-submit")).Click();

                _wait.Until(d => d.PageSource.Contains("john@smith.com")).Should().BeTrue();
        }
}

