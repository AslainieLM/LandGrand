using System; // Gives the range error

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class RollingLog // Places one log that can roll either way
    { // Opens the log
        public float Spawn { get; } // Where the middle starts
        public float End { get; } // Where the roll ends
        public int Direction { get; } // One goes right and minus one goes left
        public float Offset => LeftRoll.Offset; // Shifts the cycle in seconds

        public const float WarningSeconds = RollingBoulder.WarningSeconds; // Keeps the same warning time
        public const float Size = RollingBoulder.Size; // Keeps the same hit box
        public const float MaxRun = RollingBoulder.MaxRun; // Fits the roll after the warning
        internal RollingBoulder LeftRoll { get; } // Shares the boulder rule in left-facing coordinates

        public RollingLog(float spawn, float end, int direction, float offset = 0f) // Sets the route and timing
        { // Opens the setup
            if (direction != -1 && direction != 1) throw new ArgumentOutOfRangeException(nameof(direction)); // Requires a left or right roll
            float run = (end - spawn) * direction; // Measures the run in the chosen direction
            if (!(run > 0f && run <= MaxRun)) throw new ArgumentOutOfRangeException(nameof(end)); // The end must follow the spawn within the cycle
            Spawn = spawn; // Keeps the start
            End = end; // Keeps the end
            Direction = direction; // Keeps the direction
            LeftRoll = new RollingBoulder(-direction * spawn, -direction * end, offset); // Mirrors a right roll so the old rule can run it
        } // Ends the setup

        public float Phase(float time) => LeftRoll.Phase(time); // Gives the time inside the cycle
        public bool IsWarning(float time) => LeftRoll.IsWarning(time); // Warns before each roll
    } // Ends the log

    public sealed class RollingLogRun // Tracks one log's skip decision during a run
    { // Opens the attempt state
        readonly RollingLog log; // Keeps the placed log
        readonly RollingBoulderRun run; // Reuses the boulder's cycle and skip rule

        public RollingLogRun(RollingLog log) // Starts one log attempt
        { // Opens the setup
            this.log = log; // Keeps the direction
            run = new RollingBoulderRun(log.LeftRoll); // Starts the rule in left-facing coordinates
        } // Ends the setup

        public void Update(float time, float explorerX) => run.Update(time, -log.Direction * explorerX); // Checks the start in the same coordinates
        public float CurrentX(float time) => -log.Direction * run.CurrentX(time); // Gives the middle in level coordinates
        public bool Hits(float left, float bottom, float width, float height, float time) => RollingBoulder.HitsAt(CurrentX(time), left, bottom, width, height); // Uses the same box as a boulder
    } // Ends the attempt state
} // Ends the rules