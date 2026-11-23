using LandGrand.Rules; // Uses the saved progress and catalog rules
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{
    public class NextPlayableTests // Checks that Next and Coming soon only use levels that have data (D-066)
    {
        [Test]
        public void DifficultyHasLevelsOnlyWhereDataExists() // Coming soon uses this check
        {
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Mountains, Difficulty.Easy)); // Mountains Easy has data
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Mountains, Difficulty.Hard)); // Mountains Hard has data
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Hills, Difficulty.Easy)); // Hills Easy has data
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Hills, Difficulty.Normal)); // Hills Normal has its five trails
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Hills, Difficulty.Hard)); // Hills Hard has seven trails
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plains, Difficulty.Easy)); // Plains Easy has three trails
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plains, Difficulty.Normal)); // Plains Normal has five trails
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plains, Difficulty.Hard)); // Plains Hard has seven trails (D-071)
        }

        [Test] // Checks the new route from Easy to Normal
        public void NextAfterHillsEasy3OpensHillsNormal1() // Shows Next after the last Easy clear
        { // Opens the check
            var p = new PlayerProgress(); // A fresh save
            ClearMountains(p); // Opens Hills
            ClearHillsEasy(p); // Clears the three Hills Easy trails
            var next = p.NextPlayableAfter(Episode.Hills, Difficulty.Easy, 3); // Looks for the next playable trail
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Shows Next
            Assert.AreEqual(Episode.Hills, next.Episode); // Stays on Hills
            Assert.AreEqual(Difficulty.Normal, next.Difficulty); // Opens Normal
            Assert.AreEqual(1, next.Level); // Opens the first trail
        } // Ends the check

        [Test] // Checks the end of the new route
        public void NextAfterHillsNormal5OpensHard1() // Shows Next after the last Normal clear
        { // Opens the check
            var p = new PlayerProgress(); // A fresh save
            ClearMountains(p); // Opens Hills
            ClearHillsEasy(p); // Opens Hills Normal
            for (int n = 1; n <= 5; n++) p.RecordClear(Episode.Hills, Difficulty.Normal, n, 1); // Clears all five new trails
            var next = p.NextPlayableAfter(Episode.Hills, Difficulty.Normal, 5); // Looks for the next trail
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Shows Next Level
            Assert.AreEqual(Episode.Hills, next.Episode); // Stays in Hills
            Assert.AreEqual(Difficulty.Hard, next.Difficulty); // Opens Hard
            Assert.AreEqual(1, next.Level); // Starts on the first Hard trail
        } // Ends the check

        [Test]
        public void NextInsideHillsEasyStillOpensTheNextLevel() // Levels with data keep working
        {
            var p = new PlayerProgress(); // A fresh save
            ClearMountains(p); // Opens Hills
            p.RecordClear(Episode.Hills, Difficulty.Easy, 1, 1); // Hills Easy 1 cleared
            var next = p.NextPlayableAfter(Episode.Hills, Difficulty.Easy, 1); // What comes next
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Another level
            Assert.AreEqual(2, next.Level); // Hills Easy 2 has data
        }

        [Test]
        public void MountainsEasy3StillOpensNormal1() // Mountains keeps its order
        {
            var p = new PlayerProgress(); // A fresh save
            ClearMountainsEasy(p); // Easy 1 to 3 cleared
            var next = p.NextPlayableAfter(Episode.Mountains, Difficulty.Easy, 3); // What comes next
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Another level
            Assert.AreEqual(Difficulty.Normal, next.Difficulty); // Normal opens
            Assert.AreEqual(1, next.Level); // Normal 1
        }

        [Test]
        public void MountainsNormal5StillOpensHard1() // Mountains keeps its order
        {
            var p = new PlayerProgress(); // A fresh save
            ClearMountainsEasy(p); // Easy 1 to 3 cleared
            for (int n = 1; n <= PlayerProgress.LevelCount(Difficulty.Normal); n++) p.RecordClear(Episode.Mountains, Difficulty.Normal, n, 1); // Normal 1 to 5 cleared
            var next = p.NextPlayableAfter(Episode.Mountains, Difficulty.Normal, 5); // What comes next
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Another level
            Assert.AreEqual(Difficulty.Hard, next.Difficulty); // Hard opens
            Assert.AreEqual(1, next.Level); // Hard 1
        }

        [Test]
        public void NextEpisodeAfterMountainsHard7StillOpensHills() // The landform route is unchanged
        {
            var p = new PlayerProgress(); // A fresh save
            ClearMountains(p); // All 15 Mountains levels cleared
            var next = p.NextPlayableAfter(Episode.Mountains, Difficulty.Hard, 7); // What comes next
            Assert.AreEqual(NextStep.NextEpisode, next.Step); // The next landform
            Assert.AreEqual(Episode.Hills, next.Episode); // Hills
        }

        [Test]
        public void NothingOpenBeforeClearsStaysNone() // A level that is not open gives None as before
        {
            var p = new PlayerProgress(); // A fresh save
            Assert.AreEqual(NextStep.None, p.NextPlayableAfter(Episode.Mountains, Difficulty.Easy, 1).Step); // Easy 2 is still locked
        }

        [Test] // Checks the final Hills clear
        public void HillsHard7OffersTheEpisodeReviewAndUnlocksPlains() // Follows the generic episode flow
        { // Opens the check
            var p = new PlayerProgress(); // Starts a new save
            ClearMountains(p); // Opens Hills
            var hills = System.Linq.Enumerable.Where(LevelCatalog.All, level => level.Episode == Episode.Hills); // Uses the built Hills trails
            foreach (var level in hills) // Clears every trail before the finale
                if (level.Difficulty != Difficulty.Hard || level.Number != 7) p.RecordClear(level.Episode, level.Difficulty, level.Number, 1); // Leaves the last trail open
            Assert.IsFalse(p.IsEpisodeComplete(Episode.Hills)); // Hills needs the final clear
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Plains)); // Plains waits for Hills
            p.RecordClear(Episode.Hills, Difficulty.Hard, 7, 1); // Finishes Hills
            Assert.IsTrue(p.IsEpisodeComplete(Episode.Hills)); // All fifteen trails count
            var next = p.NextPlayableAfter(Episode.Hills, Difficulty.Hard, 7); // Reads the Result Screen route
            Assert.AreEqual(NextStep.NextEpisode, next.Step); // Result opens the Landform Description Screen
            Assert.AreEqual(Episode.Plains, next.Episode); // Points to the next landform
            Assert.IsTrue(p.IsEpisodeUnlocked(Episode.Plains)); // The map can open Plains
            Assert.IsTrue(p.ShouldAnimateUnlock(Episode.Plains, false)); // The first visit shows the unlock
            Assert.IsFalse(p.ShouldAnimateUnlock(Episode.Plains, true)); // The seen flag prevents a repeat
            Assert.IsTrue(p.IsLevelAvailable(Episode.Plains, Difficulty.Easy, 1)); // Plains Easy 1 opens from the map
            Assert.IsNotNull(LevelCatalog.Find(Episode.Plains, Difficulty.Easy, 1)); // The first trail has playable data
        } // Ends the check

        [Test] // Checks the end of Plains Easy
        public void PlainsEasy3OpensPlainsNormal1() // The first Normal trail is ready
        { // Starts the Plains route check
            var p = ClearedBeforePlainsNormal(); // Finishes Plains Easy
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Plains, Difficulty.Normal)); // Opens the Normal card
            var next = p.NextPlayableAfter(Episode.Plains, Difficulty.Easy, 3); // Reads the next step from the Result Screen
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Shows the Next button
            Assert.AreEqual(Episode.Plains, next.Episode); // Stays in Plains
            Assert.AreEqual(Difficulty.Normal, next.Difficulty); // Opens the Normal trail
            Assert.AreEqual(1, next.Level); // Starts at the first Normal trail
        } // Ends the Plains route check

        [Test] // Checks the end of Plains Normal
        public void PlainsNormal5OpensHardCardAndShowsNext() // The Hard card opens its first trail (D-071)
        { // Starts the final Normal check
            var p = ClearedBeforePlainsNormal(); // Finishes Plains Easy
            for (int n = 1; n <= 5; n++) p.RecordClear(Episode.Plains, Difficulty.Normal, n, 1); // Finishes all five Normal trails
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Plains, Difficulty.Hard)); // Opens the Hard card
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plains, Difficulty.Hard)); // Tapping Hard opens its trail
            var next = p.NextPlayableAfter(Episode.Plains, Difficulty.Normal, 5); // Reads the Result Screen route
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Shows Next on the Result Screen
            Assert.AreEqual((Episode.Plains, Difficulty.Hard, 1), (next.Episode, next.Difficulty, next.Level)); // Points at Plains Hard 1
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Plateaus)); // Plateaus still needs the whole Plains trail
        } // Ends the final Normal check

        static PlayerProgress ClearedBeforePlainsNormal() // Builds a save ready for Plains Normal
        { // Starts the save
            var progress = new PlayerProgress(); // Starts with no clears
            foreach (var trail in LevelCatalog.All) // Reads the available earlier trails
                if (trail.Episode != Episode.Plains || trail.Difficulty == Difficulty.Easy) progress.RecordClear(trail.Episode, trail.Difficulty, trail.Number, 1); // Clears the trails before Plains Normal
            return progress; // Opens Plains Normal
        } // Ends the save setup

        static void ClearMountains(PlayerProgress p) // Clears all 15 Mountains levels
        {
            foreach (var d in new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard }) // Each difficulty
                for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) p.RecordClear(Episode.Mountains, d, n, 1); // Each level, one star
        }

        static void ClearMountainsEasy(PlayerProgress p) // Clears the three Mountains Easy levels
        {
            for (int n = 1; n <= PlayerProgress.LevelCount(Difficulty.Easy); n++) p.RecordClear(Episode.Mountains, Difficulty.Easy, n, 1); // Easy 1 to 3, one star
        }

        static void ClearHillsEasy(PlayerProgress p) // Clears the three Hills Easy levels
        {
            for (int n = 1; n <= PlayerProgress.LevelCount(Difficulty.Easy); n++) p.RecordClear(Episode.Hills, Difficulty.Easy, n, 1); // Hills Easy 1 to 3, one star
        }
    }
}
