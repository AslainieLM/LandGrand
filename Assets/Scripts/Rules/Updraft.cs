using System; // Gives Math.Min and Math.Max

namespace LandGrand.Rules // Holds rules without Unity types
{ // Opens the rules
    /// <summary>A cliff updraft (D-072): a column of rising wind that lifts the explorer to a set height and holds them there.</summary>
    public sealed class Updraft // One column of rising wind
    { // Opens the updraft rule
        public const float Width = 32f; // The column width in pixels
        public const float RiseSpeed = 90f; // The rising speed in pixels per second

        public float X { get; } // The left edge of the column
        public float Top { get; } // The feet height where the explorer hovers

        public Updraft(float x, float top) // Places a column on the ground
        { // Starts the placement
            X = x; // Saves the left edge
            Top = top; // Saves the hover height
        } // Ends the placement

        public bool Contains(float middleX, float feetY) // Checks if the explorer is inside the column
        { // Starts the inside check
            return middleX >= X && middleX <= X + Width && // The middle is between the two side edges
                   feetY >= -0.5f && feetY <= Top + 0.5f; // The feet are between the ground and the hover height
        } // Ends the inside check

        public float RiseVelocity(float feetY, float dt) // The upward speed for this frame
        { // Starts the speed rule
            if (dt <= 0f) return 0f; // No time passed, so no rise
            float exact = (Top - feetY) / dt; // The speed that reaches the top in this frame
            return Math.Max(0f, Math.Min(exact, RiseSpeed)); // Never sinks and never rises faster than the limit
        } // Ends the speed rule
    } // Ends the updraft rule
} // Closes the rules
