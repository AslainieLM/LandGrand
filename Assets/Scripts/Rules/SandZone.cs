namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class SandZone // Places a stretch of soft sand
    { // Opens the sand zone
        public float X { get; } // Left edge in level pixels
        public float Width { get; } // Width in level pixels

        public SandZone(float x, float width) // Sets both edges of the sand
        { // Opens the setup
            X = x; // Keeps the left edge
            Width = width; // Keeps the width
        } // Ends the setup
        public bool Contains(float px) => px >= X && px <= X + Width; // Both edges count as sand, as the spec says (same name as IceZone)
    } // Ends the sand zone
} // Ends the rules
