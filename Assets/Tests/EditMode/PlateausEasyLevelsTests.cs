using System.Linq; // Gives list checks for the authored layouts
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    /// <summary>Checks the three Plateaus Easy trails against layouts.md and spec sections 5 to 7 (D-072).</summary>
    public class PlateausEasyLevelsTests // Checks the three Plateaus trails
    { // Opens the Plateaus checks
        const float FirstUpdraftSafeDistance = 80f; // Space around the first updraft in spec section 5
        const float UpdraftHazardDistance = 60f; // Space between an updraft and a pit or spikes
        const float UpdraftCheckpointDistance = 40f; // Space between an updraft and the checkpoint flag
        const float HeadAboveFeet = 26f; // The explorer's head sits this far above its feet
        const float SafeTop = 196f; // The highest head height below the HUD

        static LevelData Easy(int number) => LevelCatalog.Find(Episode.Plateaus, Difficulty.Easy, number); // Finds one Plateaus Easy trail

        [TestCase(1, 4800f, 4740f, 2400f, LevelTheme.PineapplePlateau, "Bukidnon Plateau", "Plateaus", "lesson_bukidnon", "basalt,andesite,hematite,halloysite,basalt", 4072f, 84f)] // Checks the pineapple plateau and its secret
        [TestCase(2, 5400f, 5240f, 2700f, LevelTheme.LakePlateau, "Lake Lanao", "Plateau Lakes", "lesson_lanao", "tuff,volcanic_breccia,goethite,halloysite,tuff", 3162f, 84f)] // Checks the lake plateau and its secret
        [TestCase(3, 5400f, 5240f, 2500f, LevelTheme.FallsEdge, "Maria Cristina Falls", "Plateau Rivers", "lesson_maria_cristina", "laterite,gibbsite,goethite,cristobalite,laterite", 3368f, 160f)] // Checks the falls edge and its secret
        public void EasyTrailHasTheAuthoredShapeAndContent(int number, float width, float goal, float checkpoint, LevelTheme theme, string banner, string title, string picture, string kinds, float secretX, float secretY) // Checks the authored trail data
        { // Starts the trail check
            var level = Easy(number); // Finds this Plateaus trail
            Assert.AreEqual("PlateausEasy" + number, level.SceneName); // Uses the common scene name
            Assert.AreEqual((40f, 0f), (level.Start.X, level.Start.Y)); // Starts on the ground
            Assert.AreEqual((goal, 0f), (level.Goal.X, level.Goal.Y)); // The goal sits on the ground
            Assert.AreEqual(width, level.Layout.Width); // Keeps the authored width
            Assert.AreEqual(theme, level.Layout.Theme); // Uses the right ground, backdrop and goal picture
            Assert.AreEqual(120, level.TimeLimitSeconds); // The standard Easy timer
            var flag = level.Checkpoints.Single(); // Each Easy trail has exactly one flag
            Assert.AreEqual((checkpoint, 0f), (flag.X, flag.Y)); // Keeps the flag on the ground
            Assert.AreEqual(banner, level.BannerName); // Names the place at the start
            Assert.AreEqual(title, level.Lesson.Title); // Shows the lesson topic
            Assert.AreEqual(picture, level.Lesson.Picture); // Shows the matching lesson picture
            Assert.IsTrue(level.Lesson.IsDraft); // Shows the teacher review note
            Assert.AreEqual(4, level.Lesson.Sentences.Count); // Four lesson lines
            Assert.IsTrue(level.Lesson.Sentences.All(line => line.Length <= 44)); // Keeps each line short
            Assert.AreEqual(3, level.Review.Count); // Three Result Screen lines
            Assert.IsTrue(level.HasDraftContent); // All text is a draft (Q-04)
            Assert.AreEqual(5, level.Pickups.Count); // Five pickups in Easy
            CollectionAssert.AreEqual(kinds.Split(','), level.Pickups.Select(p => p.Kind.Id)); // Keeps the pickup kind order
            Assert.IsTrue(level.Pickups.All(p => p.Kind == ItemCatalog.Find(p.Kind.Id))); // Uses the shared kind objects
            Assert.IsTrue(level.Pickups.All(p => p.Kind.Landforms.Contains(Episode.Plateaus))); // Every find is on the Plateaus shelf
            var highest = level.Pickups.Where(p => p.Place.Y == level.Pickups.Max(other => other.Place.Y)).Single(); // Exactly one secret, on the highest spot
            Assert.AreEqual((secretX, secretY), (highest.Place.X, highest.Place.Y)); // Keeps the secret in the authored spot
        } // Ends the trail check

        [Test] // Checks the hazards, boulders, ledges and pickup spots of the first trail
        public void BukidnonPlateauKeepsTheLayoutNumbers() // Copies layouts.md exactly
        { // Starts the layout check
            var level = Easy(1); // Bukidnon Plateau
            CollectionAssert.AreEqual(new[] { (HazardType.Pit, 1300f, 48f), (HazardType.Spikes, 2950f, 32f), (HazardType.Pit, 3400f, 48f) }, level.Hazards.Select(h => (h.Type, h.Place.X, h.Width))); // Two pits and one spike trap
            CollectionAssert.AreEqual(new[] { (450f, BoulderSize.Small), (1050f, BoulderSize.Big), (1800f, BoulderSize.Small), (2800f, BoulderSize.Big), (3700f, BoulderSize.Small), (4250f, BoulderSize.Big) }, level.Layout.Boulders.Select(b => (b.X, b.Size))); // Six boulders
            CollectionAssert.AreEqual(new[] { (650f, 32f, 80f), (1500f, 28f, 64f), (2050f, 40f, 64f), (3100f, 28f, 80f), (3900f, 28f, 48f), (3960f, 56f, 64f), (4040f, 84f, 64f) }, level.Layout.Ledges.Select(l => (l.X, l.Top, l.Width))); // Seven ledges with the three-step climb
            CollectionAssert.AreEqual(new[] { (466f, 24f), (690f, 32f), (2082f, 40f), (4072f, 84f), (4600f, 16f) }, level.Pickups.Select(p => (p.Place.X, p.Place.Y))); // Five pickup spots
        } // Ends the layout check

        [Test] // Checks the hazards, boulders, ledges and pickup spots of the second trail
        public void LakeLanaoKeepsTheLayoutNumbers() // Copies layouts.md exactly
        { // Starts the layout check
            var level = Easy(2); // Lake Lanao
            CollectionAssert.AreEqual(new[] { (HazardType.Pit, 900f, 48f), (HazardType.Spikes, 1800f, 32f), (HazardType.Pit, 3600f, 56f), (HazardType.Spikes, 4100f, 32f) }, level.Hazards.Select(h => (h.Type, h.Place.X, h.Width))); // Two pits and two spike traps
            CollectionAssert.AreEqual(new[] { (450f, BoulderSize.Small), (1500f, BoulderSize.Big), (2250f, BoulderSize.Small), (4450f, BoulderSize.Small), (4850f, BoulderSize.Big) }, level.Layout.Boulders.Select(b => (b.X, b.Size))); // Five boulders
            CollectionAssert.AreEqual(new[] { (1150f, 28f, 80f), (1950f, 40f, 64f), (3050f, 40f, 64f), (3130f, 84f, 72f), (3300f, 28f, 64f), (4650f, 28f, 64f) }, level.Layout.Ledges.Select(l => (l.X, l.Top, l.Width))); // Six ledges
            CollectionAssert.AreEqual(new[] { (466f, 24f), (1182f, 28f), (1982f, 40f), (3162f, 84f), (4466f, 24f) }, level.Pickups.Select(p => (p.Place.X, p.Place.Y))); // Five pickup spots
        } // Ends the layout check

        [Test] // Checks the hazards, boulders, ledges, updrafts and pickup spots of the third trail
        public void MariaCristinaFallsKeepsTheLayoutNumbers() // Copies layouts.md exactly
        { // Starts the layout check
            var level = Easy(3); // Maria Cristina Falls
            CollectionAssert.AreEqual(new[] { (HazardType.Pit, 900f, 48f), (HazardType.Spikes, 1500f, 32f), (HazardType.Pit, 2100f, 56f), (HazardType.Spikes, 3000f, 32f), (HazardType.Spikes, 3100f, 32f), (HazardType.Pit, 3800f, 56f), (HazardType.Spikes, 4300f, 32f) }, level.Hazards.Select(h => (h.Type, h.Place.X, h.Width))); // Three pits and four spike traps
            CollectionAssert.AreEqual(new[] { (350f, BoulderSize.Small), (1700f, BoulderSize.Big), (2800f, BoulderSize.Small), (4050f, BoulderSize.Big), (4700f, BoulderSize.Small) }, level.Layout.Boulders.Select(b => (b.X, b.Size))); // Five boulders
            CollectionAssert.AreEqual(new[] { (640f, 64f, 64f), (1240f, 112f, 64f), (1900f, 40f, 64f), (2300f, 28f, 80f), (3340f, 160f, 56f), (4540f, 80f, 64f) }, level.Layout.Ledges.Select(l => (l.X, l.Top, l.Width))); // Six ledges, four of them for the updrafts
            CollectionAssert.AreEqual(new[] { (600f, 68f), (1200f, 116f), (3300f, 164f), (4500f, 84f) }, level.Layout.Updrafts.Select(u => (u.X, u.Top))); // Four updrafts
            CollectionAssert.AreEqual(new[] { (366f, 24f), (1272f, 112f), (3066f, 0f), (3368f, 160f), (4716f, 24f) }, level.Pickups.Select(p => (p.Place.X, p.Place.Y))); // Five pickup spots
        } // Ends the layout check

        [Test] // Checks which kinds each trail teaches and reviews
        public void TrailsTeachAndReviewTheirKinds() // E1 teaches four kinds; E2 and E3 bring one back
        { // Starts the kind check
            CollectionAssert.AreEquivalent(new[] { "basalt", "andesite", "hematite", "halloysite" }, Easy(1).Kinds.Select(k => k.Id)); // E1 teaches four kinds
            CollectionAssert.Contains(Easy(2).Kinds.Select(k => k.Id).ToArray(), "halloysite"); // E2 reviews halloysite from E1
            CollectionAssert.Contains(Easy(2).Kinds.Select(k => k.Id).ToArray(), "goethite"); // Goethite is first taught in E2
            CollectionAssert.Contains(Easy(3).Kinds.Select(k => k.Id).ToArray(), "goethite"); // E3 reviews goethite from E2
        } // Ends the kind check

        [Test] // Checks the fairness of the four updrafts
        public void FallsEdgeUpdraftsAreFair() // Spec section 5 placement rules
        { // Starts the updraft check
            var level = Easy(3); // Maria Cristina Falls
            var updrafts = level.Layout.Updrafts; // The four columns
            Assert.AreEqual(4, updrafts.Count); // Four columns
            float firstX = updrafts.Min(u => u.X); // The leftmost updraft is the practice one
            foreach (var u in updrafts) // Checks each column
            { // Starts one column
                float right = u.X + Updraft.Width; // Right edge of the column
                float hazardGap = u.X == firstX ? FirstUpdraftSafeDistance : UpdraftHazardDistance; // The first column has extra room
                Assert.IsTrue(level.Hazards.All(h => right + hazardGap <= h.Place.X || h.Place.X + h.Width + hazardGap <= u.X), "updraft at " + u.X); // Keeps away from pits and spikes
                Assert.IsTrue(level.Checkpoints.All(c => right + UpdraftCheckpointDistance <= c.X || c.X + UpdraftCheckpointDistance <= u.X), "updraft at " + u.X); // Keeps the flag clear
                Assert.IsTrue(level.Layout.Ledges.Any(l => l.X == u.X + 40f && l.Top == u.Top - 4f), "updraft at " + u.X); // Has its landing ledge 8 px right and 4 px down
                Assert.That(u.Top + HeadAboveFeet, Is.LessThanOrEqualTo(SafeTop), "updraft at " + u.X); // The hovering head stays below the HUD
            } // Ends one column
        } // Ends the updraft check

        [Test] // Checks that only one trail has updrafts
        public void OnlyFallsEdgeHasUpdraftsAmongTheEarlierTrails() // Checks trails shipped before Plateaus Normal
        { // Starts the catalog check
            var withUpdrafts = LevelCatalog.All.Where(l => (l.Episode < Episode.Plateaus || l.Difficulty == Difficulty.Easy) && l.Layout.Updrafts.Count > 0).Select(l => l.SceneName); // Checks earlier landforms and the Plateaus Easy trails
            CollectionAssert.AreEqual(new[] { "PlateausEasy3" }, withUpdrafts); // Only Maria Cristina Falls
        } // Ends the catalog check
    } // Ends the Plateaus checks
} // Closes the tests
