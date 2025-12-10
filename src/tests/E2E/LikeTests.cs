using System.Collections.ObjectModel;
using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class LikeTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        private static string _likedArtPieceSrc = string.Empty;

        [Fact, Order(0)]
        public void LikingArtPiece_ShouldShowButtonAsUnlike_WhenLiked()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                ReviewThenLikeThisArtPiece();

                IWebElement image = Wait.Until(d => d.FindElement(By.CssSelector("#artContainer img")));
                _likedArtPieceSrc = image.GetDomAttribute("src");
        }

        [Fact, Order(1)]
        public void LikedArtPiece_ShouldBeVisibleInReviewerProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElements(By.CssSelector("#likesList > div")).Count >= 1);
                ReadOnlyCollection<IWebElement> images = Wait.Until(d => d.FindElements(By.CssSelector("#likesList img")));
                images.Should().HaveCount(1);

                string srcOnReviewerProfile = images.Single().GetDomAttribute("src");
                srcOnReviewerProfile.Should().Be(_likedArtPieceSrc);
        }

        [Fact, Order(2)]
        public void LikingThenUnlikingArtPiece_ShouldNotShowItAsLiked_InVisibleReviewerProfile()
        {
                const int PREVIOUS_LIKES_COUNT = 1;
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                Wait.Until(d => d.FindElement(By.Id("reviewArt"))).Click();
                Wait.Until(ExpectedConditions.ElementToBeClickable(Driver.FindElement(By.Id("reviewForm"))));

                Driver.FindElement(By.CssSelector("#reviewForm textarea")).SendKeys("Review text! One that is long enough for the validation to pass. One that is long enough for the validation to pass.");
                Driver.FindElement(By.CssSelector("#reviewForm label[for=\"star3\"]")).Click();
                Driver.FindElement(By.CssSelector("#reviewForm button")).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("postReviewModal")));
                IWebElement likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                likeButton.Text.Should().Be("Like");

                likeButton.Click();
                Wait.Until(d => likeButton.Text == "Unlike");

                likeButton.Click();
                Wait.Until(d => likeButton.Text == "Like");

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                var foundLikedArtPieces = Wait.Until(d => d.FindElements(By.CssSelector("#likesList > div")));
                foundLikedArtPieces.Should().HaveCount(PREVIOUS_LIKES_COUNT);
        }
}
