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
        public Episode[] Landforms { get; } // The landforms whose shelves show this kind; a shared kind is on more than one (D-066)

        public ItemKind(string id, string name, string description, string picture, bool isDraft, bool isRock, params Episode[] landforms) // Makes a kind; one or more landforms
        {
            Id = id; // Remember the id
            Name = name; // Remember the name
            Description = description; // Remember the description
            Picture = picture; // Remember the picture name
            IsDraft = isDraft; // Remember the review state
            IsRock = isRock; // Remember whether it is a rock or a mineral
            Landforms = landforms.ToArray(); // Keep our own copy of the landforms
        }

        /// <summary>True when this kind is on the shelf of this landform.</summary>
        public bool BelongsTo(Episode episode) => Landforms.Contains(episode); // Looks the landform up in the list
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
        public bool IsWater { get; } // True when this pit is filled with water

        public Hazard(HazardType type, Spot place, float width, bool isWater = false) // Makes a hazard
        { // Opens the placement
            Type = type; // Saves the hazard type
            Place = place; // Saves the left edge
            Width = width; // Saves the width
            IsWater = isWater; // Saves whether there is water in the pit
        } // Ends the placement
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
        public IReadOnlyList<Spot> Checkpoints { get; } // Flags in trail order
        public IReadOnlyList<Hazard> Hazards { get; } // Pits and spike traps
        public Spot Goal { get; } // The cave entrance with the flag
        public IReadOnlyList<string> Review { get; } // Two or three sentences that close the lesson on the Result Screen (draft, D-043)
        public LevelLayout Layout { get; } // Width, theme, boulders, ledges and vents; the builder makes the scene from it
        public string BannerName { get; } // Name shown on the level start banner, for example "Volcano Trail" (D-055)

        public LevelData(Episode episode, Difficulty difficulty, int number, Lesson lesson, Pickup[] pickups, Spot start, Spot[] checkpoints, Hazard[] hazards, Spot goal, string[] review = null, LevelLayout layout = null, string bannerName = null) // Makes a level
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
            Checkpoints = checkpoints.ToArray(); // Keep our own copy of the flags
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
        public static readonly ItemKind Granite = new ItemKind("granite", "Granite", "A hard, speckled rock. It forms when melted rock cools slowly deep underground.", "granite", true, true, Episode.Mountains, Episode.Plains); // A rock, first taught in the Mountains lesson; also on the Plains shelf (D-071)
        public static readonly ItemKind Quartz = new ItemKind("quartz", "Quartz", "A hard, glassy mineral that is often clear or white. It is found in granite.", "quartz", true, false, Episode.Mountains, Episode.Hills, Episode.Plains); // A mineral, taught in Mountains and shared with Hills and Plains
        public static readonly ItemKind Feldspar = new ItemKind("feldspar", "Feldspar", "A common mineral that is often pink or white. It is a main part of granite.", "feldspar", true, false, Episode.Mountains, Episode.Hills, Episode.Plains); // A mineral, taught in Mountains and shared with Hills and Plains (D-071)
        public static readonly ItemKind Mica = new ItemKind("mica", "Mica", "A shiny mineral that peels into thin, flat sheets. It is found in granite.", "mica", true, false, Episode.Mountains, Episode.Plains); // A mineral, first taught in the Mountains lesson; also on the Plains shelf (D-071)

        // Mountains Easy 2, volcanic mountains (D-055). Drafts like the rest (Q-04).
        public static readonly ItemKind Andesite = new ItemKind("andesite", "Andesite", "A grey rock made when lava cools. Many Philippine volcanoes are made of it.", "andesite", true, true, Episode.Mountains, Episode.Hills, Episode.Plains, Episode.Plateaus); // A rock from cooled lava, taught in Mountains and shared with Hills, Plains and Plateaus (D-072)
        public static readonly ItemKind Obsidian = new ItemKind("obsidian", "Obsidian", "A shiny black rock like glass. It forms when lava cools very fast.", "obsidian", true, true, Episode.Mountains); // Volcanic glass, a rock
        public static readonly ItemKind Pumice = new ItemKind("pumice", "Pumice", "A light rock full of tiny holes from gas bubbles. Some pumice can float on water.", "pumice", true, true, Episode.Mountains, Episode.Plains); // A light, bubbly rock shared with Plains

        // Mountains Easy 3, folded mountains (D-055). Drafts like the rest (Q-04).
        public static readonly ItemKind Slate = new ItemKind("slate", "Slate", "A dark rock that splits into flat, smooth sheets. It forms when mud rock is squeezed.", "slate", true, true, Episode.Mountains); // A rock changed by squeezing
        public static readonly ItemKind Marble = new ItemKind("marble", "Marble", "A smooth rock, often white. It forms when limestone is heated and squeezed.", "marble", true, true, Episode.Mountains); // A rock changed by heat and squeezing
        public static readonly ItemKind Garnet = new ItemKind("garnet", "Garnet", "A dark red mineral that grows as small crystals inside squeezed rock.", "garnet", true, false, Episode.Mountains); // A mineral that grows in changed rock

        public static readonly ItemKind Sandstone = new ItemKind("sandstone", "Sandstone", "A rock made of sand grains stuck together. It wears away in wind and rain.", "sandstone", true, true, Episode.Mountains, Episode.Plains); // Draft Mountains kind, also on the Plains shelf
        public static readonly ItemKind Clay = new ItemKind("clay", "Clay", "Very fine soil from weathered rock. It feels sticky when it is wet.", "clay", true, false, Episode.Mountains, Episode.Hills, Episode.Plains); // Draft kind on the Mountains, Hills and Plains shelves
        public static readonly ItemKind Conglomerate = new ItemKind("conglomerate", "Conglomerate", "A rock of round pebbles stuck in sand. Rivers rounded the pebbles long ago.", "conglomerate", true, true, Episode.Mountains, Episode.Plains); // Draft Mountains kind, also on the Plains shelf
        public static readonly ItemKind Shale = new ItemKind("shale", "Shale", "A rock made from mud that settled in water. It splits into thin layers.", "shale", true, true, Episode.Mountains, Episode.Plains); // Draft Mountains kind, also on the Plains shelf
        public static readonly ItemKind Basalt = new ItemKind("basalt", "Basalt", "A dark rock made from cooled lava. It often has tiny holes from gas.", "basalt", true, true, Episode.Mountains, Episode.Plateaus); // Draft Mountains kind, also on the Plateaus shelf (D-072)
        public static readonly ItemKind Magnetite = new ItemKind("magnetite", "Magnetite", "A black mineral that sticks to a magnet. It makes the black sand in rivers.", "magnetite", true, false, Episode.Mountains, Episode.Hills); // Draft kind on the Mountains and Hills shelves
        public static readonly ItemKind Sulfur = new ItemKind("sulfur", "Sulfur", "A bright yellow mineral. It forms around hot volcanic vents.", "sulfur", true, false, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Pyrite = new ItemKind("pyrite", "Pyrite", "Called fool's gold. Its shiny gold cubes are not real gold.", "pyrite", true, false, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Limestone = new ItemKind("limestone", "Limestone", "A pale rock made from shells under the sea. Heat can turn it into marble.", "limestone", true, true, Episode.Mountains, Episode.Hills, Episode.Plains); // Draft kind on the Mountains, Hills and Plains shelves
        public static readonly ItemKind Gneiss = new ItemKind("gneiss", "Gneiss", "A rock with light and dark bands. Deep heat and squeezing make it.", "gneiss", true, true, Episode.Mountains); // Draft Mountains kind

        public static readonly ItemKind Scoria = new ItemKind("scoria", "Scoria", "A red-brown lava rock full of holes. Gas bubbles made the holes.", "scoria", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Hornblende = new ItemKind("hornblende", "Hornblende", "A black mineral shaped like long sticks. It is found in lava rock like andesite.", "hornblende", true, false, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Diorite = new ItemKind("diorite", "Diorite", "A rock with black and white spots. Magma cooled slowly underground to make it.", "diorite", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Gold = new ItemKind("gold", "Gold", "A shiny yellow metal found in mountain rocks. It never rusts.", "gold", true, false, Episode.Mountains, Episode.Plains); // Draft Mountains kind, also on the Plains shelf
        public static readonly ItemKind Tuff = new ItemKind("tuff", "Tuff", "A light rock made of volcanic ash. The ash was pressed together over time.", "tuff", true, true, Episode.Mountains, Episode.Plains, Episode.Plateaus); // Draft Mountains kind, also on the Plains (D-071) and Plateaus (D-072) shelves
        public static readonly ItemKind Hematite = new ItemKind("hematite", "Hematite", "A heavy mineral with iron in it. It leaves a red streak when rubbed.", "hematite", true, false, Episode.Mountains, Episode.Plateaus); // Draft Mountains kind, also on the Plateaus shelf (D-072)
        public static readonly ItemKind Serpentinite = new ItemKind("serpentinite", "Serpentinite", "A smooth green rock that looks like snake skin. Few plants grow on its soil.", "serpentinite", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Chromite = new ItemKind("chromite", "Chromite", "A heavy black mineral. It gives us chromium for shiny metal parts.", "chromite", true, false, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Schist = new ItemKind("schist", "Schist", "A shiny rock that splits into thin layers. Heat and squeezing made it.", "schist", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Calcite = new ItemKind("calcite", "Calcite", "A clear or white mineral. It is the main mineral in limestone and marble.", "calcite", true, false, Episode.Mountains, Episode.Hills, Episode.Plains); // Draft kind on the Mountains, Hills and Plains shelves (D-071)
        public static readonly ItemKind Trachyte = new ItemKind("trachyte", "Trachyte", "A grey lava rock with tiny crystals. Much of Kilimanjaro's top is made of it.", "trachyte", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Tanzanite = new ItemKind("tanzanite", "Tanzanite", "A blue-violet gem. It is found only near Mount Kilimanjaro in Tanzania.", "tanzanite", true, false, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Gabbro = new ItemKind("gabbro", "Gabbro", "A dark rock with big grains. It forms deep under the sea floor.", "gabbro", true, true, Episode.Mountains); // Draft Mountains kind
        public static readonly ItemKind Olivine = new ItemKind("olivine", "Olivine", "A green mineral in lava. It makes the green sand of a Hawaii beach.", "olivine", true, false, Episode.Mountains); // Draft Mountains kind

        // Hills kinds (D-066). Drafts like the rest (Q-04). They are on the Hills shelf only.
        public static readonly ItemKind CoralStone = new ItemKind("coral_stone", "Coral Stone", "A pale rock made from old coral. The Ivatan build strong stone houses with it.", "coral_stone", true, true, Episode.Hills); // A rock from old coral, first taught on Hills
        public static readonly ItemKind Limonite = new ItemKind("limonite", "Limonite", "A yellow-brown iron mineral. It gives many soils their rusty color.", "limonite", true, false, Episode.Hills); // A mineral that colors soil, first taught on Hills
        public static readonly ItemKind Kaolinite = new ItemKind("kaolinite", "Kaolinite", "A soft, white clay mineral. It forms when feldspar slowly breaks down.", "kaolinite", true, false, Episode.Hills); // A soft clay mineral, first taught on Hills

        public static readonly ItemKind Aragonite = new ItemKind("aragonite", "Aragonite", "A mineral that corals and shells use to build their hard parts.", "aragonite", true, false, Episode.Hills); // Draft mineral on the Hills shelf
        public static readonly ItemKind Coquina = new ItemKind("coquina", "Coquina", "A rock made of broken shells pressed and stuck together.", "coquina", true, true, Episode.Hills); // Draft rock on the Hills shelf
        public static readonly ItemKind Flowstone = new ItemKind("flowstone", "Flowstone", "Sheets of stone left by water flowing over cave walls.", "flowstone", true, true, Episode.Hills); // Draft rock on the Hills shelf
        public static readonly ItemKind Dripstone = new ItemKind("dripstone", "Dripstone", "Stone made by dripping water. It forms stalactites.", "dripstone", true, true, Episode.Hills); // Draft rock on the Hills shelf
        public static readonly ItemKind VolcanicBreccia = new ItemKind("volcanic_breccia", "Volcanic Breccia", "Sharp pieces of volcanic rock glued together by ash.", "volcanic_breccia", true, true, Episode.Hills, Episode.Plateaus); // Draft rock on the Hills shelf, also on the Plateaus shelf (D-072)
        public static readonly ItemKind Ignimbrite = new ItemKind("ignimbrite", "Ignimbrite", "A rock made when a hot cloud of volcanic ash settles and welds.", "ignimbrite", true, true, Episode.Hills); // Draft rock on the Hills shelf
        public static readonly ItemKind Zeolite = new ItemKind("zeolite", "Zeolite", "A mineral that forms in volcanic ash. It is full of tiny holes.", "zeolite", true, false, Episode.Hills); // Draft mineral on the Hills shelf

        public static readonly ItemKind Peridotite = new ItemKind("peridotite", "Peridotite", "A dark green rock from deep inside the Earth. It is full of olivine.", "peridotite", true, true, Episode.Hills); // Draft Hills rock
        public static readonly ItemKind Chlorite = new ItemKind("chlorite", "Chlorite", "A soft green mineral. It forms when rocks like basalt slowly change.", "chlorite", true, false, Episode.Hills); // Draft Hills mineral
        public static readonly ItemKind Dacite = new ItemKind("dacite", "Dacite", "A pale grey volcanic rock with small light crystals.", "dacite", true, true, Episode.Hills, Episode.Plains); // Draft Hills rock, also on the Plains shelf
        public static readonly ItemKind Quartzite = new ItemKind("quartzite", "Quartzite", "A very hard rock made when sandstone is heated and squeezed.", "quartzite", true, true, Episode.Hills); // Draft Hills rock
        public static readonly ItemKind Travertine = new ItemKind("travertine", "Travertine", "A cream rock made by spring water. Romans built with it.", "travertine", true, true, Episode.Hills); // Draft Hills rock
        public static readonly ItemKind Leucite = new ItemKind("leucite", "Leucite", "A white mineral with round crystals, found in some volcanic rock.", "leucite", true, false, Episode.Hills); // Draft Hills mineral

        public static readonly ItemKind Siltstone = new ItemKind("siltstone", "Siltstone", "A smooth rock made of silt, finer than sand. Silt settles in quiet water.", "siltstone", true, true, Episode.Plains); // Draft Plains rock
        public static readonly ItemKind Halite = new ItemKind("halite", "Halite", "Rock salt. It forms when salty sea or lake water dries up.", "halite", true, false, Episode.Plains); // Draft Plains mineral
        public static readonly ItemKind Gypsum = new ItemKind("gypsum", "Gypsum", "A very soft white mineral left by drying sea water. It is used for plaster and chalk.", "gypsum", true, false, Episode.Plains); // Draft Plains mineral
        public static readonly ItemKind Mudstone = new ItemKind("mudstone", "Mudstone", "A fine rock made from hardened mud and clay.", "mudstone", true, true, Episode.Plains); // Draft Plains rock

        public static readonly ItemKind Peat = new ItemKind("peat", "Peat", "Soft brown matter made of old, wet plants. It is not a true rock yet, but in time it can become coal.", "peat", true, true, Episode.Plains); // Peat uses the rock shelf while its card explains the difference
        public static readonly ItemKind Laterite = new ItemKind("laterite", "Laterite", "A red, iron-rich rock or soil of hot, wet places. People cut it into building blocks.", "laterite", true, true, Episode.Plains, Episode.Plateaus); // Draft Plains rock, also on the Plateaus shelf (D-072)

        public static readonly ItemKind Chalk = new ItemKind("chalk", "Chalk", "A soft, white rock made of the tiny shells of sea plankton. An old sea once covered the Great Plains.", "chalk", true, true, Episode.Plains); // Draft Plains Hard rock (D-071)
        public static readonly ItemKind Loess = new ItemKind("loess", "Loess", "Fine yellow dust carried by the wind and laid down in thick layers. It is not a true rock, but it makes rich farm soil.", "loess", true, true, Episode.Plains); // Loess uses the rock shelf while its card explains the difference (D-071)
        public static readonly ItemKind Flint = new ItemKind("flint", "Flint", "A very hard stone made of tiny quartz grains. People made tools from it and struck it to make sparks.", "flint", true, true, Episode.Plains); // Draft Plains Hard rock (D-071)

        public static readonly ItemKind Halloysite = new ItemKind("halloysite", "Halloysite", "A soft white clay mineral. It forms when volcanic rock breaks down.", "halloysite", true, false, Episode.Plateaus); // Draft Plateaus mineral (D-072)
        public static readonly ItemKind Goethite = new ItemKind("goethite", "Goethite", "A brown iron mineral. It makes soil look rusty.", "goethite", true, false, Episode.Plateaus); // Draft Plateaus mineral (D-072)
        public static readonly ItemKind Gibbsite = new ItemKind("gibbsite", "Gibbsite", "A soft white mineral rich in aluminium. It forms in warm, wet soils.", "gibbsite", true, false, Episode.Plateaus); // Draft Plateaus mineral (D-072)
        public static readonly ItemKind Cristobalite = new ItemKind("cristobalite", "Cristobalite", "A form of silica, like quartz. It can make tiny white balls in volcanic rock.", "cristobalite", true, false, Episode.Plateaus); // Draft Plateaus mineral (D-072)

        /// <summary>All kinds, in the order the levels introduce them. The Collection shelves keep this order.</summary>
        public static IReadOnlyList<ItemKind> All { get; } = new[] { Granite, Quartz, Feldspar, Mica, Andesite, Obsidian, Pumice, Slate, Marble, Garnet, Sandstone, Clay, Conglomerate, Shale, Basalt, Magnetite, Sulfur, Pyrite, Limestone, Gneiss, Scoria, Hornblende, Diorite, Gold, Tuff, Hematite, Serpentinite, Chromite, Schist, Calcite, Trachyte, Tanzanite, Gabbro, Olivine, CoralStone, Limonite, Kaolinite, Aragonite, Coquina, Flowstone, Dripstone, VolcanicBreccia, Ignimbrite, Zeolite, Peridotite, Chlorite, Dacite, Quartzite, Travertine, Leucite, Siltstone, Halite, Gypsum, Mudstone, Peat, Laterite, Chalk, Loess, Flint, Halloysite, Goethite, Gibbsite, Cristobalite }; // Appends the Plains Normal and Plains Hard kinds, then the four Plateaus Easy kinds (D-072)

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
                new[] { new Spot(2400f, 0f) }, // Checkpoint flag in the middle
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
                new[] { new Spot(2700f, 0f) }, // Checkpoint flag in the middle
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
                new[] { new Spot(2500f, 0f) }, // Checkpoint flag in the middle
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

            new LevelData(Episode.Mountains, Difficulty.Normal, 1, // Mount Pulag
                new Lesson("Weathering", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mount Pulag is our third highest mountain.", // Exact draft lesson sentence
                    "On cold nights, frost forms on its peak.", // Exact draft lesson sentence
                    "Ice, rain and roots slowly crack the rock.", // Exact draft lesson sentence
                    "This breaking of rock is called weathering.", // Exact draft lesson sentence
                }, "lesson_pulag", true), // Lesson picture and draft mark
                new[] // Seven placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Sandstone, new Spot(732f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(1236f, 64f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(2560f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Feldspar, new Spot(3000f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sandstone, new Spot(3684f, 72f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(5232f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(6500f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // Two safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(900f, 0f), 48f), new Hazard(HazardType.Pit, new Spot(2000f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(3300f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(5200f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(6200f, 0f), 96f), new Hazard(HazardType.Spikes, new Spot(1700f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4150f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5550f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6850f, 0f), 32f) }, // Pits and spikes
                new Spot(7040f, 0f), // Goal
                new[] { "Weathering breaks rock into small pieces.", "Feldspar can weather into soft clay.", "Pieces of rock and clay help make soil." }, // Exact draft review
                new LevelLayout(7200f, LevelTheme.PineForest, // Checked trail width
                    new[] { B(420f, BoulderSize.Small), B(1480f, BoulderSize.Big), B(2650f, BoulderSize.Small), B(3950f, BoulderSize.Big), B(4450f, BoulderSize.Small), B(5850f, BoulderSize.Big), B(6650f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(700f, 40f, 64f), new Ledge(3580f, 40f, 64f), new Ledge(3660f, 72f, 48f) }, // Fixed ledges
                    vents: new SteamVent[0], // Steam vents
                    crumbleLedges: new[] { new CrumbleLedge(1150f, 40f), new CrumbleLedge(1220f, 64f), new CrumbleLedge(2000f, 0f), new CrumbleLedge(2032f, 0f), new CrumbleLedge(2064f, 0f), new CrumbleLedge(6200f, 0f), new CrumbleLedge(6232f, 0f), new CrumbleLedge(6264f, 0f) }, // Breakable slabs
                    rockfalls: new Rockfall[0], // Falling rocks
                    movingPlatforms: new MovingPlatform[0], // Moving crossings
                    windZones: new WindZone[0]), // Gust areas
                "Mount Pulag"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Normal, 2, // Mount Pinatubo
                new Lesson("Erosion", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mount Pinatubo erupted in 1991.", // Exact draft lesson sentence
                    "Thick ash covered its slopes.", // Exact draft lesson sentence
                    "Heavy rain washed the ash down as lahar.", // Exact draft lesson sentence
                    "Carrying rock and soil away is erosion.", // Exact draft lesson sentence
                }, "lesson_pinatubo", true), // Lesson picture and draft mark
                new[] // Seven placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Conglomerate, new Spot(1100f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Shale, new Spot(2232f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Pumice, new Spot(2528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Quartz, new Spot(3000f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Conglomerate, new Spot(4340f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Shale, new Spot(5430f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Pumice, new Spot(6232f, 40f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // Two safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(800f, 0f), 48f), new Hazard(HazardType.Pit, new Spot(2500f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(3500f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(5100f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(6200f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1300f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4440f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6880f, 0f), 32f) }, // Pits and spikes
                new Spot(7040f, 0f), // Goal
                new[] { "Erosion carries rock and soil to new places.", "Rivers round pebbles that end up in conglomerate.", "Mud that settles can harden into shale." }, // Exact draft review
                new LevelLayout(7200f, LevelTheme.AshGorge, // Checked trail width
                    new[] { B(500f, BoulderSize.Small), B(1500f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(4000f, BoulderSize.Big), B(4600f, BoulderSize.Small), B(5900f, BoulderSize.Big), B(6700f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(2200f, 40f, 64f), new Ledge(4300f, 48f, 80f) }, // Fixed ledges
                    vents: new SteamVent[0], // Steam vents
                    crumbleLedges: new CrumbleLedge[0], // Breakable slabs
                    rockfalls: new[] { new Rockfall(1100f, 0f), new Rockfall(1900f, 1f), new Rockfall(3000f, 0.5f), new Rockfall(3700f, 2f), new Rockfall(5400f, 0f), new Rockfall(5460f, 1.5f), new Rockfall(6500f, 1f) }, // Falling rocks
                    movingPlatforms: new MovingPlatform[0], // Moving crossings
                    windZones: new WindZone[0]), // Gust areas
                "Mount Pinatubo"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Normal, 3, // Mount Matutum
                new Lesson("Mountains and Water", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Matutum is near General Santos City.", // Exact draft lesson sentence
                    "Its forest soaks up rain like a sponge.", // Exact draft lesson sentence
                    "Tree roots hold the soil in place.", // Exact draft lesson sentence
                    "Its rivers bring water to nearby towns.", // Exact draft lesson sentence
                }, "lesson_matutum", true), // Lesson picture and draft mark
                new[] // Seven placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Basalt, new Spot(1292f, 72f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Magnetite, new Spot(2100f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Andesite, new Spot(3016f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Basalt, new Spot(3732f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Magnetite, new Spot(4224f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Andesite, new Spot(5300f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Basalt, new Spot(6900f, 0f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // Two safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(2000f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(3400f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(5200f, 0f), 240f), new Hazard(HazardType.Pit, new Spot(6100f, 0f), 200f), new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(3000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4600f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6600f, 0f), 32f) }, // Pits and spikes
                new Spot(7040f, 0f), // Goal
                new[] { "Forests on mountains hold soil and store water.", "Rivers can carry black magnetite sand.", "Caring for forests protects our water." }, // Exact draft review
                new LevelLayout(7200f, LevelTheme.MossyForest, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1600f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3900f, BoulderSize.Big), B(4450f, BoulderSize.Small), B(5700f, BoulderSize.Big), B(6750f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1260f, 72f, 64f), new Ledge(3700f, 40f, 64f), new Ledge(4200f, 48f, 48f) }, // Fixed ledges
                    vents: new SteamVent[0], // Steam vents
                    crumbleLedges: new CrumbleLedge[0], // Breakable slabs
                    rockfalls: new Rockfall[0], // Falling rocks
                    movingPlatforms: new[] { new MovingPlatform(1000f, 1200f, 40f), new MovingPlatform(2000f, 2152f, 0f), new MovingPlatform(5200f, 5392f, 0f), new MovingPlatform(6100f, 6252f, 0f) }, // Moving crossings
                    windZones: new WindZone[0]), // Gust areas
                "Mount Matutum"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Normal, 4, // Mount Apo
                new Lesson("Minerals from Mountains", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mount Apo is the highest in the country.", // Exact draft lesson sentence
                    "Hot steam comes out of vents near its top.", // Exact draft lesson sentence
                    "Yellow sulfur forms around these vents.", // Exact draft lesson sentence
                    "Mountains give us many useful minerals.", // Exact draft lesson sentence
                }, "lesson_apo", true), // Lesson picture and draft mark
                new[] // Seven placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Pyrite, new Spot(1560f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Obsidian, new Spot(2200f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sulfur, new Spot(2976f, 100f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Quartz, new Spot(3632f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Pyrite, new Spot(4082f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sulfur, new Spot(5476f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Obsidian, new Spot(6950f, 0f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // Two safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(800f, 0f), 48f), new Hazard(HazardType.Pit, new Spot(1500f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(3600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(5400f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(6300f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1100f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(3900f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6700f, 0f), 32f) }, // Pits and spikes
                new Spot(7040f, 0f), // Goal
                new[] { "Mountains hold many minerals we use.", "Sulfur is used to make medicine and fertilizer.", "Pyrite looks like gold, but it is not gold." }, // Exact draft review
                new LevelLayout(7200f, LevelTheme.MossyForest, // Checked trail width
                    new[] { B(500f, BoulderSize.Small), B(1900f, BoulderSize.Big), B(2600f, BoulderSize.Small), B(3300f, BoulderSize.Big), B(4600f, BoulderSize.Small), B(5000f, BoulderSize.Big), B(6500f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(2940f, 100f, 72f), new Ledge(4050f, 48f, 64f) }, // Fixed ledges
                    vents: new[] { new SteamVent(2900f) }, // Steam vents
                    crumbleLedges: new[] { new CrumbleLedge(1500f, 0f), new CrumbleLedge(1532f, 0f), new CrumbleLedge(1564f, 0f) }, // Breakable slabs
                    rockfalls: new[] { new Rockfall(2200f, 0f), new Rockfall(4300f, 1.5f) }, // Falling rocks
                    movingPlatforms: new[] { new MovingPlatform(5400f, 5552f, 0f) }, // Moving crossings
                    windZones: new WindZone[0]), // Gust areas
                "Mount Apo"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Normal, 5, // Mount Everest
                new Lesson("The Highest Mountain", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mount Everest is the highest on Earth.", // Exact draft lesson sentence
                    "Moving plates still push it up slowly.", // Exact draft lesson sentence
                    "Its top is cold, windy and snowy.", // Exact draft lesson sentence
                    "Rock at its top was once under the sea.", // Exact draft lesson sentence
                }, "lesson_everest", true), // Lesson picture and draft mark
                new[] // Seven placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Limestone, new Spot(1232f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gneiss, new Spot(2224f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Marble, new Spot(2916f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Garnet, new Spot(3528f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(4804f, 72f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gneiss, new Spot(5532f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Marble, new Spot(6900f, 0f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // Two safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(2200f, 0f), 48f), new Hazard(HazardType.Pit, new Spot(3500f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(4000f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(5500f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6500f, 0f), 32f) }, // Pits and spikes
                new Spot(7040f, 0f), // Goal
                new[] { "Moving plates lift mountains very slowly.", "Limestone at Everest's top holds old sea fossils.", "Heat and squeezing turn rock into gneiss." }, // Exact draft review
                new LevelLayout(7200f, LevelTheme.SnowPeak, // Checked trail width
                    new[] { B(600f, BoulderSize.Small), B(1700f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3800f, BoulderSize.Big), B(4600f, BoulderSize.Small), B(6000f, BoulderSize.Big), B(6800f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1200f, 48f, 64f), new Ledge(4700f, 40f, 64f), new Ledge(4780f, 72f, 48f) }, // Fixed ledges
                    vents: new SteamVent[0], // Steam vents
                    crumbleLedges: new[] { new CrumbleLedge(4000f, 0f), new CrumbleLedge(4032f, 0f), new CrumbleLedge(4064f, 0f) }, // Breakable slabs
                    rockfalls: new[] { new Rockfall(4300f, 0f) }, // Falling rocks
                    movingPlatforms: new MovingPlatform[0], // Moving crossings
                    windZones: new[] { new WindZone(900f, 500f, 0f), new WindZone(2000f, 400f, 1f), new WindZone(3300f, 600f, 2f), new WindZone(5300f, 500f, 0.5f), new WindZone(6200f, 500f, 1.5f) }), // Gust areas
                "Mount Everest"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 1, // Mount Mayon
                new Lesson("Volcano Safety", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mayon in Albay is our most active volcano.", // Exact draft lesson sentence
                    "It has an almost perfect cone shape.", // Exact draft lesson sentence
                    "Experts watch it and set alert levels.", // Exact draft lesson sentence
                    "Stay out of the danger zone near it.", // Exact draft lesson sentence
                }, "lesson_mayon", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Scoria, new Spot(1032f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hornblende, new Spot(1528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Scoria, new Spot(2600f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Andesite, new Spot(3732f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hornblende, new Spot(4232f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Scoria, new Spot(6332f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hornblende, new Spot(6404f, 72f)), // Secret pickup
                    new Pickup(ItemCatalog.Scoria, new Spot(8732f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hornblende, new Spot(9216f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(3000f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(4200f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(6600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(8200f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2800f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5400f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(8400f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Active volcanoes can erupt again.", "Experts at PHIVOLCS watch them and warn people.", "Follow the alert levels and stay safe." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.LavaSlope, // Checked trail width
                    new[] { B(400f, BoulderSize.Small), B(1650f, BoulderSize.Big), B(2150f, BoulderSize.Small), B(3250f, BoulderSize.Big), B(5800f, BoulderSize.Small), B(6800f, BoulderSize.Big), B(7350f, BoulderSize.Small), B(7950f, BoulderSize.Big), B(9200f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1000f, 40f, 64f), new Ledge(3700f, 48f, 64f), new Ledge(6300f, 40f, 64f), new Ledge(6380f, 72f, 48f), new Ledge(8700f, 48f, 64f) }, // Fixed ledges
                    rockfalls: new[] { new Rockfall(2600f, 0f), new Rockfall(4500f, 1f), new Rockfall(8600f, 0.5f) }, // Falling rocks
                    rollingBoulders: new[] { new RollingBoulder(900f, 700f, 0f), new RollingBoulder(2000f, 1800f, 1f), new RollingBoulder(3600f, 3380f, 0f), new RollingBoulder(5200f, 4980f, 2f), new RollingBoulder(6200f, 5980f, 0.5f), new RollingBoulder(7800f, 7580f, 1.5f), new RollingBoulder(9000f, 8780f, 0f) }), // Rolling boulders
                "Mount Mayon"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 2, // Sierra Madre
                new Lesson("Mountain Ranges", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "A range is a long row of mountains.", // Exact draft lesson sentence
                    "The Sierra Madre is our longest range.", // Exact draft lesson sentence
                    "It runs along the east side of Luzon.", // Exact draft lesson sentence
                    "It helps weaken some strong typhoons.", // Exact draft lesson sentence
                }, "lesson_sierra_madre", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Diorite, new Spot(1732f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(2932f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Diorite, new Spot(3232f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(3974f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(5086f, 64f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Diorite, new Spot(6832f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(7804f, 72f)), // Secret pickup
                    new Pickup(ItemCatalog.Diorite, new Spot(8532f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(8966f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1300f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(2900f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(4100f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(5600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(6800f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(8500f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2600f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(3650f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4300f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6200f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7400f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(8300f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Mountains in a row make a mountain range.", "The Sierra Madre helps slow typhoon winds and rain.", "Its forests need our care too." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.StormRidge, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1600f, BoulderSize.Big), B(3100f, BoulderSize.Small), B(3950f, BoulderSize.Big), B(5300f, BoulderSize.Small), B(6500f, BoulderSize.Big), B(7000f, BoulderSize.Small), B(8000f, BoulderSize.Big), B(8950f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1700f, 40f, 64f), new Ledge(3200f, 48f, 64f), new Ledge(7700f, 40f, 64f), new Ledge(7780f, 72f, 48f) }, // Fixed ledges
                    crumbleLedges: new[] { new CrumbleLedge(2900f, 0f), new CrumbleLedge(2932f, 0f), new CrumbleLedge(2964f, 0f), new CrumbleLedge(6800f, 0f), new CrumbleLedge(6832f, 0f), new CrumbleLedge(6864f, 0f), new CrumbleLedge(5000f, 40f), new CrumbleLedge(5070f, 64f) }, // Breakable slabs
                    windZones: new[] { new WindZone(700f, 500f, 0f), new WindZone(1900f, 400f, 1f), new WindZone(3300f, 250f, 2f), new WindZone(5800f, 320f, 0.5f), new WindZone(7600f, 600f, 1.5f), new WindZone(8800f, 400f, 0f) }, // Gust areas
                    rollingBoulders: new[] { new RollingBoulder(2200f, 2000f, 0f), new RollingBoulder(4600f, 4380f, 1f), new RollingBoulder(9300f, 9080f, 2f) }), // Rolling boulders
                "Sierra Madre"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 3, // Kitanglad Range
                new Lesson("Mountains Are Homes", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Kitanglad is a mountain range in Bukidnon.", // Exact draft lesson sentence
                    "Dulang-dulang is our second highest peak.", // Exact draft lesson sentence
                    "The Philippine eagle lives in its forest.", // Exact draft lesson sentence
                    "Mountains are homes for many animals.", // Exact draft lesson sentence
                }, "lesson_kitanglad", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Tuff, new Spot(1292f, 72f)), // Secret pickup
                    new Pickup(ItemCatalog.Hematite, new Spot(1900f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(2786f, 64f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hematite, new Spot(3724f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Basalt, new Spot(4100f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(5424f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hematite, new Spot(6500f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(7586f, 64f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Hematite, new Spot(8100f, 32f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1800f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(3300f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(4000f, 0f), 240f), new Hazard(HazardType.Pit, new Spot(5600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(6400f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(8000f, 0f), 240f), new Hazard(HazardType.Pit, new Spot(8900f, 0f), 56f), new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2200f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(3600f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(8700f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Mountain forests are homes for plants and animals.", "The Philippine eagle nests in tall trees.", "Saving forests saves their homes." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.EagleForest, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1300f, BoulderSize.Big), B(2550f, BoulderSize.Small), B(3800f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5800f, BoulderSize.Big), B(6800f, BoulderSize.Small), B(7300f, BoulderSize.Big), B(8500f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1260f, 72f, 64f), new Ledge(3700f, 40f, 64f), new Ledge(5400f, 48f, 48f) }, // Fixed ledges
                    crumbleLedges: new[] { new CrumbleLedge(2700f, 40f), new CrumbleLedge(2770f, 64f), new CrumbleLedge(7500f, 40f), new CrumbleLedge(7570f, 64f) }, // Breakable slabs
                    movingPlatforms: new[] { new MovingPlatform(1000f, 1200f, 40f), new MovingPlatform(1800f, 1952f, 0f), new MovingPlatform(4000f, 4192f, 0f), new MovingPlatform(6400f, 6552f, 0f), new MovingPlatform(8000f, 8192f, 0f) }, // Moving crossings
                    rollingBoulders: new[] { new RollingBoulder(3000f, 2850f, 0f), new RollingBoulder(5300f, 5080f, 1.5f), new RollingBoulder(9300f, 9100f, 0.5f) }), // Rolling boulders
                "Kitanglad Range"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 4, // Mount Hamiguitan
                new Lesson("Caring for Mountains", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mount Hamiguitan is in Davao Oriental.", // Exact draft lesson sentence
                    "Tiny old trees grow on its red soil.", // Exact draft lesson sentence
                    "It is a World Heritage Site we protect.", // Exact draft lesson sentence
                    "Roots hold soil and help stop landslides.", // Exact draft lesson sentence
                }, "lesson_hamiguitan", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Serpentinite, new Spot(1432f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chromite, new Spot(1932f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Serpentinite, new Spot(2600f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chromite, new Spot(3586f, 64f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Magnetite, new Spot(4332f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Serpentinite, new Spot(4932f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chromite, new Spot(6632f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Serpentinite, new Spot(8332f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chromite, new Spot(8886f, 64f)), // Secret pickup
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1100f, 0f), 48f), new Hazard(HazardType.Pit, new Spot(1900f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(3200f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(4300f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(5600f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(6600f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(8300f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(800f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5800f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(9300f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Protected mountains keep forests safe.", "Tree roots hold soil so rain does not wash it away.", "We can help by not cutting trees." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.BonsaiForest, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1300f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3800f, BoulderSize.Big), B(4650f, BoulderSize.Small), B(5400f, BoulderSize.Big), B(7400f, BoulderSize.Small), B(8600f, BoulderSize.Big) }, // Boulders
                    new[] { new Ledge(1400f, 40f, 64f), new Ledge(4900f, 48f, 64f) }, // Fixed ledges
                    crumbleLedges: new[] { new CrumbleLedge(1900f, 0f), new CrumbleLedge(1932f, 0f), new CrumbleLedge(1964f, 0f), new CrumbleLedge(4300f, 0f), new CrumbleLedge(4332f, 0f), new CrumbleLedge(4364f, 0f), new CrumbleLedge(6600f, 0f), new CrumbleLedge(6632f, 0f), new CrumbleLedge(6664f, 0f), new CrumbleLedge(3500f, 40f), new CrumbleLedge(3570f, 64f), new CrumbleLedge(8800f, 40f), new CrumbleLedge(8870f, 64f) }, // Breakable slabs
                    rockfalls: new[] { new Rockfall(2600f, 0f), new Rockfall(5200f, 1.5f), new Rockfall(7600f, 0.5f), new Rockfall(9100f, 1f) }, // Falling rocks
                    rollingBoulders: new[] { new RollingBoulder(3000f, 2800f, 0.5f), new RollingBoulder(6200f, 5980f, 1f), new RollingBoulder(8100f, 7880f, 2f) }), // Rolling boulders
                "Mount Hamiguitan"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 5, // Matterhorn
                new Lesson("Glaciers Shape Mountains", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "The Matterhorn is in the Alps of Europe.", // Exact draft lesson sentence
                    "A glacier is a huge river of slow ice.", // Exact draft lesson sentence
                    "Glaciers carved its four steep sides.", // Exact draft lesson sentence
                    "Ice shapes mountains over many years.", // Exact draft lesson sentence
                }, "lesson_matterhorn", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Schist, new Spot(2032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(2632f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Schist, new Spot(3832f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(4116f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gneiss, new Spot(5432f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Schist, new Spot(5504f, 72f)), // Secret pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(7432f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Schist, new Spot(8432f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(8832f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1300f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(2600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(3800f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(5000f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(6100f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(7400f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(8800f, 0f), 64f), new Hazard(HazardType.Spikes, new Spot(1600f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4400f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5900f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(9020f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Glaciers are rivers of ice that move very slowly.", "As they move, they scrape and carve rock.", "Glaciers gave the Matterhorn its sharp shape." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.GlacierHorn, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1400f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4100f, BoulderSize.Small), B(5500f, BoulderSize.Big), B(6300f, BoulderSize.Small), B(7700f, BoulderSize.Big), B(8300f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(2000f, 48f, 64f), new Ledge(5400f, 40f, 64f), new Ledge(5480f, 72f, 48f), new Ledge(8400f, 48f, 64f) }, // Fixed ledges
                    crumbleLedges: new[] { new CrumbleLedge(3800f, 0f), new CrumbleLedge(3832f, 0f), new CrumbleLedge(3864f, 0f), new CrumbleLedge(7400f, 0f), new CrumbleLedge(7432f, 0f), new CrumbleLedge(7464f, 0f) }, // Breakable slabs
                    windZones: new[] { new WindZone(1800f, 500f, 0f), new WindZone(4200f, 500f, 1f), new WindZone(6500f, 500f, 2f), new WindZone(9080f, 300f, 0.5f) }, // Gust areas
                    rollingBoulders: new[] { new RollingBoulder(4700f, 4480f, 0f), new RollingBoulder(7000f, 6780f, 1.5f), new RollingBoulder(9300f, 9100f, 0.5f) }, // Rolling boulders
                    iceZones: new[] { new IceZone(600f, 600f), new IceZone(1800f, 500f), new IceZone(3000f, 700f), new IceZone(5200f, 600f), new IceZone(6500f, 500f), new IceZone(8000f, 700f) }), // Icy ground
                "Matterhorn"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 6, // Mount Kilimanjaro
                new Lesson("Climbing Changes Weather", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Kilimanjaro is the highest in Africa.", // Exact draft lesson sentence
                    "It is near the equator, but has snow.", // Exact draft lesson sentence
                    "The air gets colder as you climb higher.", // Exact draft lesson sentence
                    "Plants change from forest to bare rock.", // Exact draft lesson sentence
                }, "lesson_kilimanjaro", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Trachyte, new Spot(1576f, 100f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tanzanite, new Spot(2100f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Trachyte, new Spot(2700f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tanzanite, new Spot(3776f, 100f)), // Secret pickup
                    new Pickup(ItemCatalog.Obsidian, new Spot(4332f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Trachyte, new Spot(5332f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tanzanite, new Spot(6800f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Trachyte, new Spot(8632f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tanzanite, new Spot(9028f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1100f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(2000f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(3100f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(4300f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(5300f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(6700f, 0f), 240f), new Hazard(HazardType.Pit, new Spot(7600f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(9000f, 0f), 56f), new Hazard(HazardType.Spikes, new Spot(800f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2600f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(3300f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4650f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(6550f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7400f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "The higher you climb, the colder it gets.", "Kilimanjaro has farms, forest and snow on one mountain.", "Its snow and ice are shrinking." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.EquatorSummit, // Checked trail width
                    new[] { B(450f, BoulderSize.Small), B(1300f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3450f, BoulderSize.Big), B(4550f, BoulderSize.Small), B(5450f, BoulderSize.Big), B(6200f, BoulderSize.Small), B(7000f, BoulderSize.Big), B(8500f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(1540f, 100f, 72f), new Ledge(3740f, 100f, 72f), new Ledge(8600f, 48f, 64f) }, // Fixed ledges
                    vents: new[] { new SteamVent(1500f), new SteamVent(3700f) }, // Steam vents
                    crumbleLedges: new[] { new CrumbleLedge(4300f, 0f), new CrumbleLedge(4332f, 0f), new CrumbleLedge(4364f, 0f) }, // Breakable slabs
                    rockfalls: new[] { new Rockfall(2700f, 0f), new Rockfall(5000f, 1f), new Rockfall(7900f, 0.5f) }, // Falling rocks
                    movingPlatforms: new[] { new MovingPlatform(2000f, 2152f, 0f), new MovingPlatform(6700f, 6892f, 0f) }, // Moving crossings
                    rollingBoulders: new[] { new RollingBoulder(4250f, 4060f, 1f), new RollingBoulder(5800f, 5580f, 0f), new RollingBoulder(9300f, 9080f, 2f) }, // Rolling boulders
                    iceZones: new[] { new IceZone(6000f, 500f), new IceZone(8200f, 700f) }), // Icy ground
                "Mount Kilimanjaro"), // Start banner
            new LevelData(Episode.Mountains, Difficulty.Hard, 7, // Mauna Kea
                new Lesson("Mountains Under the Sea", new[] // Four draft lesson sentences
                { // Opens the lesson
                    "Mauna Kea is a volcano in Hawaii.", // Exact draft lesson sentence
                    "Most of it is hidden under the sea.", // Exact draft lesson sentence
                    "From its base, it is taller than Everest.", // Exact draft lesson sentence
                    "Many mountains stand on the sea floor.", // Exact draft lesson sentence
                }, "lesson_mauna_kea", true), // Lesson picture and draft mark
                new[] // Nine placed pickups
                { // Opens the pickup list
                    new Pickup(ItemCatalog.Gabbro, new Spot(1932f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Olivine, new Spot(2276f, 100f)), // Secret pickup
                    new Pickup(ItemCatalog.Gabbro, new Spot(3100f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Olivine, new Spot(4516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Basalt, new Spot(5532f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gabbro, new Spot(6732f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Olivine, new Spot(7716f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gabbro, new Spot(8032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Olivine, new Spot(9032f, 40f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Three safe flags
                new[] { new Hazard(HazardType.Pit, new Spot(1000f, 0f), 56f), new Hazard(HazardType.Pit, new Spot(1900f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(3000f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(4200f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(5500f, 0f), 96f), new Hazard(HazardType.Pit, new Spot(6400f, 0f), 200f), new Hazard(HazardType.Pit, new Spot(8000f, 0f), 64f), new Hazard(HazardType.Pit, new Spot(8900f, 0f), 56f), new Hazard(HazardType.Spikes, new Spot(700f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(2750f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(4000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(5000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(8300f, 0f), 32f), new Hazard(HazardType.Spikes, new Spot(9200f, 0f), 32f) }, // Pits and spikes
                new Spot(9440f, 0f), // Goal
                new[] { "Some mountains start at the bottom of the sea.", "From its base, Mauna Kea is over 10,000 m tall.", "Its top is a good place to watch stars." }, // Exact draft review
                new LevelLayout(9600f, LevelTheme.SeaPeak, // Checked trail width
                    new[] { B(400f, BoulderSize.Small), B(1250f, BoulderSize.Big), B(2850f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(6200f, BoulderSize.Big), B(7700f, BoulderSize.Small), B(8750f, BoulderSize.Small) }, // Boulders
                    new[] { new Ledge(2240f, 100f, 72f), new Ledge(6700f, 48f, 64f), new Ledge(9000f, 40f, 64f) }, // Fixed ledges
                    vents: new[] { new SteamVent(2200f) }, // Steam vents
                    crumbleLedges: new[] { new CrumbleLedge(1900f, 0f), new CrumbleLedge(1932f, 0f), new CrumbleLedge(1964f, 0f), new CrumbleLedge(5500f, 0f), new CrumbleLedge(5532f, 0f), new CrumbleLedge(5564f, 0f) }, // Breakable slabs
                    rockfalls: new[] { new Rockfall(2600f, 0f), new Rockfall(5100f, 1.5f) }, // Falling rocks
                    movingPlatforms: new[] { new MovingPlatform(3000f, 3152f, 0f), new MovingPlatform(6400f, 6552f, 0f) }, // Moving crossings
                    windZones: new[] { new WindZone(7400f, 500f, 0f) }, // Gust areas
                    rollingBoulders: new[] { new RollingBoulder(1600f, 1400f, 0f), new RollingBoulder(3900f, 3680f, 1f), new RollingBoulder(6000f, 5780f, 2f), new RollingBoulder(8600f, 8380f, 0.5f) }, // Rolling boulders
                    iceZones: new[] { new IceZone(4400f, 300f), new IceZone(7500f, 400f) }), // Icy ground
                "Mauna Kea"), // Start banner
            // Hills, Easy 1 to 3 (D-066, spec section 6). Drafts like the rest (Q-04). E3 has four soft sand zones.
            new LevelData(Episode.Hills, Difficulty.Easy, 1, // Hills, Easy, level 1: Chocolate Hills
                new Lesson("Hills", new[] // Draft lesson for Grade 5
                {
                    "Hills are lower and rounder than mountains.", // What a hill is
                    "Bohol has over 1,200 Chocolate Hills.", // How many there are
                    "They are made of old sea limestone.", // What they are made of
                    "Dry grass turns them brown like chocolate.", // Why they look brown
                }, "lesson_chocolate_hills", true), // Picture name, and still a draft
                new[] // Five collectibles
                {
                    new Pickup(ItemCatalog.Limestone, new Spot(316f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Calcite, new Spot(560f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Clay, new Spot(1140f, 28f)), // On a low ledge
                    new Pickup(ItemCatalog.CoralStone, new Spot(3668f, 56f)), // Secret spot: the high ledge after the step
                    new Pickup(ItemCatalog.Limestone, new Spot(4600f, 16f)), // Near the viewing deck
                },
                new Spot(32f, 0f), // Start at the left edge
                new[] { new Spot(2400f, 0f) }, // Checkpoint flag in the middle
                new[] // Two pits and one spike trap
                {
                    new Hazard(HazardType.Pit, new Spot(1880f, 0f), 48f), // Pit before the checkpoint
                    new Hazard(HazardType.Pit, new Spot(3150f, 0f), 48f), // Pit after the checkpoint
                    new Hazard(HazardType.Spikes, new Spot(3450f, 0f), 32f), // Spikes after the pits
                },
                new Spot(4740f, 0f), // Viewing deck at the goal
                new[] { "Hills are lower and rounder than mountains.", "The Chocolate Hills are limestone from an old sea floor.", "Limestone has calcite in it, and the hills sit on clay." }, // Draft review
                new LevelLayout(4800f, LevelTheme.ChocolateHills, // Width and theme
                    new[] { B(300f, BoulderSize.Small), B(900f, BoulderSize.Big), B(1400f, BoulderSize.Small), B(2150f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3300f, BoulderSize.Big), B(3900f, BoulderSize.Small), B(4300f, BoulderSize.Big) }, // Eight boulders
                    new[] { L(520f, 40f, 80f), L(1100f, 28f, 80f), L(1600f, 40f, 64f), L(2950f, 28f, 80f), L(3560f, 28f, 48f), L(3620f, 56f, 96f), L(4000f, 28f, 80f) }), // Seven ledges
                "Chocolate Hills"), // Start banner

            new LevelData(Episode.Hills, Difficulty.Easy, 2, // Hills, Easy, level 2: Batanes Hills
                new Lesson("Rolling Hills", new[] // Draft lesson for Grade 5
                {
                    "Batanes has wide, rolling green hills.", // Where the hills are
                    "Grass roots hold the hill soil in place.", // Why grass helps
                    "Cows and goats eat grass on the hills.", // Animals on the hills
                    "Too much grazing can leave soil bare.", // What can go wrong
                }, "lesson_batanes", true), // Picture name, and still a draft
                new[] // Five collectibles
                {
                    new Pickup(ItemCatalog.Andesite, new Spot(376f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Limonite, new Spot(1332f, 28f)), // On a low ledge
                    new Pickup(ItemCatalog.Kaolinite, new Spot(2082f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.CoralStone, new Spot(3112f, 84f)), // Secret spot: climb to the high ledge
                    new Pickup(ItemCatalog.Andesite, new Spot(4316f, 24f)), // On a boulder near the house
                },
                new Spot(32f, 0f), // Start at the left edge
                new[] { new Spot(2700f, 0f) }, // Checkpoint flag in the middle
                new[] // Two pits and two spike traps
                {
                    new Hazard(HazardType.Pit, new Spot(700f, 0f), 48f), // Pit near the start
                    new Hazard(HazardType.Spikes, new Spot(1800f, 0f), 32f), // Spikes before the checkpoint
                    new Hazard(HazardType.Pit, new Spot(3380f, 0f), 56f), // Pit after the checkpoint
                    new Hazard(HazardType.Spikes, new Spot(3800f, 0f), 32f), // Spikes near the house
                },
                new Spot(5240f, 0f), // Stone house at the goal
                new[] { "Rolling hills have gentle, rounded slopes.", "Grass and roots keep hill soil from washing away.", "Feldspar in rock slowly breaks down into kaolinite clay." }, // Draft review
                new LevelLayout(5400f, LevelTheme.RollingPasture, // Width and theme
                    new[] { B(360f, BoulderSize.Small), B(1550f, BoulderSize.Big), B(2350f, BoulderSize.Small), B(4300f, BoulderSize.Small), B(4700f, BoulderSize.Big) }, // Five boulders
                    new[] { L(1300f, 28f, 80f), L(2050f, 40f, 64f), L(3000f, 40f, 64f), L(3080f, 84f, 72f), L(3300f, 28f, 64f), L(3452f, 28f, 64f), L(4900f, 28f, 64f) }), // Seven ledges
                "Batanes Hills"), // Start banner

            new LevelData(Episode.Hills, Difficulty.Easy, 3, // Hills, Easy, level 3: La Paz Dunes
                new Lesson("Sand Dunes", new[] // Draft lesson for Grade 5
                {
                    "Wind can pile sand into hills called dunes.", // What a dune is
                    "La Paz in Ilocos Norte has big sand dunes.", // Where to see them
                    "Sea and river bring the sand to the coast.", // Where the sand comes from
                    "Plants on dunes help keep the sand still.", // How dunes stay put
                }, "lesson_la_paz", true), // Picture name, and still a draft
                new[] // Five collectibles
                {
                    new Pickup(ItemCatalog.Quartz, new Spot(500f, 72f)), // On top of the stepped climb
                    new Pickup(ItemCatalog.Magnetite, new Spot(1532f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Limonite, new Spot(3266f, 0f)), // On the ground between the two spike traps
                    new Pickup(ItemCatalog.Feldspar, new Spot(3512f, 84f)), // Secret spot: climb back up to the high ledge
                    new Pickup(ItemCatalog.Quartz, new Spot(4416f, 24f)), // On a boulder near the dune marker
                },
                new Spot(32f, 0f), // Start at the left edge
                new[] { new Spot(2500f, 0f) }, // Checkpoint flag in the middle
                new[] // Three pits and three spike traps
                {
                    new Hazard(HazardType.Pit, new Spot(800f, 0f), 48f), // Pit 1: one jump clears it
                    new Hazard(HazardType.Spikes, new Spot(1300f, 0f), 32f), // Spikes 1
                    new Hazard(HazardType.Pit, new Spot(2800f, 0f), 64f), // The wide pit: needs the double jump
                    new Hazard(HazardType.Spikes, new Spot(3200f, 0f), 32f), // First of the spike pair
                    new Hazard(HazardType.Spikes, new Spot(3300f, 0f), 32f), // Second of the spike pair, with limonite between them
                    new Hazard(HazardType.Pit, new Spot(3900f, 0f), 56f), // Pit 3
                },
                new Spot(5240f, 0f), // Dune marker at the goal
                new[] { "Dunes are hills of sand piled up by the wind.", "Most sand grains are quartz, a tough mineral.", "The black grains in Ilocos sand are magnetite." }, // Draft review
                new LevelLayout(5400f, LevelTheme.SandDunes, // Width and theme
                    new[] { B(1050f, BoulderSize.Big), B(1800f, BoulderSize.Small), B(4400f, BoulderSize.Small), B(4700f, BoulderSize.Big), B(5000f, BoulderSize.Small) }, // Five boulders
                    new[] { L(300f, 24f, 64f), L(380f, 48f, 64f), L(460f, 72f, 80f), L(1500f, 40f, 64f), L(2000f, 28f, 80f), L(3500f, 28f, 56f), L(3580f, 56f, 56f), L(3480f, 84f, 64f) }, // Eight ledges
                    sandZones: new[] { new SandZone(560f, 160f), new SandZone(1900f, 400f), new SandZone(2640f, 150f), new SandZone(4050f, 330f) }), // Four soft sand zones; the first is safe ground
                "La Paz Dunes"), // Start banner
            new LevelData(Episode.Hills, Difficulty.Normal, 1, // Hundred Islands
                new Lesson("Hundred Islands", new[] // Four draft lesson lines from section 7
                { // Opens the lesson
                    "Hundred Islands is in Pangasinan.", // The authored lesson line
                    "Its hills are old coral reefs.", // The authored lesson line
                    "Waves wore them into small hills.", // The authored lesson line
                    "Corals build reefs from aragonite.", // The authored lesson line
                }, "lesson_hundred_islands", true), // Lesson art and the draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Aragonite, new Spot(436f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Coquina, new Spot(848f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.CoralStone, new Spot(1232f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Aragonite, new Spot(2600f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.CoralStone, new Spot(3524f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Coquina, new Spot(4312f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Aragonite, new Spot(6716f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two safe flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1800f, 0f), 104f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(3500f, 0f), 48f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(6100f, 0f), 192f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5300f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Goal near the right end
                new[] // Three draft review lines from section 7
                { // Opens the review
                    "Hundred Islands are hills made from old coral reefs.", // The authored review line
                    "Corals build their reefs from aragonite.", // The authored review line
                    "Coquina is a rock made of broken shells.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.IslandHills, // Width and theme
                    new[] { B(420f, BoulderSize.Small), B(1500f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3900f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5900f, BoulderSize.Big), B(6700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1200f, 40f, 64f), L(4200f, 40f, 64f), L(4280f, 84f, 64f) }, // Ledges from the checked layout
                    floats: new[] { new SinkingFloat(824f, 0f, 800f), new SinkingFloat(1828f, 0f, 1800f), new SinkingFloat(6132f, 0f, 6100f), new SinkingFloat(6212f, 0f, 6100f) }, // Four rafts at the water line
                    pools: new[] { new ShallowPool(800f, 96f) }), // The practice pool has a safe floor
                "Hundred Islands"), // Start banner
            new LevelData(Episode.Hills, Difficulty.Normal, 2, // Callao Cave
                new Lesson("Callao Cave", new[] // Four draft lesson lines from section 7
                { // Opens the lesson
                    "Callao Cave is in a limestone hill.", // The authored lesson line
                    "Rain slowly dissolves limestone.", // The authored lesson line
                    "Drips build stalactites and flowstone.", // The authored lesson line
                    "Do not disturb bats in caves.", // The authored lesson line
                }, "lesson_callao_cave", true), // Lesson art and the draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Flowstone, new Spot(516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Dripstone, new Spot(1120f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(2600f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Flowstone, new Spot(3332f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Dripstone, new Spot(5112f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Calcite, new Spot(6550f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Flowstone, new Spot(6716f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two safe flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(900f, 0f), 48f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3300f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(5600f, 0f), 56f), // Pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1700f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6300f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Goal near the right end
                new[] // Three draft review lines from section 7
                { // Opens the review
                    "Callao Cave formed inside a limestone hill.", // The authored review line
                    "Dripping water leaves dripstone and flowstone.", // The authored review line
                    "Calcite is the mineral in cave stone.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.LimestoneCave, // Width and theme
                    new[] { B(500f, BoulderSize.Small), B(2000f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(4200f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5200f, BoulderSize.Big), B(6700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1088f, 40f, 64f), L(5000f, 40f, 64f), L(5080f, 84f, 64f) }, // Ledges from the checked layout
                    bats: new[] { new Bat(1350f, 16f, 24f, 12f, 3f, 0f), new Bat(3800f, 16f, 24f, 12f, 2.6f, 0.5f), new Bat(6000f, 16f, 24f, 12f, 3.2f, 1.2f) }, // Three bat loops
                    ambienceOverride: "amb_cave_drips"), // The cave has its own ambience
                "Callao Cave"), // Start banner
            new LevelData(Episode.Hills, Difficulty.Normal, 3, // Pagsanjan Gorge
                new Lesson("Pagsanjan Gorge", new[] // Four draft lesson lines from section 7
                { // Opens the lesson
                    "Pagsanjan Gorge is in Laguna.", // The authored lesson line
                    "A river cut this deep, narrow valley.", // The authored lesson line
                    "Water and stones wear the rock away.", // The authored lesson line
                    "Boatmen paddle up to the falls.", // The authored lesson line
                }, "lesson_pagsanjan", true), // Lesson art and the draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.VolcanicBreccia, new Spot(516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(1120f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Andesite, new Spot(1932f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.VolcanicBreccia, new Spot(3160f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(4112f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Andesite, new Spot(5832f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.VolcanicBreccia, new Spot(6766f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two safe flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1700f, 0f), 56f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3700f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(5800f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(2000f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4250f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6400f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Goal near the right end
                new[] // Three draft review lines from section 7
                { // Opens the review
                    "Rivers slowly cut valleys into the land.", // The authored review line
                    "Pagsanjan Gorge was cut by a river.", // The authored review line
                    "Volcanic breccia is broken volcanic rock glued by ash.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.RiverGorge, // Width and theme
                    new[] { B(500f, BoulderSize.Small), B(1450f, BoulderSize.Big), B(2650f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4550f, BoulderSize.Small), B(6050f, BoulderSize.Big), B(6750f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1900f, 40f, 64f), L(4000f, 40f, 64f), L(4080f, 84f, 64f) }, // Ledges from the checked layout
                    currents: new[] { new CurrentZone(1000f, 240f, 1), new CurrentZone(3000f, 320f, -1), new CurrentZone(5300f, 300f, 1) }), // Three currents with the middle one flowing left
                "Pagsanjan Gorge"), // Start banner
            new LevelData(Episode.Hills, Difficulty.Normal, 4, // Banaue Rice Terraces
                new Lesson("Banaue Rice Terraces", new[] // Four draft lesson lines from section 7
                { // Opens the lesson
                    "Ifugao farmers built the rice terraces.", // The authored lesson line
                    "Terraces are steps cut into slopes.", // The authored lesson line
                    "Steps slow water and keep soil in place.", // The authored lesson line
                    "The terraces have clay-rich soil.", // The authored lesson line
                }, "lesson_banaue", true), // Lesson art and the draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Clay, new Spot(516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Kaolinite, new Spot(748f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(2032f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Kaolinite, new Spot(3356f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(4112f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Kaolinite, new Spot(5360f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(6766f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two safe flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1200f, 0f), 56f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3300f, 0f), 112f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(5900f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1800f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4300f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6500f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Goal near the right end
                new[] // Three draft review lines from section 7
                { // Opens the review
                    "Rice terraces are steps on hill and mountain slopes.", // The authored review line
                    "Terraces slow water and keep soil from washing away.", // The authored review line
                    "Clay helps the terrace soil hold water.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.RiceTerraces, // Width and theme
                    new[] { B(500f, BoulderSize.Small), B(1500f, BoulderSize.Big), B(2650f, BoulderSize.Small), B(3800f, BoulderSize.Big), B(4550f, BoulderSize.Small), B(6200f, BoulderSize.Big), B(6750f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(700f, 32f, 96f), L(2000f, 40f, 64f), L(4000f, 40f, 64f), L(4080f, 84f, 64f) }, // Ledges from the checked layout
                    floats: new[] { new SinkingFloat(3332f, 0f, 3300f) }, // One raft over the terrace pond
                    bats: new[] { new Bat(2900f, 16f, 24f, 12f, 3f, 0f) }, // One bat over the trail
                    currents: new[] { new CurrentZone(5200f, 280f, -1) }), // One irrigation channel flows left
                "Banaue Rice Terraces"), // Start banner
            new LevelData(Episode.Hills, Difficulty.Normal, 5, // Cappadocia
                new Lesson("Cappadocia", new[] // Four draft lesson lines from section 7
                { // Opens the lesson
                    "Cappadocia is in Türkiye.", // The authored lesson line
                    "Volcanoes once covered it in thick ash.", // The authored lesson line
                    "Wind and rain carved fairy chimneys.", // The authored lesson line
                    "Hard ignimbrite caps protect them.", // The authored lesson line
                }, "lesson_cappadocia", true), // Lesson art and the draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Ignimbrite, new Spot(516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Zeolite, new Spot(924f, 104f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(1532f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Ignimbrite, new Spot(2924f, 124f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Zeolite, new Spot(4280f, 112f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(5584f, 112f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Ignimbrite, new Spot(6766f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Start at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two safe flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3400f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(4200f, 0f), 160f), // Pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6100f, 0f), 64f), // Pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3700f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6500f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Goal near the right end
                new[] // Three draft review lines from section 7
                { // Opens the review
                    "Fairy chimneys are cones of soft volcanic rock.", // The authored review line
                    "Hard ignimbrite caps keep the rain off them.", // The authored review line
                    "Zeolite forms inside volcanic ash.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.FairyChimneys, // Width and theme
                    new[] { B(500f, BoulderSize.Small), B(1200f, BoulderSize.Big), B(2600f, BoulderSize.Small), B(3100f, BoulderSize.Big), B(5000f, BoulderSize.Small), B(5900f, BoulderSize.Big), B(6750f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(4192f, 112f, 176f), L(5552f, 112f, 64f) }, // Ledges from the checked layout
                    movingPlatforms: new[] { MovingPlatform.Vertical(900f, 24f, 80f, 0f), MovingPlatform.Vertical(2900f, 24f, 96f, 0.5f), MovingPlatform.Vertical(4144f, 24f, 120f, 0f), MovingPlatform.Vertical(5500f, 24f, 104f, 1f) }), // Four balloons with fixed x and different climb heights
                "Cappadocia"), // Start banner
            HillsHard(1, new Lesson("Tarsier Forest", new[] // Tarsier Forest
            { // Opens the draft lesson
                "Hills give homes to many animals.", // Uses the authored lesson line
                "Tarsiers live in Bohol's forest hills.", // Uses the authored lesson line
                "Trees on hills give them food and shade.", // Uses the authored lesson line
                "Keep forests safe so animals have homes.", // Uses the authored lesson line
            }, "lesson_tarsier_forest", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Limestone, new Spot(396f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Peridotite, new Spot(650f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(1528f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Peridotite, new Spot(2632f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(3032f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(4032f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Peridotite, new Spot(6204f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Limestone, new Spot(7824f, 36f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Peridotite, new Spot(8932f, 48f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(3000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(4300f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6500f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8300f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(2800f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3900f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(6900f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(8600f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Hills are homes for animals like tarsiers.", // Uses the authored review line
                    "Forests on hills give food and shelter.", // Uses the authored review line
                    "Peridotite is a dark green rock from deep inside the Earth.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.TarsierForest, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(380f, BoulderSize.Small), // Uses the checked placement
                        B(1000f, BoulderSize.Big), // Uses the checked placement
                        B(1900f, BoulderSize.Small), // Uses the checked placement
                        B(2100f, BoulderSize.Small), // Uses the checked placement
                        B(3500f, BoulderSize.Big), // Uses the checked placement
                        B(4500f, BoulderSize.Small), // Uses the checked placement
                        B(5200f, BoulderSize.Small), // Uses the checked placement
                        B(5800f, BoulderSize.Big), // Uses the checked placement
                        B(6800f, BoulderSize.Small), // Uses the checked placement
                        B(7800f, BoulderSize.Big), // Uses the checked placement
                        B(9000f, BoulderSize.Small) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(2600f, 40f, 64f), // Uses the checked placement
                        L(4000f, 48f, 64f), // Uses the checked placement
                        L(6100f, 40f, 64f), // Uses the checked placement
                        L(6180f, 84f, 48f), // Uses the checked placement
                        L(8900f, 48f, 64f) // Uses the checked placement
                    }, // Ends the placements
                    mudZones: new[] // Slippery stretches
                    { // Opens the placements
                        new MudZone(500f, 300f), // Uses the checked placement
                        new MudZone(1700f, 500f), // Uses the checked placement
                        new MudZone(3200f, 500f), // Uses the checked placement
                        new MudZone(5000f, 400f), // Uses the checked placement
                        new MudZone(6000f, 400f), // Uses the checked placement
                        new MudZone(7500f, 600f), // Uses the checked placement
                        new MudZone(8800f, 400f) // Uses the checked placement
                    }), // Ends the placements
                "Tarsier Forest"), // Shows the banner at the start
            HillsHard(2, new Lesson("Antipolo Slopes", new[] // Antipolo Slopes
            { // Opens the draft lesson
                "Heavy rain can loosen soil on slopes.", // Uses the authored lesson line
                "Watch for new cracks in the ground.", // Uses the authored lesson line
                "Tilting trees or posts are a warning.", // Uses the authored lesson line
                "Move away and tell an adult.", // Uses the authored lesson line
            }, "lesson_antipolo_slope", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Basalt, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Chlorite, new Spot(1528f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Basalt, new Spot(2532f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Chlorite, new Spot(2932f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Chlorite, new Spot(3804f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Basalt, new Spot(4916f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Basalt, new Spot(6732f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Chlorite, new Spot(7324f, 36f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Basalt, new Spot(8532f, 48f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(2900f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(5600f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6700f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8500f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1250f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(2700f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3260f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(4650f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5900f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(8650f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Cracks, tilting trees and muddy springs can warn of a landslide.", // Uses the authored review line
                    "Move away from the slope and tell an adult.", // Uses the authored review line
                    "Chlorite is a soft green mineral found in old basalt.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.RainySlope, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(1650f, BoulderSize.Big), // Uses the checked placement
                        B(2200f, BoulderSize.Small), // Uses the checked placement
                        B(3100f, BoulderSize.Big), // Uses the checked placement
                        B(4900f, BoulderSize.Small), // Uses the checked placement
                        B(5450f, BoulderSize.Big), // Uses the checked placement
                        B(6500f, BoulderSize.Small), // Uses the checked placement
                        B(7300f, BoulderSize.Big), // Uses the checked placement
                        B(8000f, BoulderSize.Small), // Uses the checked placement
                        B(9100f, BoulderSize.Big) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(2500f, 40f, 64f), // Uses the checked placement
                        L(3700f, 40f, 64f), // Uses the checked placement
                        L(3780f, 84f, 48f) // Uses the checked placement
                    }, // Ends the placements
                    logs: new[] // Left and right log routes
                    { // Opens the placements
                        new RollingLog(1000f, 800f, -1, 0f), // Uses the checked placement
                        new RollingLog(2000f, 1800f, -1, 1.0f), // Uses the checked placement
                        new RollingLog(3600f, 3380f, -1, 0f), // Uses the checked placement
                        new RollingLog(4300f, 4520f, 1, 0.5f), // Uses the checked placement
                        new RollingLog(5300f, 5080f, -1, 2.0f), // Uses the checked placement
                        new RollingLog(6100f, 6330f, 1, 1.0f), // Uses the checked placement
                        new RollingLog(7800f, 7580f, -1, 1.5f), // Uses the checked placement
                        new RollingLog(8100f, 8320f, 1, 2.5f), // Uses the checked placement
                        new RollingLog(9000f, 8780f, -1, 0.5f) // Uses the checked placement
                    }), // Ends the placements
                "Antipolo Slopes"), // Shows the banner at the start
            HillsHard(3, new Lesson("Masungi Georeserve", new[] // Masungi Georeserve
            { // Opens the draft lesson
                "Masungi has sharp limestone hills.", // Uses the authored lesson line
                "Rain slowly dissolves the limestone.", // Uses the authored lesson line
                "People made trails to protect it.", // Uses the authored lesson line
                "Stay on trails to keep hills safe.", // Uses the authored lesson line
            }, "lesson_masungi", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Limestone, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(1528f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(2832f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(3052f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(4032f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(5632f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(7004f, 96f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Limestone, new Spot(8032f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(9252f, 48f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(2800f, 0f), 96f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(5600f, 0f), 96f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6600f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8000f, 0f), 96f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8900f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3300f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(4450f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(6250f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(7600f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(8650f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Rain slowly dissolves limestone into sharp shapes.", // Uses the authored review line
                    "Staying on trails protects the hills.", // Uses the authored review line
                    "Calcite is the mineral that makes limestone.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.KarstRidge, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(1000f, BoulderSize.Big), // Uses the checked placement
                        B(2200f, BoulderSize.Small), // Uses the checked placement
                        B(3600f, BoulderSize.Big), // Uses the checked placement
                        B(4300f, BoulderSize.Small), // Uses the checked placement
                        B(5300f, BoulderSize.Big), // Uses the checked placement
                        B(6900f, BoulderSize.Small), // Uses the checked placement
                        B(7400f, BoulderSize.Big), // Uses the checked placement
                        B(8400f, BoulderSize.Small) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(3020f, 40f, 64f), // Uses the checked placement
                        L(6980f, 96f, 48f), // Uses the checked placement
                        L(9220f, 48f, 64f) // Uses the checked placement
                    }, // Ends the placements
                    crumbleLedges: new[] // Crumbling steps and bridges
                    { // Opens the placements
                        new CrumbleLedge(2800f, 0f), // Uses the checked placement
                        new CrumbleLedge(2832f, 0f), // Uses the checked placement
                        new CrumbleLedge(2864f, 0f), // Uses the checked placement
                        new CrumbleLedge(5600f, 0f), // Uses the checked placement
                        new CrumbleLedge(5632f, 0f), // Uses the checked placement
                        new CrumbleLedge(5664f, 0f), // Uses the checked placement
                        new CrumbleLedge(8000f, 0f), // Uses the checked placement
                        new CrumbleLedge(8032f, 0f), // Uses the checked placement
                        new CrumbleLedge(8064f, 0f), // Uses the checked placement
                        new CrumbleLedge(6830f, 40f), // Uses the checked placement
                        new CrumbleLedge(6900f, 64f) // Uses the checked placement
                    }, // Ends the placements
                    bats: new[] // Bat loops
                    { // Opens the placements
                        new Bat(1800f, 16f, 24f, 12f, 3.0f, 0f), // Uses the checked placement
                        new Bat(2150f, 16f, 24f, 12f, 2.8f, 1.0f), // Uses the checked placement
                        new Bat(3800f, 16f, 24f, 12f, 2.6f, 0.4f), // Uses the checked placement
                        new Bat(4600f, 16f, 24f, 12f, 2.8f, 0.6f), // Uses the checked placement
                        new Bat(5950f, 16f, 24f, 12f, 2.6f, 1.2f), // Uses the checked placement
                        new Bat(9150f, 16f, 24f, 12f, 2.6f, 0.3f) // Uses the checked placement
                    }, // Ends the placements
                    mudZones: new[] // Slippery stretches
                    { // Opens the placements
                        new MudZone(500f, 400f), // Uses the checked placement
                        new MudZone(1700f, 400f), // Uses the checked placement
                        new MudZone(3450f, 450f), // Uses the checked placement
                        new MudZone(5000f, 450f), // Uses the checked placement
                        new MudZone(6700f, 120f), // Uses the checked placement
                        new MudZone(7300f, 250f), // Uses the checked placement
                        new MudZone(8150f, 400f) // Uses the checked placement
                    }), // Ends the placements
                "Masungi Georeserve"), // Shows the banner at the start
            HillsHard(4, new Lesson("Pililla Wind Farm", new[] // Pililla Wind Farm
            { // Opens the draft lesson
                "Hilltops are often very windy.", // Uses the authored lesson line
                "Pililla in Rizal has 27 wind turbines.", // Uses the authored lesson line
                "Wind turns them to make electricity.", // Uses the authored lesson line
                "Wind power is clean and never runs out.", // Uses the authored lesson line
            }, "lesson_pililla", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Andesite, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Dacite, new Spot(878f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Andesite, new Spot(2050f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Dacite, new Spot(3732f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Andesite, new Spot(4032f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Dacite, new Spot(6080f, 112f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Dacite, new Spot(7584f, 112f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Andesite, new Spot(8232f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Andesite, new Spot(8516f, 24f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(850f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(2700f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(5300f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6000f, 0f), 160f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8200f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3100f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5700f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(6900f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(9100f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Windy hills are good places for wind turbines.", // Uses the authored review line
                    "Wind power makes clean electricity.", // Uses the authored review line
                    "Dacite is a pale volcanic rock.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.WindHills, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(1350f, BoulderSize.Big), // Uses the checked placement
                        B(2200f, BoulderSize.Small), // Uses the checked placement
                        B(3560f, BoulderSize.Big), // Uses the checked placement
                        B(4200f, BoulderSize.Small), // Uses the checked placement
                        B(5550f, BoulderSize.Big), // Uses the checked placement
                        B(6250f, BoulderSize.Small), // Uses the checked placement
                        B(7000f, BoulderSize.Big), // Uses the checked placement
                        B(8500f, BoulderSize.Small) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(3700f, 40f, 64f), // Uses the checked placement
                        L(5992f, 112f, 176f), // Uses the checked placement
                        L(7552f, 112f, 64f) // Uses the checked placement
                    }, // Ends the placements
                    movingPlatforms: new[] // Vertical balloon routes
                    { // Opens the placements
                        MovingPlatform.Vertical(5944f, 24f, 120f, 0f), // Uses the checked placement
                        MovingPlatform.Vertical(7500f, 24f, 120f, 1.0f) // Uses the checked placement
                    }, // Ends the placements
                    windZones: new[] // Timed gusts
                    { // Opens the placements
                        new WindZone(600f, 400f, 0f), // Uses the checked placement
                        new WindZone(2500f, 400f, 1.0f), // Uses the checked placement
                        new WindZone(5000f, 500f, 0.5f), // Uses the checked placement
                        new WindZone(7900f, 500f, 1.5f) // Uses the checked placement
                    }, // Ends the placements
                    logs: new[] // Left and right log routes
                    { // Opens the placements
                        new RollingLog(1700f, 1500f, -1, 0.5f), // Uses the checked placement
                        new RollingLog(3300f, 3530f, 1, 1.0f), // Uses the checked placement
                        new RollingLog(4500f, 4280f, -1, 2.0f), // Uses the checked placement
                        new RollingLog(6400f, 6620f, 1, 1.5f), // Uses the checked placement
                        new RollingLog(8900f, 8680f, -1, 0.5f) // Uses the checked placement
                    }, // Ends the placements
                    ambienceOverride: "amb_wind_turbines"), // Plays the local ambience
                "Pililla Wind Farm"), // Shows the banner at the start
            HillsHard(5, new Lesson("Clew Bay", new[] // Clew Bay
            { // Opens the draft lesson
                "Long ago, ice sheets covered Ireland.", // Uses the authored lesson line
                "Moving ice left small rounded hills.", // Uses the authored lesson line
                "These hills are called drumlins.", // Uses the authored lesson line
                "Clew Bay has about a hundred of them.", // Uses the authored lesson line
            }, "lesson_clew_bay", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Schist, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Quartzite, new Spot(1378f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Schist, new Spot(2750f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Quartzite, new Spot(3132f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Schist, new Spot(3524f, 36f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Quartzite, new Spot(5200f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Schist, new Spot(6032f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Quartzite, new Spot(7004f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Schist, new Spot(8832f, 48f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1350f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(2000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(3100f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(4550f, 0f), 56f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6000f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(8800f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3250f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(4400f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5900f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(6800f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Glaciers left rounded hills called drumlins.", // Uses the authored review line
                    "Clew Bay's islands are drumlins in the sea.", // Uses the authored review line
                    "Quartzite is a very hard rock made from sand.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.DrumlinBay, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(800f, BoulderSize.Small), // Uses the checked placement
                        B(1050f, BoulderSize.Big), // Uses the checked placement
                        B(2200f, BoulderSize.Small), // Uses the checked placement
                        B(3500f, BoulderSize.Big), // Uses the checked placement
                        B(4200f, BoulderSize.Small), // Uses the checked placement
                        B(5350f, BoulderSize.Big), // Uses the checked placement
                        B(6400f, BoulderSize.Small), // Uses the checked placement
                        B(8200f, BoulderSize.Big), // Uses the checked placement
                        B(8350f, BoulderSize.Small) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(6900f, 40f, 64f), // Uses the checked placement
                        L(6980f, 84f, 48f) // Uses the checked placement
                    }, // Ends the placements
                    currents: new[] // Shallow current strips
                    { // Opens the placements
                        new CurrentZone(1720f, 200f, -1), // Uses the checked placement
                        new CurrentZone(2480f, 520f, -1), // Uses the checked placement
                        new CurrentZone(4950f, 560f, -1), // Uses the checked placement
                        new CurrentZone(7280f, 560f, -1), // Uses the checked placement
                        new CurrentZone(8950f, 350f, -1) // Uses the checked placement
                    }, // Ends the placements
                    mudZones: new[] // Slippery stretches
                    { // Opens the placements
                        new MudZone(600f, 400f), // Uses the checked placement
                        new MudZone(3300f, 500f), // Uses the checked placement
                        new MudZone(6200f, 500f), // Uses the checked placement
                        new MudZone(8300f, 400f) // Uses the checked placement
                    }, // Ends the placements
                    logs: new[] // Left and right log routes
                    { // Opens the placements
                        new RollingLog(1700f, 1480f, -1, 0f), // Uses the checked placement
                        new RollingLog(3900f, 4120f, 1, 1.0f), // Uses the checked placement
                        new RollingLog(5800f, 5580f, -1, 2.0f), // Uses the checked placement
                        new RollingLog(7900f, 8130f, 1, 0.5f), // Uses the checked placement
                        new RollingLog(8700f, 8480f, -1, 1.5f) // Uses the checked placement
                    }), // Ends the placements
                "Clew Bay"), // Shows the banner at the start
            HillsHard(6, new Lesson("Seven Hills of Rome", new[] // Seven Hills of Rome
            { // Opens the draft lesson
                "Rome was built on seven hills.", // Uses the authored lesson line
                "Hills were easy to defend long ago.", // Uses the authored lesson line
                "Romans built with travertine and tuff.", // Uses the authored lesson line
                "Many old buildings still stand today.", // Uses the authored lesson line
            }, "lesson_rome_hills", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Tuff, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Travertine, new Spot(1300f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Leucite, new Spot(2056f, 32f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Tuff, new Spot(3232f, 48f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Travertine, new Spot(3900f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Leucite, new Spot(4204f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Tuff, new Spot(5136f, 32f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Travertine, new Spot(6300f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Leucite, new Spot(8100f, 40f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1200f, 0f), 200f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(2000f, 0f), 112f, isWater: true), // Water pit with a flag return
                    new Hazard(HazardType.Pit, new Spot(3200f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(3800f, 0f), 200f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(5000f, 0f), 192f, isWater: true), // Water pit with a flag return
                    new Hazard(HazardType.Pit, new Spot(6200f, 0f), 200f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(7400f, 0f), 112f, isWater: true), // Water pit with a flag return
                    new Hazard(HazardType.Pit, new Spot(8000f, 0f), 220f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1700f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(3500f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(7800f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(9000f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Rome grew on seven hills.", // Uses the authored review line
                    "Romans built with travertine from nearby springs.", // Uses the authored review line
                    "Leucite is a white mineral in some volcanic rock.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.CityHills, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(1500f, BoulderSize.Big), // Uses the checked placement
                        B(2250f, BoulderSize.Small), // Uses the checked placement
                        B(3000f, BoulderSize.Big), // Uses the checked placement
                        B(4600f, BoulderSize.Small), // Uses the checked placement
                        B(5300f, BoulderSize.Big), // Uses the checked placement
                        B(6000f, BoulderSize.Small), // Uses the checked placement
                        B(7000f, BoulderSize.Big), // Uses the checked placement
                        B(8300f, BoulderSize.Small), // Uses the checked placement
                        B(9150f, BoulderSize.Big) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(4100f, 40f, 64f), // Uses the checked placement
                        L(4180f, 84f, 48f) // Uses the checked placement
                    }, // Ends the placements
                    movingPlatforms: new[] // Platforms span the wide pits
                    { // Opens the placements
                        new MovingPlatform(1200f, 1352f, 0f), // Uses the checked placement
                        new MovingPlatform(3800f, 3952f, 0f), // Uses the checked placement
                        new MovingPlatform(6200f, 6352f, 0f), // Uses the checked placement
                        new MovingPlatform(8000f, 8172f, 0f) // Uses the checked placement
                    }, // Ends the placements
                    floats: new[] // Floats over the water pits
                    { // Opens the placements
                        new SinkingFloat(2032f, 0f, 2000f), // Uses the checked placement
                        new SinkingFloat(5032f, 0f, 5000f), // Uses the checked placement
                        new SinkingFloat(5112f, 0f, 5000f), // Uses the checked placement
                        new SinkingFloat(7432f, 0f, 7400f) // Uses the checked placement
                    }, // Ends the placements
                    logs: new[] // Left and right log routes
                    { // Opens the placements
                        new RollingLog(900f, 700f, -1, 0f), // Uses the checked placement
                        new RollingLog(2700f, 2920f, 1, 1.0f), // Uses the checked placement
                        new RollingLog(3560f, 3790f, 1, 0.7f), // Uses the checked placement
                        new RollingLog(4500f, 4280f, -1, 2.0f), // Uses the checked placement
                        new RollingLog(5700f, 5920f, 1, 0.5f), // Uses the checked placement
                        new RollingLog(6900f, 6680f, -1, 1.5f), // Uses the checked placement
                        new RollingLog(8500f, 8720f, 1, 2.5f) // Uses the checked placement
                    }, // Ends the placements
                    ambienceOverride: "amb_city_hills"), // Plays the local ambience
                "Seven Hills of Rome"), // Shows the banner at the start
            HillsHard(7, new Lesson("Guilin Karst", new[] // Guilin Karst
            { // Opens the draft lesson
                "Guilin in China has tower-shaped hills.", // Uses the authored lesson line
                "They are made of thick limestone.", // Uses the authored lesson line
                "Rain and rivers carved them over time.", // Uses the authored lesson line
                "They are part of a UNESCO karst site.", // Uses the authored lesson line
            }, "lesson_guilin", true), // Sets the artwork and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Limestone, new Spot(416f, 24f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(700f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(1532f, 40f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(2750f, 0f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Limestone, new Spot(3056f, 32f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(4040f, 112f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(6564f, 96f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Limestone, new Spot(7856f, 32f)), // Uses the checked pickup
                    new Pickup(ItemCatalog.Calcite, new Spot(8432f, 48f)), // Uses the checked pickup
                }, // Ends the pickups
                new[] // Pits and spikes in the layout
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 96f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(3000f, 0f), 112f, isWater: true), // Water pit with a flag return
                    new Hazard(HazardType.Pit, new Spot(3600f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(3956f, 0f), 160f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(6100f, 0f), 96f), // Uses the checked pit
                    new Hazard(HazardType.Pit, new Spot(7800f, 0f), 192f, isWater: true), // Water pit with a flag return
                    new Hazard(HazardType.Pit, new Spot(8400f, 0f), 64f), // Uses the checked pit
                    new Hazard(HazardType.Spikes, new Spot(1000f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Uses the checked spike strip
                    new Hazard(HazardType.Spikes, new Spot(9250f, 0f), 32f), // Uses the checked spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Guilin's tower hills are made of limestone.", // Uses the authored review line
                    "Rain and rivers carved them over a long time.", // Uses the authored review line
                    "Limestone hills like these are called karst.", // Uses the authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.KarstTowers, // Uses the Hard trail width and theme
                    new[] // Ground boulders
                    { // Opens the placements
                        B(400f, BoulderSize.Small), // Uses the checked placement
                        B(1150f, BoulderSize.Big), // Uses the checked placement
                        B(2200f, BoulderSize.Small), // Uses the checked placement
                        B(4600f, BoulderSize.Big), // Uses the checked placement
                        B(5200f, BoulderSize.Small), // Uses the checked placement
                        B(7550f, BoulderSize.Big), // Uses the checked placement
                        B(8700f, BoulderSize.Small) // Uses the checked placement
                    }, // Ends the placements
                    new[] // Stone ledges
                    { // Opens the placements
                        L(3948f, 112f, 176f), // Uses the checked placement
                        L(6540f, 96f, 48f) // Uses the checked placement
                    }, // Ends the placements
                    crumbleLedges: new[] // Crumbling steps and bridges
                    { // Opens the placements
                        new CrumbleLedge(1500f, 0f), // Uses the checked placement
                        new CrumbleLedge(1532f, 0f), // Uses the checked placement
                        new CrumbleLedge(1564f, 0f), // Uses the checked placement
                        new CrumbleLedge(6100f, 0f), // Uses the checked placement
                        new CrumbleLedge(6132f, 0f), // Uses the checked placement
                        new CrumbleLedge(6164f, 0f), // Uses the checked placement
                        new CrumbleLedge(6400f, 40f), // Uses the checked placement
                        new CrumbleLedge(6470f, 64f) // Uses the checked placement
                    }, // Ends the placements
                    movingPlatforms: new[] // Vertical balloon routes
                    { // Opens the placements
                        MovingPlatform.Vertical(3900f, 24f, 120f, 0f) // Uses the checked placement
                    }, // Ends the placements
                    windZones: new[] // Timed gusts
                    { // Opens the placements
                        new WindZone(1100f, 500f, 0f), // Uses the checked placement
                        new WindZone(3300f, 500f, 1.0f), // Uses the checked placement
                        new WindZone(8100f, 500f, 0.5f) // Uses the checked placement
                    }, // Ends the placements
                    floats: new[] // Floats over the water pits
                    { // Opens the placements
                        new SinkingFloat(3032f, 0f, 3000f), // Uses the checked placement
                        new SinkingFloat(7832f, 0f, 7800f), // Uses the checked placement
                        new SinkingFloat(7912f, 0f, 7800f) // Uses the checked placement
                    }, // Ends the placements
                    bats: new[] // Bat loops
                    { // Opens the placements
                        new Bat(1300f, 16f, 24f, 12f, 2.6f, 0f), // Uses the checked placement
                        new Bat(5800f, 16f, 24f, 12f, 2.4f, 0.8f) // Uses the checked placement
                    }, // Ends the placements
                    currents: new[] // Shallow current strips
                    { // Opens the placements
                        new CurrentZone(2480f, 400f, -1), // Uses the checked placement
                        new CurrentZone(7300f, 400f, -1) // Uses the checked placement
                    }, // Ends the placements
                    mudZones: new[] // Slippery stretches
                    { // Opens the placements
                        new MudZone(500f, 400f), // Uses the checked placement
                        new MudZone(4150f, 400f), // Uses the checked placement
                        new MudZone(5000f, 400f), // Uses the checked placement
                        new MudZone(8550f, 350f) // Uses the checked placement
                    }, // Ends the placements
                    logs: new[] // Left and right log routes
                    { // Opens the placements
                        new RollingLog(2000f, 1780f, -1, 0.5f), // Uses the checked placement
                        new RollingLog(4300f, 4520f, 1, 1.0f), // Uses the checked placement
                        new RollingLog(6900f, 6680f, -1, 2.0f), // Uses the checked placement
                        new RollingLog(8950f, 9170f, 1, 1.5f) // Uses the checked placement
                    }), // Ends the placements
                "Guilin Karst"), // Shows the banner at the start

            // Plains Easy trails (D-069). The layouts were checked at 84 pixels per second.
            new LevelData(Episode.Plains, Difficulty.Easy, 1, // Central Luzon Plain
                new Lesson("Plains", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "Plains are wide, flat and low lands.", // Draft lesson line
                    "Central Luzon has our biggest plain.", // Draft lesson line
                    "Nueva Ecija is the Rice Granary.", // Draft lesson line
                    "Flat land is easy to farm and build on.", // Draft lesson line
                }, "lesson_central_luzon", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Clay, new Spot(416f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Sandstone, new Spot(640f, 32f)), // On a low dike
                    new Pickup(ItemCatalog.Shale, new Spot(2032f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Siltstone, new Spot(3972f, 84f)), // Secret spot at the top of the three steps
                    new Pickup(ItemCatalog.Clay, new Spot(4600f, 16f)), // Near the granary
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2400f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3300f, 0f), 48f), // Gap in the ground
                }, // Ends the hazards
                new Spot(4740f, 0f), // Rice Granary at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "Plains are wide, flat and low lands.", // Draft review line
                    "Plain soil often has clay, silt and sand.", // Draft review line
                    "Sand hardens into sandstone; silt into siltstone.", // Draft review line
                }, // Ends the review
                new LevelLayout(4800f, LevelTheme.RicePlain, // Width and theme
                    new[] { B(400f, BoulderSize.Small), B(1000f, BoulderSize.Big), B(1900f, BoulderSize.Small), B(2700f, BoulderSize.Big), B(3600f, BoulderSize.Small), B(4200f, BoulderSize.Big) }, // Ground boulders
                    new[] { L(600f, 32f, 80f), L(1200f, 28f, 64f), L(2000f, 40f, 64f), L(3000f, 28f, 80f), L(3800f, 28f, 48f), L(3860f, 56f, 64f), L(3940f, 84f, 64f) }), // Stone ledges
                "Central Luzon Plain"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Easy, 2, // Pangasinan Salt Beds
                new Lesson("Coastal Plains", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "Some plains lie flat beside the sea.", // Draft lesson line
                    "Pangasinan means place of salt.", // Draft lesson line
                    "Sea water dries in ponds and leaves salt.", // Draft lesson line
                    "That salt is a mineral called halite.", // Draft lesson line
                }, "lesson_pangasinan", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Halite, new Spot(416f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Gypsum, new Spot(1132f, 28f)), // On a low ledge
                    new Pickup(ItemCatalog.Quartz, new Spot(2032f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Siltstone, new Spot(3112f, 84f)), // Secret spot on the upper ledge
                    new Pickup(ItemCatalog.Halite, new Spot(4416f, 24f)), // On a boulder near the salt hut
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2700f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(800f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(1700f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3500f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(4000f, 0f), 32f), // Spike trap
                }, // Ends the hazards
                new Spot(5240f, 0f), // Salt Hut at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "Coastal plains lie flat beside the sea.", // Draft review line
                    "Drying sea water leaves halite and gypsum.", // Draft review line
                    "Beach sand is mostly quartz, a tough mineral.", // Draft review line
                }, // Ends the review
                new LevelLayout(5400f, LevelTheme.SaltBeds, // Width and theme
                    new[] { B(400f, BoulderSize.Small), B(1400f, BoulderSize.Big), B(2300f, BoulderSize.Small), B(4400f, BoulderSize.Small), B(4800f, BoulderSize.Big) }, // Ground boulders
                    new[] { L(1100f, 28f, 80f), L(2000f, 40f, 64f), L(3000f, 40f, 64f), L(3080f, 84f, 72f), L(3250f, 28f, 64f), L(4600f, 28f, 64f) }), // Stone ledges
                "Pangasinan Salt Beds"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Easy, 3, // Isabela River Plain
                new Lesson("River Plains", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "Rivers can build plains, too.", // Draft lesson line
                    "The Cagayan is our longest river.", // Draft lesson line
                    "Floods spread silt, sand and clay.", // Draft lesson line
                    "Farmers grow crops on this rich soil.", // Draft lesson line
                }, "lesson_isabela", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Conglomerate, new Spot(366f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Mudstone, new Spot(1272f, 96f)), // Reached from the second pile
                    new Pickup(ItemCatalog.Halite, new Spot(3066f, 0f)), // Between two spike traps
                    new Pickup(ItemCatalog.Limestone, new Spot(3388f, 120f)), // Secret spot needs a bounce and double jump
                    new Pickup(ItemCatalog.Conglomerate, new Spot(4716f, 24f)), // On a boulder near the farm shed
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2500f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(900f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(2100f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(3000f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Spikes, new Spot(3100f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3800f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(4300f, 0f), 32f), // Spike trap
                }, // Ends the hazards
                new Spot(5240f, 0f), // Farm Shed at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "Rivers spread soil when they flood a plain.", // Draft review line
                    "Hardened mud and clay become mudstone.", // Draft review line
                    "River gravel can harden into conglomerate.", // Draft review line
                }, // Ends the review
                new LevelLayout(5400f, LevelTheme.RiverPlain, // Width and theme
                    new[] { B(350f, BoulderSize.Small), B(1700f, BoulderSize.Big), B(2800f, BoulderSize.Small), B(4050f, BoulderSize.Big), B(4700f, BoulderSize.Small) }, // Ground boulders
                    new[] { L(1240f, 96f, 64f), L(1900f, 40f, 64f), L(2300f, 28f, 80f), L(3360f, 120f, 56f) }, // Stone ledges
                    strawPiles: new[] { new StrawPile(600f), new StrawPile(1200f), new StrawPile(3300f), new StrawPile(4500f) }), // Four bounce piles
                "Isabela River Plain"), // Start banner

            // Plains Normal trails from the checked layouts (D-070).
            new LevelData(Episode.Plains, Difficulty.Normal, 1, // Candaba Swamp
                new Lesson("Wetland Plains", new[] // Four draft lesson lines from section 6
                { // Opens the lesson
                    "Some plains are wet and swampy.", // The authored lesson line
                    "Candaba Swamp floods in the wet season.", // The authored lesson line
                    "In the dry season, farmers plant there.", // The authored lesson line
                    "Wild ducks rest there on long trips.", // The authored lesson line
                }, "lesson_candaba", true), // Sets the lesson picture and draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Peat, new Spot(436f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(848f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(1132f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(2600f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mudstone, new Spot(3424f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(4312f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Clay, new Spot(6716f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two checkpoint flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1700f, 0f), 104f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(3400f, 0f), 48f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(6000f, 0f), 192f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5300f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Bird Hide at the goal
                new[] // Three draft review lines
                { // Opens the review
                    "Wetlands are plains that hold water.", // The authored review line
                    "Dead plants pile up in wetlands as peat.", // The authored review line
                    "Over a long time, peat can turn into coal.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.CandabaWetland, // Width and theme
                    new[] { B(420f, BoulderSize.Small), B(1400f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(3900f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5700f, BoulderSize.Big), B(6700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1100f, 40f, 64f), L(4200f, 40f, 64f), L(4280f, 84f, 64f) }, // Ledges from the checked layout
                    floats: new[] { new SinkingFloat(824f, 0f, 800f), new SinkingFloat(1728f, 0f, 1700f), new SinkingFloat(6032f, 0f, 6000f), new SinkingFloat(6112f, 0f, 6000f) }, // Rafts at their water line
                    pools: new[] { new ShallowPool(800f, 96f) }), // The practice crossing has a safe floor
                "Candaba Swamp"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Normal, 2, // Pinatubo Lahar Plain
                new Lesson("Volcanic Plains", new[] // Four draft lesson lines from section 6
                { // Opens the lesson
                    "Volcanoes can bury and build plains.", // The authored lesson line
                    "Mount Pinatubo erupted in 1991.", // The authored lesson line
                    "Lahars are rivers of volcanic mud.", // The authored lesson line
                    "Lahar mud covered towns and farms.", // The authored lesson line
                }, "lesson_pinatubo_lahar", true), // Sets the lesson picture and draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Pumice, new Spot(916f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Dacite, new Spot(1032f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Andesite, new Spot(2200f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Pumice, new Spot(3224f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Dacite, new Spot(4012f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Andesite, new Spot(5432f, 28f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Pumice, new Spot(6816f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two checkpoint flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1300f, 0f), 96f), // Dry lahar gap
                    new Hazard(HazardType.Pit, new Spot(3600f, 0f), 56f), // Dry lahar gap
                    new Hazard(HazardType.Pit, new Spot(5700f, 0f), 96f), // Dry lahar gap
                    new Hazard(HazardType.Spikes, new Spot(2900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6600f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Lahar Marker at the goal
                new[] // Three draft review lines
                { // Opens the review
                    "Lahars carry volcanic sand and rock.", // The authored review line
                    "Pumice is a light rock full of holes.", // The authored review line
                    "Pinatubo's rocks include andesite and dacite.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.LaharPlain, // Width and theme
                    new[] { B(900f, BoulderSize.Small), B(1700f, BoulderSize.Big), B(2600f, BoulderSize.Small), B(3200f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5300f, BoulderSize.Big), B(6800f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1000f, 40f, 64f), L(3900f, 40f, 64f), L(3980f, 84f, 64f), L(5400f, 28f, 64f) }, // Ledges from the checked layout
                    crumbleLedges: new[] { new CrumbleLedge(1300f, 0f), new CrumbleLedge(1332f, 0f), new CrumbleLedge(1364f, 0f), new CrumbleLedge(5700f, 0f), new CrumbleLedge(5732f, 0f), new CrumbleLedge(5764f, 0f) }, // Both bridges start at ground level
                    sandZones: new[] { new SandZone(500f, 200f), new SandZone(2000f, 300f), new SandZone(4100f, 350f), new SandZone(6150f, 300f) }), // Soft sand from the checked layout
                "Pinatubo Lahar Plain"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Normal, 3, // Agno River Delta
                new Lesson("Deltas", new[] // Four draft lesson lines from section 6
                { // Opens the lesson
                    "A delta forms where a river meets the sea.", // The authored lesson line
                    "The river drops its sand and mud there.", // The authored lesson line
                    "The Agno built the Pangasinan Plain.", // The authored lesson line
                    "People pan for gold along the Agno.", // The authored lesson line
                }, "lesson_agno", true), // Sets the lesson picture and draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Gold, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Quartz, new Spot(700f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(1632f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(2650f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Gold, new Spot(3712f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Quartz, new Spot(5000f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mudstone, new Spot(6716f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two checkpoint flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1300f, 0f), 48f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(3200f, 0f), 64f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(5200f, 0f), 56f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Spikes, new Spot(2800f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6500f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Fish Pen Hut at the goal
                new[] // Three draft review lines
                { // Opens the review
                    "Deltas are new land built by rivers.", // The authored review line
                    "Gold can be panned from river sand.", // The authored review line
                    "Quartz sand and silt fill the delta.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.RiverDelta, // Width and theme
                    new[] { B(400f, BoulderSize.Small), B(1000f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(5500f, BoulderSize.Big), B(6700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1600f, 40f, 64f), L(3600f, 40f, 64f), L(3680f, 84f, 64f), L(6300f, 28f, 64f) }, // Ledges from the checked layout
                    currents: new[] { new CurrentZone(600f, 200f, 1), new CurrentZone(1900f, 300f, -1), new CurrentZone(3900f, 400f, 1), new CurrentZone(5800f, 300f, -1) }), // Currents keep their listed directions
                "Agno River Delta"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Normal, 4, // Agusan Marsh
                new Lesson("Living on Wetlands", new[] // Four draft lesson lines from section 6
                { // Opens the lesson
                    "Agusan Marsh is a huge wetland in Mindanao.", // The authored lesson line
                    "Some Manobo families live in floating homes.", // The authored lesson line
                    "The marsh is a protected wetland.", // The authored lesson line
                    "It was home to Lolong, a big crocodile.", // The authored lesson line
                }, "lesson_agusan", true), // Sets the lesson picture and draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Peat, new Spot(516f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(932f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mudstone, new Spot(1852f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(3150f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(4112f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Mudstone, new Spot(5150f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(6000f, 0f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two checkpoint flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1800f, 0f), 104f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(3800f, 0f), 56f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(6200f, 0f), 64f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4300f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6600f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Floating House at the goal
                new[] // Three draft review lines
                { // Opens the review
                    "Wetland soil is soft and full of old plants.", // The authored review line
                    "Peat, clay and mud fill the marsh.", // The authored review line
                    "Protecting wetlands keeps animals safe.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.FloatingVillage, // Width and theme
                    new[] { B(500f, BoulderSize.Small), B(2600f, BoulderSize.Big), B(3500f, BoulderSize.Small), B(4500f, BoulderSize.Big), B(5600f, BoulderSize.Small), B(6800f, BoulderSize.Big) }, // Boulders in trail order
                    new[] { L(900f, 40f, 64f), L(4000f, 40f, 64f), L(4080f, 84f, 64f), L(5800f, 28f, 64f) }, // Ledges from the checked layout
                    sandZones: new[] { new SandZone(3000f, 300f) }, // Soft sand from the checked layout
                    floats: new[] { new SinkingFloat(1828f, 0f, 1800f) }, // Rafts at their water line
                    currents: new[] { new CurrentZone(5000f, 300f, 1) }), // Currents keep their listed directions
                "Agusan Marsh"), // Start banner

            new LevelData(Episode.Plains, Difficulty.Normal, 5, // Mekong Delta
                new Lesson("River Deltas of the World", new[] // Four draft lesson lines from section 6
                { // Opens the lesson
                    "The Mekong Delta is in southern Vietnam.", // The authored lesson line
                    "It is flat land built from river mud.", // The authored lesson line
                    "People sell fruit from boats there.", // The authored lesson line
                    "Laterite is red rock of hot, wet lands.", // The authored lesson line
                }, "lesson_mekong", true), // Sets the lesson picture and draft flag
                new[] // Seven pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Laterite, new Spot(466f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(1132f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Laterite, new Spot(2076f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(2900f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Laterite, new Spot(4112f, 84f)), // Secret pickup on the high route
                    new Pickup(ItemCatalog.Clay, new Spot(5100f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mudstone, new Spot(6716f, 24f)), // Pickup from the checked layout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts at the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f) }, // The two checkpoint flags
                new[] // The listed pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(2000f, 0f), 200f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(3600f, 0f), 200f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Pit, new Spot(5900f, 0f), 200f, isWater: true), // Water pit with the Normal checkpoint return
                    new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3100f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5300f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new Spot(7040f, 0f), // Market Pier at the goal
                new[] // Three draft review lines
                { // Opens the review
                    "A delta is flat, low land at a river's mouth.", // The authored review line
                    "River mud makes deltas good for farming.", // The authored review line
                    "Laterite is red because of its iron.", // The authored review line
                }, // Ends the review
                new LevelLayout(7200f, LevelTheme.FloatingMarket, // Width and theme
                    new[] { B(450f, BoulderSize.Small), B(1250f, BoulderSize.Big), B(2700f, BoulderSize.Small), B(4300f, BoulderSize.Big), B(5000f, BoulderSize.Small), B(6700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(1100f, 40f, 64f), L(4000f, 40f, 64f), L(4080f, 84f, 64f), L(6400f, 28f, 64f) }, // Ledges from the checked layout
                    movingPlatforms: new[] { new MovingPlatform(800f, 912f, 0f), new MovingPlatform(2000f, 2152f, 0f), new MovingPlatform(3600f, 3752f, 0f), new MovingPlatform(5900f, 6052f, 0f) }, // The four horizontal market boats
                    pools: new[] { new ShallowPool(800f, 160f) }), // The practice crossing has a safe floor
                "Mekong Delta"), // Start banner

            PlainsHard(1, new Lesson("Grassland Plains", new[] // Great Plains (D-071)
            { // Opens the draft lesson
                "The Great Plains are wide, flat grassland.", // The authored lesson line
                "They lie in the middle of North America.", // The authored lesson line
                "Long ago, a sea covered these plains.", // The authored lesson line
                "Its tiny shells became white chalk.", // The authored lesson line
            }, "lesson_great_plains", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Chalk, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(1528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chalk, new Spot(2532f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(2932f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chalk, new Spot(3804f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Shale, new Spot(4916f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(6732f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(7324f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sandstone, new Spot(8532f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Dry pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(2900f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(5600f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6700f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(8500f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1250f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(2700f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3260f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4650f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(7000f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(8650f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Grassland plains are flat and wide.", // The authored review line
                    "Chalk is made of tiny sea shells.", // The authored review line
                    "Hay is dried grass for farm animals.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.Prairie, // Hard width and theme
                    new[] { B(400f, BoulderSize.Small), B(1650f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3100f, BoulderSize.Big), B(4900f, BoulderSize.Small), B(5450f, BoulderSize.Big), B(6500f, BoulderSize.Small), B(7300f, BoulderSize.Big), B(8000f, BoulderSize.Small), B(9100f, BoulderSize.Big) }, // Boulders in trail order
                    new[] { L(2500f, 40f, 64f), L(3700f, 40f, 64f), L(3780f, 84f, 48f) }, // Ledges from the checked layout
                    logs: new[] // Hay bales on the rolling-log rule
                    { // Opens the hay routes
                        new RollingLog(1000f, 800f, -1, 0f), // Practice bale rolls toward the explorer
                        new RollingLog(2000f, 1800f, -1, 1.0f), // Hay route from the checked layout
                        new RollingLog(3600f, 3380f, -1, 0f), // Hay route from the checked layout
                        new RollingLog(4300f, 4520f, 1, 0.5f), // Hay route from the checked layout
                        new RollingLog(5300f, 5080f, -1, 2.0f), // Hay route from the checked layout
                        new RollingLog(6100f, 6330f, 1, 1.0f), // Hay route from the checked layout
                        new RollingLog(7800f, 7580f, -1, 1.5f), // Hay route from the checked layout
                        new RollingLog(8100f, 8320f, 1, 2.5f), // Hay route from the checked layout
                        new RollingLog(9000f, 8780f, -1, 0.5f) // Hay route from the checked layout
                    }, // Ends the hay routes
                    strawPiles: new[] { new StrawPile(2050f), new StrawPile(4550f), new StrawPile(6900f) }), // Three bounce piles
                "Great Plains"), // Start banner

            PlainsHard(2, new Lesson("Plains of Animals", new[] // Serengeti (D-071)
            { // Opens the draft lesson
                "The Serengeti is a plain in Tanzania.", // The authored lesson line
                "Its grass grows on old volcanic ash.", // The authored lesson line
                "Millions of wildebeest walk across it.", // The authored lesson line
                "Watch wild animals from far away.", // The authored lesson line
            }, "lesson_serengeti", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Granite, new Spot(396f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(1100f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(1528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(2632f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sandstone, new Spot(3032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(4032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(6204f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Siltstone, new Spot(7824f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Shale, new Spot(8932f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Dry pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(4300f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6500f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(8300f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(2800f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(8600f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Grass plains feed huge herds.", // The authored review line
                    "Granite rocks stand on the Serengeti.", // The authored review line
                    "Tuff is rock made of volcanic ash.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.Savanna, // Hard width and theme
                    new[] { B(380f, BoulderSize.Small), B(1000f, BoulderSize.Big), B(1900f, BoulderSize.Small), B(2100f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5200f, BoulderSize.Small), B(5800f, BoulderSize.Big), B(6800f, BoulderSize.Small), B(7800f, BoulderSize.Big), B(9000f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(2600f, 40f, 64f), L(4000f, 48f, 64f), L(6100f, 40f, 64f), L(6180f, 84f, 48f), L(8900f, 48f, 64f) }, // Ledges from the checked layout
                    bats: new[] // Wildebeest herds on the bat rule at ground level
                    { // Opens the herds
                        new Bat(650f, 6f, 48f, 0f, 5.0f, 0f), // Practice herd
                        new Bat(2000f, 6f, 48f, 0f, 4.6f, 1.0f), // Herd from the checked layout
                        new Bat(3650f, 6f, 48f, 0f, 4.8f, 0.5f), // Herd from the checked layout
                        new Bat(5000f, 6f, 48f, 0f, 4.4f, 2.0f), // Herd from the checked layout
                        new Bat(6100f, 6f, 48f, 0f, 5.0f, 1.5f), // Herd from the checked layout
                        new Bat(7600f, 6f, 48f, 0f, 4.6f, 0.8f) // Herd from the checked layout
                    }), // Ends the herds
                "Serengeti"), // Start banner

            PlainsHard(3, new Lesson("Farm and Ranch Plains", new[] // Pampas (D-071)
            { // Opens the draft lesson
                "The Pampas are grasslands of Argentina.", // The authored lesson line
                "The soil there is rich for farming.", // The authored lesson line
                "Gauchos ride horses and herd cattle.", // The authored lesson line
                "Wind once spread fine dust called loess.", // The authored lesson line
            }, "lesson_pampas", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Loess, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mica, new Spot(1378f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Feldspar, new Spot(2400f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Loess, new Spot(3132f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Quartz, new Spot(3524f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mica, new Spot(4900f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Feldspar, new Spot(6032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Loess, new Spot(7004f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Clay, new Spot(8832f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Dry pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1350f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(2000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3100f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(4550f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(8800f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3250f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4400f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6800f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Plains are good for farms and ranches.", // The authored review line
                    "Loess is fine dust laid down by wind.", // The authored review line
                    "Mica and feldspar are grains in loess.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.Pampas, // Hard width and theme
                    new[] { B(400f, BoulderSize.Small), B(800f, BoulderSize.Small), B(1050f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4200f, BoulderSize.Small), B(5350f, BoulderSize.Big), B(6400f, BoulderSize.Small), B(8200f, BoulderSize.Big), B(8350f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(6900f, 40f, 64f), L(6980f, 84f, 48f) }, // Ledges from the checked layout
                    bats: new[] // Cattle herds on the bat rule at ground level
                    { // Opens the herds
                        new Bat(2740f, 6f, 48f, 0f, 4.8f, 0f), // Herd from the checked layout
                        new Bat(5230f, 6f, 48f, 0f, 4.4f, 1.0f), // Herd from the checked layout
                        new Bat(7560f, 6f, 48f, 0f, 5.0f, 2.0f) // Herd from the checked layout
                    }, // Ends the herds
                    logs: new[] // Hay bales on the rolling-log rule
                    { // Opens the hay routes
                        new RollingLog(1700f, 1480f, -1, 0f), // Hay route from the checked layout
                        new RollingLog(3900f, 4120f, 1, 1.0f), // Hay route from the checked layout
                        new RollingLog(5800f, 5580f, -1, 2.0f), // Hay route from the checked layout
                        new RollingLog(7900f, 8130f, 1, 0.5f), // Hay route from the checked layout
                        new RollingLog(8700f, 8480f, -1, 1.5f) // Hay route from the checked layout
                    }), // Ends the hay routes
                "Pampas"), // Start banner

            PlainsHard(4, new Lesson("Cold Plains", new[] // West Siberian Plain (D-071)
            { // Opens the draft lesson
                "The West Siberian Plain is in Russia.", // The authored lesson line
                "It is the largest flat lowland on Earth.", // The authored lesson line
                "Its winters are long and very cold.", // The authored lesson line
                "Huge bogs there are full of peat.", // The authored lesson line
            }, "lesson_siberia", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Peat, new Spot(396f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(650f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(1528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(2632f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mudstone, new Spot(3052f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sandstone, new Spot(4032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Peat, new Spot(6204f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Clay, new Spot(7824f, 36f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(8932f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Pits, two bog ponds and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3000f, 0f), 104f, isWater: true), // Bog pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(4300f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6500f, 0f), 104f, isWater: true), // Bog pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(8300f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(2800f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6900f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(8600f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Cold plains freeze in winter.", // The authored review line
                    "Bogs hold peat made of old plants.", // The authored review line
                    "Peat can slowly turn into coal.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.FrozenPlain, // Hard width and theme
                    new[] { B(380f, BoulderSize.Small), B(1000f, BoulderSize.Big), B(1900f, BoulderSize.Small), B(2100f, BoulderSize.Small), B(3500f, BoulderSize.Big), B(4500f, BoulderSize.Small), B(5200f, BoulderSize.Small), B(5800f, BoulderSize.Big), B(6800f, BoulderSize.Small), B(7800f, BoulderSize.Big), B(9000f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(2600f, 40f, 64f), L(4000f, 48f, 64f), L(6100f, 40f, 64f), L(6180f, 84f, 48f), L(8900f, 48f, 64f) }, // Ledges from the checked layout
                    iceZones: new[] { new IceZone(500f, 300f), new IceZone(1700f, 500f), new IceZone(3200f, 500f), new IceZone(5000f, 400f), new IceZone(6000f, 400f), new IceZone(7500f, 600f), new IceZone(8800f, 400f) }, // Frozen ground on the ice rule
                    floats: new[] { new SinkingFloat(3028f, 0f, 3000f), new SinkingFloat(6528f, 0f, 6500f) }, // Peat-bog floats at the water line
                    ambienceOverride: "amb_cold_plain"), // The cold wind loop
                "West Siberian Plain"), // Start banner

            PlainsHard(5, new Lesson("Limestone Plains", new[] // Nullarbor Plain (D-071)
            { // Opens the draft lesson
                "The Nullarbor Plain is in Australia.", // The authored lesson line
                "Its name means \"no tree\".", // The authored lesson line
                "It is one huge sheet of limestone.", // The authored lesson line
                "Long ago, people mined flint in a cave.", // The authored lesson line
            }, "lesson_nullarbor", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Flint, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(1528f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Calcite, new Spot(2832f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Flint, new Spot(3052f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chalk, new Spot(4032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(5632f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Flint, new Spot(7004f, 96f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Calcite, new Spot(8032f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Limestone, new Spot(9252f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Dry pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(2800f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(5600f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(6600f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(8000f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(8900f, 0f), 56f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1200f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3300f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(4450f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(6250f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(7600f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(8650f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Limestone forms from shells in old seas.", // The authored review line
                    "Flint is very hard and makes sparks.", // The authored review line
                    "Calcite is the mineral in limestone.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.LimestonePlain, // Hard width and theme
                    new[] { B(400f, BoulderSize.Small), B(1000f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(3600f, BoulderSize.Big), B(4300f, BoulderSize.Small), B(5300f, BoulderSize.Big), B(6900f, BoulderSize.Small), B(7400f, BoulderSize.Big), B(8400f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(3020f, 40f, 64f), L(6980f, 96f, 48f), L(9220f, 48f, 64f) }, // Ledges from the checked layout
                    crumbleLedges: new[] { new CrumbleLedge(2800f, 0f), new CrumbleLedge(2832f, 0f), new CrumbleLedge(2864f, 0f), new CrumbleLedge(5600f, 0f), new CrumbleLedge(5632f, 0f), new CrumbleLedge(5664f, 0f), new CrumbleLedge(8000f, 0f), new CrumbleLedge(8032f, 0f), new CrumbleLedge(8064f, 0f), new CrumbleLedge(6830f, 40f), new CrumbleLedge(6900f, 64f) }, // Three bridges and two steps
                    bats: new[] // Flying bat loops
                    { // Opens the bats
                        new Bat(1800f, 16f, 24f, 12f, 3.0f, 0f), // Bat from the checked layout
                        new Bat(2150f, 16f, 24f, 12f, 2.8f, 1.0f), // Bat from the checked layout
                        new Bat(3800f, 16f, 24f, 12f, 2.6f, 0.4f), // Bat from the checked layout
                        new Bat(4600f, 16f, 24f, 12f, 2.8f, 0.6f), // Bat from the checked layout
                        new Bat(5950f, 16f, 24f, 12f, 2.6f, 1.2f), // Bat from the checked layout
                        new Bat(9150f, 16f, 24f, 12f, 2.6f, 0.3f) // Bat from the checked layout
                    }), // Ends the bats
                "Nullarbor Plain"), // Start banner

            PlainsHard(6, new Lesson("River Flood Plains", new[] // Indo-Gangetic Plain (D-071)
            { // Opens the draft lesson
                "Three big rivers built this huge plain.", // The authored lesson line
                "Their floods leave rich soil behind.", // The authored lesson line
                "Many people farm rice and wheat here.", // The authored lesson line
                "It is in India and nearby countries.", // The authored lesson line
            }, "lesson_ganges", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Laterite, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(1300f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(2056f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Sandstone, new Spot(3232f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Clay, new Spot(3900f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Laterite, new Spot(4204f, 84f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Mudstone, new Spot(5136f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Siltstone, new Spot(6300f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Laterite, new Spot(8100f, 40f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Flood ponds and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1200f, 0f), 200f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(2000f, 0f), 112f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(3200f, 0f), 64f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(3800f, 0f), 200f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(5000f, 0f), 192f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(6200f, 0f), 200f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(7400f, 0f), 112f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Pit, new Spot(8000f, 0f), 200f, isWater: true), // Flood pond with a flag return
                    new Hazard(HazardType.Spikes, new Spot(1700f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(3500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(7800f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(9000f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Rivers build plains with their mud.", // The authored review line
                    "Floods bring new soil to the farms.", // The authored review line
                    "Laterite is red, iron-rich rock.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.GangesPlain, // Hard width and theme
                    new[] { B(400f, BoulderSize.Small), B(1500f, BoulderSize.Big), B(2250f, BoulderSize.Small), B(3000f, BoulderSize.Big), B(4600f, BoulderSize.Small), B(5300f, BoulderSize.Big), B(6000f, BoulderSize.Small), B(7000f, BoulderSize.Big), B(8300f, BoulderSize.Small), B(9150f, BoulderSize.Big) }, // Boulders in trail order
                    new[] { L(4100f, 40f, 64f), L(4180f, 84f, 48f) }, // Ledges from the checked layout
                    floats: new[] // Floats over the flood ponds
                    { // Opens the floats
                        new SinkingFloat(1232f, 0f, 1200f), new SinkingFloat(1312f, 0f, 1200f), // Two floats over the first pond
                        new SinkingFloat(2032f, 0f, 2000f), // One float over the second pond
                        new SinkingFloat(3832f, 0f, 3800f), new SinkingFloat(3912f, 0f, 3800f), // Two floats over the fourth pond
                        new SinkingFloat(5032f, 0f, 5000f), new SinkingFloat(5112f, 0f, 5000f), // Two floats over the fifth pond
                        new SinkingFloat(6232f, 0f, 6200f), new SinkingFloat(6312f, 0f, 6200f), // Two floats over the sixth pond
                        new SinkingFloat(7432f, 0f, 7400f), // One float over the seventh pond
                        new SinkingFloat(8032f, 0f, 8000f), new SinkingFloat(8112f, 0f, 8000f) // Two floats over the last pond
                    }, // Ends the floats
                    currents: new[] { new CurrentZone(550f, 450f, 1), new CurrentZone(2500f, 550f, -1), new CurrentZone(4100f, 600f, -1), new CurrentZone(5650f, 450f, 1), new CurrentZone(6500f, 600f, -1), new CurrentZone(8300f, 600f, -1) }), // River currents keep their listed directions
                "Indo-Gangetic Plain"), // Start banner

            PlainsHard(7, new Lesson("Plains of the World", new[] // Mongolian Steppe, the finale (D-071)
            { // Opens the draft lesson
                "The steppe is a grassland across Mongolia.", // The authored lesson line
                "Herders live in round tents called gers.", // The authored lesson line
                "Winters on the steppe are very cold.", // The authored lesson line
                "You explored plains around the world!", // The authored lesson line
            }, "lesson_steppe", true), // Sets the lesson picture and draft flag
                new[] // Nine pickups in trail order
                { // Opens the pickups
                    new Pickup(ItemCatalog.Granite, new Spot(416f, 24f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Chalk, new Spot(700f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Loess, new Spot(1532f, 40f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Flint, new Spot(2750f, 0f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Tuff, new Spot(3032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Mica, new Spot(4032f, 48f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Loess, new Spot(6564f, 96f)), // Secret pickup on the high ledge
                    new Pickup(ItemCatalog.Feldspar, new Spot(7856f, 32f)), // Pickup from the checked layout
                    new Pickup(ItemCatalog.Granite, new Spot(8432f, 48f)), // Pickup from the checked layout
                }, // Ends the pickups
                new[] // Dry pits and spikes
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1500f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(3000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(3600f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(4000f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Pit, new Spot(6100f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(7800f, 0f), 96f), // Dry pit under a crumbling bridge
                    new Hazard(HazardType.Pit, new Spot(8400f, 0f), 64f), // Dry pit from the checked layout
                    new Hazard(HazardType.Spikes, new Spot(1000f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(5500f, 0f), 32f), // Spike strip
                    new Hazard(HazardType.Spikes, new Spot(9250f, 0f), 32f), // Spike strip
                }, // Ends the hazards
                new[] // Three draft review lines
                { // Opens the review
                    "Steppes are wide, grassy plains.", // The authored review line
                    "Herders move their animals to new grass.", // The authored review line
                    "Plains are lands that are mostly flat.", // The authored review line
                }, // Ends the review
                new LevelLayout(9600f, LevelTheme.Steppe, // Hard width and theme
                    new[] { B(400f, BoulderSize.Small), B(1150f, BoulderSize.Big), B(2200f, BoulderSize.Small), B(4600f, BoulderSize.Big), B(5200f, BoulderSize.Small), B(7550f, BoulderSize.Big), B(8700f, BoulderSize.Small) }, // Boulders in trail order
                    new[] { L(6540f, 96f, 48f) }, // The secret ledge
                    crumbleLedges: new[] { new CrumbleLedge(1500f, 0f), new CrumbleLedge(1532f, 0f), new CrumbleLedge(1564f, 0f), new CrumbleLedge(6100f, 0f), new CrumbleLedge(6132f, 0f), new CrumbleLedge(6164f, 0f), new CrumbleLedge(7800f, 0f), new CrumbleLedge(7832f, 0f), new CrumbleLedge(7864f, 0f), new CrumbleLedge(6400f, 40f), new CrumbleLedge(6470f, 64f) }, // Three bridges and two steps
                    iceZones: new[] { new IceZone(500f, 400f), new IceZone(4150f, 400f), new IceZone(5000f, 400f), new IceZone(8550f, 350f) }, // Frozen ground on the ice rule
                    bats: new[] { new Bat(1300f, 6f, 48f, 0f, 4.6f, 0f), new Bat(5800f, 6f, 48f, 0f, 4.4f, 0.8f) }, // Horse herds on the bat rule at ground level
                    currents: new[] { new CurrentZone(2480f, 400f, -1), new CurrentZone(7300f, 400f, -1) }, // Both currents flow left
                    logs: new[] { new RollingLog(2000f, 1780f, -1, 0.5f), new RollingLog(4300f, 4520f, 1, 1.0f), new RollingLog(6900f, 6680f, -1, 2.0f), new RollingLog(8950f, 9170f, 1, 1.5f) }), // Hay bales on the rolling-log rule
                "Mongolian Steppe"), // Start banner

            // Plateaus Easy trails (D-072). The layouts were checked by the bot at 84 pixels per second.
            new LevelData(Episode.Plateaus, Difficulty.Easy, 1, // Bukidnon Plateau
                new Lesson("Plateaus", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "A plateau is high land with a flat top.", // Draft lesson line
                    "Its sides are steep, like a table.", // Draft lesson line
                    "Bukidnon is a plateau in Mindanao.", // Draft lesson line
                    "Farmers grow pineapples on its top.", // Draft lesson line
                }, "lesson_bukidnon", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Basalt, new Spot(466f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Andesite, new Spot(690f, 32f)), // On a low ledge
                    new Pickup(ItemCatalog.Hematite, new Spot(2082f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Halloysite, new Spot(4072f, 84f)), // Secret spot at the top of the three steps
                    new Pickup(ItemCatalog.Basalt, new Spot(4600f, 16f)), // Near the lookout
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2400f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(1300f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(2950f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3400f, 0f), 48f), // Gap in the ground
                }, // Ends the hazards
                new Spot(4740f, 0f), // Farm Lookout at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "A plateau is high, flat land with steep sides.", // Draft review line
                    "Basalt and andesite come from old volcanoes.", // Draft review line
                    "Halloysite and hematite are in its red soil.", // Draft review line
                }, // Ends the review
                new LevelLayout(4800f, LevelTheme.PineapplePlateau, // Width and theme
                    new[] { B(450f, BoulderSize.Small), B(1050f, BoulderSize.Big), B(1800f, BoulderSize.Small), B(2800f, BoulderSize.Big), B(3700f, BoulderSize.Small), B(4250f, BoulderSize.Big) }, // Ground boulders
                    new[] { L(650f, 32f, 80f), L(1500f, 28f, 64f), L(2050f, 40f, 64f), L(3100f, 28f, 80f), L(3900f, 28f, 48f), L(3960f, 56f, 64f), L(4040f, 84f, 64f) }), // Stone ledges
                "Bukidnon Plateau"), // Start banner

            new LevelData(Episode.Plateaus, Difficulty.Easy, 2, // Lake Lanao
                new Lesson("Plateau Lakes", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "Some plateaus hold big lakes.", // Draft lesson line
                    "Lake Lanao is about 700 m high.", // Draft lesson line
                    "Volcanoes helped make its basin.", // Draft lesson line
                    "It is our second-largest lake.", // Draft lesson line
                }, "lesson_lanao", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Tuff, new Spot(466f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.VolcanicBreccia, new Spot(1182f, 28f)), // On a low ledge
                    new Pickup(ItemCatalog.Goethite, new Spot(1982f, 40f)), // On a ledge
                    new Pickup(ItemCatalog.Halloysite, new Spot(3162f, 84f)), // Secret spot on the upper ledge (review kind)
                    new Pickup(ItemCatalog.Tuff, new Spot(4466f, 24f)), // On a boulder near the torogan
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2700f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(900f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(1800f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3600f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(4100f, 0f), 32f), // Spike trap
                }, // Ends the hazards
                new Spot(5240f, 0f), // Torogan at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "Lake Lanao lies high on a plateau.", // Draft review line
                    "Volcanic ash hardens into tuff.", // Draft review line
                    "Goethite is a rusty mineral in the soil.", // Draft review line
                }, // Ends the review
                new LevelLayout(5400f, LevelTheme.LakePlateau, // Width and theme
                    new[] { B(450f, BoulderSize.Small), B(1500f, BoulderSize.Big), B(2250f, BoulderSize.Small), B(4450f, BoulderSize.Small), B(4850f, BoulderSize.Big) }, // Ground boulders
                    new[] { L(1150f, 28f, 80f), L(1950f, 40f, 64f), L(3050f, 40f, 64f), L(3130f, 84f, 72f), L(3300f, 28f, 64f), L(4650f, 28f, 64f) }), // Stone ledges
                "Lake Lanao"), // Start banner

            new LevelData(Episode.Plateaus, Difficulty.Easy, 3, // Maria Cristina Falls
                new Lesson("Plateau Rivers", new[] // Draft lesson for Grade 5
                { // Opens the lesson
                    "Rivers flow off the edge of plateaus.", // Draft lesson line
                    "Lake Lanao drains into the Agus River.", // Draft lesson line
                    "It falls 98 m at Maria Cristina Falls.", // Draft lesson line
                    "The falls make power for Mindanao.", // Draft lesson line
                }, "lesson_maria_cristina", true), // Shows the draft note
                new[] // Five pickups
                { // Opens the pickups
                    new Pickup(ItemCatalog.Laterite, new Spot(366f, 24f)), // On the first boulder
                    new Pickup(ItemCatalog.Gibbsite, new Spot(1272f, 112f)), // On the ledge reached by the second updraft
                    new Pickup(ItemCatalog.Goethite, new Spot(3066f, 0f)), // Between two spike traps (review kind)
                    new Pickup(ItemCatalog.Cristobalite, new Spot(3368f, 160f)), // Secret spot only the third updraft reaches
                    new Pickup(ItemCatalog.Laterite, new Spot(4716f, 24f)), // On a boulder near the viewing deck
                }, // Ends the pickups
                new Spot(40f, 0f), // Starts on the ground
                new[] { new Spot(2500f, 0f) }, // The checkpoint flag
                new[] // Gaps and spike traps
                { // Opens the hazards
                    new Hazard(HazardType.Pit, new Spot(900f, 0f), 48f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(1500f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(2100f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(3000f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Spikes, new Spot(3100f, 0f), 32f), // Spike trap
                    new Hazard(HazardType.Pit, new Spot(3800f, 0f), 56f), // Gap in the ground
                    new Hazard(HazardType.Spikes, new Spot(4300f, 0f), 32f), // Spike trap
                }, // Ends the hazards
                new Spot(5240f, 0f), // Falls Viewing Deck at the goal
                new[] // Draft lines for the Result Screen
                { // Opens the review
                    "Rivers fall off plateau edges as waterfalls.", // Draft review line
                    "Maria Cristina Falls runs over basalt.", // Draft review line
                    "Gibbsite and laterite form in warm, wet soil.", // Draft review line
                }, // Ends the review
                new LevelLayout(5400f, LevelTheme.FallsEdge, // Width and theme
                    new[] { B(350f, BoulderSize.Small), B(1700f, BoulderSize.Big), B(2800f, BoulderSize.Small), B(4050f, BoulderSize.Big), B(4700f, BoulderSize.Small) }, // Ground boulders
                    new[] { L(640f, 64f, 64f), L(1240f, 112f, 64f), L(1900f, 40f, 64f), L(2300f, 28f, 80f), L(3340f, 160f, 56f), L(4540f, 80f, 64f) }, // Stone ledges, four of them beside the updrafts
                    updrafts: new[] { new Updraft(600f, 68f), new Updraft(1200f, 116f), new Updraft(3300f, 164f), new Updraft(4500f, 84f) }), // Four cliff updrafts
                "Maria Cristina Falls"), // Start banner
        }; // Ends the catalog

        static LevelData HillsHard(int number, Lesson lesson, Pickup[] pickups, Hazard[] hazards, string[] review, LevelLayout layout, string banner) // Uses the shared Hard start, flags and goal
        { // Opens the trail setup
            return new LevelData(Episode.Hills, Difficulty.Hard, number, lesson, pickups, new Spot(40f, 0f), // Starts near the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Uses the three Hard flags
                hazards, new Spot(9440f, 0f), review, layout, banner); // Ends at the shared goal
        } // Ends the trail setup

        static LevelData PlainsHard(int number, Lesson lesson, Pickup[] pickups, Hazard[] hazards, string[] review, LevelLayout layout, string banner) // Uses the shared Hard start, flags and goal for Plains (D-071)
        { // Opens the trail setup
            return new LevelData(Episode.Plains, Difficulty.Hard, number, lesson, pickups, new Spot(40f, 0f), // Starts near the left
                new[] { new Spot(2400f, 0f), new Spot(4800f, 0f), new Spot(7200f, 0f) }, // Uses the three Hard flags
                hazards, new Spot(9440f, 0f), review, layout, banner); // Ends at the shared goal
        } // Ends the trail setup

        static Boulder B(float x, BoulderSize size) => new Boulder(x, size); // Short way to write a boulder
        static Ledge L(float x, float top, float width) => new Ledge(x, top, width); // Short way to write a ledge

        /// <summary>Every level with data, in play order.</summary>
        public static IReadOnlyList<LevelData> All => Levels; // The builder makes one scene for each

        public static bool HasLevels(Episode e) => Levels.Any(l => l.Episode == e); // Checks whether this landform has playable data

        /// <summary>True when this difficulty of the landform has at least one level with data. An open difficulty without data says "Coming soon" (D-066).</summary>
        public static bool HasLevels(Episode e, Difficulty d) => Levels.Any(l => l.Episode == e && l.Difficulty == d); // Checks one difficulty for playable levels

        /// <summary>The level's data, or null when that level has no data yet.</summary>
        public static LevelData Find(Episode e, Difficulty d, int number) => Levels.FirstOrDefault(l => l.Episode == e && l.Difficulty == d && l.Number == number); // Looks the level up
    }
}
