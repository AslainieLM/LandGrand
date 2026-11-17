using System.Collections.Generic; // Uses the solid list
using System.Linq; // Loads frame lists
using LandGrand.Play; // Uses object views
using LandGrand.Rules; // Uses layout data
using UnityEngine; // Uses scene objects
using M = global::MainMenuBuilder; // Reuses sprite helpers
public static partial class LevelBuilder // Builds objects from shared level data
{ // Opens the builder
    static void BuildObjects(LevelData data, LevelGeometry geometry, ExplorerMotor explorer, LevelController controller) // Builds the four object kinds
    { // Starts the object build
        var solids = new List<DynamicSolid>(); // Remembers moving and vanishing solids
        foreach (var ledge in data.Layout.CrumbleLedges) // Each breakable slab
        { // Opens the slab build
            var view = new GameObject("Crumbling Ledge").AddComponent<CrumbleLedgeView>(); // Owns the warning and break
            view.transform.SetParent(geometry.transform, false); // Keeps it with the level
            view.x = ledge.X; view.top = ledge.Top; // Exact layout placement
            view.explorer = explorer; view.level = controller; // Watches feet and plays sound
            view.frames = Enumerable.Range(0, 4).Select(i => S($"crumble_{i:00}")).ToArray(); // Whole through broken
            view.breakSound = NormalSound("sfx_crumble.wav"); // Break sound
            view.chip = S("Props/crumble_bit"); // Falling pieces
            view.art = M.Renderer("Art", view.frames[0], 6, view.transform); // Same depth as a fixed ledge
            view.art.transform.position = LevelGeometry.ToWorld(ledge.X + 16f, ledge.Top - 5f); // Initial slab position
            solids.Add(view); // Holds the explorer up like a ledge
        } // Ends the slab build
        for (int i = 0; i < data.Layout.MovingPlatforms.Count; i++) // Each moving crossing
        { // Opens the platform build
            var platform = data.Layout.MovingPlatforms[i]; // Reads the route for this platform
            if (platform.IsVertical) // This route is a balloon basket
            { // Opens the basket build
                solids.Add(BuildBalloon(platform, i, geometry.transform, controller)); // Adds the moving basket top to the shapes
                continue; // The horizontal slab build does not apply
            } // Ends the basket build
            var view = new GameObject("Moving Platform").AddComponent<MovingPlatformView>(); // Owns the moving solid
            view.transform.SetParent(geometry.transform, false); // Keeps it with the level
            view.x0 = platform.X0; view.x1 = platform.X1; view.top = platform.Top; // Exact travel endpoints
            view.art = M.Renderer("Art", S("platform"), 6, view.transform); // Same depth as a ledge
            view.art.transform.position = LevelGeometry.ToWorld(platform.X0 + 24f, platform.Top - 5f); // Starts at the near end
            solids.Add(view); // Makes it carry the explorer like a ledge
        } // Ends the platform build
        BuildFloats(data, geometry, controller, solids); // Adds the rafts and the water sounds
        geometry.dynamicSolids = solids.ToArray(); // Gives the motor the placed solids
        foreach (var rock in data.Layout.Rockfalls) // Each timed rock
        { // Opens the rock build
            var view = new GameObject("Falling Rock").AddComponent<RockfallView>(); // Owns its warning and fall
            view.transform.SetParent(geometry.transform, false); // Keeps it with the level
            view.x = rock.X; view.offset = rock.Offset; // Exact landing spot and timing
            view.explorer = explorer; view.level = controller; // Distance and sound player
            view.warningFrames = Enumerable.Range(0, 2).Select(i => S($"rock_warn_{i:00}")).ToArray(); // Shadow grows darker
            view.pebbleFrames = Enumerable.Range(0, 2).Select(i => S($"Props/pebbles_{i:00}")).ToArray(); // Trickle warning
            view.dustFrames = explorer.dustFrames; // Reuses start-screen dust
            view.landSound = NormalSound("sfx_rockfall.wav"); // Landing sound
            view.rockSprite = S("rock_fall"); // Falling rock picture
            view.shadow = M.Renderer("Shadow", view.warningFrames[0], 7, view.transform); // Warning above the ground
            view.shadow.transform.position = LevelGeometry.ToWorld(rock.X, view.warningFrames[0].rect.height / 2f); // Rests on ground
            view.rock = M.Renderer("Rock", null, 19, view.transform); // Just behind the explorer
            view.pebbles = M.Renderer("Pebbles", null, 19, view.transform); // Trickle above the warning
        } // Ends the rock build
        foreach (var wind in data.Layout.WindZones) // Each gust area
        { // Opens the gust build
            var view = new GameObject("Wind Zone").AddComponent<WindZoneView>(); // Owns streaks and sound
            view.transform.SetParent(geometry.transform, false); // Keeps it with the level
            view.x = wind.X; view.width = wind.Width; view.offset = wind.Offset; // Exact zone and timing
            view.explorer = explorer; view.level = controller; // Distance and sound player
            view.frames = Enumerable.Range(0, 4).Select(i => S($"Props/gust_{i:00}")).ToArray(); // Streak animation
            view.gustSound = NormalSound("sfx_gust.wav"); // Warning sound
            view.streaks = Enumerable.Range(0, 6).Select(i => M.Renderer("Streak " + i, null, 22, view.transform)).ToArray(); // Six streaks in front of the explorer
        } // Ends the gust build
    } // Ends the object build
    static AudioClip NormalSound(string name) // Loads a Normal effect with a clear missing-file error
    { // Starts the sound lookup
        var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Loads the existing recording
        if (clip == null) Debug.LogError("Missing Assets/Audio/" + name + ". Run: python Tools/Audio/make_normal_audio.py"); // Names the repair command
        return clip; // Gives the sound to its view
    } // Ends the lookup
} // Ends the builder
