using LandGrand.Rules; // Uses the travel rule
using UnityEngine; // Uses sprites and time
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    [DefaultExecutionOrder(-100)] // Moves before the explorer
    public class MovingPlatformView : DynamicSolid // Draws a moving solid
    { // Opens the view
        public float x0, x1, top; // Saved layout placement
        public SpriteRenderer art; // Platform picture
        MovingPlatform rule; // Plain travel rule
        Rect bounds; // Current solid shape
        public override Rect Bounds => bounds; // Shape used by the motor
        public override bool IsSolid => true; // Always supports the explorer
        void Start() // Reads the saved placement
        { // Starts setup
            rule = new MovingPlatform(x0, x1, top); // Builds the rule
            bounds = new Rect(rule.LeftAt(LevelClock.Now), top - 10f, MovingPlatform.Width, 10f); // Places the slab
            PreviousBounds = bounds; // No movement on the first frame
        } // Ends setup
        void Update() // Moves on the shared level clock
        { // Starts the frame
            PreviousBounds = bounds; // Keeps the old position for carrying
            bounds.x = rule.LeftAt(LevelClock.Now); // Reads the exact travel rule
            art.transform.position = LevelGeometry.ToWorld(Mathf.Round(bounds.center.x), top - 5f); // Draws on whole pixels
        } // Ends the frame
    } // Ends the view
} // Ends the play code
