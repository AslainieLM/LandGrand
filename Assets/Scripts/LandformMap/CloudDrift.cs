using UnityEngine; // Gives access to MonoBehaviour, SpriteRenderer and Mathf

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>A cloud that drifts slowly to the right across one region, hiding near the end and showing again after the start.</summary>
    [RequireComponent(typeof(SpriteRenderer))] // This script needs a SpriteRenderer on the same object
    public class CloudDrift : MonoBehaviour // Moves one cloud
    {
        public float startX = 30f; // Where the cloud starts, in map pixels
        public float endX = 206f; // Where the cloud ends, in map pixels
        public float y = 36f; // Height of the cloud, in map pixels from the top
        public float speedPixelsPerSecond = 4f; // How fast it drifts
        public float fadePixels = 16f; // How close to either end the cloud stays hidden

        SpriteRenderer sr; // Draws the cloud
        float x; // Where the cloud is now, in map pixels

        void Awake() // Runs once
        {
            sr = GetComponent<SpriteRenderer>(); // Find the cloud picture
            x = Mathf.Lerp(startX, endX, 0.3f); // Start a little way along, already visible
        }

        void Update() // Runs every frame: moves and fades the cloud
        {
            x += speedPixelsPerSecond * Time.deltaTime; // Drift right
            if (x > endX) x = startX; // At the end, start again at the beginning
            float edge = Mathf.Min(x - startX, endX - x); // Distance to the nearer end
            sr.enabled = edge >= fadePixels; // Hidden near the ends; no see-through steps, so every pixel stays a palette colour
            transform.localPosition = MapPixels.ToWorld(x, y); // Show it on a whole pixel
        }
    }
}
