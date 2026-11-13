namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>Checks the player's name: 1 to 12 letters, numbers or spaces.</summary>
    public static class NameRule // Name checker
    {
        /// <summary>The most characters a name can have.</summary>
        public const int MaxLength = 12; // Fits on the map and the result screen

        /// <summary>
        /// Cleans the typed name (removes spaces at both ends) and checks it.
        /// Returns true with the cleaned name, or false with a short message for the player.
        /// </summary>
        public static bool TryClean(string raw, out string name, out string message) // Never throws
        {
            name = ""; // No name until it passes
            string text = (raw ?? "").Trim(' '); // Remove spaces at both ends; a missing name counts as empty
            if (text.Length == 0) { message = "Type your name."; return false; } // Nothing typed
            if (text.Length > MaxLength) { message = "Use 12 letters or less."; return false; } // Too long
            foreach (char c in text) // Look at each character
            {
                if (c != ' ' && !char.IsLetterOrDigit(c)) { message = "Use only letters, numbers and spaces."; return false; } // Symbols are not allowed
            }
            name = text; // Give back the cleaned name
            message = ""; // No message when all is well
            return true; // The name is accepted
        }
    }
}
