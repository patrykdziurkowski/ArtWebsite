using System.Collections.ObjectModel;
using FluentAssertions;
using OpenQA.Selenium;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class LikeTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        private static string _likedArtPieceSrc = string.Empty;

        [Fact, Order(0)]
        public void LikingArtPiece_ShouldShowButtonAsUnlike_EvenAfterRefreshing()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece");
                IWebElement likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                likeButton.Text.Should().Be("Like");
                likeButton.Click();
                Wait.Until(d => likeButton.Text == "Unlike");
                likeButton.Text.Should().Be("Unlike");
                IWebElement image = Wait.Until(d => d.FindElement(By.CssSelector("#artContainer img")));
                _likedArtPieceSrc = image.GetDomAttribute("src");

                Driver.Navigate().Refresh();
                likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                Wait.Until(d => likeButton.Text == "Unlike");
                likeButton.Text.Should().Be("Unlike");
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
        public void UnlikingArtPiece_ShouldShowButtonAsLike_EvenAfterRefreshing()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece");
                IWebElement likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                likeButton.Text.Should().Be("Unlike");
                likeButton.Click();
                Wait.Until(d => likeButton.Text == "Like");
                likeButton.Text.Should().Be("Like");

                Driver.Navigate().Refresh();
                likeButton = Wait.Until(d => d.FindElement(By.Id("likeArtPiece")));
                likeButton.Text.Should().Be("Like");
        }

        [Fact, Order(3)]
        public void UnlikedArtPiece_ShouldNotBeVisibleInReviewerProfile()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                Wait.Until(d => d.FindElements(By.CssSelector("#likesList > *")).Count >= 0);
                ReadOnlyCollection<IWebElement> images = Wait.Until(d => d.FindElements(By.CssSelector("#likesList img")));
                images.Should().BeEmpty();
        }
}
