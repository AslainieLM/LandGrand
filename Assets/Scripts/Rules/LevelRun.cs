namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>How an attempt at a level stands. A later ticket adds time up. The pop-up pause is IsPaused on the run.</summary>
    public enum RunOutcome { Playing, Cleared } // Attempt states

    /// <summary>What happened when the explorer touched a pickup (ticket 08).</summary>
    public enum CollectResult { None, FirstFind, Repeat } // Nothing new; first find of a kind (pop-up); a known kind (chime and sparkle)

    /// <summary>
    /// One attempt at a level (ticket 07): it starts at the level's start and is cleared when the explorer reaches the goal.
    /// Hazards and the checkpoint (ticket 09) and the timer (ticket 10) build on it.
    /// </summary>
    public sealed class LevelRun // The state of one attempt
    {
        /// <summary>The level being played.</summary>
        public LevelData Level { get; } // Never changes during the attempt

        /// <summary>Playing, or cleared once the goal is reached.</summary>
        public RunOutcome Outcome { get; private set; } = RunOutcome.Playing; // Every attempt starts by playing

        /// <summary>Where the explorer appears: the level's start.</summary>
        public Spot RespawnPoint => Level.Start; // Ticket 09 moves it to the checkpoint

        /// <summary>Items collected so far in this attempt.</summary>
        public int Collected { get; private set; } // Counts each pickup once

        /// <summary>The kind whose "You found an item!" pop-up is open, or null when no pop-up is open.</summary>
        public ItemKind PendingFind { get; private set; } // Set by a first find, cleared by OK

        /// <summary>True while the pop-up is open: the game waits for OK.</summary>
        public bool IsPaused => PendingFind != null; // The timer in ticket 10 stops here too

        readonly bool[] taken; // Which pickups were taken, by their place in the level's list

        /// <summary>Stars earned: 0 while playing, 1 to 3 once cleared (D-039).</summary>
        public int Stars { get; private set; } // Set when the goal is reached

        public LevelRun(LevelData level) { Level = level; taken = new bool[level.Pickups.Count]; } // Starts an attempt at this level

        /// <summary>True when that pickup was already taken in this attempt.</summary>
        public bool IsCollected(int index) => index >= 0 && index < taken.Length && taken[index]; // False for numbers outside the list

        /// <summary>
        /// The explorer touched a pickup. It counts once. A kind never found before is saved as found and asks for the pop-up (FirstFind);
        /// a kind already found, in this level or earlier, only chimes (Repeat).
        /// </summary>
        public CollectResult Collect(int index, PlayerProgress progress) // Takes a pickup
        {
            if (Outcome != RunOutcome.Playing || IsPaused || index < 0 || index >= taken.Length || taken[index]) return CollectResult.None; // Over, waiting for OK, a wrong number, or taken already
            taken[index] = true; // Mark it taken
            Collected++; // Count it
            var kind = Level.Pickups[index].Kind; // What it is
            if (progress.HasFoundKind(kind.Id)) return CollectResult.Repeat; // Known kind: chime and sparkle only
            progress.RecordFoundKind(kind.Id); // Remember the find for every later attempt
            PendingFind = kind; // Open the pop-up
            return CollectResult.FirstFind; // Ask for the pop-up
        }

        /// <summary>The player pressed OK on the pop-up.</summary>
        public void DismissFind() => PendingFind = null; // The game goes on

        /// <summary>The explorer reached the goal. True only the first time; later touches change nothing.</summary>
        public bool ReachGoal() // Clears the level
        {
            if (Outcome != RunOutcome.Playing || IsPaused) return false; // Already over, or a pop-up is still open
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
