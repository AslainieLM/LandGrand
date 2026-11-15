using System.Linq; // Filters the collection
using NUnit.Framework; // Uses test checks
namespace LandGrand.Rules.Tests // Tests profile rules
{ // Opens the tests
    public class NormalProfileTests // Checks paging and the new badges
    { // Opens the group
        [Test] // Runs this check
        public void TwentyKindsUseTwoPagesAndClampPageNumbers() // Keeps every kind reachable
        { // Starts the check
            var shelf = Collection.Shelves(new PlayerProgress())[0]; // Mountains shelf
            Assert.AreEqual(2, shelf.PageCount(12)); // Two pages
            Assert.AreEqual(12, shelf.SlotsOnPage(0, 12).Count); // Full first page
            Assert.AreEqual(8, shelf.SlotsOnPage(1, 12).Count); // Remaining kinds
            Assert.AreEqual("conglomerate", shelf.SlotsOnPage(1, 12)[0].Kind.Id); // Next kind in lesson order
            Assert.AreEqual("granite", shelf.SlotsOnPage(-1, 12)[0].Kind.Id); // Clamps to first page
            Assert.AreEqual("conglomerate", shelf.SlotsOnPage(99, 12)[0].Kind.Id); // Clamps to last page
            var twelve = new Shelf(Episode.Mountains, shelf.Slots.Take(12).ToArray()); // Exactly one grid
            Assert.AreEqual(1, twelve.PageCount(12)); // No page arrows needed
        } // Ends the check
        [Test] // Runs this check
        public void PhilippinePeaksNeedsNormalOneThroughFour() // Easy clears do not count
        { // Starts the check
            var p = new PlayerProgress(); // Fresh explorer
            var badge = AchievementCatalog.Find("philippine_peaks"); // Silver peaks badge
            for (int n = 1; n <= 3; n++) p.RecordClear(Episode.Mountains, Difficulty.Easy, n, 1); // Clears Easy
            Assert.IsFalse(badge.IsEarned(p)); // Normal still required
            for (int n = 1; n <= 3; n++) p.RecordClear(Episode.Mountains, Difficulty.Normal, n, 1); // First three Normal trails
            Assert.IsFalse(badge.IsEarned(p)); // Apo still required
            p.RecordClear(Episode.Mountains, Difficulty.Normal, 4, 1); // Clears Apo
            Assert.IsTrue(badge.IsEarned(p)); // Earns the badge
        } // Ends the check
        [Test] // Runs this check
        public void EverestClearEarnsTopOfTheWorld() // Requires Normal five
        { // Starts the check
            var p = new PlayerProgress(); // Fresh explorer
            var badge = AchievementCatalog.Find("top_of_the_world"); // Gold summit badge
            p.RecordClear(Episode.Mountains, Difficulty.Normal, 4, 3); // Apo is not Everest
            Assert.IsFalse(badge.IsEarned(p)); // Still locked
            p.RecordClear(Episode.Mountains, Difficulty.Normal, 5, 1); // Reaches Everest goal
            Assert.IsTrue(badge.IsEarned(p)); // One star is enough
        } // Ends the check
        [Test] // Runs this check
        public void ScholarNeedsFifteenKnownDistinctKinds() // Repeated and unknown finds do not count
        { // Starts the check
            var p = new PlayerProgress(); // Fresh explorer
            var badge = AchievementCatalog.Find("rock_scholar"); // Silver collection badge
            foreach (var kind in ItemCatalog.All.Take(14)) p.RecordFoundKind(kind.Id); // Fourteen known kinds
            p.RecordFoundKind("granite"); p.RecordFoundKind("unknown"); // Duplicate and unknown entries
            Assert.IsFalse(badge.IsEarned(p)); // Still fourteen
            p.RecordFoundKind("basalt"); // Fifteenth known kind
            Assert.IsTrue(badge.IsEarned(p)); // Earns the badge
        } // Ends the check
    } // Ends the group
} // Ends the tests
