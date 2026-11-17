using System.Linq; // Checks whether the feet are in any ice zone
using LandGrand.Rules; // Gives the shared standing-on-a-zone check
using UnityEngine; // Gives access to the level part and sound clip

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Checks the ice before the motor moves
    public sealed class IceWatcher : MonoBehaviour // Connects ice to the explorer
    { // Opens the part
        public AudioClip slideSound; // Plays when a slide begins
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
            explorer.OnIce = ZoneFooting.IsStandingIn(explorer.Grounded, explorer.StandingOn != null, feet.y, feet.x, x => level.Run.Level.Layout.IceZones.Any(zone => zone.Contains(x))); // Same flat-ground test as sand; only solid ground at height zero can be icy
        } // Ends the ground check

        void LateUpdate() // Checks for the start of the slide sound
        { // Starts the sound check
            if (level == null || !LevelClock.Running || level.explorer.Frozen) return; // Paused frames leave the slide alone
            bool sliding = level.explorer.SlidingOnIce; // The motor read the held arrow this frame
            if (sliding && !wasSliding) level.PlaySfx(slideSound); // Plays once per slide
            wasSliding = sliding; // Remembers the slide on the next frame
        } // Ends the sound check
    } // Ends the part
} // Ends the play code
