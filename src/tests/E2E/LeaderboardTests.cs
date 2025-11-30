using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class LeaderboardTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        [Fact, Order(0)]
        public void CreatingUserWithArtistProfile_ShowsThatUserInBothLeaderboards()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 1)
                        .Should().BeTrue();
                var artistName = Driver.FindElement(By.CssSelector("#leaderboard-body > tr > td:nth-child(2)")).Text;
                artistName.Should().Be("SomeArtist");

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 1)
                        .Should().BeTrue();
                Wait.Until(d => d.FindElement(By.CssSelector("#leaderboard-body > tr > td:nth-child(2)")).Text == "SomeUser123")
                        .Should().BeTrue();
        }

        [Fact, Order(1)]
        public void CreatingMoreUsersWithAristProfiles_ShowsMoreInBothLeaderboards()
        {
                ResetTestContext();

                const int ROWS_PER_QUERY = 20;
                for (int i = 0; i < ROWS_PER_QUERY + 1; i++)
                {
                        CreateUserWithArtistProfile(
                                userName: $"SomeUser{i}",
                                name: $"SomeArtist{i}",
                                email: $"some{i}@email.com");
                        if (i != ROWS_PER_QUERY)
                        {
                                Logout();
                        }
                }

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 20)
                        .Should().BeTrue();
                Driver.FindElement(By.Id("load-more-leaderboard")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 21)
                        .Should().BeTrue();

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 20)
                        .Should().BeTrue();
                Driver.FindElement(By.Id("load-more-leaderboard")).Click();
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 21)
                        .Should().BeTrue();
        }

        [Fact, Order(2)]
        public void Leaderboard_SortsReviewersAndArtistsByPoints()
        {
                ResetTestContext();

                CreateUserWithArtistProfile(
                        userName: $"FewPointsReviewer",
                        name: $"ManyPointsArtist",
                        email: $"fewPoints@email.com");
                UploadArtPiece();

                CreateUserWithArtistProfile(
                        userName: $"ManyPointsReviewer",
                        name: $"FewPointsArtist",
                        email: $"manyPoints@email.com");
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                ReviewThisArtPiece();

                Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d =>
                {
                        var artistRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var artistNames = artistRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(2)")).Text).ToList();
                        if (artistNames.Count != 2)
                        {
                                return false;
                        }

                        return artistNames[0] == "ManyPointsArtist" && artistNames[1] == "FewPointsArtist";
                }).Should().BeTrue();

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                Wait.Until(d =>
                {
                        var reviewerRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var reviewerNames = reviewerRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(2)")).Text).ToList();
                        if (reviewerNames.Count != 2)
                        {
                                return false;
                        }

                        return reviewerNames[0] == "ManyPointsReviewer" && reviewerNames[1] == "FewPointsReviewer";
                }).Should().BeTrue();
        }

        [Fact, Order(3)]
        public void FilteringLeaderboardByTime_DoesntShowPointsAcquiredOutsideThatGivenTimePeriod()
        {
                ResetTestContext();
                CreateUserWithArtistProfile();
                UploadArtPiece();
                UploadArtPiece();
                UploadArtPiece();
                ReviewThisArtPiece();
                ReviewThisArtPiece();
                ReviewThisArtPiece();

                WebServer.ExecuteSql(@"
                        UPDATE TOP (1) [dbo].[ArtistPointAwards]
                        SET DateAwarded = '1970-01-01'
                ");
                WebServer.ExecuteSql(@"
                        UPDATE TOP (1) [dbo].[ReviewerPointAwards]
                        SET DateAwarded = '1970-01-01'
                ");

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d =>
                {
                        var artistRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var artistPoints = artistRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text).ToList();
                        if (artistPoints.Count != 1)
                        {
                                return false;
                        }

                        return artistPoints.Single() == "30";
                }).Should().BeTrue();

                Driver.FindElement(By.CssSelector(".time-btn[data-days=\"365\"]")).Click();
                Wait.Until(d =>
                {
                        var artistRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var artistPoints = artistRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text).ToList();
                        if (artistPoints.Count != 1)
                        {
                                return false;
                        }

                        return artistPoints.Single() == "20";
                }).Should().BeTrue();

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                Wait.Until(d =>
                {
                        var reviewerRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var reviewerPoints = reviewerRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text).ToList();
                        if (reviewerPoints.Count != 1)
                        {
                                return false;
                        }

                        return reviewerPoints.Single() == "30";
                }).Should().BeTrue();

                Driver.FindElement(By.CssSelector(".time-btn[data-days=\"365\"]")).Click();
                Wait.Until(d =>
                {
                        var reviewerRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var reviewerPoints = reviewerRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text).ToList();
                        if (reviewerPoints.Count != 1)
                        {
                                return false;
                        }

                        return reviewerPoints.Single() == "20";
                }).Should().BeTrue();
        }
}
