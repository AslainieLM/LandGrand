using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Sends a small flock of birds across the sky every few seconds.</summary>
    public class BirdFlock : MonoBehaviour
    {
        [Tooltip("A disabled bird object with a SpriteFlipbook to copy.")]
        public GameObject birdTemplate;
        public Vector2 secondsBetweenFlocks = new Vector2(7f, 13f);
        public Vector2Int birdsPerFlock = new Vector2Int(3, 6);
        public Vector2 speedUnitsPerSecond = new Vector2(1.4f, 2.0f);
        [Tooltip("World y range for the flock.")]
        public Vector2 heightRange = new Vector2(0.4f, 2.0f);

        float nextFlock = 2f;
        Camera cam;

        void Start() => cam = Camera.main;

        void Update()
        {
            if (birdTemplate == null || cam == null) return;
            nextFlock -= Time.deltaTime;
            if (nextFlock > 0f) return;
            nextFlock = Random.Range(secondsBetweenFlocks.x, secondsBetweenFlocks.y);

            float halfWidth = cam.orthographicSize * cam.aspect;
            float startX = cam.transform.position.x + halfWidth + 0.5f;
            float y = Random.Range(heightRange.x, heightRange.y);
            float speed = Random.Range(speedUnitsPerSecond.x, speedUnitsPerSecond.y);
            int count = Random.Range(birdsPerFlock.x, birdsPerFlock.y + 1);
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3(startX + i * 0.3f + Random.Range(0f, 0.15f), y + Random.Range(-0.25f, 0.25f), 0f);
                var bird = Instantiate(birdTemplate, pos, Quaternion.identity, transform);
                bird.SetActive(true);
                var mover = bird.AddComponent<DriftLeft>();
                mover.speed = speed;
                mover.destroyBelowX = cam.transform.position.x - halfWidth - 1f;
            }
        }
    }
}
