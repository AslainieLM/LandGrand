using System.IO; // Gives file and folder tools
using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks saving and loading only through ProgressStore and PlayerProgress.</summary>
    public class ProgressSaveTests // A group of checks
    {
        string folder; // A throw-away folder for each test
        string path; // The save file inside that folder

        [SetUp] // Unity runs this method as a SetUp
        public void MakeFolder() // Runs before each test
        {
            folder = Path.Combine(Path.GetTempPath(), "LandGrandTest_" + System.Guid.NewGuid().ToString("N")); // A unique folder name
            Directory.CreateDirectory(folder); // Create the folder
            path = Path.Combine(folder, "progress.json"); // The save file path
        }

        [TearDown] // Unity runs this method as a TearDown
        public void RemoveFolder() // Runs after each test
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, true); // Delete the folder and its files
        }

        static PlayerProgress Sample() // Builds a profile with some of everything
        {
            var p = new PlayerProgress { Name = "Ana \"Rock\" 7" }; // A name with quotes to test escaping
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 1, 3); // A 3-star clear
            p.RecordClear(Episode.Mountains, Difficulty.Easy, 2, 1); // A 1-star clear
            p.RecordFoundKind("granite"); // A found kind
            p.RecordFoundKind("mica"); // Another found kind
            return p; // Give it back
        }

        static void AssertSame(PlayerProgress a, PlayerProgress b) // Compares what the public methods show
        {
            Assert.AreEqual(a.ToJson(), b.ToJson()); // Same full content, including Normal, Hard and every found kind
            Assert.AreEqual(a.Name, b.Name); // Same name
            for (int n = 1; n <= 3; n++) // Check each Easy level
            {
                Assert.AreEqual(a.BestStars(Episode.Mountains, Difficulty.Easy, n), b.BestStars(Episode.Mountains, Difficulty.Easy, n)); // Same stars
                Assert.AreEqual(a.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, n), b.IsLevelAvailable(Episode.Mountains, Difficulty.Easy, n)); // Same availability
            }
            foreach (var kind in new[] { "granite", "mica", "quartz" }) Assert.AreEqual(a.HasFoundKind(kind), b.HasFoundKind(kind)); // Same found kinds
        }

        [Test] // Unity runs this method as a Test
        public void SaveThenLoad_GivesEqualProgress() // Round trip keeps everything
        {
            var store = new ProgressStore(path); // A store for the test file
            var saved = Sample(); // A profile with data
            store.Save(saved); // Write it
            AssertSame(saved, store.Load()); // Read it back and compare
        }

        [Test] // Unity runs this method as a Test
        public void MissingFile_GivesFreshProfile() // First launch has no file
        {
            var loaded = new ProgressStore(path).Load(); // Load with no file
            AssertSame(new PlayerProgress(), loaded); // It equals a new profile
        }

        [TestCase("")] // Empty file
        [TestCase("not json at all")] // Garbage text
        [TestCase("{\"name\":\"Ana\",\"levels\":[")] // Cut off in the middle
        public void CorruptFile_GivesFreshProfile(string text) // A broken file never crashes the game
        {
            File.WriteAllText(path, text); // Put the bad text in the save file
            AssertSame(new PlayerProgress(), new ProgressStore(path).Load()); // Loading gives a fresh profile
        }

        [Test] // Unity runs this method as a Test
        public void SecondSave_ReplacesTheFirst() // The swap works when a file already exists
        {
            var store = new ProgressStore(path); // A store for the test file
            store.Save(new PlayerProgress { Name = "First" }); // First save
            store.Save(Sample()); // Second save
            AssertSame(Sample(), store.Load()); // Load shows the second save
        }

        [Test] // Unity runs this method as a Test
        public void InterruptedSave_KeepsThePreviousSave() // A half-written temporary file must not harm the real save
        {
            var store = new ProgressStore(path); // A store for the test file
            store.Save(Sample()); // A good save exists
            File.WriteAllText(path + ".tmp", "{\"name\":\"Brok"); // Pretend the phone died while writing the temporary file
            AssertSame(Sample(), store.Load()); // The good save still loads
            store.Save(Sample()); // A later save still works
            AssertSame(Sample(), store.Load()); // And loads
        }
    }
}
