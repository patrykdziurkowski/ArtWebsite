using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class UserDetailsTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void ViewProfile_NavigatesToArtistsProfile_WhenClickedOnArtist()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                ReviewThisArtPieceThenLoadNext();
                Logout();
                Login(WebServer.ROOT_TEST_EMAIL, WebServer.ROOT_TEST_PASSWORD);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewers/SomeUser123");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.Id("art-piece-details-artist"))).Click();
                Wait.Until(d => d.FindElement(By.Id("user-details-view-profile"))).Click();

                Wait.Until(d => d.Url.Contains("/Artists/SomeArtist"));
        }

        [Fact, Order(1)]
        public void ViewProfile_NavigatesToReviewersProfile_WhenClickedOnReviewer()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                ReviewThisArtPieceThenLoadNext();
                Logout();
                Login(WebServer.ROOT_TEST_EMAIL, WebServer.ROOT_TEST_PASSWORD);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/SomeArtist");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.ClassName("review-reviewer-name"))).Click();
                Wait.Until(d => d.FindElement(By.Id("user-details-view-profile"))).Click();

                Wait.Until(d => d.Url.Contains("/Reviewers/SomeUser123"));
        }
}
