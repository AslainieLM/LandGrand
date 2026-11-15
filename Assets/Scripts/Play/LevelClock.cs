namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The level's own time in seconds. It stops while the game is paused (the start banner, the pause menu,
    /// and the "You found an item!" pop-up), so platforms, rocks, wind, crumbling ledges and vents all wait too.
    /// The level controller moves it on; the moving objects read it instead of Unity's clock.
    /// </summary>
    public static class LevelClock // One shared clock for every moving object in the level
    {
        /// <summary>Seconds of play since the level opened, not counting paused time.</summary>
        public static float Now { get; private set; } // Read by the moving objects

        /// <summary>True while the game is playing and not paused.</summary>
        public static bool Running { get; private set; } // Read by short effects such as falling chips

        internal static void Reset() // A new level starts at time 0
        {
            Now = 0f; // Back to the start
            Running = false; // Waits for the first playing frame
        }

        internal static void Step(bool playing, float seconds) // Moves time on only while playing
        {
            Running = playing; // Remember for the effects
            if (playing) Now += seconds; // Paused time does not count
        }
    }
}
