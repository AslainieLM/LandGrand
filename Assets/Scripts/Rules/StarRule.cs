namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>Works out the stars for a cleared level from the items collected.</summary>
    public static class StarRule
    {
        const int OneStarMaxPercent = 40; // 0 to 40% of the items gives 1 star

        /// <summary>Returns 1, 2 or 3 stars: up to 40% gives 1, above 40% gives 2, all items gives 3.</summary>
        public static int StarsFor(int collected, int total) // Reads the counts and returns the stars
        {
            if (total <= 0) return 1; // A level with no items still gives the minimum 1 star
            if (collected >= total) return 3; // All items collected gives 3 stars
            if (collected * 100 <= OneStarMaxPercent * total) return 1; // 40% or less gives 1 star (whole-number maths, no rounding)
            return 2; // Anything between gives 2 stars
        }
    }
}
