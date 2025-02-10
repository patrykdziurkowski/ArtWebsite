using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ArtistsTests : WebDriverBase
{
        public ArtistsTests(WebDriverInitializer initializer)
                : base(initializer)
        {
        }

        [Fact, Order(0)]
        public void Index_RedirectsToLogin_WhenNotLoggedIn()
        {
                ResetTestContext();

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
        public void Index_DoesntRedirect_WhenHasArtistProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Action action = () => Wait.Until(d => d.Url.Contains("Setup"));
                action.Should().Throw<WebDriverTimeoutException>();
        }

        [Fact, Order(4)]
        public void Setup_RedirectsToIndex_WhenUserHasArtistProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Wait.Until(d => d.Url.Contains("Setup") == false);
        }

        [Fact, Order(5)]
        public void Deactivate_RedirectsToIndex()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");
                Driver.FindElement(By.Id("deactivate-artist-popup")).Click();

                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deactivate-artist"))).Click();

                Wait.Until(d => DriverIsAtBaseUrl()).Should().BeTrue();
        }

        [Fact, Order(6)]
        public void Index_RedirectsAgain_WhenArtistDeactivated()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

}
