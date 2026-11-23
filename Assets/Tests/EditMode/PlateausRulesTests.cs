using System.Linq; // Gives list searches and counts
using NUnit.Framework; // Gives the test tools

namespace LandGrand.Rules.Tests // Holds the plain rule tests
{ // Opens the tests
    /// <summary>Checks the Plateaus rules added for Plateaus Easy (D-072): the updraft, kinds, themes and badges.</summary>
    public class PlateausRulesTests // Groups the Plateaus rule checks
    { // Opens the checks
        static bool Earned(string id, PlayerProgress p) => AchievementCatalog.Find(id).IsEarned(p); // Short way to ask about one badge

        static void ClearEpisode(PlayerProgress p, Episode e, int stars = 1) // Clears every level of one landform
        { // Starts the clears
            foreach (Difficulty d in System.Enum.GetValues(typeof(Difficulty))) // Easy, Normal and Hard
                for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) p.RecordClear(e, d, n, stars); // Each level number
        } // Ends the clears

        [TestCase(100f, 30f, true)] // The left side edge counts as inside
        [TestCase(132f, 30f, true)] // The right side edge counts as inside
        [TestCase(99.9f, 30f, false)] // Just left of the column is outside
        [TestCase(132.1f, 30f, false)] // Just right of the column is outside
        [TestCase(116f, -0.5f, true)] // Feet half a pixel below the ground still count
        [TestCase(116f, -0.6f, false)] // Feet lower than that do not count
        [TestCase(116f, 60.5f, true)] // Feet half a pixel above the top still count
        [TestCase(116f, 60.6f, false)] // Feet just above that are outside
        public void Updraft_ContainsTheColumnFromTheGroundToItsTop(float middleX, float feetY, bool inside) // Checks the column edges
        { // Starts the edge check
            var updraft = new Updraft(100f, 60f); // A column at x 100 that holds the feet at 60
            Assert.AreEqual(inside, updraft.Contains(middleX, feetY)); // Matches the expected answer
        } // Ends the edge check

        [Test] // Checks the fixed sizes
        public void Updraft_IsThirtyTwoWideAndRisesAtNinety() // Keeps the spec numbers
        { // Starts the size check
            Assert.AreEqual(32f, Updraft.Width); // The column width
            Assert.AreEqual(90f, Updraft.RiseSpeed); // The rising speed in pixels per second
            var updraft = new Updraft(240f, 72f); // A column with its own place and height
            Assert.AreEqual(240f, updraft.X); // Keeps the left edge
            Assert.AreEqual(72f, updraft.Top); // Keeps the hover height
        } // Ends the size check

        [Test] // Checks the rise far below the top
        public void RiseVelocity_IsNinetyFarBelowTheTop() // Rises at full speed
        { // Starts the speed check
            Assert.AreEqual(90f, new Updraft(100f, 60f).RiseVelocity(0f, 1f / 60f), 1e-4f); // Full speed from the ground
        } // Ends the speed check

        [Test] // Checks the rise at the top
        public void RiseVelocity_IsZeroAtTheTop() // Hovers without moving
        { // Starts the hover check
            Assert.AreEqual(0f, new Updraft(100f, 60f).RiseVelocity(60f, 1f / 60f)); // No rise at the top
            Assert.AreEqual(0f, new Updraft(100f, 60f).RiseVelocity(60.4f, 1f / 60f)); // Never pushes down from just above it
        } // Ends the hover check

        [Test] // Checks the last step to the top
        public void RiseVelocity_LandsExactlyOnTheTopWhenClose() // Stops the feet at the top in one step
        { // Starts the last step check
            const float dt = 1f / 60f; // One frame at 60 frames per second
            var updraft = new Updraft(100f, 60f); // A column with its top at 60
            float feet = 59f; // One pixel below the top, closer than 90 times dt
            float v = updraft.RiseVelocity(feet, dt); // The speed for this frame
            Assert.Less(v, Updraft.RiseSpeed); // Slower than full speed
            Assert.AreEqual(60f, feet + v * dt, 1e-4f); // The feet end on the top
        } // Ends the last step check

