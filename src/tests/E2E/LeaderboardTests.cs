using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class LeaderboardTests(WebDriverInitializer initializer, SharedPerTestClass shared)
        : WebDriverBase(initializer, shared)
{
        [Fact, Order(0)]
        public void CreatingUserWithArtistProfile_ShowsThatUserInBothLeaderboards()
        {
                CreateUserWithArtistProfile();
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 1)
                        .Should().BeTrue();
                var artistName = Driver.FindElement(By.CssSelector("#leaderboard-body > tr > td:nth-child(2)")).Text;
                artistName.Should().Be("SomeArtist");

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                // includes existing admin
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 2)
                        .Should().BeTrue();
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr > td:nth-child(2)"))
                        .Any(e => e.Text == "SomeUser123")).Should().BeTrue();
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
                // 21 users + 1 existing admin 
                Wait.Until(d => d.FindElements(By.CssSelector("#leaderboard-body > tr")).Count == 22)
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
                ReviewThisArtPieceThenLoadNext();

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
                        // includes 1 existing admin
                        if (reviewerNames.Count != 3)
                        {
                                return false;
                        }

                        return reviewerNames.Any(name => name == "ManyPointsReviewer")
                                && reviewerNames.Any(name => name == "FewPointsReviewer");
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
                ReviewThisArtPieceThenLoadNext();
                ReviewThisArtPieceThenLoadNext();
                ReviewThisArtPieceThenLoadNext();

                WebServer.ExecuteSql(@"
                        UPDATE TOP (1) [dbo].[ArtistPointAwards]
                        SET DateAwarded = '1970-01-01'
                ");
                WebServer.ExecuteSql(@"
                        UPDATE TOP (1) [dbo].[ReviewerPointAwards]
                        SET DateAwarded = '1970-01-01'
                ");

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                int artistPointsAllTime = GetLeaderboardPointsForNonAdminUser("artists", "null");

                Driver.FindElement(By.CssSelector(".time-btn[data-days=\"365\"]")).Click();
                int artistPointsThisYear = GetLeaderboardPointsForNonAdminUser("artists", "365");

                Driver.FindElement(By.Id("btn-reviewers")).Click();
                int reviewerPointsAllTime = GetLeaderboardPointsForNonAdminUser("reviewers", "null");

                Driver.FindElement(By.CssSelector(".time-btn[data-days=\"365\"]")).Click();
                int reviewerPointsThisYear = GetLeaderboardPointsForNonAdminUser("reviewers", "365");

                (artistPointsAllTime - artistPointsThisYear).Should().Be(10);
                (reviewerPointsAllTime - reviewerPointsThisYear).Should().Be(10);
        }

        private int GetLeaderboardPointsForNonAdminUser(string mode, string days)
        {
                Wait.Until(d => d.FindElement(By.Id("leaderboard")).GetDomAttribute("data-mode") == mode);
                Wait.Until(d => d.FindElement(By.Id("leaderboard")).GetDomAttribute("data-days") == days);

                int? pointsValue = null;
                Wait.Until(d =>
                {
                        var rows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        List<int?> points = [.. rows
                                .Where(r => r.FindElement(By.CssSelector("td:nth-child(2)")).Text != WebServer.ROOT_TEST_USERNAME)
                                .Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text)
                                .Select(r => int.Parse(r))];

                        pointsValue = points.SingleOrDefault();
                        return points.Count == 1;
                });

                return pointsValue!.Value;
        }
}
