using FluentAssertions;
using tests.e2e.fixtures;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

public class AuthenticationTests : WebDriverBase
{
        public AuthenticationTests(WebDriverInitializer initializer)
                : base(initializer)
        {
        }


        [Fact, Order(0)]
        public async Task RegisterPage_Loads()
        {
                await ResetTestContextAsync();

                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/Identity/Account/Register");

                Driver.Title.Should().Contain("Register");
        }

        [Fact, Order(1)]
        public async Task Register_RedirectsToEmailConfirmation_WhenSuccessful()
        {
                await RegisterAsync();

                Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email.")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public async Task Login_ShouldWork()
        {
                await LoginAsync();

                Wait.Until(d => d.PageSource.Contains("john@smith.com")).Should().BeTrue();
        }
}

