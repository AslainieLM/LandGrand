using System; // Gives the floor function

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class RollingBoulder // Places one boulder that rolls left
    { // Opens the boulder
        public float Spawn { get; } // Where the middle starts
        public float End { get; } // Where the roll ends
        public float Offset { get; } // Shifts the cycle in seconds

        public const float CycleSeconds = 3.5f; // Time from one warning to the next
        public const float WarningSeconds = 0.8f; // Time before the roll starts
        public const float Speed = 90f; // Pixels rolled left each second
        public const float Size = 14f; // Hit box width and height
        public const float SkipDistance = 48f; // Gives the explorer space when the roll starts
        public const float MaxRun = Speed * (CycleSeconds - WarningSeconds); // Longest roll that fits

        public RollingBoulder(float spawn, float end, float offset = 0f) // Sets the roll and its timing
        { // Opens the setup
            Spawn = spawn; // Keeps the start
            End = end; // Keeps the end
            Offset = offset; // Keeps the time shift
        } // Ends the setup

        public int CycleIndex(float t) => (int)Math.Floor((t + Offset) / CycleSeconds); // Counts full shifted cycles
        public float Phase(float t) => t - (CycleIndex(t) * CycleSeconds - Offset); // Gives time inside the cycle
        float RollStart(float t) => CycleIndex(t) * CycleSeconds + WarningSeconds - Offset; // Finds the roll start on the level clock
        public bool IsWarning(float t) => t < RollStart(t); // Dust warns before the roll starts
        public float XAt(float t) // Gives the middle while rolling
        { // Starts the position check
            float elapsed = t - RollStart(t); // Time since the roll began
            if (elapsed < 0f) return float.NaN; // The boulder is still warning
            float x = Spawn - elapsed * Speed; // Moves left along the ground
            return x >= End ? x : float.NaN; // Goes away after the end
        } // Ends the position check

        public bool Hits(float left, float bottom, float width, float height, float t) => HitsAt(XAt(t), left, bottom, width, height); // Checks the roll without a skip
        internal static bool HitsAt(float x, float left, float bottom, float width, float height) // Measures the overlap at this position
        { // Starts the hit check
            return left < x + Size / 2f && left + width > x - Size / 2f && bottom < Size && bottom + height > 0f; // Both boxes overlap; NaN cannot touch
        } // Ends the hit check
    } // Ends the boulder
} // Ends the rules
