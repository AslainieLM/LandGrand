namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public enum CrumblePhase { Whole, Cracking, Gone, Back } // What the slab is doing
    public sealed class CrumbleLedge // One placed level object
    { // Opens the object
        public const float Width = 32f; // Width of one ledge
        public float X { get; } // Keeps the layout value
        public float Top { get; } // Keeps the layout value
        public CrumbleLedge(float x, float top) { X = x; Top = top; } // Keeps the placement
        public const float BreakSeconds = 0.5f; // Delay after the first touch
        public const float ReturnSeconds = 3f; // Time gone after breaking
        public static CrumblePhase StateAt(float? touchedAt, float now) // Reads the first touch
        { // Starts the phase check
            if (!touchedAt.HasValue || now < touchedAt.Value) return CrumblePhase.Whole; // Not touched yet
            float age = now - touchedAt.Value; // Time since the first touch
            if (age < BreakSeconds) return CrumblePhase.Cracking; // Shakes before breaking
            return age < BreakSeconds + ReturnSeconds ? CrumblePhase.Gone : CrumblePhase.Back; // Gone then ready again
        } // Ends the phase check
        public static bool IsSolid(float? touchedAt, float now) => StateAt(touchedAt, now) != CrumblePhase.Gone; // Only the broken slab is absent
        public static float CrackProgress(float? touchedAt, float now) => StateAt(touchedAt, now) == CrumblePhase.Cracking ? (now - touchedAt.Value) / BreakSeconds : 0f; // Share of the warning elapsed
    } // Ends the object
} // Ends the rules
