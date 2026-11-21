using System.Linq; // Gives list helpers such as Count and Distinct
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the level data of ticket 06: Mountains Easy 1, its lesson, its items and its goals.</summary>
    public class LevelCatalogTests // A group of checks
    {
        static LevelData MountainsEasy1 => LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 1); // The first level

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_IsDescribedAsData() // Episode, difficulty, number and time limit
        {
            var level = MountainsEasy1; // The level under test
            Assert.IsNotNull(level); // It exists
            Assert.AreEqual(Episode.Mountains, level.Episode); // Right landform
            Assert.AreEqual(Difficulty.Easy, level.Difficulty); // Right difficulty
            Assert.AreEqual(1, level.Number); // Right number
            Assert.AreEqual(120, level.TimeLimitSeconds); // Easy gives 2 minutes (D-039)
        }

        [Test] // Unity runs this method as a Test
        public void LevelsWithoutData_AreNotFound() // Later levels are not written yet
        {
            Assert.IsNull(LevelCatalog.Find(Episode.Mountains, Difficulty.Hard, 8)); // Hard has only seven levels
            Assert.IsNull(LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 4)); // Easy has only three levels
            Assert.IsNotNull(LevelCatalog.Find(Episode.Hills, Difficulty.Easy, 1)); // Hills Easy 1 has data now (D-066)
            Assert.IsNull(LevelCatalog.Find(Episode.Hills, Difficulty.Easy, 4)); // Hills Easy has only three levels
            Assert.IsNotNull(LevelCatalog.Find(Episode.Hills, Difficulty.Normal, 1)); // Hills Normal has its first trail
            Assert.IsNull(LevelCatalog.Find(Episode.Hills, Difficulty.Normal, 6)); // Normal ends after its fifth trail
            Assert.IsNotNull(LevelCatalog.Find(Episode.Hills, Difficulty.Hard, 1)); // Hills Hard has its first trail
            Assert.IsNull(LevelCatalog.Find(Episode.Hills, Difficulty.Hard, 8)); // Hills Hard ends after its seventh trail
            Assert.IsNotNull(LevelCatalog.Find(Episode.Plains, Difficulty.Easy, 1)); // Plains Easy starts at level one
            Assert.IsNull(LevelCatalog.Find(Episode.Plains, Difficulty.Easy, 4)); // Easy ends after the third trail
            Assert.IsNull(LevelCatalog.Find(Episode.Plains, Difficulty.Normal, 1)); // Normal still waits for its data
            Assert.IsNull(LevelCatalog.Find(Episode.Plains, Difficulty.Hard, 1)); // Hard still waits for its data
            Assert.IsNull(LevelCatalog.Find(Episode.Plateaus, Difficulty.Easy, 1)); // Plateaus still waits for its data
        }

        [Test] // Unity runs this method as a Test
        public void TimeLimits_FollowTheDifficulty() // Easy 2, Normal 3, Hard 4 minutes (D-039)
        {
            Assert.AreEqual(120, LevelData.TimeLimitFor(Difficulty.Easy)); // 2 minutes
            Assert.AreEqual(180, LevelData.TimeLimitFor(Difficulty.Normal)); // 3 minutes
            Assert.AreEqual(240, LevelData.TimeLimitFor(Difficulty.Hard)); // 4 minutes
        }

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_HasADraftLessonOfThreeOrFourSentences() // The lesson before the level
        {
            var lesson = MountainsEasy1.Lesson; // The lesson under test
            Assert.AreEqual("Mountains", lesson.Title); // Named after the landform
            Assert.That(lesson.Sentences.Count, Is.InRange(3, 4)); // Three or four short sentences
            Assert.IsFalse(string.IsNullOrEmpty(lesson.Picture)); // It has a picture
            Assert.IsTrue(lesson.IsDraft); // Not reviewed by a teacher yet (D-043)
        }

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_HasFivePickupsOfTheFourMountainsKinds() // Granite, quartz, feldspar and mica; one twice
        {
            var level = MountainsEasy1; // The level under test
            Assert.AreEqual(5, level.Pickups.Count); // Easy levels have 5 collectibles
            CollectionAssert.AreEqual(new[] { "granite", "quartz", "feldspar", "mica" }, level.Kinds.Select(k => k.Id).ToArray()); // Each kind once, in the order they appear
        }

        [Test] // Unity runs this method as a Test
        public void ItemKinds_AreSharedData() // The same kind object is used by every level
        {
            var granite = ItemCatalog.Find("granite"); // Granite from the shared list
            Assert.IsNotNull(granite); // It exists
            Assert.AreSame(granite, MountainsEasy1.Pickups.First(p => p.Kind.Id == "granite").Kind); // The level uses the shared kind, not a copy
            Assert.IsNull(ItemCatalog.Find("diamond")); // Unknown kinds are not found
        }

        [Test] // Unity runs this method as a Test
        public void ItemKinds_HaveANamePictureAndDraftDescription() // What the lesson and the pop-up show
        {
            foreach (var kind in ItemCatalog.All) // Every kind
            {
                Assert.IsFalse(string.IsNullOrEmpty(kind.Name), kind.Id); // A name
                Assert.IsFalse(string.IsNullOrEmpty(kind.Picture), kind.Id); // A picture
                Assert.IsFalse(string.IsNullOrEmpty(kind.Description), kind.Id); // A short description
                Assert.IsTrue(kind.IsDraft, kind.Id); // Not reviewed by a teacher yet (D-043)
            }
            Assert.AreEqual(ItemCatalog.All.Count, ItemCatalog.All.Select(k => k.Id).Distinct().Count()); // No two kinds share an id
        }

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_ShowsItsGoals() // The guide on the Lesson Screen
        {
            CollectionAssert.AreEqual(new[] { "Collect 5/5", "Finish before time runs out" }, MountainsEasy1.Goals.ToArray()); // Both goals, in order
        }

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_HasACheckpointHazardsAndAGoalInOrder() // Start, checkpoint and goal from left to right
        {
            var level = MountainsEasy1; // The level under test
            Assert.That(level.Checkpoints[0].X, Is.GreaterThan(level.Start.X)); // Checkpoint after the start
            Assert.That(level.Goal.X, Is.GreaterThan(level.Checkpoints[0].X)); // Goal after the checkpoint
            Assert.AreEqual(1, level.Hazards.Count(h => h.Type == HazardType.Pit)); // One pit
            Assert.AreEqual(1, level.Hazards.Count(h => h.Type == HazardType.Spikes)); // One spike trap
        }

        [Test] // Unity runs this method as a Test
        public void MountainsEasy1_IsMarkedAsDraft() // The Lesson Screen shows the draft note
        {
            Assert.IsTrue(MountainsEasy1.HasDraftContent); // Lesson or items still need a teacher's review
            Assert.AreEqual("Draft — needs teacher review", LevelData.DraftNote); // The note's words (D-043)
        }

        [Test] // Unity runs this method as a Test
        public void SceneName_NamesTheLevelScene() // Start opens this scene (ticket 07)
        {
            Assert.AreEqual("MountainsEasy1", MountainsEasy1.SceneName); // Landform, difficulty and number
        }

        [Test] // Unity runs this method as a Test
        public void Review_HasTwoOrThreeDraftSentences() // Shown at the end of the Result Screen (ticket 11)
        {
            Assert.That(MountainsEasy1.Review.Count, Is.InRange(2, 3)); // Two or three sentences
            Assert.IsTrue(MountainsEasy1.Review.All(r => !string.IsNullOrWhiteSpace(r))); // None empty
        }
    }
}
