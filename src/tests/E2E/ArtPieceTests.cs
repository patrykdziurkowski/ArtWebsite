using FluentAssertions;
using OpenQA.Selenium;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.e2e;

[Collection("Web server collection")]
public class ArtPieceTests : IClassFixture<WebDriverBase>
{
        private readonly WebDriverBase _context;

        public ArtPieceTests(WebDriverBase context)
        {
                _context = context;
        }

        [Fact, Order(0)]
        public async Task UploadingArtPiece_AddsNewArtPiece_WhenSuccess()
        {
                _context.WebServer.ClearData();
                await _context.RegisterAsync();
                await _context.LoginAsync();
                await _context.CreateArtistProfileAsync();
                string filePath = "../../../resources/exampleImage.png";
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/ArtPiece/Upload");
                _context.Driver.Navigate().GoToUrl("http://localhost/ArtPiece/Upload"); // this second GoToUrl needs to be here, for some reason...
                _context.Wait.Until(d => d.FindElement(By.Id("image-input"))).SendKeys(Path.GetFullPath(filePath));
                _context.Wait.Until(d => d.FindElement(By.Id("description-input"))).SendKeys("Description!");

                _context.Driver.FindElement(By.Id("upload-submit")).Click();

                _context.Wait.Until(d => d.Url.Equals("http://localhost/ArtPiece"));
                _context.Driver.FindElements(By.TagName("img")).Should().HaveCount(1);
        }
}
