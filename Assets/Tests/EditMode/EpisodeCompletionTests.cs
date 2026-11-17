using LandGrand.Rules; // Uses the saved progress rules
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class EpisodeCompletionTests // Checks what the last Mountains clear opens
    { // Opens the checks
        [Test] // Runs the completion check
        public void HillsNeedsAllFifteenMountainsClearsEvenWithOneStar() // Keeps the episode unlock tied to clears
        { // Starts with every clear but one
            var progress = new PlayerProgress(); // A fresh save
            RecordMountainsExceptLast(progress); // Clears 14 of the 15 levels
            Assert.IsFalse(progress.IsEpisodeComplete(Episode.Mountains)); // One level still missing
            Assert.IsFalse(progress.IsEpisodeUnlocked(Episode.Hills)); // Hills is still locked
            Assert.AreEqual(NextStep.None, progress.NextAfter(Episode.Mountains, Difficulty.Hard, 7).Step); // No next episode yet
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 1); // Clears the last level with one star
            Assert.IsTrue(progress.IsEpisodeComplete(Episode.Mountains)); // Completes Mountains
            Assert.IsTrue(progress.IsEpisodeUnlocked(Episode.Hills)); // Opens Hills
            Assert.IsFalse(progress.IsEpisodeUnlocked(Episode.Plains)); // Plains needs Hills completed
            Assert.AreEqual(NextStep.NextEpisode, progress.NextAfter(Episode.Mountains, Difficulty.Hard, 7).Step); // Opens the next episode summary
            Assert.IsTrue(PlayerProgress.TryFromJson(progress.ToJson(), out var loaded)); // Uses the existing save format
            Assert.IsTrue(loaded.IsEpisodeUnlocked(Episode.Hills)); // The saved clears open Hills after a reload
        } // Ends the completion check

        [Test] // Runs the catalog and total check
        public void StarsUseBestClearsAndEachLandformPlansForFortyFive() // Counts only stars from the chosen landform
        { // Sets up two landforms
            var progress = new PlayerProgress(); // Starts with no stars
            progress.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // A three-star clear
            progress.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // A worse replay
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 1, 2); // Two more stars
            progress.RecordClear(Episode.Hills, Difficulty.Easy, 1, 1); // This belongs to another landform
            Assert.AreEqual(5, progress.StarsFor(Episode.Mountains)); // Keeps the best stars from Mountains
            Assert.AreEqual(45, PlayerProgress.MaxStarsFor(Episode.Mountains)); // Three stars for each of 15 levels
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Mountains)); // Mountains opens difficulty selection
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Hills)); // Hills has Easy levels now, so it is not Coming soon (D-066)
        } // Ends the total check

        [Test] // Runs the animation decision check
        public void OnlyAnOpenUnseenLandformGetsTheAnimation() // Tracks the seen choice per landform
        { // Starts the checks
            var progress = new PlayerProgress(); // Mountains alone is open
            Assert.IsFalse(progress.ShouldAnimateUnlock(Episode.Mountains, false)); // Never animates the starting landform
            Assert.IsFalse(progress.ShouldAnimateUnlock(Episode.Hills, false)); // Cannot animate before Hills opens
            RecordMountainsExceptLast(progress); // Clears the earlier trails
            progress.RecordClear(Episode.Mountains, Difficulty.Hard, 7, 1); // Opens Hills
            Assert.IsTrue(progress.ShouldAnimateUnlock(Episode.Hills, false)); // First visit gets the effect
            Assert.IsFalse(progress.ShouldAnimateUnlock(Episode.Hills, true)); // Its own seen flag stops a repeat
            Assert.IsFalse(progress.ShouldAnimateUnlock(Episode.Plains, false)); // A locked landform still waits
        } // Ends the animation decision check

        [Test] // Runs the episode order check
        public void NextEpisodeFollowsTheMapOrderAndStopsAtPlateaus() // Uses the same order on every route
        { // Starts the check
            Assert.AreEqual(Episode.Hills, PlayerProgress.NextEpisodeFor(Episode.Mountains)); // Mountains leads to Hills
            Assert.AreEqual(Episode.Plains, PlayerProgress.NextEpisodeFor(Episode.Hills)); // Hills leads to Plains
            Assert.AreEqual(Episode.Plateaus, PlayerProgress.NextEpisodeFor(Episode.Plains)); // Plains leads to Plateaus
            Assert.IsNull(PlayerProgress.NextEpisodeFor(Episode.Plateaus)); // Plateaus ends the list
        } // Ends the order check

        static void RecordMountainsExceptLast(PlayerProgress progress) // Clears the 14 trails before Hard 7
        { // Starts the setup
            foreach (var level in LevelCatalog.All) // Uses the real trails
                if (level.Episode == Episode.Mountains && (level.Difficulty != Difficulty.Hard || level.Number < 7)) progress.RecordClear(level.Episode, level.Difficulty, level.Number, 1); // Leaves the final goal uncleared
        } // Ends the setup
    } // Ends the checks
} // Ends the tests
