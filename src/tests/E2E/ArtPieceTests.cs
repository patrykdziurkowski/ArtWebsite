using System;
using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.e2e.fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

[Collection("Web server collection")]
public class ArtPieceTests : IClassFixture<WebDriverBase>
{
        private readonly WebDriverBase _context;

        public ArtPieceTests(WebDriverBase context)
        {
                _context = context;
        }

        [Fact, Order(0)]
        public async Task UploadingArtPiece_RedirectsToIndex_WhenSuccess()
        {
                _context.WebServer.ClearData();
                await _context.RegisterAsync();
                await _context.LoginAsync();
                await _context.CreateArtistProfileAsync();
                string filePath = "../../../resources/exampleImage.png";
                await _context.Driver.Navigate().GoToUrlAsync("http://localhost/ArtPiece/Upload");
                IWebElement imageInput = _context.Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Image")));
                imageInput.SendKeys(Path.GetFullPath(filePath));
                _context.Driver.FindElement(By.Id("Description")).SendKeys("Description!");

                _context.Driver.FindElement(By.Id("upload-submit")).Click();

                _context.Wait.Until(d => d.Url.Equals("http://localhost/ArtPiece"));
        }
}
