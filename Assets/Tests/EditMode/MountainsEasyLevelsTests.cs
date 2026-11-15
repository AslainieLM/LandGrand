using System.Linq; // Gives list helpers such as Count, Select and All
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the Mountains Easy 2 and Easy 3 levels (D-055), every level's layout data, steam vents and the start banner.</summary>
    public class MountainsEasyLevelsTests // A group of checks
    {
        static LevelData Easy(int n) => LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, n); // One Mountains Easy level

        [Test] // Unity runs this method as a Test
        public void Catalog_HasTheThreeMountainsEasyLevels_InPlayOrder() // 1, 2, 3
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Easy).Select(l => l.Number).ToArray()); // In order
            Assert.IsTrue(LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Easy).All(l => l.Episode == Episode.Mountains && l.Difficulty == Difficulty.Easy)); // All Mountains Easy
            Assert.AreEqual("MountainsEasy2", Easy(2).SceneName); // Start opens this scene
            Assert.AreEqual("MountainsEasy3", Easy(3).SceneName); // And this one
        }

        [Test] // Unity runs this method as a Test
        public void EveryLevel_FollowsTheEasyRules() // 5 pickups, 120 s, a checkpoint between start and goal (D-039)
        {
            foreach (var level in LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Easy)) // Every level
            {
                Assert.AreEqual(5, level.Pickups.Count, level.SceneName); // Five collectibles
                Assert.AreEqual(120, level.TimeLimitSeconds, level.SceneName); // Two minutes
                Assert.That(level.Checkpoints[0].X, Is.GreaterThan(level.Start.X), level.SceneName); // Checkpoint after the start
                Assert.That(level.Goal.X, Is.GreaterThan(level.Checkpoints[0].X), level.SceneName); // Goal after the checkpoint
                Assert.That(level.Goal.X, Is.LessThan(level.Layout.Width - 40f), level.SceneName); // Room for the goal picture before the end
                Assert.IsFalse(string.IsNullOrEmpty(level.BannerName), level.SceneName); // A name for the start banner
                Assert.That(level.Review.Count, Is.InRange(2, 3), level.SceneName); // Two or three review sentences
                Assert.That(level.Lesson.Sentences.Count, Is.InRange(3, 4), level.SceneName); // Three or four lesson sentences
                foreach (var s in level.Lesson.Sentences) Assert.That(s.Length, Is.LessThanOrEqualTo(44), s); // Each fits one 312-pixel line in Andika (44 letters measured at about 298 pixels)
                Assert.IsTrue(level.HasDraftContent, level.SceneName); // Still a draft (Q-04)
            }
        }

        [Test] // Unity runs this method as a Test
        public void EveryLevel_KeepsItsPartsInsideTheLevel_AndOffThePits() // Nothing floats past the end or over a gap
        {
            foreach (var level in LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Easy)) // Every level
            {
                var pits = level.Hazards.Where(h => h.Type == HazardType.Pit).ToArray(); // The gaps in the ground
                bool OverPit(float from, float to) => pits.Any(p => to > p.Place.X && from < p.Place.X + p.Width); // Does this stretch touch a gap?
                foreach (var p in level.Pickups) Assert.That(p.Place.X, Is.InRange(0f, level.Layout.Width), level.SceneName); // Pickups inside
                foreach (var h in level.Hazards) Assert.That(h.Place.X + h.Width, Is.LessThan(level.Layout.Width), level.SceneName); // Hazards inside
                foreach (var b in level.Layout.Boulders) Assert.IsFalse(OverPit(b.X, b.X + b.Width), level.SceneName + " boulder at " + b.X); // Boulders sit on ground
                foreach (var v in level.Layout.Vents) Assert.IsFalse(OverPit(v.X, v.X + v.Width), level.SceneName + " vent at " + v.X); // Vents sit in ground
                Assert.IsFalse(OverPit(level.Start.X - 8f, level.Start.X + 8f), level.SceneName); // The start has ground
                Assert.IsFalse(OverPit(level.Checkpoints[0].X - 8f, level.Checkpoints[0].X + 8f), level.SceneName); // The checkpoint has ground
                Assert.IsFalse(OverPit(level.Goal.X - 40f, level.Goal.X + 40f), level.SceneName); // The goal has ground
            }
        }

        [Test] // Unity runs this method as a Test
        public void Easy1_KeepsItsBuiltLayout() // Moving the layout into data must not change the level
        {
            var layout = Easy(1).Layout; // The first level's layout
            Assert.AreEqual(4800f, layout.Width); // Ten screens
            Assert.AreEqual(LevelTheme.Meadow, layout.Theme); // Grass and a cave
            Assert.AreEqual(9, layout.Boulders.Count); // Nine boulders
            Assert.AreEqual(9, layout.Ledges.Count); // Nine ledges
            Assert.AreEqual(0, layout.Vents.Count); // No vents
            Assert.AreEqual("Mountain Trail", Easy(1).BannerName); // Its banner
            CollectionAssert.AreEqual(new[] { 300f, 900f, 1300f, 2150f, 2700f, 3250f, 3850f, 4150f, 4350f }, layout.Boulders.Select(b => b.X).ToArray()); // Boulders where the old builder put them
            CollectionAssert.AreEqual(new[] { 520f, 1050f, 1500f, 2300f, 2920f, 3560f, 3620f, 3950f, 4480f }, layout.Ledges.Select(l => l.X).ToArray()); // Ledges where the old builder put them
            CollectionAssert.AreEqual(new[] { 40f, 28f, 28f, 28f, 28f, 28f, 56f, 28f, 28f }, layout.Ledges.Select(l => l.Top).ToArray()); // At the old heights
        }

        [Test] // Unity runs this method as a Test
        public void Easy2_IsTheVolcanoTrail() // Volcanic mountains (D-055)
        {
            var level = Easy(2); // The level under test
            Assert.AreEqual("Volcanic Mountains", level.Lesson.Title); // Lesson topic
            Assert.AreEqual("lesson_volcano", level.Lesson.Picture); // Its picture
            Assert.AreEqual("Volcano Trail", level.BannerName); // Its banner
            Assert.AreEqual(LevelTheme.Volcanic, level.Layout.Theme); // Dark lava rock
            CollectionAssert.AreEqual(new[] { "andesite", "quartz", "obsidian", "pumice" }, level.Kinds.Select(k => k.Id).ToArray()); // Three new kinds and quartz to review
            Assert.AreEqual(2, level.Hazards.Count(h => h.Type == HazardType.Pit)); // Two pits
            Assert.AreEqual(2, level.Hazards.Count(h => h.Type == HazardType.Spikes)); // Two spike traps
            Assert.AreEqual(2, level.Layout.Vents.Count); // A safe vent to try, and the one under the secret ledge
        }

        [Test] // Unity runs this method as a Test
        public void Easy2_SecretPumice_IsOnALedgeHigherThanADoubleJump() // Only the second vent reaches it
        {
            var level = Easy(2); // The level under test
            var pumice = level.Pickups.First(p => p.Kind == ItemCatalog.Pumice); // The secret pickup
            var ledge = level.Layout.Ledges.First(l => pumice.Place.X >= l.X && pumice.Place.X <= l.X + l.Width && l.Top == pumice.Place.Y); // The ledge it lies on
            Assert.That(ledge.Top, Is.GreaterThan(80f)); // A jump and a double jump reach about 76 pixels
            float launchHeight = SteamVent.LaunchSpeed * SteamVent.LaunchSpeed / (2f * 900f); // How high a puff throws the explorer, with the game's gravity
            Assert.That(launchHeight, Is.GreaterThan(ledge.Top)); // The puff gets above it
            Assert.IsTrue(level.Layout.Vents.Any(v => v.X + v.Width + 12f <= ledge.X && ledge.X - (v.X + v.Width) <= 24f)); // A vent sits just left of the ledge, with room so the rising explorer does not bump its head
        }

        [Test] // Unity runs this method as a Test
        public void Easy3_IsFoldedPeaks() // Folded mountains (D-055)
        {
            var level = Easy(3); // The level under test
            Assert.AreEqual("Folded Mountains", level.Lesson.Title); // Lesson topic
            Assert.AreEqual("lesson_folds", level.Lesson.Picture); // Its picture
            Assert.AreEqual("Folded Peaks", level.BannerName); // Its banner
            Assert.AreEqual(LevelTheme.FoldedRock, level.Layout.Theme); // Grey folded rock
            CollectionAssert.AreEqual(new[] { "slate", "mica", "marble", "garnet" }, level.Kinds.Select(k => k.Id).ToArray()); // Three new kinds and mica to review
            Assert.AreEqual(3, level.Hazards.Count(h => h.Type == HazardType.Pit)); // Three pits
            Assert.AreEqual(3, level.Hazards.Count(h => h.Type == HazardType.Spikes)); // Three spike traps
            Assert.AreEqual(0, level.Layout.Vents.Count); // Vents are only in Easy 2
            Assert.IsTrue(level.Hazards.Any(h => h.Type == HazardType.Pit && h.Width >= 64f)); // The wide pit that needs the double jump
        }

        [Test] // Unity runs this method as a Test
        public void NewItemKinds_AreMountainsRocksAndOneMineral() // Home landform and Rock/Mineral tags (D-055)
        {
            var rocks = new[] { ItemCatalog.Andesite, ItemCatalog.Obsidian, ItemCatalog.Pumice, ItemCatalog.Slate, ItemCatalog.Marble }; // The five new rocks
            foreach (var k in rocks) { Assert.IsTrue(k.IsRock, k.Id); Assert.AreEqual(Episode.Mountains, k.HomeLandform, k.Id); } // Rocks of the Mountains shelf
            Assert.IsFalse(ItemCatalog.Garnet.IsRock); // Garnet is a mineral
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Garnet.HomeLandform); // On the Mountains shelf too
            Assert.AreEqual(Episode.Mountains, ItemCatalog.Find("basalt").HomeLandform); // Normal now teaches basalt
        }

        [Test] // Unity runs this method as a Test
        public void ClearingAllThreeEasyLevels_UnlocksNormal() // LG-31
        {
            var p = new PlayerProgress(); // A fresh profile
            foreach (var level in LevelCatalog.All.Where(l => l.Difficulty == Difficulty.Easy)) { var run = new LevelRun(level); run.ReachGoal(); run.RecordResult(p); } // Clear each level with no items
            Assert.IsTrue(p.IsDifficultyUnlocked(Episode.Mountains, Difficulty.Normal)); // Normal opens
            Assert.AreEqual(0, p.NextLevelAfter(Episode.Mountains, Difficulty.Easy, 3)); // Continue after Easy 3 goes back to the trail
            Assert.IsNotNull(LevelCatalog.Find(Episode.Mountains, Difficulty.Normal, 1)); // Normal is now playable
        }

        [Test] // Unity runs this method as a Test
        public void Next_IsTheNextLevel_InsideADifficulty() // Easy 1 cleared: Next Level is Easy 2 (D-059)
        {
            var p = new PlayerProgress(); // A fresh profile
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 2); // Clear Easy 1
            var next = p.NextAfter(Episode.Mountains, Difficulty.Easy, 1); // What comes next
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Another level
            Assert.AreEqual(Difficulty.Easy, next.Difficulty); // Same difficulty
            Assert.AreEqual(2, next.Level); // Level 2
        }

        [Test] // Unity runs this method as a Test
        public void Next_AfterTheLastEasyLevel_IsNormalLevel1() // LG-32: the next available level in order
        {
            var p = new PlayerProgress(); // A fresh profile
            for (int n = 1; n <= 3; n++) p.RecordClear(Episode.Mountains, Difficulty.Easy, n, 1); // Clear all of Easy
            var next = p.NextAfter(Episode.Mountains, Difficulty.Easy, 3); // What comes next
            Assert.AreEqual(NextStep.NextLevel, next.Step); // Still a level, not an episode
            Assert.AreEqual(Difficulty.Normal, next.Difficulty); // The next difficulty
            Assert.AreEqual(1, next.Level); // Its first level
        }

        [Test] // Unity runs this method as a Test
        public void Next_AfterTheWholeLandform_IsTheNextEpisode() // Hard 7 cleared: Next Episode (D-059)
        {
            var p = new PlayerProgress(); // A fresh profile
            foreach (var d in new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard }) // Every difficulty
                for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) p.RecordClear(Episode.Mountains, d, n, 1); // Clear every level
            var next = p.NextAfter(Episode.Mountains, Difficulty.Hard, 7); // What comes next
            Assert.AreEqual(NextStep.NextEpisode, next.Step); // The next landform
            Assert.AreEqual(Episode.Hills, next.Episode); // Hills follows Mountains (D-039)
        }

        [Test] // Unity runs this method as a Test
        public void Next_IsNone_WhenNothingOpenFollows() // An uncleared level, or the very last landform
        {
            var p = new PlayerProgress(); // A fresh profile, nothing cleared
            Assert.AreEqual(NextStep.None, p.NextAfter(Episode.Mountains, Difficulty.Easy, 1).Step); // Easy 1 not cleared: Easy 2 is locked
        }

        [Test] // Unity runs this method as a Test
        public void ReviewPickup_OfAKnownKind_OnlyChimes() // Quartz in Easy 2 after Easy 1
        {
            var p = new PlayerProgress(); // A fresh profile
            p.RecordFoundKind("quartz"); // Found in Easy 1
            var run = new LevelRun(Easy(2)); // Play Easy 2
            int quartz = Easy(2).Pickups.ToList().FindIndex(x => x.Kind == ItemCatalog.Quartz); // Where quartz is in the list
            Assert.AreEqual(CollectResult.Repeat, run.Collect(quartz, p)); // Chime only, no pop-up
        }

        [Test] // Unity runs this method as a Test
        public void ReviewPickup_OfANeverFoundKind_PopsUp() // Someone who skipped quartz in Easy 1
        {
            var p = new PlayerProgress(); // A fresh profile, nothing found
            var run = new LevelRun(Easy(2)); // Play Easy 2
            int quartz = Easy(2).Pickups.ToList().FindIndex(x => x.Kind == ItemCatalog.Quartz); // Where quartz is in the list
            Assert.AreEqual(CollectResult.FirstFind, run.Collect(quartz, p)); // The pop-up shows
        }

        [Test] // Unity runs this method as a Test
        public void AllFiveItems_GiveThreeStars_OnEasy2AndEasy3() // Star rule on the new levels
        {
            foreach (int n in new[] { 2, 3 }) // Both new levels
            {
                var p = new PlayerProgress(); // A fresh profile
                var run = new LevelRun(Easy(n)); // An attempt
                for (int i = 0; i < 5; i++) { run.Collect(i, p); run.DismissFind(); } // Take all five, closing each pop-up
                Assert.IsTrue(run.ReachGoal()); // Reach the goal
                Assert.AreEqual(3, run.Stars, "Easy " + n); // Three stars
            }
        }

        [Test] // Unity runs this method as a Test
        public void Vent_PuffsOnAFixedCycle() // Rest, warning, puff, every 2 seconds
        {
            Assert.AreEqual(VentPhase.Idle, SteamVent.PhaseAt(0.2f)); // Resting
            Assert.AreEqual(VentPhase.Warning, SteamVent.PhaseAt(1.3f)); // The small wisp
            Assert.AreEqual(VentPhase.Puff, SteamVent.PhaseAt(1.8f)); // The puff
            Assert.AreEqual(VentPhase.Idle, SteamVent.PhaseAt(2.2f)); // The next cycle rests again
            Assert.AreEqual(VentPhase.Puff, SteamVent.PhaseAt(3.8f)); // And puffs again
        }

        [Test] // Unity runs this method as a Test
        public void Vent_LaunchesOnlyWhenStandingOnItDuringAPuff() // No puff, no launch
        {
            var vent = new SteamVent(100f); // A vent from x 100 to 124
            var run = new LevelRun(Easy(2)); // An attempt
            Assert.IsTrue(run.VentLaunches(vent, 112f, true, 1.8f)); // On it, puffing
            Assert.IsFalse(run.VentLaunches(vent, 112f, true, 0.5f)); // On it, resting
            Assert.IsFalse(run.VentLaunches(vent, 112f, true, 1.3f)); // On it, only the warning wisp
            Assert.IsFalse(run.VentLaunches(vent, 112f, false, 1.8f)); // In the air above it
            Assert.IsFalse(run.VentLaunches(vent, 140f, true, 1.8f)); // Next to it
        }

        [Test] // Unity runs this method as a Test
        public void Vent_NeverChangesTimeItemsRespawnOrStars() // It only throws the explorer up (D-055)
        {
            var p = new PlayerProgress(); // A fresh profile
            var run = new LevelRun(Easy(2)); // An attempt
            run.Tick(10f); // Some time passed
            run.Collect(0, p); run.DismissFind(); // One item taken
            float time = run.RemainingSeconds; var respawn = run.RespawnPoint; int taken = run.Collected; // The state before the launch
            Assert.IsTrue(run.VentLaunches(Easy(2).Layout.Vents[0], 1012f, true, 1.8f)); // A launch
            Assert.AreEqual(time, run.RemainingSeconds); // Same time
            Assert.AreEqual(taken, run.Collected); // Same items
            Assert.AreEqual(respawn.X, run.RespawnPoint.X); // Same respawn point
            Assert.IsTrue(run.ReachGoal()); // Clear it
            Assert.AreEqual(StarRule.StarsFor(1, 5), run.Stars); // Stars only from the items
        }

        [Test] // Unity runs this method as a Test
        public void Vent_DoesNotLaunchWhilePausedOrOver() // The pause menu, a pop-up, or after the goal
        {
            var vent = new SteamVent(100f); // A vent
            var run = new LevelRun(Easy(2)); // An attempt
            run.Pause(); // Pause menu open
            Assert.IsFalse(run.VentLaunches(vent, 112f, true, 1.8f)); // No launch while paused
            run.Resume(); // Back to play
            run.ReachGoal(); // Cleared
            Assert.IsFalse(run.VentLaunches(vent, 112f, true, 1.8f)); // No launch after the goal
        }

        [Test] // Unity runs this method as a Test
        public void StartBanner_HoldsTheClock_UntilItEnds() // No time passes before the run starts (D-055)
        {
            var run = new LevelRun(Easy(1)); // A fresh attempt
            run.BeginIntro(); // The banner shows
            Assert.IsTrue(run.InIntro); // The intro is on
            run.Tick(5f); // Five seconds on the banner
            Assert.AreEqual(120f, run.RemainingSeconds); // The clock did not move
            Assert.IsFalse(run.Pause()); // No pause menu over the banner
            Assert.AreEqual(CollectResult.None, run.Collect(0, new PlayerProgress())); // Nothing can be collected yet
            run.EndIntro(); // The banner leaves
            run.Tick(1f); // One second of play
            Assert.AreEqual(119f, run.RemainingSeconds); // Now the clock runs
        }

        [Test] // Unity runs this method as a Test
        public void Timer_WarnsInTheLast20Seconds_AndTicksInTheLast10() // The countdown effect (D-058)
        {
            var run = new LevelRun(Easy(1)); // A fresh attempt with 120 s
            Assert.IsFalse(run.IsTimeLow); // Plenty of time
            run.Tick(99.5f); // 20.5 s left
            Assert.IsFalse(run.IsTimeLow); // Not yet
            run.Tick(0.5f); // 20 s left
            Assert.IsTrue(run.IsTimeLow); // Red and hopping from here
            Assert.IsFalse(run.IsTicking); // No tick sound yet
            run.Tick(10f); // 10 s left
            Assert.IsTrue(run.IsTicking); // Ticks from here
            run.Tick(20f); // Time is up
            Assert.IsFalse(run.IsTimeLow); // No warning once the time is up
            Assert.IsFalse(run.IsTicking); // And no ticking
        }

        [Test] // Unity runs this method as a Test
        public void StartBanner_OnlyAtTheStartOfAnAttempt() // A later call cannot stop the clock
        {
            var run = new LevelRun(Easy(1)); // A fresh attempt
            run.Tick(3f); // Play has started
            run.BeginIntro(); // Too late for a banner
            Assert.IsFalse(run.InIntro); // No intro
        }
    }
}
