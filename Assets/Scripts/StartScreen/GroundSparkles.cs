using System;
using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>Makes the coloured minerals painted into the ground layer twinkle now and then.</summary>
    public class GroundSparkles : MonoBehaviour
    {
#pragma warning disable 0649 // filled in by JsonUtility
        [Serializable] class Point { public float x; public float y; public string kind; }
        [Serializable] class Points { public float canvasWidth; public float canvasHeight; public Point[] minerals; }
#pragma warning restore 0649

        public TextAsset pointsJson;
        public ParallaxLayer groundLayer;
        [Tooltip("A disabled sparkle object with a one-shot SpriteFlipbook.")]
        public GameObject sparkleTemplate;
        public Vector2 secondsBetweenSparkles = new Vector2(0.3f, 0.9f);
        public float pixelsPerUnit = 32f;

        Points points;
        float next;
        Camera cam;

        void Start()
        {
            cam = Camera.main;
            if (pointsJson != null) points = JsonUtility.FromJson<Points>(pointsJson.text);
        }

        void Update()
        {
            if (points?.minerals == null || points.minerals.Length == 0 || groundLayer == null || sparkleTemplate == null) return;
            next -= Time.deltaTime;
            if (next > 0f) return;
            next = UnityEngine.Random.Range(secondsBetweenSparkles.x, secondsBetweenSparkles.y);

            var p = points.minerals[UnityEngine.Random.Range(0, points.minerals.Length)];
            float x = groundLayer.CanvasToWorldX(p.x);
            float halfWidth = cam.orthographicSize * cam.aspect;
            if (Mathf.Abs(x - cam.transform.position.x) > halfWidth) return;
            float y = (points.canvasHeight / 2f - p.y) / pixelsPerUnit;
            var sparkle = Instantiate(sparkleTemplate, new Vector3(x, y, 0f), Quaternion.identity, transform);
            sparkle.SetActive(true);
            sparkle.AddComponent<FollowLayer>().Init(groundLayer, p.x);
        }
    }
}
