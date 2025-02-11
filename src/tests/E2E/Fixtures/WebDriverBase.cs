using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace tests.E2E.Fixtures;

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

        public void Register(string email = "john@smith.com", string password = "Ex@mpl3")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Identity/Account/Register");
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);
                Driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);

                Driver.FindElement(By.Id("registerSubmit")).Click();

                Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("confirm-link"))).Click();
                Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email."));
        }

        public void Login(string email = "john@smith.com", string password = "Ex@mpl3")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Identity/Account/Login");
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);

                Driver.FindElement(By.Id("login-submit")).Click();

                Wait.Until(d => d.PageSource.Contains(email));
        }

        public void Logout()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/");
                Driver.FindElement(By.Id("logout")).Click();
                Wait.Until(d => d.PageSource.Contains("login"));
        }

        public void CreateArtistProfile(string name = "SomeArtist")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Driver.FindElement(By.Id("Name")).SendKeys(name);
                Driver.FindElement(By.Id("Summary")).SendKeys("My description!");
                Driver.FindElement(By.Id("setup-artist")).Click();

                Wait.Until(d => d.Url.Contains("/Artist/Setup") == false);
        }

        public void CreateUserWithArtistProfile(string email = "john@smith.com",
                string password = "Ex@mpl3", string name = "SomeArtist")
        {
                Register(email, password);
                Login(email, password);
                CreateArtistProfile(name);
        }

        public void UploadArtPiece()
        {
                string filePath = "../../../resources/exampleImage.png";
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece/Upload");
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece/Upload");
                Driver.FindElement(By.Id("image-input")).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                Driver.FindElement(By.Id("upload-submit")).Click();
                Wait.Until(d => d.Url.Contains("/ArtPiece/Upload") == false);
        }

        public bool DriverIsAtBaseUrl()
        {
                return Wait.Until(d =>
                {
                        Uri uri = new(d.Url);
                        return string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/");
                });
        }

        public void ResetTestContext()
        {
                if (Driver.PageSource.Contains("logout", StringComparison.OrdinalIgnoreCase))
                {
                        Logout();
                }
                WebServer.ClearData();
        }
}
