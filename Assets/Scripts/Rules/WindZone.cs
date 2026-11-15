namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public enum WindPhase { Calm, Warning, Push } // Gust cycle stages
    public sealed class WindZone // One placed level object
    { // Opens the object
        public float X { get; } // Keeps the layout value
        public float Width { get; } // Keeps the layout value
        public float Offset { get; } // Keeps the layout value
        public WindZone(float x, float width, float offset = 0f) { X = x; Width = width; Offset = offset; } // Keeps the placement
        public const float CycleSeconds = 4f; // Full gust cycle
        public const float WarnAt = 2f; // Streaks warn first
        public const float PushStartsAt = 2.6f; // Push begins after the warning
        public const float PushSpeed = 40f; // Leftward push speed
        public WindPhase PhaseAt(float seconds) // Reads the shifted cycle
        { // Starts the phase check
            float t = (seconds + Offset) % CycleSeconds; // Time within this cycle
            if (t < 0f) t += CycleSeconds; // Keeps earlier times positive
            if (t < WarnAt) return WindPhase.Calm; // No streaks or push
            return t < PushStartsAt ? WindPhase.Warning : WindPhase.Push; // Warns then pushes
        } // Ends the phase check
        public float PushAt(float feetX, float seconds) => feetX >= X && feetX <= X + Width && PhaseAt(seconds) == WindPhase.Push ? -PushSpeed : 0f; // Pushes left only inside the zone
    } // Ends the object
} // Ends the rules
