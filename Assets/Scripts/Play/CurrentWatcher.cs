using System.Linq; // Checks all of the built river marks
using LandGrand.Rules; // Gives the current and footing rules
using UnityEngine; // Gives sprite renderers and pixel math

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Reads footing before the motor walks
    public sealed class CurrentWatcher : MonoBehaviour // Shows each river strip and applies its push
    { // Opens the part
        [System.Serializable] // Lets the builder save each strip's marks
        public struct Strip // The views for one current strip
        { // Opens the saved view
            public SpriteRenderer[] marks; // The ripple marks along this water
        } // Ends the saved view

        public Strip[] strips; // Current strips in layout order
        public Sprite[] frames; // Two ripple chevrons
        LevelController level; // Gives the explorer and river layout
        const float FrameSeconds = 0.15f; // Each ripple frame lasts as long as a sand puff frame
        public const float MarkSpacing = 48f; // Keeps the marks well apart

        internal bool IsBuilt => GetComponent<LevelController>() != null && strips != null && strips.All(s => s.marks != null && s.marks.Length > 0 && s.marks.All(m => m != null)) && frames != null && frames.Length == 2 && frames.All(s => s != null); // Needs both pictures and all river marks
        internal void Begin(LevelController owner) => level = owner; // Connects the part to the attempt

        void Start() // Starts the optional part once the attempt is ready
        { // Opens setup
            var owner = GetComponent<LevelController>(); // Finds this level's controller
            if (owner == null || owner.Run == null) return; // The controller handles an old scene
            if (!IsBuilt || strips.Length != owner.Run.Level.Layout.Currents.Count) // Checks the scene against the layout
            { // Opens the missing-part message
                Debug.LogError("The river currents are missing their ripple marks. Run LandGrand > Build Levels."); // Gives the repair command
                return; // Waits for a rebuilt scene
            } // Ends the missing-part check
            Begin(owner); // Reads the level data
        } // Ends setup

        void Update() // Sets the push for this step and shows the current
        { // Opens the update
            if (level == null || level.Run.IsPaused || level.Run.Outcome != RunOutcome.Playing) return; // Keeps the ripple marks still while the game waits
            var explorer = level.explorer; // Reads the feet and footing before movement
            var feet = explorer.Position; // Uses the center of the feet
            var rules = level.Run.Level.Layout.Currents; // Finds the laid-out strips
            float time = LevelClock.Now; // Pausing also stops the river marks
            float push = 0f; // Dry ground has no river push
            var sprite = frames[Mathf.FloorToInt(time / FrameSeconds) % frames.Length]; // Shares one frame across the marks
            for (int i = 0; i < strips.Length; i++) // Checks each stretch of water
            { // Opens a strip
                var zone = rules[i]; // Its left edge, width and flow
                push += zone.PushAt(explorer.Grounded, explorer.StandingOn != null, feet.y, feet.x); // Applies only while on flat ground
                DrawStrip(zone, strips[i].marks, time, sprite); // Moves its marks in the same direction
            } // Ends a strip
            explorer.CurrentPushX = push; // Gives the motor the ground push
        } // Ends the update

        static void DrawStrip(CurrentZone zone, SpriteRenderer[] marks, float time, Sprite frame) // Scrolls ripple marks within one strip
        { // Opens the art update
            float spacing = zone.Width / marks.Length; // Shares the strip evenly between marks
            float travel = time * CurrentZone.Speed; // Moves at the water's speed
            float halfWidth = frame.rect.width / 2f; // Measures the full visible mark
            for (int i = 0; i < marks.Length; i++) // Places each mark in the moving row
            { // Opens a mark
                float offset = Mathf.Repeat((i + 0.5f) * spacing + travel, zone.Width); // Wraps at the ends of the strip
                var mark = marks[i]; // Draws one ripple
                mark.enabled = offset >= halfWidth && offset <= zone.Width - halfWidth; // Keeps the whole mark in the water
                mark.flipX = zone.Direction < 0; // Points left for a leftward flow
                float x = zone.X + (mark.flipX ? zone.Width - offset : offset); // Positions it along the flow
                mark.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), -4f); // Puts it midway down the top tile
                mark.sprite = frame; // Changes the ripple picture
            } // Ends a mark
        } // Ends the art update
    } // Ends the part
} // Ends the play code
