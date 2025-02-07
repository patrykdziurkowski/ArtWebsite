using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.e2e.fixtures;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

public class ArtistTests : WebDriverBase
{
        public ArtistTests(WebDriverInitializer initializer)
                : base(initializer)
        {
        }

        [Fact, Order(0)]
        public async Task Index_RedirectsToLogin_WhenNotLoggedIn()
        {
                await ResetTestContextAsync();

                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Login")).Should().BeTrue();
        }

        [Fact, Order(1)]
        public async Task Index_RedirectsToSetup_WhenNoArtistProfile()
        {
                await RegisterAsync();
                await LoginAsync();
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Setup_RedirectsToIndex_WhenSuccess()
        {
                await CreateArtistProfileAsync();

                Wait.Until(d => d.Url.Contains("/Artist/Index") || d.Url.EndsWith("/Artist/") || d.Url.EndsWith("/Artist")).Should().BeTrue();
        }

        [Fact, Order(3)]
        public async Task Index_DoesntRedirect_WhenHasArtistProfile()
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Action action = () => Wait.Until(d => d.Url.Contains("Setup"));
                action.Should().Throw<WebDriverTimeoutException>();
        }

        [Fact, Order(4)]
        public async Task Setup_RedirectsToIndex_WhenUserHasArtistProfile()
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Setup");

                Wait.Until(d => d.Url.Contains("Setup") == false);
        }

        [Fact, Order(5)]
        public async Task Deactivate_RedirectsToIndex()
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");
                Driver.FindElement(By.Id("deactivate-artist-popup")).Click();

                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deactivate-artist"))).Click();

                Wait.Until(d => DriverIsAtBaseUrl()).Should().BeTrue();
        }

        [Fact, Order(6)]
        public async Task Index_RedirectsAgain_WhenArtistDeactivated()
        {
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/Index");

                Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

}
