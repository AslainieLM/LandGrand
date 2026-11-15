namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class IceZone // Places a stretch of icy ground
    { // Opens the ice zone
        public float X { get; } // Left edge in level pixels
        public float Width { get; } // Width in level pixels

        public IceZone(float x, float width) // Sets both edges of the ice
        { // Opens the setup
            X = x; // Keeps the left edge
            Width = width; // Keeps the width
        } // Ends the setup
        public bool Contains(float x) => x >= X && x <= X + Width; // Both edges count as ice
    } // Ends the ice zone
} // Ends the rules
