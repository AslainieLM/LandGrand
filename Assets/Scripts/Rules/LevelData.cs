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
        public bool IsRock { get; } // True for a rock, false for a mineral; the item card shows "Rock" or "Mineral"
        public Episode HomeLandform { get; } // The landform whose lesson first teaches it; its shelf in the Collection (D-052)

        public ItemKind(string id, string name, string description, string picture, bool isDraft, bool isRock, Episode homeLandform) // Makes a kind
        {
            Id = id; // Remember the id
            Name = name; // Remember the name
            Description = description; // Remember the description
            Picture = picture; // Remember the picture name
            IsDraft = isDraft; // Remember the review state
            IsRock = isRock; // Remember whether it is a rock or a mineral
            HomeLandform = homeLandform; // Remember its home landform
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
        public IReadOnlyList<string> Review { get; } // Two or three sentences that close the lesson on the Result Screen (draft, D-043)
        public LevelLayout Layout { get; } // Width, theme, boulders, ledges and vents; the builder makes the scene from it
        public string BannerName { get; } // Name shown on the level start banner, for example "Volcano Trail" (D-055)

        public LevelData(Episode episode, Difficulty difficulty, int number, Lesson lesson, Pickup[] pickups, Spot start, Spot checkpoint, Hazard[] hazards, Spot goal, string[] review = null, LevelLayout layout = null, string bannerName = null) // Makes a level
        {
            Layout = layout ?? LevelLayout.Plain(goal.X + 60f); // A plain layout a little past the goal when none is given (test levels)
            BannerName = bannerName ?? ""; // No banner name means no banner
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
            Review = (review ?? new string[0]).ToArray(); // Keep our own copy; a level may have no review yet
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
        public static readonly ItemKind Granite = new ItemKind("granite", "Granite", "A hard, speckled rock. It forms when melted rock cools slowly deep underground.", "granite", true, true, Episode.Mountains); // A rock, first taught in the Mountains lesson
        public static readonly ItemKind Quartz = new ItemKind("quartz", "Quartz", "A hard, glassy mineral that is often clear or white. It is found in granite.", "quartz", true, false, Episode.Mountains); // A mineral, first taught in the Mountains lesson
        public static readonly ItemKind Feldspar = new ItemKind("feldspar", "Feldspar", "A common mineral that is often pink or white. It is a main part of granite.", "feldspar", true, false, Episode.Mountains); // A mineral, first taught in the Mountains lesson
        public static readonly ItemKind Mica = new ItemKind("mica", "Mica", "A shiny mineral that peels into thin, flat sheets. It is found in granite.", "mica", true, false, Episode.Mountains); // A mineral, first taught in the Mountains lesson

        // Mountains Easy 2, volcanic mountains (D-055). Drafts like the rest (Q-04).
        public static readonly ItemKind Andesite = new ItemKind("andesite", "Andesite", "A grey rock made when lava cools. Many Philippine volcanoes are made of it.", "andesite", true, true, Episode.Mountains); // A rock from cooled lava
        public static readonly ItemKind Obsidian = new ItemKind("obsidian", "Obsidian", "A shiny black rock like glass. It forms when lava cools very fast.", "obsidian", true, true, Episode.Mountains); // Volcanic glass, a rock
        public static readonly ItemKind Pumice = new ItemKind("pumice", "Pumice", "A light rock full of tiny holes from gas bubbles. Some pumice can float on water.", "pumice", true, true, Episode.Mountains); // A light, bubbly rock

        // Mountains Easy 3, folded mountains (D-055). Drafts like the rest (Q-04).
        public static readonly ItemKind Slate = new ItemKind("slate", "Slate", "A dark rock that splits into flat, smooth sheets. It forms when mud rock is squeezed.", "slate", true, true, Episode.Mountains); // A rock changed by squeezing
        public static readonly ItemKind Marble = new ItemKind("marble", "Marble", "A smooth rock, often white. It forms when limestone is heated and squeezed.", "marble", true, true, Episode.Mountains); // A rock changed by heat and squeezing
        public static readonly ItemKind Garnet = new ItemKind("garnet", "Garnet", "A dark red mineral that grows as small crystals inside squeezed rock.", "garnet", true, false, Episode.Mountains); // A mineral that grows in changed rock

        /// <summary>All kinds, in the order the levels introduce them. The Collection shelves keep this order.</summary>
        public static IReadOnlyList<ItemKind> All { get; } = new[] { Granite, Quartz, Feldspar, Mica, Andesite, Obsidian, Pumice, Slate, Marble, Garnet }; // The full list

        /// <summary>The kind with this id, or null when there is none.</summary>
        public static ItemKind Find(string id) => All.FirstOrDefault(k => k.Id == id); // Looks the id up
    }

    /// <summary>Every level that has data. Levels without data are not playable yet.</summary>
    public static class LevelCatalog // Shared level list
    {
        static readonly List<LevelData> Levels = new List<LevelData> // All levels with data
        {
            // Mountains, Easy, level 1 (D-043): a meadow trail to a cave. Its whole layout is here; the builder reads it.
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
                new Spot(4740f, 0f), // Cave entrance at the right end
                new[] // Draft review for the Result Screen; needs a teacher check like the lesson (D-043, Q-04)
                {
                    "Mountains are high land with steep sides.", // Repeats the lesson
                    "Granite is a hard rock made of quartz, feldspar and mica.", // The rock and its minerals
                    "Hard rock like granite helps mountains stay tall.", // Why it matters
                },
                new LevelLayout(4800f, LevelTheme.Meadow, // Ten screens of meadow, about 65 s of walking (D-046)
                    new[] { B(300f, BoulderSize.Small), B(900f, BoulderSize.Big), B(1300f, BoulderSize.Small), B(2150f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3250f, BoulderSize.Big), B(3850f, BoulderSize.Big), B(4150f, BoulderSize.Small), B(4350f, BoulderSize.Big) }, // Nine boulders along the trail
                    new[] { L(520f, 40f, 80f), L(1050f, 28f, 80f), L(1500f, 28f, 64f), L(2300f, 28f, 64f), L(2920f, 28f, 80f), L(3560f, 28f, 48f), L(3620f, 56f, 96f), L(3950f, 28f, 80f), L(4480f, 28f, 48f) }), // Low ledges, plus a step and a high ledge
                "Mountain Trail"), // Name on the start banner

            // Mountains, Easy, level 2 (D-055): volcanic mountains. Teach, test, twist: each danger first appears on its own.
            new LevelData(Episode.Mountains, Difficulty.Easy, 2, // Mountains, Easy, level 2
                new Lesson("Volcanic Mountains", new[] // Draft lesson for Grade 5
                {
                    "Some mountains are volcanoes.", // What a volcano is
                    "Hot melted rock, called lava, flows out.", // How the rock is made
                    "Cooled lava turns into rock like andesite.", // The new rock
                    "Mayon and Apo are Philippine volcanoes.", // Examples the learners know
                }, "lesson_volcano", true), // Picture name, and still a draft
                new[] // Five collectibles: three new kinds, quartz again as a review, and andesite twice
                {
                    new Pickup(ItemCatalog.Andesite, new Spot(376f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Quartz, new Spot(1012f, 84f)), // Floating over the first vent: the puff or a double jump reaches it
                    new Pickup(ItemCatalog.Obsidian, new Spot(2082f, 40f)), // On a ledge after the first spikes
                    new Pickup(ItemCatalog.Pumice, new Spot(3072f, 100f)), // Secret spot: the high ledge only the second vent reaches
                    new Pickup(ItemCatalog.Andesite, new Spot(4316f, 24f)), // On a boulder near the arch
                },
                new Spot(32f, 0f), // Start at the left edge
                new Spot(2700f, 0f), // Checkpoint flag in the middle
                new[] // Two pits and two spike traps
                {
                    new Hazard(HazardType.Pit, new Spot(700f, 0f), 48f), // Pit 1: one jump clears it
                    new Hazard(HazardType.Spikes, new Spot(1800f, 0f), 32f), // Spikes 1, on flat ground
                    new Hazard(HazardType.Pit, new Spot(3380f, 0f), 56f), // Pit 2, between two ledges
                    new Hazard(HazardType.Spikes, new Spot(3800f, 0f), 32f), // Spikes 2, just after a low ledge
                },
                new Spot(5240f, 0f), // Lava-rock arch at the right end
                new[] // Draft review for the Result Screen (D-043, Q-04)
                {
                    "Volcanoes are mountains made by lava.", // Repeats the lesson
                    "Lava cools into rocks like andesite, pumice and obsidian.", // The rocks of the level
                    "Fast cooling makes glassy obsidian.", // Why obsidian looks like glass
                },
                new LevelLayout(5400f, LevelTheme.Volcanic, // Dark lava rock, eleven screens
                    new[] { B(360f, BoulderSize.Small), B(1550f, BoulderSize.Big), B(2350f, BoulderSize.Small), B(4300f, BoulderSize.Small), B(4700f, BoulderSize.Big) }, // Five boulders
                    new[] { L(1300f, 28f, 80f), L(2050f, 40f, 64f), L(3032f, 100f, 72f), L(3300f, 28f, 64f), L(3452f, 28f, 64f), L(3748f, 32f, 40f), L(4900f, 28f, 64f) }, // Ledges; the one at height 100 is the secret spot
                    new[] { new SteamVent(1000f), new SteamVent(2992f) }), // First vent on flat ground to try safely; second one under the secret ledge
                "Volcano Trail"), // Name on the start banner

            // Mountains, Easy, level 3 (D-055): folded mountains. A climb, a wide pit for the double jump, and a spike pair.
            new LevelData(Episode.Mountains, Difficulty.Easy, 3, // Mountains, Easy, level 3
                new Lesson("Folded Mountains", new[] // Draft lesson for Grade 5
                {
                    "Land pushed together makes mountains.", // How folded mountains form
                    "Rock bends into folds, like a pushed rug.", // A picture the learners know
                    "Heat and squeezing turn old rock into new.", // How rocks change
                    "Mud rock becomes slate this way.", // A changed rock; marble's story is in its pop-up
                }, "lesson_folds", true), // Picture name, and still a draft
                new[] // Five collectibles: three new kinds, mica again as a review, and slate twice
                {
                    new Pickup(ItemCatalog.Slate, new Spot(500f, 72f)), // On top of the stepped climb
                    new Pickup(ItemCatalog.Mica, new Spot(1532f, 40f)), // On a ledge after the first spikes
                    new Pickup(ItemCatalog.Marble, new Spot(3266f, 0f)), // On the ground between the two spike traps
                    new Pickup(ItemCatalog.Garnet, new Spot(3512f, 84f)), // Secret spot: the high ledge reached by climbing back up the folds
                    new Pickup(ItemCatalog.Slate, new Spot(4416f, 24f)), // On a boulder near the summit
                },
                new Spot(32f, 0f), // Start at the left edge
                new Spot(2500f, 0f), // Checkpoint flag in the middle
                new[] // Three pits and three spike traps
                {
                    new Hazard(HazardType.Pit, new Spot(800f, 0f), 48f), // Pit 1: one jump clears it
                    new Hazard(HazardType.Spikes, new Spot(1300f, 0f), 32f), // Spikes 1
                    new Hazard(HazardType.Pit, new Spot(2800f, 0f), 64f), // The wide pit: too far for one jump (about 46 pixels), easy with the double jump (about 71)
                    new Hazard(HazardType.Spikes, new Spot(3200f, 0f), 32f), // First of the spike pair
                    new Hazard(HazardType.Spikes, new Spot(3300f, 0f), 32f), // Second of the spike pair, with marble between them
                    new Hazard(HazardType.Pit, new Spot(3900f, 0f), 56f), // Pit 3
                },
                new Spot(5240f, 0f), // Summit stone pile at the right end
                new[] // Draft review for the Result Screen (D-043, Q-04)
                {
                    "Folded mountains form when land is pushed together.", // Repeats the lesson
                    "Heat and squeezing change rocks, like mud rock into slate.", // How rocks change
                    "Garnet crystals can grow inside changed rock.", // The mineral of the level
                },
                new LevelLayout(5400f, LevelTheme.FoldedRock, // Grey folded rock, eleven screens
                    new[] { B(1050f, BoulderSize.Big), B(1800f, BoulderSize.Small), B(4400f, BoulderSize.Small), B(4700f, BoulderSize.Big), B(5000f, BoulderSize.Small) }, // Five boulders
                    new[] { L(300f, 24f, 64f), L(380f, 48f, 64f), L(460f, 72f, 80f), L(1500f, 40f, 64f), L(2000f, 28f, 80f), L(3500f, 28f, 56f), L(3580f, 56f, 56f), L(3480f, 84f, 64f) }), // A stepped climb at the start and the folds back up to the secret ledge
                "Folded Peaks"), // Name on the start banner
        };

        static Boulder B(float x, BoulderSize size) => new Boulder(x, size); // Short way to write a boulder
        static Ledge L(float x, float top, float width) => new Ledge(x, top, width); // Short way to write a ledge

        /// <summary>Every level with data, in play order.</summary>
        public static IReadOnlyList<LevelData> All => Levels; // The builder makes one scene for each

        /// <summary>The level's data, or null when that level has no data yet.</summary>
        public static LevelData Find(Episode e, Difficulty d, int number) => Levels.FirstOrDefault(l => l.Episode == e && l.Difficulty == d && l.Number == number); // Looks the level up
    }
}
