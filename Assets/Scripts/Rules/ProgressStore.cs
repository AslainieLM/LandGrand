using System.IO; // Gives file tools

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>Saves and loads PlayerProgress as one file. A save goes to a temporary file first, so an interrupted save keeps the old one.</summary>
    public class ProgressStore // Saves and loads the progress file
    {
        readonly string path; // The real save file

        public ProgressStore(string path) { this.path = path; } // Remember where to save

        /// <summary>Writes the progress. The old file stays safe until the new one is fully written.</summary>
        public void Save(PlayerProgress progress) // Safe save
        {
            string dir = Path.GetDirectoryName(path); // Folder of the save file
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir); // Make sure the folder exists
            string temp = path + ".tmp"; // Temporary file next to the real one
            File.WriteAllText(temp, progress.ToJson()); // Write everything to the temporary file first
            if (File.Exists(path)) File.Replace(temp, path, null); // Swap the new file in for the old one
            else File.Move(temp, path); // No old file: just rename
        }

        /// <summary>Reads the progress. A missing or broken file gives a fresh profile.</summary>
        public PlayerProgress Load() // Never throws for bad files
        {
            try // Run the risky part; broken text is handled below
            {
                if (File.Exists(path) && PlayerProgress.TryFromJson(File.ReadAllText(path), out var progress)) return progress; // Good file
            }
            catch (System.Exception) { } // Any read problem: fall through to a fresh profile
            return new PlayerProgress(); // Fresh profile
        }
    }
}
