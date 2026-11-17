using LandGrand.Rules; // Gives the vertical stand and travel rule
using UnityEngine; // Gives sprites, sound and solid bounds

namespace LandGrand.Play // Holds the moving level objects
{ // Opens the play code
    [DefaultExecutionOrder(-100)] // Moves the basket before the motor tries to carry its rider
    public sealed class BalloonView : DynamicSolid // A balloon with a one-way basket top
    { // Opens the balloon
        public LevelController level; // Gives the shared sound player and layout
        public int platformIndex; // Finds its platform in the layout
        public SpriteRenderer art; // The envelope and basket
        public SpriteRenderer flame; // The fire above the basket
        public Sprite[] flameFrames; // Two frames of the rising flame
        public AudioClip burner; // The sound at the start of a climb
        MovingPlatform rule; // The vertical platform for this balloon
        Rect bounds; // The current basket top
        bool wasRising; // Remembers the last direction while running
        const float HearingPixels = 240f; // Plays the burner within half the camera's width
        const float FlameFrameSeconds = 0.1f; // Flickers ten times per second

        public override Rect Bounds => bounds; // Returns only the basket shape
        public override bool IsSolid => rule != null; // The basket holds once the layout is loaded
        internal bool IsBuilt => art != null && flame != null && burner != null && flameFrames != null && flameFrames.Length == 2 && flameFrames[0] != null && flameFrames[1] != null; // Needs the art and the burner sound

        void Start() // Starts the vertical platform
        { // Opens the start check
            if (level == null || level.Run == null) return; // The controller handles an old scene
            var platforms = level.Run.Level.Layout.MovingPlatforms; // Reads the layout routes
            if (!IsBuilt || platformIndex < 0 || platformIndex >= platforms.Count || !platforms[platformIndex].IsVertical) // Checks the route and all saved pieces
            { // Opens the missing-part message
                Debug.LogError("The balloon is missing its vertical route, pictures or sound. Run LandGrand > Build Levels."); // Gives the repair command
                return; // Waits for a rebuilt scene
            } // Ends the missing-part check
            Begin(level); // Connects the vertical platform
        } // Ends the start check

        internal void Begin(LevelController owner) // Connects the balloon to the attempt
        { // Opens setup
            level = owner; // Keeps the owner for sound and pause checks
            rule = owner.Run.Level.Layout.MovingPlatforms[platformIndex]; // Reads the authored travel rule
            bounds = new Rect(rule.X0, rule.TopAt(LevelClock.Now) - 10f, MovingPlatform.Width, 10f); // A ten-pixel basket under the top
            PreviousBounds = bounds; // No movement on the first frame
            Draw(LevelClock.Now, rule.IsRisingAt(LevelClock.Now)); // Places the picture at its starting phase
        } // Ends setup

        void Update() // Moves the basket and toggles the burner
        { // Opens the frame
            if (rule == null || level.Run.IsPaused || level.Run.Outcome != RunOutcome.Playing) return; // Skips pending travel when the game waits
            float now = LevelClock.Now; // Travel stops while the level waits
            PreviousBounds = bounds; // The motor needs the old height to carry its rider
            bounds.y = rule.TopAt(now) - bounds.height; // Moves up or down without moving along the trail
            bool rising = rule.IsRisingAt(now); // Finds this part of the travel loop
            Draw(now, rising); // Aligns the flame and basket art to the solid top
            if (!LevelClock.Running) return; // Paused frames leave the burner edge alone
            if (rising && !wasRising && Mathf.Abs(bounds.center.x - level.explorer.Position.x) <= HearingPixels) level.PlaySfx(burner); // Hears the start of a climb nearby
            wasRising = rising; // Counts each climb once
        } // Ends the frame

        void Draw(float time, bool rising) // Keeps the basket art in step with the collision top
        { // Opens the art update
            float y = Mathf.Round(bounds.yMax); // Draws the top on a whole pixel
            art.transform.position = LevelGeometry.ToWorld(bounds.center.x, y + 30f); // The basket top is ten pixels from the bottom of the eighty-pixel sprite
            flame.transform.position = LevelGeometry.ToWorld(bounds.center.x, y + 6f); // Hangs the flame just above the basket
            flame.enabled = rising; // The burner is off during descent
            if (rising) flame.sprite = flameFrames[Mathf.FloorToInt(time / FlameFrameSeconds) % flameFrames.Length]; // Flickers only while rising
        } // Ends the art update
    } // Ends the balloon
} // Ends the play code
