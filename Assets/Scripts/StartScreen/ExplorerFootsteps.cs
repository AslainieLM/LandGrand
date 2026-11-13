using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Puffs dust under the front foot on the two contact frames of the walk cycle.
    /// The dust then scrolls away with the ground, as if left behind on the path.
    /// </summary>
    [RequireComponent(typeof(SpriteFlipbook))] // This script needs a SpriteFlipbook on the same object
    public class ExplorerFootsteps : MonoBehaviour
    {
        [Tooltip("A disabled dust object to copy for each step.")]
        public GameObject dustTemplate; // Hidden dust puff to copy
        [Tooltip("The ground layer, so the dust moves with it.")]
        public ParallaxLayer ground; // Ground layer the dust moves with
        [Tooltip("Where the front foot lands, in pixels from the explorer's centre (x right, y up).")]
        public Vector2 footOffsetPixels = new Vector2(3f, -12f); // Where the front foot lands
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit
        public int[] contactSteps = { 0, 4 }; // Walk steps where a foot touches the ground

        SpriteFlipbook flipbook; // Plays the walk frames

        void Awake() => flipbook = GetComponent<SpriteFlipbook>(); // Find the flipbook once at the start
        void OnEnable() => flipbook.FrameChanged += OnFrame; // Listen for new frames when turned on
        void OnDisable() => flipbook.FrameChanged -= OnFrame; // Stop listening when turned off

        void OnFrame(int step) // Runs each time the shown frame changes
        {
            if (dustTemplate == null || System.Array.IndexOf(contactSteps, step) < 0) return; // Only on foot-contact steps, and only if there is a dust puff
            var offset = new Vector3(footOffsetPixels.x, footOffsetPixels.y, 0f) / pixelsPerUnit; // Foot offset in world units
            var dust = Instantiate(dustTemplate, transform.position + offset, Quaternion.identity, transform.parent); // Copy the dust puff to the foot
            dust.name = "Dust"; // Give the copy a clear name
            dust.SetActive(true); // Show it
            if (ground != null) dust.AddComponent<GroundDrift>().ground = ground; // Make it move with the ground
        }
    }
}
