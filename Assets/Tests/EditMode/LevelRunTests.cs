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

        // ---- Ticket 08: collecting and the first-find pop-up ----

        // Pickups of Mountains Easy 1 in order: 0 granite, 1 quartz, 2 feldspar, 3 mica, 4 granite.

        [Test] // Unity runs this method as a Test
        public void CollectingAPickup_CountsOnceAndAsksForThePopupOnAFirstFind() // A kind never found before
        {
            var run = new LevelRun(Level); // Start an attempt
            var progress = new PlayerProgress(); // Nothing found yet
            Assert.AreEqual(CollectResult.FirstFind, run.Collect(0, progress)); // First granite ever: the pop-up is wanted
            Assert.AreEqual(1, run.Collected); // It counts
            Assert.AreEqual("granite", run.PendingFind.Id); // The pop-up shows granite
            Assert.IsTrue(run.IsCollected(0)); // This pickup is taken
        }

        [Test] // Unity runs this method as a Test
        public void TouchingACollectedPickupAgain_CountsNothing() // Each pickup counts once
        {
            var run = new LevelRun(Level); // Start an attempt
            var progress = new PlayerProgress(); // Nothing found yet
            run.Collect(0, progress); // Take it
            run.DismissFind(); // Close the pop-up
            Assert.AreEqual(CollectResult.None, run.Collect(0, progress)); // Nothing new
            Assert.AreEqual(1, run.Collected); // Still one
        }

        [Test] // Unity runs this method as a Test
        public void ASecondPickupOfAFoundKind_OnlyChimes() // Granite again in the same level
        {
            var run = new LevelRun(Level); // Start an attempt
            var progress = new PlayerProgress(); // Nothing found yet
            run.Collect(0, progress); // First granite: pop-up
            run.DismissFind(); // Close it
            Assert.AreEqual(CollectResult.Repeat, run.Collect(4, progress)); // Second granite: chime and sparkle only
            Assert.IsNull(run.PendingFind); // No pop-up
            Assert.AreEqual(2, run.Collected); // Both count
        }

        [Test] // Unity runs this method as a Test
        public void AKindFoundInAnEarlierSession_DoesNotAskForThePopup() // Found kinds are saved
        {
            var progress = new PlayerProgress(); // A profile
            progress.RecordFoundKind("quartz"); // Quartz was found on an earlier play
            var run = new LevelRun(Level); // Start a new attempt
            Assert.AreEqual(CollectResult.Repeat, run.Collect(1, progress)); // Quartz is known: chime only
            Assert.IsNull(run.PendingFind); // No pop-up
        }

        [Test] // Unity runs this method as a Test
        public void AFirstFind_IsRememberedInTheProgress() // So the next attempt knows it
        {
            var progress = new PlayerProgress(); // A profile
            new LevelRun(Level).Collect(3, progress); // Find mica
            Assert.IsTrue(progress.HasFoundKind("mica")); // Saved as found
        }

        [Test] // Unity runs this method as a Test
        public void WhileThePopupIsOpen_TheRunIsPaused() // The game waits for OK
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.IsFalse(run.IsPaused); // Not paused at first
            run.Collect(0, new PlayerProgress()); // First find
            Assert.IsTrue(run.IsPaused); // Paused for the pop-up
            run.DismissFind(); // OK
            Assert.IsFalse(run.IsPaused); // Playing again
            Assert.IsNull(run.PendingFind); // Nothing waiting
        }

        [Test] // Unity runs this method as a Test
        public void ABadPickupNumber_IsIgnored() // Safe against wrong numbers
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.AreEqual(CollectResult.None, run.Collect(-1, new PlayerProgress())); // Too low
            Assert.AreEqual(CollectResult.None, run.Collect(99, new PlayerProgress())); // Too high
            Assert.AreEqual(0, run.Collected); // Nothing counted
        }

        [Test] // Unity runs this method as a Test
        public void TheGoalWaitsForThePopup() // The level cannot end behind an open pop-up
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Collect(0, new PlayerProgress()); // Pop-up open
            Assert.IsFalse(run.ReachGoal()); // Not cleared yet
            run.DismissFind(); // OK
            Assert.IsTrue(run.ReachGoal()); // Now it clears
        }

        [Test] // Unity runs this method as a Test
        public void StarsFollowTheCollectedCount() // The star rule from ticket 01 (0-40% 1 star, 41-99% 2, 100% 3)
        {
            var progress = new PlayerProgress(); // A profile
            var two = new LevelRun(Level); // 3 of 5 = 60%
            for (int i = 0; i < 3; i++) { two.Collect(i, progress); two.DismissFind(); } // Take three
            two.ReachGoal(); // Finish
            Assert.AreEqual(2, two.Stars); // Two stars
            var all = new LevelRun(Level); // All five
            for (int i = 0; i < 5; i++) { all.Collect(i, progress); all.DismissFind(); } // Take them all
            all.ReachGoal(); // Finish
            Assert.AreEqual(3, all.Stars); // Three stars
        }
    }
}
