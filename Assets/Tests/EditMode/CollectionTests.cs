using System.Linq; // Gives list helpers such as Select and All
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the Collection and the Profile totals of the Profile ticket 01 (D-052), only through public methods.</summary>
    public class CollectionTests // A group of checks
    {
        [Test] // Unity runs this method as a Test
        public void EveryItemKind_HasAHomeLandform_AndIsARockOrMineral() // Each kind sits on exactly one shelf
        {
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Granite.HomeLandform); // Granite is taught in the Mountains lesson
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Quartz.HomeLandform); // So is quartz
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Feldspar.HomeLandform); // So is feldspar
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Mica.HomeLandform); // So is mica
            Assert.IsTrue(ItemCatalog.Granite.IsRock); // Granite is a rock
            Assert.IsFalse(ItemCatalog.Quartz.IsRock); // Quartz is a mineral
            Assert.IsFalse(ItemCatalog.Feldspar.IsRock); // Feldspar is a mineral
            Assert.IsFalse(ItemCatalog.Mica.IsRock); // Mica is a mineral
        }

        [Test] // Unity runs this method as a Test
        public void Shelves_AreOnePerLandform_InEpisodeOrder() // Mountains, Hills, Plains, Plateaus
        {
            var shelves = Collection.Shelves(new PlayerProgress()); // Shelves of a fresh profile
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Hills, Episode.Plains, Episode.Plateaus }, shelves.Select(s => s.Landform).ToArray()); // One per landform, in order
        }

        [Test] // Unity runs this method as a Test
        public void MountainsShelf_HoldsTheTenMountainsKinds_InLevelOrder() // Easy 1, then Easy 2, then Easy 3 (D-055)
        {
            var mountains = Collection.Shelves(new PlayerProgress())[0]; // The first shelf
            CollectionAssert.AreEqual(new[] { "granite", "quartz", "feldspar", "mica", "andesite", "obsidian", "pumice", "slate", "marble", "garnet" }, mountains.Slots.Select(s => s.Kind.Id).ToArray()); // Ten kinds in order
            Assert.IsFalse(mountains.IsComingSoon); // It has kinds, so it is not "Coming soon"
        }

        [Test] // Unity runs this method as a Test
        public void LandformsWithoutItemKinds_AreComingSoon() // Hills, Plains and Plateaus wait on Q-04
        {
            var shelves = Collection.Shelves(new PlayerProgress()); // Shelves of a fresh profile
            Assert.IsTrue(shelves[1].IsComingSoon); // Hills
            Assert.IsTrue(shelves[2].IsComingSoon); // Plains
            Assert.IsTrue(shelves[3].IsComingSoon); // Plateaus
            Assert.AreEqual(0, shelves[1].Slots.Count); // No slots on a "Coming soon" shelf
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_HasNothingFound() // Every slot is still hidden
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsTrue(Collection.Shelves(p).SelectMany(s => s.Slots).All(slot => !slot.IsFound)); // No slot is found
            Assert.AreEqual(0, Collection.FoundCount(p)); // Nothing found
            Assert.AreEqual(10, Collection.TotalCount); // Ten Mountains kinds exist so far (D-055)
        }

        [Test] // Unity runs this method as a Test
        public void FoundState_FollowsTheProgress() // Found kinds show, the others stay hidden
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("quartz"); // Found quartz
            p.RecordFoundKind("mica"); // Found mica
            var slots = Collection.Shelves(p)[0].Slots; // The Mountains slots
            CollectionAssert.AreEqual(new[] { false, true, false, true, false, false, false, false, false, false }, slots.Select(s => s.IsFound).ToArray()); // Only quartz and mica are found
            Assert.AreEqual(2, Collection.FoundCount(p)); // Two found
        }

        [Test] // Unity runs this method as a Test
        public void MountainsShelf_SplitsIntoTwoPagesOfFive() // The Profile shows five slots and a page arrow (D-055)
        {
            var mountains = Collection.Shelves(new PlayerProgress())[0]; // Ten kinds
            Assert.AreEqual(2, mountains.PageCount(5)); // Two pages
            CollectionAssert.AreEqual(new[] { "granite", "quartz", "feldspar", "mica", "andesite" }, mountains.SlotsOnPage(0, 5).Select(s => s.Kind.Id).ToArray()); // First page
            CollectionAssert.AreEqual(new[] { "obsidian", "pumice", "slate", "marble", "garnet" }, mountains.SlotsOnPage(1, 5).Select(s => s.Kind.Id).ToArray()); // Second page
            Assert.AreEqual(5, mountains.SlotsOnPage(9, 5).Count); // A page past the end shows the last page
        }

        [Test] // Unity runs this method as a Test
        public void ComingSoonShelf_HasOneEmptyPage() // Nothing to page through
        {
            var hills = Collection.Shelves(new PlayerProgress())[1]; // No kinds yet
            Assert.AreEqual(1, hills.PageCount(5)); // One page
            Assert.AreEqual(0, hills.SlotsOnPage(0, 5).Count); // With nothing on it
        }

        [Test] // Unity runs this method as a Test
        public void UnknownFoundKinds_AreNotCounted() // An old or unknown id in the save does not break the count
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("granite"); // A real kind
            p.RecordFoundKind("unobtainium"); // Not in the catalog
            Assert.AreEqual(1, Collection.FoundCount(p)); // Only granite counts
        }

        [Test] // Unity runs this method as a Test
        public void Totals_AddUpStarsAndClearedLevels() // The numbers in the explorer strip
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.AreEqual(0, p.TotalStars); // No stars yet
            Assert.AreEqual(0, p.ClearedLevelCount); // No levels cleared yet
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // Level 1 with 3 stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // Level 2 with 1 star
            Assert.AreEqual(4, p.TotalStars); // 3 + 1
            Assert.AreEqual(2, p.ClearedLevelCount); // Two levels
        }

        [Test] // Unity runs this method as a Test
        public void Totals_AWorseReplay_DoesNotLowerThem() // Best stars only (D-045)
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // First try: 3 stars
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 1); // Replay: 1 star
            Assert.AreEqual(3, p.TotalStars); // Still 3
            Assert.AreEqual(1, p.ClearedLevelCount); // Still one level
        }
    }
}
