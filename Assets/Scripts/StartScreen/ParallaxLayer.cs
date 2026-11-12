using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Scrolls a horizontally tiling background layer to the left forever.
    /// The first child holds the layer sprite; a second copy is made at start so the seam never shows.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        /// <summary>
        /// Shared multiplier for every layer that follows the explorer's walk.
        /// 1 = walking, 0 = the explorer has stopped (for example, to pick up a mineral).
        /// </summary>
        public static float WalkSpeedScale = 1f;

        public float speedPixelsPerSecond;
        [Tooltip("Off for layers that drift on their own, such as clouds.")]
        public bool followsWalk = true;
        public float widthPixels = 960f;
        public float pixelsPerUnit = 32f;

        /// <summary>How far the layer has scrolled, in canvas pixels, from 0 up to widthPixels.</summary>
        public float OffsetPixels { get; private set; }

        Transform first;
        Transform second;

        void Start()
        {
            first = transform.GetChild(0);
            second = Instantiate(first.gameObject, transform).transform;
            second.name = first.name + " (wrap copy)";
            Apply();
        }

        void Update()
        {
            float scale = followsWalk ? WalkSpeedScale : 1f;
            OffsetPixels = Mathf.Repeat(OffsetPixels + speedPixelsPerSecond * scale * Time.deltaTime, widthPixels);
            Apply();
        }

        void Apply()
        {
            // Round to whole pixels so pixel art never shimmers between pixels.
            float x = -Mathf.Round(OffsetPixels) / pixelsPerUnit;
            float width = widthPixels / pixelsPerUnit;
            first.localPosition = new Vector3(x, 0f, 0f);
            second.localPosition = new Vector3(x + width, 0f, 0f);
        }

        /// <summary>World x of a canvas x coordinate, wrapped to the copy nearest the screen centre.</summary>
        public float CanvasToWorldX(float canvasX)
        {
            float width = widthPixels / pixelsPerUnit;
            float x = (canvasX - widthPixels / 2f - Mathf.Round(OffsetPixels)) / pixelsPerUnit;
            x = Mathf.Repeat(x + width / 2f, width) - width / 2f;
            return transform.position.x + x;
        }
    }
}
