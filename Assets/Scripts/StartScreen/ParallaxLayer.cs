using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Scrolls a horizontally tiling background layer to the left forever.
    /// The first child holds the layer sprite; a second copy is made at start so the seam never shows.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        /// <summary>
        /// Shared multiplier for every layer that follows the explorer's walk.
        /// 1 = walking, 0 = the explorer has stopped (for example, to pick up a mineral).
        /// </summary>
        public static float WalkSpeedScale = 1f; // Shared speed for all layers that follow the walk

        public float speedPixelsPerSecond; // How fast this layer scrolls
        [Tooltip("Off for layers that drift on their own, such as clouds.")]
        public bool followsWalk = true; // Stop with the explorer, or keep drifting
        public float widthPixels = 960f; // Width of the layer art
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit

        /// <summary>How far the layer has scrolled, in canvas pixels, from 0 up to widthPixels.</summary>
        public float OffsetPixels { get; private set; } // How far it has scrolled

        Transform first; // The layer picture
        Transform second; // A copy placed right after it

        void Start() // Runs once before the first frame
        {
            first = transform.GetChild(0); // Find the layer picture
            second = Instantiate(first.gameObject, transform).transform; // Copy it
            second.name = first.name + " (wrap copy)"; // Give the copy a clear name
            Apply(); // Put both in place
        }

        void Update() // Runs every frame
        {
            float scale = followsWalk ? WalkSpeedScale : 1f; // Walk speed, or full speed for clouds
            OffsetPixels = Mathf.Repeat(OffsetPixels + speedPixelsPerSecond * scale * Time.deltaTime, widthPixels); // Scroll a little, wrapping back after one full width
            Apply(); // Move the pictures
        }

        void Apply() // Puts both pictures at the current scroll
        {
            // Round to whole pixels so pixel art never shimmers between pixels.
            float x = -Mathf.Round(OffsetPixels) / pixelsPerUnit; // Left edge of the first picture
            float width = widthPixels / pixelsPerUnit; // Layer width in world units
            first.localPosition = new Vector3(x, 0f, 0f); // Place the first picture
            second.localPosition = new Vector3(x + width, 0f, 0f); // Place the copy right after it
        }

        /// <summary>World x of a canvas x coordinate, wrapped to the copy nearest the screen centre.</summary>
        public float CanvasToWorldX(float canvasX) // Turns a spot on the layer art into a world x
        {
            float width = widthPixels / pixelsPerUnit; // Layer width in world units
            float x = (canvasX - widthPixels / 2f - Mathf.Round(OffsetPixels)) / pixelsPerUnit; // Spot position after scrolling
            x = Mathf.Repeat(x + width / 2f, width) - width / 2f; // Use the copy closest to the screen middle
            return transform.position.x + x; // Add the layer's own position
        }
    }
}
