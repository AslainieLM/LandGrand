namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>What a steam vent is doing: resting, showing a small warning wisp, or puffing (D-055).</summary>
    public enum VentPhase { Idle, Warning, Puff } // Vent states

    /// <summary>
    /// A steam vent set into the ground (D-055, Easy 2). It puffs on a fixed cycle; standing on it during a puff
    /// launches the explorer high. It never hurts and never touches the timer, items, respawn point or stars.
    /// </summary>
    public sealed class SteamVent // One vent
    {
        /// <summary>Seconds in one full cycle: rest, warning, puff.</summary>
        public const float CycleSeconds = 2f; // About every 2 seconds (D-055)

        /// <summary>Seconds into the cycle when the warning wisp starts.</summary>
        public const float WarningAt = 1.2f; // Rest for the first 1.2 seconds

        /// <summary>Seconds into the cycle when the puff starts; it lasts to the end of the cycle.</summary>
        public const float PuffAt = 1.55f; // A short warning, then the puff

        /// <summary>
        /// Upward speed in pixels per second that a puff gives the explorer: about 128 pixels high, far more than a double jump (about 76).
        /// High enough that an explorer holding right rises above a ledge starting 16 pixels right of the vent before passing under it.
        /// </summary>
        public const float LaunchSpeed = 480f; // Speed squared divided by twice the gravity (900) gives the height

        public float X { get; } // Left edge in level pixels
        public float Width { get; } // Width in pixels

        public SteamVent(float x, float width = 24f) { X = x; Width = width; } // Makes a vent; the picture is 24 pixels wide

        /// <summary>The middle of the vent across, in level pixels.</summary>
        public float CenterX => X + Width / 2f; // Halfway between its edges

        /// <summary>How far through the puff a time is: 0 when the puff starts, 1 when it ends. 0 outside the puff.</summary>
        public static float PuffProgress(float seconds) // Lets the picture grow through its frames
        {
            if (PhaseAt(seconds) != VentPhase.Puff) return 0f; // Not puffing
            float t = seconds % CycleSeconds; // Seconds into the cycle
            if (t < 0f) t += CycleSeconds; // Keep it positive
            return (t - PuffAt) / (CycleSeconds - PuffAt); // Share of the puff gone by
        }

        /// <summary>True when feet at this x stand on the vent.</summary>
        public bool Covers(float feetX) => feetX >= X && feetX <= X + Width; // Inside its left and right edges

        /// <summary>The phase at a time in seconds since the level opened. Every vent shares one clock, so they puff together.</summary>
        public static VentPhase PhaseAt(float seconds) // Works out the phase
        {
            float t = seconds % CycleSeconds; // Seconds into the current cycle
            if (t < 0f) t += CycleSeconds; // Keep it positive for times before zero
            if (t >= PuffAt) return VentPhase.Puff; // End of the cycle: the puff
            return t >= WarningAt ? VentPhase.Warning : VentPhase.Idle; // A wisp just before it, rest before that
        }
    }
}
