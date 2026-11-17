using System.Linq; // Gives list helpers such as Select and All
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the Collection and the Profile totals of the Profile ticket 01 (D-052), only through public methods.</summary>
    public class CollectionTests // A group of checks
    {
        [Test] // Unity runs this method as a Test
        public void EveryItemKind_IsOnAShelf_AndIsARockOrMineral() // Each kind sits on one or more shelves (D-066)
        {
            Assert.IsTrue(ItemCatalog.Granite.BelongsTo(Episode.Mountains)); // Granite is taught in the Mountains lesson
            Assert.IsTrue(ItemCatalog.Quartz.BelongsTo(Episode.Mountains)); // So is quartz
            Assert.IsTrue(ItemCatalog.Feldspar.BelongsTo(Episode.Mountains)); // So is feldspar
            Assert.IsTrue(ItemCatalog.Mica.BelongsTo(Episode.Mountains)); // So is mica
            Assert.IsTrue(ItemCatalog.Granite.IsRock); // Granite is a rock
            Assert.IsFalse(ItemCatalog.Quartz.IsRock); // Quartz is a mineral
            Assert.IsFalse(ItemCatalog.Feldspar.IsRock); // Feldspar is a mineral
            Assert.IsFalse(ItemCatalog.Mica.IsRock); // Mica is a mineral
            foreach (var kind in ItemCatalog.All) Assert.IsTrue(kind.Landforms.Length >= 1, kind.Id); // Every kind is on at least one shelf
        }

        [Test] // Unity runs this method as a Test
        public void SharedKinds_AreOnBothMountainsAndHillsShelves_AndCountOnce() // Seven kinds are on both shelves (D-066)
        {
            var shared = new[] { ItemCatalog.Limestone, ItemCatalog.Calcite, ItemCatalog.Clay, ItemCatalog.Andesite, ItemCatalog.Quartz, ItemCatalog.Magnetite, ItemCatalog.Feldspar }; // The shared kinds from the spec
            foreach (var kind in shared) // Each shared kind
            {
                Assert.IsTrue(kind.BelongsTo(Episode.Mountains), kind.Id); // On the Mountains shelf
                Assert.IsTrue(kind.BelongsTo(Episode.Hills), kind.Id); // On the Hills shelf too
            }
            Assert.IsFalse(ItemCatalog.Granite.BelongsTo(Episode.Hills)); // Granite is Mountains only
            Assert.IsFalse(ItemCatalog.CoralStone.BelongsTo(Episode.Mountains)); // Coral stone is Hills only
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("quartz"); // Find a shared kind once
            Assert.AreEqual(1, Collection.FoundCount(p)); // It counts once, not once per shelf
            Assert.AreEqual(1, Collection.Shelves(p)[(int)Episode.Mountains].Slots.Count(s => s.IsFound)); // Found on the Mountains shelf
            Assert.AreEqual(1, Collection.Shelves(p)[(int)Episode.Hills].Slots.Count(s => s.IsFound)); // And on the Hills shelf
        }

        [Test] // Unity runs this method as a Test
        public void ShelfFoundCount_CountsOnlyThatShelf() // "Found here 1/17" on Hills, "Found here 2/34" on Mountains
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("granite"); // A Mountains-only kind
            p.RecordFoundKind("quartz"); // A shared kind
            var shelves = Collection.Shelves(p); // Every shelf
            Assert.AreEqual(2, shelves[(int)Episode.Mountains].FoundCount); // Both are on the Mountains shelf
            Assert.AreEqual(1, shelves[(int)Episode.Hills].FoundCount); // Only quartz is on the Hills shelf
            Assert.AreEqual(0, shelves[(int)Episode.Plains].FoundCount); // Plains has no kinds yet
        }

        [Test] // Unity runs this method as a Test
        public void HillsEasyKeepsItsThreeHillsOnlyKinds() // The first three Hills-only kinds are still on the shelf
        {
            CollectionAssert.AreEqual(new[] { "coral_stone", "limonite", "kaolinite" }, ItemCatalog.All.Where(k => k.Landforms.Length == 1 && k.BelongsTo(Episode.Hills)).Take(3).Select(k => k.Id)); // The original Hills-only kinds keep their order
            Assert.IsTrue(ItemCatalog.CoralStone.IsRock); // Coral stone is a rock
            Assert.IsFalse(ItemCatalog.Limonite.IsRock); // Limonite is a mineral
            Assert.IsFalse(ItemCatalog.Kaolinite.IsRock); // Kaolinite is a mineral
            Assert.IsTrue(ItemCatalog.All.Where(k => k.BelongsTo(Episode.Hills)).All(k => k.IsDraft)); // All Hills text is a draft (Q-04)
        }

        [Test] // Unity runs this method as a Test
        public void Shelves_AreOnePerLandform_InEpisodeOrder() // Mountains, Hills, Plains, Plateaus
        {
            var shelves = Collection.Shelves(new PlayerProgress()); // Shelves of a fresh profile
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Hills, Episode.Plains, Episode.Plateaus }, shelves.Select(s => s.Landform).ToArray()); // One per landform, in order
        }

        [Test] // Unity runs this method as a Test
        public void MountainsShelf_KeepsEasyAndNormalKindsInOrder() // New kinds come after these lessons
        {
            var mountains = Collection.Shelves(new PlayerProgress())[0]; // The first shelf
            CollectionAssert.AreEqual(new[] { "granite", "quartz", "feldspar", "mica", "andesite", "obsidian", "pumice", "slate", "marble", "garnet", "sandstone", "clay", "conglomerate", "shale", "basalt", "magnetite", "sulfur", "pyrite", "limestone", "gneiss" }, mountains.Slots.Take(20).Select(s => s.Kind.Id).ToArray()); // Easy and Normal kinds in order
            Assert.IsFalse(mountains.IsComingSoon); // It has kinds, so it is not "Coming soon"
        }

        [Test] // Unity runs this method as a Test
        public void LandformsWithoutItemKinds_AreComingSoon() // Plains and Plateaus wait for their levels
        {
            var shelves = Collection.Shelves(new PlayerProgress()); // Shelves of a fresh profile
            Assert.IsFalse(shelves[1].IsComingSoon); // Hills has its kinds now (D-066)
            Assert.AreEqual(17, shelves[1].Slots.Count); // Ten from Easy plus seven from Normal
            Assert.IsTrue(shelves[2].IsComingSoon); // Plains
            Assert.IsTrue(shelves[3].IsComingSoon); // Plateaus
            Assert.AreEqual(0, shelves[2].Slots.Count); // No slots on a "Coming soon" shelf
        }

        [Test] // Unity runs this method as a Test
        public void FreshProfile_HasNothingFound() // Every slot is still hidden
        {
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsTrue(Collection.Shelves(p).SelectMany(s => s.Slots).All(slot => !slot.IsFound)); // No slot is found
            Assert.AreEqual(0, Collection.FoundCount(p)); // Nothing found
            Assert.AreEqual(44, Collection.TotalCount); // Thirty-four Mountains kinds and ten Hills-only kinds
        }

        [Test] // Unity runs this method as a Test
        public void FoundState_FollowsTheProgress() // Found kinds show, the others stay hidden
        {
            var p = new PlayerProgress(); // A brand new profile
            p.RecordFoundKind("quartz"); // Found quartz
            p.RecordFoundKind("mica"); // Found mica
            var slots = Collection.Shelves(p)[0].Slots; // The Mountains slots
            CollectionAssert.AreEqual(new[] { "quartz", "mica" }, slots.Where(s => s.IsFound).Select(s => s.Kind.Id).ToArray()); // Only quartz and mica are found
            Assert.AreEqual(2, Collection.FoundCount(p)); // Two found
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
