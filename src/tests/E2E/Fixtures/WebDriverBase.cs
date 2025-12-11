using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace tests.E2E.Fixtures;

[Collection("Web server collection")]
public abstract class WebDriverBase(WebDriverInitializer initializer)
{
        public const string HTTP_PROTOCOL_PREFIX = "https://";
        public IWebDriver Driver { get; } = initializer.Driver;
        public WebDriverWait Wait { get; } = initializer.Wait;
        public WebServer WebServer { get; } = initializer.WebServer;

        public void Register(string userName = "SomeUser123",
                string email = "john@smith.com", string password = "Ex@mpl3")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Register");
                Wait.Until(d => d.FindElement(By.Id("Input_UserName"))).SendKeys(userName);
                Driver.FindElement(By.Id("Input_Email")).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);
                Driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);

                Driver.FindElement(By.Id("registerSubmit")).Click();

                Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("confirm-link"))).Click();
                Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email."));
        }

        public void Login(string email = "john@smith.com", string password = "Ex@mpl3")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Login");
                Wait.Until(d => d.FindElement(By.Id("Input_Email"))).SendKeys(email);
                Driver.FindElement(By.Id("Input_Password")).SendKeys(password);

                Driver.FindElement(By.Id("login-submit")).Click();

                Wait.Until(d => d.FindElement(By.CssSelector("#logout")));
        }

        public void Logout()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/");
                Wait.Until(d => d.FindElement(By.Id("logout"))).Click();
                Wait.Until(d => d.FindElement(By.Id("login")));
        }

        public void CreateArtistProfile(string name = "SomeArtist")
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Wait.Until(d => d.FindElement(By.Id("Name"))).SendKeys(name);
                Driver.FindElement(By.Id("Summary")).SendKeys("My description!");
                Driver.FindElement(By.Id("setup-artist")).Click();

                Wait.Until(d => d.Url.Contains("/Artist/Setup") == false);
        }

        public void CreateUserWithArtistProfile(string userName = "SomeUser123",
                string email = "john@smith.com", string password = "Ex@mpl3",
                string name = "SomeArtist")
        {
                Register(userName, email, password);
                Login(email, password);
                CreateArtistProfile(name);
        }

        public void UploadArtPiece()
        {
                string filePath = "../../../resources/exampleImage.png";
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece/Upload");
                Wait.Until(d => d.FindElement(By.Id("image-input"))).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                var submit = Driver.FindElement(By.Id("upload-submit"));
                ScrollIntoView(submit);
                submit.Click();

                Wait.Until(d => d.Url.Contains("/ArtPiece/Upload") == false);
        }

        public void ReviewThisArtPiece()
        {
                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.Id("reviewArt"))).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("reviewModal")));
                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm > button")).Click();

                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("reviewModal")));
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("postReviewModal")));
                Wait.Until(d => d.FindElement(By.CssSelector("#postReviewModal .btn-close"))).Click();

                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("postReviewModal")));
        }

        public void ReviewThenLikeThisArtPiece()
        {
                Wait.Until(d => d.FindElement(By.Id("reviewArt"))).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(Driver.FindElement(By.Id("reviewForm"))));

                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("postReviewModal")));
                IWebElement likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                likeButton.Text.Should().Be("Like");
                likeButton.Click();
                Wait.Until(d => likeButton.Text == "Unlike").Should().BeTrue();
        }

        public bool DriverIsAtBaseUrl()
        {
                return Wait.Until(d =>
                {
                        Uri uri = new(d.Url);
                        return string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/");
                });
        }

        public void ScrollIntoView(IWebElement element)
        {
                Actions actions = new(Driver);
                actions.MoveToElement(element);
                actions.Perform();
                Thread.Sleep(1000);
        }

        public void ResetTestContext()
        {
                if (Driver.FindElements(By.Id("logout")).Count != 0)
                {
                        Logout();
                }
                WebServer.ClearData();
        }
}
