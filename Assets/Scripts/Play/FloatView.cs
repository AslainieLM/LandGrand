using LandGrand.Rules; // Gives the sink and rise rule
using UnityEngine; // Gives a solid shape and sprite renderers

namespace LandGrand.Play // Holds the moving level objects
{ // Opens the play code
    public sealed class FloatView : DynamicSolid // A bamboo raft that supports only from above
    { // Opens the raft
        public SpriteRenderer raft; // Draws the bamboo
        public SpriteRenderer ripple; // Draws the warning at the water line
        SinkingFloat rule; // Holds the layout and sink rule
        Rect bounds; // Holds the solid top for the motor
        SinkingFloatState motion; // Tracks the height and footing in the plain rules
        bool hadTop; // Remembers whether the raft held before this move
        const float RippleFrameSeconds = 0.15f; // Matches the short ground-effect frames

        public override Rect Bounds => bounds; // The raft with its current top
        public override bool IsSolid => rule != null && rule.Holds(bounds.yMax); // The real collision top holds until full depth
        internal override bool HadTop => hadTop; // A submerged raft has no old top to sweep
        internal bool IsBuilt => raft != null && ripple != null; // Needs both pictures

        internal void Begin(SinkingFloat placement, float now) // Starts the raft at the water line
        { // Opens the setup
            rule = placement; // Links to the catalog data
            motion = new SinkingFloatState(placement, now); // Starts the sink clock for this attempt
            bounds = new Rect(rule.X, rule.Top - 10f, SinkingFloat.Width, 10f); // Places its first solid top
            PreviousBounds = bounds; // A new raft has not moved
            hadTop = true; // The new raft starts dry and solid
            ripple.enabled = false; // The raft starts calm
        } // Ends the setup

        internal bool Step(float now, bool ridden, Sprite[] ripples) // Moves the raft and reports its splash
        { // Opens the raft update
            hadTop = IsSolid; // Remembers whether it held before moving
            PreviousBounds = bounds; // Saves the old position for carrying
            bounds.y = motion.Step(now, ridden) - bounds.height; // Reads the next height from the sink and rise rule
            bool shake = ridden && IsSolid; // Warns while the explorer is on the raft
            int frame = Mathf.FloorToInt(now / RippleFrameSeconds) % ripples.Length; // Changes the warning on the level clock
            float mid = bounds.center.x; // Keeps the collision shape steady while the art shakes
            raft.transform.position = LevelGeometry.ToWorld(mid + (shake ? (frame == 0 ? -1f : 1f) : 0f), Mathf.Round(bounds.center.y)); // Draws the shaken raft on whole pixels
            raft.enabled = IsSolid; // The fully submerged raft is hidden
            ripple.transform.position = LevelGeometry.ToWorld(mid, rule.Top - 2f); // Ripples stay at the water line
            ripple.sprite = ripples[frame]; // Shows the warning ripple frame
            ripple.enabled = shake; // Only warns while standing on it
            return hadTop && !IsSolid; // Reports the instant it goes under
        } // Ends the raft update
    } // Ends the raft
} // Ends the play code
