using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.Rendering.Universal; // Gives access to the Pixel Perfect Camera
using UnityEngine.UI; // Gives access to CanvasScaler

namespace LandGrand
{
    /// <summary>
    /// Scales the UI by the same whole number as the Pixel Perfect Camera scales the game art.
    /// Then one UI pixel is always exactly one art pixel, so pixel letters never come out uneven.
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))] // This script needs a CanvasScaler on the same object
    public class PixelPerfectUiScale : MonoBehaviour
    {
        public PixelPerfectCamera pixelCamera; // The camera whose zoom the UI follows

        CanvasScaler scaler; // Sets the size of the whole UI

        void Awake() // Runs once when the object is created
        {
            scaler = GetComponent<CanvasScaler>(); // Find the scaler
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; // The size is set by this script, not by the screen size
            Apply(); // Set the size before the first frame is drawn
        }

        void LateUpdate() // Runs every frame, after the camera has worked out its zoom
        {
            Apply(); // Follow the zoom if the screen size changed
        }

        void Apply() // Gives the UI the same zoom as the camera
        {
            // Work out the zoom the same way the Pixel Perfect Camera does (PixelPerfectCameraInternal).
            // Reading pixelCamera.pixelRatio here crashed: in Awake the camera may not have set itself up yet.
            int refX = pixelCamera != null ? pixelCamera.refResolutionX : 480; // Game width in art pixels
            int refY = pixelCamera != null ? pixelCamera.refResolutionY : 270; // Game height in art pixels
            int zoom = Mathf.Min(Screen.width / refX, Screen.height / refY); // Biggest whole zoom that fits both ways
            if (zoom < 1) zoom = 1; // Never smaller than one
            if (!Mathf.Approximately(scaler.scaleFactor, zoom)) scaler.scaleFactor = zoom; // Only change it when it is different
        }
    }
}
