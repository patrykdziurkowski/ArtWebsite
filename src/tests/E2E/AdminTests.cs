using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class AdminTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void ArtistProfileInfo_ShouldBeUpdated_WhenChangedByAdmin_WhoIsNotTheOwner()
        {
                CreateUserWithArtistProfile(
                        userName: "userName",
                        name: "artistName"
                );
                Logout();
                Login(WebServer.ROOT_TEST_EMAIL, WebServer.ROOT_TEST_PASSWORD);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/artistName");

                Wait.Until(d => d.FindElement(By.Id("editProfileModalButton"))).Click();
                IWebElement submitButton = Wait.Until(ExpectedConditions
                        .ElementToBeClickable(By.Id("editArtistProfile")));
                IWebElement nameInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Name\"]"));
                nameInput.Clear();
                nameInput.SendKeys("NewName");
                IWebElement summaryInput = Driver.FindElement(By.CssSelector("#editForm input[name=\"Summary\"]"));
                summaryInput.Clear();
                summaryInput.SendKeys("New summary text!");
                submitButton.Click();

                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == "NewName").Should().BeTrue();
                Wait.Until(d => d.FindElement(By.Id("artistSummary")).Text == "New summary text!").Should().BeTrue();
        }

        [Fact, Order(1)]
        public void DeactivatingSomeoneElsesArtistProfile_ShouldWork()
        {
                Driver.FindElement(By.Id("deactivate-artist-popup")).Click();
                IWebElement submitButton = Wait.Until(ExpectedConditions
                        .ElementToBeClickable(By.Id("deactivate-artist")));
                submitButton.Click();

                Driver.Navigate().Refresh();

                Wait.Until(d => d.Url.Contains("/Artist/Setup"));

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/artistName");
                Wait.Until(d => d.PageSource.Contains("404"));
        }

        [Fact, Order(2)]
        public void Deleting_SomeoneElsesReview_DeletesIt()
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

                Wait.Until(d => d.FindElement(By.ClassName("delete-review-button"))).Click();
                IAlert alert = Wait.Until(d => d.SwitchTo().Alert());
                alert.Accept();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("artPieceDetailsModal")));

                Driver.FindElements(By.ClassName(".art-piece-card")).Should().BeEmpty();
        }

        [Fact, Order(3)]
        public void Updating_SomeoneElsesReviewerName_Works()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewers/SomeUser123");
                Wait.Until(d => d.FindElement(By.Id("editProfile"))).Click();

                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("editProfileModal")));
                var nameInput = Driver.FindElement(By.Name("name"));
                nameInput.Clear();
                nameInput.SendKeys("newName");
                Driver.FindElement(By.Id("editReviewerProfile")).Click();
                Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("editProfileModal")));
                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == "newName");

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == "newName");
        }

        [Fact, Order(4)]
        public void Updating_SomeoneElsesReview_UpdatesIt()
        {
                const string NEW_COMMENT = "This is a new review comment that was edited by an admin. This is a new review comment that was edited by an admin. This is a new review comment that was edited by an admin.";

                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                ReviewThisArtPieceThenLoadNext();
                Logout();
                Login(WebServer.ROOT_TEST_EMAIL, WebServer.ROOT_TEST_PASSWORD);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewers/SomeUser123");
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card"))).Click();
                Wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("artPieceDetailsModal")));

                Wait.Until(d => d.FindElement(By.ClassName("edit-review-button"))).Click();
                Wait.Until(d => d.FindElement(By.CssSelector("label[for=\"star1\"]"))).Click();
                var commentEdit = Wait.Until(d => d.FindElement(By.Id("editComment")));
                commentEdit.Clear();
                commentEdit.SendKeys(NEW_COMMENT);
                Driver.FindElement(By.Id("submitEdit")).Click();
                Wait.Until(d => d.FindElement(By.ClassName("review-comment")).Text == NEW_COMMENT);
                Driver.FindElement(By.ClassName("review-rating")).Text.Should().Be("1");

                Driver.Navigate().Refresh();
                Wait.Until(d => d.FindElement(By.CssSelector(".art-piece-card")).Text.Contains(NEW_COMMENT));
                Driver.FindElements(By.ClassName("checked-star")).Should().HaveCount(1);
        }

        [Fact, Order(5)]
        public void Updating_SomeoneElsesArtPiece_ChangesIt()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/SomeArtist");
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
