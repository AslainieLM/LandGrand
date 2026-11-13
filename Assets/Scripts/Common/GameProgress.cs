using LandGrand.Rules; // Gives PlayerProgress and ProgressStore
using System.IO; // Gives Path
using UnityEngine; // Gives Application and RuntimeInitializeOnLoadMethod

namespace LandGrand // Game code lives here
{
    /// <summary>
    /// The one place every scene reaches the saved player progress.
    /// It loads the save file when the game opens, before the first scene, so the Main Menu already has it.
    /// Volume settings are not here: they stay in GameSettings.
    /// </summary>
    public static class GameProgress // Shared progress holder
    {
        const string FileName = "progress.json"; // Name of the save file

        static ProgressStore store; // Reads and writes the file

        /// <summary>The player's progress. Loaded once at launch.</summary>
        public static PlayerProgress Current { get; private set; } = new PlayerProgress(); // Fresh until Load runs

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // Unity calls this once before any scene loads
        static void LoadAtLaunch() // Loads the save file at game start
        {
            store = new ProgressStore(Path.Combine(Application.persistentDataPath, FileName)); // Save file lives in the app's private data folder
            Current = store.Load(); // Read it; a missing or broken file gives a fresh profile
        }

        /// <summary>Writes the progress to the device now. Call it after name entry and after each level result.</summary>
        public static void Save() => store?.Save(Current); // Safe save through a temporary file
    }
}
