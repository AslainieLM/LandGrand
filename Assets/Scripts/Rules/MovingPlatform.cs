namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public sealed class MovingPlatform // One placed level object
    { // Opens the object
        public const float Width = 48f; // Width of one platform
        public float X0 { get; } // Keeps the layout value
        public float X1 { get; } // Keeps the layout value
        public float Top { get; } // Keeps the layout value
        public float HighTop { get; } // Keeps the top at the end of the climb
        public float Phase { get; } // Keeps the start offset in seconds
        public bool IsVertical => HighTop > Top; // A basket goes higher than its start
        public const float BalloonMinRise = 48f; // Shortest basket climb in the layouts
        public const float BalloonMaxRise = 96f; // Longest basket climb in the layouts

        public MovingPlatform(float x0, float x1, float top) : this(x0, x1, top, top, 0f) { } // Keeps the original horizontal route

        MovingPlatform(float x0, float x1, float top, float highTop, float phase) // Sets the ends of the route
        { // Opens the placement
            X0 = x0; // Saves the start x
            X1 = x1; // Saves the end x
            Top = top; // Saves the low top
            HighTop = highTop; // Saves the high top
            Phase = phase; // Saves the offset in seconds
        } // Ends the placement

        public static MovingPlatform Vertical(float x, float lowTop, float highTop, float phase) // Places a basket that stays at the same x
            => new MovingPlatform(x, x, lowTop, highTop, phase); // Uses both basket heights

        public const float Speed = 40f; // Travel speed in pixels per second
        public float LeftAt(float seconds) // Reads the moving left edge
        { // Starts the travel check
            return X0 + OffsetAt(seconds * Speed, X1 - X0); // Uses the original sideways trip
        } // Ends the travel check

        public float TopAt(float seconds) => Top + OffsetAt(seconds * Speed + Phase * Speed, HighTop - Top); // The basket rises and falls while the slab stays level

        public bool IsRisingAt(float seconds) => IsVertical && TravelAt(seconds * Speed + Phase * Speed, HighTop - Top) < HighTop - Top; // True on the way up

        static float OffsetAt(float pixels, float distance) // Gets the offset along either axis
        { // Opens the offset check
            if (distance <= 0f) return 0f; // A fixed axis stays still
            float travel = TravelAt(pixels, distance); // Wraps each round trip
            return travel <= distance ? travel : distance * 2f - travel; // Goes out then returns
        } // Ends the offset check

        static float TravelAt(float pixels, float distance) => (pixels % (distance * 2f) + distance * 2f) % (distance * 2f); // Wraps each round trip
    } // Ends the object
} // Ends the rules
