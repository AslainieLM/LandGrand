using System.Linq; // Gives list helpers such as Any and Select
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Tests for the plain rules
{ // Opens the namespace
    public class PlateausNormalLevelsTests // Checks the five Plateaus Normal trails (D-073)
    { // Opens the tests
        static LevelData Normal(int number) => LevelCatalog.Find(Episode.Plateaus, Difficulty.Normal, number); // Finds one Plateaus Normal trail

        [TestCase(1, LevelTheme.TableTop, "Table Mountain", "Table Tops", "lesson_table_mountain", 4112f, 84f)] // Secret above the stepped ledges
        [TestCase(2, LevelTheme.MesaValley, "Monument Valley", "Mesas and Buttes", "lesson_monument_valley", 4012f, 84f)] // The higher hematite is a review pickup
        [TestCase(3, LevelTheme.CanyonRim, "Grand Canyon", "Canyons", "lesson_grand_canyon", 3712f, 84f)] // Secret above the canyon floor
        [TestCase(4, LevelTheme.TepuiTop, "Mount Roraima", "Table-top Mountains", "lesson_roraima", 5068f, 144f)] // Secret on the second updraft ledge
        [TestCase(5, LevelTheme.SaltFlat, "Salar de Uyuni", "High Plateaus", "lesson_uyuni", 3712f, 84f)] // Secret above the salt
        public void TrailHasItsNormalSizeAndDraftContent(int number, LevelTheme theme, string banner, string title, string picture, float secretX, float secretY) // Checks the authored trail
        { // Opens the trail check
            var level = Normal(number); // Loads the trail by its episode and number
            Assert.IsNotNull(level); // Each of the five trails has data
            Assert.AreEqual("PlateausNormal" + number, level.SceneName); // Uses the scene name the selection screen expects
            Assert.AreEqual(7200f, level.Layout.Width); // Keeps the Normal width
            Assert.AreEqual(new Spot(40f, 0f), level.Start); // Starts near the left edge
            Assert.AreEqual(new Spot(7040f, 0f), level.Goal); // Ends at the shared Normal goal
            CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, level.Checkpoints); // Keeps both flag positions
            Assert.AreEqual(180, level.TimeLimitSeconds); // Gives three minutes
            Assert.AreEqual(theme, level.Layout.Theme); // Selects the authored terrain and goal art
            Assert.AreEqual(banner, level.BannerName); // Uses the place name on the banner
            Assert.AreEqual(title, level.Lesson.Title); // Uses the authored lesson title
            Assert.AreEqual(picture, level.Lesson.Picture); // Loads the picture for this lesson
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Shows four lesson lines
            Assert.IsTrue(level.Lesson.Sentences.All(line => line.Length <= 44)); // Fits the lines on screen
            Assert.AreEqual(3, level.Review.Count); // Shows three result review lines
            Assert.IsTrue(level.Lesson.IsDraft && level.HasDraftContent); // Shows the teacher review note
            Assert.AreEqual(7, level.Pickups.Count); // Gives seven finds
            Assert.IsTrue(level.Pickups.All(p => p.Kind.IsDraft && p.Kind.BelongsTo(Episode.Plateaus))); // Every find has a draft card on the shelf
            Assert.That(secretY, Is.GreaterThanOrEqualTo(72f)); // The authored secret is on a high ledge
            Assert.AreEqual(1, level.Pickups.Count(p => p.Place.Equals(new Spot(secretX, secretY)))); // Places one find at the marked secret spot
            Assert.IsTrue(level.Layout.Ledges.Any(l => l.Top == secretY && l.X <= secretX && secretX <= l.X + l.Width)); // Gives the secret a landing ledge
        } // Ends the trail check

        [Test] // Checks the water that cannot return the explorer
        public void TableMountainHasFourCableCarsAndOneSafePool() // Reuses horizontal platforms and the shallow pool
        { // Opens the cable car check
            var level = Normal(1); // Reads Table Mountain
            var pool = level.Layout.Pools.Single(); // Reads its one shallow pool
            Assert.AreEqual((800f, 160f), (pool.X, pool.Width)); // Fits the full first crossing
            CollectionAssert.AreEqual(new[] { (800f, 912f), (2000f, 2152f), (3600f, 3752f), (5900f, 6052f) }, level.Layout.MovingPlatforms.Select(p => (p.X0, p.X1))); // Checks all cable routes
            Assert.IsTrue(level.Layout.MovingPlatforms.All(p => !p.IsVertical && p.Top == 0f)); // Keeps the solid at the ground line
            Assert.AreEqual(pool.X + pool.Width, level.Layout.MovingPlatforms[0].X1 + MovingPlatform.Width); // The first car spans the safe water
            Assert.AreEqual(48f, MovingPlatform.Width); // Uses the shared deck width
            Assert.AreEqual(40f, MovingPlatform.Speed); // Uses the shared travel speed
        } // Ends the cable car check

        [TestCase(1, false)] // Table Mountain's other crossings are dry pits
        [TestCase(2, false)] // Monument Valley has dry pits
        [TestCase(3, true)] // Grand Canyon's river fills its pits
        [TestCase(4, true)] // Roraima's summit pools fill its pits
        [TestCase(5, false)] // Salar de Uyuni has dry pits
        public void PitsUseTheAuthoredWaterSetting(int number, bool water) // Checks the return hazards
        { // Opens the pit check
            var level = Normal(number); // Reads this trail
            var pits = level.Hazards.Where(h => h.Type == HazardType.Pit).ToArray(); // Separates pits from spike traps
            Assert.AreEqual(3, pits.Length); // Gives each trail three return pits
            Assert.IsTrue(pits.All(p => p.IsWater == water)); // Uses water only for the river and summit pools
            Assert.AreEqual(3, level.Hazards.Count(h => h.Type == HazardType.Spikes && h.Width == 32f)); // Keeps three spike strips
        } // Ends the pit check

        [TestCase(2, 2450f, 116f, 4700f, 100f)] // Monument Valley has two updrafts
        [TestCase(4, 3000f, 116f, 5000f, 148f)] // Roraima has two updrafts
        public void UpdraftsLeadToTheirLandingLedges(int number, float x1, float top1, float x2, float top2) // Checks the updraft routes
        { // Opens the updraft check
            var layout = Normal(number).Layout; // Reads the trail's layout
            CollectionAssert.AreEqual(new[] { (x1, top1), (x2, top2) }, layout.Updrafts.Select(u => (u.X, u.Top))); // Keeps both columns in place
            foreach (var updraft in layout.Updrafts) // Checks each column
                Assert.IsTrue(layout.Ledges.Any(l => l.X == updraft.X + 40f && l.Top == updraft.Top - 4f)); // Checks the landing ledge beside the column
        } // Ends the updraft check

        [Test] // Checks objects in the middle three trails
        public void SandBridgesCurrentsAndFloatsUseTheirCheckedPlacements() // Reads the existing object types
        { // Opens the placement check
            var mesa = Normal(2).Layout; // Reads Monument Valley
            CollectionAssert.AreEqual(new[] { (500f, 200f), (2000f, 300f), (4100f, 350f), (6150f, 300f) }, mesa.SandZones.Select(s => (s.X, s.Width))); // Keeps all four sand strips
            CollectionAssert.AreEqual(new[] { 1300f, 1332f, 1364f, 5700f, 5732f, 5764f }, mesa.CrumbleLedges.Select(l => l.X)); // Forms the two bridges
            Assert.IsTrue(mesa.CrumbleLedges.All(l => l.Top == 0f)); // Bridges stand at ground height
            var canyon = Normal(3).Layout; // Reads Grand Canyon
            CollectionAssert.AreEqual(new[] { (600f, 200f, 1), (1900f, 300f, -1), (3900f, 400f, 1), (5800f, 300f, -1) }, canyon.Currents.Select(c => (c.X, c.Width, c.Direction))); // Preserves both directions
            var tepui = Normal(4).Layout; // Reads Roraima
            CollectionAssert.AreEqual(new[] { (1828f, 0f, 1800f), (6208f, 0f, 6200f) }, tepui.Floats.Select(f => (f.X, f.Top, f.WaterX))); // Attaches both floats to their water pits
        } // Ends the placement check

        [Test] // Checks salt crust and herds
        public void SalarDeUyuniUsesThreeIceStripsAndThreeGroundHerds() // Keeps the ice and bat rules
        { // Opens the salt flat check
            var layout = Normal(5).Layout; // Reads Salar de Uyuni
            CollectionAssert.AreEqual(new[] { (600f, 300f), (3900f, 400f), (5900f, 400f) }, layout.IceZones.Select(i => (i.X, i.Width))); // Keeps the salt strips in place
            CollectionAssert.AreEqual(new[] { (1900f, 4.6f, 0f), (3000f, 4.8f, 1f), (5000f, 4.4f, 2f) }, layout.Bats.Select(b => (b.Center.X, b.Period, b.Phase))); // Keeps each herd's route and timing
            Assert.IsTrue(layout.Bats.All(b => b.Center.Y == 6f && b.RadiusX == 48f && b.RadiusY == 0f)); // Keeps all three herds on the ground
        } // Ends the salt flat check

        [Test] // Checks which trails use the new art roles
        public void CableCarsSaltCrustAndHerdsStayOnTheirOwnTrails() // Checks all five layouts
        { // Opens the trail check
            var layouts = Enumerable.Range(1, 5).Select(n => Normal(n).Layout).ToArray(); // Reads the layouts in trail order
            CollectionAssert.AreEqual(new[] { 4, 0, 0, 0, 0 }, layouts.Select(l => l.MovingPlatforms.Count)); // Only Table Mountain has cars
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 3 }, layouts.Select(l => l.IceZones.Count)); // Only Uyuni has salt crust
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 3 }, layouts.Select(l => l.Bats.Count)); // Only Uyuni has herds
        } // Ends the trail check

        [Test] // Checks the route from Easy
        public void ClearingPlateausEasyOpensTableMountain() // Follows the route used by the Result Screen
        { // Opens the route check
            var progress = new PlayerProgress(); // Starts a save for the test
            foreach (var level in LevelCatalog.All.Where(l => l.Episode < Episode.Plateaus)) // Reads every earlier trail
                progress.RecordClear(level.Episode, level.Difficulty, level.Number, 1); // Opens Plateaus Easy
            progress.RecordClear(Episode.Plateaus, Difficulty.Easy, 1, 1); // Clears Bukidnon
            progress.RecordClear(Episode.Plateaus, Difficulty.Easy, 2, 1); // Clears Lake Lanao
            Assert.IsFalse(progress.IsDifficultyUnlocked(Episode.Plateaus, Difficulty.Normal)); // Still needs Maria Cristina Falls
            progress.RecordClear(Episode.Plateaus, Difficulty.Easy, 3, 1); // Finishes Plateaus Easy
            Assert.IsTrue(progress.IsDifficultyUnlocked(Episode.Plateaus, Difficulty.Normal)); // Opens the Normal card
            var next = progress.NextPlayableAfter(Episode.Plateaus, Difficulty.Easy, 3); // Reads the Next button's destination
            Assert.AreEqual((NextStep.NextLevel, Episode.Plateaus, Difficulty.Normal, 1), (next.Step, next.Episode, next.Difficulty, next.Level)); // Leads to Table Mountain
        } // Ends the route check

        [TestCase(1, "table_top")] // The Table Mountain clear earns Table Top
        [TestCase(3, "canyon_explorer")] // The Grand Canyon clear earns Canyon Explorer
        public void ClearingTheAuthoredTrailEarnsItsExistingBadge(int number, string badgeId) // Checks the badge for the real trail data
        { // Opens the badge check
            var level = Normal(number); // Reads the level used by play
            var progress = new PlayerProgress(); // Starts without this clear
            var badge = AchievementCatalog.ForLandform(level.Episode).Single(entry => entry.Id == badgeId); // Finds the existing badge
            Assert.IsFalse(badge.IsEarned(progress)); // Keeps the badge locked before the clear
            progress.RecordClear(level.Episode, level.Difficulty, level.Number, 1); // Records this trail's clear
            Assert.IsTrue(badge.IsEarned(progress)); // Earns the matching badge
        } // Ends the badge check
    } // Ends the tests
} // Ends the namespace
