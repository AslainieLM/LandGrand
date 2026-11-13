using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks one attempt at a level (ticket 07): it starts at the start and clears at the goal.</summary>
    public class LevelRunTests // A group of checks
    {
        static LevelData Level => LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 1); // Mountains Easy 1

        [Test] // Unity runs this method as a Test
        public void NewRun_IsPlayingFromTheStart() // A fresh attempt
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.AreEqual(RunOutcome.Playing, run.Outcome); // Still playing
            Assert.AreEqual(Level.Start.X, run.RespawnPoint.X); // The explorer appears at the start
            Assert.AreEqual(Level.Start.Y, run.RespawnPoint.Y); // At the start's height too
            Assert.AreEqual(0, run.Stars); // No stars before the goal
        }

        [Test] // Unity runs this method as a Test
        public void ReachingTheGoal_ClearsTheLevelWithStars() // The end of the level
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.IsTrue(run.ReachGoal()); // The first touch of the goal counts
            Assert.AreEqual(RunOutcome.Cleared, run.Outcome); // The level is cleared
            Assert.AreEqual(1, run.Stars); // Nothing collected still gives 1 star (D-039)
        }

        [Test] // Unity runs this method as a Test
        public void ReachingTheGoalAgain_ChangesNothing() // The explorer may keep touching the goal
        {
            var run = new LevelRun(Level); // Start an attempt
            run.ReachGoal(); // First touch
            Assert.IsFalse(run.ReachGoal()); // Later touches do not count again
            Assert.AreEqual(RunOutcome.Cleared, run.Outcome); // Still cleared
        }

        [Test] // Unity runs this method as a Test
        public void ARunThatIsNotCleared_SavesNothing() // Leaving early keeps progress as it was
        {
            var progress = new PlayerProgress(); // A brand new profile
            new LevelRun(Level).RecordResult(progress); // Record before the goal
            Assert.IsFalse(progress.IsCleared(Episode.Mountains, Difficulty.Easy, 1)); // Not cleared
        }

        [Test] // Unity runs this method as a Test
        public void AClearedRun_IsSavedAndOpensTheNextLevel() // The clear goes into the progress
        {
            var progress = new PlayerProgress(); // A brand new profile
            var run = new LevelRun(Level); // Start an attempt
            run.ReachGoal(); // Clear it
            run.RecordResult(progress); // Save the result
            Assert.IsTrue(progress.IsCleared(Episode.Mountains, Difficulty.Easy, 1)); // Cleared
            Assert.AreEqual(1, progress.BestStars(Episode.Mountains, Difficulty.Easy, 1)); // With its stars
            Assert.IsTrue(progress.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, 2)); // Level 2 opens
        }
    }
}
