using FluentAssertions;
using OpenQA.Selenium;
using tests.e2e.fixtures;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

public class ArtPieceTests : WebDriverBase
{
        public ArtPieceTests(WebDriverInitializer initializer)
                : base(initializer)
        {
        }

        [Fact, Order(0)]
        public async Task UploadingArtPiece_Fails_WhenUploadedNonImageFile()
        {
                await ResetTestContextAsync();
                await RegisterAsync();
                await LoginAsync();
                await CreateArtistProfileAsync();
                string filePath = "../../../resources/exampleNonImage.txt";
                await Driver.Navigate().GoToUrlAsync("http://localhost/ArtPiece/Upload");
                Driver.Navigate().GoToUrl("http://localhost/ArtPiece/Upload"); // this second GoToUrl needs to be here, for some reason...
                Driver.FindElement(By.Id("image-input")).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                Driver.FindElement(By.Id("upload-submit")).Click();

                Wait.Until(d => d.FindElement(By.ClassName("field-validation-error"))).Should().NotBeNull();
        }

        [Fact, Order(1)]
        public void UploadingArtPiece_AddsNewArtPiece_WhenUploadingImage()
        {
                string filePath = "../../../resources/exampleImage.png";
                Driver.Navigate().GoToUrl("http://localhost/ArtPiece/Upload");
                Driver.FindElement(By.Id("image-input")).SendKeys(Path.GetFullPath(filePath));
                Driver.FindElement(By.Id("description-input")).SendKeys("Description!");

                Driver.FindElement(By.Id("upload-submit")).Click();

                Wait.Until(d => d.Url.Equals("http://localhost/ArtPiece"));
                Driver.FindElements(By.TagName("img")).Should().HaveCount(1);
        }
}
