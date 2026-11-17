using System.Linq; // Gives list helpers such as Where, Any and OrderBy
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the soft sand rule (spec section 5), its zone edges, Hills Easy 3's sand and that Mountains has none.</summary>
    public class SoftSandTests // A group of checks
    {
        static LevelData HillsThree => LevelCatalog.Find(Episode.Hills, Difficulty.Easy, 3); // La Paz Dunes

        [Test] // Unity runs this method as a Test
        public void WalkSpeed_IsSixtyPercent_OnSand_AndUnchangedOffSand() // The walking speed rule
        {
            Assert.AreEqual(0.6f, SoftSand.WalkShare, 0.0001f); // Sixty percent of the walking speed
            Assert.AreEqual(0.3f, SoftSand.StepSeconds, 0.0001f); // A step every 0.3 seconds
            Assert.AreEqual(84f * 0.6f, SoftSand.WalkSpeed(84f, true), 0.001f); // On sand the walk is slower
            Assert.AreEqual(84f, SoftSand.WalkSpeed(84f, false), 0.001f); // Off sand the walk is the same
            Assert.AreEqual(-84f * 0.6f, SoftSand.WalkSpeed(-84f, true), 0.001f); // Walking left is slowed the same way
            Assert.AreEqual(0f, SoftSand.WalkSpeed(0f, true), 0.001f); // Standing still stays still
        }

        [Test] // Unity runs this method as a Test
        public void Contains_IncludesBothEdges_AndNothingOutside() // The zone edges
        {
            var zone = new SandZone(560f, 160f); // Sand from 560 to 720
            Assert.IsTrue(zone.Contains(560f)); // The left edge counts
            Assert.IsTrue(zone.Contains(720f)); // The right edge counts
            Assert.IsTrue(zone.Contains(640f)); // The middle counts
            Assert.IsFalse(zone.Contains(559.9f)); // Just before the left edge does not
            Assert.IsFalse(zone.Contains(720.1f)); // Just after the right edge does not
        }

        [Test] // Unity runs this method as a Test
        public void IsOnSand_NeedsFlatGroundInsideAZone() // Ledges, boulders and the air are not sand
        {
            var zones = new[] { new SandZone(560f, 160f) }; // One zone
            Assert.IsTrue(SoftSand.IsOnSand(true, false, 0f, 600f, zones)); // Flat ground inside the zone
            Assert.IsFalse(SoftSand.IsOnSand(false, false, 0f, 600f, zones)); // In the air, not on sand
            Assert.IsFalse(SoftSand.IsOnSand(true, true, 0f, 600f, zones)); // On a moving or breakable solid, not on sand
            Assert.IsFalse(SoftSand.IsOnSand(true, false, 24f, 600f, zones)); // On a raised top, such as a ledge, not on sand
            Assert.IsFalse(SoftSand.IsOnSand(true, false, 0f, 500f, zones)); // Flat ground outside the zone
        }

        [Test] // Unity runs this method as a Test
        public void LevelLayout_HasNoSandUnlessItIsGiven() // Old levels keep their rules
        {
            Assert.AreEqual(0, LevelLayout.Plain(400f).SandZones.Count); // The plain layout has no sand
        }

        [Test] // Unity runs this method as a Test
        public void HillsEasy3_HasItsFourSandZones() // Spec section 6
        {
            var zones = HillsThree.Layout.SandZones; // La Paz's sand
            Assert.AreEqual(4, zones.Count); // Four zones
            Assert.AreEqual(560f, zones[0].X); // First zone starts at 560
            Assert.AreEqual(160f, zones[0].Width); // and is 160 wide
            Assert.AreEqual(1900f, zones[1].X); // Second zone starts at 1900
            Assert.AreEqual(400f, zones[1].Width); // and is 400 wide
            Assert.AreEqual(2640f, zones[2].X); // Third zone starts at 2640, just before the wide pit
            Assert.AreEqual(150f, zones[2].Width); // and is 150 wide
            Assert.AreEqual(4050f, zones[3].X); // Fourth zone starts at 4050
            Assert.AreEqual(330f, zones[3].Width); // and is 330 wide
        }

        [Test] // Unity runs this method as a Test
        public void HillsEasy3_FirstSandZoneHasNoPitOrSpikesWithin80Pixels() // The first patch is safe ground to learn on
        {
            var first = HillsThree.Layout.SandZones.OrderBy(zone => zone.X).First(); // The first zone
            foreach (var hazard in HillsThree.Hazards) // Every pit and spike trap
            {
                bool near = hazard.Place.X < first.X + first.Width + 80f && hazard.Place.X + hazard.Width > first.X - 80f; // Too close to the zone
                Assert.IsFalse(near, $"Hazard at {hazard.Place.X} is within 80 px of the first sand zone"); // None may be that close
            }
        }

        [Test] // Unity runs this method as a Test
        public void HillsEasy3_NoSandZoneIsWithin40PixelsOfTheCheckpoint() // Sand never sits next to the flag
        {
            foreach (var zone in HillsThree.Layout.SandZones) // Every zone
                foreach (var flag in HillsThree.Checkpoints) // The checkpoint flag
                {
                    bool near = flag.X > zone.X - 40f && flag.X < zone.X + zone.Width + 40f; // Within 40 px of the zone
                    Assert.IsFalse(near, $"Checkpoint at {flag.X} is within 40 px of sand at {zone.X}"); // None may be that close
                }
        }

        [Test] // Unity runs this method as a Test
        public void MountainsAndHillsEasy1And2_HaveNoSandZones() // Those levels play exactly as before
        {
            foreach (var level in LevelCatalog.All.Where(l => l.Episode == Episode.Mountains || (l.Episode == Episode.Hills && l.Difficulty == Difficulty.Easy && l.Number < 3))) // Mountains and Hills Easy 1 and 2
                Assert.AreEqual(0, level.Layout.SandZones.Count, level.SceneName); // No sand
        }
    }
}
