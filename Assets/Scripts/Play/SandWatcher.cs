using LandGrand.Rules; // Uses the soft sand rules
using UnityEngine; // Gives sounds, pictures and scene objects

namespace LandGrand.Play // Level play parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Finds the sand before the motor moves
    public sealed class SandWatcher : MonoBehaviour // One part of the level screen: slows the explorer on soft sand and shows its steps
    { // Opens the part
        public AudioClip stepSound; // The crunch played on each step in the sand
        public Sprite[] puffFrames; // The two sand puff pictures
        public SpriteRenderer puff; // Shows the puff at the feet

        LevelController level; // The attempt and explorer
        int lastStep = -1; // The last step of this walk on sand, or -1 when not walking on sand
        float walkStart = -1f; // Level time when this walk on sand began
        float puffStart = -1f; // Level time when the last puff began, or -1 for none

        internal bool IsBuilt => GetComponent<LevelController>() != null && puff != null && puffFrames != null && puffFrames.Length == 2; // Shares a controller and has its pictures
        internal void Begin(LevelController owner) => level = owner; // Connects the part to its level
        void Start() // Connects the optional part after the level begins
        { // Starts the setup
            if (IsBuilt) Begin(GetComponent<LevelController>()); // Uses its own level controller
            else Debug.LogError("The sand part is missing its pictures. Run LandGrand > Build Levels."); // Say what to do
        } // Ends the setup

        void Update() // Finds the sand under the explorer, then plays its steps and puffs
        { // Starts the frame
            if (level == null || level.Run == null) return; // Waits for the attempt
            var explorer = level.explorer; // The explorer
            var feet = explorer.Position; // Middle of the feet, read before the motor moves
            bool onSand = SoftSand.IsOnSand(explorer.Grounded, explorer.StandingOn != null, feet.y, feet.x, level.Run.Level.Layout.SandZones); // The rule decides
            explorer.OnSand = onSand; // The motor walks slower this frame
            bool held = explorer.input.Direction != 0; // An arrow is held down
            float now = LevelClock.Now; // The level's own time, which stops while paused
            bool running = LevelClock.Running && !explorer.Frozen; // The game is moving, not paused or in a banner
            if (!onSand || !held) lastStep = -1; // Stopping means the next walk on sand starts with a step at once
            else if (running) // A walk on sand while the game runs
            { // Starts the walk check
                if (lastStep < 0) // The walk on sand has just started
                { // Starts the first step
                    walkStart = now; // The walk starts now
                    lastStep = 0; // Step 0 is this one
                    PlayStep(now); // First step and its puff start now
                } // Ends the first step
                else // The walk is going on
                { // Starts the later steps
                    int step = Mathf.FloorToInt((now - walkStart) / SoftSand.StepSeconds); // Steps counted from the start of this walk
                    if (step > lastStep) // A new step has begun
                    { // Starts the new step
                        lastStep = step; // Remember this step
                        PlayStep(walkStart + step * SoftSand.StepSeconds); // Each step and puff is on its own time, from the walk's start
                    } // Ends the new step
                } // Ends the later steps
            } // Ends the walk check
            float age = now - puffStart; // Seconds since the last puff began
            bool showPuff = onSand && LevelClock.Running && puffStart >= 0f && age < 2f * SoftSand.PuffFrameSeconds; // The two pictures play only on sand while the game runs
            puff.enabled = showPuff; // Shows or hides the puff
            if (!showPuff) return; // Nothing more to draw
            puff.sprite = puffFrames[age < SoftSand.PuffFrameSeconds ? 0 : 1]; // First picture, then the second
            puff.transform.position = LevelGeometry.ToWorld(Mathf.Round(feet.x), Mathf.Round(feet.y) + 2f); // At the feet, like the dust puff
        } // Ends the frame

        void PlayStep(float puffBegins) // One crunch and one puff that begins at the given level time
        { // Starts the step
            if (stepSound != null) level.PlaySfx(stepSound, 0.5f); // The crunch, quieter than the other effects
            puffStart = puffBegins; // The puff's two pictures are timed from this moment
        } // Ends the step
    } // Ends the part
} // Ends the play code
