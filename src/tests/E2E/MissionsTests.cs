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
                int missionProgress = int.Parse(Wait.Until(d => d.FindElement(By.Id("mission-progress"))).Text);
                missionProgress.Should().Be(1);
        }

        [Fact, Order(2)]
        public void MissionWidget_ShowsCompletedAndAwardsPoints_WhenFinished()
        {
                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                string missionDescription = Wait.Until(d => d.FindElement(By.Id("mission-description"))).Text;
                MissionType missionType = MissionTypeHelpers.GetMissionForDescription(missionDescription);

                int stepsAlreadyPerformed = 1;
                int stepsToPerform = missionType.GetMaxProgressCount() - stepsAlreadyPerformed;

                ProgressMission(missionType, times: stepsToPerform);

                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Leaderboard");
                int missionProgress = int.Parse(Wait.Until(d => d.FindElement(By.Id("mission-progress"))).Text);
                missionProgress.Should().Be(missionType.GetMaxProgressCount());

                int artistQuestPointValue = (missionType.GetRecipient() == MissionRecipient.Artist)
                        ? 25
                        : 0;
                int reviewerQuestPointValue = (missionType.GetRecipient() == MissionRecipient.Reviewer)
                        ? 25
                        : 0;
                Wait.Until(d =>
                {
                        var artistRows = d.FindElements(By.CssSelector("#leaderboard-body > tr"));
                        var artistPoints = artistRows.Select(r => r.FindElement(By.CssSelector("td:nth-child(3)")).Text).ToList();
                        if (artistPoints.Count != 1)
                        {
                                return false;
                        }

                        return int.Parse(artistPoints.Single()) == _artistPoints + artistQuestPointValue;
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

                        return int.Parse(reviewerPoints.Single()) == _reviewerPoints + reviewerQuestPointValue;
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
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Artists/someArtist");
                        }
                        else if (missionType == MissionType.VisitReviewersProfiles)
                        {
                                Driver.Navigate().GoToUrl($"{HTTP_PROTOCOL_PREFIX}localhost/Reviewer");
                        }
                        else
                        {
                                throw new InvalidEnumArgumentException($"Could not determine how to progress a mission of type {missionType}");
                        }
                }
        }
}
