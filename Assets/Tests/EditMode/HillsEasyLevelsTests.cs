using System.Linq; // Gives list helpers such as Where, Select and Distinct
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the Hills Easy 1 to 3 level data (D-066, spec section 6): trails, items, lessons, themes and safe spaces.</summary>
    public class HillsEasyLevelsTests // A group of checks
    {
        static LevelData Hills(int n) => LevelCatalog.Find(Episode.Hills, Difficulty.Easy, n); // One Hills Easy level
        static LevelData[] HillsEasy => LevelCatalog.All.Where(l => l.Episode == Episode.Hills && l.Difficulty == Difficulty.Easy).ToArray(); // All three Hills Easy levels

        [Test] // Unity runs this method as a Test
        public void Catalog_HasTheThreeHillsEasyLevels_InPlayOrder() // 1, 2, 3
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, HillsEasy.Select(l => l.Number).ToArray()); // In order
            Assert.AreEqual("HillsEasy1", Hills(1).SceneName); // Start opens this scene
            Assert.AreEqual("HillsEasy2", Hills(2).SceneName); // And this one
            Assert.AreEqual("HillsEasy3", Hills(3).SceneName); // And this one
        }

        [Test] // Unity runs this method as a Test
        public void EveryLevel_FollowsTheEasyRules() // 5 pickups, 1 flag, 120 s, a draft lesson
        {
            foreach (var level in HillsEasy) // Every Hills Easy level
            {
                Assert.AreEqual(5, level.Pickups.Count, level.SceneName); // Five collectibles
                Assert.AreEqual(1, level.Checkpoints.Count, level.SceneName); // One checkpoint flag
                Assert.AreEqual(120, level.TimeLimitSeconds, level.SceneName); // Two minutes
                Assert.That(level.Checkpoints[0].X, Is.GreaterThan(level.Start.X), level.SceneName); // Checkpoint after the start
                Assert.That(level.Goal.X, Is.GreaterThan(level.Checkpoints[0].X), level.SceneName); // Goal after the checkpoint
                Assert.That(level.Goal.X, Is.LessThan(level.Layout.Width - 40f), level.SceneName); // Room for the goal picture
                Assert.IsFalse(string.IsNullOrEmpty(level.BannerName), level.SceneName); // A name for the start banner
                Assert.That(level.Review.Count, Is.InRange(2, 3), level.SceneName); // Two or three review sentences
                Assert.That(level.Lesson.Sentences.Count, Is.EqualTo(4), level.SceneName); // Four lesson sentences
                foreach (var s in level.Lesson.Sentences) Assert.That(s.Length, Is.LessThanOrEqualTo(44), s); // Each fits one line
                Assert.IsTrue(level.Lesson.IsDraft, level.SceneName); // The lesson is a draft (Q-04)
                Assert.IsTrue(level.HasDraftContent, level.SceneName); // Still a draft
            }
        }

        [Test] // Unity runs this method as a Test
        public void EveryLevel_HasOneSecretSpotOnAHighLedge() // The fourth pickup is the secret one
        {
            foreach (var level in HillsEasy) // Every Hills Easy level
            {
                var secret = level.Pickups[3]; // The secret spot
                var ledge = level.Layout.Ledges.FirstOrDefault(l => secret.Place.X >= l.X && secret.Place.X <= l.X + l.Width && l.Top == secret.Place.Y); // The ledge it lies on
                Assert.That(ledge.Width, Is.GreaterThan(0f), level.SceneName); // It lies on a ledge (a found ledge has a width; a missing one is all zeros)
                Assert.That(ledge.Top, Is.GreaterThanOrEqualTo(56f), level.SceneName); // A high ledge, out of reach from the ground
            }
        }

        [Test] // Unity runs this method as a Test
        public void EveryPickup_IsOnTheHillsShelf_AndTenKindsAreTaught() // The Hills shelf is complete after Easy (spec section 3)
        {
            foreach (var level in HillsEasy) // Every level
                foreach (var pickup in level.Pickups) // Every pickup
                    Assert.IsTrue(pickup.Kind.BelongsTo(Episode.Hills), pickup.Kind.Id); // Each kind is on the Hills shelf
            var kinds = HillsEasy.SelectMany(l => l.Kinds).Distinct().ToArray(); // Every kind the three levels use, once each
            Assert.AreEqual(10, kinds.Length); // Ten kinds: seven shared and three new
            CollectionAssert.AreEquivalent(ItemCatalog.All.Where(k => k.BelongsTo(Episode.Hills)), kinds); // They are the whole Hills shelf
        }

        [Test] // Unity runs this method as a Test
        public void EveryCheckpoint_HasAtLeast100PixelsOfSafeSpaceAroundIt() // Same check as Mountains Hard
        {
            static bool Overlaps(float left, float right, float x, float width) => left <= x + width && x <= right; // Checks whether two stretches touch
            foreach (var level in HillsEasy) // Every level
                foreach (var flag in level.Checkpoints) // Every flag
                {
                    Assert.IsFalse(level.Hazards.Any(h => Overlaps(flag.X - 100f, flag.X + 100f, h.Place.X, h.Width)), level.SceneName); // No pit or spikes near the flag
                }
        }

        [Test] // Unity runs this method as a Test
        public void Lessons_HaveTheirTitlesAndPictures() // What the Lesson Screen shows
        {
            CollectionAssert.AreEqual(new[] { "Hills", "Rolling Hills", "Sand Dunes" }, HillsEasy.Select(l => l.Lesson.Title).ToArray()); // Each lesson's title
            CollectionAssert.AreEqual(new[] { "lesson_chocolate_hills", "lesson_batanes", "lesson_la_paz" }, HillsEasy.Select(l => l.Lesson.Picture).ToArray()); // Each lesson's picture
        }

        [Test] // Unity runs this method as a Test
        public void EachLevel_HasItsThemeWidthGoalAndBanner() // The theme table in spec section 9
        {
            var one = Hills(1); // Chocolate Hills
            Assert.AreEqual(LevelTheme.ChocolateHills, one.Layout.Theme); // Its theme
            Assert.AreEqual(4800f, one.Layout.Width); // Its width
            Assert.AreEqual(4740f, one.Goal.X); // Its goal
            Assert.AreEqual("Chocolate Hills", one.BannerName); // Its banner
            var two = Hills(2); // Batanes
            Assert.AreEqual(LevelTheme.RollingPasture, two.Layout.Theme); // Its theme
            Assert.AreEqual(5400f, two.Layout.Width); // Its width
            Assert.AreEqual(5240f, two.Goal.X); // Its goal
            Assert.AreEqual("Batanes Hills", two.BannerName); // Its banner
            var three = Hills(3); // La Paz
            Assert.AreEqual(LevelTheme.SandDunes, three.Layout.Theme); // Its theme
            Assert.AreEqual(5400f, three.Layout.Width); // Its width
            Assert.AreEqual(5240f, three.Goal.X); // Its goal
            Assert.AreEqual("La Paz Dunes", three.BannerName); // Its banner
            foreach (var level in HillsEasy) Assert.AreEqual(new Spot(32f, 0f), level.Start); // Every level starts at the left edge on the ground
        }

        [Test] // Unity runs this method as a Test
        public void EachLevel_HasItsBoulderLedgeHazardCounts() // The exact numbers in spec section 6
        {
            Assert.AreEqual(8, Hills(1).Layout.Boulders.Count); // Chocolate Hills: eight boulders
            Assert.AreEqual(7, Hills(1).Layout.Ledges.Count); // and seven ledges
            Assert.AreEqual(5, Hills(2).Layout.Boulders.Count); // Batanes: five boulders
            Assert.AreEqual(7, Hills(2).Layout.Ledges.Count); // and seven ledges
            Assert.AreEqual(5, Hills(3).Layout.Boulders.Count); // La Paz: five boulders
            Assert.AreEqual(8, Hills(3).Layout.Ledges.Count); // and eight ledges
            CollectionAssert.AreEqual(new[] { 1880f, 3150f }, Hills(1).Hazards.Where(h => h.Type == HazardType.Pit).Select(h => h.Place.X).ToArray()); // Chocolate Hills pits
            CollectionAssert.AreEqual(new[] { 3450f }, Hills(1).Hazards.Where(h => h.Type == HazardType.Spikes).Select(h => h.Place.X).ToArray()); // Chocolate Hills spikes
            CollectionAssert.AreEqual(new[] { 800f, 2800f, 3900f }, Hills(3).Hazards.Where(h => h.Type == HazardType.Pit).Select(h => h.Place.X).ToArray()); // La Paz pits
            CollectionAssert.AreEqual(new[] { 1300f, 3200f, 3300f }, Hills(3).Hazards.Where(h => h.Type == HazardType.Spikes).Select(h => h.Place.X).ToArray()); // La Paz spikes
        }
    }
}
