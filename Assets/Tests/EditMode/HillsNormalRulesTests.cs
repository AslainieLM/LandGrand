using NUnit.Framework; // Gives the NUnit test tools

namespace LandGrand.Rules.Tests // Tests the plain level rules
{ // Opens the tests
    public class HillsNormalRulesTests // Checks the new object rules
    { // Opens the checks
        [TestCase(0f, 0f, 0f, 0f, true)] // A dry raft holds
        [TestCase(0.6f, 0f, 0f, 0f, true)] // The wait lasts six tenths
        [TestCase(1.6f, 0f, 0f, -8f, true)] // One second of sinking
        [TestCase(2.59f, 0f, 0f, -15.92f, true)] // Still holds before full depth
        [TestCase(2.6f, 0f, 0f, -16f, false)] // Loses support at full depth
        [TestCase(3f, 1.5f, 0f, -16f, false)] // Stays down during the rise delay
        [TestCase(3f, 2f, 0f, -8f, true)] // Rises at sixteen pixels per second
        [TestCase(3f, 2.5f, 0f, 0f, true)] // Stops at the top
        [TestCase(0.2f, 0f, 8f, -8f, true)] // Landing again restarts the wait
        [TestCase(1.1f, 0f, 8f, -12f, true)] // A new landing sinks from its current depth
        [TestCase(0f, 1.75f, 8f, -4f, true)] // A partly sunk float also rises
        public void FloatTopAndSupport(float standing, float off, float startingDepth, float top, bool holds) // Checks the wait, sink and return
        { // Opens the check
            var raft = new SinkingFloat(824f, 0f, 800f); // Places the practice raft
            float actualTop = raft.TopAt(standing, off, startingDepth); // Reads the water height change
            Assert.AreEqual(top, actualTop, 0.001f); // Allow rounding in pixel math
            Assert.AreEqual(holds, raft.Holds(actualTop)); // Checks the support edge
        } // Ends the check

        [Test] // Runs the complete float transition check
        public void FloatRestartsItsWaitWhenRiddenDuringRecovery() // Keeps the depth until the new wait ends
        { // Opens the check
            var raft = new SinkingFloatState(new SinkingFloat(824f, 0f, 800f), 0f); // Starts a fresh raft
            raft.Step(0f, true); // Steps on at the water line
            Assert.AreEqual(-8f, raft.Step(1.6f, true), 0.001f); // Sinks for a second
            Assert.AreEqual(-8f, raft.Step(1.7f, false), 0.001f); // Steps off and waits
            Assert.AreEqual(-8f, raft.Step(3.1f, false), 0.001f); // Holds during the rise delay
            Assert.AreEqual(-4f, raft.Step(3.35f, false), 0.001f); // Rises four pixels after a quarter-second
            Assert.AreEqual(-4f, raft.Step(3.36f, true), 0.001f); // Lands again and stops rising
            Assert.AreEqual(-4f, raft.Step(3.9f, true), 0.001f); // Still in the fresh wait
            Assert.AreEqual(-8f, raft.Step(4.45f, true), 0.001f); // Sinks again from the depth already reached
        } // Ends the check

        [TestCase(40.2f, 39.6f, 40f, 40.67f, true)] // A rising top catches descending feet
        [TestCase(40.2f, 40.5f, 40f, 40.67f, true)] // A rising top can also catch slower rising feet
        [TestCase(39f, 41f, 40f, 40.5f, false)] // Jumping from below passes through
        [TestCase(40.2f, 39.8f, 40.67f, 40f, false)] // Falling below the earlier top gives no landing
        [TestCase(40.8f, 39.8f, 40.67f, 40f, true)] // Falling from above can land on a descending basket
        [TestCase(40.2f, 40f, 40f, 40f, true)] // Stationary slabs keep the old landing
        public void OneWayTopChecksRelativeMotion(float feetBefore, float feetAfter, float oldTop, float top, bool lands) // Sweeps vertical platforms without blocking from below
        { // Opens the check
            Assert.AreEqual(lands, OneWayTop.Crosses(feetBefore, feetAfter, oldTop, top)); // Checks the two positions over one step
        } // Ends the check

