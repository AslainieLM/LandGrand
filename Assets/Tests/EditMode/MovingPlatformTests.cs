using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests plain rules
{ // Opens the tests
    public class MovingPlatformTests // Checks platform travel
    { // Opens the group
        [TestCase(0f, 1000f)] // Starts at the left
        [TestCase(4.999f, 1199.96f)] // Almost at the turn
        [TestCase(5f, 1200f)] // Reaches the right
        [TestCase(5.001f, 1199.96f)] // Turns back
        [TestCase(7.5f, 1100f)] // Halfway back
        [TestCase(10f, 1000f)] // Completes the trip
        public void TravelsAtFortyPixelsPerSecond(float time, float left) // Uses the exact motor rule
        { // Starts the check
            Assert.AreEqual(left, new MovingPlatform(1000f, 1200f, 40f).LeftAt(time), 0.001f); // Reads the left edge
        } // Ends the check
    } // Ends the group
} // Ends the tests
