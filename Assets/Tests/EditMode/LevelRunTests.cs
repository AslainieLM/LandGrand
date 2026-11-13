using System.Linq; // Gives list helpers such as ToArray and All
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

        // ---- Ticket 09: hazards and the checkpoint ----

        [Test] // Unity runs this method as a Test
        public void AHazardBeforeTheCheckpoint_SendsTheExplorerBackToTheStart() // No checkpoint reached yet
        {
            var run = new LevelRun(Level); // Start an attempt
            var back = run.HitHazard(); // Touch a trap or fall in the pit
            Assert.AreEqual(Level.Start.X, back.X); // Back at the start
            Assert.AreEqual(Level.Start.Y, back.Y); // At its height too
        }

        [Test] // Unity runs this method as a Test
        public void ReachingTheCheckpoint_MovesTheRespawnPoint() // The flag in the middle
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.IsFalse(run.CheckpointReached); // Not reached yet
            Assert.IsTrue(run.ReachCheckpoint()); // The first touch counts
            Assert.IsTrue(run.CheckpointReached); // Now shown as reached
            Assert.AreEqual(Level.Checkpoint.X, run.RespawnPoint.X); // The explorer comes back here now
            Assert.IsFalse(run.ReachCheckpoint()); // Touching it again changes nothing
        }

        [Test] // Unity runs this method as a Test
        public void AHazardAfterTheCheckpoint_SendsTheExplorerBackToTheCheckpoint() // The last checkpoint reached
        {
            var run = new LevelRun(Level); // Start an attempt
            run.ReachCheckpoint(); // Pass the flag
            var back = run.HitHazard(); // Then fall in the pit
            Assert.AreEqual(Level.Checkpoint.X, back.X); // Back at the flag
        }

        [Test] // Unity runs this method as a Test
        public void AHazard_KeepsCollectedItemsAndKeepsPlaying() // D-040: no health bar, items stay collected
        {
            var progress = new PlayerProgress(); // A profile
            var run = new LevelRun(Level); // Start an attempt
            run.Collect(0, progress); // Take one item
            run.DismissFind(); // Close the pop-up
            run.HitHazard(); // Touch a trap
            Assert.AreEqual(1, run.Collected); // The item stays collected
            Assert.IsTrue(run.IsCollected(0)); // And cannot be taken twice
            Assert.AreEqual(RunOutcome.Playing, run.Outcome); // Still playing
        }

        [Test] // Unity runs this method as a Test
        public void Respawns_AreCounted() // Useful for the result later
        {
            var run = new LevelRun(Level); // Start an attempt
            run.HitHazard(); // First fall
            run.HitHazard(); // Second fall
            Assert.AreEqual(2, run.Respawns); // Both counted
        }

        [Test] // Unity runs this method as a Test
        public void AfterTheGoal_HazardsAndCheckpointsDoNothing() // The level is over
        {
            var run = new LevelRun(Level); // Start an attempt
            run.ReachGoal(); // Clear the level
            run.HitHazard(); // Touch a trap
            Assert.AreEqual(0, run.Respawns); // Not counted
            Assert.IsFalse(run.ReachCheckpoint()); // The flag does nothing now
        }

        // ---- Ticket 10: timer, pause and time up ----

        [Test] // Unity runs this method as a Test
        public void ANewRun_HasTheFullTimeOfItsDifficulty() // Easy gives 2 minutes (D-039)
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.AreEqual(120f, run.RemainingSeconds, 0.001f); // 2:00
            Assert.AreEqual(Level.TimeLimitSeconds, (int)run.RemainingSeconds); // Same as the level data
            Assert.IsFalse(run.IsTimeUp); // Time is not up
        }

        [Test] // Unity runs this method as a Test
        public void TimeCountsDownWhilePlaying() // The countdown
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Tick(2.5f); // 2.5 seconds pass
            Assert.AreEqual(117.5f, run.RemainingSeconds, 0.001f); // That much less time
        }

        [Test] // Unity runs this method as a Test
        public void TimeStopsWhileThePauseMenuIsOpen() // The pause button
        {
            var run = new LevelRun(Level); // Start an attempt
            Assert.IsTrue(run.Pause()); // Open the pause menu
            run.Tick(10f); // Time passes in the real world
            Assert.AreEqual(120f, run.RemainingSeconds, 0.001f); // The game clock did not move
            run.Resume(); // Close the menu
            run.Tick(10f); // Time passes again
            Assert.AreEqual(110f, run.RemainingSeconds, 0.001f); // Now it counts
        }

        [Test] // Unity runs this method as a Test
        public void TimeStopsWhileThePopupIsOpen() // First-find pop-up
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Collect(0, new PlayerProgress()); // First find opens the pop-up
            run.Tick(10f); // Time passes in the real world
            Assert.AreEqual(120f, run.RemainingSeconds, 0.001f); // The game clock did not move
            run.DismissFind(); // OK
            run.Tick(1f); // One second passes
            Assert.AreEqual(119f, run.RemainingSeconds, 0.001f); // It counts again
        }

        [Test] // Unity runs this method as a Test
        public void ReachingZero_GivesTimeUpAndStopsTheRun() // The countdown ends
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Tick(119f); // Almost there
            Assert.IsFalse(run.IsTimeUp); // Not yet
            run.Tick(5f); // Past zero
            Assert.IsTrue(run.IsTimeUp); // Time is up
            Assert.AreEqual(RunOutcome.TimeUp, run.Outcome); // The outcome says so
            Assert.AreEqual(0f, run.RemainingSeconds, 0.001f); // Never below zero
        }

        [Test] // Unity runs this method as a Test
        public void AfterTimeUp_NothingCountsAnyMore() // The attempt is over
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Tick(200f); // Time runs out
            Assert.AreEqual(CollectResult.None, run.Collect(0, new PlayerProgress())); // No more collecting
            Assert.IsFalse(run.ReachGoal()); // No clearing
            Assert.IsFalse(run.Pause()); // No pause menu either
            Assert.AreEqual(0, run.Stars); // No stars
        }

        [Test] // Unity runs this method as a Test
        public void AClearedRun_StopsTheClock() // The goal ends the countdown
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Tick(30f); // Some time passes
            run.ReachGoal(); // Clear it
            run.Tick(30f); // More time passes
            Assert.AreEqual(90f, run.RemainingSeconds, 0.001f); // Unchanged after the goal
            Assert.AreEqual(RunOutcome.Cleared, run.Outcome); // Still cleared
        }

        [Test] // Unity runs this method as a Test
        public void AHazard_KeepsTheTimerRunning() // D-040: the timer keeps running after a respawn
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Tick(20f); // Some time passes
            run.HitHazard(); // Touch a trap
            Assert.AreEqual(100f, run.RemainingSeconds, 0.001f); // Time is not reset or refunded
            run.Tick(5f); // Time goes on
            Assert.AreEqual(95f, run.RemainingSeconds, 0.001f); // Still counting
        }

        [Test] // Unity runs this method as a Test
        public void ThePauseMenu_CannotOpenBehindThePopup() // One thing at a time
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Collect(0, new PlayerProgress()); // Pop-up open
            Assert.IsFalse(run.Pause()); // The pause menu does not open
        }

        [Test] // Unity runs this method as a Test
        public void WhilePaused_ThePickupsAndTheGoalWait() // The game is stopped
        {
            var run = new LevelRun(Level); // Start an attempt
            run.Pause(); // Open the menu
            Assert.IsTrue(run.IsPaused); // Counts as paused
            Assert.AreEqual(CollectResult.None, run.Collect(0, new PlayerProgress())); // No collecting
            Assert.IsFalse(run.ReachGoal()); // No clearing
        }

        [Test] // Unity runs this method as a Test
        public void ARetry_IsAFreshAttempt() // Retry starts again from the lesson, so the next attempt is brand new
        {
            var progress = new PlayerProgress(); // A profile
            var first = new LevelRun(Level); // First attempt
            first.Collect(0, progress); // Take an item
            first.Tick(50f); // Use some time
            first.ReachCheckpoint(); // Pass the flag
            var retry = new LevelRun(Level); // The next attempt after Retry
            Assert.AreEqual(0, retry.Collected); // Nothing collected
            Assert.AreEqual(120f, retry.RemainingSeconds, 0.001f); // Full time
            Assert.IsFalse(retry.CheckpointReached); // Flag not reached
            Assert.AreEqual(RunOutcome.Playing, retry.Outcome); // Playing
        }

        // ---- Ticket 11: what the Result Screen shows ----

        [Test] // Unity runs this method as a Test
        public void TakenFlags_MatchTheItemsCollected() // The item row on the Result Screen
        {
            var run = new LevelRun(Level); // Start an attempt
            var progress = new PlayerProgress(); // A profile
            run.Collect(1, progress); // Quartz
            run.DismissFind(); // Close the pop-up
            run.Collect(3, progress); // Mica
            run.DismissFind(); // Close the pop-up
            CollectionAssert.AreEqual(new[] { false, true, false, true, false }, run.TakenFlags.ToArray()); // Only those two are taken
        }

        [Test] // Unity runs this method as a Test
        public void GoalResults_ShowCollectAllAndFinishInTime() // The goals with ticks
        {
            var progress = new PlayerProgress(); // A profile
            var run = new LevelRun(Level); // Start an attempt
            for (int i = 0; i < 3; i++) { run.Collect(i, progress); run.DismissFind(); } // Take three of five
            run.ReachGoal(); // Finish
            var goals = run.GoalResults.ToArray(); // The goal lines
            Assert.AreEqual("Collect 5/5", goals[0].Text); // First goal
            Assert.IsFalse(goals[0].Met); // Not everything was collected
            Assert.AreEqual("Finish before time runs out", goals[1].Text); // Second goal
            Assert.IsTrue(goals[1].Met); // The goal was reached in time
        }

        [Test] // Unity runs this method as a Test
        public void GoalResults_AllMetWhenEverythingIsCollected() // A perfect run
        {
            var progress = new PlayerProgress(); // A profile
            var run = new LevelRun(Level); // Start an attempt
            for (int i = 0; i < 5; i++) { run.Collect(i, progress); run.DismissFind(); } // Take all five
            run.ReachGoal(); // Finish
            Assert.IsTrue(run.GoalResults.All(g => g.Met)); // Both ticked
        }

        // ---- Easy mode: a fall into the pit returns the explorer a short way back, not to the checkpoint ----

        static LevelData NormalLevel => new LevelData(Episode.Mountains, Difficulty.Normal, 1, // A Normal level made for this test
            new Lesson("Mountains", new[] { "A sentence." }, "lesson_mountains", true), // A tiny lesson
            new Pickup[0], new Spot(32f, 0f), new Spot(640f, 0f), new Hazard[0], new Spot(1380f, 0f)); // No items, no hazards

        [Test] // Unity runs this method as a Test
        public void EasyFallIntoThePit_ReturnsToTheSafeSpotNearTheEdge() // The new Easy rule
        {
            var run = new LevelRun(Level); // Easy level
            Assert.IsTrue(run.PitReturnsNearby); // The rule applies on Easy
            var back = run.FallIntoPit(new Spot(1850f, 0f)); // The explorer was last on the ground here
            Assert.AreEqual(1850f, back.X); // Back at that spot, not at the start
            Assert.AreEqual(1, run.Respawns); // Counted
        }

        [Test] // Unity runs this method as a Test
        public void EasyFallIntoThePit_IgnoresTheCheckpoint() // Even after the flag was passed
        {
            var run = new LevelRun(Level); // Easy level
            run.ReachCheckpoint(); // Pass the flag
            var back = run.FallIntoPit(new Spot(2450f, 0f)); // A fall after the flag
            Assert.AreEqual(2450f, back.X); // Near the fall, not at the flag
        }

        [Test] // Unity runs this method as a Test
        public void EasySpikes_StillSendTheExplorerToTheCheckpoint() // Only pits changed
        {
            var run = new LevelRun(Level); // Easy level
            run.ReachCheckpoint(); // Pass the flag
            Assert.AreEqual(Level.Checkpoint.X, run.HitHazard().X); // Spikes use the checkpoint rule (D-040)
        }

        [Test] // Unity runs this method as a Test
        public void NormalFallIntoThePit_StillGoesToTheCheckpoint() // The short return is for Easy only
        {
            var run = new LevelRun(NormalLevel); // Normal level
            Assert.IsFalse(run.PitReturnsNearby); // The rule does not apply
            var back = run.FallIntoPit(new Spot(300f, 0f)); // Ignored on Normal
            Assert.AreEqual(32f, back.X); // Back at the start
            run.ReachCheckpoint(); // Pass the flag
            Assert.AreEqual(640f, run.FallIntoPit(new Spot(900f, 0f)).X); // Then back at the checkpoint
        }

        [Test] // Unity runs this method as a Test
        public void AFallIntoThePit_KeepsItemsAndTimeAndCountsOnlyWhilePlaying() // Same promises as other hazards
        {
            var progress = new PlayerProgress(); // A profile
            var run = new LevelRun(Level); // Easy level
            run.Collect(0, progress); // Take an item
            run.DismissFind(); // Close the pop-up
            run.Tick(10f); // Some time passes
            run.FallIntoPit(new Spot(1850f, 0f)); // Fall in the pit
            Assert.AreEqual(1, run.Collected); // The item stays
            Assert.AreEqual(110f, run.RemainingSeconds, 0.001f); // The timer is untouched
            run.ReachGoal(); // Clear the level
            run.FallIntoPit(new Spot(1850f, 0f)); // A late fall
            Assert.AreEqual(1, run.Respawns); // Not counted after the end
        }
    }
}
