namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public enum RockfallPhase { Warning, Falling, Landed, Clear } // Rock cycle stages
    public sealed class Rockfall // One placed level object
    { // Opens the object
        public float X { get; } // Keeps the layout value
        public float Offset { get; } // Keeps the layout value
        public Rockfall(float x, float offset = 0f) { X = x; Offset = offset; } // Keeps the placement
        public const float CycleSeconds = 3f; // Full cycle length
        public const float FallStart = 1.2f; // Warning ends
        public const float Land = 1.5f; // Rock reaches the ground
        public const float Gone = 1.65f; // Rock stops hurting
        public const float Size = 16f; // Rock width and height
        public float CycleTime(float seconds) // Wraps the shifted time
        { // Starts the time check
            float t = (seconds + Offset) % CycleSeconds; // Time within this cycle
            return t < 0f ? t + CycleSeconds : t; // Keeps earlier times positive
        } // Ends the time check
        public RockfallPhase PhaseAt(float seconds) // Reads the phase
        { // Starts the phase check
            float t = CycleTime(seconds); // Time in this cycle
            if (t < FallStart) return RockfallPhase.Warning; // Shadow and pebbles
            if (t < Land) return RockfallPhase.Falling; // Falling from above
            return t < Gone ? RockfallPhase.Landed : RockfallPhase.Clear; // Grounded then gone
        } // Ends the phase check
        public float? RockBottomAt(float seconds) // Gives the bottom of the rock
        { // Starts the height check
            var phase = PhaseAt(seconds); // What the rock is doing
            if (phase == RockfallPhase.Falling) return 270f * (Land - CycleTime(seconds)) / (Land - FallStart); // Falls to ground
            return phase == RockfallPhase.Landed ? (float?)0f : null; // No height while absent
        } // Ends the height check
        public bool Hits(float bodyLeft, float bodyBottom, float bodyWidth, float bodyHeight, float seconds) // Checks the moving hit box
        { // Starts the hit check
            float? bottom = RockBottomAt(seconds); // Only active rocks have a height
            return bottom.HasValue && bodyLeft < X + Size / 2f && bodyLeft + bodyWidth > X - Size / 2f && bodyBottom < bottom.Value + Size && bodyBottom + bodyHeight > bottom.Value; // Boxes overlap
        } // Ends the hit check
    } // Ends the object
} // Ends the rules