        [Test] // Checks the pool floor and recovering raft
        public void ReappearingRaftRisesPastFeetOnThePoolFloor() // The submerged top cannot catch the explorer
        { // Opens the check
            Assert.IsFalse(OneWayTop.Crosses(-16f, -16.1f, -16f, -15.999f, false)); // The first solid step starts above the explorer
            Assert.IsFalse(OneWayTop.Crosses(-16f, -16.1f, -15.999f, -15.7f)); // The next rising step also starts above the explorer
            Assert.IsTrue(OneWayTop.Crosses(-15.9f, -16.1f, -16f, -15.999f, false)); // A falling explorer above the new top can still land
        } // Ends the check

        [TestCase(0f, 124f, 16f)] // Starts at the right edge
        [TestCase(0.75f, 100f, 28f)] // Reaches the top
        [TestCase(1.5f, 76f, 16f)] // Reaches the left edge
        [TestCase(2.25f, 100f, 4f)] // Reaches the bottom
        [TestCase(3f, 124f, 16f)] // Loops to its first spot
        public void BatPositionFollowsAnEllipse(float time, float x, float y) // Checks each quarter turn
        { // Opens the check
            var bat = new Bat(100f, 16f, 24f, 12f, 3f, 0f); // Places a bat with no phase offset
            Spot position = bat.PositionAt(time); // Reads the bat center
            Assert.AreEqual(x, position.X, 0.001f); // Checks the horizontal radius
            Assert.AreEqual(y, position.Y, 0.001f); // Checks the vertical radius
        } // Ends the check

        [Test] // Runs the bat touch check
        public void BatPhaseAndTouchUseTheBodyEdges() // Checks phase and contact bounds
        { // Opens the check
            var bat = new Bat(100f, 16f, 24f, 12f, 3f, 0.75f); // Starts at the top of the loop
            Spot center = bat.PositionAt(0f); // Gets the first center
            Assert.AreEqual(28f, center.Y); // Phase puts it at the top already
            Assert.IsTrue(Bat.Touches(new Spot(86.01f, 0f), center)); // A small overlap at the left counts
            Assert.IsFalse(Bat.Touches(new Spot(86f, 0f), center)); // Just meeting the left edge does not count
            Assert.IsFalse(Bat.Touches(new Spot(100f, 34f), center)); // Just meeting the top edge does not count
            Assert.IsFalse(Bat.Touches(new Spot(100f, -4f), center)); // Just meeting the lower edge does not count
        } // Ends the check

        [Test] // Runs the bat push check
        public void BatPushGoesAwayAndHasAOneSecondRest() // Checks push direction and timing
        { // Opens the check
            Assert.AreEqual(-1, Bat.PushDirection(99f, 100f, 1)); // Pushes left on the left
            Assert.AreEqual(1, Bat.PushDirection(101f, 100f, -1)); // Pushes right on the right
            Assert.AreEqual(1, Bat.PushDirection(100f, 100f, -1)); // Pushes against a left walk under the bat
            Assert.AreEqual(-1, Bat.PushDirection(100f, 100f, 0)); // Uses the bot's leftward tie-break when idle
            Assert.AreEqual(24f, Bat.PushSpeed * Bat.PushSeconds); // The whole push spans twenty-four pixels
            Assert.IsFalse(Bat.CanPush(0.999f)); // Still resting just before one second
            Assert.IsTrue(Bat.CanPush(1f)); // Can push again at one second
        } // Ends the check

