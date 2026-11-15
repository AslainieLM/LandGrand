using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests plain rules
{ // Opens the tests
    public class CrumbleLedgeTests // Checks breaking and returning
    { // Opens the group
        [TestCase(0f, true)] // Just touched
        [TestCase(0.499f, true)] // Almost broken
        [TestCase(0.5f, false)] // Breaks now
        [TestCase(0.501f, false)] // Stays gone
        [TestCase(3.499f, false)] // Almost back
        [TestCase(3.5f, true)] // Returns three seconds after breaking
        [TestCase(3.501f, true)] // Stays back until touched again
        public void SolidFollowsTimeSinceTouch(float now, bool solid) // Checks the time edges
        { // Starts the check
            Assert.AreEqual(solid, CrumbleLedge.IsSolid(0f, now)); // Uses the first touch
        } // Ends the check
        [Test] // Runs this check
        public void UntouchedLedgeStaysWhole() // Does not break on its own
        { // Starts the check
            Assert.AreEqual(CrumblePhase.Whole, CrumbleLedge.StateAt(null, 50f)); // Never touched
            Assert.AreEqual(CrumblePhase.Cracking, CrumbleLedge.StateAt(2f, 2.25f)); // Halfway to breaking
            Assert.AreEqual(0.5f, CrumbleLedge.CrackProgress(2f, 2.25f)); // Half cracked
            Assert.AreEqual(CrumblePhase.Back, CrumbleLedge.StateAt(2f, 5.5f)); // Ready again
        } // Ends the check
    } // Ends the group
} // Ends the tests
