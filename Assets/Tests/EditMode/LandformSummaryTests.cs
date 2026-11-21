using System.Linq; // Checks the draft lines
using NUnit.Framework; // Gives the test checks

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    public class LandformSummaryTests // Checks the completed landform reviews
    { // Opens the checks
        [Test] // Runs the Mountains check
        public void MountainsHasFiveShortDraftLinesAndItsOwnPicture() // Checks the full review content
        { // Starts the check
            var summary = LandformSummary.For(Episode.Mountains); // Loads the finished Mountains review
            Assert.AreEqual("Mountains", summary.Title); // Names the landform
            Assert.AreEqual("bg_mountains", summary.Picture); // Uses the menu picture
            Assert.IsTrue(summary.IsDraft); // Still needs a teacher's review
            Assert.AreEqual(5, summary.Sentences.Count); // Five lines
            Assert.IsTrue(summary.Sentences.All(s => s.Length <= 44)); // Fits the short reading lines
        } // Ends the Mountains check

        [Test] // Runs the Hills check
        public void HillsHasFiveShortDraftLinesAndItsOwnPicture() // Checks the Hills review content (D-066)
        { // Starts the check
            var summary = LandformSummary.For(Episode.Hills); // Loads the Hills review
            Assert.AreEqual("Hills", summary.Title); // Names the landform
            Assert.AreEqual("bg_hills", summary.Picture); // Uses the Hills menu picture
            Assert.IsTrue(summary.IsDraft); // Still needs a teacher's review
            CollectionAssert.AreEqual(new[] { "Hills are lower and rounder than mountains.", "Rain, rivers, wind and ice shape hills.", "Many hills are made of limestone or sand.", "Grass and trees hold hill soil in place.", "People farm, live and use wind on hills." }, summary.Sentences); // The exact draft lines
            Assert.IsTrue(summary.Sentences.All(s => s.Length <= 44)); // Fits the short reading lines
        } // Ends the Hills check

        [Test] // Runs the Plains check
        public void PlainsHasFiveDraftLinesAndItsOwnPicture() // Checks the exact Plains summary
        { // Starts the Plains check
            var summary = LandformSummary.For(Episode.Plains); // Loads the Plains review
            Assert.AreEqual("Plains", summary.Title); // Names the landform
            Assert.AreEqual("bg_plains", summary.Picture); // Uses the Plains picture
            Assert.IsTrue(summary.IsDraft); // Still needs a teacher's review
            CollectionAssert.AreEqual(new[] { "Plains are wide, flat and low lands.", "Rivers, wind, ice and lava can make plains.", "Plains soil is often silt, sand and clay.", "Many people farm and live on plains.", "Some plains lie flat beside the sea." }, summary.Sentences); // Keeps the five draft lines
        } // Ends the Plains check

        [TestCase(Episode.Plateaus, "bg_plateaus")] // Plateaus has a future review
        public void OtherLandformsHaveOnlyAComingSoonLine(Episode landform, string art) // Keeps the placeholders short
        { // Starts the check
            var summary = LandformSummary.For(landform); // Loads the short review
            Assert.AreEqual(landform.ToString(), summary.Title); // Keeps the landform's name
            Assert.AreEqual(art, summary.Picture); // Uses the correct menu art
            CollectionAssert.AreEqual(new[] { "Coming soon" }, summary.Sentences); // Only the placeholder line
            Assert.IsFalse(summary.IsDraft); // No draft lesson yet
        } // Ends the placeholder check
    } // Ends the checks
} // Ends the tests
