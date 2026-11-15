using LandGrand.Rules; // Uses the ice rules
using NUnit.Framework; // Uses the test checks

namespace LandGrand.Tests // Holds rule tests
{ // Opens the test group
    public class IceSlideTests // Checks ice on the ground
    { // Opens the tests
        [TestCase(0f, 72f, 0.225f, 36f)] // Gains speed at 160 per second
        [TestCase(0f, -72f, 0.45f, -72f)] // Gains speed to the left as well
        [TestCase(72f, 0f, 0.36f, 36f)] // Loses speed at 100 per second
        [TestCase(72f, 0f, 0.72f, 0f)] // Comes to rest in 0.72 seconds
        [TestCase(72f, -72f, 0.5f, 22f)] // Turning slows at the same rate
        [TestCase(-30f, -72f, 1f, -72f)] // Never goes past the faster target
        [TestCase(-72f, 10f, 1f, 10f)] // Never goes past the target when turning
        public void WalkSpeedMovesToTheTarget(float current, float target, float dt, float expected) // Checks the change in one step
        { // Starts the check
            Assert.AreEqual(expected, IceSlide.Next(current, target, dt), 0.001f); // Compares the walking speed
        } // Ends the check

        [TestCase(20f, 0, false)] // Exactly twenty is below the sound trigger
        [TestCase(21f, 0, true)] // Releasing at speed begins a slide
        [TestCase(21f, -1, true)] // Turning left begins a slide
        [TestCase(-21f, 1, true)] // Turning right begins a slide
        [TestCase(-21f, -1, false)] // Holding the same direction does not begin a slide
        public void SlideSoundRequiresSpeedAndAReleaseOrTurn(float current, int direction, bool slides) // Checks when a slide can start
        { // Starts the check
            Assert.AreEqual(slides, IceSlide.IsSliding(current, direction)); // Tests the speed and direction rule
        } // Ends the slide check

        [Test] // Runs the zone check
        public void BothIceEdgesCount() // Checks the ground interval
        { // Starts the check
            var ice = new IceZone(600f, 600f); // Places the first ice patch
            Assert.IsFalse(ice.Contains(599.9f)); // Just before the ice
            Assert.IsTrue(ice.Contains(600f)); // At the left edge
            Assert.IsTrue(ice.Contains(1200f)); // At the right edge
            Assert.IsFalse(ice.Contains(1200.1f)); // Just after the ice
        } // Ends the check
    } // Ends the tests
} // Ends the test group
