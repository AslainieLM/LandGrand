using LandGrand.Rules; // Gives the rolling boulder rules
using UnityEngine; // Gives sprites and sound

namespace LandGrand.Play // Holds the level object views
{ // Opens the play code
    public sealed class RollingBoulderView : MonoBehaviour // Shows one boulder warning, roll and dust puff
    { // Opens the view
        public RollingBoulderWatcher watcher; // Shares the roll and skip decision with traps
        public int index; // This boulder's place in the level list
        public SpriteRenderer warning, rock, dust; // Draw the three parts of the cycle
        public Sprite[] warningFrames, rollFrames, dustFrames; // Use the supplied pictures
        public AudioClip rumble; // Plays once near a warning
        public AudioSource rollSound; // Loops close to a rolling boulder
        AudioSource rumblePlayer; // Pauses the warning without affecting other sounds
        int? heardCycle; // The warning already heard
        bool wasRolling; // Finds the end of a roll
        float dustAt = float.NegativeInfinity; // When the dust puff began
        const float RollFrameSeconds = 0.1f; // Turns the boulder every tenth of a second
        const float DustFramesPerSecond = 14f; // Matches the explorer dust speed

        void Awake() // Makes the level-clocked warning player
        { // Starts the sound setup
            rumblePlayer = gameObject.AddComponent<AudioSource>(); // Gives the warning its own sound source
            rumblePlayer.playOnAwake = false; // Does not rumble until the explorer is near
            rumblePlayer.clip = rumble; // Uses the builder's warning recording
        } // Ends the sound setup

        void LateUpdate() // Draws the same clock time that checked for hits
        { // Starts the frame
            var level = watcher.Level; // Gets the shared attempt
            if (level == null) return; // Waits until the watcher begins
            var rule = level.Run.Level.Layout.RollingBoulders[index]; // Finds the placed boulder
            float now = LevelClock.Now; // Stops pictures when the level waits
            float phase = rule.Phase(now); // Time in the current cycle
            bool warningNow = rule.IsWarning(now); // The boulder is still off the trail
            warning.sprite = warningNow ? warningFrames[phase < 0.4f ? 0 : 1] : null; // Changes the warning halfway through
            float x = watcher.CurrentX(index, now); // Only an allowed roll has a position
            bool rolling = !float.IsNaN(x); // True until the middle passes the end
            rock.sprite = rolling ? rollFrames[Mathf.Max(0, (int)((phase - RollingBoulder.WarningSeconds) / RollFrameSeconds)) % rollFrames.Length] : null; // Turns the crack as it rolls
            if (rolling) rock.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), 8f); // The 16-pixel picture sits on ground
            if (wasRolling && !rolling) dustAt = now; // Shows dust only after an actual roll
            wasRolling = rolling; // Remembers whether the boulder was visible
            int dustFrame = Mathf.FloorToInt((now - dustAt) * DustFramesPerSecond); // Reads the dust animation on the level clock
            dust.sprite = dustFrame >= 0 && dustFrame < dustFrames.Length ? dustFrames[dustFrame] : null; // Hides the finished dust
            PlaySounds(level, rule, now, warningNow, x); // Updates the sounds for this explorer
        } // Ends the frame

        void PlaySounds(LevelController level, RollingBoulder rule, float now, bool warningNow, float x) // Plays the warning and nearby roll
        { // Starts the sound check
            if (!LevelClock.Running) // Both parts of the boulder sound wait while the game waits
            { // Starts the pause
                rumblePlayer.Pause(); // Holds the warning at its current point
                rollSound.Pause(); // Holds the loop at its current point
                return; // No sounds start while the clock is held
            } // Ends the pause
            rumblePlayer.UnPause(); // Continues a warning after play resumes
            float explorerX = level.explorer.Position.x; // The distance starts at the explorer
            int cycle = rule.CycleIndex(now); // Lets a new warning be heard once
            if (warningNow && heardCycle != cycle && Mathf.Abs(explorerX - rule.Spawn) <= 300f) // A nearby warning still sounding
            { // Starts the rumble
                heardCycle = cycle; // Remembers this warning
                rumblePlayer.volume = GameSettings.SfxVolume; // Uses the player's effect volume
                rumblePlayer.Play(); // Plays the warning once
            } // Ends the rumble
            if (float.IsNaN(x) || Mathf.Abs(explorerX - x) > 260f) rollSound.Stop(); // Silent when the rock is gone or too far away
            else // The explorer can hear an active roll
            { // Starts the audible roll
                rollSound.volume = GameSettings.SfxVolume; // Follows the player's sound volume
                rollSound.UnPause(); // Continues the loop after a pause
                if (!rollSound.isPlaying) rollSound.Play(); // Starts a loop when first heard
            } // Ends the audible roll
        } // Ends the sound check
    } // Ends the view
} // Ends the play code
