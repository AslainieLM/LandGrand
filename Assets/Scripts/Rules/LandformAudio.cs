using System.Collections.Generic; // Gives Dictionary for the audio tables

namespace LandGrand.Rules // Plain C# rules with no Unity types
{ // Opens the block
    /// <summary>
    /// Which music and ambience files each landform uses (D-066), and which script makes each file. File names have no extension.
    /// Plateaus uses the Mountains files and scripts for now.
    /// </summary>
    public static class LandformAudio // The audio table for levels
    { // Opens the block
        static readonly Dictionary<Episode, string> Music = new Dictionary<Episode, string> // One level tune per landform
        { // Opens the block
            { Episode.Mountains, "bgm_mountains" }, // The Mountains tune
            { Episode.Hills, "bgm_hills" }, // The Hills tune
            { Episode.Plains, "bgm_plains" }, // The Plains tune
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
            { Episode.Plains, new Dictionary<Difficulty, string> // Plains ambience
                { // Opens Plains ambience
                    { Difficulty.Easy, "amb_grass_easy" }, // Wind and crickets
                    { Difficulty.Normal, "amb_wetland_normal" }, // Frogs and water birds
                    { Difficulty.Hard, "amb_dry_hard" }, // Dry wind and far thunder
                } }, // Ends Plains ambience
        }; // Ends the ambience table

        static readonly Dictionary<Episode, string> MusicScripts = new Dictionary<Episode, string> // The script that makes each tune
        { // Opens the block
            { Episode.Mountains, "make_mountains_audio.py" }, // Mountains tune
            { Episode.Hills, "make_hills_audio.py" }, // All Hills sounds come from one script (D-066)
            { Episode.Plains, "make_plains_audio.py" }, // Makes the Plains tune
        }; // Ends the music script table

        static readonly Dictionary<string, string> AmbienceScripts = new Dictionary<string, string> // The generator for each ambience file
        { // Opens the table
            { "amb_breeze_easy", "make_mountains_audio.py" }, // The Easy Mountains breeze
            { "amb_wind_normal", "make_normal_audio.py" }, // The Normal Mountains wind
            { "amb_wind_hard", "make_hard_audio.py" }, // The Hard Mountains wind
            { "amb_stream_easy", "make_hills_audio.py" }, // The Easy Hills stream
            { "amb_stream_normal", "make_hills_audio.py" }, // The Normal Hills stream
            { "amb_rain_hard", "make_hills_audio.py" }, // The Hard Hills rain
            { "amb_cave_drips", "make_hills_normal_audio.py" }, // The Callao Cave loop
            { "amb_wind_turbines", "make_hills_hard_audio.py" }, // The Pililla wind farm loop
            { "amb_city_hills", "make_hills_hard_audio.py" }, // The Seven Hills of Rome loop
            { "amb_grass_easy", "make_plains_audio.py" }, // Makes the grass loop
            { "amb_wetland_normal", "make_plains_audio.py" }, // Makes the wetland loop
            { "amb_dry_hard", "make_plains_audio.py" }, // Makes the dry wind loop
            { "amb_cold_plain", "make_plains_hard_audio.py" }, // Makes the West Siberian Plain cold wind (D-071)
        }; // Ends the ambience script table

        /// <summary>The level tune for this landform's levels.</summary>
        public static string MusicFor(Episode episode) => Music.TryGetValue(episode, out var file) ? file : Music[Episode.Mountains]; // Landforms without a tune use the Mountains tune

        /// <summary>The level ambience for this landform and difficulty.</summary>
        public static string AmbienceFor(Episode episode, Difficulty difficulty) => (Ambience.TryGetValue(episode, out var files) ? files : Ambience[Episode.Mountains])[difficulty]; // Landforms without their own sounds use the Mountains sounds

        public static string AmbienceFor(LevelData level) => level.Layout.AmbienceOverride ?? AmbienceFor(level.Episode, level.Difficulty); // Uses the level's own sound if it has one

        /// <summary>The script that makes this landform's tune, for the missing-file message.</summary>
        public static string MusicScriptFor(Episode episode) => MusicScripts.TryGetValue(episode, out var script) ? script : MusicScripts[Episode.Mountains]; // Landforms without a script use the Mountains script

        /// <summary>The script that makes this landform's ambience for this difficulty, for the missing-file message.</summary>
        public static string AmbienceScriptFor(Episode episode, Difficulty difficulty) => AmbienceScripts[AmbienceFor(episode, difficulty)]; // Finds the script for the chosen table entry

        public static string AmbienceScriptFor(LevelData level) => AmbienceScripts.TryGetValue(AmbienceFor(level), out var script) ? script : null; // Finds the script for the level's chosen loop if one is listed
    } // Ends the block
} // Ends the block
