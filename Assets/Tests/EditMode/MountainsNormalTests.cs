using System.Linq; // Uses catalog filters
using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests plain rules
{ // Opens the tests
    public class MountainsNormalTests // Checks the five specified trails
    { // Opens the group
        [Test] // Runs this check
        public void FiveNormalLevelsHaveSevenItemsAndTwoSafeFlags() // Checks the shared Normal rules
        { // Starts the check
            var levels = LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Normal).ToArray(); // Finds Normal trails
            Assert.AreEqual(5, levels.Length); // All five are playable
            foreach (var level in levels) // Checks each trail
            { // Opens the trail checks
                Assert.AreEqual(7200f, level.Layout.Width); // Uses the checked width
                Assert.AreEqual(7040f, level.Goal.X); // Uses the checked goal
                Assert.AreEqual(40f, level.Start.X); // Uses the checked start
                Assert.AreEqual(180, level.TimeLimitSeconds); // Three minutes
                Assert.AreEqual(7, level.Pickups.Count); // Seven pickups
                CollectionAssert.AreEqual(new[] { 2400f, 4800f }, level.Checkpoints.Select(p => p.X)); // Both flags in order
                foreach (var flag in level.Checkpoints) Assert.IsFalse(level.Hazards.Any(h => h.Type == HazardType.Pit && flag.X >= h.Place.X && flag.X <= h.Place.X + h.Width)); // Flags have ground
                foreach (var item in level.Pickups) // Checks placed items
                { // Opens the item check
                    Assert.That(item.Place.X, Is.InRange(0f, 7200f)); // Item is inside
                    Assert.AreSame(ItemCatalog.Find(item.Kind.Id), item.Kind); // Uses a known shared kind
                } // Ends the item check
                foreach (var hazard in level.Hazards) Assert.That(hazard.Place.X + hazard.Width, Is.InRange(0f, 7200f)); // Hazards are inside
                foreach (var ledge in level.Layout.CrumbleLedges) Assert.That(ledge.X + CrumbleLedge.Width, Is.InRange(0f, 7200f)); // Slabs are inside
                foreach (var rock in level.Layout.Rockfalls) Assert.That(rock.X, Is.InRange(8f, 7192f)); // Rocks are inside
                foreach (var platform in level.Layout.MovingPlatforms) Assert.That(platform.X1 + MovingPlatform.Width, Is.InRange(platform.X0, 7200f)); // Platforms stay inside
                foreach (var wind in level.Layout.WindZones) Assert.That(wind.X + wind.Width, Is.InRange(0f, 7200f)); // Gusts stay inside
                foreach (var sentence in level.Lesson.Sentences) Assert.That(sentence.Length, Is.LessThanOrEqualTo(44)); // Reading lines fit
                Assert.AreEqual(3, level.Review.Count); // Three review sentences
                Assert.IsTrue(level.HasDraftContent); // Still needs teacher review
            } // Ends the trail checks
        } // Ends the check
        [Test] // Runs this check
        public void NormalIntroducesTenKindsInLessonOrder() // Checks the Normal collection entries
        { // Starts the check
            CollectionAssert.AreEqual(new[] { "sandstone", "clay", "conglomerate", "shale", "basalt", "magnetite", "sulfur", "pyrite", "limestone", "gneiss" }, ItemCatalog.All.Skip(10).Take(10).Select(k => k.Id)); // Exact introduction order
            Assert.IsTrue(ItemCatalog.All.All(k => k.HomeLandform == Episode.Mountains && k.IsDraft)); // Shared draft shelf
        } // Ends the check
    } // Ends the group
} // Ends the tests
