using System; // Gives signs and distances

namespace LandGrand.Rules // Holds the plain game rules
{ // Opens the rules
    public static class IceSlide // Changes walking speed on ice
    { // Opens the rule
        public const float SpeedUp = 160f; // Gains this much speed each second
        public const float SlowDown = 100f; // Sheds this much speed each second
        public const float SlideSoundThreshold = 20f; // The sound only starts above this walking speed

        public static bool IsSliding(float current, int direction) => Math.Abs(current) > SlideSoundThreshold && current * direction <= 0f; // Releasing or turning at speed can start a slide

        public static float Next(float current, float target, float dt) // Takes one step toward the held speed
        { // Starts the speed change
            bool faster = (current == 0f || Math.Sign(current) == Math.Sign(target)) && Math.Abs(target) > Math.Abs(current); // Speeds up only in the held direction
            float step = (faster ? SpeedUp : SlowDown) * dt; // Finds the largest change this frame
            float gap = target - current; // How far the speed must move
            return Math.Abs(gap) <= step ? target : current + Math.Sign(gap) * step; // Stops at the target
        } // Ends the speed change
    } // Ends the rule
} // Ends the rules
