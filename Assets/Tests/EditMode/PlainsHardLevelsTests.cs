using System; // Gives the ends of a hay run
using System.Linq; // Gives list searches and counts
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class PlainsHardLevelsTests // Checks the seven Plains Hard trails (D-071)
    { // Opens the checks
        static LevelData Hard(int number) => LevelCatalog.Find(Episode.Plains, Difficulty.Hard, number); // Finds one trail
        static LevelData[] All => LevelCatalog.All.Where(l => l.Episode == Episode.Plains && l.Difficulty == Difficulty.Hard).ToArray(); // Finds all seven trails
        static bool Overlaps(float left, float right, float x, float width) => left < x + width && x < right; // Checks whether two spans cross

        [Test] // Checks the full trail list
        public void CatalogHasSevenPlainsHardScenesInOrder() // Keeps the scene names and their order
        { // Opens the check
            CollectionAssert.AreEqual(Enumerable.Range(1, 7), All.Select(l => l.Number)); // Hard has seven levels
            CollectionAssert.AreEqual(Enumerable.Range(1, 7).Select(n => $"PlainsHard{n}"), All.Select(l => l.SceneName)); // The builder has seven scene names
            Assert.IsNull(LevelCatalog.Find(Episode.Plains, Difficulty.Hard, 8)); // Hard ends after its seventh trail
        } // Ends the check

        [TestCase(1, LevelTheme.Prairie, "Great Plains", "Grassland Plains", "lesson_great_plains", 6, 7, 3, 4, 3804f, 84f, "chalk calcite chalk limestone chalk shale calcite limestone sandstone")] // Checks the Great Plains trail
        [TestCase(2, LevelTheme.Savanna, "Serengeti", "Plains of Animals", "lesson_serengeti", 5, 6, 5, 6, 6204f, 84f, "granite tuff granite tuff sandstone granite tuff siltstone shale")] // Checks the Serengeti trail
        [TestCase(3, LevelTheme.Pampas, "Pampas", "Farm and Ranch Plains", "lesson_pampas", 6, 5, 2, 7, 7004f, 84f, "loess mica feldspar loess quartz mica feldspar loess clay")] // Checks the Pampas trail
        [TestCase(4, LevelTheme.FrozenPlain, "West Siberian Plain", "Cold Plains", "lesson_siberia", 5, 6, 5, 6, 6204f, 84f, "peat clay siltstone peat mudstone sandstone peat clay siltstone")] // Checks the West Siberian Plain trail
        [TestCase(5, LevelTheme.LimestonePlain, "Nullarbor Plain", "Limestone Plains", "lesson_nullarbor", 7, 6, 3, 6, 7004f, 96f, "flint limestone calcite flint chalk limestone flint calcite limestone")] // Checks the Nullarbor Plain trail
        [TestCase(6, LevelTheme.GangesPlain, "Indo-Gangetic Plain", "River Flood Plains", "lesson_ganges", 8, 5, 2, 5, 4204f, 84f, "laterite clay siltstone sandstone clay laterite mudstone siltstone laterite")] // Checks the Indo-Gangetic Plain trail
        [TestCase(7, LevelTheme.Steppe, "Mongolian Steppe", "Plains of the World", "lesson_steppe", 7, 3, 1, 6, 6564f, 96f, "granite chalk loess flint tuff mica loess feldspar granite")] // Checks the Mongolian Steppe trail
        public void TrailHasHardSizesAndItsAuthoredPickups(int number, LevelTheme theme, string banner, string title, string picture, int pits, int spikes, int ledges, int secret, float secretX, float secretY, string kinds) // Checks layout and lesson data
        { // Opens the check
            var level = Hard(number); // Reads the trail
            Assert.AreEqual(9600f, level.Layout.Width); // Uses the shared Hard width
            Assert.AreEqual(new Spot(40f, 0f), level.Start); // Starts near the left
            Assert.AreEqual(new Spot(9440f, 0f), level.Goal); // Ends near the right
            CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, level.Checkpoints); // Uses three safe flags
            Assert.AreEqual(240, level.TimeLimitSeconds); // Gives four minutes
            Assert.AreEqual(9, level.Pickups.Count); // Nine pickups on each trail
            CollectionAssert.AreEqual(kinds.Split(' '), level.Pickups.Select(p => p.Kind.Id)); // Uses the nine kinds in trail order
            Assert.IsTrue(level.Pickups.All(p => p.Kind == ItemCatalog.Find(p.Kind.Id))); // Every pickup uses a kind from the catalog
            Assert.AreEqual(new Spot(secretX, secretY), level.Pickups.Single(p => p.Place.Y >= 72f).Place); // Exactly one high pickup
            Assert.AreEqual(new Spot(secretX, secretY), level.Pickups[secret].Place); // Keeps the secret in its trail place
            Assert.IsTrue(level.Layout.Ledges.Any(l => l.Top == secretY && l.X <= secretX && secretX <= l.X + l.Width)); // The secret sits on a ledge
            Assert.AreEqual(pits, level.Hazards.Count(h => h.Type == HazardType.Pit)); // Matches the pit list
            Assert.AreEqual(spikes, level.Hazards.Count(h => h.Type == HazardType.Spikes && h.Width == 32f)); // Matches the spike list
            Assert.AreEqual(ledges, level.Layout.Ledges.Count); // Matches the ledge list
            Assert.AreEqual(theme, level.Layout.Theme); // Each trail has its own theme
            Assert.AreEqual(banner, level.BannerName); // Shows the place at the start
            Assert.AreEqual(title, level.Lesson.Title); // Uses the lesson title
            Assert.AreEqual(picture, level.Lesson.Picture); // Uses the matching lesson picture
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Uses four draft lines
            Assert.IsTrue(level.Lesson.Sentences.All(s => s.Length <= 44)); // The sentences fit the lesson
            Assert.IsTrue(level.HasDraftContent && level.Lesson.IsDraft); // Flags the text for teacher review
            Assert.AreEqual(3, level.Review.Count); // Shows three review lines
        } // Ends the check

        [Test] // Checks the first hay bale and the first herd
        public void FirstHayAndHerdHaveSafePracticeGround() // Leaves room to try each new look
        { // Opens the check
            var hay = Hard(1).Layout.Logs.First(); // Reads the first hay bale
            Assert.AreEqual(1000f, hay.Spawn); // Spawns on the right
            Assert.AreEqual(800f, hay.End); // Ends on the left
            Assert.AreEqual(-1, hay.Direction); // Rolls toward the explorer
            Assert.IsFalse(Hard(1).Hazards.Any(h => Overlaps(680f, 1120f, h.Place.X, h.Width))); // Leaves clear practice ground
            var herd = Hard(2).Layout.Bats.First(); // Reads the first herd
            Assert.AreEqual(new Spot(650f, 6f), herd.Center); // Walks on the practice stretch
            Assert.IsEmpty(Hard(2).Layout.Logs); // Herds are the only new object in the Serengeti
        } // Ends the check

        [Test] // Checks every herd shape
        public void HerdsWalkOnTheGround() // A herd is the bat rule on the ground
        { // Opens the check
            foreach (var number in new[] { 2, 3, 7 }) // The three herd trails
            { // Opens the trail check
                var herds = Hard(number).Layout.Bats; // Reads the herds
                Assert.IsNotEmpty(herds); // Each herd trail has herds
                Assert.IsTrue(herds.All(h => h.Center.Y == 6f && h.RadiusY == 0f && h.RadiusX == 48f)); // Centre y 6, no up-down move, 48 px sideways
            } // Ends the trail check
            Assert.IsTrue(Hard(5).Layout.Bats.All(b => b.Center.Y == 16f && b.RadiusX == 24f && b.RadiusY == 12f)); // Nullarbor keeps flying bats
        } // Ends the check

        [Test] // Checks the hay directions in trail order
        public void HayBalesKeepTheirAuthoredRoutes() // Copies the hay routes from the layout
        { // Opens the check
            CollectionAssert.AreEqual(new[] { -1, -1, -1, 1, -1, 1, -1, 1, -1 }, Hard(1).Layout.Logs.Select(l => l.Direction)); // Great Plains directions
            CollectionAssert.AreEqual(new[] { -1, 1, -1, 1, -1 }, Hard(3).Layout.Logs.Select(l => l.Direction)); // Pampas directions
            CollectionAssert.AreEqual(new[] { -1, 1, -1, 1 }, Hard(7).Layout.Logs.Select(l => l.Direction)); // Steppe directions
            CollectionAssert.AreEqual(new[] { 0f, 1.0f, 0f, 0.5f, 2.0f, 1.0f, 1.5f, 2.5f, 0.5f }, Hard(1).Layout.Logs.Select(l => l.Offset)); // Great Plains timing
            CollectionAssert.AreEqual(new[] { (2000f, 1780f, 0.5f), (4300f, 4520f, 1.0f), (6900f, 6680f, 2.0f), (8950f, 9170f, 1.5f) }, Hard(7).Layout.Logs.Select(l => (l.Spawn, l.End, l.Offset))); // Steppe routes and timing
        } // Ends the check

        [TestCase(1)] // Checks Great Plains
        [TestCase(2)] // Checks Serengeti
        [TestCase(3)] // Checks Pampas
        [TestCase(4)] // Checks West Siberian Plain
        [TestCase(5)] // Checks Nullarbor Plain
        [TestCase(6)] // Checks Indo-Gangetic Plain
        [TestCase(7)] // Checks Mongolian Steppe
        public void HazardsAndZonesLeaveClearFlags(int number) // Checks the authored safety gaps
        { // Opens the check
            var level = Hard(number); // Reads the trail
            var layout = level.Layout; // Reads the object lists
            foreach (var flag in level.Checkpoints) // Checks each flag
            { // Opens the flag check
                float x = flag.X; // Measures from the flag
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(x - 100f, x + 100f, h.Place.X, h.Width))); // Keeps pits and spikes away
                Assert.IsFalse(layout.CrumbleLedges.Any(c => Overlaps(x - 100f, x + 100f, c.X, CrumbleLedge.Width))); // Keeps crumbling ledges away
                Assert.IsFalse(layout.Bats.Any(b => Overlaps(x - 100f, x + 100f, b.Center.X - b.RadiusX - Bat.Width / 2f, b.RadiusX * 2f + Bat.Width))); // Covers each full bat or herd walk
                Assert.IsFalse(layout.IceZones.Any(i => Overlaps(x - 40f, x + 40f, i.X, i.Width))); // Keeps frozen ground away
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(x - 40f, x + 40f, c.X, c.Width))); // Keeps currents away
                Assert.IsFalse(layout.Boulders.Any(b => b.X <= x && x <= b.X + b.Width)); // Keeps the flag off boulders
                Assert.IsFalse(layout.StrawPiles.Any(s => Overlaps(x - 100f, x + 100f, s.X, StrawPile.Width))); // Keeps straw piles away
                Assert.IsFalse(layout.Logs.Any(l => Overlaps(x - 100f, x + 100f, Math.Min(l.Spawn, l.End) - 8f, Math.Abs(l.End - l.Spawn) + 16f))); // Keeps the full hay path away
            } // Ends the flag checks
            foreach (var log in layout.Logs) // Checks the ground along every hay route
            { // Opens the route check
                float left = Math.Min(log.Spawn, log.End) - 8f; // Allows room for the hay picture
                float right = Math.Max(log.Spawn, log.End) + 8f; // Allows room at the other end
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(left, right, h.Place.X, h.Width))); // Hay has clear ground
                Assert.IsFalse(layout.Boulders.Any(b => Overlaps(left, right, b.X, b.Width))); // Hay can pass without crossing a boulder
                Assert.IsFalse(layout.Currents.Any(c => Overlaps(left, right, c.X, c.Width))); // Hay does not roll through water
                Assert.IsFalse(layout.StrawPiles.Any(s => Overlaps(left, right, s.X, StrawPile.Width))); // Hay does not roll through a straw pile
                Assert.IsFalse(level.Pickups.Any(p => p.Place.Y == 0f && p.Place.X >= left && p.Place.X <= right)); // No ground pickup lies in the path
            } // Ends the route checks
            foreach (var ice in layout.IceZones) // Checks each frozen stretch
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(ice.X, ice.X + ice.Width, h.Place.X, h.Width))); // No pit or spikes inside the ice
        } // Ends the check

        [Test] // Checks where each object belongs
        public void EachTrailUsesItsAuthoredObjectsAndAmbience() // Checks the hard mix by trail
        { // Opens the check
            CollectionAssert.AreEqual(new[] { 9, 0, 5, 0, 0, 0, 4 }, All.Select(l => l.Layout.Logs.Count)); // Counts the hay bales
            CollectionAssert.AreEqual(new[] { 3, 0, 0, 0, 0, 0, 0 }, All.Select(l => l.Layout.StrawPiles.Count)); // Counts the straw piles
            CollectionAssert.AreEqual(new[] { 0, 6, 3, 0, 6, 0, 2 }, All.Select(l => l.Layout.Bats.Count)); // Counts the herds and bats
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 7, 0, 0, 4 }, All.Select(l => l.Layout.IceZones.Count)); // Counts the frozen stretches
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 2, 0, 12, 0 }, All.Select(l => l.Layout.Floats.Count)); // Counts the floats
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 11, 0, 11 }, All.Select(l => l.Layout.CrumbleLedges.Count)); // Counts the crumble ledges
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 0, 6, 2 }, All.Select(l => l.Layout.Currents.Count)); // Counts the currents
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 2, 0, 8, 0 }, All.Select(l => l.Hazards.Count(h => h.IsWater))); // Counts the water pits
            Assert.IsTrue(All.All(l => l.Layout.MudZones.Count == 0 && l.Layout.WindZones.Count == 0 && l.Layout.MovingPlatforms.Count == 0 && l.Layout.SandZones.Count == 0)); // No other object types
            CollectionAssert.AreEqual(new[] { "amb_dry_hard", "amb_dry_hard", "amb_dry_hard", "amb_cold_plain", "amb_dry_hard", "amb_dry_hard", "amb_dry_hard" }, All.Select(LandformAudio.AmbienceFor)); // Only the West Siberian Plain uses its own loop
        } // Ends the check

        [Test] // Checks the water pits and their floats
        public void WaterPitsAndFloatsMatchTheLayout() // H4 has two bog ponds; every H6 pit is water
        { // Opens the check
            var siberia = Hard(4); // Reads the cold trail
            CollectionAssert.AreEqual(new[] { (3000f, 104f), (6500f, 104f) }, siberia.Hazards.Where(h => h.IsWater).Select(h => (h.Place.X, h.Width))); // The two bog ponds
            CollectionAssert.AreEqual(new[] { (3028f, 3000f), (6528f, 6500f) }, siberia.Layout.Floats.Select(f => (f.X, f.WaterX))); // One float over each pond
            Assert.AreEqual("amb_cold_plain", siberia.Layout.AmbienceOverride); // The cold wind loop
            Assert.AreEqual("make_plains_hard_audio.py", LandformAudio.AmbienceScriptFor(siberia)); // A missing loop names its script
            var ganges = Hard(6); // Reads the flood trail
            Assert.IsTrue(ganges.Hazards.Where(h => h.Type == HazardType.Pit).All(h => h.IsWater)); // Every pit is a flood pond
            foreach (var raft in ganges.Layout.Floats) // Checks each float
                Assert.IsTrue(ganges.Hazards.Any(h => h.IsWater && h.Place.X == raft.WaterX && raft.X >= h.Place.X && raft.X + SinkingFloat.Width <= h.Place.X + h.Width)); // Sits inside its pond
            Assert.IsTrue(All.Where(l => l.Number != 4).All(l => l.Layout.AmbienceOverride == null)); // The others use the table loop
        } // Ends the check

        [Test] // Checks the Plains Hard route
        public void PlainsNormal5OpensPlainsHard1() // The Hard card is no longer Coming soon
        { // Opens the check
            var p = ClearedBeforePlainsHard(); // Finishes Plains Easy and Normal
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Plains, Difficulty.Hard)); // Opens the Hard card
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plains, Difficulty.Hard)); // Tapping Hard opens its trail
            var next = p.NextPlayableAfter(Episode.Plains, Difficulty.Normal, 5); // Reads the Result Screen route
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Shows Next
            Assert.AreEqual((Episode.Plains, Difficulty.Hard, 1), (next.Episode, next.Difficulty, next.Level)); // Points at Plains Hard 1
        } // Ends the check

        [Test] // Checks the end of Plains
        public void PlainsHard7HidesNextUntilPlainsIsCompleteThenOpensPlateaus() // Uses the generic episode flow
        { // Opens the check
            var p = ClearedBeforePlainsHard(); // Finishes Plains Easy and Normal
            p.RecordClear(Episode.Plains, Difficulty.Hard, 7, 1); // Clears only the last Hard trail
            Assert.AreEqual(NextStep.None, p.NextPlayableAfter(Episode.Plains, Difficulty.Hard, 7).Step); // No level follows Hard 7
            Assert.IsFalse(p.IsEpisodeUnlocked(Episode.Plateaus)); // Plateaus waits for all fifteen
            for (int n = 1; n <= 6; n++) p.RecordClear(Episode.Plains, Difficulty.Hard, n, 1); // Clears the other Hard trails
            Assert.IsTrue(p.IsEpisodeComplete(Episode.Plains)); // All fifteen Plains trails count
            Assert.IsTrue(p.IsEpisodeUnlocked(Episode.Plateaus)); // The map can open Plateaus
            Assert.IsTrue(p.ShouldAnimateUnlock(Episode.Plateaus, false)); // The first visit shows the unlock
            var next = p.NextPlayableAfter(Episode.Plains, Difficulty.Hard, 7); // Reads the Result Screen route
            Assert.AreEqual(NextStep.NextEpisode, next.Step); // Result opens the Landform Description Screen
            Assert.AreEqual(Episode.Plateaus, next.Episode); // Points to the next landform
            Assert.IsTrue(LevelCatalog.HasLevels(Episode.Plateaus, Difficulty.Easy)); // Plateaus Easy has its trails now (D-072)
        } // Ends the check

        [Test] // Checks the four Plains Hard badges
        public void PlainsHardBadgesFollowTheirRules() // Prairie Ranger, World Plains, Steady Plains and Plain Master
        { // Opens the check
            var p = ClearedBeforePlainsHard(); // Finishes Plains Easy and Normal
            Assert.IsFalse(Earned("prairie_ranger", p)); // No Hard clear yet
            p.RecordClear(Episode.Plains, Difficulty.Hard, 1, 1); // Clears Great Plains
            Assert.IsTrue(Earned("prairie_ranger", p)); // Any Hard clear is enough
            p.RecordClear(Episode.Plains, Difficulty.Hard, 5, 3); // Clears Nullarbor Plain
            p.RecordClear(Episode.Plains, Difficulty.Hard, 6, 3); // Clears Indo-Gangetic Plain
            Assert.IsFalse(Earned("world_plains", p)); // Hard 7 is still missing
            p.RecordClear(Episode.Plains, Difficulty.Hard, 7, 3); // Clears Mongolian Steppe
            Assert.IsTrue(Earned("world_plains", p)); // Hard 5 to 7 are cleared
            Assert.IsFalse(Earned("plain_master", p)); // Hard 2 to 4 are still missing
            for (int n = 2; n <= 4; n++) p.RecordClear(Episode.Plains, Difficulty.Hard, n, 3); // Clears the rest with three stars
            Assert.IsTrue(Earned("plain_master", p)); // All fifteen Plains trails are cleared
            Assert.IsFalse(Earned("steady_plains", p)); // Great Plains has only one star
            p.RecordClear(Episode.Plains, Difficulty.Hard, 1, 3); // Replays Great Plains for three stars
            Assert.IsTrue(Earned("steady_plains", p)); // Three stars on all seven
        } // Ends the check

        [Test] // Checks that older trails keep their data
        public void EarlierTrailsKeepTheirCountsAndObjects() // Plains Hard adds trails without changing the old ones
        { // Opens the check
            var earlier = LevelCatalog.All.Except(All).Where(l => l.Episode != Episode.Plateaus).ToArray(); // Gets the older trails; Plateaus came after Plains Hard (D-072)
            Assert.AreEqual(38, earlier.Length); // Mountains 15, Hills 15, Plains Easy 3 and Plains Normal 5
            Assert.IsTrue(earlier.All(l => l.Layout.Theme < LevelTheme.Prairie)); // No old trail uses a Plains Hard theme
            Assert.IsTrue(earlier.SelectMany(l => l.Layout.Bats).All(b => b.RadiusY > 0f)); // Old bats keep flying loops
            Assert.IsFalse(earlier.SelectMany(l => l.Pickups).Any(p => new[] { "chalk", "loess", "flint" }.Contains(p.Kind.Id))); // The new kinds only lie on Plains Hard
            Assert.AreEqual(0, earlier.Count(l => l.Episode == Episode.Plains && l.Difficulty == Difficulty.Hard)); // No older Plains Hard trail
        } // Ends the check

        static bool Earned(string id, PlayerProgress p) => AchievementCatalog.Find(id).IsEarned(p); // Short way to ask about one badge

        static PlayerProgress ClearedBeforePlainsHard() // Builds a save ready for Plains Hard
        { // Starts the save
            var progress = new PlayerProgress(); // Starts with no clears
            foreach (var trail in LevelCatalog.All) // Reads the trails with data
                if (trail.Episode != Episode.Plains || trail.Difficulty != Difficulty.Hard) progress.RecordClear(trail.Episode, trail.Difficulty, trail.Number, 1); // Clears every trail before Plains Hard
            return progress; // Opens Plains Hard
        } // Ends the save setup
    } // Ends the checks
} // Ends the tests
