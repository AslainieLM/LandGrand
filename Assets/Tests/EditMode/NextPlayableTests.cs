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
            Assert.IsFalse(LevelCatalog.HasLevels(Episode.Hills, Difficulty.Normal)); // Hills Normal has no data yet
            Assert.IsFalse(LevelCatalog.HasLevels(Episode.Hills, Difficulty.Hard)); // Hills Hard has no data yet
        }

        [Test]
        public void NextAfterHillsEasy3IsHiddenBecauseHillsNormalHasNoData() // Next is hidden on Hills Easy 3
        {
            var p = new PlayerProgress(); // A fresh save
            ClearMountains(p); // Opens Hills
            ClearHillsEasy(p); // Hills Easy 1 to 3 cleared
            Assert.AreEqual(NextStep.NextLevel, p.NextAfter(Episode.Hills, Difficulty.Easy, 3).Step); // The old rule still points at Normal 1
            Assert.AreEqual(NextStep.None, p.NextPlayableAfter(Episode.Hills, Difficulty.Easy, 3).Step); // With no data, nothing playable follows
        }

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
