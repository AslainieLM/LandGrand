using System; // Lets Unity save the two renderers for each pile
using System.Linq; // Checks the saved pictures and views
using LandGrand.Rules; // Gives the straw pile rules
using UnityEngine; // Gives the scene parts, pictures and sound

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    public sealed class StrawBounceWatcher : MonoBehaviour // Launches the explorer and shows the bounce
    { // Opens the part
        [Serializable] // Saves the pile views in the scene
        public struct View // Holds the pictures for one pile
        { // Opens the view
            public SpriteRenderer pile; // Shows the normal or squashed pile
            public SpriteRenderer bits; // Shows the flying straw
        } // Ends the view

        public View[] views; // One view per pile in layout order
        public Sprite normal; // The pile at rest
        public Sprite squash; // The pile pressed down
        public Sprite[] bitsFrames; // The two straw bits pictures
        public AudioClip bounceSound; // The straw bounce sound

        const float BounceVolume = 0.7f; // Scales the bounce volume as in spec section 5
        const float BitsFrameSeconds = 0.1f; // Each straw bits frame lasts a tenth of a second
        LevelController level; // Gives the explorer and the level layout
        float[] bouncedAt; // Keeps the time of the last bounce on each pile

        internal bool IsBuilt => GetComponent<LevelController>() != null // Shares the level controller
            && views != null && views.All(view => view.pile != null && view.bits != null) // Each pile has both renderers
            && normal != null && squash != null && bounceSound != null // Has the pile pictures and bounce sound
            && bitsFrames?.Length == 2 && bitsFrames.All(frame => frame != null); // Has both straw bits pictures

        internal void Begin(LevelController owner) // Starts with every pile at rest
        { // Opens the setup
            level = owner; // Keeps its level controller
            bouncedAt = new float[views.Length]; // Tracks the most recent bounce for each pile
            for (int i = 0; i < bouncedAt.Length; i++) bouncedAt[i] = float.NegativeInfinity; // No pile has bounced yet
        } // Ends the setup

        void Start() // Connects once the level has started
        { // Opens the connection
            var owner = GetComponent<LevelController>(); // Finds the level on this object
            if (IsBuilt && owner.Run != null && views.Length == owner.Run.Level.Layout.StrawPiles.Count) Begin(owner); // Pairs the views with the layout
            else Debug.LogError("The straw pile part needs its views, pictures and sound. Run LandGrand > Build Levels."); // Reports an incomplete scene
        } // Ends the connection

        void LateUpdate() // Checks the explorer after the motor lands
        { // Opens the frame
            if (level == null) return; // Waits for a ready level
            var explorer = level.explorer; // The explorer to bounce
            var feet = explorer.Position; // Reads the feet in level pixels
            var piles = level.Run.Level.Layout.StrawPiles; // The piles in layout order
            float now = LevelClock.Now; // Animation waits during a pause
            for (int i = 0; i < piles.Count; i++) // Checks and draws each pile
            { // Opens this pile's step
                if (LevelClock.Running && !explorer.Frozen && explorer.Grounded && piles[i].IsOnTop(feet.x, explorer.size.x / 2f, feet.y)) // Feet are grounded on the solid top
                { // Starts a bounce
                    explorer.Launch(StrawPile.LaunchSpeed); // Throws the explorer up with a double jump ready
                    level.PlaySfx(bounceSound, BounceVolume); // Plays the bounce at its own volume scale
                    bouncedAt[i] = now; // Starts both pictures on the same frame
                } // Ends the bounce
                float age = now - bouncedAt[i]; // Seconds since the last bounce here
                var view = views[i]; // The pictures for this pile
                view.pile.sprite = age < StrawPile.SquashSeconds ? squash : normal; // Returns to normal after the squash
                view.bits.enabled = age < 2f * BitsFrameSeconds; // Hides the bits after both frames
                if (view.bits.enabled) view.bits.sprite = bitsFrames[age < BitsFrameSeconds ? 0 : 1]; // Shows the first pose, then the second
            } // Ends this pile's step
        } // Ends the frame
    } // Ends the part
} // Closes the play code
