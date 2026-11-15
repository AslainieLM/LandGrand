using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests plain rules
{ // Opens the tests
    public class RockfallTests // Checks the falling rock
    { // Opens the group
        [TestCase(0f, RockfallPhase.Warning)] // Cycle begins
        [TestCase(1.199f, RockfallPhase.Warning)] // Last warning moment
        [TestCase(1.2f, RockfallPhase.Falling)] // Fall begins
        [TestCase(1.201f, RockfallPhase.Falling)] // Falling now
        [TestCase(1.499f, RockfallPhase.Falling)] // Almost landed
        [TestCase(1.5f, RockfallPhase.Landed)] // Lands now
        [TestCase(1.501f, RockfallPhase.Landed)] // Still on ground
        [TestCase(1.649f, RockfallPhase.Landed)] // Almost gone
        [TestCase(1.65f, RockfallPhase.Clear)] // Gone now
        [TestCase(1.651f, RockfallPhase.Clear)] // Stays gone
        [TestCase(2.999f, RockfallPhase.Clear)] // Almost next cycle
        [TestCase(3f, RockfallPhase.Warning)] // Wraps to warning
        public void CycleEdges(float time, RockfallPhase phase) // Checks each threshold
        { // Starts the check
            Assert.AreEqual(phase, new Rockfall(100f).PhaseAt(time)); // Uses the level clock
        } // Ends the check
        [Test] // Runs this check
        public void OffsetAndHitBoxFollowTheRock() // Warning cannot hurt
        { // Starts the check
            var rock = new Rockfall(100f, 1f); // Starts one second into the cycle
            Assert.AreEqual(RockfallPhase.Warning, rock.PhaseAt(2f)); // Offset wraps
            Assert.IsFalse(rock.Hits(94f, 120f, 12f, 26f, 0f)); // Warning is safe
            Assert.AreEqual(135f, rock.RockBottomAt(0.35f).Value, 0.001f); // Halfway down
            Assert.IsTrue(rock.Hits(94f, 120f, 12f, 26f, 0.35f)); // Rock passes through the body
            Assert.IsFalse(rock.Hits(120f, 120f, 12f, 26f, 0.35f)); // Outside its width
            Assert.IsFalse(rock.Hits(94f, 0f, 12f, 26f, 0.7f)); // Gone is safe
            Assert.IsNull(rock.RockBottomAt(0.7f)); // No rock to draw
        } // Ends the check
    } // Ends the group
} // Ends the tests
