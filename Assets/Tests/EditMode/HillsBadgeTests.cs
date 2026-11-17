using System.Linq; // Reads the shelf and level lists
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{
    /// <summary>Checks the ten Hills badges (spec section 3, D-066): each one is worked out from saved clears and stars.</summary>
    public class HillsBadgeTests // A group of checks
    {
        static bool Earned(string id, PlayerProgress p) => AchievementCatalog.Find(id).IsEarned(p); // Short way to ask about one badge

        [Test] // Unity runs this method as a Test
        public void PastureAndDuneBadges_FollowHillsEasy2AndEasy3() // Silver badges for the second and third Hills Easy levels
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Hills, Difficulty.Easy, 2, 1); // Clear Hills Easy 2
            Assert.IsTrue(Earned("pasture_pal", p)); // Pasture Pal earned
            Assert.IsFalse(Earned("dune_surfer", p)); // Easy 3 not yet
            p.RecordClear(Episode.Hills, Difficulty.Easy, 3, 1); // Clear Hills Easy 3
            Assert.IsTrue(Earned("dune_surfer", p)); // Dune Surfer earned
        }

        [Test] // Unity runs this method as a Test
        public void RollingStar_NeedsThreeStarsOnAllThreeHillsEasy() // Gold badge for perfect Hills Easy levels
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Hills, Difficulty.Easy, 1, 3); // Three stars
            p.RecordClear(Episode.Hills, Difficulty.Easy, 2, 3); // Three stars
            p.RecordClear(Episode.Hills, Difficulty.Easy, 3, 2); // Only two
            Assert.IsFalse(Earned("rolling_star", p)); // Not yet
            p.RecordClear(Episode.Hills, Difficulty.Easy, 3, 3); // A better replay (D-045)
            Assert.IsTrue(Earned("rolling_star", p)); // Earned
        }

        [Test] // Unity runs this method as a Test
        public void HillCollector_NeedsEveryHillsKind() // All seventeen kinds on the Hills shelf, including shared kinds
        {
            var p = new PlayerProgress(); // A brand new profile
            var kinds = ItemCatalog.All.Where(k => k.BelongsTo(Episode.Hills)).ToList(); // The Hills shelf
            Assert.AreEqual(17, kinds.Count); // The shelf grew by seven kinds
            foreach (var k in kinds.Take(10)) p.RecordFoundKind(k.Id); // Loads a save with the original complete shelf
            Assert.IsFalse(Earned("hill_collector", p)); // The original ten do not fill the expanded shelf
            foreach (var k in kinds.Skip(10)) p.RecordFoundKind(k.Id); // Fills the new part of the shelf
            Assert.IsTrue(Earned("hill_collector", p)); // Earned
        }

        [Test] // Unity runs this method as a Test
        public void HillsNormalAndHardBadges_StayLockedOnAFreshProfile() // No level data or clears yet, so nothing is earned
        {
            var p = new PlayerProgress(); // A brand new profile
            foreach (var id in new[] { "philippine_hills", "balloon_rider", "slope_ranger", "world_hills", "steady_star", "hill_master" }) // Every Hills badge past Easy
                Assert.IsFalse(Earned(id, p), id); // Still locked
        }

        [Test] // Unity runs this method as a Test
        public void HillsNormalAndHardRules_WorkOnceTheirClearsExist() // The rules are right, even though the levels come later
        {
            var normal = new PlayerProgress(); // Normal clears only
            for (int n = 1; n <= 4; n++) normal.RecordClear(Episode.Hills, Difficulty.Normal, n, 1); // Normal 1 to 4
            Assert.IsTrue(Earned("philippine_hills", normal)); // Silver badge after Normal 1 to 4
            Assert.IsFalse(Earned("balloon_rider", normal)); // Normal 5 not yet
            normal.RecordClear(Episode.Hills, Difficulty.Normal, 5, 1); // Normal 5
            Assert.IsTrue(Earned("balloon_rider", normal)); // Gold badge after Normal 5

            var hard = new PlayerProgress(); // Hard clears only
            hard.RecordClear(Episode.Hills, Difficulty.Hard, 4, 1); // One Hard level
            Assert.IsTrue(Earned("slope_ranger", hard)); // Any Hard clear is enough
            Assert.IsFalse(Earned("world_hills", hard)); // Hard 5 to 7 not yet
            for (int n = 5; n <= 7; n++) hard.RecordClear(Episode.Hills, Difficulty.Hard, n, 3); // Hard 5 to 7 with three stars
            Assert.IsTrue(Earned("world_hills", hard)); // World Hills earned
            Assert.IsFalse(Earned("steady_star", hard)); // Hard 1 to 3 have no stars yet
            for (int n = 1; n <= 7; n++) hard.RecordClear(Episode.Hills, Difficulty.Hard, n, 3); // Every Hard level with three stars
            Assert.IsTrue(Earned("steady_star", hard)); // Steady Star earned
        }

        [Test] // Unity runs this method as a Test
        public void HillMaster_NeedsEveryHillsLevel() // All fifteen Hills levels, on every difficulty
        {
            var p = new PlayerProgress(); // A brand new profile
            foreach (Difficulty d in new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard }) // Each difficulty
                for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) // Each level number
                    if (!(d == Difficulty.Hard && n == 7)) p.RecordClear(Episode.Hills, d, n, 1); // Clear all but Hard 7
            Assert.IsFalse(Earned("hill_master", p)); // One level still remains
            p.RecordClear(Episode.Hills, Difficulty.Hard, 7, 1); // Clear the last one
            Assert.IsTrue(Earned("hill_master", p)); // Earns the Hills badge
        }

        [Test] // Unity runs this method as a Test
        public void HillsClears_DoNotEarnMountainsBadges() // Each landform keeps its own badges
        {
            var p = new PlayerProgress(); // A brand new profile
            for (int n = 1; n <= 7; n++) p.RecordClear(Episode.Hills, Difficulty.Hard, n, 3); // Every Hills Hard level, three stars
            Assert.IsFalse(Earned("world_peaks", p)); // World Peaks needs Mountains Hard 5 to 7
            Assert.IsFalse(Earned("hard_star", p)); // Hard Star needs Mountains Hard
            Assert.IsFalse(Earned("brave_hiker", p)); // Brave Hiker needs a Mountains Hard clear
        }
    }
}
