using LandGrand.Rules; // Uses the level rules
using NUnit.Framework; // Uses the test checks

namespace LandGrand.Tests // Holds rule tests
{ // Opens the test group
    public class NormalCheckpointTests // Checks the two flags
    { // Opens the tests
        [Test] // Runs this check
        public void FurthestCheckpointWinsAndNeverMovesBack() // Keeps the latest safe flag
        { // Starts the check
            var easy = LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 1); // Reuses the lesson
            var data = new LevelData(Episode.Mountains, Difficulty.Normal, 1, easy.Lesson, new Pickup[0], new Spot(40f, 0f), new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, new Hazard[0], new Spot(7040f, 0f)); // Makes two flags
            var run = new LevelRun(data); // Starts the attempt
            Assert.AreEqual(2, data.Checkpoints.Count); // Keeps both flags
            Assert.AreEqual(40f, run.RespawnPoint.X); // Starts at the beginning
            Assert.IsTrue(run.ReachCheckpoint(0)); // Reaches the first flag
            Assert.AreEqual(2400f, run.RespawnPoint.X); // Returns to the first flag
            Assert.IsTrue(run.ReachCheckpoint(1)); // Reaches the second flag
            Assert.AreEqual(4800f, run.RespawnPoint.X); // Returns to the second flag
            Assert.IsFalse(run.ReachCheckpoint(0)); // Cannot move the return point back
            Assert.AreEqual(4800f, run.HitHazard().X); // A hazard uses the furthest flag
        } // Ends the check
        [Test] // Runs this check
        public void LayoutListsAreEmptyUnlessSuppliedAndKeepTheirOwnCopies() // Keeps optional data safe
        { // Starts the check
            var empty = LevelLayout.Plain(7200f); // Makes a plain trail
            Assert.IsEmpty(empty.CrumbleLedges); // No breakable ledges
            Assert.IsEmpty(empty.Rockfalls); // No falling rocks
            Assert.IsEmpty(empty.MovingPlatforms); // No moving platforms
            Assert.IsEmpty(empty.WindZones); // No gusts
            var ledges = new[] { new CrumbleLedge(2000f, 0f) }; // One bridge slab
            var rocks = new[] { new Rockfall(1100f, 1f) }; // One falling rock
            var platforms = new[] { new MovingPlatform(2000f, 2152f, 0f) }; // One crossing
            var winds = new[] { new WindZone(900f, 500f, 0f) }; // One gust area
            var layout = new LevelLayout(7200f, LevelTheme.PineForest, new Boulder[0], new Ledge[0], crumbleLedges: ledges, rockfalls: rocks, movingPlatforms: platforms, windZones: winds); // Supplies the four lists
            ledges[0] = null; rocks[0] = null; platforms[0] = null; winds[0] = null; // Changes the caller's arrays
            Assert.AreEqual(2000f, layout.CrumbleLedges[0].X); // Keeps the slab
            Assert.AreEqual(1100f, layout.Rockfalls[0].X); // Keeps the rock
            Assert.AreEqual(2152f, layout.MovingPlatforms[0].X1); // Keeps the crossing
            Assert.AreEqual(500f, layout.WindZones[0].Width); // Keeps the gust area
        } // Ends the check
    } // Ends the tests
} // Ends the test group
