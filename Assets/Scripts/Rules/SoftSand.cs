using System.Collections.Generic; // Gives IReadOnlyList
using System.Linq; // Gives Any

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public static class SoftSand // Slows walking on soft sand (decision D-066, spec section 5)
    { // Opens the rule
        public const float WalkShare = 0.6f; // Walking keeps 60% of its speed on sand
        public const float StepSeconds = 0.3f; // One step sound and one puff every 0.3 seconds while walking on sand
        public const float PuffFrameSeconds = 0.15f; // Each of the two puff pictures shows for this long

        public static float WalkSpeed(float walk, bool onSand) => onSand ? walk * WalkShare : walk; // Only walking on sand is slower; jumps and air speed are not changed

        public static bool IsOnSand(bool grounded, bool onSolid, float feetY, float feetX, IReadOnlyList<SandZone> zones) // Sand counts only on flat ground inside a zone
            => ZoneFooting.IsStandingIn(grounded, onSolid, feetY, feetX, x => zones.Any(zone => zone.Contains(x))); // Same flat-ground test as ice; ledges, boulders and moving solids are not sand
    } // Ends the rule
} // Ends the rules
