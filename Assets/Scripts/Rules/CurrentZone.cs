using System; // Gives the footing test delegate

namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public sealed class CurrentZone // A strip of water flowing over flat ground
    { // Opens the strip
        public const float Speed = 30f; // Push in pixels per second
        public float X { get; } // Left edge of the strip
        public float Width { get; } // Width in pixels
        public int Direction { get; } // Right is one and left is minus one
        readonly Func<float, bool> inStrip; // Keeps the footing test for repeated checks

        public CurrentZone(float x, float width, int direction) // Places the strip
        { // Opens the placement
            X = x; // Saves the left edge
            Width = width; // Saves the width
            Direction = direction; // Saves the way the water flows
            inStrip = Contains; // Reuses the same edge test each frame
        } // Ends the placement

        public bool Contains(float x) => x >= X && x <= X + Width; // Includes both edges of the strip

        public float PushAt(bool grounded, bool onSolid, float feetY, float feetX) // Gets the water's push under the explorer
            => ZoneFooting.IsStandingIn(grounded, onSolid, feetY, feetX, inStrip) ? Direction * Speed : 0f; // Pushes only on flat ground in the water
    } // Ends the strip
} // Ends the rules
