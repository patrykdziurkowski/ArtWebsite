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
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                string imagePathBeforeReview = Wait.Until(d => d.FindElement(By.Id("artPieceImage"))
                        .GetDomAttribute("src"));
                Driver.FindElement(By.Id("reviewArt")).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(
                        Driver.FindElement(By.Id("reviewForm"))));

                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("postReviewModal")));
                Wait.Until(d => d.FindElement(By.CssSelector("#postReviewModal .btn-close"))).Click();

                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("postReviewModal")));
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
                ReviewRandomArtPiece();
                Wait.Until(d => d.FindElement(By.Id("likeArtPiece"))).Click();
                await Task.Delay(1000);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElement(By.CssSelector("#reviewerTabs button[data-bs-target='#likesTab']"))).Click();
                bool hasOneLike = Wait.Until(d => d.FindElements(By.CssSelector("#likesList > * ")).Count == 1);
                hasOneLike.Should().BeTrue();
        }

        [Fact, Order(3)]
        public void ReviewingArtPieceAgain_ShowsNoArtPiecesLeft_WhenAllHaveBeenReviewed()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                Wait.Until(d => d.FindElement(By.XPath("//*[text()='No more images to review.']"))).Should().NotBeNull();
        }

        [Fact, Order(4)]
        public void DetailsPage_ShowsOtherReviewersReviews_WhenPresent()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                Logout();

                const int REVIEWERS_COUNT = 11;
                const int LAST_REVIEWER = REVIEWERS_COUNT;
                for (int i = 1; i <= REVIEWERS_COUNT; i++)
                {
                        CreateUserWithArtistProfile(
                                userName: $"userName{i}",
                                email: $"email{i}@someEmail.com",
                                name: $"someArtist{i}"
                        );

                        ReviewRandomArtPiece();

                        if (i != LAST_REVIEWER)
                        {
                                Wait.Until(d => d.FindElement(By.CssSelector("#postReviewModal .btn-close"))).Click();
                        }
                        else
                        {
                                int totalReviewsOnThisArtPiece = REVIEWERS_COUNT;
                                Driver.FindElement(By.Id("viewArtPiece")).Click();
                                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));
                                Driver.FindElement(By.Id("loadMoreReviews")).Click();
                                Wait.Until(d => d.FindElements(By.CssSelector("#artReviewsContainer > *")).Count == totalReviewsOnThisArtPiece).Should().BeTrue();
                        }

                        Logout();
                }
        }

        [Fact, Order(5)]
        public void DetailsPage_ShowsOtherReviewersReviews_InOrderByPoints()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();
                Logout();

                const int REVIEWERS_COUNT = 3;
                const int LAST_REVIEWER = REVIEWERS_COUNT;
                for (int i = 1; i <= REVIEWERS_COUNT; i++)
                {
                        CreateUserWithArtistProfile(
                                userName: $"userName{i}",
                                email: $"email{i}@someEmail.com",
                                name: $"someArtist{i}"
                        );
                        ReviewRandomArtPiece();

                        if (i != LAST_REVIEWER)
                        {
                                ReviewRandomArtPiece();
                        }
                        else
                        {
                                int totalReviewsOnThisArtPiece = REVIEWERS_COUNT;
                                Driver.FindElement(By.Id("viewArtPiece")).Click();
                                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));
                                Wait.Until(d => d.FindElements(By.CssSelector("#artReviewsContainer > *")).Count == totalReviewsOnThisArtPiece).Should().BeTrue();
                                var reviewElements = Driver.FindElements(By.CssSelector("#artReviewsContainer > *"));
                                var reviewerPointsText = reviewElements.Select(r => r.FindElement(By.ClassName("review-points")).Text);

                                int[] points = [.. reviewerPointsText.Select(text => int.Parse(text.Split(' ').First()))];
                                int[] pointsOrderedDescending = [.. points.OrderDescending()];
                                points.Should().BeEquivalentTo(pointsOrderedDescending);
                        }

                        Logout();
                }
        }

        private void ReviewRandomArtPiece()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                Driver.FindElement(By.Id("reviewArt")).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(Driver.FindElement(By.Id("reviewForm"))));

                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("postReviewModal")));
        }
}
