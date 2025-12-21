using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class BoostsTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void BoostingArtPiece_ShouldShowBoostedArtPiece_OnArtistProfile()
        {
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist");
                // click the second art piece's boost button since it should come later in order
                // in the art browsing section when without boost
                Wait.Until(d => d.FindElements(By.CssSelector(".boost-btn")).Count >= 2);
                IWebElement secondBoostButton = Wait.Until(d => d.FindElements(By.CssSelector(".boost-btn")))[1];
                ScrollIntoView(secondBoostButton);
                Wait.Until(ExpectedConditions.ElementToBeClickable(secondBoostButton)).Click();

                Wait.Until(d => d.FindElement(By.CssSelector(".boosted-art-piece img"))).Should().NotBeNull();
                Action findingBoostButton = () => Driver.FindElement(By.CssSelector(".boost-btn"));
                findingBoostButton.Should().Throw<NoSuchElementException>();
        }

}