        [Test] // Checks a layout with no updrafts
        public void LevelLayout_HasNoUpdraftsByDefault() // Old callers do not change
        { // Starts the default check
            Assert.AreEqual(0, LevelLayout.Plain(800f).Updrafts.Count); // An empty list, never null
        } // Ends the default check

        [Test] // Checks a layout with updrafts
        public void LevelLayout_KeepsItsOwnCopyOfTheUpdrafts() // Nobody can change the layout later
        { // Starts the copy check
            var list = new[] { new Updraft(300f, 64f) }; // One column
            var layout = new LevelLayout(800f, LevelTheme.FallsEdge, new Boulder[0], new Ledge[0], updrafts: list); // A layout with that column
            list[0] = new Updraft(500f, 10f); // Changes the original array
            Assert.AreEqual(300f, layout.Updrafts.Single().X); // The layout still has the first column
        } // Ends the copy check

        [Test] // Checks every older level
        public void MountainsHillsAndPlainsLevels_HaveNoUpdrafts() // Old levels stay as before
        { // Starts the old level check
            var old = LevelCatalog.All.Where(l => l.Episode != Episode.Plateaus).ToArray(); // Every level outside Plateaus
            Assert.Greater(old.Length, 0); // There are older levels to check
            Assert.IsTrue(old.All(l => l.Layout.Updrafts.Count == 0)); // None has an updraft
        } // Ends the old level check

        [Test] // Checks the saved theme numbers
        public void PlateausThemes_AppendAfterSteppe() // Keeps the older theme values
        { // Starts the theme check
            var expected = new[] { LevelTheme.Steppe, LevelTheme.PineapplePlateau, LevelTheme.LakePlateau, LevelTheme.FallsEdge }; // The last Plains theme and the three additions
            CollectionAssert.AreEqual(new[] { 43, 44, 45, 46 }, expected.Select(theme => (int)theme)); // Keeps the saved numeric order
            Assert.AreEqual(47, System.Enum.GetValues(typeof(LevelTheme)).Length); // Nothing else was added
            var old = new[] { "Meadow", "Volcanic", "FoldedRock", "PineForest", "AshGorge", "MossyForest", "SnowPeak", "LavaSlope", "StormRidge", "EagleForest", "BonsaiForest", "GlacierHorn", "EquatorSummit", "SeaPeak", "ChocolateHills", "RollingPasture", "SandDunes", "IslandHills", "LimestoneCave", "RiverGorge", "RiceTerraces", "FairyChimneys", "TarsierForest", "RainySlope", "KarstRidge", "WindHills", "DrumlinBay", "CityHills", "KarstTowers", "RicePlain", "SaltBeds", "RiverPlain", "CandabaWetland", "LaharPlain", "RiverDelta", "FloatingVillage", "FloatingMarket", "Prairie", "Savanna", "Pampas", "FrozenPlain", "LimestonePlain", "GangesPlain", "Steppe" }; // The 44 themes before Plateaus
            for (int i = 0; i < old.Length; i++) Assert.AreEqual(old[i], ((LevelTheme)i).ToString(), "theme " + i); // Each old theme keeps its saved number
        } // Ends the theme check

        [Test] // Checks a frame with no time
        public void RiseVelocity_IsZeroWhenNoTimePassed() // Never divides by zero (not in the spec; keeps NaN out of the motor)
        { // Starts the zero time check
            Assert.AreEqual(0f, new Updraft(100f, 60f).RiseVelocity(0f, 0f)); // Below the top, no rise without time
            Assert.AreEqual(0f, new Updraft(100f, 60f).RiseVelocity(60f, 0f)); // At the top, no rise either
        } // Ends the zero time check