        [TestCase(true, false, 0f, 100f, 1, 30f)] // Includes the left edge
        [TestCase(true, false, 0f, 340f, -1, -30f)] // Includes the right edge and reverses direction
        [TestCase(true, false, 0f, 340.01f, 1, 0f)] // Stops beyond the strip
        [TestCase(false, false, 0f, 200f, 1, 0f)] // Airborne feet do not count
        [TestCase(true, true, 0f, 200f, 1, 0f)] // Standing on another solid does not count
        [TestCase(true, false, -0.01f, 200f, 1, 0f)] // The flat footing tolerance excludes its limit
        public void CurrentPushNeedsFlatGroundInTheStrip(bool grounded, bool onSolid, float feetY, float feetX, int direction, float push) // Checks the water edges and footing
        { // Opens the check
            var current = new CurrentZone(100f, 240f, direction); // Sets up a directed strip
            Assert.AreEqual(push, current.PushAt(grounded, onSolid, feetY, feetX)); // Reads the ground push
        } // Ends the check

        [TestCase(-1f, 1040f)] // Keeps the old negative-time wrap
        [TestCase(0f, 1000f)] // Starts at the left end
        [TestCase(1.25f, 1050f)] // Moves right at forty pixels per second
        [TestCase(5f, 1200f)] // Reaches the right end
        [TestCase(8.75f, 1050f)] // Moves left at forty pixels per second
        [TestCase(10f, 1000f)] // Returns to the start
        public void MountainsPlatformKeepsItsHorizontalPositions(float time, float x) // Checks the Mountains Normal route at both ends and in between
        { // Opens the check
            var platform = new MovingPlatform(1000f, 1200f, 40f); // Uses the existing Mountains placement
            Assert.AreEqual(x, platform.LeftAt(time)); // Matches the original left edge
            Assert.AreEqual(40f, platform.TopAt(time)); // Keeps the original flat height
        } // Ends the check

        [TestCase(0f, 24f, true)] // Starts at the low end going up
        [TestCase(0.5f, 44f, true)] // Rises at forty pixels per second
        [TestCase(2.4f, 120f, false)] // Turns at the high end
        [TestCase(3.3f, 84f, false)] // Falls at forty pixels per second
        [TestCase(4.8f, 24f, true)] // Starts the next climb
        public void BalloonMovesVertically(float time, float top, bool rising) // Checks a whole round trip
        { // Opens the check
            var platform = MovingPlatform.Vertical(4144f, 24f, 120f, 0f); // Places the crossing basket
            Assert.AreEqual(4144f, platform.LeftAt(time)); // The basket stays at the same x
            Assert.AreEqual(top, platform.TopAt(time), 0.001f); // The basket moves only up and down
            Assert.AreEqual(rising, platform.IsRisingAt(time)); // The burner follows the direction
        } // Ends the check

        [Test] // Runs the phase check
        public void BalloonPhaseStartsPartwayAlongTheRoute() // Checks the named layout offset
        { // Opens the check
            var platform = MovingPlatform.Vertical(2900f, 24f, 96f, 0.5f); // Places the second balloon
            Assert.AreEqual(44f, platform.TopAt(0f)); // The trip has advanced by twenty pixels
        } // Ends the check

        [TestCase(null, "amb_stream_normal", "make_hills_audio.py")] // Uses the table when no override is set
        [TestCase("amb_cave_drips", "amb_cave_drips", "make_hills_normal_audio.py")] // Uses the authored cave loop
        public void LevelAmbienceResolvesItsFileAndSourceScript(string sound, string file, string script) // Checks both parts of the audio lookup
        { // Opens the check
            var layout = new LevelLayout(200f, LevelTheme.Meadow, new Boulder[0], new Ledge[0], ambienceOverride: sound); // Supplies optional ambience
            var level = new LevelData(Episode.Hills, Difficulty.Normal, 1, null, new Pickup[0], new Spot(40f, 0f), new Spot[0], new Hazard[0], new Spot(140f, 0f), layout: layout); // Makes a small test level
            Assert.AreEqual(file, LandformAudio.AmbienceFor(level)); // Chooses the clip name
            Assert.AreEqual(script, LandformAudio.AmbienceScriptFor(level)); // Chooses the generator for the missing clip message
        } // Ends the check
    } // Ends the checks
} // Ends the tests
