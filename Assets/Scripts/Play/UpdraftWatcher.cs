using System; // Lets Unity save the renderers for each column
using System.Linq; // Checks the saved pictures and views
using LandGrand.Rules; // Gives the updraft rules
using UnityEngine; // Gives the scene parts, pictures and sound

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Sets the hover height before the motor moves, like the wind
    public sealed class UpdraftWatcher : MonoBehaviour // Lifts the explorer in a cliff updraft and draws the wind (D-072)
    { // Opens the part
        [Serializable] // Saves the column views in the scene
        public struct View // Holds the pictures for one column
        { // Opens the view
            public SpriteRenderer column; // Shows the tiled rising wind
            public SpriteRenderer[] leaves; // Shows the two rising leaves
            public SpriteRenderer[] dust; // Shows the two rising dust specks
        } // Ends the view

        public View[] views; // One view per column in layout order
        public Sprite[] columnFrames; // The four wind pictures, each 32 by 32
        public Sprite[] leafFrames; // The flat and edge-on leaf
        public Sprite[] dustFrames; // The two dust speck pictures
        public AudioSource windSound; // Loops while the explorer is inside a column

        const float WindVolume = 0.5f; // Scales the wind loop volume as in spec section 5
        const float FramesPerSecond = 8f; // The wind pictures step 8 times a second
        public const float BaseHeight = 8f; // The rock crack at the foot is 8 pixels high; the builder reads it too
        public const float HeadRoom = 26f; // The column reaches the hovering head, 26 pixels above the feet
        static readonly float[] LeafX = { 10f, 22f }; // Leaf places across the column
        static readonly float[] DustX = { 6f, 26f }; // Dust places across the column
        static readonly float[] LeafPhase = { 0f, 0.5f }; // Leaves start half a loop apart
        static readonly float[] DustPhase = { 0.25f, 0.75f }; // Dust starts between the leaves
        LevelController level; // Gives the explorer and the level layout

        internal bool IsBuilt => GetComponent<LevelController>() != null // Shares the level controller
            && views != null && views.All(view => view.column != null // Each column has its wind
                && view.leaves?.Length == 2 && view.leaves.All(leaf => leaf != null) // Each column has two leaves
                && view.dust?.Length == 2 && view.dust.All(speck => speck != null)) // Each column has two dust specks
            && columnFrames?.Length == 4 && columnFrames.All(frame => frame != null) // Has all four wind pictures
            && leafFrames?.Length == 2 && leafFrames.All(frame => frame != null) // Has both leaf pictures
            && dustFrames?.Length == 2 && dustFrames.All(frame => frame != null) // Has both dust pictures
            && windSound != null && windSound.clip != null; // The loop has its sound

        internal void Begin(LevelController owner) // Starts with the explorer outside every column
        { // Opens the setup
            level = owner; // Keeps its level controller
            level.explorer.UpdraftTop = float.NaN; // No updraft at the start
        } // Ends the setup

        void Start() // Connects once the level has started
        { // Opens the connection
            var owner = GetComponent<LevelController>(); // Finds the level on this object
            if (IsBuilt && owner.Run != null && views.Length == owner.Run.Level.Layout.Updrafts.Count) Begin(owner); // Pairs the views with the layout
            else Debug.LogError("The updraft part needs its views, pictures and sound. Run LandGrand > Build Levels."); // Reports an incomplete scene
        } // Ends the connection

        void Update() // Sets the hover height before the motor moves
        { // Opens the frame
            if (level == null) return; // Waits for a ready level
            var explorer = level.explorer; // The explorer to lift
            var run = level.Run; // This attempt
            float top = float.NaN; // Outside every column by default
            if (!run.IsPaused && run.Outcome == RunOutcome.Playing && !explorer.Frozen) // Only while playing
            { // Opens the column search
                var feet = explorer.Position; // The middle of the feet in level pixels
                foreach (var updraft in run.Level.Layout.Updrafts) // Checks each column
                    if (updraft.Contains(feet.x, feet.y)) { top = updraft.Top; break; } // The first column around the feet lifts the explorer
            } // Ends the column search
            explorer.UpdraftTop = top; // NaN leaves the motor as before
            if (float.IsNaN(top) || !LevelClock.Running) windSound.Stop(); // Silent on leaving and during a pause
            else level.PlaySfx(windSound, WindVolume); // Loops at half volume through the level's effect volume
        } // Ends the frame

        void LateUpdate() // Draws the wind at the current level time
        { // Opens the drawing
            if (level == null) return; // Waits for a ready level
            float now = LevelClock.Now; // Animation waits during a pause
            int frame = (int)(now * FramesPerSecond); // The picture step for this moment
            var updrafts = level.Run.Level.Layout.Updrafts; // The columns in layout order
            for (int i = 0; i < views.Length; i++) // Draws each column
            { // Opens one column
                var view = views[i]; // The pictures for this column
                var updraft = updrafts[i]; // The rule for this column
                view.column.sprite = columnFrames[frame % columnFrames.Length]; // The wind moves up 8 rows each step
                float height = updraft.Top + HeadRoom - BaseHeight; // The wind height above the rock crack
                for (int k = 0; k < 2; k++) // Moves the two leaves and the two dust specks
                { // Opens one pair
                    Rise(view.leaves[k], leafFrames, updraft.X + LeafX[k], height, now, LeafPhase[k], frame); // A leaf rises and spins
                    Rise(view.dust[k], dustFrames, updraft.X + DustX[k], height, now, DustPhase[k], frame); // A dust speck rises and flickers
                } // Ends one pair
            } // Ends one column
        } // Ends the drawing

        static void Rise(SpriteRenderer mote, Sprite[] frames, float x, float height, float now, float phase, int frame) // Moves one leaf or speck up the column on a loop
        { // Opens the rise
            float loopSeconds = height / Updraft.RiseSpeed; // Rises at the same speed as the explorer
            float share = (now / loopSeconds + phase) % 1f; // How far up this loop is, from 0 to 1
            mote.sprite = frames[(frame + (phase < 0.5f ? 0 : 1)) % frames.Length]; // Partners show different poses
            mote.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), Mathf.Round(BaseHeight + share * height)); // Whole pixels keep the art sharp
        } // Ends the rise
    } // Ends the part
} // Closes the play code
