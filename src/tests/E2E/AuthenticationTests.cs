using FluentAssertions;
using OpenQA.Selenium;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class AuthenticationTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        [Fact, Order(0)]
        public void RegisterPage_Loads()
        {
                ResetTestContext();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Register");

                Driver.Title.Should().Contain("Register");
        }

        [Fact, Order(1)]
        public void Register_RedirectsToEmailConfirmation_WhenSuccessful()
        {
                Register();

                Wait.Until(d => d.PageSource.Contains("Thank you for confirming your email.")).Should().BeTrue();
        }

        [Fact, Order(2)]
        public void Login_ShouldWork()
        {
                Login();

                Wait.Until(d => d.FindElement(By.CssSelector("#logout"))).Should().NotBeNull();
        }
}

