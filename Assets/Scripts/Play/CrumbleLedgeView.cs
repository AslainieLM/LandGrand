using LandGrand.Rules; // Uses the break rule
using UnityEngine; // Uses sprites and time
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    [DefaultExecutionOrder(-100)] // Updates solidity before the explorer moves
    public class CrumbleLedgeView : DynamicSolid // Draws one breakable slab
    { // Opens the view
        public float x, top; // Saved placement
        public ExplorerMotor explorer; // Who stands on the slab
        public LevelController level; // Plays the break sound
        public SpriteRenderer art; // Slab picture
        public Sprite[] frames; // Whole, cracking and broken pictures
        public Sprite chip; // Falling piece picture
        public AudioClip breakSound; // Sound when the slab breaks
        float? touchedAt; // First touch until the slab returns
        CrumblePhase previous; // Detects the start of breaking
        public override Rect Bounds => new Rect(x, top - 10f, CrumbleLedge.Width, 10f); // Solid slab shape
        public override bool IsSolid => CrumbleLedge.IsSolid(touchedAt, LevelClock.Now); // Broken slabs do not block
        void Update() // Shows the current rule state
        { // Starts the frame
            float now = LevelClock.Now; // Shared level clock
            PreviousBounds = Bounds; // This slab never moves
            if (CrumbleLedge.StateAt(touchedAt, now) == CrumblePhase.Back) touchedAt = null; // Ready for another touch
            if (!touchedAt.HasValue && explorer.StandingOn == this && explorer.Grounded && !explorer.Frozen) touchedAt = now; // Starts the half-second warning once
            var phase = CrumbleLedge.StateAt(touchedAt, now); // Reads the rule
            bool breaking = phase == CrumblePhase.Gone && now - touchedAt.Value < CrumbleLedge.BreakSeconds + 0.15f; // Shows the break frame briefly
            int frame = phase == CrumblePhase.Cracking ? (CrumbleLedge.CrackProgress(touchedAt, now) < 0.5f ? 1 : 2) : breaking ? 3 : 0; // Picks the slab picture
            art.sprite = phase == CrumblePhase.Gone && !breaking ? null : frames[frame]; // Hides the broken slab
            float shake = phase == CrumblePhase.Cracking ? ((int)(now * 30f) % 2 == 0 ? -1f : 1f) : 0f; // Small warning shake
            art.transform.position = LevelGeometry.ToWorld(x + 16f + shake, top - 5f); // Only the art shakes
            if (phase == CrumblePhase.Gone && previous != phase) // Breaks once per touch
            { // Starts the break effect
                level.PlaySfx(breakSound); // Plays the crack
                for (int i = 0; i < 4; i++) FallingChipView.Spawn(chip, new Vector2(x + 4f + i * 8f, top), new Vector2((i - 1.5f) * 20f, 30f)); // Drops four chips
            } // Ends the break effect
            previous = phase; // Remembers the phase
        } // Ends the frame
    } // Ends the view
} // Ends the play code
