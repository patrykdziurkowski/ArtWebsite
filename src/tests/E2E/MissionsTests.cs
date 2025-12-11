using System.ComponentModel;
using FluentAssertions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using tests.E2E.Fixtures;
using web.Features.Missions;
using Xunit.Extensions.Ordering;

namespace tests.E2E;

public class MissionsTests(WebDriverInitializer initializer)
        : WebDriverBase(initializer)
{
        private static MissionType _initialMissionType;
        private static int _artistPoints = 0;
        private static int _reviewerPoints = 0;

        [Fact, Order(0)]
        public void MissionWidget_ShowsNoProgress_ForNewUser()
        {
                ResetTestContext();
                // set up other artist/reviewer profiles here to be used for missions later
                for (int i = 0; i < 5; i++)
                {
                        CreateUserWithArtistProfile(
                        userName: $"otherUser{i}",
                        email: $"otherEmail{i}@email.com",
                        name: $"otherArtist{i}");

                        Logout();
                }

                CreateUserWithArtistProfile(
                        userName: $"SomeUser",
                        email: $"someEmail@email.com",
                        name: $"someArtist"
                );

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                string missionDescription = Wait.Until(d => d.FindElement(By.Id("mission-description"))).Text;
                int missionProgress = int.Parse(Wait.Until(d => d.FindElement(By.Id("mission-progress"))).Text);
                int missionMaxProgress = int.Parse(Wait.Until(d => d.FindElement(By.Id("mission-max-progress"))).Text);
                MissionType missionType = MissionTypeHelpers.GetMissionForDescription(missionDescription);
                _initialMissionType = missionType;

                missionProgress.Should().Be(0);
                missionMaxProgress.Should().Be(missionType.GetMaxProgressCount());
        }

        [Fact, Order(1)]
        public void MissionWidget_ShowsPartialProgress_WhenProgressMade()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                string missionDescription = Wait.Until(d => d.FindElement(By.Id("mission-description"))).Text;
                MissionType missionType = MissionTypeHelpers.GetMissionForDescription(missionDescription);

                ProgressMission(missionType, times: 1);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d =>
                {
                        Driver.Navigate().Refresh();
                        return int.Parse(d.FindElement(By.Id("mission-progress")).Text) == 1;
                }).Should().BeTrue();
        }

        [Fact, Order(2)]
        public void MissionWidget_ShowsCompletedAndAwardsPoints_WhenFinished()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                string missionDescription = Wait.Until(d => d.FindElement(By.Id("mission-description"))).Text;
                MissionType missionType = MissionTypeHelpers.GetMissionForDescription(missionDescription);

                const int STEPS_ALREADY_PERFORMED = 1;
                int stepsToPerform = missionType.GetMaxProgressCount() - STEPS_ALREADY_PERFORMED;

                ProgressMission(missionType, times: stepsToPerform);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                Wait.Until(d =>
                {
                        Driver.Navigate().Refresh();
                        return d.FindElement(By.Id("mission-progress")).Text == missionType.GetMaxProgressCount().ToString();
                }).Should().BeTrue();

                int artistQuestPointValue = (missionType.GetRecipient() == MissionRecipient.Artist
                        || missionType.GetRecipient() == MissionRecipient.Both)
                        ? 25
                        : 0;
                int reviewerQuestPointValue = (missionType.GetRecipient() == MissionRecipient.Reviewer
                        || missionType.GetRecipient() == MissionRecipient.Both)
                        ? 25
                        : 0;
                Wait.Until(d =>
                {
                        var artistRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var artistPoints = artistRows
                                .Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text)
                                .Select(p => int.Parse(p))
                                .ToList();
                        if (artistPoints.Count != 6)
                        {
                                return false;
                        }

                        return artistPoints.Any(a => a == _artistPoints + artistQuestPointValue);
                }).Should().BeTrue();
                Driver.FindElement(By.Id("btn-reviewers")).Click();
                Wait.Until(d =>
                {
                        var reviewerRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var reviewerPoints = reviewerRows
                                .Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text)
                                .Select(p => int.Parse(p))
                                .ToList();
                        if (reviewerPoints.Count != 6)
                        {
                                return false;
                        }

                        return reviewerPoints.Any(a => a == _reviewerPoints + reviewerQuestPointValue);
                }).Should().BeTrue();
        }

        [Fact, Order(3)]
        public void MissionWidget_CreatingNewUser_ShowsDifferentMissionsForThem()
        {
                const int ITERATION_LIMIT = 1000;
                for (int i = 0; i < ITERATION_LIMIT; i++)
                {
                        Logout();
                        CreateUserWithArtistProfile(
                                userName: $"SomeUser{i}",
                                email: $"someEmail{i}@email.com",
                                name: $"someArtist{i}"
                        );

                        Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                        string missionDescription = Wait.Until(d => d.FindElement(By.Id("mission-description"))).Text;
                        int missionProgress = int.Parse(Wait.Until(d => d.FindElement(By.Id("mission-progress"))).Text);

                        missionProgress.Should().Be(0);
                        if (_initialMissionType.GetDescription() != missionDescription)
                        {
                                // success
                                break;
                        }

                        if (i == ITERATION_LIMIT - 1)
                        {
                                missionDescription.Should().Be(_initialMissionType.GetDescription());
                        }
                }
        }

        private void ProgressMission(MissionType missionType, int times)
        {
                for (int i = 0; i < times; i++)
                {
                        if (missionType == MissionType.UploadArt)
                        {
                                _artistPoints += 10;
                                UploadArtPiece();
                        }
                        else if (missionType == MissionType.BoostArt)
                        {
                                _artistPoints += 10;
                                UploadArtPiece();

                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artist");
                                Wait.Until(d => d.FindElements(By.CssSelector(".boost-btn")).Count >= 1);
                                IWebElement boostButton = Wait.Until(d => d.FindElements(By.CssSelector(".boost-btn"))).First();
                                ScrollIntoView(boostButton);
                                Wait.Until(ExpectedConditions.ElementToBeClickable(boostButton)).Click();
                        }
                        else if (missionType == MissionType.ReviewArt)
                        {
                                _artistPoints += 10;
                                UploadArtPiece();

                                _reviewerPoints += 10;
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                                ReviewThisArtPiece();
                        }
                        else if (missionType == MissionType.LikeArt)
                        {
                                _artistPoints += 10;
                                UploadArtPiece();

                                _reviewerPoints += 10;
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Browse");
                                ReviewThenLikeThisArtPiece();
                        }
                        else if (missionType == MissionType.VisitArtistsProfiles)
                        {
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/otherArtist{i}");
                                Wait.Until(d => d.FindElement(By.Id("artistName")).Text == $"otherArtist{i}");
                        }
                        else if (missionType == MissionType.VisitReviewersProfiles)
                        {
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewers/otherUser{i}");
                                Wait.Until(d => d.FindElement(By.Id("reviewerName")).Text == $"otherUser{i}");
                        }
                        else
                        {
                                throw new InvalidEnumArgumentException($"Could not determine how to progress a mission of type {missionType}");
                        }
                }
        }
}
