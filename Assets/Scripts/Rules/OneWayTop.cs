namespace LandGrand.Rules // Plain rules without Unity
{ // Opens the rules
    public static class OneWayTop // Checks contact with a top that supports only from above
    { // Opens the rule
        const float ContactTolerance = 0.01f; // Keeps the motor's existing landing tolerance

        public static bool Crosses(float feetBefore, float feetAfter, float topBefore, float topAfter, bool hadTop = true) // Checks the feet and moving top over one step
        { // Opens the rule
            float approachTop = hadTop ? topBefore : topAfter; // Uses the new top if it just became solid
            float tolerance = hadTop && topBefore == topAfter ? ContactTolerance : 0f; // Keeps the old tolerance for a steady top only
            return feetBefore >= approachTop - tolerance // The feet started above a solid top
                && feetAfter <= topAfter // The feet would reach or pass the current top
                && (feetAfter <= feetBefore || (hadTop && topAfter > topBefore)); // A solid rising top can also catch up with the feet
        } // Ends the rule
    } // Ends the rule
} // Ends the rules
