using System.Collections.Generic; // Holds the moving solid list
using System.Linq; // Loads frames and checks for water pits
using LandGrand.Play; // Gives the new level parts and views
using LandGrand.Rules; // Gives the level and object data
using UnityEditor; // Loads the Hills Normal sounds
using UnityEngine; // Gives sprites, shapes and scene objects
using K = PixelSceneKit; // Shares the editor sprite loader
using M = global::MainMenuBuilder; // Shares the sprite renderer helper

public static partial class LevelBuilder // Builds water, bats, currents and balloons from the layout
{ // Opens the builder
    static Sprite HillsNormalSprite(string name) => K.Sprite(SpriteFolder, name, K.HillsNormalArtScript); // Names the correct generator for new level sprites

    static AudioClip HillsNormalSound(string name) // Loads a Hills Normal sound
    { // Opens the audio lookup
        const string script = "Tools/Audio/make_hills_normal_audio.py"; // Generates the new effects
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Reads the imported file
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}. Run: python {script}"); // Names the generator if the asset is absent
        return clip; // Returns the clip for the view
    } // Ends the audio lookup

    static void DrawGap(float x, float width, bool wet, bool hasFloor, ThemeArt art, List<Rect> solids, Transform parent) // Draws a pit or a safe pool
    { // Opens the gap builder
        float depth = hasFloor ? SinkingFloat.PoolDepth : 64f; // The practice pool is only sixteen pixels deep
        var fill = wet ? HillsNormalSprite("water_fill") : S("pit_fill"); // Uses water or the original pit tile
        Tiled(wet ? "Water" : "Pit", fill, new Rect(x, -depth, width, depth), 3, parent); // Fills the gap without adding any collision
        if (!hasFloor) return; // Only a safe pool has a floor
        var floor = new Rect(x, -64f, width, 64f - depth); // Makes solid ground under the water
        solids.Add(floor); // The explorer can land here safely
        Tiled("Pool Floor", ThemeSprite(art, art.Soil), floor, 5, parent); // Draws the same soil as the banks
    } // Ends the gap builder

    static void BuildFloats(LevelData data, LevelGeometry geometry, LevelController controller, List<DynamicSolid> solids) // Adds raft shapes and water splashes
    { // Opens the raft builder
        var layout = data.Layout; // Reads the placed rafts and pools
        if (layout.Floats.Count == 0 && layout.Pools.Count == 0 && !data.Hazards.Any(h => h.IsWater)) return; // Dry levels need no water part
        var watcher = controller.gameObject.AddComponent<FloatWatcher>(); // Adds one part for every raft and water gap
        watcher.splash = HillsNormalSound("sfx_splash.wav"); // Loads the soft splash
        watcher.ripples = Enumerable.Range(0, 2).Select(i => HillsNormalSprite($"float_ripple_{i:00}")).ToArray(); // Reads both warning frames
        watcher.floats = new FloatView[layout.Floats.Count]; // Reserves one view per raft
        for (int i = 0; i < watcher.floats.Length; i++) // Builds each platform at the water line
        { // Opens one raft
            var rule = layout.Floats[i]; // Reads the placement
            var view = new GameObject("Sinking Float").AddComponent<FloatView>(); // Gives the raft a one-way top
            view.transform.SetParent(geometry.transform, false); // Keeps it under the level geometry
            view.raft = M.Renderer("Raft", HillsNormalSprite("float_raft"), 6, view.transform); // Draws the bamboo behind the explorer
            view.raft.transform.position = LevelGeometry.ToWorld(rule.X + SinkingFloat.Width / 2f, rule.Top - 5f); // Rests at the water line
            view.ripple = M.Renderer("Warning Ripple", watcher.ripples[0], 7, view.transform); // Shows the warning above the raft
            view.ripple.enabled = false; // Waits for a rider
            watcher.floats[i] = view; // Connects the raft to the float watcher
            solids.Add(view); // Makes its top available to the motor
        } // Ends one raft
    } // Ends the raft builder

    static void BuildBatsAndCurrents(LevelLayout layout, LevelGeometry geometry, LevelController controller) // Adds the two optional level parts
    { // Opens the part builder
        if (layout.Bats.Count > 0) // This trail has bat loops
        { // Opens the flock builder
            var flock = controller.gameObject.AddComponent<BatFlock>(); // One optional part keeps all bats
            flock.flap = HillsNormalSound("sfx_bat_flap.wav"); // Loads the soft touch sound
            flock.frames = Enumerable.Range(0, 3).Select(i => HillsNormalSprite($"bat_{i:00}")).ToArray(); // Reads all three poses
            flock.bats = layout.Bats.Select(rule => BuildBat(rule, flock.frames[0], geometry.transform)).ToArray(); // Draws each placed bat
        } // Ends the flock builder
        if (layout.Currents.Count == 0) return; // A trail without river strips needs no current part
        var watcher = controller.gameObject.AddComponent<CurrentWatcher>(); // One part supplies the ground push
        watcher.frames = Enumerable.Range(0, 2).Select(i => HillsNormalSprite($"current_mark_{i:00}")).ToArray(); // Loads the two moving marks
        watcher.strips = layout.Currents.Select(zone => BuildCurrentStrip(zone, watcher.frames[0], geometry.transform)).ToArray(); // Draws the marks for each strip
    } // Ends the part builder

    static SpriteRenderer BuildBat(Bat rule, Sprite sprite, Transform parent) // Builds one visible bat in its start pose
    { // Opens the bat build
        var art = M.Renderer("Gentle Bat", sprite, 19, parent); // Draws it just behind the explorer
        var start = rule.PositionAt(0f); // Shows the first loop position
        art.transform.position = LevelGeometry.ToWorld(Mathf.Round(start.X), Mathf.Round(start.Y)); // Starts on a whole pixel
        return art; // Gives the flock this view
    } // Ends the bat build

    static CurrentWatcher.Strip BuildCurrentStrip(CurrentZone zone, Sprite sprite, Transform parent) // Gives one strip its repeating marks
    { // Opens the mark build
        int count = Mathf.Max(1, Mathf.CeilToInt(zone.Width / CurrentWatcher.MarkSpacing)); // Shares marks across the full strip width
        var marks = new SpriteRenderer[count]; // Keeps the renderers in placement order
        for (int i = 0; i < count; i++) // Builds each mark
        { // Opens one mark
            var mark = M.Renderer("Current Mark", sprite, 6, parent); // Draws the ripple just over the tile
            mark.transform.position = LevelGeometry.ToWorld(Mathf.Round(zone.X + (i + 0.5f) * zone.Width / count), -4f); // Places the marks along the strip
            mark.flipX = zone.Direction < 0; // Points the chevrons with the flow
            marks[i] = mark; // Lets the watcher move the ripple
        } // Ends one mark
        return new CurrentWatcher.Strip { marks = marks }; // Returns the placed views for this strip
    } // Ends the mark build

    static BalloonView BuildBalloon(MovingPlatform rule, int index, Transform parent, LevelController level) // Builds a vertical platform and its burner
    { // Opens the balloon build
        var view = new GameObject("Hot-air Balloon").AddComponent<BalloonView>(); // Gives the basket a one-way top
        view.transform.SetParent(parent, false); // Keeps it under the level geometry
        view.platformIndex = index; // Reads its route from the layout at runtime
        view.level = level; // Shares the level's sound player
        view.burner = HillsNormalSound("sfx_burner.wav"); // Loads the start-of-climb sound
        view.flameFrames = Enumerable.Range(0, 2).Select(i => HillsNormalSprite($"balloon_flame_{i:00}")).ToArray(); // The two flame pictures
        view.art = M.Renderer("Balloon", HillsNormalSprite("balloon"), 6, view.transform); // Draws the envelope and platform
        view.flame = M.Renderer("Burner Flame", view.flameFrames[0], 7, view.transform); // Draws the flame in front of the ropes
        float mid = rule.X0 + MovingPlatform.Width / 2f, top = Mathf.Round(rule.TopAt(0f)); // Places the start basket at its phase height
        view.art.transform.position = LevelGeometry.ToWorld(mid, top + 30f); // Lines up the basket top with the solid
        view.flame.transform.position = LevelGeometry.ToWorld(mid, top + 6f); // Places the burner just above the basket
        view.flame.enabled = rule.IsRisingAt(0f); // Starts with the right flame for this phase
        return view; // Returns the new dynamic platform
    } // Ends the balloon build
} // Ends the builder
