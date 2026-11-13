using UnityEngine; // Gives access to MonoBehaviour, Rect and Vector3

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The level's solid shapes in level pixels (x right from the left edge, y up from the ground line).
    /// Solids block from every side (ground, boulders, walls). Ledges only hold the explorer up from above,
    /// so the explorer can jump up through them from below.
    /// </summary>
    public class LevelGeometry : MonoBehaviour // The level's shapes
    {
        public const float PixelsPerUnit = 32f; // Same as every sprite in the game

        public Rect[] solids = new Rect[0]; // Ground and boulders (the explorer is kept inside the level width by the motor)
        public Rect[] ledges = new Rect[0]; // Ledges the explorer can stand on and jump up through
        public float width = 4800f; // Level width in pixels

        /// <summary>True when something solid or a ledge top holds up a body standing at this feet spot.</summary>
        public bool HasGroundAt(float x, float y) // Used to pick a safe place to come back to
        {
            foreach (var r in solids) if (x > r.xMin && x < r.xMax && Mathf.Abs(r.yMax - y) < 0.5f) return true; // Standing on a solid top
            foreach (var r in ledges) if (x > r.xMin && x < r.xMax && Mathf.Abs(r.yMax - y) < 0.5f) return true; // Standing on a ledge top
            return false; // Nothing under the feet
        }

        /// <summary>A level pixel position as a world position.</summary>
        public static Vector3 ToWorld(float x, float y) => new Vector3(x / PixelsPerUnit, y / PixelsPerUnit, 0f); // 32 pixels make one unit

        /// <summary>A world position as a level pixel position.</summary>
        public static Vector2 ToPixels(Vector3 world) => new Vector2(world.x * PixelsPerUnit, world.y * PixelsPerUnit); // One unit is 32 pixels
    }
}
