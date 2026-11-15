namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public sealed class MovingPlatform // One placed level object
    { // Opens the object
        public const float Width = 48f; // Width of one platform
        public float X0 { get; } // Keeps the layout value
        public float X1 { get; } // Keeps the layout value
        public float Top { get; } // Keeps the layout value
        public MovingPlatform(float x0, float x1, float top) { X0 = x0; X1 = x1; Top = top; } // Keeps the placement
        public const float Speed = 40f; // Travel speed in pixels per second
        public float LeftAt(float seconds) // Reads the moving left edge
        { // Starts the travel check
            float distance = X1 - X0; // Length of one crossing
            if (distance <= 0f) return X0; // A fixed platform stays still
            float travel = ((seconds * Speed) % (distance * 2f) + distance * 2f) % (distance * 2f); // Wraps each round trip
            return X0 + (travel <= distance ? travel : distance * 2f - travel); // Goes out then returns
        } // Ends the travel check
    } // Ends the object
} // Ends the rules
