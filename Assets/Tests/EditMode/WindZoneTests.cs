using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests plain rules
{ // Opens the tests
    public class WindZoneTests // Checks gust phases and push
    { // Opens the group
        [TestCase(0f, WindPhase.Calm)] // Cycle begins
        [TestCase(1.999f, WindPhase.Calm)] // Almost warning
        [TestCase(2f, WindPhase.Warning)] // Warning starts
        [TestCase(2.001f, WindPhase.Warning)] // Warning continues
        [TestCase(2.599f, WindPhase.Warning)] // Almost pushing
        [TestCase(2.6f, WindPhase.Push)] // Push starts
        [TestCase(2.601f, WindPhase.Push)] // Push continues
        [TestCase(3.999f, WindPhase.Push)] // Last push moment
        [TestCase(4f, WindPhase.Calm)] // Wraps to calm
        public void CycleEdges(float time, WindPhase phase) // Checks the exact thresholds
        { // Starts the check
            Assert.AreEqual(phase, new WindZone(900f, 500f).PhaseAt(time)); // Reads the phase
        } // Ends the check
        [Test] // Runs this check
        public void OffsetWrapsAndPushStaysInsideTheZone() // Pushes only where placed
        { // Starts the check
            var wind = new WindZone(900f, 500f, 1f); // Shifts the cycle
            Assert.AreEqual(WindPhase.Calm, wind.PhaseAt(3f)); // Wraps with the offset
            Assert.AreEqual(-40f, wind.PushAt(1000f, 2f)); // Full push inside
            Assert.AreEqual(0f, wind.PushAt(899f, 2f)); // Safe before the zone
            Assert.AreEqual(0f, wind.PushAt(1401f, 2f)); // Safe after the zone
            Assert.AreEqual(0f, wind.PushAt(1000f, 1f)); // Warning does not push
            for (int i = 0; i < 100; i++) Assert.That(wind.PushAt(1000f, i * 0.1f), Is.InRange(-40f, 0f)); // Never pushes harder
        } // Ends the check
    } // Ends the group
} // Ends the tests
