namespace LandGrand.Rules // Holds rules without Unity types
{ // Opens the rules
    public struct PlatformLanding // Tracks one platform's rider between frames
    { // Opens the landing rule
        bool wasRiding; // Remembers support from the previous frame

        public bool Observe(bool riding, bool canPlaySound) // Checks if a new landing should make a sound
        { // Starts the landing check
            bool landed = riding && !wasRiding; // Only new support counts as a landing
            wasRiding = riding; // Tracks support during freezes so resuming cannot replay the bump
            return landed && canPlaySound; // Plays only for a landing during live play
        } // Ends the landing check
    } // Ends the landing rule
} // Closes the rules
