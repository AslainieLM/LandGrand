using System.Collections.Generic; // Gives IReadOnlyList and List
using System.Linq; // Gives Select and Distinct

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>A place in a level, in level pixels: X goes right from the left edge, Y goes up from the ground line.</summary>
    public readonly struct Spot // One point in a level
    {
        public readonly float X; // Pixels from the left edge of the level
        public readonly float Y; // Pixels up from the ground line

        public Spot(float x, float y) { X = x; Y = y; } // Makes a spot
    }

    /// <summary>One kind of rock or mineral. Kinds are shared by every level, so "first find" works across the game.</summary>
    public sealed class ItemKind // A rock or mineral kind
    {
        public string Id { get; } // Short fixed name used in the save file, for example "granite"
        public string Name { get; } // Name shown to the player
        public string Description { get; } // Short text shown in the pop-up
        public string Picture { get; } // Name of its picture file, without ".png"
        public bool IsDraft { get; } // True until a teacher reviews the text (D-043, Q-04)

        public ItemKind(string id, string name, string description, string picture, bool isDraft) // Makes a kind
        {
            Id = id; // Remember the id
            Name = name; // Remember the name
            Description = description; // Remember the description
            Picture = picture; // Remember the picture name
            IsDraft = isDraft; // Remember the review state
        }
    }

    /// <summary>The short lesson shown before a level: a title, three or four sentences and a picture.</summary>
    public sealed class Lesson // One lesson
    {
        public string Title { get; } // The landform's name
        public IReadOnlyList<string> Sentences { get; } // Three or four short sentences
        public string Picture { get; } // Name of its picture file, without ".png"
        public bool IsDraft { get; } // True until a teacher reviews the text (D-043, Q-04)

        public Lesson(string title, string[] sentences, string picture, bool isDraft) // Makes a lesson
        {
            Title = title; // Remember the title
            Sentences = sentences.ToArray(); // Keep our own copy, so nobody can change the lesson later
            Picture = picture; // Remember the picture name
            IsDraft = isDraft; // Remember the review state
        }
    }

    /// <summary>One collectible placed in a level: which kind it is and where it lies.</summary>
    public sealed class Pickup // One collectible
    {
        public ItemKind Kind { get; } // The shared kind
        public Spot Place { get; } // Where it lies

        public Pickup(ItemKind kind, Spot place) { Kind = kind; Place = place; } // Makes a pickup
    }

    /// <summary>The kinds of danger in a level. Both send the explorer back to the last checkpoint (D-040).</summary>
    public enum HazardType { Pit, Spikes } // Hazard list

    /// <summary>One danger placed in a level: its type, where it starts and how wide it is.</summary>
    public sealed class Hazard // One pit or spike trap
    {
        public HazardType Type { get; } // Pit or spikes
        public Spot Place { get; } // Left end of the danger
        public float Width { get; } // Width in level pixels

        public Hazard(HazardType type, Spot place, float width) { Type = type; Place = place; Width = width; } // Makes a hazard
    }

    /// <summary>
    /// Everything that describes one level as data: landform, difficulty, number, time limit, lesson,
    /// collectibles, start, checkpoint, hazards and goal. Scenes read it; it never changes while the game runs.
    /// </summary>
    public sealed class LevelData // One level's description
    {
        /// <summary>The note shown next to content that a teacher has not reviewed yet (D-043).</summary>
        public const string DraftNote = "Draft — needs teacher review"; // "—" is a long dash

        public Episode Episode { get; } // Which landform
        public Difficulty Difficulty { get; } // Which difficulty
        public int Number { get; } // Level number inside the difficulty, starting at 1
        public int TimeLimitSeconds { get; } // Countdown length
        public Lesson Lesson { get; } // The lesson shown before the level
        public IReadOnlyList<Pickup> Pickups { get; } // The collectibles
        public Spot Start { get; } // Where the explorer begins
        public Spot Checkpoint { get; } // The mid-level flag
        public IReadOnlyList<Hazard> Hazards { get; } // Pits and spike traps
        public Spot Goal { get; } // The cave entrance with the flag

        public LevelData(Episode episode, Difficulty difficulty, int number, Lesson lesson, Pickup[] pickups, Spot start, Spot checkpoint, Hazard[] hazards, Spot goal) // Makes a level
        {
            Episode = episode; // Remember the landform
            Difficulty = difficulty; // Remember the difficulty
            Number = number; // Remember the number
            TimeLimitSeconds = TimeLimitFor(difficulty); // Time comes from the difficulty
            Lesson = lesson; // Remember the lesson
            Pickups = pickups.ToArray(); // Keep our own copy, so nobody can change the level later
            Start = start; // Remember the start
            Checkpoint = checkpoint; // Remember the checkpoint
            Hazards = hazards.ToArray(); // Keep our own copy, so nobody can change the level later
            Goal = goal; // Remember the goal
        }

        /// <summary>Countdown length for a difficulty: Easy 2, Normal 3, Hard 4 minutes (D-039).</summary>
        public static int TimeLimitFor(Difficulty d) => d == Difficulty.Easy ? 120 : d == Difficulty.Normal ? 180 : 240; // Seconds

        /// <summary>Each kind in the level once, in the order the pickups list them. The Lesson Screen shows these.</summary>
        public IEnumerable<ItemKind> Kinds => Pickups.Select(p => p.Kind).Distinct(); // Same object means same kind

        /// <summary>The level's goals as the player reads them, for example "Collect 5/5".</summary>
        public IEnumerable<string> Goals // Goal lines for the Lesson and Result screens
        {
            get // Builds the lines each time they are read
            { // Start of the goal lines
                yield return "Collect " + Pickups.Count + "/" + Pickups.Count; // Collect every item (3 stars)
                yield return "Finish before time runs out"; // Reach the goal in time
            }
        }

        /// <summary>True when the lesson or any item still needs a teacher's review.</summary>
        public bool HasDraftContent => Lesson.IsDraft || Pickups.Any(p => p.Kind.IsDraft); // Any draft part counts

        /// <summary>Name of the scene that plays this level, for example "MountainsEasy1".</summary>
        public string SceneName => Episode.ToString() + Difficulty + Number; // Landform, difficulty and number
    }

    /// <summary>Every rock and mineral kind in the game. Levels point to these, never to copies.</summary>
    public static class ItemCatalog // Shared item kinds
    {
        // Draft texts for Grade 5 (D-043). They need a teacher's review before final use (Q-04).
        public static readonly ItemKind Granite = new ItemKind("granite", "Granite", "A hard, speckled rock. It forms when melted rock cools slowly deep underground.", "granite", true); // A rock
        public static readonly ItemKind Quartz = new ItemKind("quartz", "Quartz", "A hard, glassy mineral that is often clear or white. It is found in granite.", "quartz", true); // A mineral
        public static readonly ItemKind Feldspar = new ItemKind("feldspar", "Feldspar", "A common mineral that is often pink or white. It is a main part of granite.", "feldspar", true); // A mineral
        public static readonly ItemKind Mica = new ItemKind("mica", "Mica", "A shiny mineral that peels into thin, flat sheets. It is found in granite.", "mica", true); // A mineral

        /// <summary>All kinds, in the order the lessons introduce them.</summary>
        public static IReadOnlyList<ItemKind> All { get; } = new[] { Granite, Quartz, Feldspar, Mica }; // The full list

        /// <summary>The kind with this id, or null when there is none.</summary>
        public static ItemKind Find(string id) => All.FirstOrDefault(k => k.Id == id); // Looks the id up
    }

    /// <summary>Every level that has data. Levels without data are not playable yet.</summary>
    public static class LevelCatalog // Shared level list
    {
        static readonly List<LevelData> Levels = new List<LevelData> // All levels with data
        {
            // Mountains, Easy, level 1 (D-043). Places match the boulders and ledges in Assets/Editor/LevelBuilder.cs; change both together.
            new LevelData(Episode.Mountains, Difficulty.Easy, 1, // Mountains, Easy, level 1
                new Lesson("Mountains", new[] // Draft lesson for Grade 5
                {
                    "A mountain is land that rises very high.", // What a mountain is
                    "It has steep sides and a top called a peak.", // Its parts
                    "Many mountains have hard rock like granite.", // Its rock
                    "Granite has quartz, feldspar and mica in it.", // The minerals in the rock
                }, "lesson_mountains", true), // Picture name, and still a draft
                new[] // Five collectibles; granite appears twice
                {
                    new Pickup(ItemCatalog.Granite, new Spot(316f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Quartz, new Spot(550f, 56f)), // On a ledge
                    new Pickup(ItemCatalog.Feldspar, new Spot(1090f, 40f)), // On a low ledge
                    new Pickup(ItemCatalog.Mica, new Spot(3668f, 64f)), // On a high ledge, after the checkpoint
                    new Pickup(ItemCatalog.Granite, new Spot(4600f, 16f)), // Near the cave
                },
                new Spot(32f, 0f), // Start at the left edge
                new Spot(2400f, 0f), // Checkpoint flag in the middle
                new[] // One pit and one spike trap
                {
                    new Hazard(HazardType.Pit, new Spot(1880f, 0f), 48f), // Pit before the checkpoint
                    new Hazard(HazardType.Spikes, new Spot(3500f, 0f), 32f), // Spikes after the checkpoint
                },
                new Spot(4740f, 0f)), // Cave entrance at the right end
        };

        /// <summary>The level's data, or null when that level has no data yet.</summary>
        public static LevelData Find(Episode e, Difficulty d, int number) => Levels.FirstOrDefault(l => l.Episode == e && l.Difficulty == d && l.Number == number); // Looks the level up
    }
}
