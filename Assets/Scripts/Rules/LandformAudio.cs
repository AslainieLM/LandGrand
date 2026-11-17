using System.Collections.Generic; // Gives Dictionary for the audio tables

namespace LandGrand.Rules // Plain C# rules with no Unity types
{ // Opens the block
    /// <summary>
    /// Which music and ambience files each landform uses (D-066), and which script makes each file. File names have no extension.
    /// Plains and Plateaus have no own files yet, so they use the Mountains files and scripts.
    /// </summary>
    public static class LandformAudio // The audio table for levels
    { // Opens the block
        static readonly Dictionary<Episode, string> Music = new Dictionary<Episode, string> // One level tune per landform
        { // Opens the block
            { Episode.Mountains, "bgm_mountains" }, // The Mountains tune
            { Episode.Hills, "bgm_hills" }, // The Hills tune
        }; // Ends the music table

        static readonly Dictionary<Episode, Dictionary<Difficulty, string>> Ambience = new Dictionary<Episode, Dictionary<Difficulty, string>> // One ambience set per landform, by difficulty
        { // Opens the block
            { Episode.Mountains, new Dictionary<Difficulty, string> // Mountains ambience
                { // Opens the block
                    { Difficulty.Easy, "amb_breeze_easy" }, // Light breeze
                    { Difficulty.Normal, "amb_wind_normal" }, // Stronger wind
                    { Difficulty.Hard, "amb_wind_hard" }, // Near-storm wind
                } }, // Ends Mountains ambience
            { Episode.Hills, new Dictionary<Difficulty, string> // Hills ambience
                { // Opens the block
                    { Difficulty.Easy, "amb_stream_easy" }, // A stream
                    { Difficulty.Normal, "amb_stream_normal" }, // A stream
                    { Difficulty.Hard, "amb_rain_hard" }, // Rain
                } }, // Ends Hills ambience
        }; // Ends the ambience table

        static readonly Dictionary<Episode, string> MusicScripts = new Dictionary<Episode, string> // The script that makes each tune
        { // Opens the block
            { Episode.Mountains, "make_mountains_audio.py" }, // Mountains tune
            { Episode.Hills, "make_hills_audio.py" }, // All Hills sounds come from one script (D-066)
        }; // Ends the music script table

        static readonly Dictionary<Episode, Dictionary<Difficulty, string>> AmbienceScripts = new Dictionary<Episode, Dictionary<Difficulty, string>> // The script that makes each ambience file
        { // Opens the block
            { Episode.Mountains, new Dictionary<Difficulty, string> // Mountains ambience scripts
                { // Opens the block
                    { Difficulty.Easy, "make_mountains_audio.py" }, // Light breeze
                    { Difficulty.Normal, "make_normal_audio.py" }, // Stronger wind
                    { Difficulty.Hard, "make_hard_audio.py" }, // Near-storm wind
                } }, // Ends Mountains ambience scripts
            { Episode.Hills, new Dictionary<Difficulty, string> // Hills ambience scripts
                { // Opens the block
                    { Difficulty.Easy, "make_hills_audio.py" }, // Hills script
                    { Difficulty.Normal, "make_hills_audio.py" }, // Hills script
                    { Difficulty.Hard, "make_hills_audio.py" }, // Hills script
                } }, // Ends Hills ambience scripts
        }; // Ends the ambience script table

        /// <summary>The level tune for this landform's levels.</summary>
        public static string MusicFor(Episode episode) => Music.TryGetValue(episode, out var file) ? file : Music[Episode.Mountains]; // Landforms without a tune use the Mountains tune

        /// <summary>The level ambience for this landform and difficulty.</summary>
        public static string AmbienceFor(Episode episode, Difficulty difficulty) => (Ambience.TryGetValue(episode, out var files) ? files : Ambience[Episode.Mountains])[difficulty]; // Landforms without their own sounds use the Mountains sounds

        /// <summary>The script that makes this landform's tune, for the missing-file message.</summary>
        public static string MusicScriptFor(Episode episode) => MusicScripts.TryGetValue(episode, out var script) ? script : MusicScripts[Episode.Mountains]; // Landforms without a script use the Mountains script

        /// <summary>The script that makes this landform's ambience for this difficulty, for the missing-file message.</summary>
        public static string AmbienceScriptFor(Episode episode, Difficulty difficulty) => (AmbienceScripts.TryGetValue(episode, out var scripts) ? scripts : AmbienceScripts[Episode.Mountains])[difficulty]; // Landforms without their own scripts use the Mountains scripts
    } // Ends the block
} // Ends the block
