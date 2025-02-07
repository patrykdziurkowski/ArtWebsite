using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;

namespace tests.e2e.fixtures;

[Collection("Web server collection")]
public abstract class WebDriverBase
{
        public const string HTTP_PROTOCOL_PREFIX = "https://";
        public IWebDriver Driver { get; }
        public WebDriverWait Wait { get; }
        public WebServer WebServer { get; }

        public WebDriverBase(WebDriverInitializer initializer)
        {
                WebServer = initializer.WebServer;
                Wait = initializer.Wait;
                Driver = initializer.Driver;

        }

        public async Task RegisterAsync(string email, string password)
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Identity/Account/Register");
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);
                Driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);

                Driver.FindElement(By.Id("registerSubmit")).Click();

                Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("confirm-link"))).Click();
                Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email."));
        }

        public async Task RegisterAsync()
        {
                await RegisterAsync("john@smith.com", "Ex@mpl3");
        }

        public async Task LoginAsync(string email, string password)
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Identity/Account/Login");
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);

                Driver.FindElement(By.Id("login-submit")).Click();

                Wait.Until(d => d.PageSource.Contains(email));
        }

        public async Task LoginAsync()
        {
                await LoginAsync("john@smith.com", "Ex@mpl3");
        }

        public async Task TryLogoutAsync()
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/");
                if (Driver.PageSource.Contains("john@smith.com"))
                {
                        Driver.FindElement(By.Id("logout")).Click();
                }
        }

        public async Task CreateArtistProfileAsync(string name)
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Driver.FindElement(By.Id("Name")).SendKeys(name);
                Driver.FindElement(By.Id("Summary")).SendKeys("My description!");
                Driver.FindElement(By.Id("setup-artist")).Click();
        }

        public async Task CreateArtistProfileAsync()
        {
                await CreateArtistProfileAsync("SomeArtist");
        }

        public bool DriverIsAtBaseUrl()
        {
                return Wait.Until(d =>
                {
                        Uri uri = new(d.Url);
                        return string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/");
                });
        }

        public async Task ResetTestContextAsync()
        {
                await TryLogoutAsync();
                WebServer.ClearData();
        }
}
