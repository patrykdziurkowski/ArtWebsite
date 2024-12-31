using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

[Collection("Web server collection")]
public class ArtistTests : IClassFixture<WebDriverBase>
{
        private readonly WebDriverBase _context;
        public ArtistTests(WebDriverBase context)
        {
                _context = context;
        }

        [Fact, Order(0)]
        public async Task Index_RedirectsToLogin_WhenNotLoggedIn()
        {
                _context.WebServer.ClearData();

                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");

                _context.Wait.Until(d => d.Url.Contains("Login")).Should().BeTrue();
        }

        [Fact, Order(1)]
        public async Task Index_RedirectsToSetup_WhenNoArtistProfile()
        {
                await _context.RegisterAsync();
                await _context.LoginAsync();
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");

                _context.Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Setup_RedirectsToIndex_WhenSuccess()
        {
                await _context.CreateArtistProfileAsync();

                _context.Wait.Until(d => d.Url.Contains("/Artist/Index") || d.Url.EndsWith("/Artist/") || d.Url.EndsWith("/Artist")).Should().BeTrue();
        }

        [Fact, Order(3)]
        public async Task Index_DoesntRedirect_WhenHasArtistProfile()
        {
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");

                Action action = () => _context.Wait.Until(d => d.Url.Contains("Setup"));
                action.Should().Throw<WebDriverTimeoutException>();
        }

        [Fact, Order(4)]
        public async Task Setup_RedirectsToIndex_WhenUserHasArtistProfile()
        {
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Setup");

                _context.Wait.Until(d => d.Url.Contains("Setup") == false);
        }

        [Fact, Order(5)]
        public async Task Deactivate_RedirectsToIndex()
        {
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");
                _context.Driver.FindElement(By.Id("deactivate-artist-popup")).Click();

                _context.Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deactivate-artist"))).Click();

                _context.Wait.Until(d => _context.DriverIsAtBaseUrl()).Should().BeTrue();
        }

        [Fact, Order(6)]
        public async Task Index_RedirectsAgain_WhenArtistDeactivated()
        {
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");

                _context.Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

}
