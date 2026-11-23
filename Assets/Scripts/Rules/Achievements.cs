using System; // Gives access to Func and Enum
using System.Collections.Generic; // Gives IReadOnlyList and HashSet
using System.Linq; // Gives Any, All, Where and Count

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>The colour of a badge: bronze for first steps, silver for bigger goals, gold for the hardest (D-060).</summary>
    public enum BadgeTier { Bronze, Silver, Gold } // Badge colours

    /// <summary>
    /// One achievement (CONTEXT.md): a goal with a name, a hint on how to earn it and a badge colour.
    /// It is worked out from saved progress only, so it never needs its own save field and is never lost (D-060).
    /// Each badge also has its landform, which is the Profile page it is shown on (D-066).
    /// </summary>
    public sealed class Achievement // One badge's rule
    {
        public string Id { get; } // Short fixed name, also the badge picture name after "badge_"
        public string Name { get; } // Name shown under the badge
        public string HowToEarn { get; } // Hint shown on the badge card, also while it is locked
        public BadgeTier Tier { get; } // Bronze, silver or gold
        public Episode Landform { get; } // The landform whose badge page shows this badge
        public bool IsDraft { get; } // True until the names are reviewed, like the lesson text
        readonly Func<PlayerProgress, bool> earned; // The rule that decides it

        public Achievement(string id, string name, string howToEarn, BadgeTier tier, Episode landform, Func<PlayerProgress, bool> earned) // Makes an achievement
        {
            Id = id; Name = name; HowToEarn = howToEarn; Tier = tier; Landform = landform; IsDraft = true; this.earned = earned; // Remember everything
        }

        /// <summary>True when the progress meets the goal.</summary>
        public bool IsEarned(PlayerProgress progress) => progress != null && earned(progress); // Ask the rule
    }

    /// <summary>Every achievement in the game, in the order the Profile shows them (D-060, D-066).</summary>
    public static class AchievementCatalog // Shared list of badges
    {
        static readonly int[] LastThreeHardLevels = { 5, 6, 7 }; // The last three levels of a Hard trail: Mountains Hard 5 to 7 are the Matterhorn, Kilimanjaro and Mauna Kea

        /// <summary>Mountains, Hills, Plains and Plateaus badges in their saved order.</summary>
        public static IReadOnlyList<Achievement> All { get; } = new[] // In Profile order
        {
            new Achievement("first_steps", "First Steps", "Clear your first level.", BadgeTier.Bronze, Episode.Mountains, p => p.ClearedLevelCount >= 1), // Any cleared level
            new Achievement("rock_hound", "Rock Hound", "Find your first rock.", BadgeTier.Bronze, Episode.Mountains, p => ItemCatalog.All.Any(k => k.IsRock && p.HasFoundKind(k.Id))), // Any rock found
            new Achievement("crystal_finder", "Crystal Finder", "Find your first mineral.", BadgeTier.Bronze, Episode.Mountains, p => ItemCatalog.All.Any(k => !k.IsRock && p.HasFoundKind(k.Id))), // Any mineral found
            new Achievement("star_explorer", "Star Explorer", "Get 3 stars on any level.", BadgeTier.Silver, Episode.Mountains, p => AllLevels().Any(l => p.BestStars(l.e, l.d, l.n) >= 3)), // Three stars somewhere
            new Achievement("volcano_visitor", "Volcano Visitor", "Clear Mountains Easy 2.", BadgeTier.Silver, Episode.Mountains, p => ClearedAll(p, Episode.Mountains, Difficulty.Easy, 2)), // Volcano Trail
            new Achievement("summit_climber", "Summit Climber", "Clear Mountains Easy 3.", BadgeTier.Silver, Episode.Mountains, p => ClearedAll(p, Episode.Mountains, Difficulty.Easy, 3)), // Folded Peaks
            new Achievement("perfect_trail", "Perfect Trail", "Get 3 stars on every Mountains Easy level.", BadgeTier.Gold, Episode.Mountains, p => StarsAll(p, Episode.Mountains, Difficulty.Easy, 3, Numbers(Difficulty.Easy))), // Every Easy level perfect
            new Achievement("mountain_collector", "Mountain Collector", "Find every Mountains rock and mineral.", BadgeTier.Gold, Episode.Mountains, p => KindsAll(p, Episode.Mountains)), // The whole Mountains shelf
            new Achievement("philippine_peaks", "Philippine Peaks", "Clear Mountains Normal 1 to 4.", BadgeTier.Silver, Episode.Mountains, p => ClearedAll(p, Episode.Mountains, Difficulty.Normal, 1, 2, 3, 4)), // Four Philippine peaks
            new Achievement("top_of_the_world", "Top of the World", "Clear Mount Everest (Mountains Normal 5).", BadgeTier.Gold, Episode.Mountains, p => ClearedAll(p, Episode.Mountains, Difficulty.Normal, 5)), // Everest cleared
            new Achievement("rock_scholar", "Rock Scholar", "Find 15 different rocks and minerals.", BadgeTier.Silver, Episode.Mountains, p => Collection.FoundCount(p) >= 15), // Fifteen known kinds
            new Achievement("brave_hiker", "Brave Hiker", "Clear any Mountains Hard level.", BadgeTier.Silver, Episode.Mountains, p => ClearedAny(p, Episode.Mountains, Difficulty.Hard, Numbers(Difficulty.Hard))), // Any Hard clear in Mountains
            new Achievement("world_peaks", "World Peaks", "Clear the Matterhorn, Kilimanjaro and Mauna Kea (Hard 5 to 7).", BadgeTier.Silver, Episode.Mountains, p => ClearedAll(p, Episode.Mountains, Difficulty.Hard, LastThreeHardLevels)), // All three world peaks
            new Achievement("hard_star", "Hard Star", "Get 3 stars on all 7 Mountains Hard levels.", BadgeTier.Gold, Episode.Mountains, p => StarsAll(p, Episode.Mountains, Difficulty.Hard, 3, Numbers(Difficulty.Hard))), // Every Hard trail has three stars
            new Achievement("mountain_master", "Mountain Master", "Clear all 15 Mountains levels.", BadgeTier.Gold, Episode.Mountains, p => p.IsEpisodeComplete(Episode.Mountains)), // Finishes the entire landform
            new Achievement("pasture_pal", "Pasture Pal", "Clear Hills Easy 2.", BadgeTier.Silver, Episode.Hills, p => ClearedAll(p, Episode.Hills, Difficulty.Easy, 2)), // Batanes grass and grazing
            new Achievement("dune_surfer", "Dune Surfer", "Clear Hills Easy 3.", BadgeTier.Silver, Episode.Hills, p => ClearedAll(p, Episode.Hills, Difficulty.Easy, 3)), // La Paz dunes
            new Achievement("rolling_star", "Rolling Star", "Get 3 stars on every Hills Easy level.", BadgeTier.Gold, Episode.Hills, p => StarsAll(p, Episode.Hills, Difficulty.Easy, 3, Numbers(Difficulty.Easy))), // Every Hills Easy level perfect
            new Achievement("hill_collector", "Hill Collector", "Find every Hills rock and mineral.", BadgeTier.Gold, Episode.Hills, p => KindsAll(p, Episode.Hills)), // The whole Hills shelf
            new Achievement("philippine_hills", "Philippine Hills", "Clear Hills Normal 1 to 4.", BadgeTier.Silver, Episode.Hills, p => ClearedAll(p, Episode.Hills, Difficulty.Normal, 1, 2, 3, 4)), // Stays locked until those four levels are cleared
            new Achievement("balloon_rider", "Balloon Rider", "Clear Hills Normal 5.", BadgeTier.Gold, Episode.Hills, p => ClearedAll(p, Episode.Hills, Difficulty.Normal, 5)), // Cappadocia balloons
            new Achievement("slope_ranger", "Slope Ranger", "Clear a Hills Hard level.", BadgeTier.Silver, Episode.Hills, p => ClearedAny(p, Episode.Hills, Difficulty.Hard, Numbers(Difficulty.Hard))), // Any Hills Hard clear
            new Achievement("world_hills", "World Hills", "Clear Hills Hard 5 to 7.", BadgeTier.Silver, Episode.Hills, p => ClearedAll(p, Episode.Hills, Difficulty.Hard, LastThreeHardLevels)), // The last three Hills Hard trails
            new Achievement("steady_star", "Steady Star", "Get 3 stars on every Hills Hard level.", BadgeTier.Gold, Episode.Hills, p => StarsAll(p, Episode.Hills, Difficulty.Hard, 3, Numbers(Difficulty.Hard))), // Every Hills Hard level perfect
            new Achievement("hill_master", "Hill Master", "Clear every Hills level.", BadgeTier.Gold, Episode.Hills, p => p.IsEpisodeComplete(Episode.Hills)), // Finishes the Hills landform
            new Achievement("salt_maker", "Salt Maker", "Clear Plains Easy 2.", BadgeTier.Silver, Episode.Plains, p => ClearedAll(p, Episode.Plains, Difficulty.Easy, 2)), // Clears the salt beds
            new Achievement("straw_jumper", "Straw Jumper", "Clear Plains Easy 3.", BadgeTier.Silver, Episode.Plains, p => ClearedAll(p, Episode.Plains, Difficulty.Easy, 3)), // Clears the river plain
            new Achievement("flat_star", "Flat Star", "Get 3 stars on every Plains Easy level.", BadgeTier.Gold, Episode.Plains, p => StarsAll(p, Episode.Plains, Difficulty.Easy, 3, Numbers(Difficulty.Easy))), // Gets three stars on each Easy trail
            new Achievement("plain_collector", "Plain Collector", "Find every Plains rock and mineral.", BadgeTier.Gold, Episode.Plains, p => KindsAll(p, Episode.Plains)), // Fills the Plains shelf
            new Achievement("philippine_plains", "Philippine Plains", "Clear Plains Normal 1 to 4.", BadgeTier.Silver, Episode.Plains, p => ClearedAll(p, Episode.Plains, Difficulty.Normal, 1, 2, 3, 4)), // Clears the first four Normal trails
            new Achievement("delta_boater", "Delta Boater", "Clear Plains Normal 5.", BadgeTier.Gold, Episode.Plains, p => ClearedAll(p, Episode.Plains, Difficulty.Normal, 5)), // Clears the last Normal trail
            new Achievement("prairie_ranger", "Prairie Ranger", "Clear a Plains Hard level.", BadgeTier.Silver, Episode.Plains, p => ClearedAny(p, Episode.Plains, Difficulty.Hard, Numbers(Difficulty.Hard))), // Clears any Plains Hard trail
            new Achievement("world_plains", "World Plains", "Clear Plains Hard 5 to 7.", BadgeTier.Silver, Episode.Plains, p => ClearedAll(p, Episode.Plains, Difficulty.Hard, LastThreeHardLevels)), // Clears the last three Hard trails
            new Achievement("steady_plains", "Steady Plains", "Get 3 stars on every Plains Hard level.", BadgeTier.Gold, Episode.Plains, p => StarsAll(p, Episode.Plains, Difficulty.Hard, 3, Numbers(Difficulty.Hard))), // Gets three stars on each Hard trail
            new Achievement("plain_master", "Plain Master", "Clear every Plains level.", BadgeTier.Gold, Episode.Plains, p => p.IsEpisodeComplete(Episode.Plains)), // Finishes the Plains landform
            new Achievement("updraft_rider", "Updraft Rider", "Clear Plateaus Easy 3.", BadgeTier.Silver, Episode.Plateaus, p => ClearedAll(p, Episode.Plateaus, Difficulty.Easy, 3)), // Clears Maria Cristina Falls (D-072)
            new Achievement("table_top", "Table Top", "Clear Plateaus Normal 1.", BadgeTier.Silver, Episode.Plateaus, p => ClearedAll(p, Episode.Plateaus, Difficulty.Normal, 1)), // Clears Table Mountain
            new Achievement("high_star", "High Star", "Get 3 stars on every Plateaus Easy level.", BadgeTier.Gold, Episode.Plateaus, p => StarsAll(p, Episode.Plateaus, Difficulty.Easy, 3, Numbers(Difficulty.Easy))), // Gets three stars on each Easy trail
            new Achievement("plateau_collector", "Plateau Collector", "Find every Plateaus rock and mineral.", BadgeTier.Gold, Episode.Plateaus, p => KindsAll(p, Episode.Plateaus)), // Fills the Plateaus shelf
            new Achievement("philippine_plateaus", "Philippine Plateaus", "Clear Plateaus Easy 1 to 3.", BadgeTier.Silver, Episode.Plateaus, p => ClearedAll(p, Episode.Plateaus, Difficulty.Easy, 1, 2, 3)), // Clears the three Philippine plateaus
            new Achievement("canyon_explorer", "Canyon Explorer", "Clear Plateaus Normal 3.", BadgeTier.Gold, Episode.Plateaus, p => ClearedAll(p, Episode.Plateaus, Difficulty.Normal, 3)), // Clears the Grand Canyon
            new Achievement("mesa_ranger", "Mesa Ranger", "Clear a Plateaus Hard level.", BadgeTier.Silver, Episode.Plateaus, p => ClearedAny(p, Episode.Plateaus, Difficulty.Hard, Numbers(Difficulty.Hard))), // Clears any Plateaus Hard trail
            new Achievement("world_plateaus", "World Plateaus", "Clear Plateaus Hard 5 to 7.", BadgeTier.Silver, Episode.Plateaus, p => ClearedAll(p, Episode.Plateaus, Difficulty.Hard, LastThreeHardLevels)), // Clears the last three Hard trails
            new Achievement("steady_plateaus", "Steady Plateaus", "Get 3 stars on every Plateaus Hard level.", BadgeTier.Gold, Episode.Plateaus, p => StarsAll(p, Episode.Plateaus, Difficulty.Hard, 3, Numbers(Difficulty.Hard))), // Gets three stars on each Hard trail
            new Achievement("plateau_master", "Plateau Master", "Clear every Plateaus level.", BadgeTier.Gold, Episode.Plateaus, p => p.IsEpisodeComplete(Episode.Plateaus)), // Finishes the Plateaus landform
            new Achievement("land_grand_champion", "Land Grand Champion", "Clear every level of every landform.", BadgeTier.Gold, Episode.Plateaus, p => ((Episode[])Enum.GetValues(typeof(Episode))).All(p.IsEpisodeComplete)), // Finishes all four landforms
        }; // Ends the badge catalog

        /// <summary>The achievement with this id, or null when there is none.</summary>
        public static Achievement Find(string id) => All.FirstOrDefault(a => a.Id == id); // Looks the id up

        /// <summary>The badges on one landform's page, in Profile order. Mountains has 15, Hills and Plains have 10, and Plateaus has 11.</summary>
        public static IReadOnlyList<Achievement> ForLandform(Episode landform) => All.Where(a => a.Landform == landform).ToArray(); // Keeps the catalog order

        /// <summary>How many badges the explorer has earned.</summary>
        public static int EarnedCount(PlayerProgress progress) => All.Count(a => a.IsEarned(progress)); // Count them

        /// <summary>The ids of the badges earned now. Kept when a level starts, to see later what was new.</summary>
        public static HashSet<string> EarnedIds(PlayerProgress progress) => new HashSet<string>(All.Where(a => a.IsEarned(progress)).Select(a => a.Id)); // A set of ids

        /// <summary>Badges earned now that were not in the earlier set, in Profile order. The Result Screen shows these.</summary>
        public static IReadOnlyList<Achievement> NewlyEarned(ICollection<string> before, PlayerProgress progress) => All.Where(a => a.IsEarned(progress) && !before.Contains(a.Id)).ToArray(); // Only the new ones

        /// <summary>Level numbers 1 to the count for this difficulty, for example 1 to 7 for Hard.</summary>
        static int[] Numbers(Difficulty d) => Enumerable.Range(1, PlayerProgress.LevelCount(d)).ToArray(); // Counts from the game design

        /// <summary>True when every listed level is cleared. An empty list is never earned, so a missing set of levels stays locked.</summary>
        static bool ClearedAll(PlayerProgress p, Episode e, Difficulty d, params int[] levels) => levels.Length > 0 && levels.All(n => p.IsCleared(e, d, n)); // Every level must be cleared

        /// <summary>True when at least one listed level is cleared. An empty list is never earned.</summary>
        static bool ClearedAny(PlayerProgress p, Episode e, Difficulty d, int[] levels) => levels.Any(n => p.IsCleared(e, d, n)); // One clear is enough

        /// <summary>True when every listed level has at least this many stars. An empty list is never earned.</summary>
        static bool StarsAll(PlayerProgress p, Episode e, Difficulty d, int stars, int[] levels) => levels.Length > 0 && levels.All(n => p.BestStars(e, d, n) >= stars); // Every level needs the stars

        /// <summary>True when the explorer has found every kind on this landform's shelf. An empty shelf is never earned.</summary>
        static bool KindsAll(PlayerProgress p, Episode landform) // Checks one shelf
        {
            var kinds = ItemCatalog.All.Where(k => k.BelongsTo(landform)).ToArray(); // The kinds on this shelf
            return kinds.Length > 0 && kinds.All(k => p.HasFoundKind(k.Id)); // Every kind found, and the shelf is not empty
        }

        static IEnumerable<(Episode e, Difficulty d, int n)> AllLevels() // Every level the game can have
        {
            foreach (Episode e in Enum.GetValues(typeof(Episode))) // Each landform
                foreach (Difficulty d in Enum.GetValues(typeof(Difficulty))) // Each difficulty
                    for (int n = 1; n <= PlayerProgress.LevelCount(d); n++) yield return (e, d, n); // Each level number
        }
    }
}
