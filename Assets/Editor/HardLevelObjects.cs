using System.Collections.Generic; // Holds the ice zones
using System.Linq; // Loads the animation frames
using LandGrand.Play; // Gives the level objects
using LandGrand.Rules; // Gives the layout data
using UnityEditor; // Loads the existing sounds
using UnityEngine; // Gives scene objects and sprites
using K = PixelSceneKit; // Reuses the sprite loader
using M = global::MainMenuBuilder; // Reuses sprite drawing

public static partial class LevelBuilder // Builds the new objects from any level's layout
{ // Opens the builder
    const string HardArtScript = "Tools/Art/make_hard_art.py"; // Makes the Hard pictures
    const string HardAudioScript = "Tools/Audio/make_hard_audio.py"; // Makes the Hard sounds

    static void BuildHardObjects(LevelData data, LevelGeometry geometry, ExplorerMotor explorer, LevelController level) // Connects placed boulders and ice
    { // Starts the object build
        var layout = data.Layout; // Gets the placement lists
        if (layout.RollingBoulders.Count > 0) // Only trails with rolling boulders need the part
        { // Starts the boulder setup
            var watcher = level.gameObject.AddComponent<RollingBoulderWatcher>(); // Shares the rolls with the hazard watcher
            var rumble = HardSound("sfx_boulder_rumble.wav"); // The short warning
            var roll = HardSound("sfx_boulder_roll.wav"); // The repeating roll
            for (int i = 0; i < layout.RollingBoulders.Count; i++) // Builds each placed boulder
                BuildRollingBoulder(layout.RollingBoulders[i], i, geometry.transform, watcher, explorer.dustFrames, rumble, roll); // Adds its view
        } // Ends the boulder setup
        if (layout.IceZones.Count == 0) return; // Trails without ice need no ice part
        level.gameObject.AddComponent<IceWatcher>().slideSound = HardSound("sfx_ice_slide.wav"); // Gives the ice its slide sound
        foreach (var ice in layout.IceZones) BuildIceGlints(ice, geometry.transform); // Adds glints to each strip of icy ground
    } // Ends the object build

    static void BuildRollingBoulder(RollingBoulder rule, int index, Transform parent, RollingBoulderWatcher watcher, Sprite[] dust, AudioClip rumble, AudioClip roll) // Builds one boulder view
    { // Starts the boulder build
        var view = new GameObject("Rolling Boulder").AddComponent<RollingBoulderView>(); // Owns the rolling art
        view.transform.SetParent(parent, false); // Keeps it with the level
        view.index = index; // Finds its own rule in the layout
        view.watcher = watcher; // Connects to the shared roll state
        view.warningFrames = Enumerable.Range(0, 2).Select(i => HardSprite($"Props/rumble_dust_{i:00}")).ToArray(); // Loads both warning pictures
        view.rollFrames = Enumerable.Range(0, 4).Select(i => HardSprite($"roll_boulder_{i:00}")).ToArray(); // Loads the turning pictures
        view.dustFrames = dust; // Reuses the explorer dust
        view.warning = M.Renderer("Rumble Dust", null, 7, view.transform); // Warning dust sits above the ground
        view.warning.transform.position = LevelGeometry.ToWorld(rule.Spawn, 4f); // Centres the warning at the spawn
        view.rock = M.Renderer("Rock", null, 19, view.transform); // Rolls just behind the explorer
        view.dust = M.Renderer("End Dust", null, 19, view.transform); // The short dust puff at the end
        view.dust.transform.position = LevelGeometry.ToWorld(rule.End, 2f); // Rests on the ground
        view.rumble = rumble; // Uses the warning recording
        view.rollSound = view.gameObject.AddComponent<AudioSource>(); // A separate sound player for the loop
        view.rollSound.clip = roll; // Uses the rolling recording
        view.rollSound.loop = true; // Plays while the boulder can be heard
        view.rollSound.playOnAwake = false; // Starts silent
    } // Ends the boulder build

    static void DrawGroundTop(Rect piece, string top, IReadOnlyList<IceZone> zones, Transform parent) // Uses ice tiles in place of the usual top tiles
    { // Starts the ground art
        float from = piece.xMin; // Starts at the left edge of the ground
        foreach (var ice in zones.OrderBy(zone => zone.X)) // Checks strips from left to right
        { // Starts one strip
            float left = Mathf.Max(from, ice.X); // Clips ice to the ground already being drawn
            float right = Mathf.Min(piece.xMax, ice.X + ice.Width); // Stops ice at the right edge of the ground
            if (right <= left) continue; // This strip misses this piece
            if (left > from) Tiled("Ground Top", top, new Rect(from, -8f, left - from, 8f), 5, parent); // Normal tiles before the ice
            Tiled("Ice Top", HardSprite("ice_top"), new Rect(left, -8f, right - left, 8f), 5, parent); // Ice replaces the top tile
            from = right; // Resumes after the ice
        } // Ends the strip
        if (piece.xMax > from) Tiled("Ground Top", top, new Rect(from, -8f, piece.xMax - from, 8f), 5, parent); // The rest of the ground
    } // Ends the ground art

    static void BuildIceGlints(IceZone ice, Transform parent) // Adds small glints to one ice strip
    { // Starts the glint build
        var view = new GameObject("Ice Zone").AddComponent<IceZoneView>(); // Holds the twinkling art
        view.transform.SetParent(parent, false); // Keeps it with the level
        view.frames = Enumerable.Range(0, 2).Select(i => HardSprite($"Props/ice_glint_{i:00}")).ToArray(); // Both glint pictures
        view.glints = new SpriteRenderer[ice.Width >= 500f ? 3 : 2]; // Uses more glints on the wider strips
        for (int i = 0; i < view.glints.Length; i++) // Spaces glints across the ice
        { // Starts one glint
            var glint = M.Renderer("Glint", view.frames[0], 5, view.transform); // Glints at the ground edge
            float x = ice.X + (i + 1f) * ice.Width / (view.glints.Length + 1f); // Leaves room at both ends
            glint.transform.position = LevelGeometry.ToWorld(Mathf.Round(x), 2.5f); // Aligns the odd-sized glint to pixels
            view.glints[i] = glint; // Lets the view change its picture
        } // Ends the glint
    } // Ends the glint build

    static Sprite HardSprite(string name) => K.Sprite(SpriteFolder, name, HardArtScript); // Loads the existing Hard sprite
    static AudioClip HardSound(string name) // Loads the existing Hard effect
    { // Starts the sound lookup
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Loads the supplied recording
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}. Run: python {HardAudioScript}"); // Names the script that makes it
        return clip; // Gives the sound to the view
    } // Ends the sound lookup
} // Ends the builder
