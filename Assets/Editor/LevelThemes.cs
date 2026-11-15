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
        public string Backdrop = "Backdrop/Mountains/"; // Folder of the six moving background layers, inside the level sprite folder (D-049, D-063)
        public string BackdropScript = "Tools/Art/make_mountains_backdrop.py"; // Script that draws those layers
    }

    static readonly Dictionary<LevelTheme, ThemeArt> Themes = new Dictionary<LevelTheme, ThemeArt> // Every theme's art
    {
        [LevelTheme.Meadow] = new ThemeArt // Mountain Trail (Easy 1)
        {
            GroundTop = "grass_top", Soil = "soil", Ledge = "ledge", BoulderEnding = "", // Green grass over brown soil
            GoalName = "Cave", GoalSprite = "cave", GoalOffset = new Vector2(20f, 28f), // Cave entrance; its 56-pixel height sits on the ground and its opening is at the goal
            Props = null, // No living props
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
    };

    static ThemeArt ArtFor(LevelTheme theme) // The art of a theme, or a clear error when the theme has no entry yet
    {
        if (Themes.TryGetValue(theme, out var art)) return art; // Found it
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
    static void FullTrailSnow(LevelData level, Transform parent, int count) // Reuses the existing snow view
    { // Starts the snow setup
        var snow = new GameObject("Trail Snow").AddComponent<SummitSnow>(); // Owns the drifting flakes
        snow.transform.SetParent(parent, false); // Keeps it with the level
        snow.flakeSprites = new[] { S("Props/snow_00"), S("Props/snow_01") }; // Existing snow pictures
        snow.fromX = 0f; snow.toX = level.Layout.Width; // Covers the whole trail
        snow.count = count; // Uses sparse frost or fuller snow
    } // Ends the snow setup

    static bool OverPit(List<Hazard> pits, float x, float halfWidth) => pits.Any(p => x + halfWidth > p.Place.X && x - halfWidth < p.Place.X + p.Width); // True when a prop this wide at x would hang over a gap
}
