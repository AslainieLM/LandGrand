using NUnit.Framework; // Gives the [Test] and Assert tools

namespace LandGrand.Rules.Tests // Test code lives here
{
    /// <summary>Checks the per-landform music and ambience table (D-066), only through public methods.</summary>
    public class LandformAudioTests // A group of checks
    {
        [TestCase(Episode.Mountains, "bgm_mountains")] // Mountains keeps its tune
        [TestCase(Episode.Hills, "bgm_hills")] // Hills has its own tune
        [TestCase(Episode.Plains, "bgm_plains")] // Plains has its own tune
        [TestCase(Episode.Plateaus, "bgm_plateaus")] // Plateaus has its own tune (D-072)
        public void MusicFor_PicksTheLandformTune(Episode landform, string file) // One tune per landform
        {
            Assert.AreEqual(file, LandformAudio.MusicFor(landform)); // The file name, without extension
        }

        [TestCase(Episode.Mountains, Difficulty.Easy, "amb_breeze_easy")] // Mountains Easy keeps its breeze
        [TestCase(Episode.Mountains, Difficulty.Normal, "amb_wind_normal")] // Mountains Normal keeps its wind
        [TestCase(Episode.Mountains, Difficulty.Hard, "amb_wind_hard")] // Mountains Hard keeps its near-storm wind
        [TestCase(Episode.Hills, Difficulty.Easy, "amb_stream_easy")] // Hills Easy has a stream
        [TestCase(Episode.Hills, Difficulty.Normal, "amb_stream_normal")] // Hills Normal has a stream
        [TestCase(Episode.Hills, Difficulty.Hard, "amb_rain_hard")] // Hills Hard has rain
        [TestCase(Episode.Plains, Difficulty.Easy, "amb_grass_easy")] // Uses the Plains Easy loop
        [TestCase(Episode.Plains, Difficulty.Normal, "amb_wetland_normal")] // Uses the Plains Normal loop
        [TestCase(Episode.Plains, Difficulty.Hard, "amb_dry_hard")] // Uses the Plains Hard loop
        [TestCase(Episode.Plateaus, Difficulty.Easy, "amb_highland_easy")] // Uses the Plateaus Easy loop (D-072)
        [TestCase(Episode.Plateaus, Difficulty.Normal, "amb_canyon_normal")] // Uses the Plateaus Normal loop (D-072)
        [TestCase(Episode.Plateaus, Difficulty.Hard, "amb_highwind_hard")] // Uses the Plateaus Hard loop (D-072)
        public void AmbienceFor_PicksTheLandformAndDifficulty(Episode landform, Difficulty difficulty, string file) // Landform and difficulty choose the sound
        {
            Assert.AreEqual(file, LandformAudio.AmbienceFor(landform, difficulty)); // The file name, without extension
        }

        [TestCase(Episode.Mountains, "make_mountains_audio.py")] // Mountains tune comes from the Mountains script
        [TestCase(Episode.Hills, "make_hills_audio.py")] // Hills tune comes from the Hills script
        [TestCase(Episode.Plains, "make_plains_audio.py")] // Uses the Plains music script
        [TestCase(Episode.Plateaus, "make_plateaus_audio.py")] // Uses the Plateaus music script (D-072)
        public void MusicScriptFor_NamesTheScriptThatMakesTheTune(Episode landform, string script) // The script name for the tune
        {
            Assert.AreEqual(script, LandformAudio.MusicScriptFor(landform)); // The Python file name
        }

        [TestCase(Episode.Mountains, Difficulty.Easy, "make_mountains_audio.py")] // Mountains Easy breeze
        [TestCase(Episode.Mountains, Difficulty.Normal, "make_normal_audio.py")] // Mountains Normal wind
        [TestCase(Episode.Mountains, Difficulty.Hard, "make_hard_audio.py")] // Mountains Hard near-storm wind
        [TestCase(Episode.Hills, Difficulty.Easy, "make_hills_audio.py")] // Hills Easy stream, from the Hills script
        [TestCase(Episode.Hills, Difficulty.Normal, "make_hills_audio.py")] // Hills Normal stream, from the Hills script
        [TestCase(Episode.Hills, Difficulty.Hard, "make_hills_audio.py")] // Hills Hard rain, from the Hills script
        [TestCase(Episode.Plains, Difficulty.Easy, "make_plains_audio.py")] // Makes the Plains Easy loop
        [TestCase(Episode.Plains, Difficulty.Normal, "make_plains_audio.py")] // Makes the Plains Normal loop
        [TestCase(Episode.Plains, Difficulty.Hard, "make_plains_audio.py")] // Makes the Plains Hard loop
        [TestCase(Episode.Plateaus, Difficulty.Easy, "make_plateaus_audio.py")] // Makes the Plateaus Easy loop (D-072)
        [TestCase(Episode.Plateaus, Difficulty.Normal, "make_plateaus_audio.py")] // Makes the Plateaus Normal loop (D-072)
        [TestCase(Episode.Plateaus, Difficulty.Hard, "make_plateaus_audio.py")] // Makes the Plateaus Hard loop (D-072)
        public void AmbienceScriptFor_NamesTheScriptThatMakesTheFile(Episode landform, Difficulty difficulty, string script) // The script name for the ambience
        {
            Assert.AreEqual(script, LandformAudio.AmbienceScriptFor(landform, difficulty)); // The Python file name
        }

        [Test] // Unity runs this method as a Test
        public void EveryLandformDifficulty_HasAScriptForEachFile() // Each file in the table names a script
        {
            foreach (Episode landform in System.Enum.GetValues(typeof(Episode))) // Every landform
            {
                Assert.IsTrue(LandformAudio.MusicScriptFor(landform).EndsWith(".py"), landform.ToString()); // Has a script
                foreach (Difficulty difficulty in System.Enum.GetValues(typeof(Difficulty))) // Every difficulty
                {
                    Assert.IsTrue(LandformAudio.AmbienceScriptFor(landform, difficulty).EndsWith(".py"), landform + " " + difficulty); // Has a script
                }
            }
        }

        [Test] // Unity runs this method as a Test
        public void EveryLandformDifficulty_HasAnAmbienceAndAMusicName() // The table is complete for every episode
        {
            foreach (Episode landform in System.Enum.GetValues(typeof(Episode))) // Every landform
            {
                Assert.IsFalse(string.IsNullOrEmpty(LandformAudio.MusicFor(landform)), landform.ToString()); // Has a tune
                foreach (Difficulty difficulty in System.Enum.GetValues(typeof(Difficulty))) // Every difficulty
                {
                    Assert.IsTrue(LandformAudio.AmbienceFor(landform, difficulty).StartsWith("amb_"), landform + " " + difficulty); // Has a sound file
                }
            }
        }
    }
}
