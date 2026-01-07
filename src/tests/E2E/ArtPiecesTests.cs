using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ArtPiecesTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void UploadingArtPiece_Fails_WhenUploadedNonImageFile()
        {
                CreateUserWithArtistProfile();
                string filePath = "../../../resources/exampleNonImage.txt";
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece/Upload");
                Wait.Until(d => d.FindElement(By.Id("image-input"))).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                var submit = Driver.FindElement(By.Id("upload-submit"));
                ScrollIntoView(submit);
                submit.Click();

                Wait.Until(d => d.FindElement(By.ClassName("field-validation-error"))).Should().NotBeNull();
        }

        [Fact, Order(1)]
        public void UploadingArtPiece_AddsNewArtPieceWithTags_WhenUploadingImage()
        {
                UploadArtPiece();

                Wait.Until(d => d.FindElement(By.Id("artPieceImage"))).Should().NotBeNull();

                TimeSpan previousTimeout = Wait.Timeout;
                Wait.Timeout = TimeSpan.FromMinutes(1);
                Wait.Until(d => d.FindElements(By.ClassName("tag")).Count > 0);
                Wait.Timeout = previousTimeout;
        }

        [Fact, Order(2)]
        public void Tags_ShouldLoad_WhenPageRefreshed()
        {
                Driver.Navigate().Refresh();

                TimeSpan previousTimeout = Wait.Timeout;
                Wait.Timeout = TimeSpan.FromMinutes(1);
                Wait.Until(d => d.FindElements(By.ClassName("tag")).Count > 0);
                Wait.Timeout = previousTimeout;
        }

        [Fact, Order(3)]
        public void Tags_WhenClicked_ShowsArtPiecesThatHaveThisTag()
        {
                var tag = Driver.FindElement(By.ClassName("tag"));
                string tagName = tag.Text;
                tag.Click();

                Wait.Until(d => d.FindElement(By.Id("selectedTag")).Text == tagName);
        }

        [Fact, Order(4)]
        public void Updating_OwnArtPiece_ChangesIt()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist/");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.Id("edit-art-piece-button"))).Click();
                var textArea = Wait.Until(d => d.FindElement(By.Id("edit-art-piece-description")));
                textArea.Clear();
                textArea.SendKeys("This is a brand new art piece description");
                Driver.FindElement(By.Id("edit-art-piece-save")).Click();

                Wait.Until(d => d.FindElement(By.Id("detailsDescription")).Text == "This is a brand new art piece description");

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));
                Wait.Until(d => d.FindElement(By.Id("detailsDescription")).Text == "This is a brand new art piece description");
        }

}
