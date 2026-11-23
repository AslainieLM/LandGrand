using System.Linq; // Checks the placed bat sprites
using LandGrand.Rules; // Gives the gentle bat touch rule
using UnityEngine; // Gives the view, sound and sprite colour

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    public sealed class BatFlock : MonoBehaviour // Moves the bats and reacts to gentle contact
    { // Opens the flock
        public SpriteRenderer[] bats; // Bat renderers in layout order
        public Sprite[] frames; // The three wing pictures
        public AudioClip flap; // The soft push sound
        public float artLift; // Pixels the picture sits above the rule's centre; the touch box stays on the rule (D-071)
        public bool artFacesLeft; // True when the frames face left, so the flip is reversed
        LevelController level; // Gives the explorer and sound player
        float[] lastPush; // When each bat last touched the explorer
        Color bodyColor; // The explorer's colour before a flash
        bool flashing; // True while this part owns the flash colour
        const float WingFrameSeconds = 0.12f; // Keeps the wing changes easy to see
        const float FlashFrameSeconds = 0.05f; // Blinks twice during one push

        internal bool IsBuilt => GetComponent<LevelController>() != null && bats != null && bats.All(b => b != null) && frames != null && frames.Length >= 2 && frames.All(s => s != null) && flap != null; // Needs the frames, including the resting pose, and the sound

        internal void Begin(LevelController owner) // Connects the bats to the attempt
        { // Opens the setup
            level = owner; // Saves this attempt
            lastPush = Enumerable.Repeat(float.NegativeInfinity, bats.Length).ToArray(); // Every bat starts ready to push
        } // Ends the setup

        void Start() // Starts the optional part once the attempt is ready
        { // Opens the setup
            var owner = GetComponent<LevelController>(); // Finds this level's controller
            if (owner == null || owner.Run == null) return; // The controller handles an old scene
            if (!IsBuilt || bats.Length != owner.Run.Level.Layout.Bats.Count) // Checks the scene against the layout
            { // Opens the missing-part message
                Debug.LogError("The bat flock is missing its bats, pictures or sound. Run LandGrand > Build Levels."); // Gives the repair command
                return; // Waits for a rebuilt scene
            } // Ends the missing-part check
            Begin(owner); // Reads the level data
        } // Ends the setup

        void LateUpdate() // Draws the loops and checks contact after movement
        { // Opens the bat update
            if (level == null || level.Run.IsPaused || level.Run.Outcome != RunOutcome.Playing) return; // Keeps poses and the flash still while the game waits
            var explorer = level.explorer; // Reads the motor after it moved
            var feet = new Spot(explorer.Position.x, explorer.Position.y); // Uses the middle of the feet
            bool canTouch = LevelClock.Running && !explorer.Frozen && !explorer.BatPushActive; // Makes one push at a time while playing
            float time = LevelClock.Now; // Loops and rests freeze when paused
            var rules = level.Run.Level.Layout.Bats; // Gets all the loops
            for (int i = 0; i < bats.Length; i++) // Draws and checks each bat
            { // Opens one bat
                var rule = rules[i]; // Reads this bat's loop
                var position = rule.PositionAt(time); // Gets this bat's exact center
                bats[i].transform.position = LevelGeometry.ToWorld(Mathf.Round(position.X), Mathf.Round(position.Y) + artLift); // Draws it on whole pixels, raised by the theme's lift
                bats[i].flipX = (rule.PositionAt(time + WingFrameSeconds).X < position.X) != artFacesLeft; // Faces along the loop; left-facing frames flip the other way
                bool rested = Bat.CanPush(time - lastPush[i]); // A recent contact still has a cooldown
                bats[i].sprite = rested ? frames[Mathf.FloorToInt((time + rule.Phase) / WingFrameSeconds) % frames.Length] : frames[1]; // A resting bat glides with its wings out
                if (!canTouch || !rested || !Bat.Touches(feet, position)) continue; // Leaves this explorer alone
                int direction = Bat.PushDirection(feet.X, position.X, explorer.input.Direction); // Pushes away from the bat
                explorer.PushFromBat(direction); // Leaves checkpoint and jump state alone
                if (!flashing) bodyColor = explorer.body.color; // Saves the colour to restore
                flashing = true; // Starts the body flash
                lastPush[i] = time; // Starts this bat's rest time
                canTouch = false; // A second bat waits until the first push ends
                level.PlaySfx(flap); // Plays the soft wings
            } // Ends one bat
            ShowFlash(time, explorer); // Draws or clears the short flash
        } // Ends the bat update

        void ShowFlash(float now, ExplorerMotor explorer) // Changes the explorer's sprite during the push
        { // Opens the flash
            if (!flashing) return; // Leaves the normal sprite colour alone
            if (!explorer.BatPushActive) // The push finished or the explorer was moved
            { // Opens the reset
                explorer.body.color = bodyColor; // Restores the normal colour
                flashing = false; // Stops blinking
                return; // The flash is done
            } // Ends the reset
            var color = bodyColor; // Keeps the explorer's tint
            int frame = Mathf.FloorToInt((now - lastPush.Max()) / FlashFrameSeconds); // Counts short blinks on the level clock
            color.a *= frame % 2 == 0 ? 0.4f : 1f; // Alternates dim and full colour
            explorer.body.color = color; // Shows the blink
        } // Ends the flash
    } // Ends the flock
} // Ends the play code
