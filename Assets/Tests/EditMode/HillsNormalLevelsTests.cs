using System.Linq; // Gives list searches and counts
using NUnit.Framework; // Gives the test tools

namespace LandGrand.Rules.Tests // Tests the plain level data
{ // Opens the tests
    public class HillsNormalLevelsTests // Checks the Hills Normal data
    { // Opens the checks
        static LevelData Normal(int number) => LevelCatalog.Find(Episode.Hills, Difficulty.Normal, number); // Gets one trail
        static LevelData[] All => LevelCatalog.All.Where(l => l.Episode == Episode.Hills && l.Difficulty == Difficulty.Normal).ToArray(); // Gets all Hills Normal trails
        static bool Overlaps(float left, float right, float x, float width) => left <= x + width && x <= right; // Checks whether two stretches touch

        [Test] // Checks that all five trails are available
        public void CatalogHasFiveHillsNormalLevelsInOrder() // Keeps the trail order and scene names
        { // Opens the check
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, All.Select(l => l.Number)); // Five levels in order
            CollectionAssert.AreEqual(new[] { "HillsNormal1", "HillsNormal2", "HillsNormal3", "HillsNormal4", "HillsNormal5" }, All.Select(l => l.SceneName)); // The five scenes
        } // Ends the check

        [TestCase(1, LevelTheme.IslandHills, "Hundred Islands", "lesson_hundred_islands", 3, 2, 3, 5, 84f)] // First trail and the high island ledge
        [TestCase(2, LevelTheme.LimestoneCave, "Callao Cave", "lesson_callao_cave", 3, 2, 3, 4, 84f)] // Cave and its secret trail
        [TestCase(3, LevelTheme.RiverGorge, "Pagsanjan Gorge", "lesson_pagsanjan", 3, 3, 3, 4, 84f)] // Gorge and its secret ledge
        [TestCase(4, LevelTheme.RiceTerraces, "Banaue Rice Terraces", "lesson_banaue", 3, 3, 4, 4, 84f)] // Terraces and the high terrace
        [TestCase(5, LevelTheme.FairyChimneys, "Cappadocia", "lesson_cappadocia", 4, 3, 2, 5, 112f)] // Balloons and the high secret ledge
        public void LevelUsesNormalSizesAndItsAuthoredContent(int number, LevelTheme theme, string banner, string picture, int pits, int spikes, int ledges, int secretIndex, float secretY) // Checks the trail size, objects and draft
        { // Opens the check
            var level = Normal(number); // Reads the described trail
            Assert.AreEqual(7200f, level.Layout.Width); // The shared trail width
            Assert.AreEqual(new Spot(40f, 0f), level.Start); // Same starting point
            Assert.AreEqual(new Spot(7040f, 0f), level.Goal); // Goal near the right edge
            CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, level.Checkpoints); // Two flags at fixed places
            Assert.AreEqual(180, level.TimeLimitSeconds); // Three minutes
            Assert.AreEqual(7, level.Pickups.Count); // Seven pickups
            Assert.AreEqual(7, level.Layout.Boulders.Count); // Seven boulders
            Assert.AreEqual(ledges, level.Layout.Ledges.Count); // The listed ledges
            Assert.AreEqual(pits, level.Hazards.Count(h => h.Type == HazardType.Pit)); // The listed pits
            Assert.AreEqual(spikes, level.Hazards.Count(h => h.Type == HazardType.Spikes && h.Width == 32f)); // The listed spike strips
            Assert.AreEqual(theme, level.Layout.Theme); // Each trail has its own art
            Assert.AreEqual(banner, level.BannerName); // The place name at the start
            Assert.AreEqual(picture, level.Lesson.Picture); // The right lesson art
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Four short lines
            Assert.IsTrue(level.Lesson.Sentences.All(s => s.Length <= 44)); // Each fits on one line
            Assert.AreEqual(3, level.Review.Count); // Three review lines
            Assert.IsTrue(level.Lesson.IsDraft && level.HasDraftContent); // The material needs teacher review
            Spot secret = level.Pickups[secretIndex].Place; // The listed secret pickup
            Assert.AreEqual(secretY, secret.Y); // It is on the high route
            Assert.IsTrue(level.Layout.Ledges.Any(l => l.Top == secret.Y && Overlaps(l.X, l.X + l.Width, secret.X, 0f))); // It lies on a ledge
        } // Ends the check

        [TestCase(1)] // Checks the island trail
        [TestCase(2)] // Checks the cave trail
        [TestCase(3)] // Checks the river trail
        [TestCase(4)] // Checks the terrace trail
        [TestCase(5)] // Checks the balloon trail
        public void FlagsAndNewObjectsHaveTheirRequiredClearance(int number) // Checks the bot's safety distances
        { // Opens the check
            var level = Normal(number); // Reads the described trail
            var layout = level.Layout; // Reads each object list
            var batSpans = layout.Bats.Select(b => (Left: b.Center.X - b.RadiusX - Bat.Width / 2, Width: b.RadiusX * 2 + Bat.Width)).ToArray(); // Covers the full reach of each loop
            foreach (Spot flag in level.Checkpoints) // Checks both safe flags
            { // Opens one flag
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(flag.X - 100f, flag.X + 100f, h.Place.X, h.Width))); // Pits and spikes stay away
                Assert.IsFalse(layout.Pools.Any(p => Overlaps(flag.X - 100f, flag.X + 100f, p.X, p.Width))); // Safe pools stay away too
                Assert.IsFalse(batSpans.Any(b => Overlaps(flag.X - 100f, flag.X + 100f, b.Left, b.Width))); // Bat loops stay away
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(flag.X - 40f, flag.X + 40f, c.X, c.Width))); // Current strips stay away
                Assert.IsFalse(layout.MovingPlatforms.Any(m => Overlaps(flag.X - 40f, flag.X + 40f, m.X0, MovingPlatform.Width))); // Balloon baskets stay away
            } // Ends the flag checks
            foreach (var hazard in level.Hazards) // Checks the space around each pit or spike
            { // Opens one hazard
                Assert.That(hazard.Place.X, Is.GreaterThanOrEqualTo(200f)); // Gives the start time to teach
                Assert.That(hazard.Place.X + hazard.Width, Is.LessThanOrEqualTo(level.Goal.X - 120f)); // Leaves the goal clear
                Assert.IsFalse(batSpans.Any(b => Overlaps(b.Left - 80f, b.Left + b.Width + 80f, hazard.Place.X, hazard.Width))); // Bats cannot push into a nearby hazard
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(c.X - 80f, c.X + c.Width + 80f, hazard.Place.X, hazard.Width))); // Water currents also leave safe banks
            } // Ends the hazard checks
            foreach (var m in layout.MovingPlatforms) // Checks the balloon route extents
            { // Opens one basket
                Assert.That(m.HighTop - m.Top, Is.InRange(MovingPlatform.BalloonMinRise, MovingPlatform.BalloonMaxRise)); // Stays in the checked rise range
                Assert.IsFalse(level.Hazards.Any(h => h.Type == HazardType.Pit && Overlaps(m.X0, m.X0 + MovingPlatform.Width, h.Place.X, h.Width))); // Every basket starts on the near bank
            } // Ends the balloon checks
        } // Ends the check

        [Test] // Checks the first encounters
        public void IntroducingLevelsGiveOneSafePracticeSpotPerNewObject() // Lets the explorer learn without a nearby hazard
        { // Opens the check
            var island = Normal(1); // First float level
            var pool = island.Layout.Pools.Single(); // The only safe pool
            Assert.AreEqual(800f, pool.X); // Its planned left edge
            Assert.AreEqual(96f, pool.Width); // Its full width
            Assert.AreEqual(16f, SinkingFloat.PoolDepth); // Its solid floor sits sixteen pixels down
            var raft = island.Layout.Floats.First(); // The practice raft
            Assert.AreEqual(pool.X, raft.WaterX); // This raft floats over the pool
            Assert.IsFalse(island.Hazards.Any(h => Overlaps(pool.X - 80f, pool.X + pool.Width + 80f, h.Place.X, h.Width))); // The pool is safe and clear
            var cave = Normal(2); // First gentle bat level
            var bat = cave.Layout.Bats.First(); // The practice bat
            float batReach = bat.RadiusX + Bat.Width / 2f + 80f; // Covers the bat's hit box and surrounding path
            Assert.IsFalse(cave.Hazards.Any(h => Overlaps(bat.Center.X - batReach, bat.Center.X + batReach, h.Place.X, h.Width))); // Has clear ground both ways
            Assert.AreEqual(1, Normal(3).Layout.Currents.First().Direction); // The first current helps travel to the right
            var chimney = Normal(5); // First balloon level
            var balloon = chimney.Layout.MovingPlatforms.First(); // The practice balloon
            Assert.IsFalse(chimney.Hazards.Any(h => Overlaps(balloon.X0 - 80f, balloon.X0 + MovingPlatform.Width + 80f, h.Place.X, h.Width))); // Has clear ground under and near the basket
        } // Ends the check

        [Test] // Checks the new objects by trail
        public void WaterBatsCurrentsAndBalloonsAreOnTheirAuthoredTrails() // Checks the mix without placing anything on old trails
        { // Opens the check
            CollectionAssert.AreEqual(new[] { 4, 0, 0, 1, 0 }, All.Select(l => l.Layout.Floats.Count)); // Rafts in N1 and N4
            CollectionAssert.AreEqual(new[] { 0, 3, 0, 1, 0 }, All.Select(l => l.Layout.Bats.Count)); // Bats in N2 and N4
            CollectionAssert.AreEqual(new[] { 0, 0, 3, 1, 0 }, All.Select(l => l.Layout.Currents.Count)); // Currents in N3 and N4
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 4 }, All.Select(l => l.Layout.MovingPlatforms.Count)); // Balloons in N5 alone
            CollectionAssert.AreEqual(new[] { 3, 0, 0, 1, 0 }, All.Select(l => l.Hazards.Count(h => h.IsWater && h.Type == HazardType.Pit))); // Marks the four water pits
            CollectionAssert.AreEqual(new[] { 1, -1, 1 }, Normal(3).Layout.Currents.Select(c => c.Direction)); // The middle gorge strip flows left
            Assert.AreEqual(-1, Normal(4).Layout.Currents.Single().Direction); // The irrigation channel flows left
        } // Ends the check

        [Test] // Checks the Normal hazard return
        public void HillsNormalWaterUsesTheFurthestReachedCheckpoint() // Treats a fall in water as a Normal pit
        { // Opens the check
            var level = Normal(1); // Uses the level with water pits
            var run = new LevelRun(level); // Starts an attempt
            Assert.IsFalse(run.PitReturnsNearby); // Uses the Normal hazard rule
            Assert.AreEqual(level.Start, run.FallIntoPit(new Spot(1799f, 0f))); // Returns to the start before flags
            run.ReachCheckpoint(0); // Passes the first flag
            run.ReachCheckpoint(1); // Passes the second flag
            Assert.AreEqual(level.Checkpoints[1], run.FallIntoPit(new Spot(6099f, 0f))); // Returns to the furthest flag
        } // Ends the check

        [Test] // Checks the special cave audio
        public void OnlyCallaoOverridesHillsNormalAmbience() // Leaves the other trails on the stream
        { // Opens the check
            CollectionAssert.AreEqual(new[] { "amb_stream_normal", "amb_cave_drips", "amb_stream_normal", "amb_stream_normal", "amb_stream_normal" }, All.Select(LandformAudio.AmbienceFor)); // Only the cave changes it
        } // Ends the check

        [Test] // Checks the Collection expansion
        public void SevenAuthoredDraftKindsAppendToTheHillsShelf() // Leaves the Mountains shelf alone
        { // Opens the check
            var shelves = Collection.Shelves(new PlayerProgress()); // Reads every shelf
            Assert.AreEqual(34, shelves[(int)Episode.Mountains].Slots.Count); // The original Mountains shelf size
            Assert.AreEqual(23, shelves[(int)Episode.Hills].Slots.Count); // The shelf includes the Hard kinds
            var newKinds = ItemCatalog.All.Skip(37).Take(7).ToArray(); // Reads only the Normal kinds
            CollectionAssert.AreEqual(new[] { "aragonite", "coquina", "flowstone", "dripstone", "volcanic_breccia", "ignimbrite", "zeolite" }, newKinds.Select(k => k.Id)); // Stable catalog order
            Assert.IsTrue(newKinds.All(k => k.IsDraft && k.Landforms.SequenceEqual(new[] { Episode.Hills }) && k.Description.Length <= 80 && k.Picture == k.Id)); // Hills draft pictures and short item cards
            CollectionAssert.AreEqual(new[] { false, true, true, true, true, true, false }, newKinds.Select(k => k.IsRock)); // The authored mineral and rock labels
        } // Ends the check

        [Test] // Checks isolation from the earlier levels
        public void MountainsAndHillsEasyHaveNoHillsNormalObjectsOrOverrides() // Keeps old layouts on their own rules
        { // Opens the check
            var older = LevelCatalog.All.Where(l => l.Episode == Episode.Mountains || l.Difficulty == Difficulty.Easy); // Finds levels already shipped
            foreach (var level in older) // Checks each older trail
            { // Opens one trail
                var layout = level.Layout; // Reads its object data
                Assert.IsEmpty(layout.Pools, level.SceneName); // No practice water
                Assert.IsEmpty(layout.Floats, level.SceneName); // No rafts
                Assert.IsEmpty(layout.Bats, level.SceneName); // No gentle bats
                Assert.IsEmpty(layout.Currents, level.SceneName); // No river strips
                Assert.IsFalse(layout.MovingPlatforms.Any(p => p.IsVertical), level.SceneName); // No balloons
                Assert.IsNull(layout.AmbienceOverride, level.SceneName); // Keeps its table ambience
            } // Ends the trail check
        } // Ends the check
    } // Ends the checks
} // Ends the tests
