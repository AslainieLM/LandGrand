using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Moves an object up and down by whole pixels, for the floating title.</summary>
    public class PixelBob : MonoBehaviour
    {
        public float periodSeconds = 2.4f; // Seconds for one full up-and-down
        public int amplitudePixels = 1; // How many pixels it moves up and down
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit

        Vector3 basePosition; // Where it rests

        void Start() => basePosition = transform.localPosition; // Remember the rest position

        void Update() // Runs every frame
        {
            float wave = Mathf.Sin(Time.time * Mathf.PI * 2f / periodSeconds); // Smooth up-and-down value from -1 to 1
            float pixels = Mathf.Round(wave * amplitudePixels); // Turn it into whole pixels
            transform.localPosition = basePosition + new Vector3(0f, pixels / pixelsPerUnit, 0f); // Move to the rest position plus that offset
        }
    }
}
