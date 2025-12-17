using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class AdminTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        [Fact, Order(0)]
        public void ArtistProfileInfo_ShouldBeUpdated_WhenChangedByAdmin_WhoIsNotTheOwner()
        {
                ResetTestContext();

                CreateUserWithArtistProfile(
                        userName: "userName",
                        name: "artistName"
                );
                Logout();
                Login(
                        Environment.GetEnvironmentVariable("ROOT_EMAIL")!,
                        Environment.GetEnvironmentVariable("ROOT_PASSWORD")!
                );

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/artistName");

                Wait.Until(d => d.FindElement(By.Id("editProfileModalButton"))).Click();
                IWebElement submitButton = Wait.Until(ExpectedConditions
                        .ElementToBeClickable(By.Id("editArtistProfile")));
                IWebElement nameInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Name\"]"));
                nameInput.Clear();
                nameInput.SendKeys("NewName");
                IWebElement summaryInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Summary\"]"));
                summaryInput.Clear();
                summaryInput.SendKeys("New summary text!");
                submitButton.Click();

                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == "NewName").Should().BeTrue();
                Wait.Until(d => d.FindElement(By.Id("artistSummary")).Text == "New summary text!").Should().BeTrue();
        }
}
