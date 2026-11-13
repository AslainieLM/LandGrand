using UnityEngine; // Gives access to MonoBehaviour, Transform and Mathf

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// One layer of the moving mountain background (decision D-046).
    /// Put it on a child of the camera. The layer slides left as the camera moves right, so far layers move less than near ones.
    /// The first child holds the layer picture; a second copy is made at start so the seam never shows.
    /// </summary>
    [DefaultExecutionOrder(100)] // Runs after the camera follow, so the layer uses this frame's camera place
    public class BackdropLayer : MonoBehaviour // Scrolls one background layer with the camera
    {
        [Tooltip("0 = fixed to the screen (sky). 1 = moves with the ground. Far layers use small numbers.")]
        public float follow = 0.2f; // Share of the camera's movement this layer copies
        public float driftPixelsPerSecond = 0f; // Slow drift of its own, for clouds
        public float widthPixels = 960f; // Width of the layer art, which tiles sideways
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit

        Transform first; // The layer picture
        Transform second; // A copy placed right after it
        Transform cam; // The camera this layer rides on
        float drift; // Pixels drifted so far

        void Start() // Runs once before the first frame
        {
            first = transform.GetChild(0); // Find the layer picture
            second = Instantiate(first.gameObject, transform).transform; // Copy it
            second.name = first.name + " (wrap copy)"; // Give the copy a clear name
            cam = transform.parent; // The layers sit under the camera
        }

        void LateUpdate() // Runs every frame after the camera moved
        {
            drift += driftPixelsPerSecond * Time.deltaTime; // Clouds drift on their own
            float camPixels = cam.position.x * pixelsPerUnit; // Camera place across, in pixels
            float offset = Mathf.Repeat(camPixels * follow + drift, widthPixels); // How far the layer has slid, wrapping after one width
            float x = -Mathf.Round(offset) / pixelsPerUnit; // Whole pixels so the art stays sharp
            float width = widthPixels / pixelsPerUnit; // Layer width in world units
            first.localPosition = new Vector3(x, 0f, 0f); // Place the first picture
            second.localPosition = new Vector3(x + width, 0f, 0f); // Place the copy right after it
        }
    }
}
