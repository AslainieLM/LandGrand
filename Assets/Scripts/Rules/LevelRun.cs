namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>How an attempt at a level stands. Later tickets add pause, pop-up and time up.</summary>
    public enum RunOutcome { Playing, Cleared } // Attempt states

    /// <summary>
    /// One attempt at a level (ticket 07): it starts at the level's start and is cleared when the explorer reaches the goal.
    /// Collecting (ticket 08), hazards and the checkpoint (ticket 09) and the timer (ticket 10) build on it.
    /// </summary>
    public sealed class LevelRun // The state of one attempt
    {
        /// <summary>The level being played.</summary>
        public LevelData Level { get; } // Never changes during the attempt

        /// <summary>Playing, or cleared once the goal is reached.</summary>
        public RunOutcome Outcome { get; private set; } = RunOutcome.Playing; // Every attempt starts by playing

        /// <summary>Where the explorer appears: the level's start.</summary>
        public Spot RespawnPoint => Level.Start; // Ticket 09 moves it to the checkpoint

        /// <summary>Items collected so far. Always 0 until ticket 08 adds collecting.</summary>
        public int Collected => 0; // Nothing can be collected yet

        /// <summary>Stars earned: 0 while playing, 1 to 3 once cleared (D-039).</summary>
        public int Stars { get; private set; } // Set when the goal is reached

        public LevelRun(LevelData level) { Level = level; } // Starts an attempt at this level

        /// <summary>The explorer reached the goal. True only the first time; later touches change nothing.</summary>
        public bool ReachGoal() // Clears the level
        {
            if (Outcome != RunOutcome.Playing) return false; // Already over
            Outcome = RunOutcome.Cleared; // The level is cleared
            Stars = StarRule.StarsFor(Collected, Level.Pickups.Count); // Stars from the items collected
            return true; // This touch counted
        }

        /// <summary>Saves a clear into the progress (a worse replay keeps the best, D-045). An unfinished attempt saves nothing.</summary>
        public void RecordResult(PlayerProgress progress) // Writes the result into the progress
        {
            if (Outcome != RunOutcome.Cleared) return; // Nothing to save yet
            progress.RecordClear(Level.Episode, Level.Difficulty, Level.Number, Stars); // Keep the clear and its stars
        }
    }
}
