using System.Collections.Generic; // Gives access to Dictionary and List
using System.Linq; // Gives access to list helpers such as Any and Where
using LandGrand.Play; // Gives access to the level play scripts such as SummitSnow and EagleFlyOff
using LandGrand.Rules; // Gives access to LevelTheme, LevelData and Hazard
using UnityEngine; // Gives access to GameObject, Transform and Mathf
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Everything that changes with the level theme, in one place (D-055): the ground, ledge and boulder pictures,
/// the goal at the end of the level, and the living props. A new theme is one new entry in Themes.
/// </summary>
public static partial class LevelBuilder // The theme half of the level builder
{
    /// <summary>The art of one level theme.</summary>
    sealed class ThemeArt // One theme's pictures and props
    {
        public string GroundTop; // Tile along the top edge of the ground
        public string Soil; // Tile for the rock or soil below the top edge
        public string Ledge; // Tile for the stone ledges
        public string BoulderEnding; // Added to "boulder_small" and "boulder_big", for example "_volcanic"
        public string GoalName; // Object name of the goal picture, for example "Cave"
        public string GoalSprite; // Sprite of the goal, for example "cave"
        public Vector2 GoalOffset; // Middle of the goal picture: pixels right of the goal line, and pixels up from the ground line
        public System.Action<LevelData, List<Hazard>, Transform, ExplorerMotor> Props; // Builds the living props; null when the theme has none
        public string Backdrop; // Folder of the six moving background layers, inside the level sprite folder; every theme names its own (D-049, D-063)
        public string BackdropScript; // Script that draws those layers
    }

