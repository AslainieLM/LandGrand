using System; // Lets Unity save each log view
using System.Linq; // Makes one attempt state per log
using LandGrand.Rules; // Gives the log rules
using UnityEngine; // Gives scene parts, pictures and sound

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    public sealed class LogRoller : MonoBehaviour // Draws the logs and checks their hits
    { // Opens the part
        [Serializable] // Lets the builder save both pictures
        public struct View // Holds the pictures for one log
        { // Opens the view
            public SpriteRenderer warning; // Draws the dust at the spawn
            public SpriteRenderer log; // Draws the rolling log
        } // Ends the view

        public View[] views; // The views in layout order
        public Sprite[] warningFrames; // Both dust pictures
        public Sprite[] rollFrames; // All four roll pictures
        public AudioSource rollSound; // Loops while a rolling log is near
        LevelController level; // Gives the attempt and explorer
        RollingLogRun[] runs; // Stores the skip for each log
        float checkedAt = float.NaN; // Keeps both hits and art on the same skip decision
        const float RollFrameSeconds = 0.1f; // Matches the boulder turn rate
        const float HearingDistance = 260f; // Matches the boulder roll's range

        internal bool IsBuilt => GetComponent<LevelController>() != null // Shares the level controller object
            && views != null && views.All(view => view.warning != null && view.log != null) // Each log has a pair of renderers
            && warningFrames?.Length == 2 && warningFrames.All(frame => frame != null) // Both warning pictures are ready
            && rollFrames?.Length == 4 && rollFrames.All(frame => frame != null) // All four rolling pictures are ready
            && rollSound != null && rollSound.clip != null; // The loop has its sound

        internal void Begin(LevelController owner) // Starts the part with its own level
        { // Opens the setup
            level = owner; // Keeps the attempt
            runs = owner.Run.Level.Layout.Logs.Select(log => new RollingLogRun(log)).ToArray(); // Starts each independent roll
            checkedAt = float.NaN; // Lets the first frame check for skips
        } // Ends the setup

        void Start() // Connects after the controller makes the attempt
        { // Opens the connection
            var owner = GetComponent<LevelController>(); // Finds the controller on this object
            if (IsBuilt && owner.Run != null && views.Length == owner.Run.Level.Layout.Logs.Count) Begin(owner); // Starts when each log has a view
            else Debug.LogError("The log part needs its views, pictures and sound. Run LandGrand > Build Levels."); // Reports an incomplete scene
        } // Ends the connection

        void CheckRollStarts(float now) // Makes a single skip decision for each roll
        { // Opens the roll check
            if (checkedAt == now) return; // The hit check and drawings share this frame
            checkedAt = now; // Saves this clock time
            float x = level.explorer.Position.x; // Measures the distance from the explorer
            foreach (var run in runs) run.Update(now, x); // Checks each log at the start of its roll
        } // Ends the roll check

        internal bool Hits(Rect body) // Checks whether a live log touches the explorer
        { // Opens the hit check
            if (runs == null) return false; // Waits for a ready attempt
            float now = LevelClock.Now; // Uses the same time as the pictures
            CheckRollStarts(now); // Checks each skip before a hit
            return runs.Any(run => run.Hits(body.x, body.y, body.width, body.height, now)); // Any hit counts as a trap
        } // Ends the hit check

        void LateUpdate() // Draws the warning and the roll at the current level time
        { // Opens the frame
            if (runs == null) return; // Waits for a ready attempt
            float now = LevelClock.Now; // Holds the pictures while paused
            CheckRollStarts(now); // Uses the same skip state as the hit check
            bool audible = false; // Checks whether any active roll can be heard
            var rules = level.Run.Level.Layout.Logs; // Reads the authored routes
            float explorerX = level.explorer.Position.x; // Measures sound from the explorer
            for (int i = 0; i < runs.Length; i++) // Updates one picture pair per log
            { // Opens one log
                var rule = rules[i]; // Finds this log's route
                var view = views[i]; // Finds its warning and roll pictures
                float phase = rule.Phase(now); // Reads the current cycle
                view.warning.sprite = rule.IsWarning(now) ? warningFrames[phase < RollingLog.WarningSeconds / 2f ? 0 : 1] : null; // Switches dust halfway through the warning
                float x = runs[i].CurrentX(now); // A skipped or finished roll has no position
                if (float.IsNaN(x)) // There is no live log to draw
                { // Opens the hidden view
                    view.log.sprite = null; // Hides the log
                    continue; // Checks the next log
                } // Ends the hidden view
                int frame = (int)((phase - RollingLog.WarningSeconds) / RollFrameSeconds) % rollFrames.Length; // Turns at the boulder frame rate
                if (rule.Direction > 0) frame = rollFrames.Length - 1 - frame; // Reverses the frame order for right rolls
                view.log.sprite = rollFrames[frame]; // Draws this part of the turn
                view.log.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), view.log.sprite.rect.height / 2f); // Sits the log on the ground line
                audible |= Mathf.Abs(explorerX - x) <= HearingDistance; // Plays only when a rolling log is near
            } // Ends the log drawings
            if (!LevelClock.Running) rollSound.Pause(); // Holds the loop with the level clock
            else if (audible) level.PlaySfx(rollSound); // Uses the level's effect volume
            else rollSound.Stop(); // Stops when the log leaves or is out of range
        } // Ends the frame
    } // Ends the part
} // Ends the play code