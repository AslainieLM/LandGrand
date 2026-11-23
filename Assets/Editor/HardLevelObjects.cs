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
    const string HillsAudioScript = "Tools/Audio/make_hills_audio.py"; // Makes the Hills sounds, such as the sand step
    const string HillsArtScript = "Tools/Art/make_hills_easy_art.py"; // Makes the Hills pictures, such as the sand puff

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
        if (layout.Logs.Count > 0) BuildLogs(layout, ArtFor(layout.Theme), geometry.transform, level); // Only levels with logs need their part; the theme names the roller art
        if (layout.IceZones.Count == 0 && layout.MudZones.Count == 0) return; // Dry ground needs no slide part
        var watcherForGround = level.gameObject.AddComponent<IceWatcher>(); // Both surfaces use the ice rule
        if (layout.IceZones.Count > 0) watcherForGround.slideSound = HardSound("sfx_ice_slide.wav"); // Gives the ice its slide sound
        if (layout.MudZones.Count > 0) BuildMudSplash(watcherForGround, geometry.transform); // Gives mud its sound and splash
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

    static void DrawGroundTop(Rect piece, ThemeArt art, LevelLayout layout, Transform parent) // Uses ice, sand or water on the ground line
    { // Starts the ground art
        string iceTile = art.UsedIceTop; // The theme's ice tile, or the Mountains Hard ice (D-071)
        string iceScript = art.IceTopScript ?? HardArtScript; // The script that draws that tile
        var strips = layout.IceZones.Select(ice => (x: ice.X, w: ice.Width, name: "Ice Top", tile: iceTile, script: iceScript)) // Each ice strip
            .Concat(layout.SandZones.Select(s => (x: s.X, w: s.Width, name: "Soft Sand Top", tile: "soft_top", script: HillsArtScript))) // Each soft sand strip
            .Concat(layout.Currents.Select(c => (x: c.X, w: c.Width, name: "Current Top", tile: "current_top", script: K.HillsNormalArtScript))) // Each river strip
            .Concat(layout.MudZones.Select(m => (x: m.X, w: m.Width, name: "Mud Top", tile: "mud_top", script: K.HillsHardArtScript))) // Each mud strip
            .OrderBy(strip => strip.x); // Checks strips from left to right
        var top = ThemeSprite(art, art.GroundTop); // Loads the normal top for this theme
        float from = piece.xMin; // Starts at the left edge of the ground
        foreach (var strip in strips) // Each strip in order
        { // Starts one strip
            float left = Mathf.Max(from, strip.x); // Clips the strip to the ground already being drawn
            float right = Mathf.Min(piece.xMax, strip.x + strip.w); // Stops the strip at the right edge of the ground
            if (right <= left) continue; // This strip misses this piece
            if (left > from) Tiled("Ground Top", top, new Rect(from, -8f, left - from, 8f), 5, parent); // Normal tiles before the strip
            Tiled(strip.name, K.Sprite(SpriteFolder, strip.tile, strip.script), new Rect(left, -8f, right - left, 8f), 5, parent); // The ice or sand tile replaces the top tile
            from = right; // Resumes after the strip
        } // Ends the strip
        if (piece.xMax > from) Tiled("Ground Top", top, new Rect(from, -8f, piece.xMax - from, 8f), 5, parent); // The rest of the ground
    } // Ends the ground art

    static void BuildSoftSand(LevelLayout layout, LevelGeometry geometry, LevelController level) // Adds the sand part, its step sound and its puffs
    { // Starts the sand build
        if (layout.SandZones.Count == 0) return; // Trails without soft sand need no sand part
        var watcher = level.gameObject.AddComponent<SandWatcher>(); // The sand part sits on the level controller
        watcher.stepSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_sand_step.wav"); // The sand crunch
        if (watcher.stepSound == null) Debug.LogError($"Missing Assets/Audio/sfx_sand_step.wav. Run: python {HillsAudioScript}"); // Names the script that makes it
        watcher.puffFrames = Enumerable.Range(0, 2).Select(i => K.Sprite(SpriteFolder, $"Props/sand_puff_{i:00}", HillsArtScript)).ToArray(); // Both puff pictures
        watcher.puff = M.Renderer("Sand Puff", watcher.puffFrames[0], 19, geometry.transform); // Shows at the feet, just behind the explorer
        watcher.puff.enabled = false; // Hidden until the first step
    } // Ends the sand build

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

    static void BuildLogs(LevelLayout layout, ThemeArt art, Transform parent, LevelController level) // Adds the log roll part and its pictures
    { // Opens the log build
        var roller = level.gameObject.AddComponent<LogRoller>(); // Shares live rolls with the hazard watcher
        string artScript = art.RollerArtScript ?? K.HillsHardArtScript; // The theme's roller script, or the Hills Hard one (D-071)
        string warn = art.UsedRollerWarnFrames; // The theme's warning frames, or the log dust
        string roll = art.UsedRollerFrames; // The theme's roll frames, or the log
        roller.warningFrames = Enumerable.Range(0, 2).Select(i => K.Sprite(SpriteFolder, $"{warn}_{i:00}", artScript)).ToArray(); // Reads the two warning frames
        roller.rollFrames = Enumerable.Range(0, 4).Select(i => K.Sprite(SpriteFolder, $"{roll}_{i:00}", artScript)).ToArray(); // Reads the four roll frames
        roller.views = layout.Logs.Select(rule => BuildLogView(rule, parent)).ToArray(); // Builds one view per route
        roller.rollSound = level.gameObject.AddComponent<AudioSource>(); // Gives nearby logs a shared loop
        roller.rollSound.clip = ThemeSound(art.UsedRollerSound, art.RollerSoundScript ?? HillsHardAudioScript); // Loads the theme's rolling loop
        roller.rollSound.loop = true; // The sound follows the time the roll is near
        roller.rollSound.playOnAwake = false; // Waits for a roll
    } // Ends the log build

    static LogRoller.View BuildLogView(RollingLog rule, Transform parent) // Draws the warning at the spawn and the moving log
    { // Opens the view build
        var root = new GameObject("Rolling Log").transform; // Keeps both pictures together
        root.SetParent(parent, false); // Puts the view under the level geometry
        var warning = M.Renderer("Log Warning", null, 7, root); // Starts hidden above the ground
        warning.transform.position = LevelGeometry.ToWorld(rule.Spawn, 2f); // Sits the four-pixel warning on the ground
        var log = M.Renderer("Log", null, 19, root); // Rolls behind the explorer
        return new LogRoller.View { warning = warning, log = log }; // Gives both pictures to the part
    } // Ends the view build

    static void BuildMudSplash(IceWatcher watcher, Transform parent) // Gives the mud its own sound and pictures
    { // Opens the splash build
        watcher.mudSlideSound = HillsHardSound("sfx_mud_squelch.wav"); // Plays once when a mud slide begins
        watcher.mudSplashFrames = Enumerable.Range(0, 2).Select(i => K.Sprite(SpriteFolder, $"mud_splash_{i:00}", K.HillsHardArtScript)).ToArray(); // Reads both mud splash frames
        watcher.mudSplash = M.Renderer("Mud Splash", watcher.mudSplashFrames[0], 19, parent); // Follows the explorer's feet
        watcher.mudSplash.enabled = false; // Waits for a slide
    } // Ends the splash build

    static AudioClip ThemeSound(string name, string script) // Loads one effect that a theme names, with the script that makes it
    { // Opens the sound lookup
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Finds the supplied recording
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}. Run: python {script}"); // Names the generator from the theme data
        return clip; // Gives the recording to the caller
    } // Ends the sound lookup

    static AudioClip HillsHardSound(string name) // Loads one Hills Hard effect
    { // Opens the sound lookup
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Finds the supplied recording
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}. Run: python Tools/Audio/make_hills_hard_audio.py"); // Names the right generator
        return clip; // Gives the recording to the caller
    } // Ends the sound lookup

    static Sprite HardSprite(string name) => K.Sprite(SpriteFolder, name, HardArtScript); // Loads the existing Hard sprite
    static AudioClip HardSound(string name) // Loads the existing Hard effect
    { // Starts the sound lookup
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name); // Loads the supplied recording
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}. Run: python {HardAudioScript}"); // Names the script that makes it
        return clip; // Gives the sound to the view
    } // Ends the sound lookup
} // Ends the builder
