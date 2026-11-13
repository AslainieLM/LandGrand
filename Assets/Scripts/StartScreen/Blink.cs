using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Turns a sprite on and off, for the "Tap to Start" prompt.</summary>
    [RequireComponent(typeof(SpriteRenderer))] // This script needs a SpriteRenderer on the same object
    public class Blink : MonoBehaviour
    {
        public float onSeconds = 0.6f; // How long the sprite stays visible
        public float offSeconds = 0.4f; // How long the sprite stays hidden

        SpriteRenderer spriteRenderer; // The sprite to show and hide

        void Awake() => spriteRenderer = GetComponent<SpriteRenderer>(); // Find the sprite once at the start

        void Update() // Runs every frame
        {
            float t = Mathf.Repeat(Time.time, onSeconds + offSeconds); // Time inside the current on-off cycle
            spriteRenderer.enabled = t < onSeconds; // Visible in the first part of the cycle, hidden in the rest
        }
    }
}
