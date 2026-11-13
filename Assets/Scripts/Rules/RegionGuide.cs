namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>The guide text that a locked region of the Landform Map shows when it is tapped.</summary>
    public static class RegionGuide // Words for locked regions
    {
        /// <summary>"Finish (the landform before it) first", or empty text for Mountains, which is never locked.</summary>
        public static string ForLocked(Episode e) // Names the landform the player must finish
        {
            if (e == Episode.Mountains) return ""; // The first landform has nothing before it
            return "Finish " + (e - 1) + " first"; // Landforms unlock in list order, so the one before is e - 1
        }

        /// <summary>"Clear all (the difficulty before it) levels", or empty text for Easy, which opens with its landform.</summary>
        public static string ForLockedDifficulty(Difficulty d) // Names the difficulty the player must clear
        {
            if (d == Difficulty.Easy) return ""; // Easy has nothing before it
            return "Clear all " + (d - 1) + " levels"; // Difficulties unlock in list order, so the one before is d - 1
        }
    }
}
