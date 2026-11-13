using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks what the Landform Map shows: star progress per region and the guide on locked regions.</summary>
    public class LandformMapRulesTests // A group of checks
    {
        [Test] // Unity runs this method as a Test
        public void EachRegion_HasFortyFiveStarsToEarn() // 3 Easy + 5 Normal + 7 Hard levels, 3 stars each
        {
            Assert.AreEqual(45, PlayerProgress.MaxEpisodeStars); // 15 levels times 3 stars
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_HasNoStarsInAnyRegion() // A new player shows 0/45 everywhere
        {
            var p = new PlayerProgress(); // A brand new profile
            foreach (Episode e in System.Enum.GetValues(typeof(Episode))) // Every landform
                Assert.AreEqual(0, p.EpisodeStars(e)); // Nothing earned yet
        }

        [Test] // Unity runs this method as a Test
        public void EpisodeStars_AddsTheBestStarsOfThatRegionOnly() // Stars are counted per landform
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // 3 stars on Easy 1
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // 1 star on Easy 2
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 2); // A better replay: now 2 stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // A worse replay keeps 3 stars
            p.RecordClear(Episode.Hills, Difficulty.Easy, 1, 3); // Stars in another landform
            Assert.AreEqual(5, p.EpisodeStars(Episode.Mountains)); // 3 + 2
            Assert.AreEqual(3, p.EpisodeStars(Episode.Hills)); // Counted apart
            Assert.AreEqual(0, p.EpisodeStars(Episode.Plains)); // Nothing here
        }

        [Test] // Unity runs this method as a Test
        public void LockedRegions_AskToFinishTheLandformBefore() // The guide names the landform before it
        {
            Assert.AreEqual("Finish Mountains first", RegionGuide.ForLocked(Episode.Hills)); // Hills comes after Mountains
            Assert.AreEqual("Finish Hills first", RegionGuide.ForLocked(Episode.Plains)); // Plains comes after Hills
            Assert.AreEqual("Finish Plains first", RegionGuide.ForLocked(Episode.Plateaus)); // Plateaus comes after Plains
        }

        [Test] // Unity runs this method as a Test
        public void Mountains_HasNoLockedGuide() // The first landform is never locked
        {
            Assert.AreEqual("", RegionGuide.ForLocked(Episode.Mountains)); // Nothing to finish before it
        }
    }
}
