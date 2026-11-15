using System.Linq; // Sets up one attempt state per boulder
using LandGrand.Rules; // Gives the timing and skip rules
using UnityEngine; // Gives the level part and body box

namespace LandGrand.Play // Holds the level parts
{ // Opens the play code
    public sealed class RollingBoulderWatcher : MonoBehaviour // Shares one roll state between the pictures and traps
    { // Opens the part
        LevelController level; // Gives the current attempt and explorer
        RollingBoulderRun[] runs; // Keeps the skip decision for each boulder
        float checkedAt = float.NaN; // Tracks the last clock time checked

        internal bool IsBuilt => GetComponent<LevelController>() != null; // This part goes on the level controller object
        internal LevelController Level => level; // Gives the views their level
        internal void Begin(LevelController owner) // Connects the boulders to this attempt
        { // Starts the setup
            level = owner; // Keeps the level
            runs = owner.Run.Level.Layout.RollingBoulders.Select(boulder => new RollingBoulderRun(boulder)).ToArray(); // Starts the independent cycles
        } // Ends the setup

        void Start() // Waits until the level has made its attempt
        { // Starts the connection
            var owner = GetComponent<LevelController>(); // Finds the level that owns this part
            if (IsBuilt && owner.Run != null) Begin(owner); // Starts only on a ready level
        } // Ends the connection

        void CheckRollStarts(float now) // Makes the skip decision once for this clock time
        { // Starts the roll check
            if (checkedAt == now) return; // The view and traps share the same decision
            checkedAt = now; // Remembers this frame's time
            foreach (var run in runs) run.Update(now, level.explorer.Position.x); // Tests the explorer at the start
        } // Ends the roll check

        internal float CurrentX(int index, float now) // Reports where to draw one rolling boulder
        { // Starts the position check
            CheckRollStarts(now); // Covers frames when traps are not checked
            return runs[index].CurrentX(now); // Skipped rolls have no position
        } // Ends the position check

        internal bool Hits(Rect body) // Checks all live boulder boxes
        { // Starts the trap check
            if (runs == null) return false; // Waits for the attempt
            float now = LevelClock.Now; // Uses the same time as the pictures
            CheckRollStarts(now); // Checks the skip before allowing a hit
            return runs.Any(run => run.Hits(body.x, body.y, body.width, body.height, now)); // Any hit counts as a trap
        } // Ends the trap check
    } // Ends the part
} // Ends the play code
