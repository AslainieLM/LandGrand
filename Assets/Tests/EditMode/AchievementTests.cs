using System.Linq; // Gives list helpers such as Select and Count
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the achievements of D-060: each badge is worked out from saved progress only.</summary>
    public class AchievementTests // A group of checks
    {
        static bool Earned(string id, PlayerProgress p) => AchievementCatalog.Find(id).IsEarned(p); // Short way to ask about one badge

        [Test] // Unity runs this method as a Test
        public void Catalog_HasTheEightStarterBadges_InOrder() // The draft set (D-060)
        {
            CollectionAssert.AreEqual(new[] { "first_steps", "rock_hound", "crystal_finder", "star_explorer", "volcano_visitor", "summit_climber", "perfect_trail", "mountain_collector", "philippine_peaks", "top_of_the_world", "rock_scholar" }, AchievementCatalog.All.Select(a => a.Id).ToArray()); // Ids in order
            foreach (var a in AchievementCatalog.All) // Every badge
            {
                Assert.IsFalse(string.IsNullOrEmpty(a.Name), a.Id); // A name
                Assert.IsFalse(string.IsNullOrEmpty(a.HowToEarn), a.Id); // A hint
                Assert.IsTrue(a.IsDraft, a.Id); // Draft names until reviewed
            }
            Assert.IsNull(AchievementCatalog.Find("nothing")); // Unknown ids are not found
        }

        [Test] // Unity runs this method as a Test
        public void Tiers_GoBronzeSilverGold() // Three of each colour, two gold
        {
            CollectionAssert.AreEqual(new[] { BadgeTier.Bronze, BadgeTier.Bronze, BadgeTier.Bronze, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Silver }, AchievementCatalog.All.Select(a => a.Tier).ToArray()); // In order
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_HasNoBadges() // Nothing earned yet
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.AreEqual(0, AchievementCatalog.EarnedCount(p)); // None
            Assert.AreEqual(11, AchievementCatalog.All.Count); // Out of eight
        }

        [Test] // Unity runs this method as a Test
        public void FirstSteps_ComesWithTheFirstClear() // Any level, any stars
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // Clear Easy 1 with one star
            Assert.IsTrue(Earned("first_steps", p)); // Earned
            Assert.IsFalse(Earned("star_explorer", p)); // One star is not three
        }

        [Test] // Unity runs this method as a Test
        public void RockHoundAndCrystalFinder_FollowTheFirstRockAndMineral() // Granite is a rock, quartz a mineral
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("granite"); // A rock
            Assert.IsTrue(Earned("rock_hound", p)); // Rock badge
            Assert.IsFalse(Earned("crystal_finder", p)); // No mineral yet
            p.RecordFoundKind("quartz"); // A mineral
            Assert.IsTrue(Earned("crystal_finder", p)); // Mineral badge
        }

        [Test] // Unity runs this method as a Test
        public void StarExplorer_NeedsThreeStarsOnOneLevel() // Any level
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 3); // Three stars on Easy 2
            Assert.IsTrue(Earned("star_explorer", p)); // Earned
        }

        [Test] // Unity runs this method as a Test
        public void VolcanoVisitorAndSummitClimber_FollowEasy2AndEasy3() // The two new levels (D-055)
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // Clear Easy 2
            Assert.IsTrue(Earned("volcano_visitor", p)); // Volcano Trail cleared
            Assert.IsFalse(Earned("summit_climber", p)); // Easy 3 not yet
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 3, 1); // Clear Easy 3
            Assert.IsTrue(Earned("summit_climber", p)); // Folded Peaks cleared
        }

        [Test] // Unity runs this method as a Test
        public void PerfectTrail_NeedsThreeStarsOnEveryMountainsEasyLevel() // Gold
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // Three stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 3); // Three stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 3, 2); // Only two
            Assert.IsFalse(Earned("perfect_trail", p)); // Not yet
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 3, 3); // A better replay (D-045)
            Assert.IsTrue(Earned("perfect_trail", p)); // Earned
        }

        [Test] // Unity runs this method as a Test
        public void MountainCollector_NeedsEveryMountainsKind() // All ten (D-055)
        {
            var p = new PlayerProgress(); // A brand new profile
            var kinds = ItemCatalog.All.Where(k => k.HomeLandform == Episode.Mountains).ToList(); // The Mountains shelf
            foreach (var k in kinds.Skip(1)) p.RecordFoundKind(k.Id); // All but one
            Assert.IsFalse(Earned("mountain_collector", p)); // Not yet
            p.RecordFoundKind(kinds[0].Id); // The last one
            Assert.IsTrue(Earned("mountain_collector", p)); // Earned
        }

        [Test] // Unity runs this method as a Test
        public void NewlyEarned_ListsOnlyBadgesEarnedSinceBefore() // For the Result Screen notice
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("granite"); // Rock Hound, earned in an earlier level
            var before = AchievementCatalog.EarnedIds(p); // What the explorer had when the level began
            p.RecordFoundKind("quartz"); // Crystal Finder, during this level
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // First Steps and Star Explorer, at the goal
            CollectionAssert.AreEqual(new[] { "crystal_finder", "first_steps", "star_explorer" }, AchievementCatalog.NewlyEarned(before, p).Select(a => a.Id).OrderBy(id => id).ToArray()); // Only the new ones
        }
    }
}
