using FluentAssertions;
using OpenQA.Selenium;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class SearchBarTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void TopSearchBar_AllowsSearchingForTags_WithFiltering()
        {
                CreateUserWithArtistProfile();
                CreateUserWithArtistProfile(
                        userName: "userUser",
                        email: "user@user.com",
                        name: "artistArtist"
                );

                UploadArtPiece();
                UploadArtPiece("../../../resources/exampleImage2.png");

                var tags1 = GetTagsForArtPiece(artPieceIndex: 0);
                var tags2 = GetTagsForArtPiece(artPieceIndex: 1);

                var tagsUniqueForArtPiece1 = tags1.Except(tags2);
                var tagsUniqueForArtPiece2 = tags2.Except(tags1);

                Driver.FindElement(By.ClassName("search-input")).SendKeys(tagsUniqueForArtPiece1.First());
                Wait.Until(d => d.FindElement(By.CssSelector(".search-results a"))).Click();
                Wait.Until(d => !string.IsNullOrEmpty(d.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src")));
                string imagePath1 = Driver.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src");

                Driver.FindElement(By.ClassName("search-input")).SendKeys(tagsUniqueForArtPiece2.First());
                Wait.Until(d => d.FindElement(By.CssSelector(".search-results a"))).Click();
                Wait.Until(d => !string.IsNullOrEmpty(d.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src")));
                string imagePath2 = Driver.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src");

                imagePath1.Should().NotBe(imagePath2);
        }

        [Fact, Order(1)]
        public void TopSearchBar_AllowsSearchingForArtists()
        {
                Driver.FindElement(By.ClassName("search-input")).SendKeys("r");
                Wait.Until(d =>
                {
                        var links = d.FindElements(By.CssSelector(".search-results a"));
                        bool allExpectedSearchResultsPresent = links.Any(l => l.Text == "SomeArtist")
                                && links.Any(l => l.Text == "SomeUser123")
                                && links.Any(l => l.Text == "artistArtist")
                                && links.Any(l => l.Text == "userUser");
                        return allExpectedSearchResultsPresent;
                });

                Wait.Until(d => d.FindElements(By.CssSelector(".search-results a"))
                        .SingleOrDefault(l => l.Text == "SomeArtist")).Click();
                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == "SomeArtist");
        }

        [Fact, Order(2)]
        public void TopSearchBar_AllowsSearchingForReviewers()
        {
                Driver.FindElement(By.ClassName("search-input")).SendKeys("r");

                Wait.Until(d => d.FindElements(By.CssSelector(".search-results a"))
                        .SingleOrDefault(l => l.Text == "userUser")).Click();
                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == "userUser");
        }

        private List<string> GetTagsForArtPiece(int artPieceIndex)
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist");
                Wait.Until(d => d.FindElements(By.ClassName("art-piece-card")).Count == 2);
                var artPieces = Driver.FindElements(By.ClassName("art-piece-card"));
                artPieces[artPieceIndex].Click();
                Wait.Until(d => d.FindElements(By.CssSelector(".tag")).Count > 0);
                var tags = Driver.FindElements(By.CssSelector(".tag")).Select(e => e.Text).ToList();
                Driver.FindElement(By.Id("details-back")).Click();
                return tags;

        }

}
