using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ReviewsTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        [Fact, Order(0)]
        public void ReviewingArtPiece_ChangesArtPiece_WhenReviewed()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece");
                string imagePathBeforeReview = Wait.Until(d => d.FindElement(By.Id("artPieceImage"))
                        .GetDomAttribute("src"));
                Driver.FindElement(By.Id("reviewArt")).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(
                        Driver.FindElement(By.Id("reviewForm"))));

                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("reviewForm")));
                string imagePathAfterReview = Driver.FindElement(By.Id("artPieceImage")).GetDomAttribute("src");
                imagePathAfterReview.Should().NotBe(imagePathBeforeReview);
        }

        [Fact, Order(1)]
        public void ReviewerProfile_ShowsReviews_WhenReviewed()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");

                bool hasOneReview = Wait.Until(d => d.FindElements(By.CssSelector("#reviewsList > *")).Count == 1);
                hasOneReview.Should().BeTrue();
                Driver.FindElements(By.ClassName("checked-star")).Should().HaveCount(3);
        }

        [Fact, Order(2)]
        public async Task LikingArtPiece_ShowsLikeOnReviewerProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece");

                Wait.Until(d => d.FindElement(By.Id("likeArtPiece"))).Click();
                await Task.Delay(1000);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElement(By.CssSelector("#reviewerTabs button[data-bs-target='#likesTab']"))).Click();
                bool hasOneLike = Wait.Until(d => d.FindElements(By.CssSelector("#likesList > * ")).Count == 1);
                hasOneLike.Should().BeTrue();
        }

}
