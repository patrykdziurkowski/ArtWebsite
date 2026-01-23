using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using web;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ArtistsTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void Index_RedirectsToLogin_WhenNotLoggedIn()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Login")).Should().BeTrue();
        }

        [Fact, Order(1)]
        public void Index_RedirectsToSetup_WhenNoArtistProfile()
        {
                Register();
                Login();
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public void Setup_RedirectsToIndex_WhenSuccess()
        {
                CreateArtistProfile();

                Wait.Until(d => d.Url.Contains("/Artists/Index") || d.Url.EndsWith("/Artist/") || d.Url.EndsWith("/Artist")).Should().BeTrue();
        }

        [Fact, Order(3)]
        public void Setup_RedirectsToIndex_WhenUserHasArtistProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Wait.Until(d => d.Url.Contains("Setup") == false);
        }

        [Fact, Order(4)]
        public void Index_ShouldInitiallyShow5ArtistsArtPieces_WhenArtistHas8ArtPieces()
        {
                for (int i = 0; i < 8; ++i)
                {
                        UploadArtPiece();
                }

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                bool has5ArtPieces = Wait.Until(d => d.FindElements(By.CssSelector("#artPiecesList > *")).Count == 5);
                has5ArtPieces.Should().BeTrue();
        }

        [Fact, Order(5)]
        public void Index_ShouldShowAllArtistsArtPieces_WhenMoreLoaded()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.FindElement(By.Id("loadMoreArtPieces")));
                IWebElement loadMoreArtPieces = Driver.FindElement(By.Id("loadMoreArtPieces"));
                ScrollIntoView(loadMoreArtPieces);
                Wait.Until(ExpectedConditions.ElementToBeClickable(loadMoreArtPieces)).Click();

                bool has8ArtPieces = Wait.Until(d => d.FindElements(By.CssSelector("#artPiecesList > *")).Count == 8);
                has8ArtPieces.Should().BeTrue();
        }


        [Fact, Order(6)]
        public void ArtistProfileInfo_ShouldBeChanged_WhenUpdated()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.FindElement(By.Id("editProfileModalButton"))).Click();
                IWebElement submitButton = Wait.Until(ExpectedConditions
                        .ElementToBeClickable(By.Id("editArtistProfile")));
                IWebElement nameInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Name\"]"));
                nameInput.Clear();
                nameInput.SendKeys("NewName");
                IWebElement summaryInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Summary\"]"));
                summaryInput.Clear();
                summaryInput.SendKeys("New summary text!");
                string path = Path.GetFullPath("../../../resources/exampleImage.png");
                Driver.FindElement(By.Name("Image")).SendKeys(path);
                submitButton.Click();

                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == "NewName").Should().BeTrue();
                Wait.Until(d => d.FindElement(By.Id("artistSummary")).Text == "New summary text!").Should().BeTrue();

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == "NewName").Should().BeTrue();
                Wait.Until(d => d.FindElement(By.Id("artistSummary")).Text == "New summary text!").Should().BeTrue();
                Driver.FindElement(By.Id("artist-profile-picture")).GetAttribute("src")
                        .Should().NotContain(Constants.DEFAULT_PROFILE_PICTURE_PATH);
        }

        [Fact, Order(7)]
        public void Deactivate_RedirectsToIndex()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");
                Wait.Until(d => d.FindElement(By.Id("deactivate-artist-popup"))).Click();

                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deactivate-artist"))).Click();

                Wait.Until(d => DriverIsAtBaseUrl()).Should().BeTrue();
        }

        [Fact, Order(8)]
        public void Index_RedirectsAgain_WhenArtistDeactivated()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

}
