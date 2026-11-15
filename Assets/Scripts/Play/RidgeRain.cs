using UnityEngine; // Gives the rain its sprites and positions

namespace LandGrand.Play // Holds the level effects
{ // Opens the play code
    [DefaultExecutionOrder(100)] // Follows the camera after it moves
    public sealed class RidgeRain : MonoBehaviour // Draws rain in front of the explorer
    { // Opens the view
        public Transform cameraTransform; // Keeps the streaks in the view as it moves
        public SpriteRenderer[] streaks; // The see-through foreground pictures
        public Sprite[] frames; // Two rain pictures

        void LateUpdate() // Moves each streak on the level clock
        { // Starts the frame
            Vector2 center = LevelGeometry.ToPixels(cameraTransform.position); // The camera middle in level pixels
            float now = LevelClock.Now; // Stops the rain when the game waits
            int frame = (int)(now / 0.15f); // Switches rain pictures every 0.15 seconds
            for (int i = 0; i < streaks.Length; i++) // Spaces the streaks across the screen
            { // Moves one streak
                float x = center.x + Mathf.Repeat(i * 79f - now * 34f, 504f) - 252f; // Blows the streak to the left
                float y = center.y + 145f - Mathf.Repeat(i * 43f + now * 160f, 290f); // Falls and comes back above the camera
                streaks[i].transform.position = LevelGeometry.ToWorld(Mathf.Round(x), Mathf.Round(y)); // Draws it on whole pixels
                streaks[i].sprite = frames[(frame + i) % frames.Length]; // Nearby streaks use different pictures
            } // Ends the streak update
        } // Ends the frame
    } // Ends the view
} // Ends the play code
