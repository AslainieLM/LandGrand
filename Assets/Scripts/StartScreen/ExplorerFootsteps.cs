using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Puffs dust under the front foot on the two contact frames of the walk cycle.
    /// The dust then scrolls away with the ground, as if left behind on the path.
    /// </summary>
    [RequireComponent(typeof(SpriteFlipbook))]
    public class ExplorerFootsteps : MonoBehaviour
    {
        [Tooltip("A disabled dust object to copy for each step.")]
        public GameObject dustTemplate;
        [Tooltip("The ground layer, so the dust moves with it.")]
        public ParallaxLayer ground;
        [Tooltip("Where the front foot lands, in pixels from the explorer's centre (x right, y up).")]
        public Vector2 footOffsetPixels = new Vector2(3f, -12f);
        public float pixelsPerUnit = 32f;
        public int[] contactSteps = { 0, 4 };

        SpriteFlipbook flipbook;

        void Awake() => flipbook = GetComponent<SpriteFlipbook>();
        void OnEnable() => flipbook.FrameChanged += OnFrame;
        void OnDisable() => flipbook.FrameChanged -= OnFrame;

        void OnFrame(int step)
        {
            if (dustTemplate == null || System.Array.IndexOf(contactSteps, step) < 0) return;
            var offset = new Vector3(footOffsetPixels.x, footOffsetPixels.y, 0f) / pixelsPerUnit;
            var dust = Instantiate(dustTemplate, transform.position + offset, Quaternion.identity, transform.parent);
            dust.name = "Dust";
            dust.SetActive(true);
            if (ground != null) dust.AddComponent<GroundDrift>().ground = ground;
        }
    }
}
