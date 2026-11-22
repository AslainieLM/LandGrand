using System; // Gives the ends of a log run
using System.Linq; // Gives list searches and counts
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class HillsHardLevelsTests // Checks the authored trails
    { // Opens the checks
        static LevelData Hard(int number) => LevelCatalog.Find(Episode.Hills, Difficulty.Hard, number); // Finds one trail
        static LevelData[] All => LevelCatalog.All.Where(l => l.Episode == Episode.Hills && l.Difficulty == Difficulty.Hard).ToArray(); // Finds all seven trails
        static bool Overlaps(float left, float right, float x, float width) => left < x + width && x < right; // Checks whether two spans cross

        [Test] // Checks the full trail list
        public void CatalogHasSevenHillsHardScenesInOrder() // Keeps the scene names and their order
        { // Opens the check
            CollectionAssert.AreEqual(Enumerable.Range(1, 7), All.Select(l => l.Number)); // Hard has seven levels
            CollectionAssert.AreEqual(Enumerable.Range(1, 7).Select(n => $"HillsHard{n}"), All.Select(l => l.SceneName)); // The builder has seven scene names
        } // Ends the check

        [TestCase(1, LevelTheme.TarsierForest, "Tarsier Forest", "lesson_tarsier_forest", 5, 6, 5, 6, 6204f, 84f, "limestone peridotite limestone peridotite limestone limestone peridotite limestone peridotite")] // Checks the Tarsier Forest trail
        [TestCase(2, LevelTheme.RainySlope, "Antipolo Slopes", "lesson_antipolo_slope", 6, 7, 3, 4, 3804f, 84f, "basalt chlorite basalt chlorite chlorite basalt basalt chlorite basalt")] // Checks the Antipolo Slopes trail
        [TestCase(3, LevelTheme.KarstRidge, "Masungi Georeserve", "lesson_masungi", 7, 6, 3, 6, 7004f, 96f, "limestone calcite limestone calcite limestone calcite calcite limestone calcite")] // Checks the Masungi Georeserve trail
        [TestCase(4, LevelTheme.WindHills, "Pililla Wind Farm", "lesson_pililla", 6, 5, 3, 6, 7584f, 112f, "andesite dacite andesite dacite andesite dacite dacite andesite andesite")] // Checks the Pililla Wind Farm trail
        [TestCase(5, LevelTheme.DrumlinBay, "Clew Bay", "lesson_clew_bay", 6, 5, 2, 7, 7004f, 84f, "schist quartzite schist quartzite schist quartzite schist quartzite schist")] // Checks the Clew Bay trail
        [TestCase(6, LevelTheme.CityHills, "Seven Hills of Rome", "lesson_rome_hills", 8, 5, 2, 5, 4204f, 84f, "tuff travertine leucite tuff travertine leucite tuff travertine leucite")] // Checks the Seven Hills of Rome trail
        [TestCase(7, LevelTheme.KarstTowers, "Guilin Karst", "lesson_guilin", 7, 3, 2, 6, 6564f, 96f, "limestone calcite limestone calcite limestone calcite calcite limestone calcite")] // Checks the Guilin Karst trail
        public void TrailHasHardSizesAndItsAuthoredPickups(int number, LevelTheme theme, string banner, string picture, int pits, int spikes, int ledges, int secret, float secretX, float secretY, string kinds) // Checks layout and lesson data
        { // Opens the check
            var level = Hard(number); // Reads the trail
            Assert.AreEqual(9600f, level.Layout.Width); // Uses the shared Hard width
            Assert.AreEqual(new Spot(40f, 0f), level.Start); // Starts near the left
            Assert.AreEqual(new Spot(9440f, 0f), level.Goal); // Ends near the right
            CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, level.Checkpoints); // Uses three safe flags
            Assert.AreEqual(240, level.TimeLimitSeconds); // Gives four minutes
            CollectionAssert.AreEqual(kinds.Split(' '), level.Pickups.Select(p => p.Kind.Id)); // Uses the nine kinds in trail order
            Assert.AreEqual(new Spot(secretX, secretY), level.Pickups[secret].Place); // Keeps the designated high pickup
            Assert.That(secretY, Is.GreaterThanOrEqualTo(72f)); // The secret sits high
            Assert.IsTrue(level.Layout.Ledges.Any(l => l.Top == secretY && l.X <= secretX && secretX <= l.X + l.Width)); // The secret sits on a ledge
            Assert.AreEqual(pits, level.Hazards.Count(h => h.Type == HazardType.Pit)); // Matches the pit list
            Assert.AreEqual(spikes, level.Hazards.Count(h => h.Type == HazardType.Spikes && h.Width == 32f)); // Matches the spike list
            Assert.AreEqual(ledges, level.Layout.Ledges.Count); // Matches the ledge list
            Assert.AreEqual(theme, level.Layout.Theme); // Each trail has its own theme
            Assert.AreEqual(banner, level.BannerName); // Shows the place at the start
            Assert.AreEqual(picture, level.Lesson.Picture); // Uses the matching lesson picture
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Uses four draft lines
            Assert.IsTrue(level.Lesson.Sentences.All(s => s.Length <= 44)); // The sentences fit the lesson
            Assert.IsTrue(level.HasDraftContent && level.Lesson.IsDraft); // Flags the text for teacher review
            Assert.AreEqual(3, level.Review.Count); // Shows three review lines
        } // Ends the check

        [Test] // Checks the first mud and log
        public void FirstMudAndLogHaveSafePracticeGround() // Leaves room to try each rule
        { // Opens the check
            var mud = Hard(1).Layout.MudZones.First(); // Reads the first mud
            Assert.AreEqual(500f, mud.X); // Starts on the practice stretch
            Assert.AreEqual(300f, mud.Width); // Gives room to feel the slide
            Assert.IsFalse(Hard(1).Hazards.Any(h => Overlaps(380f, 920f, h.Place.X, h.Width))); // Leaves both sides clear
            Assert.IsEmpty(Hard(1).Layout.Logs); // Mud is the only new object
            var log = Hard(2).Layout.Logs.First(); // Reads the first log
            Assert.AreEqual(1000f, log.Spawn); // Spawns on the right
            Assert.AreEqual(800f, log.End); // Ends on the left
            Assert.AreEqual(-1, log.Direction); // Rolls toward the explorer
            Assert.IsFalse(Hard(2).Hazards.Any(h => Overlaps(680f, 1120f, h.Place.X, h.Width))); // Leaves clear practice ground
        } // Ends the check

        [TestCase(1)] // Checks Tarsier Forest
        [TestCase(2)] // Checks Antipolo Slopes
        [TestCase(3)] // Checks Masungi Georeserve
        [TestCase(4)] // Checks Pililla Wind Farm
        [TestCase(5)] // Checks Clew Bay
        [TestCase(6)] // Checks Seven Hills of Rome
        [TestCase(7)] // Checks Guilin Karst
        public void HazardsAndZonesLeaveClearFlags(int number) // Checks the authored safety gaps
        { // Opens the check
            var level = Hard(number); // Reads the trail
            var layout = level.Layout; // Reads the object lists
            foreach (var flag in level.Checkpoints) // Checks each flag
            { // Opens the flag check
                float x = flag.X; // Measures from the flag
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(x - 100f, x + 100f, h.Place.X, h.Width))); // Keeps pits and spikes away
                Assert.IsFalse(layout.CrumbleLedges.Any(c => Overlaps(x - 100f, x + 100f, c.X, CrumbleLedge.Width))); // Keeps crumbling ledges away
                Assert.IsFalse(layout.Bats.Any(b => Overlaps(x - 100f, x + 100f, b.Center.X - b.RadiusX - Bat.Width / 2f, b.RadiusX * 2f + Bat.Width))); // Covers each full bat loop
                Assert.IsFalse(layout.MudZones.Any(m => Overlaps(x - 40f, x + 40f, m.X, m.Width))); // Keeps mud away
                Assert.IsFalse(layout.WindZones.Any(w => Overlaps(x - 40f, x + 40f, w.X, w.Width))); // Keeps gusts away
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(x - 40f, x + 40f, c.X, c.Width))); // Keeps currents away
                Assert.IsFalse(layout.Boulders.Any(b => b.X <= x && x <= b.X + b.Width)); // Keeps the flag off boulders
                Assert.IsFalse(layout.MovingPlatforms.Any(p => Overlaps(x - (p.IsVertical ? 40f : 100f), x + (p.IsVertical ? 40f : 100f), p.X0, p.X1 - p.X0 + MovingPlatform.Width))); // Keeps moving platforms and balloons away
                Assert.IsFalse(layout.Logs.Any(l => Overlaps(x - 100f, x + 100f, Math.Min(l.Spawn, l.End) - 8f, Math.Abs(l.End - l.Spawn) + 16f))); // Keeps the full log path away
            } // Ends the flag checks
            foreach (var log in layout.Logs) // Checks the ground along every log route
            { // Opens the route check
                float left = Math.Min(log.Spawn, log.End) - 8f; // Allows room for the log picture
                float right = Math.Max(log.Spawn, log.End) + 8f; // Allows room at the other end
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(left, right, h.Place.X, h.Width))); // Logs have clear ground
                Assert.IsFalse(layout.Boulders.Any(b => Overlaps(left, right, b.X, b.Width))); // Logs can pass without crossing a boulder
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(left, right, c.X, c.Width))); // Logs do not roll through water
                Assert.IsFalse(level.Pickups.Any(p => p.Place.Y == 0f && p.Place.X >= left && p.Place.X <= right)); // No ground pickup lies in the path
            } // Ends the route checks
        } // Ends the check

        [Test] // Checks where each moving object belongs
        public void EachTrailUsesItsAuthoredObjectsAndAmbience() // Checks the hard mix by trail
        { // Opens the check
            CollectionAssert.AreEqual(new[] { 7, 0, 7, 0, 4, 0, 4 }, All.Select(l => l.Layout.MudZones.Count)); // Counts the mud zones
            CollectionAssert.AreEqual(new[] { 0, 9, 0, 5, 5, 7, 4 }, All.Select(l => l.Layout.Logs.Count)); // Counts the logs
            CollectionAssert.AreEqual(new[] { 0, 0, 6, 0, 0, 0, 2 }, All.Select(l => l.Layout.Bats.Count)); // Counts the bats
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 4, 0, 0, 3 }, All.Select(l => l.Layout.WindZones.Count)); // Counts the wind zones
            CollectionAssert.AreEqual(new[] { 0, 0, 11, 0, 0, 0, 8 }, All.Select(l => l.Layout.CrumbleLedges.Count)); // Counts the crumble ledges
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 5, 0, 2 }, All.Select(l => l.Layout.Currents.Count)); // Counts the currents
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 0, 4, 3 }, All.Select(l => l.Layout.Floats.Count)); // Counts the floats
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 0, 4, 0 }, All.Select(l => l.Layout.MovingPlatforms.Count(p => !p.IsVertical))); // Counts the sideways platforms
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 2, 0, 0, 1 }, All.Select(l => l.Layout.MovingPlatforms.Count(p => p.IsVertical))); // Counts the balloon baskets
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 0, 3, 2 }, All.Select(l => l.Hazards.Count(h => h.IsWater))); // Counts the water pits
            CollectionAssert.AreEqual(new[] { "amb_rain_hard", "amb_rain_hard", "amb_rain_hard", "amb_wind_turbines", "amb_rain_hard", "amb_city_hills", "amb_rain_hard" }, All.Select(LandformAudio.AmbienceFor)); // Only Pililla and Rome use their own loops
        } // Ends the check

        [Test] // Checks the shelf and order
        public void SixDraftKindsAppendToTheHillsShelf() // Review kinds keep their existing shelves
        { // Opens the check
            var shelves = Collection.Shelves(new PlayerProgress()); // Reads the default shelves
            Assert.AreEqual(34, shelves[(int)Episode.Mountains].Slots.Count); // Keeps the Mountains count
            Assert.AreEqual(23, shelves[(int)Episode.Hills].Slots.Count); // Grows the Hills shelf by six
            var kinds = ItemCatalog.All.Skip(44).Take(6).ToArray(); // Reads the six Hills Hard additions
            CollectionAssert.AreEqual(new[] { "peridotite", "chlorite", "dacite", "quartzite", "travertine", "leucite" }, kinds.Select(k => k.Id)); // Keeps the new kind order
            Assert.IsTrue(kinds.All(k => k.IsDraft && k.Description.Length <= 80 && k.Picture == k.Id)); // Checks the original cards
            Assert.IsTrue(kinds.Where(k => k.Id != "dacite").All(k => k.Landforms.SequenceEqual(new[] { Episode.Hills }))); // Keeps the other five Hills-only kinds
            CollectionAssert.AreEqual(new[] { Episode.Hills, Episode.Plains }, ItemCatalog.Dacite.Landforms); // Dacite stays on Hills and joins Plains
            CollectionAssert.AreEqual(new[] { true, false, true, true, true, false }, kinds.Select(k => k.IsRock)); // Checks rock and mineral labels
            Assert.IsTrue(new[] { "limestone", "calcite", "andesite" }.All(id => ItemCatalog.Find(id).BelongsTo(Episode.Hills))); // Shared review kinds keep their old slots
            Assert.IsFalse(new[] { "basalt", "schist", "tuff" }.Any(id => ItemCatalog.Find(id).BelongsTo(Episode.Hills))); // These review kinds stay on Mountains
        } // Ends the check

        [Test] // Checks earlier layouts
        public void EarlierTrailsHaveNoMudOrLogs() // Keeps the new jobs limited to their own data
        { // Opens the check
            var earlier = LevelCatalog.All.Except(All); // Gets the earlier trails
            Assert.IsTrue(earlier.All(l => l.Layout.MudZones.Count == 0 && l.Layout.Logs.Count == 0)); // Earlier layouts keep the default lists
        } // Ends the check
    } // Ends the checks
} // Ends the tests