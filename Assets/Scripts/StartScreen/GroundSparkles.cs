using System; // Gives access to [Serializable]
using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and JsonUtility

namespace LandGrand.StartScreen
{
    /// <summary>Makes the coloured minerals painted into the ground layer twinkle now and then.</summary>
    public class GroundSparkles : MonoBehaviour
    {
#pragma warning disable 0649 // filled in by JsonUtility
        [Serializable] class Point { public float x; public float y; public string kind; } // One mineral spot read from the JSON file
        [Serializable] class Points { public float canvasWidth; public float canvasHeight; public Point[] minerals; } // The whole JSON file: canvas size and mineral spots
#pragma warning restore 0649 // Turn the unused-field warning back on

        public TextAsset pointsJson; // JSON file with the mineral spots in the ground art
        public ParallaxLayer groundLayer; // Ground layer the sparkles stay on
        [Tooltip("A disabled sparkle object with a one-shot SpriteFlipbook.")]
        public GameObject sparkleTemplate; // Hidden sparkle to copy
        public Vector2 secondsBetweenSparkles = new Vector2(0.3f, 0.9f); // Shortest and longest wait between sparkles
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit

        Points points; // The spots read from the JSON file
        float next; // Seconds left until the next sparkle
        Camera cam; // The camera, used to find the screen edges

        void Start() // Runs once before the first frame
        {
            cam = Camera.main; // Find the main camera
            if (pointsJson != null) points = JsonUtility.FromJson<Points>(pointsJson.text); // Read the mineral spots
        }

        void Update() // Runs every frame
        {
            if (points?.minerals == null || points.minerals.Length == 0 || groundLayer == null || sparkleTemplate == null) return; // Do nothing if anything needed is missing
            next -= Time.deltaTime; // Count down the wait
            if (next > 0f) return; // Keep waiting until the countdown reaches zero
            next = UnityEngine.Random.Range(secondsBetweenSparkles.x, secondsBetweenSparkles.y); // Pick a new random wait

            var p = points.minerals[UnityEngine.Random.Range(0, points.minerals.Length)]; // Pick a random mineral spot
            float x = groundLayer.CanvasToWorldX(p.x); // Where that spot is in the world now
            float halfWidth = cam.orthographicSize * cam.aspect; // Half the screen width in world units
            if (Mathf.Abs(x - cam.transform.position.x) > halfWidth) return; // Skip spots that are off the screen
            float y = (points.canvasHeight / 2f - p.y) / pixelsPerUnit; // Height of the spot in world units
            var sparkle = Instantiate(sparkleTemplate, new Vector3(x, y, 0f), Quaternion.identity, transform); // Copy the sparkle to the spot
            sparkle.SetActive(true); // Show it
            sparkle.AddComponent<FollowLayer>().Init(groundLayer, p.x); // Keep it on the spot as the ground scrolls
        }
    }
}