    static readonly Dictionary<LevelTheme, ThemeArt> Themes = new Dictionary<LevelTheme, ThemeArt> // Every theme's art
    {
        [LevelTheme.Meadow] = new ThemeArt // Mountain Trail (Easy 1)
        {
            GroundTop = "grass_top", Soil = "soil", Ledge = "ledge", BoulderEnding = "", // Green grass over brown soil
            GoalName = "Cave", GoalSprite = "cave", GoalOffset = new Vector2(20f, 28f), // Cave entrance; its 56-pixel height sits on the ground and its opening is at the goal
            Props = null, // No living props
            Backdrop = "Backdrop/Mountains/", BackdropScript = "Tools/Art/make_mountains_backdrop.py", // The alpine Mountains background (D-049)
        },
        [LevelTheme.Volcanic] = new ThemeArt // Volcano Trail (Easy 2)
        {
            GroundTop = "volcanic_top", Soil = "volcanic_soil", Ledge = "ledge_volcanic", BoulderEnding = "_volcanic", // Dark lava rock
            GoalName = "Arch", GoalSprite = "arch", GoalOffset = new Vector2(12f, 28f), // Lava-rock arch; its 56-pixel height sits on the ground and its open middle starts at the goal
            Props = VolcanicProps, // Lava cracks and fumarole wisps
            Backdrop = "Backdrop/Volcanic/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        },
        [LevelTheme.FoldedRock] = new ThemeArt // Folded Peaks (Easy 3)
        {
            GroundTop = "fold_top", Soil = "fold_soil", Ledge = "ledge_fold", BoulderEnding = "_fold", // Grey folded rock
            GoalName = "Cairn", GoalSprite = "cairn", GoalOffset = new Vector2(24f, 22f), // Summit stone pile; its 44-pixel height sits on the ground, just past the goal
            Props = FoldedProps, // Summit snow and the eagle
            Backdrop = "Backdrop/FoldedRock/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        },
        [LevelTheme.PineForest] = new ThemeArt // Normal trail art
        { // Opens the theme
            GroundTop = "pine_top", Soil = "pine_soil", Ledge = "ledge_pine", BoulderEnding = "_pine", // Theme ground and stone
            GoalName = "Ranger Hut", GoalSprite = "hut", GoalOffset = new Vector2(0f, 24f), // Grounded goal picture
            Props = PineProps, // Theme decoration
            Backdrop = "Backdrop/PineForest/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        }, // Ends the theme
        [LevelTheme.AshGorge] = new ThemeArt // Normal trail art
        { // Opens the theme
            GroundTop = "ash_top", Soil = "ash_soil", Ledge = "ledge_ash", BoulderEnding = "_ash", // Theme ground and stone
            GoalName = "Stone Bridge", GoalSprite = "bridge", GoalOffset = new Vector2(36f, 22f), // Grounded goal picture
            Props = null, // Theme decoration
            Backdrop = "Backdrop/AshGorge/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        }, // Ends the theme
        [LevelTheme.MossyForest] = new ThemeArt // Normal trail art
        { // Opens the theme
            GroundTop = "moss_top", Soil = "moss_soil", Ledge = "ledge_moss", BoulderEnding = "_moss", // Theme ground and stone
            GoalName = "Trail Sign", GoalSprite = "trail_sign", GoalOffset = new Vector2(16f, 20f), // Grounded goal picture
            Props = MossProps, // Theme decoration
            Backdrop = "Backdrop/MossyForest/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        }, // Ends the theme
        [LevelTheme.SnowPeak] = new ThemeArt // Normal trail art
        { // Opens the theme
            GroundTop = "snow_top", Soil = "snow_soil", Ledge = "ledge_snow", BoulderEnding = "_snow", // Theme ground and stone
            GoalName = "Summit Flags", GoalSprite = "summit_flags", GoalOffset = new Vector2(28f, 28f), // Grounded goal picture
            Props = SnowProps, // Theme decoration
            Backdrop = "Backdrop/SnowPeak/", BackdropScript = "Tools/Art/make_normal_backdrops.py", // Its own background, drawn for its mountain (D-063)
        }, // Ends the theme
        [LevelTheme.LavaSlope] = new ThemeArt // Watch Station trail art
        { // Opens the theme
            GroundTop = "lava_top", Soil = "lava_soil", Ledge = "ledge_lava", BoulderEnding = "_lava", // Ground and stone pictures
            GoalName = "Watch Station", GoalSprite = "watch_station", GoalOffset = new Vector2(24f, 24f), // Goal rests on the ground
            Props = LavaSlopeProps, // Decorations for this theme
            Backdrop = "Backdrop/LavaSlope/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.StormRidge] = new ThemeArt // Lookout Tower trail art
        { // Opens the theme
            GroundTop = "storm_top", Soil = "storm_soil", Ledge = "ledge_storm", BoulderEnding = "_storm", // Ground and stone pictures
            GoalName = "Lookout Tower", GoalSprite = "lookout_tower", GoalOffset = new Vector2(20f, 32f), // Goal rests on the ground
            Props = StormRidgeProps, // Decorations for this theme
            Backdrop = "Backdrop/StormRidge/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.EagleForest] = new ThemeArt // Eagle Nest trail art
        { // Opens the theme
            GroundTop = "eagle_top", Soil = "eagle_soil", Ledge = "ledge_eagle", BoulderEnding = "_eagle", // Ground and stone pictures
            GoalName = "Eagle Nest", GoalSprite = "eagle_nest", GoalOffset = new Vector2(28f, 28f), // Goal rests on the ground
            Props = EagleForestProps, // Decorations for this theme
            Backdrop = "Backdrop/EagleForest/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.BonsaiForest] = new ThemeArt // Sanctuary Sign trail art
        { // Opens the theme
            GroundTop = "bonsai_top", Soil = "bonsai_soil", Ledge = "ledge_bonsai", BoulderEnding = "_bonsai", // Ground and stone pictures
            GoalName = "Sanctuary Sign", GoalSprite = "sanctuary_sign", GoalOffset = new Vector2(20f, 20f), // Goal rests on the ground
            Props = BonsaiForestProps, // Decorations for this theme
            Backdrop = "Backdrop/BonsaiForest/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.GlacierHorn] = new ThemeArt // Alpine Hut trail art
        { // Opens the theme
            GroundTop = "glacier_top", Soil = "glacier_soil", Ledge = "ledge_glacier", BoulderEnding = "_glacier", // Ground and stone pictures
            GoalName = "Alpine Hut", GoalSprite = "alpine_hut", GoalOffset = new Vector2(28f, 24f), // Goal rests on the ground
            Props = GlacierHornProps, // Decorations for this theme
            Backdrop = "Backdrop/GlacierHorn/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.EquatorSummit] = new ThemeArt // Summit Board trail art
        { // Opens the theme
            GroundTop = "equator_top", Soil = "equator_soil", Ledge = "ledge_equator", BoulderEnding = "_equator", // Ground and stone pictures
            GoalName = "Summit Board", GoalSprite = "summit_board", GoalOffset = new Vector2(24f, 20f), // Goal rests on the ground
            Props = EquatorSummitProps, // Decorations for this theme
            Backdrop = "Backdrop/EquatorSummit/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.SeaPeak] = new ThemeArt // Telescope Dome trail art
        { // Opens the theme
            GroundTop = "sea_top", Soil = "sea_soil", Ledge = "ledge_sea", BoulderEnding = "_sea", // Ground and stone pictures
            GoalName = "Telescope Dome", GoalSprite = "telescope_dome", GoalOffset = new Vector2(28f, 24f), // Goal rests on the ground
            Props = SeaPeakProps, // Decorations for this theme
            Backdrop = "Backdrop/SeaPeak/", BackdropScript = "Tools/Art/make_hard_backdrops.py", // This trail's six background layers
        }, // Ends the theme
        [LevelTheme.ChocolateHills] = new ThemeArt // Hills Easy 1 art
        { // Opens the theme
            GroundTop = "choc_top", Soil = "choc_soil", Ledge = "ledge_choc", BoulderEnding = "_choc", // Ground and stone pictures
            GoalName = "Viewing Deck", GoalSprite = "viewing_deck", GoalOffset = new Vector2(24f, 24f), // Goal rests on the ground
            Props = ChocolateHillsProps, // Palm trees and cogon grass
            Backdrop = "Backdrop/ChocolateHills/", BackdropScript = "Tools/Art/make_hills_easy_backdrops.py", // Its own six background layers (D-063)
        }, // Ends the theme
        [LevelTheme.RollingPasture] = new ThemeArt // Hills Easy 2 art
        { // Opens the theme
            GroundTop = "pasture_top", Soil = "pasture_soil", Ledge = "ledge_pasture", BoulderEnding = "_pasture", // Ground and stone pictures
            GoalName = "Stone House", GoalSprite = "stone_house", GoalOffset = new Vector2(8f, 24f), // Goal rests on the ground
            Props = RollingPastureProps, // Goats, cows and stone fences
            Backdrop = "Backdrop/RollingPasture/", BackdropScript = "Tools/Art/make_hills_easy_backdrops.py", // Its own six background layers (D-063)
        }, // Ends the theme
        [LevelTheme.SandDunes] = new ThemeArt // Hills Easy 3 art
        { // Opens the theme
            GroundTop = "dune_top", Soil = "dune_soil", Ledge = "ledge_dune", BoulderEnding = "_dune", // Ground and stone pictures
            GoalName = "Dune Marker", GoalSprite = "dune_marker", GoalOffset = new Vector2(8f, 24f), // Goal rests on the ground
            Props = SandDunesProps, // Beach grass
            Backdrop = "Backdrop/SandDunes/", BackdropScript = "Tools/Art/make_hills_easy_backdrops.py", // Its own six background layers (D-063)
        }, // Ends the theme
    };

    static ThemeArt ArtFor(LevelTheme theme) // The art of a theme, or a clear error when the theme has no entry yet
    {
        if (Themes.TryGetValue(theme, out var art)) // Found it
        {
            if (string.IsNullOrEmpty(art.Backdrop)) throw new KeyNotFoundException($"Level theme {theme} has no background. Give it its own Backdrop folder in Assets/Editor/LevelThemes.cs (D-063)."); // Every theme, in every landform, gets its own background
            return art; // Ready to build
        }
        throw new KeyNotFoundException($"Level theme {theme} has no art yet. Add it to Themes in Assets/Editor/LevelThemes.cs."); // Say what to do
    }

    static void BuildGoal(LevelData level, ThemeArt art, Transform parent) // The end of the level: the theme's goal picture, with the goal flag
    {
        float goalX = level.Goal.X; // The goal from the level data
        var goal = M.Renderer(art.GoalName, S(art.GoalSprite), 4, parent); // The cave, arch or stone pile
        goal.transform.position = LevelGeometry.ToWorld(goalX + art.GoalOffset.x, art.GoalOffset.y); // Placed at the goal
        var goalFlag = M.Renderer("Goal Flag", S("flag"), 6, parent); // Flag before the goal
        goalFlag.transform.position = LevelGeometry.ToWorld(goalX - 28f, 14f); // Just before the goal; its 28-pixel height sits on the ground
    }

    static void BuildProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Living props: decoration only, no effect on play (D-055)
    {
        ArtFor(level.Layout.Theme).Props?.Invoke(level, pits, parent, explorer); // Only themes with props build any
    }

    static void VolcanicProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Volcano Trail: glowing cracks in the rock and faint fumaroles behind the trail
    {
        var layout = level.Layout; // Width and vents
        var crackFrames = Enumerable.Range(0, 4).Select(i => S($"Props/lava_crack_{i:00}")).ToArray(); // Glow cycle
        var wispFrames = Enumerable.Range(0, 4).Select(i => S($"Props/wisp_{i:00}")).ToArray(); // Faint wisp cycle
        int n = 0; // Counts the props, so neighbours glow out of step
        for (float x = 180f; x < layout.Width - 120f; x += 340f) // A crack about every 340 pixels
        {
            if (OverPit(pits, x, 12f)) continue; // Never over a pit
            var crack = M.Renderer("Lava Crack", crackFrames[0], 6, parent); // In front of the rock
            crack.transform.position = LevelGeometry.ToWorld(x, -22f - (n % 3) * 9f); // At different depths in the rock face
            Loop(crack.gameObject, crackFrames, 3f + (n % 3) * 0.7f); // Slow glow, each a little different
            n++; // Next prop
        }
        for (float x = 420f; x < layout.Width - 200f; x += 760f) // A wisp about every 760 pixels
        {
            if (OverPit(pits, x, 8f) || layout.Vents.Any(v => Mathf.Abs(v.CenterX - x) < 60f)) continue; // Not over a pit, and well away from the real vents
            var wisp = M.Renderer("Fumarole Wisp", wispFrames[0], 4, parent); // Behind boulders and the explorer
            wisp.transform.position = LevelGeometry.ToWorld(x, 12f); // Its 24-pixel height starts at the ground line
            Loop(wisp.gameObject, wispFrames, 5f + (n % 2)); // Gentle rising
            n++; // Next prop
        }
    }

    static void FoldedProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Folded Peaks: snow near the summit and an eagle that flies off
    {
        var layout = level.Layout; // Width and boulders
        var snow = new GameObject("Summit Snow").AddComponent<SummitSnow>(); // Drifting flakes
        snow.transform.SetParent(parent, false); // Keep it under the level
        snow.flakeSprites = new[] { S("Props/snow_00"), S("Props/snow_01") }; // Small and big flakes
        snow.fromX = layout.Width - 1500f; // The last stretch before the summit
        snow.toX = layout.Width; // Up to the end
        var bigBoulders = layout.Boulders.Where(b => b.Size == BoulderSize.Big && b.X < level.Goal.X).OrderByDescending(b => b.X).ToList(); // Big boulders before the summit, nearest the summit first
        if (bigBoulders.Count == 0) return; // No boulder for the eagle to sit on
        var perch = bigBoulders[0]; // The last big boulder before the summit
        var eagle = new GameObject("Eagle").AddComponent<EagleFlyOff>(); // The startled eagle
        eagle.transform.SetParent(parent, false); // Keep it under the level
        eagle.body = M.Renderer("Art", S("Props/eagle_00"), 8, eagle.transform); // Its picture, in front of the boulder
        eagle.transform.position = LevelGeometry.ToWorld(perch.X + 24f, 36f + 8f); // Standing on top of the 36-pixel boulder
        eagle.perched = S("Props/eagle_00"); // Sitting
        eagle.flapFrames = Enumerable.Range(1, 3).Select(i => S($"Props/eagle_{i:00}")).ToArray(); // Flying
        eagle.explorer = explorer; // Who startles it
    }

    static void PineProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Trees and very light frost
    { // Starts the pine decoration
        for (float x = 180f; x < level.Layout.Width - 120f; x += 260f) // Spaces the trees along the trail
        { // Opens the tree placement
            if (OverPit(pits, x, 12f) || level.Layout.Ledges.Any(l => x + 12f > l.X && x - 12f < l.X + l.Width) || level.Layout.CrumbleLedges.Any(l => x + 12f > l.X && x - 12f < l.X + CrumbleLedge.Width)) continue; // Keeps trees off pits and ledges
            var tree = M.Renderer("Pine Tree", S("Props/pine_tree"), 3, parent); // Behind the playable stone
            tree.transform.position = LevelGeometry.ToWorld(x, 24f); // Roots on ground
        } // Ends tree placement
        FullTrailSnow(level, parent, 36); // Sparse frost across the whole trail
    } // Ends the pine decoration
    static void MossProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Ferns and vent sulfur
    { // Starts the moss decoration
        for (float x = 90f; x < level.Layout.Width - 80f; x += 180f) // Spaces the ferns
        { // Opens fern placement
            if (OverPit(pits, x, 8f)) continue; // Keeps ferns on ground
            var fern = M.Renderer("Fern", S("Props/fern"), 7, parent); // In front of the ground
            fern.transform.position = LevelGeometry.ToWorld(x, 6f); // Roots on ground
        } // Ends fern placement
        foreach (var vent in level.Layout.Vents) // Decorates every vent in this theme
            foreach (float side in new[] { -1f, 1f }) // Both sides of the rim
            { // Opens sulfur placement
                var crust = M.Renderer("Sulfur Crust", S("Props/sulfur_crust"), 7, parent); // Yellow crust near the vent
                crust.transform.position = LevelGeometry.ToWorld(vent.CenterX + side * 24f, 3f); // Rests beside the rim
            } // Ends sulfur placement
    } // Ends the moss decoration
    static void SnowProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) => FullTrailSnow(level, parent, 192); // Snow across the whole peak
    static void FullTrailSnow(LevelData level, Transform parent, int count, bool useLevelClock = false) // Reuses the existing snow view
    { // Starts the snow setup
        var snow = new GameObject("Trail Snow").AddComponent<SummitSnow>(); // Owns the drifting flakes
        snow.transform.SetParent(parent, false); // Keeps it with the level
        snow.flakeSprites = new[] { S("Props/snow_00"), S("Props/snow_01") }; // Existing snow pictures
        snow.fromX = 0f; snow.toX = level.Layout.Width; // Covers the whole trail
        snow.count = count; // Uses sparse frost or fuller snow
        snow.useLevelClock = useLevelClock; // New Hard snow waits when play pauses
    } // Ends the snow setup

    static void LavaSlopeProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "burnt_tree", 210f, 390f); // Scorched trees off the trail objects
    } // Ends the prop setup

    static void StormRidgeProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "bent_tree", 220f, 350f); // Trees lean into the rain
        var rain = new GameObject("Ridge Rain").AddComponent<RidgeRain>(); // A light foreground shower
        rain.frames = Enumerable.Range(0, 2).Select(i => HardSprite($"Props/rain_{i:00}")).ToArray(); // Both rain pictures
        rain.cameraTransform = Camera.main.transform; // The streaks follow the camera
        rain.streaks = Enumerable.Range(0, 18).Select(i => M.Renderer("Rain Streak", rain.frames[0], 22, rain.transform)).ToArray(); // Draws rain in front
        foreach (var streak in rain.streaks) streak.color = new Color(1f, 1f, 1f, 0.42f); // The explorer shows through the rain
    } // Ends the prop setup

    static void EagleForestProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "tall_tree", 240f, 360f); // Tall forest canopy
        GroundProps(level, parent, "orchid", 135f, 245f); // Flowers at the forest floor
        var eagle = new GameObject("Forest Eagle").AddComponent<ForestEagle>(); // A passing bird above the trail
        eagle.cameraTransform = Camera.main.transform; // Crosses the view from time to time
        eagle.frames = Enumerable.Range(1, 3).Select(i => S($"Props/eagle_{i:00}")).ToArray(); // Reuses the flying frames
        eagle.art = M.Renderer("Art", null, 8, eagle.transform); // The eagle stays behind the explorer
        eagle.art.flipX = true; // Faces left as it flies
    } // Ends the prop setup

    static void BonsaiForestProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "bonsai_tree", 210f, 310f); // Small old trees
        GroundProps(level, parent, "pitcher_plant", 135f, 220f); // Pitcher plants on the forest floor
    } // Ends the prop setup

    static void GlacierHornProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "edelweiss", 200f, 235f); // Small mountain flowers
        FullTrailSnow(level, parent, 192, useLevelClock: true); // Reuses the peak snow and pauses it with the level
    } // Ends the prop setup

