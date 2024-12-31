using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace tests.e2e.fixtures;

[Collection("Web server collection")]
public class WebDriverBase : IDisposable
{
        public IWebDriver Driver { get; }
        public WebDriverWait Wait { get; }
        public WebServer WebServer { get; }

        public WebDriverBase(WebServer webServer)
        {
                WebServer = webServer;
                ChromeOptions options = new();
                options.AddArguments("--headless");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--window-size=1920,1080");
                options.AddArguments("--ignore-certificate-errors");
                options.AddArguments("--disable-web-security");
                options.AddArguments("--allow-running-insecure-content");
                Driver = new ChromeDriver(options);
                Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
                Driver.Quit();
                Driver.Dispose();
        }

        public async Task RegisterAsync(string email, string password)
        {
                await Driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");
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
                await Driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Login");
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);

                Driver.FindElement(By.Id("login-submit")).Click();

                Wait.Until(d => d.PageSource.Contains(email));
        }

        public async Task LoginAsync()
        {
                await LoginAsync("john@smith.com", "Ex@mpl3");
        }

        public async Task LogoutAsync()
        {
                await Driver.Navigate().GoToUrlAsync("http://localhost/");
                Driver.FindElement(By.Id("logout")).Click();
        }

        public bool DriverIsAtBaseUrl()
        {
                return Wait.Until(d =>
                {
                        Uri uri = new(d.Url);
                        return string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/");
                });
        }
}
