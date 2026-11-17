namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class MudZone // Places a stretch of slippery mud
    { // Opens the mud zone
        public float X { get; } // Left edge in level pixels
        public float Width { get; } // Width in level pixels

        public MudZone(float x, float width) // Sets both edges of the mud
        { // Opens the setup
            X = x; // Keeps the left edge
            Width = width; // Keeps the width
        } // Ends the setup

        public bool Contains(float x) => x >= X && x <= X + Width; // Both edges count as mud
    } // Ends the mud zone
} // Ends the rules