    static void EquatorSummitProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "groundsel", 230f, 370f); // Tall groundsels along the climb
    } // Ends the prop setup

    static void SeaPeakProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Adds the theme's decoration
    { // Starts the prop setup
        GroundProps(level, parent, "silversword", 200f, 290f); // Silverswords on the slope
    } // Ends the prop setup

    static void ChocolateHillsProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Palm trees and cogon grass on the hills
    { // Starts the prop setup
        HillsGroundProps(level, parent, "palm_tree", 200f, 420f, 20f, 3); // Palm trees about every 420 pixels, behind the ground
        HillsGroundProps(level, parent, "cogon_tuft", 90f, 150f, 4f, 7); // Cogon grass tufts about every 150 pixels, in front
    } // Ends the prop setup

    static void RollingPastureProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Goats, cows and stone fences on the pasture
    { // Starts the prop setup
        HillsGroundProps(level, parent, "stone_fence", 260f, 520f, 5f, 3); // Stone fences about every 520 pixels, behind the ground
        var goatFrames = new[] { S("Props/goat_00"), S("Props/goat_01") }; // Two goat pictures for the swap
        float[] goatX = { 600f, 1700f, 3950f }; // Where the three goats stand
        float[] goatSeconds = { 1.5f, 2f, 2.5f }; // How long each goat keeps one picture; each goat is a little different
        for (int i = 0; i < goatX.Length; i++) HillsAnimal(level, parent, "Goat", goatFrames, goatX[i], 6f, goatSeconds[i]); // Places each goat, if its spot passes the fit check
        var cowFrames = new[] { S("Props/cow_00"), S("Props/cow_01") }; // Two cow pictures for the swap
        float[] cowX = { 1100f, 2550f, 4550f }; // Where the three cows stand
        float[] cowSeconds = { 2f, 1.8f, 2.4f }; // How long each cow keeps one picture; each cow is a little different
        for (int i = 0; i < cowX.Length; i++) HillsAnimal(level, parent, "Cow", cowFrames, cowX[i], 8f, cowSeconds[i]); // Places each cow, if its spot passes the fit check
    } // Ends the prop setup

    static void SandDunesProps(LevelData level, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Beach grass on the dunes
    { // Starts the prop setup
        HillsGroundProps(level, parent, "beach_grass", 120f, 200f, 6f, 7); // Beach grass about every 200 pixels, in front
    } // Ends the prop setup

    static void HillsGroundProps(LevelData level, Transform parent, string name, float start, float spacing, float centreY, int order) // Places one kind of Hills decoration along the trail
    { // Starts the placement
        var sprite = S("Props/" + name); // Loads the prop picture
        float halfWidth = sprite.rect.width / 2f; // Half the picture width, so the whole picture is checked
        for (float x = start; x < level.Goal.X - 90f; x += spacing) // Spaces the props out until the goal
        { // Tests one place
            if (!HillsPropFits(level, x, halfWidth)) continue; // Skips any place over a gap, ledge or boulder
            var prop = M.Renderer(name, sprite, order, parent); // Draws the decoration
            prop.transform.position = LevelGeometry.ToWorld(x, centreY); // Puts its middle at the given height
        } // Moves to the next place
    } // Ends the placement

    static void HillsAnimal(LevelData level, Transform parent, string name, Sprite[] frames, float x, float centreY, float seconds) // One animal that swaps its two pictures on a timer
    { // Starts the animal
        float halfWidth = frames[0].rect.width / 2f; // Half the picture width, so the whole animal is checked
        if (!HillsPropFits(level, x, halfWidth)) { Debug.LogWarning($"{name} at x {x} is refused by the fit check and is not placed"); return; } // Report the refused spot and do not move the animal
        var animal = M.Renderer(name, frames[0], 4, parent); // Draws it behind the explorer
        animal.transform.position = LevelGeometry.ToWorld(x, centreY); // Puts its middle at the given spot
        Loop(animal.gameObject, frames, 1f / seconds); // Swaps the frames every given number of seconds
    } // Ends the animal

    static bool HillsPropFits(LevelData level, float x, float halfWidth) // The shared check, plus the Hills rules for ledges and soft sand
    { // Opens the block
        bool Overlaps(float left, float right) => x + halfWidth >= left && x - halfWidth <= right; // One overlap test for both ledges and sand
        var layout = level.Layout; // The placed objects
        return PropFits(level, x, halfWidth) // Pits, pickups, flags, boulders and the rest
            && !layout.Ledges.Any(l => Overlaps(l.X, l.X + l.Width)) // No prop under a ledge
            && !layout.SandZones.Any(s => Overlaps(s.X, s.X + Mathf.Min(16f, s.Width))); // No grass on the first 16 pixels of soft sand, so the soft tiles stay readable (spec section 9)
    } // Ends the block

    static void GroundProps(LevelData level, Transform parent, string name, float start, float spacing) // Places one kind of ground decoration
    { // Starts the placement
        var sprite = HardSprite("Props/" + name); // Loads the theme prop
        float halfWidth = sprite.rect.width / 2f; // Keeps the whole prop out of the path
        for (float x = start; x < level.Goal.X - 90f; x += spacing) // Spaces props out until the goal
        { // Tests one place
            if (!PropFits(level, x, halfWidth)) continue; // Leaves game objects easy to see
            var prop = M.Renderer(name, sprite, 4, parent); // Draws decoration behind objects
            prop.transform.position = LevelGeometry.ToWorld(x, sprite.rect.height / 2f); // Sets its base at ground level
        } // Moves to the next place
    } // Ends the placement

    static bool PropFits(LevelData level, float x, float halfWidth) // Checks the whole prop against trail objects
    { // Starts the space check
        bool Over(float left, float right) => x + halfWidth >= left && x - halfWidth <= right; // Checks a span of trail
        var layout = level.Layout; // The placed objects
        return !level.Hazards.Any(h => Over(h.Place.X, h.Place.X + h.Width)) // Keeps pits and spikes clear
            && !level.Pickups.Any(p => Over(p.Place.X - 8f, p.Place.X + 8f)) // Leaves the full pickup visible
            && !level.Checkpoints.Any(c => Over(c.X - 12f, c.X + 12f)) // Leaves room for the flag
            && !layout.RollingBoulders.Any(b => Over(b.End - 8f, b.Spawn + 8f)) // Keeps clear of rolling boulders
            && !layout.Rockfalls.Any(r => Over(r.X - 8f, r.X + 8f)) // Keeps clear of falling rocks
            && !layout.Vents.Any(v => Over(v.X, v.X + v.Width)) // Keeps the vents clear
            && !layout.Boulders.Any(b => Over(b.X, b.X + b.Width)) // Keeps the fixed rocks visible
            && !layout.CrumbleLedges.Any(l => Over(l.X, l.X + CrumbleLedge.Width)) // Leaves room for a breaking step
            && !layout.MovingPlatforms.Any(p => Over(p.X0, p.X1 + MovingPlatform.Width)) // Leaves room for a moving crossing
            && !layout.IceZones.Any(i => Over(i.X, i.X + i.Width)); // Keeps the ice easy to see
    } // Ends the space check

    static bool OverPit(List<Hazard> pits, float x, float halfWidth) => pits.Any(p => x + halfWidth > p.Place.X && x - halfWidth < p.Place.X + p.Width); // True when a prop this wide at x would hang over a gap
}
