using System.Collections.Generic; // Gives access to List
using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select
using LandGrand.Play; // Gives access to the level play scripts
using LandGrand.Rules; // Gives access to LevelCatalog, LevelData and LevelLayout
using LandGrand.StartScreen; // Gives access to SpriteFlipbook for the looping props
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite, Rect and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds every level in LevelCatalog as its own scene (Assets/Scenes/MountainsEasy1.unity, MountainsEasy2.unity, ...)
/// and adds them to the build list after the Lesson Screen, in play order (D-055).
/// Run it from the menu: LandGrand > Build Levels. It builds every landform's levels. Draw the art first with: python Tools/Art/make_level_art.py
/// Everything that differs between levels comes from the level data (LevelData and its LevelLayout); this builder holds no level numbers.
/// Level positions are level pixels: x goes right from the left edge, y goes up from the ground line (the top of the grass).
/// Screen (UI) positions are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static partial class LevelBuilder // Makes the level scenes; the theme art lives in LevelThemes.cs
{
    const string SpriteFolder = "Assets/Art/Level/Sprites/"; // Level PNGs
    const string ArtScript = "Tools/Art/make_level_art.py"; // Script that draws them
    const string StartSprites = "Assets/Art/StartScreen/Sprites/"; // The explorer and dust come from the start screen
    const string SelectionSprites = "Assets/Art/Selection/Sprites/"; // The result stars come from the selection art
    const string ItemSprites = "Assets/Art/Items/Sprites/"; // Rock and mineral pictures
    const string BadgeSprites = "Assets/Art/MainMenu/Sprites/Profile/"; // Badges and the "New badge!" ribbon (D-060)
    static readonly Color32 DraftRed = new Color32(168, 63, 36, 255); // Scarf shade from the palette, so the draft note stands out
    static readonly Color32 Clay = new Color32(154, 85, 50, 255); // Clay brown from the palette, for headings
    const string LessonPath = "Assets/Scenes/Lesson.unity"; // The levels go after this scene

    // Mountains background layers (decision D-049): name, share of the camera's movement, own drift in pixels per second.
    // Each level theme picks its folder of layer pictures in LevelThemes.cs (D-063); the Easy themes share Backdrop/Mountains/.
    const string BackdropFolder = "Backdrop/Mountains/"; // Inside the level sprite folder
    const string BackdropScript = "Tools/Art/make_mountains_backdrop.py"; // Script that draws the layers
    static readonly (string name, float follow, float drift)[] Layers = // Far layers move least; every theme's set uses the same six names
    {
        ("L0", 0f, 0f), ("L1", 0.03f, 1.5f), ("L2", 0.1f, 0f), ("L3", 0.2f, 0f), ("L4", 0.35f, 0f), ("L5", 0.55f, 0f), // Sky, clouds, far snowy range, middle rocky range, pine forest, alpine meadow
    };

    [MenuItem("LandGrand/Build Levels")] // Adds the command to Unity's LandGrand menu; it builds every level, not only Mountains
    public static void BuildAll() // Builds and saves one scene per level
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(SpriteFolder + "banner_plate.png") || !File.Exists(SpriteFolder + "arch.png") || !File.Exists(SpriteFolder + BackdropFolder + "L0.png") || !File.Exists(SelectionSprites + "Trail/star_full.png") || !File.Exists(ItemSprites + "garnet.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python {ArtScript}, python {BackdropScript}, python Tools/Art/make_lesson_art.py and python Tools/Art/make_selection_art.py"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        M.ConfigurePixelSprites(SpriteFolder, _ => false); // Crisp pixel art for the level parts; this refreshes the assets, so the font is loaded after it
        M.ConfigurePixelSprites(ItemSprites, _ => false); // Crisp pixel art for the item pictures, also before the font
        M.ConfigurePixelSprites(BadgeSprites, _ => false); // Crisp badges for the "New badge!" notice (D-060)
        var font = M.EnsureFont(); // The pixel font at 2x (decision D-050), loaded after every refresh so the reference stays valid
        if (font == null) return; // EnsureFont already said what is wrong
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        string after = EditorBuildSettings.scenes.Any(scene => scene.path == LandformDescriptionBuilder.ScenePath) ? LandformDescriptionBuilder.ScenePath : LessonPath; // Keeps the summary right after the Lesson Screen
        foreach (var level in LevelCatalog.All) // Every level with data, in play order
        {
            string path = "Assets/Scenes/" + level.SceneName + ".unity"; // For example Assets/Scenes/MountainsEasy2.unity
            BuildLevel(level, path, font); // Make and save its scene
            K.AddSceneAfter(path, after); // In the build list, in play order
            after = path; // The next level goes after this one
        }
        Debug.Log($"Built {LevelCatalog.All.Count} levels. Open StartScreen, press Play, and go Mountains > Easy > a level > Start. Arrows or A/D walk; Space jumps."); // Tell the user it worked
    }

    static void BuildLevel(LevelData level, string path, TMPro.TMP_FontAsset font) // Builds one level scene from its data
    {
        var layout = level.Layout; // Width, theme, boulders, ledges and vents
        var art = ArtFor(layout.Theme); // The ground, ledge, boulder, goal and prop art of this level theme (LevelThemes.cs)
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var ppc = K.Camera(); // The same 480x270 pixel-perfect view as the other screens
        var background = new GameObject("Background").transform; // Holds the moving mountain layers
        background.SetParent(ppc.transform, false); // Rides on the camera
        background.localPosition = new Vector3(0f, 0f, 10f); // In front of the camera's near edge
        for (int i = 0; i < Layers.Length; i++) // Each mountain layer, far to near
        {
            var layerSprite = K.Sprite(SpriteFolder + art.Backdrop, Layers[i].name, art.BackdropScript); // This theme's layer art (D-063)
            var layer = new GameObject(Layers[i].name).AddComponent<BackdropLayer>(); // Slides the layer with the camera
            layer.transform.SetParent(background, false); // Keep it under Background
            layer.follow = Layers[i].follow; // How much it copies the camera
            layer.driftPixelsPerSecond = Layers[i].drift; // Clouds drift on their own
            layer.useLevelClock = art.PauseBackdropDrift; // The theme can freeze its drifting decoration during a pause
            layer.widthPixels = 960f; // Layer art is two screens wide
            layer.pixelsPerUnit = LevelGeometry.PixelsPerUnit; // Same pixel size as the sprites
            M.Renderer("Art", layerSprite, -100 + i * 10, layer.transform); // The picture; farther layers draw first
        }

        var geometry = new GameObject("Level").AddComponent<LevelGeometry>(); // The level's shapes
        geometry.width = layout.Width; // From the level data
        var solids = new List<Rect>(); // Ground pieces, boulders and ledges
        var pits = level.Hazards.Where(h => h.Type == HazardType.Pit).OrderBy(h => h.Place.X).ToList(); // Gaps in the ground, left to right
        var gaps = pits.Select(h => (x: h.Place.X, w: h.Width, wet: h.IsWater, hasFloor: false)) // Reads the pit gaps
            .Concat(layout.Pools.Select(p => (x: p.X, w: p.Width, wet: true, hasFloor: true))) // Adds each safe pool to the ground plan
            .OrderBy(g => g.x).Append((x: layout.Width, w: 0f, wet: false, hasFloor: false)); // Ends the ground at the trail width
        float groundFrom = 0f; // Where the next piece of ground starts
        foreach (var gap in gaps) // Ground up to each gap, then up to the level end
        { // Opens a ground piece
            var piece = new Rect(groundFrom, -64f, gap.x - groundFrom, 64f); // One piece of ground
            solids.Add(piece); // The explorer can stand on it
            DrawGroundTop(piece, art, layout, geometry.transform); // Swaps ice, sand or water into the ground top
            Tiled("Soil", ThemeSprite(art, art.Soil), new Rect(piece.x, -64f, piece.width, 56f), 5, geometry.transform); // Rock or soil below it
            if (gap.w > 0f) DrawGap(gap.x, gap.w, gap.wet, gap.hasFloor, art, solids, geometry.transform); // Draws the inside and adds a safe floor when specified
            groundFrom = gap.x + gap.w; // The next piece starts after this gap
        } // Ends a ground piece
        var spikeAreas = new List<Rect>(); // Danger areas of the spike traps
        foreach (var h in level.Hazards.Where(h => h.Type == HazardType.Spikes)) // Each spike trap
        {
            Tiled("Spikes", "spikes", new Rect(h.Place.X, 0f, h.Width, 10f), 7, geometry.transform); // Draws the spikes
            spikeAreas.Add(new Rect(h.Place.X + 3f, 0f, h.Width - 6f, 8f)); // The part that hurts, a little smaller than the picture
        }

        foreach (var b in layout.Boulders) // Each boulder
        {
            string name = b.Size == BoulderSize.Small ? "boulder_small" : "boulder_big"; // The boulder's base name
            var sprite = ThemeSprite(art, name + art.BoulderEnding); // Its picture from this theme
            float w = sprite.rect.width, h = sprite.rect.height; // Its size in pixels
            var r = M.Renderer(name, sprite, 6, geometry.transform); // Draws the boulder
            r.transform.position = LevelGeometry.ToWorld(b.X + w / 2f, h / 2f); // Sitting on the ground
            solids.Add(new Rect(b.X + 1f, 0f, w - 2f, h - 1f)); // Its solid shape, inside the dark outline
        }
        foreach (var l in layout.Ledges) // Each ledge
        {
            var r = new Rect(l.X, l.Top - 10f, l.Width, 10f); // Its shape: a 10-pixel stone slab
            Tiled("Ledge", ThemeSprite(art, art.Ledge), r, 6, geometry.transform); // Draws the theme ledge
            solids.Add(r); // Solid like a boulder: it blocks the sides and the head, and holds the explorer from above, so nobody walks through it
        }

        BuildGoal(level, art, geometry.transform); // The cave, arch or stone pile, with its flag
        var checkpoints = new SpriteRenderer[level.Checkpoints.Count]; // One flag per safe point
        for (int i = 0; i < checkpoints.Length; i++) // Builds flags in trail order
        { // Opens the flag build
            checkpoints[i] = M.Renderer("Checkpoint Flag " + (i + 1), S("checkpoint_off"), 6, geometry.transform); // Unreached flag
            checkpoints[i].transform.position = LevelGeometry.ToWorld(level.Checkpoints[i].X, level.Checkpoints[i].Y + 14f); // Rests on its safe point
        } // Ends the flag build

        var explorer = new GameObject("Explorer").AddComponent<ExplorerMotor>(); // The explorer
        var body = M.Renderer("Body", Spr("Explorer/explorer_08"), 20, explorer.transform); // The explorer picture, in front of the level
        body.transform.localPosition = new Vector3(0f, 15f / LevelGeometry.PixelsPerUnit, 0f); // Feet (15 pixels below the picture's middle) at the position
        explorer.body = body; // The picture the motor animates
        explorer.geometry = geometry; // What the explorer stands on
        explorer.walkFrames = Enumerable.Range(0, 8).Select(i => Spr($"Explorer/explorer_{i:00}")).ToArray(); // Walk cycle, as on the start screen
        explorer.standFrame = Spr("Explorer/explorer_08"); // Standing
        explorer.riseFrame = Spr("Explorer/explorer_03"); // Long stride for going up
        explorer.fallFrame = Spr("Explorer/explorer_07"); // Feet together for coming down
        explorer.flipFrames = Enumerable.Range(0, 4).Select(i => S($"Flip/flip_{i:00}")).ToArray(); // Somersault for the double jump
        explorer.dustFrames = Enumerable.Range(0, 3).Select(i => Spr($"Dust/dust_{i:00}")).ToArray(); // Dust puff for the double jump

        BuildVents(layout, geometry.transform); // Steam vents with their puffs (Easy 2)
        BuildProps(level, pits, geometry.transform, explorer); // Decoration only: lava cracks, wisps, snow, the eagle

        var spots = new List<PickupSpot>(); // The collectibles placed in the level
        for (int i = 0; i < level.Pickups.Count; i++) // One object per pickup, in the order of LevelData
        {
            var pickup = level.Pickups[i]; // The data of this pickup
            var spot = new GameObject("Pickup " + (i + 1) + " " + pickup.Kind.Id).AddComponent<PickupSpot>(); // The collectible
            spot.transform.SetParent(geometry.transform, false); // Keep it under the level
            spot.index = i; // Its place in the list, so the rules know it
            spot.feet = new Vector2(pickup.Place.X, pickup.Place.Y); // Where it lies: middle of its bottom edge
            spot.art = M.Renderer("Art", Item(pickup.Kind.Picture), 12, spot.transform); // Its picture, in front of the ledges
            spots.Add(spot); // Remember it
        }

        var follow = ppc.gameObject.AddComponent<CameraFollow>(); // The camera follows the explorer
        follow.target = explorer; // Follow the explorer
        follow.geometry = geometry; // Stop at the level's ends

        var music = K.LevelMusic(level); // Uses the landform tune and the ambience chosen for this level
        K.Events(); // Sends taps to the buttons
        var root = K.Canvas("Level UI", ppc, out var group); // UI with the same whole-number zoom as the art

        var controls = M.Rect("Touch Controls", root, K.CanvasCenter, K.CanvasCenter); // Holder for the on-screen buttons
        var input = new GameObject("Controls").AddComponent<LevelInput>(); // Joins touch and keyboard
        input.left = Hold("Left", controls, "btn_left", new Vector2(34f, 240f)); // Bottom-left: walk left
        input.right = Hold("Right", controls, "btn_right", new Vector2(80f, 240f)); // Next to it: walk right
        input.jump = Hold("Jump", controls, "btn_jump", new Vector2(444f, 238f)); // Bottom-right: jump
        explorer.input = input; // The explorer reads these controls

        var controller = new GameObject("Level Controller").AddComponent<LevelController>(); // The level brain
        controller.episode = level.Episode; // Which level this scene plays: the controller reads its data from the catalog
        controller.difficulty = level.Difficulty; // Its difficulty
        controller.number = level.Number; // Its number
        controller.explorer = explorer; // The explorer
        controller.touchControls = controls.gameObject; // Hidden after the goal
        var parts = controller.gameObject; // The parts sit on the same object as the controller, one Inspector box each
        var hazards = controller.hazards = parts.AddComponent<HazardWatcher>(); // Checkpoint, spikes and pits
        var collector = controller.collector = parts.AddComponent<PickupCollector>(); // Items, counter and pop-up
        var timer = controller.timer = parts.AddComponent<LevelTimer>(); // The countdown
        var menus = controller.menus = parts.AddComponent<LevelMenus>(); // Pause and Time's up
        var bar = controller.progressBar = parts.AddComponent<TrailProgressBar>(); // The trail bar
        controller.result = parts.AddComponent<ResultScreen>(); // "Level cleared!" panel
        controller.wind = parts.AddComponent<WindWatcher>(); // Gust push; levels without gusts stay calm
        BuildStrawPiles(layout, geometry.transform, solids, controller); // Builds the solid piles and connects the bounce part
        geometry.solids = solids.ToArray(); // Ground, boulders, ledges and straw piles
        BuildObjects(level, art, geometry, explorer, controller); // Builds the shared objects with this theme
        BuildHardObjects(level, geometry, explorer, controller); // Builds rolling traps and slippery surfaces from the layout
        BuildSoftSand(layout, geometry, controller); // Adds the soft sand part, its step sound and puffs (Hills Easy 3)
        BuildBatsAndCurrents(layout, geometry, controller); // Adds bat and current parts when the trail has those objects
        BuildResult(level, controller.result, root, font); // The Result Screen panel
        collector.pickups = spots.ToArray(); // The collectibles
        controller.chimes = Enumerable.Range(1, 3).Select(i => AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Audio/sfx_mineral_chime_{i}.wav")).ToArray(); // Chimes for items, stars and badges
        hazards.hitSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_hit.wav"); // Trap or pit
        hazards.checkpointSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_checkpoint.wav"); // Flag reached
        if (hazards.hitSound == null || hazards.checkpointSound == null) Debug.LogError("Missing sfx_hit.wav or sfx_checkpoint.wav. Run: python Tools/Audio/make_mountains_audio.py"); // Say what to do
        timer.tickSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_tick.wav"); // Countdown tick in the last 10 seconds (D-058)
        if (timer.tickSound == null) Debug.LogError("Missing sfx_tick.wav. Run: python Tools/Audio/make_mountains_audio.py"); // Say what to do
        if (layout.Vents.Count > 0) // Only levels with vents need the hiss
        {
            controller.steamSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_steam.wav"); // The vent puff
            if (controller.steamSound == null) Debug.LogError("Missing sfx_steam.wav. Run: python Tools/Audio/make_mountains_audio.py"); // Say what to do
        }
        hazards.spikeAreas = spikeAreas.ToArray(); // The spike traps
        hazards.checkpointFlags = checkpoints; // One flag picture per checkpoint
        hazards.checkpointOnSprite = S("checkpoint_on"); // The raised flag
        var flash = M.Rect("Hazard Flash", root, K.CanvasCenter, K.CanvasCenter); // Full-screen red flash when the explorer is sent back
        flash.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        hazards.hazardFlash = flash.gameObject.AddComponent<Image>(); // The flash picture
        hazards.hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0f); // See-through until needed
        hazards.hazardFlash.raycastTarget = false; // Never blocks taps
        BuildCollecting(level, collector, root, font); // The counter at the top left and the first-find pop-up
        BuildTimerAndMenus(level, timer, menus, bar, root, font); // The countdown, the pause button, the pause menu and the Time up panel
        controller.banner = BuildBanner(root, font); // The level start banner, over the play area (D-055)
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group

        K.FixFonts(); // Right pixel font on every text, never TextMeshPro's smooth default font
        Directory.CreateDirectory(Path.GetDirectoryName(path)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, path); // Save the scene
    }

    static void BuildStrawPiles(LevelLayout layout, Transform parent, List<Rect> solids, LevelController controller) // Builds the straw pile art, boxes and bounce sounds
    { // Opens the pile build
        if (layout.StrawPiles.Count == 0) return; // No new part on levels without piles
        var watcher = controller.gameObject.AddComponent<StrawBounceWatcher>(); // One part handles every pile
        watcher.normal = K.Sprite(SpriteFolder, "straw_pile", K.PlainsEasyArtScript); // Shows the resting pile
        watcher.squash = K.Sprite(SpriteFolder, "straw_pile_squash", K.PlainsEasyArtScript); // Shows the pile pressed down
        watcher.bitsFrames = Enumerable.Range(0, 2).Select(i => K.Sprite(SpriteFolder, $"Props/straw_bits_{i:00}", K.PlainsEasyArtScript)).ToArray(); // Loads the two bits poses
        watcher.bounceSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_straw_bounce.wav"); // Loads the bounce sound
        if (watcher.bounceSound == null) Debug.LogError("Missing Assets/Audio/sfx_straw_bounce.wav. Run: python Tools/Audio/make_plains_audio.py"); // Names the sound generator
        watcher.views = new StrawBounceWatcher.View[layout.StrawPiles.Count]; // Matches the layout order
        for (int i = 0; i < layout.StrawPiles.Count; i++) // Builds each pile
        { // Opens one pile
            var pile = layout.StrawPiles[i]; // Takes the left edge from the layout
            float middle = pile.X + StrawPile.Width / 2f; // Finds the center of the pile
            var art = M.Renderer($"Straw Pile {i + 1}", watcher.normal, 6, parent); // Draws the pile at boulder depth
            art.transform.position = LevelGeometry.ToWorld(middle, StrawPile.Height / 2f); // Sits the pile on the ground
            var bits = M.Renderer("Straw Bits", watcher.bitsFrames[0], 19, art.transform); // Draws straw just behind the explorer
            bits.transform.position = LevelGeometry.ToWorld(middle, StrawPile.Height + 3f); // The bits start above the pile's top
            bits.enabled = false; // The bits wait for the first bounce
            watcher.views[i] = new StrawBounceWatcher.View { pile = art, bits = bits }; // Connects the two renderers
            solids.Add(new Rect(pile.X + 1f, 0f, StrawPile.Width - 2f, StrawPile.Height - 1f)); // Sets the box inside the dark outline
        } // Ends one pile
    } // Ends the pile build

    static void BuildVents(LevelLayout layout, Transform parent) // Each steam vent: its rim in the ground, a warning wisp and a puff above it (D-055)
    {
        if (layout.Vents.Count == 0) return; // Most levels have none
        var warnFrames = Enumerable.Range(0, 2).Select(i => S($"Props/steam_warn_{i:00}")).ToArray(); // The small warning wisp
        var puffFrames = Enumerable.Range(0, 4).Select(i => S($"Props/steam_{i:00}")).ToArray(); // The tall puff
        for (int i = 0; i < layout.Vents.Count; i++) // Each vent
        {
            var vent = layout.Vents[i]; // Its data
            float mid = vent.CenterX; // Its middle across
            var view = new GameObject("Steam Vent " + (i + 1)).AddComponent<SteamVentView>(); // Draws its cycle
            view.transform.SetParent(parent, false); // Keep it under the level
            var rim = M.Renderer("Vent", S("vent"), 7, view.transform); // The vent in the ground, in front of the ground top
            rim.transform.position = LevelGeometry.ToWorld(mid, -4f); // Its 8-pixel height ends at the ground line
            view.warn = M.Renderer("Warning", null, 19, view.transform); // The wisp, just behind the explorer
            view.warn.transform.position = LevelGeometry.ToWorld(mid, 8f); // Its 16-pixel height starts at the ground line
            view.steam = M.Renderer("Steam", null, 19, view.transform); // The puff, just behind the explorer
            view.steam.transform.position = LevelGeometry.ToWorld(mid, 32f); // Its 64-pixel height starts at the ground line
            view.warnFrames = warnFrames; // Wisp pictures
            view.puffFrames = puffFrames; // Puff pictures
        }
    }

    static void Loop(GameObject go, Sprite[] frames, float framesPerSecond) // Makes a picture play its frames for ever
    {
        var flip = go.AddComponent<SpriteFlipbook>(); // Plays the frames
        flip.frames = frames; // The pictures
        flip.framesPerSecond = framesPerSecond; // Its speed
        flip.loop = true; // Never stops
    }

    static LevelBanner BuildBanner(Transform root, TMPro.TMP_FontAsset font) // The level start banner: a plate with the level name, hidden above the screen until it slides in (D-055)
    {
        var c = K.CanvasCenter; // Middle of the screen
        var holder = M.Rect("Level Banner", root, c, c); // Holder; the banner script shows and hides it
        var at = new Vector2(240f, 96f); // Where the plate rests: centred, under the top strip of the HUD
        var plate = M.Picture("Plate", holder, S("banner_plate"), at, c); // The 208x32 cream plate
        plate.raycastTarget = false; // Never blocks taps
        var name = M.Label("Name", plate.transform, font, "", at, at, new Vector2(196f, 22f), Clay, TMPro.TextAlignmentOptions.Midline); // The level name in the pixel font at 22 px (D-050)
        var banner = holder.gameObject.AddComponent<LevelBanner>(); // Slides it in and out
        banner.plate = plate.rectTransform; // The plate to move
        banner.label = name; // The name label
        banner.shownY = plate.rectTransform.anchoredPosition.y; // Its resting height
        banner.hiddenY = banner.shownY + 160f; // Above the top of the screen
        plate.rectTransform.anchoredPosition = new Vector2(plate.rectTransform.anchoredPosition.x, banner.hiddenY); // Start hidden, so it never flashes
        holder.gameObject.SetActive(false); // Hidden until the level starts
        return banner; // The controller plays it
    }

    static void BuildCollecting(LevelData level, PickupCollector collector, Transform root, TMPro.TMP_FontAsset font) // Makes the counter and the hidden pop-up
    {
        var c = K.CanvasCenter; // Middle of the screen
        var counterAt = new Vector2(84f, 30f); // Middle of the top-left panel: 6 pixels from the left and top edges (decisions D-049, D-050)
        var counter = M.Panel("Counter", root, counterAt, new Vector2(156f, 46f)); // Cream panel at the top left, x 6 to 162 (6 pixels before the trail bar), wide enough for "Mountains Normal 3"; two 22-pixel lines: the level name, then the count
        collector.levelNameLabel = M.Label("Level Name", counter, font, level.Episode + " " + level.Difficulty + " " + level.Number, new Vector2(14f + 70f, 19f), counterAt, new Vector2(140f, 22f), Clay, TMPro.TextAlignmentOptions.MidlineLeft); // Which level this is, for example "Mountains Easy 2"
        M.Picture("Gem", counter, S("icon_gem"), new Vector2(20f, 41f), counterAt); // Collectible icon, under the name
        collector.counterLabel = M.Label("Count", counter, font, "0/" + level.Pickups.Count, new Vector2(30f + 20f, 41f), counterAt, new Vector2(40f, 22f), K.Ink, TMPro.TextAlignmentOptions.MidlineLeft); // The 0/5 count, right of the gem

        counter.SetAsFirstSibling(); // Draw the counter behind the Result Screen and the pop-up
        var find = M.Rect("Find Item", root, c, c); // Holder of the whole pop-up, hidden at the start
        find.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = find.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color(0f, 0f, 0f, 0.55f); // See-through black
        dim.raycastTarget = true; // Blocks taps on the controls while the pop-up is open
        var panel = M.Panel("Panel", find, c, new Vector2(360f, 176f)); // Cream pop-up panel, x 60 to 420, y 47 to 223, roomy enough for 22-pixel text (decision D-050)
        M.Label("Heading", panel, font, "You found an item!", new Vector2(240f, 64f), c, new Vector2(320f, 22f), Clay, TMPro.TextAlignmentOptions.Midline); // Title
        M.Picture("Slot", panel, LessonSprite("item_slot"), new Vector2(92f, 98f), c); // Frame behind the picture
        collector.findPicture = M.Picture("Picture", panel, null, new Vector2(92f, 98f), c); // The item picture; set when the pop-up opens
        collector.findPicture.rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
        collector.findName = M.ReadingLabel("Name", panel, "", new Vector2(216f, 98f), c, new Vector2(200f, 22f), Clay, TMPro.TextAlignmentOptions.MidlineLeft); // Item name, right of the picture, in the reading font (D-051)
        collector.findDescription = M.ReadingLabel("Description", panel, "", new Vector2(240f, 158f), c, new Vector2(336f, 72f), K.Ink, TMPro.TextAlignmentOptions.TopLeft); // Short description in the reading font (D-051), x 72 to 408, up to three 24-pixel lines from y 122
        collector.findDescription.textWrappingMode = TMPro.TextWrappingModes.Normal; // Let the sentence wrap onto three lines
        var ok = M.Picture("OK", panel, S("btn_ok"), new Vector2(240f, 200f), c); // OK button, bottom of the pop-up
        ok.raycastTarget = true; // Buttons must catch taps
        var button = ok.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        button.targetGraphic = ok; // The picture that changes
        button.spriteState = new SpriteState { pressedSprite = S("btn_ok_pressed") }; // Pushed-in picture
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        collector.findOk = button; // The controller connects it
        collector.findPanel = find.gameObject; // The controller shows and hides it
        collector.itemPictures = ItemCatalog.All.Select(k => Item(k.Picture)).ToArray(); // Pictures of every kind
        collector.sparkleFrames = Enumerable.Range(0, 4).Select(i => Spr($"Sparkle/sparkle_{i:00}")).ToArray(); // Sparkle frames from the start screen
    }

    static void BuildTimerAndMenus(LevelData level, LevelTimer timer, LevelMenus menus, TrailProgressBar bar, Transform root, TMPro.TMP_FontAsset font) // Makes the countdown, the pause button and the two hidden menus
    {
        var c = K.CanvasCenter; // Middle of the screen
        var timerPanel = M.Panel("Timer", root, new Vector2(240f, 18f), new Vector2(56f, 22f)); // Small cream panel at the top centre
        int limit = level.TimeLimitSeconds; // The countdown's start
        timer.label = M.Label("Time", timerPanel, font, (limit / 60) + ":" + (limit % 60).ToString("00"), new Vector2(240f, 18f), new Vector2(240f, 18f), new Vector2(52f, 16f), K.Ink, TMPro.TextAlignmentOptions.Midline); // The countdown
        menus.pauseButton = SpriteButton("Pause", root, "btn_pause", new Vector2(462f, 19f), c); // Pause button, top right, 6 pixels from the right and top edges
        BuildProgress(level, bar, root); // The trail bar under the countdown
        timerPanel.SetAsFirstSibling(); // Draw the countdown behind the pop-up and the result panel
        menus.pauseButton.transform.SetAsFirstSibling(); // Same for the pause button

        // Pause and Time's up use icon buttons with a small name under each, like the Main Menu's Settings, Tutorial and Exit (D-057).
        // Panel y 80 to 190: heading at 100, icons at 136 (y 121 to 151), names at 164.
        menus.pausePanel = MenuPanel("Pause Menu", root, font, "Paused", 110f, out var pauseBox); // Dark backdrop and panel, 240 wide
        menus.resumeButton = LabelledIcon("Resume", pauseBox, "btn_resume_icon", 172f); // Green play icon: back to the game
        menus.pauseRetryButton = LabelledIcon("Retry", pauseBox, "btn_retry_icon", 240f); // Circle arrow: start again from the lesson
        menus.pauseLevelsButton = LabelledIcon("Levels", pauseBox, "btn_levels_icon", 308f); // Grid of squares: leave to the level selection (D-064)

        menus.timeUpPanel = MenuPanel("Time Up", root, font, "Time" + (char)39 + "s up!", 110f, out var timeUpBox); // Dark backdrop and panel, same size
        menus.timeUpRetryButton = LabelledIcon("Retry", timeUpBox, "btn_retry_icon", 204f); // Circle arrow: start again from the lesson
        menus.timeUpLevelsButton = LabelledIcon("Levels", timeUpBox, "btn_levels_icon", 276f); // Grid of squares: leave to the level selection (D-064)
    }

    static Button LabelledIcon(string name, Transform panel, string spriteName, float x) // A 34x30 icon button with its name under it in the reading font (D-051, D-057)
    {
        var at = new Vector2(x, 136f); // Middle of the icon on the screen
        var button = SpriteButton(name, panel, spriteName, at, K.CanvasCenter); // The icon; its picture swaps to the pressed one
        var label = M.ReadingLabel("Label", button.transform, name, new Vector2(x, 164f), at, new Vector2(80f, 22f), K.Ink, TMPro.TextAlignmentOptions.Midline); // Its name, 4 pixels under the icon; 80 wide for the longest name
        label.textWrappingMode = TMPro.TextWrappingModes.NoWrap; // One line
        label.raycastTarget = false; // Taps on the name do not block anything
        return button; // The controller connects it
    }

    static void BuildProgress(LevelData level, TrailProgressBar bar, Transform root) // Makes the trail bar: start on the left, the checkpoint flag, the goal flag on the right, and the explorer marker (decision D-049)
    {
        var c = K.CanvasCenter; // Middle of the screen
        var holder = M.Rect("Trail Progress", root, c, c); // Holder of the bar's parts, filling the screen's middle
        M.Picture("Track", holder, S("progress_track"), new Vector2(240f, 39f), c); // Empty bar from x 168 to 311, rows 36 to 41, just under the countdown
        float left = 169f, width = 142f; // The groove inside the frame: its left edge and width in screen pixels
        var fill = M.Picture("Fill", holder, S("progress_fill"), new Vector2(left, 39f), c); // Walked part of the bar
        fill.rectTransform.pivot = new Vector2(0f, 0.5f); // It grows to the right from its left edge
        fill.rectTransform.anchoredPosition = new Vector2(left - c.x, c.y - 39f); // Left edge at the groove's start, rows 37 to 40
        fill.rectTransform.sizeDelta = new Vector2(0f, 4f); // Nothing walked yet; the controller sets the width
        bar.checkpoints = new UnityEngine.UI.Image[level.Checkpoints.Count]; // One trail flag per safe point
        for (int i = 0; i < level.Checkpoints.Count; i++) // Places the flags in trail order
        { // Opens the trail flag build
            float checkpointX = left + Mathf.Round(level.Checkpoints[i].X / level.Goal.X * width); // Position along the groove
            bar.checkpoints[i] = M.Picture("Checkpoint " + (i + 1), holder, S("progress_checkpoint_off"), new Vector2(checkpointX + 3f, 36f), c); // Grey flag with its pole at the safe point
        } // Ends the trail flag build
        bar.checkpointOn = S("progress_checkpoint_on"); // Green flag once reached
        M.Picture("Goal", holder, S("progress_goal"), new Vector2(left + width + 3f, 36f), c); // Red flag at the right end: the goal
        bar.marker = M.Picture("Explorer", holder, S("progress_marker"), new Vector2(left, 39f), c).rectTransform; // The explorer marker, in front of the flags
        bar.fill = fill.rectTransform; // The controller widens it
        bar.left = left - c.x; // Groove start as a UI offset
        bar.width = width; // Groove width
        holder.SetAsFirstSibling(); // Draw the bar behind the pop-up and the result panel
    }

    static GameObject MenuPanel(string name, Transform root, TMPro.TMP_FontAsset font, string heading, float height, out RectTransform panel) // A dark full-screen backdrop with a cream panel and a heading
    {
        var c = K.CanvasCenter; // Middle of the screen
        var holder = M.Rect(name, root, c, c); // Holder of the whole menu, hidden at the start
        holder.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = holder.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color(0f, 0f, 0f, 0.55f); // See-through black
        dim.raycastTarget = true; // Blocks taps on the controls behind the menu
        panel = M.Panel("Panel", holder, c, new Vector2(240f, height)); // Cream panel, x 120 to 360: room for three icons 72 pixels apart with their names
        M.Label("Heading", panel, font, heading, new Vector2(240f, c.y - height / 2f + 20f), c, new Vector2(200f, 16f), Clay, TMPro.TextAlignmentOptions.Midline); // Title near the top of the panel
        return holder.gameObject; // The controller shows and hides it
    }

    static Button SpriteButton(string name, Transform parent, string spriteName, Vector2 center, Vector2 parentCenter) // A picture button whose picture swaps to its pressed version
    {
        var image = M.Picture(name, parent, S(spriteName), center, parentCenter); // Button picture
        image.raycastTarget = true; // Buttons must catch taps
        var button = image.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        button.targetGraphic = image; // The picture that changes
        button.spriteState = new SpriteState { pressedSprite = S(spriteName + "_pressed") }; // Pushed-in picture
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        return button; // The controller connects it
    }

    static void BuildResult(LevelData level, ResultScreen result, Transform root, TMPro.TMP_FontAsset font) // Makes the hidden Result Screen panel
    {
        var c = K.CanvasCenter; // Middle of the screen
        var panel = M.Panel("Result", root, c, new Vector2(456f, 250f)); // Big cream panel, x 12 to 468, y 10 to 260, laid out for 22-pixel text (decision D-050)
        M.Picture("Heading", panel, S("text_cleared"), new Vector2(240f, 30f), c); // "LEVEL CLEARED!"
        result.stars = Enumerable.Range(0, 3).Select(i => StarPicture("Star " + (i + 1), panel, new Vector2(212f + i * 28f, 58f), c)).ToArray(); // Three stars at 2x size
        result.starFull = Sel("Trail/star_full"); // Earned star
        result.starEmpty = Sel("Trail/star_empty"); // Star not earned
        BuildBadgeNotice(result, panel); // "New badge!" to the right of the stars (D-060)

        int n = level.Pickups.Count; // How many items the level holds
        var items = new Image[n]; // One picture per item
        for (int i = 0; i < n; i++) // Each item slot
        {
            var at = new Vector2(240f + (i - (n - 1) / 2f) * 40f, 90f); // Evenly spaced in a row
            M.Picture("Slot " + (i + 1), panel, LessonSprite("item_slot"), at, c); // Frame behind the item
            items[i] = M.Picture("Item " + (i + 1), panel, null, at, c); // The item picture; set when the result opens
            items[i].rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
        }
        result.items = items; // The controller fills them

        result.goalIcons = new Image[2]; // A tick or cross per goal
        result.goalTexts = new TMPro.TMP_Text[2]; // The goal lines
        for (int i = 0; i < 2; i++) // Both goals, side by side on one row to leave room for the review
        {
            float x = 70f + i * 126f; // Icon place: "Collect 5/5" on the left, "Finish before time runs out" from the middle
            result.goalIcons[i] = M.Picture("Goal Icon " + (i + 1), panel, S("icon_tick"), new Vector2(x, 120f), c); // Tick or cross
            result.goalTexts[i] = M.ReadingLabel("Goal " + (i + 1), panel, "", new Vector2(x + 10f + 115f, 120f), c, new Vector2(230f, 22f), K.Ink, TMPro.TextAlignmentOptions.MidlineLeft); // Goal words in the reading font, starting 10 pixels right of the icon
        }
        result.tickSprite = S("icon_tick"); // Goal met
        result.crossSprite = S("icon_cross"); // Goal not met

        result.reviewLabel = M.ReadingLabel("Review", panel, "", new Vector2(240f, 170f), c, new Vector2(424f, 72f), Clay, TMPro.TextAlignmentOptions.Top); // Review sentences in the reading font (D-051), wrapped onto up to three 24-pixel lines from y 134
        result.reviewLabel.textWrappingMode = TMPro.TextWrappingModes.Normal; // Let the sentences wrap onto lines
        result.reviewDraftNote = M.Label("Draft Note", panel, font, "", new Vector2(240f, 218f), c, new Vector2(300f, 22f), DraftRed, TMPro.TextAlignmentOptions.Midline); // Draft marking, under the review

        // Buttons left to right (D-059): Back to Map, Replay, Next. Each has its icon before the word.
        result.mapButton = SpriteButton("Back to Map", panel, "btn_map", new Vector2(22f + S("btn_map").rect.width / 2f, 243f), c); // Map pin: back to the Landform Map, left edge at x 22
        result.replayButton = SpriteButton("Replay", panel, "btn_replay", new Vector2(238f, 243f), c); // Circle arrow: start again from the lesson, in the middle
        result.nextButton = SpriteButton("Next", panel, "btn_next_level", new Vector2(240f, 243f), c); // Next Level or Next Episode; the controller picks the picture
        var next = (RectTransform)result.nextButton.transform; // Its box
        next.pivot = new Vector2(1f, 0.5f); // Measured from its right edge, so either picture ends at the same place
        next.anchoredPosition = new Vector2(458f - c.x, next.anchoredPosition.y); // Right edge at x 458, 10 pixels inside the panel's right side
        result.nextLevelSprite = S("btn_next_level"); // "Next Level"
        result.nextLevelPressed = S("btn_next_level_pressed"); // Its pushed-in picture
        result.nextEpisodeSprite = S("btn_next_episode"); // "Next Episode"
        result.nextEpisodePressed = S("btn_next_episode_pressed"); // Its pushed-in picture
        result.panel = panel.gameObject; // The controller shows it at the goal
    }

    static void BuildBadgeNotice(ResultScreen result, Transform panel) // The hidden "New badge!" notice: ribbon, medal and name, right of the stars (D-060)
    {
        var c = K.CanvasCenter; // Middle of the screen
        var notice = M.Rect("New Badge", panel, c, c); // Holder the controller shows
        M.Picture("Ribbon", notice, B("badge_new_ribbon"), new Vector2(396f, 48f), c); // Red "NEW BADGE!" ribbon, x 348 to 444, y 38 to 58
        result.badgeNoticePicture = M.Picture("Badge", notice, B("badge_first_steps"), new Vector2(326f, 60f), c); // The medal, x 310 to 342, right of the third star
        result.badgeNoticeName = M.ReadingLabel("Name", notice, "", new Vector2(396f, 70f), c, new Vector2(120f, 22f), Clay, TMPro.TextAlignmentOptions.Midline); // Its name under the ribbon, in the reading font
        result.badgeNoticeName.textWrappingMode = TMPro.TextWrappingModes.NoWrap; // One line
        result.badgePictures = AchievementCatalog.All.Select(a => B("badge_" + a.Id)).ToArray(); // Every earned badge picture
        result.badgeNotice = notice.gameObject; // The controller shows it
        notice.gameObject.SetActive(false); // Hidden until a badge is earned
    }

    static Image StarPicture(string name, Transform parent, Vector2 center, Vector2 parentCenter) // A result star shown at 2x size, which keeps the pixels sharp
    {
        var star = M.Picture(name, parent, Sel("Trail/star_empty"), center, parentCenter); // The star picture
        star.rectTransform.sizeDelta *= 2f; // Whole-number zoom
        return star; // The controller swaps its picture
    }

    static HoldButton Hold(string name, Transform parent, string sprite, Vector2 center) // Makes one on-screen touch button
    {
        var image = M.Picture(name, parent, S(sprite), center, K.CanvasCenter); // Button picture
        image.raycastTarget = true; // Catches touches
        var hold = image.gameObject.AddComponent<HoldButton>(); // Can be held down
        hold.image = image; // The picture it swaps
        hold.normal = S(sprite); // Normal picture
        hold.pressed = S(sprite + "_pressed"); // Held picture
        return hold; // The input reads it
    }

    static void Tiled(string name, string sprite, Rect r, int order, Transform parent) // Draws a repeated tile over a level-pixel rectangle
    {
        Tiled(name, S(sprite), r, order, parent); // Loads the tile before drawing it
    } // Ends the tile lookup

    static void Tiled(string name, Sprite sprite, Rect r, int order, Transform parent) // Draws a loaded tile across the area
    { // Starts drawing
        var sr = M.Renderer(name, sprite, order, parent); // The renderer
        sr.drawMode = SpriteDrawMode.Tiled; // Repeat the tile instead of stretching it
        sr.tileMode = SpriteTileMode.Continuous; // Plain repeating
        sr.size = new Vector2(r.width, r.height) / LevelGeometry.PixelsPerUnit; // Area to fill, in units
        sr.transform.position = LevelGeometry.ToWorld(r.center.x, r.center.y); // Centred on the rectangle
    }

    static Sprite Item(string path) => K.Sprite(ItemSprites, path, "Tools/Art/make_lesson_art.py"); // Loads one item picture
    static Sprite LessonSprite(string path) => K.Sprite("Assets/Art/Lesson/Sprites/", path, "Tools/Art/make_lesson_art.py"); // Loads one Lesson Screen sprite
    static Sprite S(string path) => K.Sprite(SpriteFolder, path, ArtScript); // Loads one level sprite
    static Sprite Spr(string path) => K.Sprite(StartSprites, path, "the start-screen art (already in the project)"); // Loads one start-screen sprite
    static Sprite B(string path) => K.Sprite(BadgeSprites, path, "Tools/Art/make_profile_art.py"); // Loads one badge sprite (D-060)
    static Sprite Sel(string path) => K.Sprite(SelectionSprites, path, "Tools/Art/make_selection_art.py"); // Loads one selection sprite
}
