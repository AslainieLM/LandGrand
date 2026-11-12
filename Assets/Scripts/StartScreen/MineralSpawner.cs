using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Places rocks and minerals on the path ahead of the explorer. They scroll with the ground;
    /// when the explorer reaches one, it stops, picks it up (with a chime and a sparkle), and walks on.
    /// </summary>
    public class MineralSpawner : MonoBehaviour
    {
        public Sprite[] mineralSprites = new Sprite[0];
        public AudioClip[] pickupSounds = new AudioClip[0];
        public ExplorerCollector explorer;
        [Tooltip("A disabled sparkle object with a one-shot SpriteFlipbook.")]
        public GameObject sparkleTemplate;
        public ParallaxLayer groundLayer;
        public float pathY = -3.15625f;
        [Tooltip("How far in front of the explorer's centre a mineral is when the explorer stops for it.")]
        public float touchDistance = 0.19f;
        [Tooltip("Backpack position, in pixels from the explorer's centre (x right, y up).")]
        public Vector2 backpackOffsetPixels = new Vector2(-4f, 4f);
        public Vector2 secondsBetweenMinerals = new Vector2(1.5f, 3f);
        [Tooltip("The first mineral starts this far in front of the explorer, so it arrives quickly.")]
        public float firstMineralAheadUnits = 3f;
        public int sortingOrder = 75;
        public int heldSortingOrder = 82;
        [Range(0f, 1f)] public float volume = 0.8f;

        AudioSource audioSource;
        Camera cam;
        float next = 1f;

        public ExplorerCollector Collector => explorer;
        internal float GroundSpeedUnits => groundLayer.speedPixelsPerSecond / groundLayer.pixelsPerUnit;
        internal Vector3 BackpackPosition => explorer.transform.position + new Vector3(backpackOffsetPixels.x, backpackOffsetPixels.y, 0f) / 32f;

        void Start()
        {
            cam = Camera.main;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            if (explorer != null && mineralSprites.Length > 0)
                Spawn(explorer.transform.position.x + firstMineralAheadUnits);
        }

        void Update()
        {
            if (mineralSprites.Length == 0 || groundLayer == null || explorer == null || cam == null) return;
            // The countdown pauses while the world is stopped.
            next -= Time.deltaTime * ParallaxLayer.WalkSpeedScale;
            if (next > 0f) return;
            next = Random.Range(secondsBetweenMinerals.x, secondsBetweenMinerals.y);

            float halfWidth = cam.orthographicSize * cam.aspect;
            Spawn(cam.transform.position.x + halfWidth + 0.5f);
        }

        void Spawn(float x)
        {
            var go = new GameObject("Mineral");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(x, pathY, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = mineralSprites[Random.Range(0, mineralSprites.Length)];
            sr.sortingOrder = sortingOrder;
            go.AddComponent<MineralPickup>().spawner = this;
        }

        internal bool Touches(Vector3 position) => position.x <= explorer.transform.position.x + touchDistance;

        internal void PlayPickupSound()
        {
            if (pickupSounds.Length > 0)
                audioSource.PlayOneShot(pickupSounds[Random.Range(0, pickupSounds.Length)], volume);
        }

        internal void SparkleAt(Vector3 position)
        {
            if (sparkleTemplate == null) return;
            var s = Instantiate(sparkleTemplate, position + new Vector3(0f, 0.1f, 0f), Quaternion.identity, transform);
            s.SetActive(true);
        }
    }
}
