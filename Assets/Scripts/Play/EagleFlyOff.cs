using UnityEngine; // Gives access to MonoBehaviour, Sprite and SpriteRenderer

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// A perched eagle near the summit (D-055, decoration only): when the explorer comes near, it flaps away up and to the right.
    /// It has no effect on play.
    /// </summary>
    public class EagleFlyOff : MonoBehaviour // A startled eagle
    {
        public ExplorerMotor explorer; // Who startles it
        public SpriteRenderer body; // The eagle picture
        public Sprite perched; // Standing with folded wings
        public Sprite[] flapFrames; // Flying frames
        public float startleDistance = 90f; // How near (in pixels) the explorer must come
        public Vector2 flightSpeed = new Vector2(70f, 55f); // Pixels per second right and up
        public float flapsPerSecond = 8f; // Wing speed
        public float goneAboveY = 320f; // Height in level pixels above the top of the screen, where the eagle is hidden

        Vector2 at; // Feet place in level pixels
        bool flying; // True once startled
        float clock; // Time in the air

        void Start() // Remembers where it sits
        {
            at = LevelGeometry.ToPixels(transform.position); // Its place in level pixels
            body.sprite = perched; // Sitting still
        }

        void Update() // Waits for the explorer, then flies
        {
            if (!flying) // Still perched
            {
                if (explorer != null && Mathf.Abs(explorer.Position.x - at.x) < startleDistance) flying = true; // Startled
                return; // Nothing more while perched
            }
            clock += Time.deltaTime; // Time in the air
            at += flightSpeed * Time.deltaTime; // Fly away
            transform.position = LevelGeometry.ToWorld(Mathf.Round(at.x), Mathf.Round(at.y)); // Whole pixels keep the art sharp
            if (flapFrames != null && flapFrames.Length > 0) body.sprite = flapFrames[(int)(clock * flapsPerSecond) % flapFrames.Length]; // Flap the wings
            if (at.y > goneAboveY) gameObject.SetActive(false); // Gone off the top of the screen
        }
    }
}
