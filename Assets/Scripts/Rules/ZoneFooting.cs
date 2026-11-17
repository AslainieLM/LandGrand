using System; // Gives Func and Math

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    /// <summary>The shared "standing on a zone" check for ice and soft sand.</summary>
    public static class ZoneFooting // One check for both zone kinds
    { // Opens the check
        public const float FlatTolerance = 0.01f; // The feet must be this close to the ground line to count as flat ground

        /// <summary>True when the explorer stands on flat solid ground at the ground line inside a zone.</summary>
        public static bool IsStandingIn(bool grounded, bool onSolid, float feetY, float feetX, Func<float, bool> inZone) // The one shared check
            => grounded && !onSolid && Math.Abs(feetY) < FlatTolerance && inZone(feetX); // Ledges, boulders and moving solids do not count
    } // Ends the check
} // Ends the rules
