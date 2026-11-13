using UnityEngine; // Gives access to MonoBehaviour and Mathf

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>Flies an object (the eagle) round and round on a flat oval, like a bird circling high above the peaks.</summary>
    public class CircleFlight : MonoBehaviour // Moves the eagle
    {
        public Vector2 centerPixels = new Vector2(150f, 52f); // Middle of the circle, in map pixels
        public Vector2 radiusPixels = new Vector2(44f, 12f); // Half width and half height of the oval
        public float secondsPerLap = 10f; // Time for one full circle

        void Update() // Runs every frame: puts the eagle at its place on the oval
        {
            float angle = Time.time * Mathf.PI * 2f / secondsPerLap; // How far round the circle it is now
            float x = centerPixels.x + Mathf.Cos(angle) * radiusPixels.x; // Across position
            float y = centerPixels.y + Mathf.Sin(angle) * radiusPixels.y; // Up-down position
            transform.localPosition = MapPixels.ToWorld(x, y); // Show it on a whole pixel
        }
    }
}
