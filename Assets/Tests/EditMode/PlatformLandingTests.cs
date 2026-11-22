using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class PlatformLandingTests // Checks the sound trigger on a moving platform
    { // Opens the landing checks
        [Test] // Checks a landing, a ride and a new landing
        public void LandingSoundsOnceUntilTheExplorerLeaves() // Checks the support changes during play
        { // Starts the landing check
            var landing = new PlatformLanding(); // Starts without a rider
            Assert.IsFalse(landing.Observe(false, true)); // In the air before the landing
            Assert.IsTrue(landing.Observe(true, true)); // New support sounds once
            Assert.IsFalse(landing.Observe(true, true)); // Staying on the platform is quiet
            Assert.IsFalse(landing.Observe(false, true)); // Leaving is quiet
            Assert.IsTrue(landing.Observe(true, true)); // The next landing sounds again
        } // Ends the landing check

        [Test] // Checks support gained while play is frozen
        public void ResumingOnTheSameSupportDoesNotSound() // Covers the stale landing edge
        { // Starts the frozen support check
            var landing = new PlatformLanding(); // Starts without a rider
            Assert.IsFalse(landing.Observe(true, false)); // Records the support while frozen
            Assert.IsFalse(landing.Observe(true, false)); // Stays quiet while frozen
            Assert.IsFalse(landing.Observe(true, true)); // Resumes with the same support
        } // Ends the frozen support check

        [Test] // Checks support cleared during a freeze
        public void LeavingWhileFrozenAllowsTheNextRealLanding() // Covers a return that clears the old support
        { // Starts the departure check
            var landing = new PlatformLanding(); // Starts without a rider
            Assert.IsTrue(landing.Observe(true, true)); // Lands before the freeze
            Assert.IsFalse(landing.Observe(true, false)); // Freezing a rider is quiet
            Assert.IsFalse(landing.Observe(false, false)); // The old support is cleared during the freeze
            Assert.IsTrue(landing.Observe(true, true)); // New support during play sounds again
        } // Ends the departure check
    } // Ends the landing checks
} // Closes the tests
