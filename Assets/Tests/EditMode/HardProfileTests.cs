using System.Linq; // Reads the shelf and level lists
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class HardProfileTests // Checks the new Collection page and badges
    { // Opens the checks
        [Test] // Runs the shelf check
        public void MountainsHasThreePagesWithTenKindsOnTheLast() // Uses the real 34 kinds
        { // Starts the paging check
            var shelf = Collection.Shelves(new PlayerProgress())[(int)Episode.Mountains]; // The complete Mountains shelf
            Assert.AreEqual(3, shelf.PageCount(12)); // Three pages of twelve slots
            CollectionAssert.AreEqual(new[] { 12, 12, 10 }, Enumerable.Range(0, 3).Select(page => shelf.SlotsOnPage(page, 12).Count)); // The last page leaves two empty slots
            Assert.AreEqual("olivine", shelf.SlotsOnPage(2, 12).Last().Kind.Id); // The last Hard kind can be reached
        } // Ends the paging check

        [Test] // Runs the first clear check
        public void BraveHikerNeedsAHardClearInMountains() // Ignores a clear on another difficulty or landform
        { // Starts the badge check
            var progress = new PlayerProgress(); // Fresh saved progress
            var badge = AchievementCatalog.Find("brave_hiker"); // The first Hard badge
            progress.RecordClear(Episode.Mountains, Difficulty.Normal, 5, 3); // A perfect Normal clear
            progress.RecordClear(Episode.Hills, Difficulty.Hard, 1, 3); // Belongs to another landform
            Assert.IsFalse(badge.IsEarned(progress)); // Neither clear counts
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 1, 1); // One star on the first Hard trail
            Assert.IsTrue(badge.IsEarned(progress)); // Any Mountains Hard clear is enough
        } // Ends the badge check

        [Test] // Runs the world peaks check
        public void WorldPeaksNeedsHardFiveSixAndSeven() // All three world mountains are required
        { // Starts the badge check
            var progress = new PlayerProgress(); // Fresh saved progress
            var badge = AchievementCatalog.Find("world_peaks"); // The world peaks medal
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 5, 1); // Matterhorn
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 1); // Mauna Kea
            Assert.IsFalse(badge.IsEarned(progress)); // Kilimanjaro is still missing
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 6, 1); // Kilimanjaro
            Assert.IsTrue(badge.IsEarned(progress)); // All three are clear
        } // Ends the badge check

        [Test] // Runs the perfect Hard check
        public void HardStarNeedsThreeStarsOnEveryHardTrail() // Clear alone is not enough for this badge
        { // Starts the badge check
            var progress = new PlayerProgress(); // Fresh saved progress
            var badge = AchievementCatalog.Find("hard_star"); // The three-star Hard medal
            for (int n = 1; n <= 6; n++) progress.RecordClear(Episode.Mountains, Difficulty.Hard, n, 3); // Six trails have three stars
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 2); // One trail is short a star
            Assert.IsFalse(badge.IsEarned(progress)); // Waits for the last star
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 3); // The replay earns the last star
            Assert.IsTrue(badge.IsEarned(progress)); // All seven now qualify
        } // Ends the badge check

        [Test] // Runs the Mountains completion check
        public void MountainMasterNeedsAllFifteenMountainsClears() // The whole episode must be finished
        { // Starts the badge check
            var progress = new PlayerProgress(); // Fresh saved progress
            var badge = AchievementCatalog.Find("mountain_master"); // The episode badge
            foreach (var level in LevelCatalog.All.Where(l => l.Episode == Episode.Mountains && l.SceneName != "MountainsHard7")) // Every trail but the last
                progress.RecordClear(level.Episode, level.Difficulty, level.Number, 1); // Stars do not affect this badge
            Assert.IsFalse(badge.IsEarned(progress)); // One trail still remains
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 1); // Clears the last trail
            Assert.IsTrue(badge.IsEarned(progress)); // Earns the episode badge
        } // Ends the badge check
    } // Ends the checks
} // Ends the tests
