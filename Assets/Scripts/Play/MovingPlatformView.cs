using LandGrand.Rules; // Uses the travel rule
using UnityEngine; // Uses sprites and time
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    [DefaultExecutionOrder(-100)] // Moves before the explorer
    public class MovingPlatformView : DynamicSolid // Draws a moving solid
    { // Opens the view
        public float x0, x1, top; // Saved layout placement
        public SpriteRenderer art; // Platform picture
        public AudioClip landSound; // A soft sound when the explorer lands
        public LevelController level; // Reads the explorer and plays the sound
        const float LandVolume = 0.6f; // Plays the bump below full volume
        PlatformLanding landing; // Tracks changes to the explorer's support
        MovingPlatform rule; // Plain travel rule
        Rect bounds; // Current solid shape
        public override Rect Bounds => bounds; // Shape used by the motor
        public override bool IsSolid => true; // Always supports the explorer
        public float ArtCenterY => top - art.sprite.rect.height / 2f; // Puts the top row on the solid top
        internal bool IsBuilt => art != null && art.sprite != null && (landSound == null || level != null); // Checks the picture and optional sound link

        void Start() // Starts after the level controller is set up
        { // Starts setup
            if (IsBuilt) Begin(); // Uses the saved placement
            else // The scene is missing a view reference
            { // Handles an incomplete scene
                Debug.LogError("The moving platform needs its picture and sound link. Run LandGrand > Build Levels."); // Reports what to rebuild
                enabled = false; // Stops updates until the scene is rebuilt
            } // Ends the incomplete scene check
        } // Ends setup

        void Begin() // Reads the saved placement
        { // Starts the platform
            landing = default; // Starts with no rider
            rule = new MovingPlatform(x0, x1, top); // Builds the rule
            bounds = new Rect(rule.LeftAt(LevelClock.Now), top - 10f, MovingPlatform.Width, 10f); // Places the slab
            PreviousBounds = bounds; // No movement on the first frame
        } // Ends setup
        void Update() // Moves on the shared level clock
        { // Starts the frame
            PreviousBounds = bounds; // Keeps the old position for carrying
            bounds.x = rule.LeftAt(LevelClock.Now); // Reads the exact travel rule
            art.transform.position = LevelGeometry.ToWorld(Mathf.Round(bounds.center.x), ArtCenterY); // Moves the picture with its deck at the solid top
        } // Ends the frame
        void LateUpdate() // Checks support after the explorer moves
        { // Starts the landing check
            if (landSound == null) return; // Quiet platforms have no sound to track
            var explorer = level.explorer; // Reads the rider after movement
            bool riding = explorer.StandingOn == this; // The explorer has this platform as support
            bool canPlaySound = LevelClock.Running && !explorer.Frozen; // Plays only during live movement
            if (landing.Observe(riding, canPlaySound)) level.PlaySfx(landSound, LandVolume); // Plays once for this landing
        } // Ends the landing check
    } // Ends the view
} // Ends the play code
