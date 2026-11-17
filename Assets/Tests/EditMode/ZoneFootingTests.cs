using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the shared "standing on a zone" rule that ice and soft sand both use.</summary>
    public class ZoneFootingTests // A group of checks
    {
        static bool Inside(float x) => x >= 560f && x <= 720f; // A stand-in zone from 560 to 720

        [Test] // Unity runs this method as a Test
        public void IsStandingIn_FlatGroundInsideTheZone_Counts() // The normal case
        {
            Assert.IsTrue(ZoneFooting.IsStandingIn(true, false, 0f, 600f, Inside)); // Flat ground inside the zone
        }

        [Test] // Unity runs this method as a Test
        public void IsStandingIn_AirSolidsRaisedGroundAndOutside_DoNotCount() // Everything else is not standing on the zone
        {
            Assert.IsFalse(ZoneFooting.IsStandingIn(false, false, 0f, 600f, Inside)); // In the air
            Assert.IsFalse(ZoneFooting.IsStandingIn(true, true, 0f, 600f, Inside)); // On a moving or breakable solid
            Assert.IsFalse(ZoneFooting.IsStandingIn(true, false, 24f, 600f, Inside)); // On a raised top, such as a ledge
            Assert.IsFalse(ZoneFooting.IsStandingIn(true, false, 0f, 500f, Inside)); // Flat ground outside the zone
        }

        [Test] // Unity runs this method as a Test
        public void FlatTolerance_IsOneHundredthOfAPixel() // One shared tolerance for ice and sand
        {
            Assert.AreEqual(0.01f, ZoneFooting.FlatTolerance, 0.00001f); // The shared value
            Assert.IsTrue(ZoneFooting.IsStandingIn(true, false, 0.005f, 600f, Inside)); // Just inside the tolerance still counts
            Assert.IsFalse(ZoneFooting.IsStandingIn(true, false, 0.02f, 600f, Inside)); // Clearly above the ground line does not
        }
    }
}
