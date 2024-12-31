using FluentAssertions;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

[Collection("Web server collection")]
public class AuthenticationTests : IClassFixture<WebDriverBase>
{
        private readonly WebDriverBase _context;
        public AuthenticationTests(WebDriverBase context)
        {
                _context = context;
        }

        [Fact, Order(0)]
        public async Task RegisterPage_Loads()
        {
                _context.WebServer.ClearData();
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/Identity/Account/Register");

                _context.Driver.Title.Should().Contain("Register");
        }

        [Fact, Order(1)]
        public async Task Register_RedirectsToEmailConfirmation_WhenSuccessful()
        {
                await _context.RegisterAsync();

                _context.Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email.")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Login_ShouldWork()
        {
                await _context.LoginAsync();

                _context.Wait.Until(d => d.PageSource.Contains("john@smith.com")).Should().BeTrue();
        }
}

