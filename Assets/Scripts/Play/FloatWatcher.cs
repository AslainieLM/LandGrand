using System.Linq; // Builds the list of water gaps
using LandGrand.Rules; // Gives water and float placements
using UnityEngine; // Gives sound and sprite references

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    [DefaultExecutionOrder(-100)] // Moves the rafts before the motor carries the explorer
    public sealed class FloatWatcher : MonoBehaviour // Keeps the rafts and water splashes in step
    { // Opens the part
        public FloatView[] floats; // The rafts in layout order
        public Sprite[] ripples; // The two warning frames
        public AudioClip splash; // The shared splash effect
        LevelController level; // Gives the explorer and sound player
        (float left, float right)[] water; // Water pits and safe pools at the ground line
        bool splashedUnder; // A raft already made this frame's splash
        bool inWater; // Whether the explorer was in water last frame

        internal bool IsBuilt => GetComponent<LevelController>() != null && floats != null && floats.All(f => f != null && f.IsBuilt) && ripples != null && ripples.Length == 2 && ripples.All(s => s != null) && splash != null; // Needs every raft picture and the splash

        internal void Begin(LevelController owner) // Connects the rafts to the attempt
        { // Opens the setup
            level = owner; // Keeps the controller for sounds and footing
            var data = level.Run.Level; // Reads the water and raft placements
            water = data.Hazards.Where(h => h.IsWater).Select(h => (h.Place.X, h.Place.X + h.Width)) // Reads the water pits
                .Concat(data.Layout.Pools.Select(p => (p.X, p.X + p.Width))).ToArray(); // Adds the safe pools
            for (int i = 0; i < floats.Length; i++) floats[i].Begin(data.Layout.Floats[i], LevelClock.Now); // Starts each float at its water line
        } // Ends the setup

        void Start() // Starts the optional part once the attempt is ready
        { // Opens the startup
            var owner = GetComponent<LevelController>(); // Finds this level's controller
            if (owner == null || owner.Run == null) return; // The controller already reports an old scene
            if (!IsBuilt || floats.Length != owner.Run.Level.Layout.Floats.Count) // Checks the placed scene pieces against the layout
            { // Opens the missing-part message
                Debug.LogError("The water part is missing its rafts, pictures or sound. Run LandGrand > Build Levels."); // Names the builder that repairs it
                return; // Waits for a rebuilt scene
            } // Ends the missing-part check
            Begin(owner); // Reads the level data
        } // Ends startup

        void Update() // Moves the raft before the motor checks its support
        { // Opens the raft updates
            if (level == null || level.Run.IsPaused || level.Run.Outcome != RunOutcome.Playing) return; // Skips pending travel while the game waits
            float now = LevelClock.Now; // Time freezes while the game waits
            var support = level.explorer.StandingOn; // Finds the raft under the explorer
            splashedUnder = false; // Starts a new sound check
            foreach (var view in floats) // Moves each raft at its own depth
                if (view.Step(now, support == view, ripples)) splashedUnder = true; // A rider went under with this raft
            if (splashedUnder) level.PlaySfx(splash); // Plays one sound when the raft goes under
        } // Ends the raft updates

        void LateUpdate() // Detects a drop after the explorer moves
        { // Opens the water check
            if (level == null || level.Run.IsPaused || !LevelClock.Running) return; // Paused frames keep the previous water contact
            var feet = level.explorer.Position; // Reads the new feet position
            bool wet = feet.y < 0f && level.explorer.StandingOn == null && water.Any(w => feet.x > w.left && feet.x < w.right); // Counts a drop into water without a raft supporting the feet
            if (wet && !inWater && !splashedUnder) level.PlaySfx(splash); // Starts a splash at the water line
            inWater = wet; // Remembers contact so it does not repeat
        } // Ends the water check
    } // Ends the part
} // Ends the play code
