using UnityEngine; // Gives access to MonoBehaviour, Rect and Mathf

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// One collectible lying in the level (ticket 08). It bobs up and down a little and tells the level controller where to look for the explorer.
    /// The level builder places it where LevelData says; its position is the middle of its bottom edge, in level pixels.
    /// </summary>
    public class PickupSpot : MonoBehaviour // A rock or mineral in the level
    {
        public int index; // Its place in the level pickup list, so the rules know which one was touched
        public SpriteRenderer art; // Its picture
        public Vector2 feet; // Middle of its bottom edge, in level pixels (y up from the ground line)
        public float bobPixels = 1.5f; // How far it bobs up and down

        /// <summary>The area the explorer must touch to collect it, in level pixels. A little smaller than the picture.</summary>
        public Rect Area => new Rect(feet.x - 10f, feet.y + 2f, 20f, 20f); // 20x20 inside the 24x24 picture

        /// <summary>The middle of the picture, in level pixels. Used for the sparkle.</summary>
        public Vector2 Center => new Vector2(feet.x, feet.y + 12f); // Half of the 24-pixel height above the bottom

        void Update() // Runs every frame: bobs on whole pixels
        {
            float bob = Mathf.Round(Mathf.Sin(Time.time * 3f + feet.x) * bobPixels); // Up or down by whole pixels so the art stays sharp
            art.transform.position = LevelGeometry.ToWorld(Center.x, Center.y + bob); // Place the picture
        }

        /// <summary>Takes the pickup out of the level.</summary>
        public void Hide() => gameObject.SetActive(false); // It disappears when collected
    }
}
