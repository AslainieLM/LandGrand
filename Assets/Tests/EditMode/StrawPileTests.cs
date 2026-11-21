using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds tests for the plain rules
{ // Opens the tests
    public class StrawPileTests // Checks the pile top and bounce
    { // Opens the pile checks
        [TestCase(95f, 15f, false)] // The body touches only the left edge
        [TestCase(94.9f, 15f, false)] // The body is outside the left edge
        [TestCase(95.1f, 15f, true)] // The body overlaps the left edge
        [TestCase(153f, 15f, false)] // The body touches only the right edge
        [TestCase(153.1f, 15f, false)] // The body is outside the right edge
        [TestCase(152.9f, 15f, true)] // The body overlaps the right edge
        [TestCase(124f, 14f, false)] // The feet are too low
        [TestCase(124f, 14.5f, true)] // The bottom of the contact band counts
        [TestCase(124f, 15f, true)] // The feet are on the top
        [TestCase(124f, 15.5f, true)] // The top of the contact band counts
        [TestCase(124f, 16.5f, false)] // The feet are too high
        public void IsOnTop_NeedsBodyOverlapAndFeetAtTheTop(float middle, float feet, bool onTop) // Tests the exact contact edges
        { // Starts the contact check
            var pile = new StrawPile(100f); // The solid runs from x 101 to 147
            Assert.AreEqual(onTop, pile.IsOnTop(middle, 6f, feet)); // Body width is twelve pixels
        } // Ends the contact check

        [Test] // Runs the bounce height check
        public void LaunchSpeed_GivesAboutEightyNinePixelsOfRise() // Uses the game's gravity of 900
        { // Starts the launch check
            float speed = StrawPile.LaunchSpeed; // The upward speed at launch
            Assert.That(speed * speed / (2f * 900f), Is.EqualTo(88.9f).Within(0.1f)); // The rise above the pile
        } // Ends the launch check
    } // Ends the pile checks
} // Closes the tests
