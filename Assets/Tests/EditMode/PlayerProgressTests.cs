using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the unlock and best-star rules only through the public PlayerProgress methods.</summary>
    public class PlayerProgressTests // A group of checks
    {
        static void ClearAll(PlayerProgress p, Episode e, Difficulty d, int count, int stars) // Clears levels 1 to count of one difficulty
        {
            for (int n = 1; n <= count; n++) p.RecordClear(e, d, n, stars); // Clear each level in order
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_OnlyMountainsIsUnlocked() // A new player starts at Mountains
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsTrue(p.IsEpisodeUnlocked(Episode.Mountains)); // Mountains is open
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Hills)); // Hills is locked
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Plains)); // Plains is locked
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Plateaus)); // Plateaus is locked
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_OnlyEasyIsUnlocked_AndOnlyLevelOneIsAvailable() // Easy level 1 is the only start
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Easy)); // Easy is open
            Assert.IsFalse(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Normal)); // Normal is locked
            Assert.IsFalse(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Hard)); // Hard is locked
            Assert.IsTrue(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 1)); // Level 1 is open
            Assert.IsFalse(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 2)); // Level 2 waits for level 1
            Assert.IsFalse(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 3)); // Level 3 waits for level 2
        }

        [Test] // Unity runs this method as a Test
        public void ClearingALevel_OpensTheNextOne_AndKeepsEarlierOnesOpen() // Levels go in order
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // Clear level 1 with 1 star
            Assert.IsTrue(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 1)); // Level 1 can be replayed
            Assert.IsTrue(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 2)); // Level 2 is now open
            Assert.IsFalse(p.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 3)); // Level 3 is still closed
        }

        [Test] // Unity runs this method as a Test
        public void NormalUnlocksAfterAllEasyLevels_WhateverTheStars() // Stars never block unlocking
        {
            var p = new PlayerProgress(); // A brand new profile
            ClearAll(p, Episode.Mountains, Difficulty.Easy, 2, 3); // Clear only 2 of 3 Easy levels
            Assert.IsFalse(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Normal)); // Normal still locked
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 3, 1); // Clear the last Easy level with only 1 star
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Normal)); // Normal is open
            Assert.IsFalse(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Hard)); // Hard is still locked
            Assert.IsTrue(p.IsLevelAvailable(Episode.Mountains, Difficulty.Normal, 1)); // Normal level 1 is open
            Assert.IsFalse(p.IsLevelAvailable(Episode.Mountains, Difficulty.Normal, 2)); // Normal level 2 waits
        }

        [Test] // Unity runs this method as a Test
        public void HardUnlocksAfterAllFiveNormalLevels_WhateverTheStars() // Stars never block unlocking
        {
            var p = new PlayerProgress(); // A brand new profile
            ClearAll(p, Episode.Mountains, Difficulty.Easy, 3, 1); // Clear every Easy level
            ClearAll(p, Episode.Mountains, Difficulty.Normal, 4, 1); // Clear 4 of 5 Normal levels
            Assert.IsFalse(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Hard)); // Hard still locked
            p.RecordClear(Episode.Mountains, Difficulty.Normal, 5, 1); // Clear the fifth Normal level
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Hard)); // Hard is open
        }

        [Test] // Unity runs this method as a Test
        public void WorseReplay_KeepsTheBestStars() // D-045: a replay never lowers stars
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // First clear with 3 stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // Worse replay with 1 star
            Assert.AreEqual(3, p.BestStars(Episode.Mountains, Difficulty.Easy, 1)); // Best stars stay at 3
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // First clear of level 2 with 1 star
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 2); // Better replay with 2 stars
            Assert.AreEqual(2, p.BestStars(Episode.Mountains, Difficulty.Easy, 2)); // Best stars rise to 2
        }

        [Test] // Unity runs this method as a Test
        public void UnclearedLevel_HasZeroStars() // No clear means no stars
        {
            Assert.AreEqual(0, new PlayerProgress().BestStars(Episode.Mountains, Difficulty.Easy, 1)); // Zero stars
        }

        [Test] // Unity runs this method as a Test
        public void FoundItemKinds_AreKept_AndNeverRemoved() // Found kinds only grow
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsFalse(p.HasFoundKind("quartz")); // Not found yet
            p.RecordFoundKind("quartz"); // Find quartz
            p.RecordFoundKind("quartz"); // Find it again: no change
            p.RecordFoundKind("mica"); // Find mica
            Assert.IsTrue(p.HasFoundKind("quartz")); // Quartz stays found
            Assert.IsTrue(p.HasFoundKind("mica")); // Mica is found
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // A clear does not remove kinds
            Assert.IsTrue(p.HasFoundKind("quartz")); // Quartz still found
        }
    }
}
