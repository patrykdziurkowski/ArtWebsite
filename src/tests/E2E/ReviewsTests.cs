using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ReviewsTests : WebDriverBase
{
        public ReviewsTests(WebDriverInitializer initializer)
                : base(initializer)
        {
        }

        [Fact, Order(0)]
        public async Task ReviewingArtPiece_ChangesArtPiece_WhenReviewed()
        {
                await ResetTestContextAsync();
                await RegisterAsync();
                await LoginAsync();
                await CreateArtistProfileAsync();
                await UploadArtPiece();
                await UploadArtPiece();
                await Driver.Navigate().GoToUrlAsync($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece");
                string imagePathBeforeReview = Wait.Until(d => d.FindElement(By.Id("artPieceImage"))
                        .GetDomAttribute("src"));
                Driver.FindElement(By.Id("reviewArt")).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(
                        Driver.FindElement(By.CssSelector("#reviewForm"))));
                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");

                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("reviewForm")));
                string imagePathAfterReview = Driver.FindElement(By.Id("artPieceImage")).GetDomAttribute("src");
                imagePathAfterReview.Should().NotBe(imagePathBeforeReview);
        }

}
