using System.Linq; // Gives list helpers such as Select and Count
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the achievements of D-060: each badge is worked out from saved progress only.</summary>
    public class AchievementTests // A group of checks
    {
        static bool Earned(string id, PlayerProgress p) => AchievementCatalog.Find(id).IsEarned(p); // Short way to ask about one badge

        [Test] // Unity runs this method as a Test
        public void Catalog_HasThirtyFiveBadges_InOrder() // Keeps each landform in order
        {
            CollectionAssert.AreEqual(new[] { "first_steps", "rock_hound", "crystal_finder", "star_explorer", "volcano_visitor", "summit_climber", "perfect_trail", "mountain_collector", "philippine_peaks", "top_of_the_world", "rock_scholar", "brave_hiker", "world_peaks", "hard_star", "mountain_master", "pasture_pal", "dune_surfer", "rolling_star", "hill_collector", "philippine_hills", "balloon_rider", "slope_ranger", "world_hills", "steady_star", "hill_master", "salt_maker", "straw_jumper", "flat_star", "plain_collector", "philippine_plains", "delta_boater", "prairie_ranger", "world_plains", "steady_plains", "plain_master" }, AchievementCatalog.All.Select(a => a.Id).ToArray()); // Ids in order
            foreach (var a in AchievementCatalog.All) // Every badge
            {
                Assert.IsFalse(string.IsNullOrEmpty(a.Name), a.Id); // A name
                Assert.IsFalse(string.IsNullOrEmpty(a.HowToEarn), a.Id); // A hint
                Assert.IsTrue(a.IsDraft, a.Id); // Draft names until reviewed
            }
            Assert.IsNull(AchievementCatalog.Find("nothing")); // Unknown ids are not found
        }

        [Test] // Unity runs this method as a Test
        public void Tiers_MatchTheBadgeOrder() // Keeps each badge tier
        {
            CollectionAssert.AreEqual(new[] { BadgeTier.Bronze, BadgeTier.Bronze, BadgeTier.Bronze, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold }, AchievementCatalog.All.Select(a => a.Tier).ToArray()); // In order
        }

        [Test] // Unity runs this method as a Test
        public void ForLandform_SplitsTheBadgesIntoPages() // Shows the badges on their landform pages
        {
            CollectionAssert.AreEqual(AchievementCatalog.All.Take(15).Select(a => a.Id), AchievementCatalog.ForLandform(Episode.Mountains).Select(a => a.Id)); // The Mountains page keeps the first 15 in order
            CollectionAssert.AreEqual(AchievementCatalog.All.Skip(15).Take(10).Select(a => a.Id), AchievementCatalog.ForLandform(Episode.Hills).Select(a => a.Id)); // The Hills page is the ten after them
            CollectionAssert.AreEqual(AchievementCatalog.All.Skip(25).Select(a => a.Id), AchievementCatalog.ForLandform(Episode.Plains).Select(a => a.Id)); // Plains gets the final ten
            Assert.AreEqual(0, AchievementCatalog.ForLandform(Episode.Plateaus).Count); // Plateaus has no badges yet
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_HasNoBadges() // Nothing earned yet
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.AreEqual(0, AchievementCatalog.EarnedCount(p)); // None
            Assert.AreEqual(35, AchievementCatalog.All.Count); // Out of thirty-five
        }

        [TestCase(2, "salt_maker")] // Salt Maker needs Plains Easy 2
        [TestCase(3, "straw_jumper")] // Straw Jumper needs Plains Easy 3
        public void PlainsClearBadge_NeedsItsOwnPlainsLevel(int level, string badge) // Checks the required clear
        { // Starts the clear check
            var progress = new PlayerProgress(); // Starts with a fresh profile
            progress.RecordClear(Episode.Hills, Difficulty.Easy, level, 3); // Clears the same number in Hills
            progress.RecordClear(Episode.Plains, Difficulty.Easy, level == 2 ? 3 : 2, 1); // Clears the other Plains badge level
            Assert.IsFalse(Earned(badge, progress)); // The required Plains clear is still missing
            progress.RecordClear(Episode.Plains, Difficulty.Easy, level, 1); // Clears the required level with one star
            Assert.IsTrue(Earned(badge, progress)); // The clear earns the badge
        } // Ends the clear check

        [Test] // Checks the Plains three-star badge
        public void FlatStar_NeedsThreeStarsOnEveryPlainsEasyLevel() // Checks the Easy trail
        { // Starts the star check
            var progress = new PlayerProgress(); // Starts with a fresh profile
            progress.RecordClear(Episode.Plains, Difficulty.Easy, 1, 3); // Gets three stars on the first trail
            progress.RecordClear(Episode.Plains, Difficulty.Easy, 2, 3); // Gets three stars on the second trail
            progress.RecordClear(Episode.Plains, Difficulty.Easy, 3, 2); // Leaves one star to earn on the last trail
            Assert.IsFalse(Earned("flat_star", progress)); // The last trail is short of three stars
            progress.RecordClear(Episode.Plains, Difficulty.Easy, 3, 3); // Earns the last star on replay
            Assert.IsTrue(Earned("flat_star", progress)); // All three trails now have three stars
        } // Ends the star check

        [Test] // Checks the Plains collector badge
        public void PlainCollector_NeedsAllTenPlainsKinds() // Counts the full Plains shelf
        { // Starts the collection check
            var progress = new PlayerProgress(); // Starts with a fresh profile
            var kinds = ItemCatalog.All.Where(kind => kind.BelongsTo(Episode.Plains)).ToArray(); // Reads the Plains shelf
            Assert.AreEqual(10, kinds.Length); // The shelf has ten kinds
            foreach (var kind in kinds.Take(9)) progress.RecordFoundKind(kind.Id); // Finds nine kinds
            Assert.IsFalse(Earned("plain_collector", progress)); // One kind is still missing
            progress.RecordFoundKind(kinds[9].Id); // Finds the tenth kind
            Assert.IsTrue(Earned("plain_collector", progress)); // The full shelf earns the badge
        } // Ends the collection check

        [Test] // Checks the Plains badge page on a new profile
        public void AllTenPlainsBadges_StartLocked() // Counts the locked Plains badges
        { // Starts the locked badge check
            var progress = new PlayerProgress(); // Starts with a fresh profile
            var badges = AchievementCatalog.ForLandform(Episode.Plains); // Opens the Plains badge page
            Assert.AreEqual(10, badges.Count); // The page has ten badges
            Assert.IsFalse(badges.Any(badge => badge.IsEarned(progress))); // Every Plains badge starts locked
        } // Ends the locked badge check

        [Test] // Checks that Plains progress respects the other landform rules
        public void PlainsEasyProgress_EarnsOnlyGeneralBadgesOnTheOtherPages() // Keeps shared rewards and landform rewards separate
        { // Starts the badge page check
            var progress = new PlayerProgress(); // Starts with a fresh profile
            for (int level = 1; level <= 3; level++) progress.RecordClear(Episode.Plains, Difficulty.Easy, level, 3); // Gets three stars on each Plains Easy level
            foreach (var kind in ItemCatalog.All.Where(kind => kind.BelongsTo(Episode.Plains))) progress.RecordFoundKind(kind.Id); // Finds every kind on the Plains shelf
            var mountains = AchievementCatalog.ForLandform(Episode.Mountains).Where(badge => badge.IsEarned(progress)); // Reads earned badges shown on the Mountains page
            CollectionAssert.AreEquivalent(new[] { "first_steps", "rock_hound", "crystal_finder", "star_explorer" }, mountains.Select(badge => badge.Id)); // Only the general badges count
            Assert.IsFalse(AchievementCatalog.ForLandform(Episode.Hills).Any(badge => badge.IsEarned(progress))); // Hills badges need Hills progress
        } // Ends the badge page check

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
        public void MountainCollector_NeedsEveryMountainsKind() // All 34 Mountains kinds
        {
            var p = new PlayerProgress(); // A brand new profile
            var kinds = ItemCatalog.All.Where(k => k.BelongsTo(Episode.Mountains)).ToList(); // The Mountains shelf
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
