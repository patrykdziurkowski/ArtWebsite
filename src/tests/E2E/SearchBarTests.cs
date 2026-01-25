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

                UploadArtPiece();
                UploadArtPiece("../../../resources/exampleImage2.png");

                var tags1 = GetTagsForArtPiece(artPieceIndex: 0);
                var tags2 = GetTagsForArtPiece(artPieceIndex: 1);

                var tagsUniqueForArtPiece1 = tags1.Except(tags2);
                var tagsUniqueForArtPiece2 = tags2.Except(tags1);

                Driver.FindElement(By.Id("tagSearchInput")).SendKeys(tagsUniqueForArtPiece1.First());
                Wait.Until(d => d.FindElement(By.CssSelector("#tagResults a"))).Click();
                Wait.Until(d => !string.IsNullOrEmpty(d.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src")));
                string imagePath1 = Driver.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src");

                Driver.FindElement(By.Id("tagSearchInput")).SendKeys(tagsUniqueForArtPiece2.First());
                Wait.Until(d => d.FindElement(By.CssSelector("#tagResults a"))).Click();
                Wait.Until(d => !string.IsNullOrEmpty(d.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src")));
                string imagePath2 = Driver.FindElement(By.CssSelector("#artContainer img")).GetAttribute("src");

                imagePath1.Should().NotBe(imagePath2);
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
