using System; // Gives the depth limits

namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public sealed class SinkingFloat // One raft on the water
    { // Opens the raft
        public const float Width = 48f; // Width of the one-way top
        public const float Wait = 0.6f; // Standing time before it sinks
        public const float SinkSpeed = 8f; // Pixels down each second
        public const float SunkDepth = 16f; // Depth that stops the raft from holding
        public const float RiseDelay = 1.5f; // Time off the raft before it rises
        public const float RiseSpeed = 16f; // Pixels up each second
        public const float PoolDepth = 16f; // Depth of a safe practice pool

        public float X { get; } // Left edge of the raft
        public float Top { get; } // Height when the raft is up
        public float WaterX { get; } // Left edge of the pit or pool below it

        public SinkingFloat(float x, float top, float waterX) // Places the raft at its water line
        { // Opens the placement
            X = x; // Saves the raft's left edge
            Top = top; // Saves the water line
            WaterX = waterX; // Links the raft to its pit or pool
        } // Ends the placement

        public float TopAt(float standingSeconds, float leftSeconds, float startingDepth = 0f) // Finds the top after standing and then leaving
        { // Opens the height calculation
            float timeToSink = Wait + (SunkDepth - startingDepth) / SinkSpeed; // Time until the raft reaches full depth
            float depth = standingSeconds >= timeToSink ? SunkDepth : startingDepth + Math.Max(0f, standingSeconds - Wait) * SinkSpeed; // Holds still first and then sinks
            float rise = Math.Max(0f, leftSeconds - RiseDelay) * RiseSpeed; // Waits for the explorer to leave before rising
            return Top - Math.Max(0f, depth - rise); // Stops rising at the water line
        } // Ends the height calculation

        public bool Holds(float top) => top > Top - SunkDepth; // The raft cannot carry anyone at full depth
    } // Ends the raft

    public sealed class SinkingFloatState // Tracks one raft during a level attempt
    { // Opens the attempt state
        readonly SinkingFloat raft; // Holds the unchanged layout and height rule
        float stateSince; // Time of the last landing or departure
        float depthAtChange; // Depth at the start of this state
        float previousTime; // Time of the last step
        bool wasRidden; // Whether the previous step carried the explorer
        public float Top { get; private set; } // Latest height of the raft

        public SinkingFloatState(SinkingFloat placement, float now) // Starts a fresh raft at the water line
        { // Opens the setup
            raft = placement; // Keeps the laid-out raft
            Top = raft.Top; // Starts at the water line
            previousTime = stateSince = now; // Records the first step
        } // Ends the setup

        public float Step(float now, bool ridden) // Finds the new height after this step
        { // Opens the float step
            if (ridden != wasRidden) // The explorer landed or left after the last step
            { // Opens the timer change
                stateSince = previousTime; // Starts from the motor step that changed the footing
                depthAtChange = raft.Top - Top; // Keeps the depth already reached
                wasRidden = ridden; // Saves the new footing state
            } // Ends the timer change
            float elapsed = now - stateSince; // Time spent in the current state
            Top = raft.TopAt(ridden ? elapsed : 0f, ridden ? 0f : elapsed, depthAtChange); // Runs the sink or rise from that depth
            previousTime = now; // Saves the clock for the next step
            return Top; // Gives the view its current height
        } // Ends the float step
    } // Ends the attempt state
} // Ends the rules
