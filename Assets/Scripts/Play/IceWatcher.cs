using System.Linq; // Checks whether the feet are in any ice zone
using LandGrand.Rules; // Gives the shared standing-on-a-zone check
using UnityEngine; // Gives access to the level part and sound clip

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Checks the ice before the motor moves
    public sealed class IceWatcher : MonoBehaviour // Connects ice to the explorer
    { // Opens the part
        public AudioClip slideSound; // Plays when a slide begins
        public AudioClip mudSlideSound; // Plays when a slide begins on mud
        public SpriteRenderer mudSplash; // Shows mud at the explorer's feet
        public Sprite[] mudSplashFrames; // Both splash pictures
        bool onMud; // Remembers the surface for this step
        float mudSlideStart; // Keeps the clock time of the first splash
        const float MudFrameSeconds = 0.15f; // Matches the sand puff frame time
        LevelController level; // Gives the part its explorer and layout
        bool wasSliding; // Remembers whether the last frame was a slide

        internal bool IsBuilt => GetComponent<LevelController>() != null; // Shares a controller with the other level parts
        internal void Begin(LevelController owner) => level = owner; // Connects the part to its level
        void Start() // Connects the optional part after the level begins
        { // Starts the setup
            if (IsBuilt) Begin(GetComponent<LevelController>()); // Uses its own level controller
        } // Ends the setup

        void Update() // Finds the ice under the explorer
        { // Starts the ground check
            if (level == null || level.Run == null) return; // Waits for the attempt
            var explorer = level.explorer; // Reads the position before the motor moves
            var feet = explorer.Position; // Uses the middle of the feet
            var layout = level.Run.Level.Layout; // Gets the two slippery zone lists
            onMud = layout.MudZones.Count > 0 && ZoneFooting.IsStandingIn(explorer.Grounded, explorer.StandingOn != null, feet.y, feet.x, x => layout.MudZones.Any(zone => zone.Contains(x))); // The same flat ground check applies to mud
            explorer.OnIce = onMud || ZoneFooting.IsStandingIn(explorer.Grounded, explorer.StandingOn != null, feet.y, feet.x, x => layout.IceZones.Any(zone => zone.Contains(x))); // Same flat-ground test as sand; only solid ground at height zero can be icy
        } // Ends the ground check

        void LateUpdate() // Checks for the start of the slide sound
        { // Starts the sound check
            if (level == null || !LevelClock.Running || level.explorer.Frozen) return; // Paused frames leave the slide alone
            bool sliding = level.explorer.SlidingOnIce; // The motor read the held arrow this frame
            if (sliding && !wasSliding) // The explorer has just started a slide
            { // Opens the first slide frame
                level.PlaySfx(onMud ? mudSlideSound : slideSound); // Plays the sound for this surface once
                mudSlideStart = LevelClock.Now; // Starts the mud pictures with the slide
            } // Ends the first slide frame
            if (mudSplash != null) DrawMudSplash(sliding && onMud); // Draws mud only while sliding on it
            wasSliding = sliding; // Remembers the slide on the next frame
        } // Ends the sound check
        void DrawMudSplash(bool show) // Follows the feet while the explorer slides
        { // Opens the splash drawing
            mudSplash.enabled = show; // Hides the pictures off mud and between slides
            if (!show) return; // A stopped slide has no picture
            int frame = Mathf.FloorToInt((LevelClock.Now - mudSlideStart) / MudFrameSeconds) % mudSplashFrames.Length; // Swaps pictures on the level clock
            mudSplash.sprite = mudSplashFrames[frame]; // Shows the splash for this moment
            var feet = level.explorer.Position; // Follows the moved explorer
            mudSplash.transform.position = LevelGeometry.ToWorld(Mathf.Round(feet.x), Mathf.Round(feet.y) + mudSplash.sprite.rect.height / 2f); // Sits the splash at the feet
            mudSplash.flipX = level.explorer.Facing < 0; // Faces the splash with the explorer
        } // Ends the splash drawing
    } // Ends the part
} // Ends the play code
