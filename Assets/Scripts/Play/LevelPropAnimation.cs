using UnityEngine; // Gives the renderer and timing math

namespace LandGrand.Play // Holds the level decorations
{ // Opens the play code
    [RequireComponent(typeof(SpriteRenderer))] // A prop has a picture on the same object
    public sealed class LevelPropAnimation : MonoBehaviour // Swaps the decoration's frames on the level clock
    { // Opens the animation
        public Sprite[] frames; // The two pictures of the prop
        public float secondsPerFrame; // The time before the next picture
        SpriteRenderer art; // Draws the prop

        void Awake() => art = GetComponent<SpriteRenderer>(); // Finds the prop picture
        void LateUpdate() => art.sprite = frames[Mathf.FloorToInt(LevelClock.Now / secondsPerFrame) % frames.Length]; // Freezes both frames while the game is paused
    } // Ends the animation
} // Ends the play code
