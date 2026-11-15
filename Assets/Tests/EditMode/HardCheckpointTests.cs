using LandGrand.Rules; // Uses the level rules
using NUnit.Framework; // Uses the test checks

namespace LandGrand.Tests // Holds the rule tests
{ // Opens the test group
    public class HardCheckpointTests // Checks three flags and the new layout data
    { // Opens the tests
        [Test] // Runs the checkpoint check
        public void HardHazardsReturnToTheFurthestOfThreeFlags() // Keeps the last point reached
        { // Sets up a Hard attempt
            var lesson = LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 1).Lesson; // Reuses a lesson
            var flags = new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }; // Places three flags
            var data = new LevelData(Episode.Mountains, Difficulty.Hard, 1, lesson, new Pickup[0], new Spot(40f, 0f), flags, new Hazard[0], new Spot(9440f, 0f)); // Makes the trail
            var run = new LevelRun(data); // Starts the attempt
            var nearby = new Spot(8000f, 0f); // A nearby spot Hard should ignore
            Assert.AreEqual(40f, run.TouchTrap(nearby).X); // Starts with no flag
            for (int i = 0; i < flags.Length; i++) // Reaches each flag
            { // Checks the next flag
                Assert.IsTrue(run.ReachCheckpoint(i)); // The new flag counts
                Assert.AreEqual(flags[i].X, run.FallIntoPit(nearby).X); // A fall uses this flag
            } // Ends the flag checks
            Assert.IsFalse(run.ReachCheckpoint(1)); // An earlier flag cannot move the return point
            Assert.AreEqual(7200f, run.TouchTrap(nearby).X); // A trap uses the third flag
        } // Ends the checkpoint check

        [Test] // Runs the layout check
        public void HardLayoutListsDefaultToEmptyAndKeepTheirOwnCopies() // Protects the placed objects
        { // Starts the check
            var empty = LevelLayout.Plain(9600f); // Makes a trail with no objects
            Assert.IsEmpty(empty.RollingBoulders); // No rolling boulders by default
            Assert.IsEmpty(empty.IceZones); // No ice by default
            var boulders = new[] { new RollingBoulder(900f, 700f) }; // Places one rolling boulder
            var ice = new[] { new IceZone(600f, 600f) }; // Places one ice zone
            var layout = new LevelLayout(9600f, LevelTheme.LavaSlope, new Boulder[0], new Ledge[0], rollingBoulders: boulders, iceZones: ice); // Gives both lists to the layout
            boulders[0] = null; // Changes the source list
            ice[0] = null; // Changes the source list
            Assert.AreEqual(900f, layout.RollingBoulders[0].Spawn); // Keeps the boulder
            Assert.AreEqual(600f, layout.IceZones[0].Width); // Keeps the ice
        } // Ends the layout check
    } // Ends the tests
} // Ends the test group
