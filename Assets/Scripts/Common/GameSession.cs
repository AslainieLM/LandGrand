using LandGrand.Rules; // Gives access to Episode and Difficulty

namespace LandGrand // Game code lives here
{
    /// <summary>
    /// What the player picked on the way to a level: landform, difficulty and level number.
    /// It only lives while the game runs; it is not saved. The map, difficulty and trail screens fill it in.
    /// </summary>
    public static class GameSession // Choices carried from one screen to the next
    {
        /// <summary>The landform tapped on the map.</summary>
        public static Episode Episode { get; set; } = Episode.Mountains; // Mountains until the player picks one

        /// <summary>The difficulty card tapped.</summary>
        public static Difficulty Difficulty { get; set; } = Difficulty.Easy; // Easy until the player picks one

        /// <summary>The level stone tapped, starting at 1.</summary>
        public static int Level { get; set; } = 1; // Level 1 until the player picks one
    }
}
