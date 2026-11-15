using System; // Gives access to Func
using System.Collections.Generic; // Gives IReadOnlyList and HashSet
using System.Linq; // Gives Any, All, Where and Count

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>The colour of a badge: bronze for first steps, silver for bigger goals, gold for the hardest (D-060).</summary>
    public enum BadgeTier { Bronze, Silver, Gold } // Badge colours

    /// <summary>
    /// One achievement (CONTEXT.md): a goal with a name, a hint on how to earn it and a badge colour.
    /// It is worked out from saved progress only, so it never needs its own save field and is never lost (D-060).
    /// </summary>
    public sealed class Achievement // One badge's rule
    {
        public string Id { get; } // Short fixed name, also the badge picture name after "badge_"
        public string Name { get; } // Name shown under the badge
        public string HowToEarn { get; } // Hint shown on the badge card, also while it is locked
        public BadgeTier Tier { get; } // Bronze, silver or gold
        public bool IsDraft { get; } // True until the names are reviewed, like the lesson text
        readonly Func<PlayerProgress, bool> earned; // The rule that decides it

        public Achievement(string id, string name, string howToEarn, BadgeTier tier, Func<PlayerProgress, bool> earned) // Makes an achievement
        {
            Id = id; Name = name; HowToEarn = howToEarn; Tier = tier; IsDraft = true; this.earned = earned; // Remember everything
        }

        /// <summary>True when the progress meets the goal.</summary>
        public bool IsEarned(PlayerProgress progress) => progress != null && earned(progress); // Ask the rule
    }

    /// <summary>Every achievement in the game, in the order the Profile shows them (D-060).</summary>
    public static class AchievementCatalog // Shared list of badges
    {
        /// <summary>The starter badges. Names are drafts; later landforms add their own.</summary>
        public static IReadOnlyList<Achievement> All { get; } = new[] // In Profile order
        {
            new Achievement("first_steps", "First Steps", "Clear your first level.", BadgeTier.Bronze, p => p.ClearedLevelCount >= 1), // Any cleared level
            new Achievement("rock_hound", "Rock Hound", "Find your first rock.", BadgeTier.Bronze, p => ItemCatalog.All.Any(k => k.IsRock && p.HasFoundKind(k.Id))), // Any rock found
            new Achievement("crystal_finder", "Crystal Finder", "Find your first mineral.", BadgeTier.Bronze, p => ItemCatalog.All.Any(k => !k.IsRock && p.HasFoundKind(k.Id))), // Any mineral found
            new Achievement("star_explorer", "Star Explorer", "Get 3 stars on any level.", BadgeTier.Silver, p => AllLevels().Any(l => p.BestStars(l.e, l.d, l.n) >= 3)), // Three stars somewhere
            new Achievement("volcano_visitor", "Volcano Visitor", "Clear Mountains Easy 2.", BadgeTier.Silver, p => p.IsCleared(Episode.Mountains, Difficulty.Easy, 2)), // Volcano Trail
            new Achievement("summit_climber", "Summit Climber", "Clear Mountains Easy 3.", BadgeTier.Silver, p => p.IsCleared(Episode.Mountains, Difficulty.Easy, 3)), // Folded Peaks
            new Achievement("perfect_trail", "Perfect Trail", "Get 3 stars on every Mountains Easy level.", BadgeTier.Gold, p => Enumerable.Range(1, PlayerProgress.LevelCount(Difficulty.Easy)).All(n => p.BestStars(Episode.Mountains, Difficulty.Easy, n) >= 3)), // Every Easy level perfect
            new Achievement("mountain_collector", "Mountain Collector", "Find every Mountains rock and mineral.", BadgeTier.Gold, p => ItemCatalog.All.Where(k => k.HomeLandform == Episode.Mountains).All(k => p.HasFoundKind(k.Id))), // The whole Mountains shelf
            new Achievement("philippine_peaks", "Philippine Peaks", "Clear Mountains Normal 1 to 4.", BadgeTier.Silver, p => Enumerable.Range(1, 4).All(n => p.IsCleared(Episode.Mountains, Difficulty.Normal, n))), // Four Philippine peaks
            new Achievement("top_of_the_world", "Top of the World", "Clear Mount Everest (Mountains Normal 5).", BadgeTier.Gold, p => p.IsCleared(Episode.Mountains, Difficulty.Normal, 5)), // Everest cleared
            new Achievement("rock_scholar", "Rock Scholar", "Find 15 different rocks and minerals.", BadgeTier.Silver, p => Collection.FoundCount(p) >= 15), // Fifteen known kinds
            new Achievement("brave_hiker", "Brave Hiker", "Clear any Mountains Hard level.", BadgeTier.Silver, p => Enumerable.Range(1, PlayerProgress.LevelCount(Difficulty.Hard)).Any(n => p.IsCleared(Episode.Mountains, Difficulty.Hard, n))), // Any Hard clear in Mountains
            new Achievement("world_peaks", "World Peaks", "Clear the Matterhorn, Kilimanjaro and Mauna Kea (Hard 5 to 7).", BadgeTier.Silver, p => Enumerable.Range(5, 3).All(n => p.IsCleared(Episode.Mountains, Difficulty.Hard, n))), // All three world peaks
            new Achievement("hard_star", "Hard Star", "Get 3 stars on all 7 Mountains Hard levels.", BadgeTier.Gold, p => Enumerable.Range(1, PlayerProgress.LevelCount(Difficulty.Hard)).All(n => p.BestStars(Episode.Mountains, Difficulty.Hard, n) == 3)), // Every Hard trail has three stars
            new Achievement("mountain_master", "Mountain Master", "Clear all 15 Mountains levels.", BadgeTier.Gold, p => p.IsEpisodeComplete(Episode.Mountains)), // Finishes the entire landform
        }; // Ends the badge catalog

        /// <summary>The achievement with this id, or null when there is none.</summary>
        public static Achievement Find(string id) => All.FirstOrDefault(a => a.Id == id); // Looks the id up

        /// <summary>How many badges the explorer has earned.</summary>
        public static int EarnedCount(PlayerProgress progress) => All.Count(a => a.IsEarned(progress)); // Count them

        /// <summary>The ids of the badges earned now. Kept when a level starts, to see later what was new.</summary>
        public static HashSet<string> EarnedIds(PlayerProgress progress) => new HashSet<string>(All.Where(a => a.IsEarned(progress)).Select(a => a.Id)); // A set of ids

        /// <summary>Badges earned now that were not in the earlier set, in Profile order. The Result Screen shows these.</summary>
        public static IReadOnlyList<Achievement> NewlyEarned(ICollection<string> before, PlayerProgress progress) => All.Where(a => a.IsEarned(progress) && !before.Contains(a.Id)).ToArray(); // Only the new ones

        static IEnumerable<(Episode e, Difficulty d, int n)> AllLevels() // Every level the game can have
        {
            foreach (Episode e in Enum.GetValues(typeof(Episode))) // Each landform
                foreach (Difficulty d in Enum.GetValues(typeof(Difficulty))) // Each difficulty
                    for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) yield return (e, d, n); // Each level number
        }
    }
}
