using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks what the difficulty cards show: stars per difficulty and the guide on locked cards.</summary>
    public class DifficultySelectionRulesTests // A group of checks
    {
        [Test] // Unity runs this method as a Test
        public void EachDifficulty_HasThreeStarsPerLevel() // Easy 0/9, Normal 0/15, Hard 0/21
        {
            Assert.AreEqual(9, PlayerProgress.MaxDifficultyStars(Difficulty.Easy)); // 3 levels
            Assert.AreEqual(15, PlayerProgress.MaxDifficultyStars(Difficulty.Normal)); // 5 levels
            Assert.AreEqual(21, PlayerProgress.MaxDifficultyStars(Difficulty.Hard)); // 7 levels
        }

        [Test] // Unity runs this method as a Test
        public void DifficultyStars_AddsTheBestStarsOfThatDifficultyOnly() // Stars are counted per card
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.AreEqual(0, p.DifficultyStars(Episode.Mountains, Difficulty.Easy)); // Nothing yet
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // 3 stars on Easy 1
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 2); // 2 stars on Easy 2
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // A worse replay keeps 2
            p.RecordClear(Episode.Mountains, Difficulty.Normal, 1, 1); // Another difficulty
            p.RecordClear(Episode.Hills, Difficulty.Easy, 1, 3); // Another landform
            Assert.AreEqual(5, p.DifficultyStars(Episode.Mountains, Difficulty.Easy)); // 3 + 2
            Assert.AreEqual(1, p.DifficultyStars(Episode.Mountains, Difficulty.Normal)); // Counted apart
            Assert.AreEqual(0, p.DifficultyStars(Episode.Mountains, Difficulty.Hard)); // Nothing here
        }

        [Test] // Unity runs this method as a Test
        public void LockedCards_AskToClearTheDifficultyBefore() // The card guide names the difficulty before it
        {
            Assert.AreEqual("Clear all Easy levels", RegionGuide.ForLockedDifficulty(Difficulty.Normal)); // Normal needs Easy
            Assert.AreEqual("Clear all Normal levels", RegionGuide.ForLockedDifficulty(Difficulty.Hard)); // Hard needs Normal
        }

        [Test] // Unity runs this method as a Test
        public void Easy_HasNoLockedGuide() // Easy is never locked in an open landform
        {
            Assert.AreEqual("", RegionGuide.ForLockedDifficulty(Difficulty.Easy)); // Nothing to clear before it
        }
    }
}
