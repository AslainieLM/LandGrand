using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>
    /// Switches the explorer between walking and picking up a mineral:
    /// stop, crouch, grab it from the ground, hold it up, put it in the backpack, walk on.
    /// The world stops scrolling while the explorer is collecting.
    /// </summary>
    [RequireComponent(typeof(SpriteFlipbook))] // This script needs a SpriteFlipbook on the same object
    public class ExplorerCollector : MonoBehaviour
    {
        public int[] walkSequence = { 0, 1, 2, 3, 4, 5, 6, 7 }; // Frames of the walk animation
        [Tooltip("Collect frames at 10 fps. Repeated entries hold a pose longer.")]
        public int[] collectSequence = { 8, 9, 10, 11, 11, 12, 13, 13, 13, 14, 15 }; // Frames of the pick-up animation
        public int grabFrame = 11; // Frame where the glove closes on the mineral
        public int showFrame = 13; // Frame where the explorer holds the mineral up
        public int stashFrame = 14; // Frame where the mineral goes into the backpack
        [Tooltip("Glove position for each frame index, in pixels from the sprite centre (x right, y up).")]
        public Vector2[] handOffsetsPixels = new Vector2[16]; // Glove position for each frame
        public float pixelsPerUnit = 32f; // Sprite pixels per world unit
        [Tooltip("Seconds for the world to start scrolling again after collecting.")]
        public float resumeSeconds = 0.3f; // How long the world takes to speed back up

        public bool IsCollecting { get; private set; } // True while the pick-up animation plays

        SpriteFlipbook flipbook; // Plays the explorer's frames
        ExplorerFootsteps footsteps; // Makes dust on each step; paused while collecting
        MineralPickup carried; // The mineral being picked up, or null
        bool grabbed; // True once the glove has closed on the mineral
        bool shown; // True once the mineral has been held up

        void Awake() // Runs once when the object is created
        {
            flipbook = GetComponent<SpriteFlipbook>(); // Find the flipbook
            footsteps = GetComponent<ExplorerFootsteps>(); // Find the footsteps, if there are any
        }

        void OnEnable() // Runs when the script turns on
        {
            flipbook.FrameChanged += OnStep; // Listen for each new frame
            flipbook.Finished += OnCollectFinished; // Listen for the end of the pick-up animation
        }

        void OnDisable() // Runs when the script turns off
        {
            flipbook.FrameChanged -= OnStep; // Stop listening for new frames
            flipbook.Finished -= OnCollectFinished; // Stop listening for the end of the animation
        }

        void Start() // Runs once before the first frame
        {
            ParallaxLayer.WalkSpeedScale = 1f; // The world scrolls at full speed
            flipbook.Play(walkSequence, true); // Start walking, looping forever
        }

        void Update() // Runs every frame
        {
            if (!IsCollecting && ParallaxLayer.WalkSpeedScale < 1f) // After collecting, while the world is still slower than normal
            {
                float rate = resumeSeconds > 0f ? Time.deltaTime / resumeSeconds : 1f; // How much to speed up this frame
                ParallaxLayer.WalkSpeedScale = Mathf.MoveTowards(ParallaxLayer.WalkSpeedScale, 1f, rate); // Ease the world back to full speed
            }
        }

        /// <summary>Starts the pick-up animation for this mineral. Returns false if already busy.</summary>
        public bool TryCollect(MineralPickup mineral) // Called by a mineral when the explorer reaches it
        {
            if (IsCollecting || mineral == null) return false; // Say no if already busy or no mineral was given
            IsCollecting = true; // Now busy collecting
            carried = mineral; // Remember which mineral
            grabbed = false; // Not grabbed yet
            shown = false; // Not held up yet
            ParallaxLayer.WalkSpeedScale = 0f; // Stop the world from scrolling
            if (footsteps != null) footsteps.enabled = false; // No dust while standing still
            flipbook.Play(collectSequence, false); // Play the pick-up animation once
            return true; // Say yes, collecting has started
        }

        /// <summary>World position of the glove in the current frame.</summary>
        public Vector3 HandPosition // Where the glove is now, in the world
        {
            get // Worked out each time it is asked for
            {
                int f = flipbook.CurrentFrame; // The frame shown now
                Vector2 px = f >= 0 && f < handOffsetsPixels.Length ? handOffsetsPixels[f] : Vector2.zero; // Glove offset for that frame, or none if unknown
                return transform.position + new Vector3(px.x, px.y, 0f) / pixelsPerUnit; // Explorer position plus the glove offset
            }
        }

        void OnStep(int step) // Runs each time the shown frame changes
        {
            if (!IsCollecting || carried == null) return; // Only matters while carrying a mineral
            int f = flipbook.CurrentFrame; // The frame shown now
            if (f >= grabFrame && f < stashFrame) // From the grab until the stash
            {
                if (!grabbed) { grabbed = true; carried.OnGrabbed(); } // On the first grab frame, tell the mineral it was grabbed (plays the chime)
                carried.HoldAt(HandPosition); // Keep the mineral in the glove
                if (f >= showFrame && !shown) { shown = true; carried.OnShown(); } // On the first hold-up frame, make it sparkle
            }
            else if (f >= stashFrame && grabbed) // Once the stash frame is reached
            {
                carried.OnStashed(); // Put the mineral in the backpack
                carried = null; // No longer carrying it
            }
        }

        void OnCollectFinished() // Runs when the pick-up animation ends
        {
            IsCollecting = false; // No longer busy
            if (carried != null) { carried.OnStashed(); carried = null; } // Make sure the mineral is put away
            if (footsteps != null) footsteps.enabled = true; // Dust on steps again
            flipbook.Play(walkSequence, true); // Walk on
        }
    }
}
