using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>Sends a small flock of birds across the sky every few seconds.</summary>
    public class BirdFlock : MonoBehaviour
    {
        [Tooltip("A disabled bird object with a SpriteFlipbook to copy.")]
        public GameObject birdTemplate; // Hidden bird that each flock copies
        public Vector2 secondsBetweenFlocks = new Vector2(7f, 13f); // Shortest and longest wait between flocks
        public Vector2Int birdsPerFlock = new Vector2Int(3, 6); // Fewest and most birds in one flock
        public Vector2 speedUnitsPerSecond = new Vector2(1.4f, 2.0f); // Slowest and fastest flying speed
        [Tooltip("World y range for the flock.")]
        public Vector2 heightRange = new Vector2(0.4f, 2.0f); // Lowest and highest flying height

        float nextFlock = 2f; // Seconds left until the next flock; the first comes after 2 seconds
        Camera cam; // The camera, used to find the screen edges

        void Start() => cam = Camera.main; // Find the main camera once at the start

        void Update() // Runs every frame
        {
            if (birdTemplate == null || cam == null) return; // Do nothing if the bird or camera is missing
            nextFlock -= Time.deltaTime; // Count down the wait
            if (nextFlock > 0f) return; // Keep waiting until the countdown reaches zero
            nextFlock = Random.Range(secondsBetweenFlocks.x, secondsBetweenFlocks.y); // Pick a new random wait for the flock after this one

            float halfWidth = cam.orthographicSize * cam.aspect; // Half the screen width in world units
            float startX = cam.transform.position.x + halfWidth + 0.5f; // Start just past the right edge of the screen
            float y = Random.Range(heightRange.x, heightRange.y); // Random height for this flock
            float speed = Random.Range(speedUnitsPerSecond.x, speedUnitsPerSecond.y); // Random speed, shared by the whole flock
            int count = Random.Range(birdsPerFlock.x, birdsPerFlock.y + 1); // Random number of birds in this flock
            for (int i = 0; i < count; i++) // Make each bird
            {
                var pos = new Vector3(startX + i * 0.3f + Random.Range(0f, 0.15f), y + Random.Range(-0.25f, 0.25f), 0f); // Space the birds out a little, with small random offsets
                var bird = Instantiate(birdTemplate, pos, Quaternion.identity, transform); // Copy the hidden bird to that spot
                bird.SetActive(true); // Show the copy
                var mover = bird.AddComponent<DriftLeft>(); // Make it fly to the left
                mover.speed = speed; // Flying speed
                mover.destroyBelowX = cam.transform.position.x - halfWidth - 1f; // Remove the bird once it passes the left edge
            }
        }
    }
}
