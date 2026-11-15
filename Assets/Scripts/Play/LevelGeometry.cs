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
        public DynamicSolid[] dynamicSolids = new DynamicSolid[0]; // Solids that move or disappear
        public System.Collections.Generic.IEnumerable<Rect> LedgeShapes // Reads every top that holds the explorer only from above
        { // Opens the shape list
            get // Builds the current list
            { // Opens the list
                foreach (var ledge in ledges) yield return ledge; // Fixed ledges
                foreach (var slab in dynamicSolids) if (slab != null && slab.IsSolid) yield return slab.Bounds; // Moving and breakable slabs act like ledges
            } // Ends the list
        } // Ends the shape list
        public DynamicSolid SupportAt(Vector2 feet, float bodyWidth) // Finds a dynamic slab under the feet
        { // Starts the support check
            foreach (var solid in dynamicSolids) // Checks each placed slab
            { // Opens the slab check
                if (solid == null || !solid.IsSolid) continue; // Gone slabs cannot support
                var bounds = solid.Bounds; // Current pixel shape
                if (feet.x + bodyWidth / 2f > bounds.xMin && feet.x - bodyWidth / 2f < bounds.xMax && Mathf.Abs(feet.y - bounds.yMax) < 0.02f) return solid; // Feet rest on its top
            } // Ends the slab check
            return null; // Standing on static ground or in the air
        } // Ends the support check
        public float width = 4800f; // Level width in pixels

        /// <summary>True when something solid or a ledge top holds up a body standing at this feet spot.</summary>
        public bool HasGroundAt(float x, float y) // Used to pick a safe place to come back to
        {
            foreach (var r in solids) if (x > r.xMin && x < r.xMax && Mathf.Abs(r.yMax - y) < 0.5f) return true; // Standing on a solid top
            foreach (var r in LedgeShapes) if (x > r.xMin && x < r.xMax && Mathf.Abs(r.yMax - y) < 0.5f) return true; // Standing on a ledge top
            return false; // Nothing under the feet
        }

        /// <summary>A level pixel position as a world position.</summary>
        public static Vector3 ToWorld(float x, float y) => new Vector3(x / PixelsPerUnit, y / PixelsPerUnit, 0f); // 32 pixels make one unit

        /// <summary>A world position as a level pixel position.</summary>
        public static Vector2 ToPixels(Vector3 world) => new Vector2(world.x * PixelsPerUnit, world.y * PixelsPerUnit); // One unit is 32 pixels
    }
}
