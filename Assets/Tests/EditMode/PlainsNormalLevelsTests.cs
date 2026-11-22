using System.Linq; // Gives list checks for the authored trails
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class PlainsNormalLevelsTests // Checks the five Plains trails
    { // Opens the trail checks
        static LevelData Normal(int number) => LevelCatalog.Find(Episode.Plains, Difficulty.Normal, number); // Finds one Plains Normal trail

        [TestCase(1, LevelTheme.CandabaWetland, "Candaba Swamp", "Wetland Plains", "lesson_candaba", 2, true, 4312f, 84f, "peat peat clay siltstone mudstone peat clay")] // Checks Candaba Swamp
        [TestCase(2, LevelTheme.LaharPlain, "Pinatubo Lahar Plain", "Volcanic Plains", "lesson_pinatubo_lahar", 3, false, 4012f, 84f, "pumice dacite andesite pumice dacite andesite pumice")] // Checks Pinatubo Lahar Plain
        [TestCase(3, LevelTheme.RiverDelta, "Agno River Delta", "Deltas", "lesson_agno", 3, true, 3712f, 84f, "gold quartz gold siltstone gold quartz mudstone")] // Checks Agno River Delta
        [TestCase(4, LevelTheme.FloatingVillage, "Agusan Marsh", "Living on Wetlands", "lesson_agusan", 3, true, 4112f, 84f, "peat clay mudstone peat clay mudstone peat")] // Checks Agusan Marsh
        [TestCase(5, LevelTheme.FloatingMarket, "Mekong Delta", "River Deltas of the World", "lesson_mekong", 3, true, 4112f, 84f, "laterite clay laterite siltstone laterite clay mudstone")] // Checks Mekong Delta
        public void LevelHasNormalSizesAndItsAuthoredContent(int number, LevelTheme theme, string banner, string title, string picture, int spikes, bool water, float secretX, float secretY, string kinds) // Checks the level data
        { // Starts the trail check
            var level = Normal(number); // Reads this trail
            Assert.AreEqual("PlainsNormal" + number, level.SceneName); // Uses the common scene name
            Assert.AreEqual(7200f, level.Layout.Width); // The shared Normal width
            Assert.AreEqual(new Spot(40f, 0f), level.Start); // Starts on the ground
            Assert.AreEqual(new Spot(7040f, 0f), level.Goal); // Finishes on the ground
            CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, level.Checkpoints); // Uses both Normal flags
            Assert.AreEqual(180, level.TimeLimitSeconds); // Gives three minutes
            Assert.AreEqual(theme, level.Layout.Theme); // Uses its own theme
            Assert.AreEqual(banner, level.BannerName); // Names the place in the banner
            Assert.AreEqual(title, level.Lesson.Title); // Uses the lesson title
            Assert.AreEqual(picture, level.Lesson.Picture); // Uses the matching illustration
            Assert.IsTrue(level.Lesson.IsDraft && level.HasDraftContent); // Shows the teacher review note
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Four lesson lines
            Assert.IsTrue(level.Lesson.Sentences.All(line => line.Length <= 44)); // Keeps each line short
            Assert.AreEqual(3, level.Review.Count); // Three review lines
            Assert.AreEqual(7, level.Pickups.Count); // Seven pickups on each trail
            CollectionAssert.AreEqual(kinds.Split(' '), level.Pickups.Select(p => p.Kind.Id)); // Keeps all kinds in trail order
            Assert.IsTrue(level.Pickups.All(p => p.Kind == ItemCatalog.Find(p.Kind.Id))); // Uses the shared item records
            var secret = level.Pickups.Single(p => p.Place.Y >= 72f); // Exactly one high pickup
            Assert.AreEqual(new Spot(secretX, secretY), secret.Place); // Keeps the secret in place
            Assert.IsTrue(level.Layout.Ledges.Any(l => l.Top == secretY && l.X <= secretX && l.X + l.Width >= secretX)); // The secret is on its ledge
            var pits = level.Hazards.Where(h => h.Type == HazardType.Pit).ToArray(); // Reads the gaps that can cause a fall
            Assert.AreEqual(3, pits.Length); // Each trail has three pits
            Assert.IsTrue(pits.All(p => p.IsWater == water)); // Only the lahar pits are dry
            Assert.AreEqual(spikes, level.Hazards.Count(h => h.Type == HazardType.Spikes && h.Width == 32f)); // Checks all the spike strips
            foreach (var flag in level.Checkpoints) // Checks both flags
                Assert.IsTrue(level.Hazards.All(h => h.Place.X >= flag.X + 100f || h.Place.X + h.Width <= flag.X - 100f)); // Gives each flag its safe space
            Assert.IsNull(level.Layout.AmbienceOverride); // Uses the normal Plains ambience from the table
            Assert.AreEqual("amb_wetland_normal", LandformAudio.AmbienceFor(level)); // Uses the wetland loop
        } // Ends the trail check

        [Test] // Checks key layouts from earlier releases
        public void EarlierLevelsKeepTheirOriginalCrossings() // Pins old routes, floats, the pool and straw piles
        { // Starts the older trail check
            var pulag = LevelCatalog.Find(Episode.Mountains, Difficulty.Normal, 1); // Reads Mountains Normal 1
            Assert.AreEqual(7200f, pulag.Layout.Width); // Keeps the old trail width
            Assert.AreEqual(new Spot(7040f, 0f), pulag.Goal); // Keeps the old goal
            Assert.IsEmpty(pulag.Layout.MovingPlatforms); // Mount Pulag has no moving platforms
            var mossyForest = LevelCatalog.Find(Episode.Mountains, Difficulty.Normal, 3); // Reads an older trail with moving platforms
            var routes = mossyForest.Layout.MovingPlatforms.Select(p => (p.X0, p.X1, p.Top)); // Reads each platform route
            CollectionAssert.AreEqual(new[] { (1000f, 1200f, 40f), (2000f, 2152f, 0f), (5200f, 5392f, 0f), (6100f, 6252f, 0f) }, routes); // Keeps all four routes
            var islands = LevelCatalog.Find(Episode.Hills, Difficulty.Normal, 1).Layout; // Reads the Hundred Islands trail
            var floats = islands.Floats.Select(raft => (raft.X, raft.Top, raft.WaterX)); // Reads the rafts and the water under them
            CollectionAssert.AreEqual(new[] { (824f, 0f, 800f), (1828f, 0f, 1800f), (6132f, 0f, 6100f), (6212f, 0f, 6100f) }, floats); // Keeps all four rafts
            var pool = islands.Pools.Single(); // Reads the practice pool
            Assert.AreEqual((800f, 96f), (pool.X, pool.Width)); // Keeps the safe pool placement
            var river = LevelCatalog.Find(Episode.Plains, Difficulty.Easy, 3).Layout; // Reads Isabela River Plain
            CollectionAssert.AreEqual(new[] { 600f, 1200f, 3300f, 4500f }, river.StrawPiles.Select(pile => pile.X)); // Keeps the four straw piles
        } // Ends the older trail check

        [Test] // Checks the two practice crossings
        public void PracticeFloatUsesTheSafePoolAndFirstCurrentPushesRight() // Shows each object in its safe setup
        { // Starts the practice check
            var wetland = Normal(1).Layout; // Reads the float trail
            var firstFloat = wetland.Floats.OrderBy(raft => raft.X).First(); // Finds the first float
            var pool = wetland.Pools.Single(); // Finds the safe practice pool
            Assert.AreEqual(pool.X, firstFloat.WaterX); // Uses the pool water line
            Assert.That(firstFloat.X, Is.InRange(pool.X, pool.X + pool.Width - SinkingFloat.Width)); // Keeps the float over the pool
            Assert.AreEqual(1, Normal(3).Layout.Currents.OrderBy(current => current.X).First().Direction); // Pushes right on the first strip
        } // Ends the practice check

        [Test] // Checks both lahar bridges and the marsh mix
        public void LaharAndMarshReuseTheAuthoredObjects() // Keeps the placed objects on their trails
        { // Starts the object check
            var lahar = Normal(2).Layout; // Reads the lahar trail
            CollectionAssert.AreEqual(new[] { 1300f, 1332f, 1364f, 5700f, 5732f, 5764f }, lahar.CrumbleLedges.Select(ledge => ledge.X)); // Three slabs in each of the two bridges
            Assert.IsTrue(lahar.CrumbleLedges.All(ledge => ledge.Top == 0f)); // Bridges start at ground level
            Assert.AreEqual(4, lahar.SandZones.Count); // The four sand stretches
            var marsh = Normal(4).Layout; // Reads the marsh trail
            Assert.AreEqual(1828f, marsh.Floats.Single().X); // One float on the water
            Assert.AreEqual(3000f, marsh.SandZones.Single().X); // One sand stretch
            Assert.AreEqual(5000f, marsh.Currents.Single().X); // One current strip
        } // Ends the object check

        [Test] // Checks every boat against the water below it
        public void MarketBoatsSpanThePoolAndPitsAtGroundLevel() // Needs a boat across each wide crossing
        { // Starts the boat check
            var level = Normal(5); // Reads the Mekong trail
            var gaps = level.Layout.Pools.Select(pool => (pool.X, pool.Width)).Concat(level.Hazards.Where(h => h.Type == HazardType.Pit).Select(h => (h.Place.X, h.Width))); // Reads the safe pool and three pits
            var boats = level.Layout.MovingPlatforms; // Reads the four boat routes
            Assert.AreEqual(4, boats.Count); // One boat per crossing
            foreach (var gap in gaps) // Checks the whole reach of each crossing
            { // Starts a crossing check
                var boat = boats.Single(platform => platform.X0 == gap.X); // Starts on the left bank
                Assert.That(boat.X1 + MovingPlatform.Width, Is.GreaterThanOrEqualTo(gap.X + gap.Width)); // Reaches the right bank
                Assert.AreEqual(0f, boat.Top); // The deck stays at ground height
                Assert.IsFalse(boat.IsVertical); // The boat moves sideways
            } // Ends a crossing check
        } // Ends the boat check
    } // Ends the trail checks
} // Closes the tests