        [Test] // Checks the saved kind order
        public void OlderKindIds_DidNotMove() // The new kinds come last
        { // Starts the kind order check
            var old = new[] { "granite", "quartz", "feldspar", "mica", "andesite", "obsidian", "pumice", "slate", "marble", "garnet", "sandstone", "clay", "conglomerate", "shale", "basalt", "magnetite", "sulfur", "pyrite", "limestone", "gneiss", "scoria", "hornblende", "diorite", "gold", "tuff", "hematite", "serpentinite", "chromite", "schist", "calcite", "trachyte", "tanzanite", "gabbro", "olivine", "coral_stone", "limonite", "kaolinite", "aragonite", "coquina", "flowstone", "dripstone", "volcanic_breccia", "ignimbrite", "zeolite", "peridotite", "chlorite", "dacite", "quartzite", "travertine", "leucite", "siltstone", "halite", "gypsum", "mudstone", "peat", "laterite", "chalk", "loess", "flint" }; // The 59 kinds before Plateaus
            CollectionAssert.AreEqual(old, ItemCatalog.All.Take(59).Select(k => k.Id)); // Same ids in the same places
            CollectionAssert.AreEqual(new[] { "halloysite", "goethite", "gibbsite", "cristobalite" }, ItemCatalog.All.Skip(59).Select(k => k.Id)); // The four Plateaus kinds come last
            Assert.AreEqual(63, Collection.TotalCount); // The new total
        } // Ends the kind order check

        [Test] // Checks the four new kinds
        public void PlateausKinds_AreDraftMineralsOnThePlateausShelfOnly() // Uses the spec text
        { // Starts the new kind check
            var kinds = ItemCatalog.All.Skip(59).ToArray(); // Reads the four Plateaus kinds
            CollectionAssert.AreEqual(new[] { "Halloysite", "Goethite", "Gibbsite", "Cristobalite" }, kinds.Select(k => k.Name)); // Checks the names
            CollectionAssert.AreEqual(new[] { "halloysite", "goethite", "gibbsite", "cristobalite" }, kinds.Select(k => k.Picture)); // Checks the picture names
            Assert.IsTrue(kinds.All(k => !k.IsRock && k.IsDraft && k.Landforms.SequenceEqual(new[] { Episode.Plateaus }))); // Draft minerals on the Plateaus shelf only
            Assert.AreEqual("A soft white clay mineral. It forms when volcanic rock breaks down.", kinds[0].Description); // Halloysite text from the spec
            Assert.AreEqual("A brown iron mineral. It makes soil look rusty.", kinds[1].Description); // Goethite text from the spec
            Assert.AreEqual("A soft white mineral rich in aluminium. It forms in warm, wet soils.", kinds[2].Description); // Gibbsite text from the spec
            Assert.AreEqual("A form of silica, like quartz. It can make tiny white balls in volcanic rock.", kinds[3].Description); // Cristobalite text from the spec
        } // Ends the new kind check

