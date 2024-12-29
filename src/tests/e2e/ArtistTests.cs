using FluentAssertions;
using OpenQA.Selenium;
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
                await _context.Register();
                await _context.Login();
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Index");

                _context.Wait.Until(d => d.Url.Contains("Setup")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Setup_RedirectsToIndex_WhenSuccess()
        {
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Artist/Setup");

                _context.Driver.FindElement(By.Id("Name")).SendKeys("SomeArtist");
                _context.Driver.FindElement(By.Id("Summary")).SendKeys("My description!");
                _context.Driver.FindElement(By.Id("setup-artist")).Click();

                _context.Wait.Until(d => d.Url.Contains("/Artist/Index")).Should().BeTrue();
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

}
