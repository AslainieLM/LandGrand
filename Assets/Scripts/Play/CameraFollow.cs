using UnityEngine; // Gives access to MonoBehaviour and Mathf

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Keeps the explorer in view by sliding the camera sideways, never past the level's ends.
    /// The camera height stays fixed, and it moves in whole pixels so the art stays sharp.
    /// </summary>
    public class CameraFollow : MonoBehaviour // Sideways camera follow
    {
        public ExplorerMotor target; // The explorer to follow
        public LevelGeometry geometry; // Gives the level width
        public float viewWidth = 480f; // Screen width in pixels
        public float centerY = 85f; // Camera middle height in level pixels (puts the ground line 50 pixels above the bottom)
        public float lookAhead = 40f; // How far right of the explorer the camera looks (the trail goes right)
        public float smoothSeconds = 0.15f; // How quickly the camera catches up

        float x; // Camera middle across, in pixels, before rounding
        float speed; // Used by the smoothing

        void Start() => x = Target(); // Begin already at the explorer

        void LateUpdate() // Runs after the explorer moved this frame
        {
            x = Mathf.SmoothDamp(x, Target(), ref speed, smoothSeconds); // Glide toward the target
            var p = LevelGeometry.ToWorld(Mathf.Round(x), centerY); // Whole pixels only
            transform.position = new Vector3(p.x, p.y, transform.position.z); // Keep the camera's distance
        }

        float Target() // Where the camera middle should be
        {
            float half = viewWidth / 2f; // Half the screen
            float want = target.Position.x + lookAhead; // A little ahead of the explorer
            return Mathf.Clamp(want, half, Mathf.Max(half, geometry.width - half)); // Never show past the level's ends
        }
    }
}
