using FluentAssertions;
using OpenQA.Selenium;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class ArtPiecesTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        [Fact, Order(0)]
        public void UploadingArtPiece_Fails_WhenUploadedNonImageFile()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                string filePath = "../../../resources/exampleNonImage.txt";
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/ArtPiece/Upload");
                Driver.FindElement(By.Id("image-input")).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                Driver.FindElement(By.Id("upload-submit")).Click();

                Wait.Until(d => d.FindElement(By.ClassName("field-validation-error"))).Should().NotBeNull();
        }

        [Fact, Order(1)]
        public void UploadingArtPiece_AddsNewArtPiece_WhenUploadingImage()
        {
                UploadArtPiece();

                Wait.Until(d => d.FindElement(By.Id("artPieceImage"))).Should().NotBeNull();
        }

}
