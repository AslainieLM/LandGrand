using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Places rocks and minerals on the path ahead of the explorer. They scroll with the ground;
    /// when the explorer reaches one, it stops, picks it up (with a chime and a sparkle), and walks on.
    /// </summary>
    public class MineralSpawner : MonoBehaviour
    {
        public Sprite[] mineralSprites = new Sprite[0]; // Pictures of each kind of mineral
        public AudioClip[] pickupSounds = new AudioClip[0]; // Chimes, one picked at random each time
        public ExplorerCollector explorer; // The explorer who picks them up
        [Tooltip("A disabled sparkle object with a one-shot SpriteFlipbook.")]
        public GameObject sparkleTemplate; // Hidden sparkle to copy
        public ParallaxLayer groundLayer; // Ground layer the minerals scroll with
        public float pathY = -3.15625f; // Height of the path in world units
        [Tooltip("How far in front of the explorer's centre a mineral is when the explorer stops for it.")]
        public float touchDistance = 0.19f; // How close the explorer gets before stopping
        [Tooltip("Backpack position, in pixels from the explorer's centre (x right, y up).")]
        public Vector2 backpackOffsetPixels = new Vector2(-4f, 4f); // Where the backpack is
        public Vector2 secondsBetweenMinerals = new Vector2(1.5f, 3f); // Shortest and longest wait between minerals
        [Tooltip("The first mineral starts this far in front of the explorer, so it arrives quickly.")]
        public float firstMineralAheadUnits = 3f; // Distance of the first mineral
        public int sortingOrder = 75; // Draw order on the path
        public int heldSortingOrder = 82; // Draw order while held, in front of the explorer
        [Range(0f, 1f)] public float volume = 0.8f; // Base chime loudness

        AudioSource audioSource; // Plays the chimes
        Camera cam; // The camera, used to find the screen edges
        float next = 1f; // Seconds left until the next mineral

        public ExplorerCollector Collector => explorer; // The explorer, for the minerals to use
        internal float GroundSpeedUnits => groundLayer.speedPixelsPerSecond / groundLayer.pixelsPerUnit; // Ground speed in world units per second
        internal Vector3 BackpackPosition => explorer.transform.position + new Vector3(backpackOffsetPixels.x, backpackOffsetPixels.y, 0f) / 32f; // Where the backpack is in the world now

        void Start() // Runs once before the first frame
        {
            cam = Camera.main; // Find the main camera
            audioSource = GetComponent<AudioSource>(); // Use the sound player on this object
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>(); // Or add one if there is none
            audioSource.playOnAwake = false; // Do not play anything by itself
            if (explorer != null && mineralSprites.Length > 0) // If everything is set up
                Spawn(explorer.transform.position.x + firstMineralAheadUnits); // Place the first mineral close by
        }

        void Update() // Runs every frame
        {
            if (mineralSprites.Length == 0 || groundLayer == null || explorer == null || cam == null) return; // Do nothing if anything needed is missing
            // The countdown pauses while the world is stopped.
            next -= Time.deltaTime * ParallaxLayer.WalkSpeedScale; // Count down the wait
            if (next > 0f) return; // Keep waiting until the countdown reaches zero
            next = Random.Range(secondsBetweenMinerals.x, secondsBetweenMinerals.y); // Pick a new random wait

            float halfWidth = cam.orthographicSize * cam.aspect; // Half the screen width in world units
            Spawn(cam.transform.position.x + halfWidth + 0.5f); // Place a new mineral just past the right edge
        }

        void Spawn(float x) // Makes one mineral at this x on the path
        {
            var go = new GameObject("Mineral"); // New object for the mineral
            go.transform.SetParent(transform, false); // Keep it under the spawner
            go.transform.position = new Vector3(x, pathY, 0f); // Put it on the path
            var sr = go.AddComponent<SpriteRenderer>(); // Draws the mineral
            sr.sprite = mineralSprites[Random.Range(0, mineralSprites.Length)]; // Pick a random kind
            sr.sortingOrder = sortingOrder; // Draw order on the path
            go.AddComponent<MineralPickup>().spawner = this; // Give it the pick-up behaviour
        }

        internal bool Touches(Vector3 position) => position.x <= explorer.transform.position.x + touchDistance; // True once the explorer has reached this spot

        internal void PlayPickupSound() // Plays one chime
        {
            if (pickupSounds.Length > 0) // Only if there are chimes
                audioSource.PlayOneShot(pickupSounds[Random.Range(0, pickupSounds.Length)], volume * GameSettings.SfxVolume); // Play a random chime, scaled by the player's sound-effects setting
        }

        internal void SparkleAt(Vector3 position) // Makes a sparkle at this spot
        {
            if (sparkleTemplate == null) return; // Only if there is a sparkle to copy
            var s = Instantiate(sparkleTemplate, position + new Vector3(0f, 0.1f, 0f), Quaternion.identity, transform); // Copy the sparkle just above the spot
            s.SetActive(true); // Show it
        }
    }
}
