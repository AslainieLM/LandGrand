using System.Linq; // Gives list checks for the authored layouts
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class PlainsEasyLevelsTests // Checks the three Plains trails
    { // Opens the Plains checks
        const float PracticePileSafeDistance = 80f; // Space around the first pile in spec section 5
        const float PileSafeDistance = 60f; // Space around the other piles in spec section 5

        [TestCase(1, 4800f, 4740f, 2400f, LevelTheme.RicePlain, "Central Luzon Plain", "Plains", "lesson_central_luzon", "clay,sandstone,shale,siltstone,clay", 3972f, 84f)] // Checks the rice plain and its secret
        [TestCase(2, 5400f, 5240f, 2700f, LevelTheme.SaltBeds, "Pangasinan Salt Beds", "Coastal Plains", "lesson_pangasinan", "halite,gypsum,quartz,siltstone,halite", 3112f, 84f)] // Checks the salt beds and their secret
        [TestCase(3, 5400f, 5240f, 2500f, LevelTheme.RiverPlain, "Isabela River Plain", "River Plains", "lesson_isabela", "conglomerate,mudstone,halite,limestone,conglomerate", 3388f, 120f)] // Checks the river plain and its secret
        public void EasyTrailHasTheAuthoredShapeAndContent(int number, float width, float goal, float checkpoint, LevelTheme theme, string banner, string title, string picture, string kinds, float secretX, float secretY) // Checks the authored trail data
        { // Starts the trail check
            var level = LevelCatalog.Find(Episode.Plains, Difficulty.Easy, number); // Finds this Plains trail
            Assert.AreEqual("PlainsEasy" + number, level.SceneName); // Uses the common scene name
            Assert.AreEqual((40f, 0f), (level.Start.X, level.Start.Y)); // Starts on the ground
            Assert.AreEqual((goal, 0f), (level.Goal.X, level.Goal.Y)); // The goal sits on the ground
            Assert.AreEqual(width, level.Layout.Width); // Keeps the authored width
            Assert.AreEqual(theme, level.Layout.Theme); // Uses the right ground and backdrop
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
            Assert.AreEqual(5, level.Pickups.Count); // Five pickups in Easy
            CollectionAssert.AreEqual(kinds.Split(','), level.Pickups.Select(p => p.Kind.Id)); // Keeps the pickup kind order
            Assert.IsTrue(level.Pickups.All(p => p.Kind == ItemCatalog.Find(p.Kind.Id))); // Uses the shared kind objects
            var highest = level.Pickups.Where(p => p.Place.Y == level.Pickups.Max(other => other.Place.Y)).Single(); // Only the secret is on the highest ledge
            Assert.AreEqual((secretX, secretY), (highest.Place.X, highest.Place.Y)); // Keeps the secret in the authored spot
        } // Ends the trail check

        [Test] // Runs the pile placement check
        public void OnlyRiverPlainHasTheFourSafePileSpots() // Verifies clearance on all four piles
        { // Starts the placement check
            var trail = LevelCatalog.All.Single(level => level.Layout.StrawPiles.Count > 0); // Only one trail uses piles
            Assert.AreEqual("PlainsEasy3", trail.SceneName); // Mountains and Hills have no piles
            CollectionAssert.AreEqual(new[] { 600f, 1200f, 3300f, 4500f }, trail.Layout.StrawPiles.Select(p => p.X)); // Keeps all four starts
            float practicePileX = trail.Layout.StrawPiles.Min(pile => pile.X); // The leftmost pile is the practice pile
            foreach (var pile in trail.Layout.StrawPiles) // Checks each pile on the river trail
            { // Starts a clearance check
                float right = pile.X + StrawPile.Width; // Right edge of the pile
                float hazardGap = pile.X == practicePileX ? PracticePileSafeDistance : PileSafeDistance; // The practice pile has extra room
                Assert.IsTrue(trail.Hazards.All(h => right + hazardGap <= h.Place.X || h.Place.X + h.Width + hazardGap <= pile.X)); // Keeps away from pits and spikes
                Assert.IsTrue(trail.Layout.Boulders.All(b => right + 40f <= b.X || b.X + b.Width + 40f <= pile.X)); // Keeps away from boulders
                Assert.IsTrue(trail.Checkpoints.All(c => right + 40f <= c.X || c.X + 40f <= pile.X)); // Keeps the flag clear
                Assert.That(pile.X, Is.InRange(200f, trail.Goal.X - 120f)); // Leaves room at the start and goal
            } // Ends the clearance check
        } // Ends the placement check
    } // Ends the Plains checks
} // Closes the tests
