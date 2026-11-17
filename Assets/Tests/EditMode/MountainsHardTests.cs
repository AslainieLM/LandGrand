using System; // Measures distances from hazards
using System.Linq; // Reads catalog lists
using NUnit.Framework; // Gives test checks

namespace LandGrand.Rules.Tests // Holds plain rule tests
{ // Opens the tests
    public class MountainsHardTests // Checks the new trail data
    { // Opens the checks
        static LevelData[] Hard => LevelCatalog.All.Where(l => l.Episode == Episode.Mountains && l.Difficulty == Difficulty.Hard).ToArray(); // All seven Hard trails
        static ItemKind[] EarlierKinds => LevelCatalog.All.Where(l => l.Episode == Episode.Mountains && l.Difficulty != Difficulty.Hard).SelectMany(l => l.Kinds).Distinct().ToArray(); // Kinds taught before Mountains Hard (Hills levels do not count here)

        [Test] // Runs the trail check
        public void SevenHardTrailsHaveThreeFlagsAndNinePickups() // Checks their shared size and time
        { // Starts the check
            CollectionAssert.AreEqual(Enumerable.Range(1, 7), Hard.Select(l => l.Number)); // Seven trails in play order
            foreach (var level in Hard) // Looks at each trail
            { // Starts the trail checks
                Assert.That(level.Layout.Width, Is.EqualTo(9600f), level.BannerName); // Trail width
                Assert.That(level.Start, Is.EqualTo(new Spot(40f, 0f)), level.BannerName); // Starts on the ground
                Assert.That(level.Goal, Is.EqualTo(new Spot(9440f, 0f)), level.BannerName); // Ends before the edge
                Assert.That(level.TimeLimitSeconds, Is.EqualTo(240), level.BannerName); // Four minutes
                CollectionAssert.AreEqual(new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, level.Checkpoints, level.BannerName); // All flags on the ground
                Assert.That(level.Pickups.Count, Is.EqualTo(9), level.BannerName); // Nine placed items
                Assert.That(level.Review.Count, Is.EqualTo(3), level.BannerName); // Three review sentences
                Assert.That(level.Lesson.Sentences.Count, Is.EqualTo(4), level.BannerName); // Four lesson sentences
                Assert.That(level.Lesson.Sentences.All(s => s.Length <= 44), Is.True, level.BannerName); // Each line fits
                Assert.That(level.HasDraftContent, Is.True, level.BannerName); // The lesson still needs review
            } // Ends the trail checks
        } // Ends the shared check

        [Test] // Runs the pickup check
        public void EachTrailTeachesTwoKindsAndReviewsOneEarlierKind() // Checks all 34 taught kinds
        { // Starts the check
            var newKinds = ItemCatalog.All.Where(k => k.BelongsTo(Episode.Mountains)).Except(EarlierKinds).ToArray(); // Kinds first taught on Hard
            Assert.That(newKinds.Length, Is.EqualTo(14)); // Seven new rocks and seven new minerals
            Assert.That(ItemCatalog.All.Count(k => k.BelongsTo(Episode.Mountains)), Is.EqualTo(34)); // The complete Mountains shelf
            foreach (var level in Hard) // Checks the items on each trail
            { // Starts the pickup checks
                var taught = level.Pickups.Where(p => newKinds.Contains(p.Kind)).GroupBy(p => p.Kind).ToArray(); // Groups the new pickups by kind
                Assert.That(taught.Length, Is.EqualTo(2), level.BannerName); // Two new kinds
                Assert.That(taught.Count(g => g.Key.IsRock), Is.EqualTo(1), level.BannerName); // One rock and one mineral
                Assert.That(taught.All(g => g.Count() == 4), Is.True, level.BannerName); // Four of each new kind
                Assert.That(level.Pickups.Count(p => EarlierKinds.Contains(p.Kind)), Is.EqualTo(1), level.BannerName); // One review pickup
            } // Ends the pickup checks
            CollectionAssert.AreEquivalent(newKinds, Hard.SelectMany(l => l.Kinds).Intersect(newKinds)); // Uses all new kinds
        } // Ends the pickup check

        [Test] // Runs the safety check
        public void RollingBouldersHaveClearRunsAwayFromFlags() // Checks the full run plus its margins
        { // Starts the check
            foreach (var level in Hard) // Checks each trail
            foreach (var roll in level.Layout.RollingBoulders) // Checks each running boulder
            { // Starts the run check
                string name = level.BannerName + " at " + roll.Spawn; // Says which run failed
                float left = roll.End - 8f, right = roll.Spawn + 8f; // Adds the clear margin
                Assert.That(roll.Spawn - roll.End, Is.InRange(1f, RollingBoulder.MaxRun), name); // Fits in one cycle
                Assert.IsFalse(level.Hazards.Any(h => Overlaps(left, right, h.Place.X, h.Width)), name); // No pit or spikes
                Assert.IsFalse(level.Layout.Boulders.Any(b => Overlaps(left, right, b.X, b.Width)), name); // No fixed boulders
                Assert.IsFalse(level.Layout.Vents.Any(v => Overlaps(left, right, v.X, v.Width)), name); // No vents
                Assert.IsFalse(level.Checkpoints.Any(c => c.X >= left - 60f && c.X <= right + 60f), name); // Flags stay well clear
            } // Ends the run check
        } // Ends the safety check

        [Test] // Runs the checkpoint and ice check
        public void FlagsAndIceHaveClearSpaceAroundThem() // Checks hazard distances
        { // Starts the check
            foreach (var level in Hard) // Checks each trail
            { // Starts the trail checks
                foreach (var flag in level.Checkpoints) // Checks the safety of this flag
                { // Starts the flag checks
                    Assert.IsFalse(level.Hazards.Any(h => Overlaps(flag.X - 100f, flag.X + 100f, h.Place.X, h.Width)), level.BannerName); // Keep pits and spikes clear
                    Assert.IsFalse(level.Layout.Rockfalls.Any(r => Math.Abs(r.X - flag.X) <= 120f), level.BannerName); // Keep falling rocks clear
                    Assert.IsFalse(level.Layout.IceZones.Any(i => i.Contains(flag.X)), level.BannerName); // The flag is off the ice
                } // Ends the flag checks
                foreach (var ice in level.Layout.IceZones) // Checks each ice patch
                    Assert.IsFalse(level.Layout.Rockfalls.Any(r => r.X >= ice.X - 30f && r.X <= ice.X + ice.Width + 30f), level.BannerName); // No rocks on or near the ice
            } // Ends the trail checks
        } // Ends the checkpoint check

        static bool Overlaps(float left, float right, float x, float width) => left <= x + width && x <= right; // Checks whether two stretches touch
    } // Ends the checks
} // Ends the tests
