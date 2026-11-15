using UnityEngine; // Gives the bird its sprites and positions

namespace LandGrand.Play // Holds the level effects
{ // Opens the play code
    [DefaultExecutionOrder(100)] // Follows the camera after it moves
    public sealed class ForestEagle : MonoBehaviour // Sends the eagle over the forest now and then
    { // Opens the view
        public Transform cameraTransform; // Gives the visible area
        public SpriteRenderer art; // Draws the eagle
        public Sprite[] frames; // Uses the existing flight pictures

        void LateUpdate() // Draws one flight on the level clock
        { // Starts the frame
            float phase = LevelClock.Now % 14f; // Leaves a gap between flights
            art.enabled = phase >= 5f && phase < 10f; // Shows the passing eagle only in flight
            if (!art.enabled) return; // The eagle is off screen
            float flight = phase - 5f; // Time since it entered
            float x = LevelGeometry.ToPixels(cameraTransform.position).x + 280f - flight * 112f; // Crosses from right to left
            art.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), Mathf.Round(173f + Mathf.Sin(flight) * 8f)); // Glides above the trail
            art.sprite = frames[(int)(flight * 8f) % frames.Length]; // Flies using the Easy eagle frames
        } // Ends the frame
    } // Ends the view
} // Ends the play code