        [Test] // Checks the six shared kinds
        public void SharedKinds_KeepTheirOldShelvesAndJoinPlateaus() // Keeps each find on its existing shelves
        { // Starts the shared kind check
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Plateaus }, ItemCatalog.Basalt.Landforms); // Basalt stays on Mountains
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Hills, Episode.Plains, Episode.Plateaus }, ItemCatalog.Andesite.Landforms); // Andesite stays on three shelves
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Plateaus }, ItemCatalog.Hematite.Landforms); // Hematite stays on Mountains
            CollectionAssert.AreEqual(new[] { Episode.Mountains, Episode.Plains, Episode.Plateaus }, ItemCatalog.Tuff.Landforms); // Tuff stays on Mountains and Plains
            CollectionAssert.AreEqual(new[] { Episode.Hills, Episode.Plateaus }, ItemCatalog.VolcanicBreccia.Landforms); // Volcanic breccia stays on Hills
            CollectionAssert.AreEqual(new[] { Episode.Plains, Episode.Plateaus }, ItemCatalog.Laterite.Landforms); // Laterite stays on Plains
        } // Ends the shared kind check

        [Test] // Checks the Plateaus shelf
        public void PlateausShelf_HasTenKindsInCatalogOrder() // Six shared and four new kinds
        { // Starts the shelf check
            var shelf = Collection.Shelves(new PlayerProgress())[(int)Episode.Plateaus]; // The Plateaus shelf of a fresh profile
            Assert.AreEqual(Episode.Plateaus, shelf.Landform); // It is the Plateaus shelf
            CollectionAssert.AreEqual(new[] { "andesite", "basalt", "tuff", "hematite", "volcanic_breccia", "laterite", "halloysite", "goethite", "gibbsite", "cristobalite" }, shelf.Slots.Select(s => s.Kind.Id)); // The exact list in catalog order
            Assert.IsFalse(shelf.IsComingSoon); // It is not "Coming soon"
        } // Ends the shelf check

        [Test] // Checks the older shelves
        public void OlderShelves_KeepTheirSizes() // Only Plateaus grew
        { // Starts the size check
            var shelves = Collection.Shelves(new PlayerProgress()); // Every shelf of a fresh profile
            CollectionAssert.AreEqual(new[] { 34, 23, 24, 10 }, shelves.Select(s => s.Slots.Count)); // Mountains, Hills, Plains and Plateaus
        } // Ends the size check

        [Test] // Checks the saved badge order
        public void OlderBadgeIds_DidNotMove() // The Plateaus badges come last
        { // Starts the badge order check
            var old = new[] { "first_steps", "rock_hound", "crystal_finder", "star_explorer", "volcano_visitor", "summit_climber", "perfect_trail", "mountain_collector", "philippine_peaks", "top_of_the_world", "rock_scholar", "brave_hiker", "world_peaks", "hard_star", "mountain_master", "pasture_pal", "dune_surfer", "rolling_star", "hill_collector", "philippine_hills", "balloon_rider", "slope_ranger", "world_hills", "steady_star", "hill_master", "salt_maker", "straw_jumper", "flat_star", "plain_collector", "philippine_plains", "delta_boater", "prairie_ranger", "world_plains", "steady_plains", "plain_master" }; // The 35 badges before Plateaus
            CollectionAssert.AreEqual(old, AchievementCatalog.All.Take(35).Select(a => a.Id)); // Same ids in the same places
            Assert.AreEqual(46, AchievementCatalog.All.Count); // The new total
        } // Ends the badge order check

        [Test] // Checks the Plateaus badge page
        public void PlateausBadges_AreElevenInSpecOrder() // Names, hints and tiers from the spec
        { // Starts the badge page check
            var badges = AchievementCatalog.ForLandform(Episode.Plateaus); // The Plateaus page
            CollectionAssert.AreEqual(new[] { "updraft_rider", "table_top", "high_star", "plateau_collector", "philippine_plateaus", "canyon_explorer", "mesa_ranger", "world_plateaus", "steady_plateaus", "plateau_master", "land_grand_champion" }, badges.Select(b => b.Id)); // The ids in saved order
            CollectionAssert.AreEqual(new[] { "Updraft Rider", "Table Top", "High Star", "Plateau Collector", "Philippine Plateaus", "Canyon Explorer", "Mesa Ranger", "World Plateaus", "Steady Plateaus", "Plateau Master", "Land Grand Champion" }, badges.Select(b => b.Name)); // The names
            CollectionAssert.AreEqual(new[] { "Clear Plateaus Easy 3.", "Clear Plateaus Normal 1.", "Get 3 stars on every Plateaus Easy level.", "Find every Plateaus rock and mineral.", "Clear Plateaus Easy 1 to 3.", "Clear Plateaus Normal 3.", "Clear a Plateaus Hard level.", "Clear Plateaus Hard 5 to 7.", "Get 3 stars on every Plateaus Hard level.", "Clear every Plateaus level.", "Clear every level of every landform." }, badges.Select(b => b.HowToEarn)); // The hints
            CollectionAssert.AreEqual(new[] { BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Silver, BadgeTier.Silver, BadgeTier.Gold, BadgeTier.Gold, BadgeTier.Gold }, badges.Select(b => b.Tier)); // The tiers
            Assert.IsTrue(badges.All(b => b.IsDraft)); // Names wait for review like the other badges
        } // Ends the badge page check

        [Test] // Checks a fresh profile
        public void PlateausBadges_StartLocked() // Nothing earned yet
        { // Starts the locked check
            var p = new PlayerProgress(); // A brand new profile
            Assert.IsFalse(AchievementCatalog.ForLandform(Episode.Plateaus).Any(b => b.IsEarned(p))); // Every Plateaus badge is locked
        } // Ends the locked check

        [Test] // Checks that other landforms do not earn Plateaus badges
        public void ThreeLandformsComplete_EarnNoPlateausBadge() // Plateaus badges need Plateaus progress
        { // Starts the other landform check
            var p = new PlayerProgress(); // A brand new profile
            ClearEpisode(p, Episode.Mountains, 3); // Every Mountains level with three stars
            ClearEpisode(p, Episode.Hills, 3); // Every Hills level with three stars
            ClearEpisode(p, Episode.Plains, 3); // Every Plains level with three stars
            Assert.IsFalse(AchievementCatalog.ForLandform(Episode.Plateaus).Any(b => b.IsEarned(p))); // Still all locked
        } // Ends the other landform check

        [Test] // Checks the master badge
        public void PlateauMaster_NeedsAllFifteenPlateausLevels() // Fourteen is not enough
        { // Starts the master check
            var p = new PlayerProgress(); // A brand new profile
            ClearEpisode(p, Episode.Plateaus); // Every Plateaus level
            var almost = new PlayerProgress(); // A second profile
            foreach (Difficulty d in System.Enum.GetValues(typeof(Difficulty))) // Easy, Normal and Hard
                for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) // Each level number
                    if (!(d == Difficulty.Hard && n == 7)) almost.RecordClear(Episode.Plateaus, d, n, 1); // All but Plateaus Hard 7
            Assert.IsFalse(Earned("plateau_master", almost)); // Fourteen levels do not earn it
            Assert.IsTrue(Earned("plateau_master", p)); // All fifteen earn it
        } // Ends the master check

        [Test] // Checks the champion badge
        public void LandGrandChampion_NeedsAllFourLandformsComplete() // Every landform must be finished
        { // Starts the champion check
            var p = new PlayerProgress(); // A brand new profile
            ClearEpisode(p, Episode.Mountains); // Finishes Mountains
            ClearEpisode(p, Episode.Hills); // Finishes Hills
            ClearEpisode(p, Episode.Plains); // Finishes Plains
            Assert.IsFalse(Earned("land_grand_champion", p)); // Three landforms are not enough
            var onlyPlateaus = new PlayerProgress(); // A second profile
            ClearEpisode(onlyPlateaus, Episode.Plateaus); // Finishes only Plateaus
            Assert.IsFalse(Earned("land_grand_champion", onlyPlateaus)); // One landform is not enough
            ClearEpisode(p, Episode.Plateaus); // Finishes Plateaus too
            Assert.IsTrue(Earned("land_grand_champion", p)); // All four earn it
        } // Ends the champion check

        [TestCase("updraft_rider", Difficulty.Easy, 3)] // Updraft Rider needs Plateaus Easy 3
        [TestCase("table_top", Difficulty.Normal, 1)] // Table Top needs Plateaus Normal 1
        [TestCase("canyon_explorer", Difficulty.Normal, 3)] // Canyon Explorer needs Plateaus Normal 3
        [TestCase("mesa_ranger", Difficulty.Hard, 4)] // Mesa Ranger needs any Plateaus Hard level
        public void PlateausClearBadge_NeedsItsOwnPlateausLevel(string badge, Difficulty d, int level) // Checks the required clear
        { // Starts the clear check
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Plains, d, level, 3); // The same level in Plains
            Assert.IsFalse(Earned(badge, p)); // Plains does not count
            p.RecordClear(Episode.Plateaus, d, level, 1); // The Plateaus level with one star
            Assert.IsTrue(Earned(badge, p)); // Now it is earned
        } // Ends the clear check

        [Test] // Checks the three-level badges
        public void PhilippinePlateausAndWorldPlateaus_NeedEveryListedLevel() // Two of three are not enough
        { // Starts the set check
            var p = new PlayerProgress(); // A brand new profile
            p.RecordClear(Episode.Plateaus, Difficulty.Easy, 1, 1); // Easy 1
            p.RecordClear(Episode.Plateaus, Difficulty.Easy, 2, 1); // Easy 2
            p.RecordClear(Episode.Plateaus, Difficulty.Hard, 5, 1); // Hard 5
            p.RecordClear(Episode.Plateaus, Difficulty.Hard, 6, 1); // Hard 6
            Assert.IsFalse(Earned("philippine_plateaus", p)); // Easy 3 is missing
            Assert.IsFalse(Earned("world_plateaus", p)); // Hard 7 is missing
            p.RecordClear(Episode.Plateaus, Difficulty.Easy, 3, 1); // Easy 3
            p.RecordClear(Episode.Plateaus, Difficulty.Hard, 7, 1); // Hard 7
            Assert.IsTrue(Earned("philippine_plateaus", p)); // All three Easy levels
            Assert.IsTrue(Earned("world_plateaus", p)); // Hard 5 to 7
        } // Ends the set check

        [Test] // Checks the star badges
        public void HighStarAndSteadyPlateaus_NeedThreeStarsOnEveryLevel() // Two stars on one level is not enough
        { // Starts the star check
            var p = new PlayerProgress(); // A brand new profile
            for (int n = 1; n <= 3; n++) p.RecordClear(Episode.Plateaus, Difficulty.Easy, n, n == 2 ? 2 : 3); // Easy 2 has only two stars
            for (int n = 1; n <= 7; n++) p.RecordClear(Episode.Plateaus, Difficulty.Hard, n, n == 7 ? 2 : 3); // Hard 7 has only two stars
            Assert.IsFalse(Earned("high_star", p)); // One Easy level is short
            Assert.IsFalse(Earned("steady_plateaus", p)); // One Hard level is short
            p.RecordClear(Episode.Plateaus, Difficulty.Easy, 2, 3); // Three stars on Easy 2
            p.RecordClear(Episode.Plateaus, Difficulty.Hard, 7, 3); // Three stars on Hard 7
            Assert.IsTrue(Earned("high_star", p)); // Every Easy level has three stars
            Assert.IsTrue(Earned("steady_plateaus", p)); // Every Hard level has three stars
        } // Ends the star check

        [Test] // Checks the collector badge
        public void PlateauCollector_NeedsAllTenPlateausKinds() // Counts the full Plateaus shelf
        { // Starts the collection check
            var p = new PlayerProgress(); // A brand new profile
            var kinds = ItemCatalog.All.Where(k => k.BelongsTo(Episode.Plateaus)).ToArray(); // The Plateaus shelf
            foreach (var kind in kinds.Take(9)) p.RecordFoundKind(kind.Id); // Finds nine kinds
            Assert.IsFalse(Earned("plateau_collector", p)); // One kind is still missing
            p.RecordFoundKind(kinds[9].Id); // Finds the last kind
            Assert.IsTrue(Earned("plateau_collector", p)); // The full shelf earns it
        } // Ends the collection check
    } // Ends the checks
} // Ends the tests
