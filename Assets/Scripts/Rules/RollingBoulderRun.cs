using System; // Gives the distance from the spawn

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public sealed class RollingBoulderRun // Keeps the skip decision for one placed boulder
    { // Opens the attempt state
        readonly RollingBoulder boulder; // The placed boulder
        int? decidedCycle; // Which roll start has already been checked
        bool skipped; // True when the explorer was too close at that start

        public RollingBoulderRun(RollingBoulder boulder) => this.boulder = boulder; // Uses this boulder's placement

        public void Update(float t, float explorerX) // Checks for the start of a new roll
        { // Starts the update
            int cycle = boulder.CycleIndex(t); // Finds the active cycle
            if (decidedCycle == cycle || boulder.IsWarning(t)) return; // Waits for the first rolling frame
            decidedCycle = cycle; // Makes one decision for this cycle
            skipped = Math.Abs(explorerX - boulder.Spawn) < RollingBoulder.SkipDistance; // Gives the explorer room
        } // Ends the update

        public float CurrentX(float t) => decidedCycle == boulder.CycleIndex(t) && skipped ? float.NaN : boulder.XAt(t); // A skipped roll has no position
        public bool Hits(float left, float bottom, float width, float height, float t) => RollingBoulder.HitsAt(CurrentX(t), left, bottom, width, height); // A skipped roll cannot hurt
    } // Ends the attempt state
} // Ends the rules
