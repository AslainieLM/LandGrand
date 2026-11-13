using System.Collections.Generic; // Gives IReadOnlyList and IEnumerable

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>How an attempt at a level stands. Time up ends it too (ticket 10). Pause and the pop-up are IsPaused on the run.</summary>
    public enum RunOutcome { Playing, Cleared, TimeUp } // Attempt states

    /// <summary>What happened when the explorer touched a pickup (ticket 08).</summary>
    public enum CollectResult { None, FirstFind, Repeat } // Nothing new; first find of a kind (pop-up); a known kind (chime and sparkle)

    /// <summary>One goal line of the level and whether the attempt met it (ticket 11).</summary>
    public readonly struct GoalResult // A goal with its tick
    {
        public readonly string Text; // For example "Collect 5/5"
        public readonly bool Met; // True gets a tick on the Result Screen

        public GoalResult(string text, bool met) { Text = text; Met = met; } // Makes a goal result
    }

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

        /// <summary>Where the explorer appears: the checkpoint once it was reached, otherwise the level start.</summary>
        public Spot RespawnPoint => CheckpointReached ? Level.Checkpoint : Level.Start; // The last checkpoint reached (D-040)

        /// <summary>True once the explorer touched the checkpoint flag.</summary>
        public bool CheckpointReached { get; private set; } // Shown as a raised flag

        /// <summary>How many times a hazard sent the explorer back in this attempt.</summary>
        public int Respawns { get; private set; } // For the result later

        /// <summary>Items collected so far in this attempt.</summary>
        public int Collected { get; private set; } // Counts each pickup once

        /// <summary>The kind whose "You found an item!" pop-up is open, or null when no pop-up is open.</summary>
        public ItemKind PendingFind { get; private set; } // Set by a first find, cleared by OK

        /// <summary>True while the pause menu is open (ticket 10).</summary>
        public bool IsMenuOpen { get; private set; } // Set by Pause, cleared by Resume

        /// <summary>True while the game waits: the first-find pop-up or the pause menu is open. The clock stops too.</summary>
        public bool IsPaused => PendingFind != null || IsMenuOpen; // Either one stops the game

        /// <summary>Seconds left on the countdown. Starts at the level time limit and never goes below 0.</summary>
        public float RemainingSeconds { get; private set; } // Counts down while playing

        /// <summary>True once the countdown reached zero.</summary>
        public bool IsTimeUp => Outcome == RunOutcome.TimeUp; // Time ran out

        readonly bool[] taken; // Which pickups were taken, by their place in the level's list

        /// <summary>Stars earned: 0 while playing, 1 to 3 once cleared (D-039).</summary>
        public int Stars { get; private set; } // Set when the goal is reached

        public LevelRun(LevelData level) { Level = level; taken = new bool[level.Pickups.Count]; RemainingSeconds = level.TimeLimitSeconds; } // Starts an attempt at this level

        /// <summary>For each pickup of the level, in order: was it taken? The Result Screen shows missed ones as grey outlines.</summary>
        public IReadOnlyList<bool> TakenFlags => (bool[])taken.Clone(); // A copy, so nobody can change the attempt

        /// <summary>The level goals with a tick each: collect everything, and finish before time runs out (reaching the goal).</summary>
        public IEnumerable<GoalResult> GoalResults // Built when read
        {
            get // The goal lines of the level, in order
            {
                int i = 0; // Which goal line this is
                foreach (var text in Level.Goals) // Each goal line
                {
                    bool met = i == 0 ? Collected == Level.Pickups.Count : Outcome == RunOutcome.Cleared; // First: everything collected. Second: the goal was reached
                    yield return new GoalResult(text, met); // The line with its tick
                    i++; // Next line
                }
            }
        }

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

        /// <summary>The explorer touched the checkpoint flag. True only the first time, and only while playing.</summary>
        public bool ReachCheckpoint() // Marks the flag as reached
        {
            if (Outcome != RunOutcome.Playing || CheckpointReached) return false; // Over, or already reached
            CheckpointReached = true; // The respawn point moves to the flag
            return true; // This touch counted
        }

        /// <summary>True when a fall into a pit only returns the explorer a short way back instead of to the checkpoint. Easy levels only.</summary>
        public bool PitReturnsNearby => Level.Difficulty == Difficulty.Easy; // The kinder rule for Easy (changes D-040 for pits)

        /// <summary>
        /// The explorer fell into a pit. On Easy it goes back to the last safe spot near the pit edge (lastSafe); on other difficulties
        /// it goes to the last checkpoint or the start. Items, the outcome and the timer are not touched.
        /// </summary>
        public Spot FallIntoPit(Spot lastSafe) // Counts the fall and gives the place to return to
        {
            if (Outcome == RunOutcome.Playing) Respawns++; // Only while playing
            return PitReturnsNearby ? lastSafe : RespawnPoint; // Easy: a short way back. Otherwise: the checkpoint or the start
        }

        /// <summary>
        /// The explorer touched a trap. Returns where to put it back: the last checkpoint reached, or the start.
        /// Collected items, the outcome and the timer are not touched (D-040: no health bar).
        /// </summary>
        public Spot HitHazard() // Counts the hazard and gives the respawn place
        {
            if (Outcome == RunOutcome.Playing) Respawns++; // Only while playing
            return RespawnPoint; // The last checkpoint or the start
        }

        /// <summary>Lets time pass. It counts down only while playing, not while paused or behind the pop-up. Reaching zero is time up.</summary>
        public void Tick(float seconds) // Called every frame with the frame time
        {
            if (Outcome != RunOutcome.Playing || IsPaused || seconds <= 0f) return; // The clock stops when over or paused
            RemainingSeconds -= seconds; // One step of the countdown
            if (RemainingSeconds > 0f) return; // Still time left
            RemainingSeconds = 0f; // Never below zero
            Outcome = RunOutcome.TimeUp; // Time is up
        }

        /// <summary>Opens the pause menu. False when the attempt is over or the pop-up is open.</summary>
        public bool Pause() // The pause button or Android Back
        {
            if (Outcome != RunOutcome.Playing || IsPaused) return false; // Not now
            IsMenuOpen = true; // Menu open: the game waits
            return true; // It opened
        }

        /// <summary>Closes the pause menu.</summary>
        public void Resume() => IsMenuOpen = false; // The game goes on

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
