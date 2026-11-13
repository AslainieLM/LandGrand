using System; // Gives access to Action and Array
using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Sprite

namespace LandGrand.StartScreen
{
    /// <summary>Plays a list of sprites as a frame animation. A sequence entry of -1 hides the sprite for that frame.</summary>
    [RequireComponent(typeof(SpriteRenderer))] // This script needs a SpriteRenderer on the same object
    public class SpriteFlipbook : MonoBehaviour
    {
        public Sprite[] frames = Array.Empty<Sprite>(); // All pictures of the animation
        [Tooltip("Frame indices to play. Leave empty to play every frame in order. -1 hides the sprite.")]
        public int[] sequence = Array.Empty<int>(); // Order to show them in
        public float framesPerSecond = 10f; // Animation speed
        public bool loop = true; // Start again at the end
        public bool destroyWhenDone; // Remove the object when a one-time animation ends

        /// <summary>Raised with the sequence step each time the shown frame changes.</summary>
        public event Action<int> FrameChanged; // Tells listeners about each new frame
        /// <summary>Raised once when a non-looping sequence reaches its end.</summary>
        public event Action Finished; // Tells listeners when a one-time animation ends

        /// <summary>The frame index currently shown, or -1 when hidden.</summary>
        public int CurrentFrame { get; private set; } = -1; // The frame shown now

        SpriteRenderer spriteRenderer; // Draws the frames
        float elapsed; // Seconds since the animation started
        int lastStep = -1; // The last step shown
        bool finished; // True once a one-time animation has ended

        int Length => sequence.Length > 0 ? sequence.Length : frames.Length; // Number of steps in the animation

        void Awake() => spriteRenderer = GetComponent<SpriteRenderer>(); // Find the sprite once at the start

        void OnEnable() => Restart(); // Start from the beginning when turned on

        /// <summary>Switches to another sequence and plays it from the start.</summary>
        public void Play(int[] newSequence, bool shouldLoop) // Play a different animation
        {
            sequence = newSequence ?? Array.Empty<int>(); // Use the new order
            loop = shouldLoop; // Loop or play once
            Restart(); // Go back to the beginning
            Update(); // Show the first frame at once
        }

        void Restart() // Resets the animation to its start
        {
            elapsed = 0f; // No time has passed
            lastStep = -1; // No step shown yet
            finished = false; // Not ended
        }

        void Update() // Runs every frame
        {
            int length = Length; // Number of steps
            if (length == 0 || framesPerSecond <= 0f || finished) return; // Do nothing if there is nothing to play or it has ended

            elapsed += Time.deltaTime; // Add this frame's time
            int step = Mathf.FloorToInt(elapsed * framesPerSecond); // Which step that time reaches
            if (step >= length) // Past the last step
            {
                if (!loop) // A one-time animation
                {
                    finished = true; // Mark it ended
                    if (destroyWhenDone) { Destroy(gameObject); return; } // Remove the object if asked
                    Finished?.Invoke(); // Tell listeners it ended
                    return; // Stop here
                }
                elapsed %= length / framesPerSecond; // Wrap the time back to the start
                step %= length; // Wrap the step back to the start
            }
            if (step == lastStep) return; // Nothing to do if the step did not change
            lastStep = step; // Remember this step

            int frame = sequence.Length > 0 ? sequence[step] : step; // Frame number for this step
            CurrentFrame = frame; // Remember the frame shown
            spriteRenderer.enabled = frame >= 0; // Hide the sprite for -1, show it otherwise
            if (frame >= 0 && frame < frames.Length) spriteRenderer.sprite = frames[frame]; // Show the frame's picture
            FrameChanged?.Invoke(step); // Tell listeners about the new step
        }
    }
}
