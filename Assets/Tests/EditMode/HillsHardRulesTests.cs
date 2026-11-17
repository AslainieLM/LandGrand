using LandGrand.Rules; // Uses the plain game rules
using NUnit.Framework; // Uses the test checks

namespace LandGrand.Tests // Holds rule tests
{ // Opens the test group
    public class HillsHardRulesTests // Checks mud, logs and their sounds
    { // Opens the tests
        [TestCase(499f, true, false, 0f, false)] // Just outside the mud
        [TestCase(500f, true, false, 0f, true)] // The left edge counts
        [TestCase(800f, true, false, 0f, true)] // The right edge counts
        [TestCase(801f, true, false, 0f, false)] // Just past the mud
        [TestCase(600f, false, false, 0f, false)] // Air does not count
        [TestCase(600f, true, true, 0f, false)] // A moving top does not count
        [TestCase(600f, true, false, 24f, false)] // A raised top does not count
        public void MudUsesTheSameFootingAndSpeedAsIce(float x, bool grounded, bool onSolid, float y, bool expected) // Checks the same step on both surfaces
        { // Starts the check
            var mud = new MudZone(500f, 300f); // Places the practice mud
            var ice = new IceZone(500f, 300f); // Places ice over the same ground
            bool onMud = ZoneFooting.IsStandingIn(grounded, onSolid, y, x, mud.Contains); // Reads the mud underfoot
            bool onIce = ZoneFooting.IsStandingIn(grounded, onSolid, y, x, ice.Contains); // Reads the ice underfoot
            Assert.AreEqual(expected, onMud); // Checks the authored footing rule
            Assert.AreEqual(onIce, onMud); // Both surfaces use the same footing
            Assert.AreEqual(expected ? 59f : 0f, onMud ? IceSlide.Next(84f, 0f, 0.25f) : 0f); // Both surfaces slow the same way
        } // Ends the check

        [TestCase(-1, 720f, 810f)] // Rolls to the left
        [TestCase(1, 1080f, 990f)] // Rolls to the right
        public void LogWarnsRollsAndLeavesAtItsEnd(int direction, float end, float afterOneSecond) // Checks a full shifted cycle
        { // Starts the check
            var log = new RollingLog(900f, end, direction, 0.5f); // Starts the warning half a second early
            var run = new RollingLogRun(log); // Starts the attempt
            Assert.IsTrue(log.IsWarning(0.299f)); // Still warning just before the start
            Assert.IsTrue(float.IsNaN(run.CurrentX(0.299f))); // No log during the warning
            run.Update(0.3f, 600f); // The explorer is far from the spawn
            Assert.IsFalse(log.IsWarning(0.3f)); // Warning ends at this clock time
            Assert.AreEqual(900f, run.CurrentX(0.3f), 0.001f); // The log starts at its spawn
            Assert.AreEqual(afterOneSecond, run.CurrentX(1.3f), 0.001f); // It moves 90 pixels in a second
            Assert.IsTrue(run.Hits(afterOneSecond - 7f, 0f, 14f, 14f, 1.3f)); // Overlapping the log on the ground is a trap
            Assert.IsFalse(run.Hits(afterOneSecond - 7f, 14f, 14f, 14f, 1.3f)); // A hop above its box is safe
            Assert.AreEqual(end, run.CurrentX(2.3f), 0.001f); // The end itself still counts
            Assert.IsTrue(float.IsNaN(run.CurrentX(2.301f))); // The log disappears beyond the end
            Assert.IsTrue(log.IsWarning(3f)); // The next shifted cycle warns
        } // Ends the check

        [TestCase(-1)] // Left rolls give the explorer space
        [TestCase(1)] // Right rolls give the explorer space
        public void LogSkipsOnlyIfTheExplorerIsCloserThanFortyEightAtStart(int direction) // Checks the skip boundary
        { // Starts the check
            var run = new RollingLogRun(new RollingLog(900f, 900f + direction * 200f, direction)); // Sets a run in the chosen direction
            run.Update(0.79f, 600f); // The warning cannot decide the skip
            run.Update(0.8f, 900f - direction * 47f); // The explorer is 47 pixels from the spawn
            Assert.IsTrue(float.IsNaN(run.CurrentX(0.8f))); // The roll was skipped
            run.Update(1.8f, 600f); // The explorer moves away after the start
            Assert.IsTrue(float.IsNaN(run.CurrentX(1.8f))); // This roll stays skipped
            run.Update(4.31f, 900f + direction * 48f); // Exactly 48 pixels away counts as safe
            Assert.IsFalse(float.IsNaN(run.CurrentX(4.31f))); // The next roll can run
            run.Update(4.4f, 900f); // The explorer moves closer after the start
            Assert.IsFalse(float.IsNaN(run.CurrentX(4.4f))); // It cannot skip a live roll
        } // Ends the check

        [Test] // Checks the longest legal roll
        public void LogRunFitsInOneCycle() // Checks the 243 pixel limit
        { // Starts the check
            Assert.AreEqual(243f, RollingLog.MaxRun, 0.001f); // Leaves room for the warning
            Assert.DoesNotThrow(() => new RollingLog(900f, 1143f, 1)); // The full run fits
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new RollingLog(900f, 1144f, 1)); // A longer run would cross the cycle
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new RollingLog(900f, 1100f, -1)); // The end must follow the direction
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new RollingLog(900f, 1100f, 0)); // A roll needs a valid direction
        } // Ends the check

        [Test] // Checks the existing boulder route
        public void BoulderStillRollsLeftAtItsOriginalSpeed() // Pins the Mountains timing
        { // Starts the check
            var boulder = new RollingBoulder(900f, 700f, 0.5f); // Gives the roll an offset
            Assert.IsTrue(boulder.IsWarning(0.29f)); // Warning ends at 0.3 seconds
            Assert.AreEqual(900f, boulder.XAt(0.3f), 0.001f); // Starts at the spawn
            Assert.AreEqual(810f, boulder.XAt(1.3f), 0.001f); // Moves left by 90 pixels in one second
            Assert.IsTrue(float.IsNaN(boulder.XAt(2.6f))); // Gone past its end
        } // Ends the check

        [TestCase("amb_wind_turbines")] // Pililla has its own loop
        [TestCase("amb_city_hills")] // Rome has its own loop
        public void HardAmbienceNamesItsGenerator(string loop) // Checks the missing file hint
        { // Starts the check
            var layout = new LevelLayout(9600f, LevelTheme.Meadow, new Boulder[0], new Ledge[0], ambienceOverride: loop); // Sets a level loop
            var level = new LevelData(Episode.Hills, Difficulty.Hard, 1, null, new Pickup[0], new Spot(40f, 0f), new Spot[0], new Hazard[0], new Spot(9440f, 0f), layout: layout); // Uses the chosen loop
            Assert.AreEqual("make_hills_hard_audio.py", LandformAudio.AmbienceScriptFor(level)); // Points to the shared script
        } // Ends the check
    } // Ends the tests
} // Ends the test group