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

        [Test] // Runs the Plateaus check
        public void PlateausHasFiveDraftLinesAndItsOwnPicture() // Checks the exact Plateaus summary (D-072)
        { // Starts the Plateaus check
            var summary = LandformSummary.For(Episode.Plateaus); // Loads the Plateaus review
            Assert.AreEqual("Plateaus", summary.Title); // Names the landform
            Assert.AreEqual("bg_plateaus", summary.Picture); // Uses the Plateaus picture
            Assert.IsTrue(summary.IsDraft); // Still needs a teacher's review
            CollectionAssert.AreEqual(new[] { "Plateaus are high lands with flat tops.", "Their sides are steep, like a table.", "Lava, rivers and wind shape plateaus.", "Rivers can cut deep canyons in them.", "Many people farm on high plateaus." }, summary.Sentences); // Keeps the five draft lines
            Assert.IsTrue(summary.Sentences.All(s => s.Length <= 44)); // Fits the short reading lines
        } // Ends the Plateaus check
    } // Ends the checks
} // Ends the tests
