using LandGrand.Rules; // Uses the boulder rules
using NUnit.Framework; // Uses the test checks

namespace LandGrand.Tests // Holds rule tests
{ // Opens the test group
    public class RollingBoulderTests // Checks the roll and its warning
    { // Opens the tests
        readonly RollingBoulder boulder = new RollingBoulder(900f, 700f); // One boulder with a 200 pixel run

        [Test] // Runs the timing check
        public void BoulderWarnsThenRollsLeftUntilItPassesTheEnd() // Checks the whole cycle
        { // Starts the check
            Assert.IsTrue(boulder.IsWarning(0.799f)); // Still warning just before the start
            Assert.IsTrue(float.IsNaN(boulder.XAt(0.799f))); // No boulder during the warning
            Assert.IsFalse(boulder.IsWarning(0.8f)); // The warning just ended
            Assert.AreEqual(900f, boulder.XAt(0.8f)); // Starts at the spawn
            Assert.AreEqual(810f, boulder.XAt(1.8f), 0.001f); // Rolls 90 pixels in a second
            Assert.IsTrue(float.IsNaN(boulder.XAt(3.1f))); // Already past the end
            Assert.IsTrue(boulder.IsWarning(3.5f)); // Warns again next cycle
            Assert.AreEqual(243f, RollingBoulder.MaxRun, 0.001f); // Longest roll fits the time left
        } // Ends the timing check

        [Test] // Runs the offset check
        public void OffsetMovesTheWarningAndTheCycleIndex() // Starts one second ahead
        { // Starts the check
            var shifted = new RollingBoulder(900f, 700f, 1f); // Moves the cycle ahead
            Assert.AreEqual(882f, shifted.XAt(0f), 0.001f); // It has already rolled 18 pixels
            Assert.AreEqual(1, shifted.CycleIndex(2.5f)); // The next cycle starts sooner
            Assert.AreEqual(0f, shifted.Phase(2.5f)); // At the beginning of that cycle
        } // Ends the offset check

        [Test] // Runs the hit box check
        public void HitBoxIsFourteenPixelsEachWayAboveGround() // Checks both edges and a jump
        { // Starts the check
            Assert.IsFalse(boulder.Hits(898f, 0f, 4f, 14f, 0.7f)); // The warning cannot hurt
            Assert.IsTrue(boulder.Hits(893f, 0f, 14f, 14f, 0.8f)); // Its whole box can hurt
            Assert.IsFalse(boulder.Hits(891f, 0f, 2f, 14f, 0.8f)); // Touching the left edge is safe
            Assert.IsFalse(boulder.Hits(907f, 0f, 2f, 14f, 0.8f)); // Touching the right edge is safe
            Assert.IsFalse(boulder.Hits(898f, 14f, 4f, 14f, 0.8f)); // A jump clears it at 14 pixels
            Assert.IsFalse(boulder.Hits(898f, -14f, 4f, 14f, 0.8f)); // The box starts on the ground
        } // Ends the hit box check

        [Test] // Runs the skip check
        public void OnlyClosenessAtTheStartCanSkipThatCycle() // Checks both sides of the warning
        { // Starts the check
            var run = new RollingBoulderRun(boulder); // Starts the object
            run.Update(0.79f, 840f); // Far away during the warning
            run.Update(0.80f, 853f); // Now 47 pixels away at the start
            Assert.IsTrue(float.IsNaN(run.CurrentX(0.80f))); // Skips the roll
            Assert.IsFalse(run.Hits(893f, 0f, 14f, 14f, 0.80f)); // The skipped roll cannot hit
            run.Update(1f, 600f); // Moving away cannot bring it back
            Assert.IsTrue(float.IsNaN(run.CurrentX(1f))); // This cycle remains skipped
            run.Update(4.29f, 900f); // Close just before the next start
            run.Update(4.31f, 852f); // Exactly 48 pixels is far enough
            Assert.IsFalse(float.IsNaN(run.CurrentX(4.31f))); // The next roll is allowed
            run.Update(4.4f, 900f); // Moving closer after the start cannot cancel it
            Assert.IsFalse(float.IsNaN(run.CurrentX(4.4f))); // Keeps the roll
        } // Ends the skip check
    } // Ends the tests
} // Ends the test group
