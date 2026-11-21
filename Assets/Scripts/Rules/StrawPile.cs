using System; // Gives Math.Abs

namespace LandGrand.Rules // Holds rules without Unity types
{ // Opens the rules
    public readonly struct StrawPile // A solid pile with a springy top
    { // Opens the pile rule
        public const float Width = 48f; // The picture width in pixels
        public const float Height = 16f; // The picture height in pixels
        public const float LaunchSpeed = 400f; // The upward speed in pixels per second
        public const float SquashSeconds = 0.15f; // Time to show the squash picture
        public float X { get; } // The left edge of the picture

        public StrawPile(float x) => X = x; // Places the pile on the ground

        public bool IsOnTop(float explorerMiddleX, float explorerHalfWidth, float feetY) // Checks for contact across the top
        { // Starts the contact check
            return explorerMiddleX + explorerHalfWidth > X + 1f && // The explorer reaches past the left edge
                   explorerMiddleX - explorerHalfWidth < X + Width - 1f && // The explorer reaches past the right edge
                   Math.Abs(feetY - (Height - 1f)) <= 0.5f; // Feet are within half a pixel of the box top
        } // Ends the contact check
    } // Ends the pile rule
} // Closes the rules
