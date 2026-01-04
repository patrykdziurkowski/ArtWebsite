using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ReviewsTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void ReviewingArtPiece_ChangesArtPiece_WhenReviewed()
        {
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();
                string imagePathBeforeReview = Wait.Until(d => d.FindElement(By.Id("artPieceImage"))
                        .GetDomAttribute("src"));

                ReviewThisArtPieceThenLoadNext();

                Wait.Until(d =>
                {
                        string imagePathAfterReview = Driver.FindElement(By.Id("artPieceImage")).GetDomAttribute("src");
                        return imagePathAfterReview != imagePathBeforeReview;
                });
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
        public void Browsing_DetailsPage_ShowsOtherReviewersReviews_InOrderByPoints()
        {
                const int REVIEWERS_COUNT = 11;

                SetupArtPieceWithReviews(REVIEWERS_COUNT);

                Driver.FindElement(By.Id("viewArtPiece")).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));
                Driver.FindElement(By.Id("details-load-more-reviews")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#artReviewsContainer > *")).Count == REVIEWERS_COUNT).Should().BeTrue();
                var reviewElements = Driver.FindElements(By.CssSelector("#artReviewsContainer > *"));
                var reviewerPointsText = reviewElements.Select(r => r.FindElement(By.ClassName("review-points")).Text);

                int[] points = [.. reviewerPointsText.Select(text => int.Parse(text.Split(' ').First()))];
                int[] pointsOrderedDescending = [.. points.OrderDescending()];
                points.Should().BeEquivalentTo(pointsOrderedDescending);

                Driver.FindElement(By.Id("details-back")).Click();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("artPieceDetailsModal")));

                Logout();
        }

        [Fact, Order(5)]
        public void Reviewer_ArtPieceDetailsPage_ShowsOtherReviewersReviews_InOrderByPoints()
        {
                const int REVIEWERS_COUNT = 11;

                SetupArtPieceWithReviews(REVIEWERS_COUNT);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Driver.FindElement(By.Id("details-load-more-reviews")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#artReviewsContainer > *")).Count == REVIEWERS_COUNT).Should().BeTrue();

                var reviewElements = Driver.FindElements(By.CssSelector("#artReviewsContainer > *"));
                var reviewerPointsText = reviewElements.Select(r => r.FindElement(By.ClassName("review-points")).Text);

                int[] points = [.. reviewerPointsText.Select(text => int.Parse(text.Split(' ').First()))];
                int[] pointsOrderedDescending = [.. points.OrderDescending()];
                points.Should().BeEquivalentTo(pointsOrderedDescending);

                Driver.FindElement(By.Id("details-back")).Click();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("artPieceDetailsModal")));

                Logout();
        }

        [Fact, Order(6)]
        public void Artist_ArtPieceDetailsPage_ShowsOtherReviewersReviews_InOrderByPoints()
        {
                const int REVIEWERS_COUNT = 11;
                SetupArtPieceWithReviews(REVIEWERS_COUNT);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/ArtistWithArt");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Driver.FindElement(By.Id("details-load-more-reviews")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#artReviewsContainer > *")).Count == REVIEWERS_COUNT).Should().BeTrue();

                var reviewElements = Driver.FindElements(By.CssSelector("#artReviewsContainer > *"));
                var reviewerPointsText = reviewElements.Select(r => r.FindElement(By.ClassName("review-points")).Text);

                int[] points = [.. reviewerPointsText.Select(text => int.Parse(text.Split(' ').First()))];
                int[] pointsOrderedDescending = [.. points.OrderDescending()];
                points.Should().BeEquivalentTo(pointsOrderedDescending);

                Driver.FindElement(By.Id("details-back")).Click();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("artPieceDetailsModal")));

                Logout();
        }

        [Fact, Order(7)]
        public void Reviewer_DeletingOwnReview_MakesItDisappear()
        {
                ResetTestContext();
                CreateUserWithArtistProfile(name: "ArtistWithArt");
                UploadArtPiece();
                ReviewThisArtPieceThenLoadNext();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.ClassName("delete-review-button"))).Click();
                IAlert alert = Wait.Until(d => d.SwitchTo().Alert());
                alert.Accept();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("artPieceDetailsModal")));

                Driver.FindElements(By.ClassName(".art-piece-card")).Should().BeEmpty();
        }

        [Fact, Order(8)]
        public void UpdatingOwnReviewerName_Works()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElement(By.Id("editProfile"))).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("editProfileModal")));
                var nameInput = Driver.FindElement(By.Name("name"));
                nameInput.Clear();
                nameInput.SendKeys("myNewName");
                Driver.FindElement(By.Id("editReviewerProfile")).Click();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("editProfileModal")));
                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == "myNewName");

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == "myNewName");
        }

        [Fact, Order(9)]
        public void UpdatingOwnReview_UpdatesIt()
        {
                const string NEW_COMMENT = "This is a new review comment that was edited by an owner. This is a new review comment that was edited by an owner. This is a new review comment that was edited by an owner.";

                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                ReviewThisArtPieceThenLoadNext();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewers/SomeUser123");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.ClassName("edit-review-button"))).Click();
                Wait.Until(d => d.FindElement(By.CssSelector("label[for=\"star1\"]"))).Click();
                var commentEdit = Wait.Until(d => d.FindElement(By.Id("editComment")));
                commentEdit.Clear();
                commentEdit.SendKeys(NEW_COMMENT);
                Driver.FindElement(By.Id("submitEdit")).Click();
                Wait.Until(d => d.FindElement(By.ClassName("review-comment")).Text == NEW_COMMENT);
                Driver.FindElement(By.ClassName("review-rating")).Text.Should().Be("1");

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card")).Text.Contains(NEW_COMMENT));
                Driver.FindElements(By.ClassName("checked-star")).Should().HaveCount(1);
        }

        private void SetupArtPieceWithReviews(int reviewCount)
        {
                ResetTestContext();
                CreateUserWithArtistProfile(name: "ArtistWithArt");
                UploadArtPiece();
                Logout();

                for (int i = 0; i < reviewCount; i++)
                {
                        CreateUserWithArtistProfile(
                                userName: $"userName{i}",
                                email: $"email{i}@someEmail.com",
                                name: $"someArtist{i}"
                        );
                        ReviewRandomArtPiece();

                        if (i == reviewCount - 1)
                        {
                                break;
                        }

                        Logout();
                }
        }

}
