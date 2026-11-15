using LandGrand.Rules; // Uses the gust rules
using UnityEngine; // Uses the shared clock
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    [DefaultExecutionOrder(-50)] // Sets the push before the motor moves
    public class WindWatcher : MonoBehaviour // One part of the level screen: gives the motor the current gust push
    { // Opens the part
        LevelController level; // The attempt and explorer
        internal bool IsBuilt => true; // Needs no built pieces; a level without gusts stays calm
        internal void Begin(LevelController owner) // Connects the part to its level
        { // Starts the connection
            level = owner; // Remember the level
        } // Ends the connection
        void Update() // Reads every placed gust
        { // Starts the frame
            if (level == null) return; // Not connected yet
            float push = 0f; // Calm by default
            var run = level.Run; // This attempt
            if (!run.IsPaused && run.Outcome == RunOutcome.Playing && !level.explorer.Frozen) // Only while playing
                foreach (var zone in run.Level.Layout.WindZones) push = Mathf.Min(push, zone.PushAt(level.explorer.Position.x, LevelClock.Now)); // Overlap never exceeds forty
            level.explorer.ExternalPushX = push; // Applies on ground and in air
        } // Ends the frame
    } // Ends the part
} // Ends the play code
