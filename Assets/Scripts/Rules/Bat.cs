using System; // Gives the ellipse math

namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public sealed class Bat // One gentle bat on an ellipse
    { // Opens the bat
        public const float Width = 16f; // Width of the bat's touch box
        public const float Height = 12f; // Height of the bat's touch box
        public const float PushDistance = 24f; // Pixels the explorer is pushed
        public const float PushSeconds = 0.2f; // Time the push lasts
        public const float PushSpeed = PushDistance / PushSeconds; // Pixels per second during the push
        public const float RestSeconds = 1f; // Time before the bat can push again
        public const float ExplorerWidth = 12f; // Width of the explorer's body
        public const float ExplorerHeight = 26f; // Height of the explorer's body

        public Spot Center { get; } // Center of the loop
        public float RadiusX { get; } // Sideways distance from the center
        public float RadiusY { get; } // Upward distance from the center
        public float Period { get; } // Seconds per loop
        public float Phase { get; } // Seconds to advance the first loop

        public Bat(float x, float y, float radiusX, float radiusY, float period, float phase) // Places the loop
        { // Opens the placement
            Center = new Spot(x, y); // Saves the center
            RadiusX = radiusX; // Saves the sideways radius
            RadiusY = radiusY; // Saves the vertical radius
            Period = period; // Saves the loop time
            Phase = phase; // Saves the start offset
        } // Ends the placement

        public Spot PositionAt(float seconds) // Gets the bat's center at this time
        { // Opens the ellipse calculation
            double angle = 2 * Math.PI * ((seconds + Phase) % Period) / Period; // Turns elapsed time into an angle
            return new Spot(Center.X + RadiusX * (float)Math.Cos(angle), Center.Y + RadiusY * (float)Math.Sin(angle)); // Reads both axes of the ellipse
        } // Ends the ellipse calculation

        public static bool Touches(Spot feet, Spot bat) // Checks the explorer's body against the bat
        { // Opens the touch check
            return Math.Abs(feet.X - bat.X) < (ExplorerWidth + Width) / 2f // The bodies overlap sideways
                && feet.Y < bat.Y + Height / 2f // The feet are below the bat's top
                && feet.Y + ExplorerHeight > bat.Y - Height / 2f; // The head is above the bat's bottom
        } // Ends the touch check

        public static int PushDirection(float explorerX, float batX, int walk) // Finds which way the explorer moves
        { // Opens the direction check
            if (explorerX < batX) return -1; // Pushes left away from the bat
            if (explorerX > batX) return 1; // Pushes right away from the bat
            return walk < 0 ? 1 : -1; // Pushes against the walk or left when idle
        } // Ends the direction check

        public static bool CanPush(float secondsSincePush) => secondsSincePush >= RestSeconds; // Contact only counts after the rest
    } // Ends the bat
} // Ends the rules
