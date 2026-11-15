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
/// Run it from the menu: LandGrand > Build Mountains Levels. Draw the art first with: python Tools/Art/make_level_art.py
/// Everything that differs between levels comes from the level data (LevelData and its LevelLayout); this builder holds no level numbers.
/// Level positions are level pixels: x goes right from the left edge, y goes up from the ground line (the top of the grass).
/// Screen (UI) positions are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class LevelBuilder // Makes the level scenes
{
    const string SpriteFolder = "Assets/Art/Level/Sprites/"; // Level PNGs
    const string ArtScript = "Tools/Art/make_level_art.py"; // Script that draws them
    const string StartSprites = "Assets/Art/StartScreen/Sprites/"; // The explorer and dust come from the start screen
    const string SelectionSprites = "Assets/Art/Selection/Sprites/"; // The result stars come from the selection art
    const string ItemSprites = "Assets/Art/Items/Sprites/"; // Rock and mineral pictures
    static readonly Color32 DraftRed = new Color32(168, 63, 36, 255); // Scarf shade from the palette, so the draft note stands out
    static readonly Color32 Clay = new Color32(154, 85, 50, 255); // Clay brown from the palette, for headings
    const string LessonPath = "Assets/Scenes/Lesson.unity"; // The levels go after this scene

    // Mountains-only background layers (decision D-049, drawn by Tools/Art/make_mountains_backdrop.py): name, share of the camera's movement, own drift in pixels per second.
    // Every Mountains level shares them; the level theme changes the ground, props and goal in front (D-055).
    const string BackdropFolder = "Backdrop/Mountains/"; // Inside the level sprite folder
    const string BackdropScript = "Tools/Art/make_mountains_backdrop.py"; // Script that draws the layers
    static readonly (string name, float follow, float drift)[] Layers = // Far layers move least
    {
        ("L0", 0f, 0f), ("L1", 0.03f, 1.5f), ("L2", 0.1f, 0f), ("L3", 0.2f, 0f), ("L4", 0.35f, 0f), ("L5", 0.55f, 0f), // Sky, clouds, far snowy range, middle rocky range, pine forest, alpine meadow
    };

    [MenuItem("LandGrand/Build Mountains Levels")] // Adds the command to Unity's LandGrand menu
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
        var font = M.EnsureFont(); // The pixel font at 2x (decision D-050), loaded after every refresh so the reference stays valid
        if (font == null) return; // EnsureFont already said what is wrong
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        string after = LessonPath; // The first level goes right after the Lesson Screen in the build list
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
        var theme = layout.Theme; // Meadow, Volcanic or FoldedRock
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var ppc = K.Camera(); // The same 480x270 pixel-perfect view as the other screens
        var background = new GameObject("Background").transform; // Holds the moving mountain layers
        background.SetParent(ppc.transform, false); // Rides on the camera
        background.localPosition = new Vector3(0f, 0f, 10f); // In front of the camera's near edge
        for (int i = 0; i < Layers.Length; i++) // Each mountain layer, far to near
        {
            var layerSprite = K.Sprite(SpriteFolder + BackdropFolder, Layers[i].name, BackdropScript); // The Mountains layer art
            var layer = new GameObject(Layers[i].name).AddComponent<BackdropLayer>(); // Slides the layer with the camera
            layer.transform.SetParent(background, false); // Keep it under Background
            layer.follow = Layers[i].follow; // How much it copies the camera
            layer.driftPixelsPerSecond = Layers[i].drift; // Clouds drift on their own
            layer.widthPixels = 960f; // Layer art is two screens wide
            layer.pixelsPerUnit = LevelGeometry.PixelsPerUnit; // Same pixel size as the sprites
            M.Renderer("Art", layerSprite, -100 + i * 10, layer.transform); // The picture; farther layers draw first
        }

        var geometry = new GameObject("Level").AddComponent<LevelGeometry>(); // The level's shapes
        geometry.width = layout.Width; // From the level data
        var solids = new List<Rect>(); // Ground pieces, boulders and ledges
        var pits = level.Hazards.Where(h => h.Type == HazardType.Pit).OrderBy(h => h.Place.X).ToList(); // Gaps in the ground, left to right
        float groundFrom = 0f; // Where the next piece of ground starts
        foreach (var pit in pits.Select(h => (x: h.Place.X, w: h.Width)).Append((x: layout.Width, w: 0f))) // Ground up to each pit, then up to the level end
        {
            var piece = new Rect(groundFrom, -64f, pit.x - groundFrom, 64f); // One piece of ground
            solids.Add(piece); // The explorer can stand on it
            Tiled("Ground Top", Themed("grass_top", "volcanic_top", "fold_top", theme), new Rect(piece.x, -8f, piece.width, 8f), 5, geometry.transform); // The ground's top edge
            Tiled("Soil", Themed("soil", "volcanic_soil", "fold_soil", theme), new Rect(piece.x, -64f, piece.width, 56f), 5, geometry.transform); // Rock or soil below it
            if (pit.w > 0f) Tiled("Pit", "pit_fill", new Rect(pit.x, -64f, pit.w, 64f), 3, geometry.transform); // Dark inside of the pit
            groundFrom = pit.x + pit.w; // The next piece starts after the pit
        }
        var spikeAreas = new List<Rect>(); // Danger areas of the spike traps
        foreach (var h in level.Hazards.Where(h => h.Type == HazardType.Spikes)) // Each spike trap
        {
            Tiled("Spikes", "spikes", new Rect(h.Place.X, 0f, h.Width, 10f), 7, geometry.transform); // Draws the spikes
            spikeAreas.Add(new Rect(h.Place.X + 3f, 0f, h.Width - 6f, 8f)); // The part that hurts, a little smaller than the picture
        }

        foreach (var b in layout.Boulders) // Each boulder
        {
            string name = b.Size == BoulderSize.Small ? "boulder_small" : "boulder_big"; // The meadow picture's name
            var sprite = S(Themed(name, name + "_volcanic", name + "_fold", theme)); // Its picture for this theme
            float w = sprite.rect.width, h = sprite.rect.height; // Its size in pixels
            var r = M.Renderer(name, sprite, 6, geometry.transform); // Draws the boulder
            r.transform.position = LevelGeometry.ToWorld(b.X + w / 2f, h / 2f); // Sitting on the ground
            solids.Add(new Rect(b.X + 1f, 0f, w - 2f, h - 1f)); // Its solid shape, inside the dark outline
        }
        foreach (var l in layout.Ledges) // Each ledge
        {
            var r = new Rect(l.X, l.Top - 10f, l.Width, 10f); // Its shape: a 10-pixel stone slab
            Tiled("Ledge", Themed("ledge", "ledge_volcanic", "ledge_fold", theme), r, 6, geometry.transform); // Draws the ledge
            solids.Add(r); // Solid like a boulder: it blocks the sides and the head, and holds the explorer from above, so nobody walks through it
        }
        geometry.solids = solids.ToArray(); // Ground, boulders and ledges

        BuildGoal(level, theme, geometry.transform); // The cave, arch or stone pile, with its flag
        var checkpoint = M.Renderer("Checkpoint Flag", S("checkpoint_off"), 6, geometry.transform); // The mid-level flag, not reached yet
        checkpoint.transform.position = LevelGeometry.ToWorld(level.Checkpoint.X, 14f); // Its 28-pixel height sits on the ground

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
        BuildProps(level, layout, pits, geometry.transform, explorer); // Decoration only: lava cracks, wisps, snow, the eagle

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

        var music = K.LevelMusic(); // The Mountains music and the Easy breeze (ticket 12)
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
        BuildResult(level, controller, root, font); // The Result Screen panel
        controller.pickups = spots.ToArray(); // The collectibles
        controller.hitSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_hit.wav"); // Trap or pit
        controller.checkpointSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_checkpoint.wav"); // Flag reached
        if (controller.hitSound == null || controller.checkpointSound == null) Debug.LogError("Missing sfx_hit.wav or sfx_checkpoint.wav. Run: python Tools/Audio/make_mountains_audio.py"); // Say what to do
        if (layout.Vents.Count > 0) // Only levels with vents need the hiss
        {
            controller.steamSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/sfx_steam.wav"); // The vent puff
            if (controller.steamSound == null) Debug.LogError("Missing sfx_steam.wav. Run: python Tools/Audio/make_mountains_audio.py"); // Say what to do
        }
        controller.spikeAreas = spikeAreas.ToArray(); // The spike traps
        controller.checkpointFlag = checkpoint; // The flag picture
        controller.checkpointOnSprite = S("checkpoint_on"); // The raised flag
        var flash = M.Rect("Hazard Flash", root, K.CanvasCenter, K.CanvasCenter); // Full-screen red flash when the explorer is sent back
        flash.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        controller.hazardFlash = flash.gameObject.AddComponent<Image>(); // The flash picture
        controller.hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0f); // See-through until needed
        controller.hazardFlash.raycastTarget = false; // Never blocks taps
        BuildCollecting(level, controller, root, font); // The counter at the top left and the first-find pop-up
        BuildTimerAndMenus(level, controller, root, font); // The countdown, the pause button, the pause menu and the Time up panel
        controller.banner = BuildBanner(root, font); // The level start banner, over the play area (D-055)
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group

        K.FixFonts(); // Right pixel font on every text, never TextMeshPro's smooth default font
        Directory.CreateDirectory(Path.GetDirectoryName(path)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, path); // Save the scene
    }

    /// <summary>Picks the sprite name for a level theme: the meadow name, or the Volcanic or Folded rock version.</summary>
    static string Themed(string meadow, string volcanic, string folded, LevelTheme theme) => theme == LevelTheme.Volcanic ? volcanic : theme == LevelTheme.FoldedRock ? folded : meadow; // One name per theme

    static void BuildGoal(LevelData level, LevelTheme theme, Transform parent) // The end of the level: a cave, a lava-rock arch or a summit stone pile, with the goal flag
    {
        float goalX = level.Goal.X; // The goal from the level data
        if (theme == LevelTheme.Volcanic) // Volcano Trail
        {
            var arch = M.Renderer("Arch", S("arch"), 4, parent); // Lava-rock arch the explorer walks under
            arch.transform.position = LevelGeometry.ToWorld(goalX + 12f, 28f); // Its 56-pixel height sits on the ground; its open middle starts at the goal
        }
        else if (theme == LevelTheme.FoldedRock) // Folded Peaks
        {
            var cairn = M.Renderer("Cairn", S("cairn"), 4, parent); // Summit stone pile
            cairn.transform.position = LevelGeometry.ToWorld(goalX + 24f, 22f); // Its 44-pixel height sits on the ground, just past the goal
        }
        else // The meadow trail
        {
            var cave = M.Renderer("Cave", S("cave"), 4, parent); // Cave entrance at the end
            cave.transform.position = LevelGeometry.ToWorld(goalX + 20f, 28f); // Its 56-pixel height sits on the ground; its opening is at the goal
        }
        var goalFlag = M.Renderer("Goal Flag", S("flag"), 6, parent); // Flag before the goal
        goalFlag.transform.position = LevelGeometry.ToWorld(goalX - 28f, 14f); // Just before the goal; its 28-pixel height sits on the ground
    }

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

    static void BuildProps(LevelData level, LevelLayout layout, List<Hazard> pits, Transform parent, ExplorerMotor explorer) // Living props: decoration only, no effect on play (D-055)
    {
        bool OverPit(float x, float halfWidth) => pits.Any(p => x + halfWidth > p.Place.X && x - halfWidth < p.Place.X + p.Width); // Keeps props off the gaps
        if (layout.Theme == LevelTheme.Volcanic) // Volcano Trail: glowing cracks in the rock and faint fumaroles behind the trail
        {
            var crackFrames = Enumerable.Range(0, 4).Select(i => S($"Props/lava_crack_{i:00}")).ToArray(); // Glow cycle
            var wispFrames = Enumerable.Range(0, 4).Select(i => S($"Props/wisp_{i:00}")).ToArray(); // Faint wisp cycle
            int n = 0; // Counts the props, so neighbours glow out of step
            for (float x = 180f; x < layout.Width - 120f; x += 340f) // A crack about every 340 pixels
            {
                if (OverPit(x, 12f)) continue; // Never over a pit
                var crack = M.Renderer("Lava Crack", crackFrames[0], 6, parent); // In front of the rock
                crack.transform.position = LevelGeometry.ToWorld(x, -22f - (n % 3) * 9f); // At different depths in the rock face
                Loop(crack.gameObject, crackFrames, 3f + (n % 3) * 0.7f); // Slow glow, each a little different
                n++; // Next prop
            }
            for (float x = 420f; x < layout.Width - 200f; x += 760f) // A wisp about every 760 pixels
            {
                if (OverPit(x, 8f) || layout.Vents.Any(v => Mathf.Abs(v.CenterX - x) < 60f)) continue; // Not over a pit, and well away from the real vents
                var wisp = M.Renderer("Fumarole Wisp", wispFrames[0], 4, parent); // Behind boulders and the explorer
                wisp.transform.position = LevelGeometry.ToWorld(x, 12f); // Its 24-pixel height starts at the ground line
                Loop(wisp.gameObject, wispFrames, 5f + (n % 2)); // Gentle rising
                n++; // Next prop
            }
        }
        else if (layout.Theme == LevelTheme.FoldedRock) // Folded Peaks: snow near the summit and an eagle that flies off
        {
            var snow = new GameObject("Summit Snow").AddComponent<SummitSnow>(); // Drifting flakes
            snow.transform.SetParent(parent, false); // Keep it under the level
            snow.flakeSprites = new[] { S("Props/snow_00"), S("Props/snow_01") }; // Small and big flakes
            snow.fromX = layout.Width - 1500f; // The last stretch before the summit
            snow.toX = layout.Width; // Up to the end
            var bigBoulders = layout.Boulders.Where(b => b.Size == BoulderSize.Big && b.X < level.Goal.X).OrderByDescending(b => b.X).ToList(); // Big boulders before the summit, nearest the summit first
            if (bigBoulders.Count > 0) // A boulder to sit on
            {
                var perch = bigBoulders[0]; // The last big boulder before the summit
                var eagle = new GameObject("Eagle").AddComponent<EagleFlyOff>(); // The startled eagle
                eagle.transform.SetParent(parent, false); // Keep it under the level
                eagle.body = M.Renderer("Art", S("Props/eagle_00"), 8, eagle.transform); // Its picture, in front of the boulder
                eagle.transform.position = LevelGeometry.ToWorld(perch.X + 24f, 36f + 8f); // Standing on top of the 36-pixel boulder
                eagle.perched = S("Props/eagle_00"); // Sitting
                eagle.flapFrames = Enumerable.Range(1, 3).Select(i => S($"Props/eagle_{i:00}")).ToArray(); // Flying
                eagle.explorer = explorer; // Who startles it
            }
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

    static void BuildCollecting(LevelData level, LevelController controller, Transform root, TMPro.TMP_FontAsset font) // Makes the counter and the hidden pop-up
    {
        var c = K.CanvasCenter; // Middle of the screen
        var counterAt = new Vector2(74f, 30f); // Middle of the top-left panel: 6 pixels from the left and top edges (decisions D-049, D-050)
        var counter = M.Panel("Counter", root, counterAt, new Vector2(136f, 46f)); // Cream panel at the top left, x 6 to 142, with two 22-pixel lines: the level name, then the count
        controller.levelNameLabel = M.Label("Level Name", counter, font, level.Episode + " " + level.Difficulty + " " + level.Number, new Vector2(14f + 60f, 19f), counterAt, new Vector2(120f, 22f), Clay, TMPro.TextAlignmentOptions.MidlineLeft); // Which level this is, for example "Mountains Easy 2"
        M.Picture("Gem", counter, S("icon_gem"), new Vector2(20f, 41f), counterAt); // Collectible icon, under the name
        controller.counterLabel = M.Label("Count", counter, font, "0/" + level.Pickups.Count, new Vector2(30f + 20f, 41f), counterAt, new Vector2(40f, 22f), K.Ink, TMPro.TextAlignmentOptions.MidlineLeft); // The 0/5 count, right of the gem

        counter.SetAsFirstSibling(); // Draw the counter behind the Result Screen and the pop-up
        var find = M.Rect("Find Item", root, c, c); // Holder of the whole pop-up, hidden at the start
        find.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = find.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color(0f, 0f, 0f, 0.55f); // See-through black
        dim.raycastTarget = true; // Blocks taps on the controls while the pop-up is open
        var panel = M.Panel("Panel", find, c, new Vector2(360f, 176f)); // Cream pop-up panel, x 60 to 420, y 47 to 223, roomy enough for 22-pixel text (decision D-050)
        M.Label("Heading", panel, font, "You found an item!", new Vector2(240f, 64f), c, new Vector2(320f, 22f), Clay, TMPro.TextAlignmentOptions.Midline); // Title
        M.Picture("Slot", panel, LessonSprite("item_slot"), new Vector2(92f, 98f), c); // Frame behind the picture
        controller.findPicture = M.Picture("Picture", panel, null, new Vector2(92f, 98f), c); // The item picture; set when the pop-up opens
        controller.findPicture.rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
        controller.findName = M.ReadingLabel("Name", panel, "", new Vector2(216f, 98f), c, new Vector2(200f, 22f), Clay, TMPro.TextAlignmentOptions.MidlineLeft); // Item name, right of the picture, in the reading font (D-051)
        controller.findDescription = M.ReadingLabel("Description", panel, "", new Vector2(240f, 158f), c, new Vector2(336f, 72f), K.Ink, TMPro.TextAlignmentOptions.TopLeft); // Short description in the reading font (D-051), x 72 to 408, up to three 24-pixel lines from y 122
        controller.findDescription.textWrappingMode = TMPro.TextWrappingModes.Normal; // Let the sentence wrap onto three lines
        var ok = M.Picture("OK", panel, S("btn_ok"), new Vector2(240f, 200f), c); // OK button, bottom of the pop-up
        ok.raycastTarget = true; // Buttons must catch taps
        var button = ok.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        button.targetGraphic = ok; // The picture that changes
        button.spriteState = new SpriteState { pressedSprite = S("btn_ok_pressed") }; // Pushed-in picture
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        controller.findOk = button; // The controller connects it
        controller.findPanel = find.gameObject; // The controller shows and hides it
        controller.itemPictures = ItemCatalog.All.Select(k => Item(k.Picture)).ToArray(); // Pictures of every kind
        controller.sparkleFrames = Enumerable.Range(0, 4).Select(i => Spr($"Sparkle/sparkle_{i:00}")).ToArray(); // Sparkle frames from the start screen
        controller.chimes = Enumerable.Range(1, 3).Select(i => AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Audio/sfx_mineral_chime_{i}.wav")).ToArray(); // Pickup chimes
    }

    static void BuildTimerAndMenus(LevelData level, LevelController controller, Transform root, TMPro.TMP_FontAsset font) // Makes the countdown, the pause button and the two hidden menus
    {
        var c = K.CanvasCenter; // Middle of the screen
        var timer = M.Panel("Timer", root, new Vector2(240f, 18f), new Vector2(56f, 22f)); // Small cream panel at the top centre
        int limit = level.TimeLimitSeconds; // The countdown's start
        controller.timerLabel = M.Label("Time", timer, font, (limit / 60) + ":" + (limit % 60).ToString("00"), new Vector2(240f, 18f), new Vector2(240f, 18f), new Vector2(52f, 16f), K.Ink, TMPro.TextAlignmentOptions.Midline); // The countdown
        controller.pauseButton = SpriteButton("Pause", root, "btn_pause", new Vector2(462f, 19f), c); // Pause button, top right, 6 pixels from the right and top edges
        BuildProgress(level, controller, root); // The trail bar under the countdown
        timer.SetAsFirstSibling(); // Draw the countdown behind the pop-up and the result panel
        controller.pauseButton.transform.SetAsFirstSibling(); // Same for the pause button

        controller.pausePanel = MenuPanel("Pause Menu", root, font, "Paused", 140f, out var pauseBox); // Dark backdrop and panel
        controller.resumeButton = SpriteButton("Resume", pauseBox, "btn_resume", new Vector2(240f, 112f), c); // Back to the game
        controller.pauseRetryButton = SpriteButton("Retry", pauseBox, "btn_retry", new Vector2(240f, 142f), c); // Start again from the lesson
        controller.pauseMenuButton = SpriteButton("Main Menu", pauseBox, "btn_menu", new Vector2(240f, 172f), c); // Leave to the Main Menu

        controller.timeUpPanel = MenuPanel("Time Up", root, font, "Time" + (char)39 + "s up!", 110f, out var timeUpBox); // Dark backdrop and panel
        controller.timeUpRetryButton = SpriteButton("Retry", timeUpBox, "btn_retry", new Vector2(240f, 128f), c); // Start again from the lesson
        controller.timeUpMenuButton = SpriteButton("Main Menu", timeUpBox, "btn_menu", new Vector2(240f, 158f), c); // Leave to the Main Menu
    }

    static void BuildProgress(LevelData level, LevelController controller, Transform root) // Makes the trail bar: start on the left, the checkpoint flag, the goal flag on the right, and the explorer marker (decision D-049)
    {
        var c = K.CanvasCenter; // Middle of the screen
        var bar = M.Rect("Trail Progress", root, c, c); // Holder of the bar's parts, filling the screen's middle
        M.Picture("Track", bar, S("progress_track"), new Vector2(240f, 39f), c); // Empty bar from x 168 to 311, rows 36 to 41, just under the countdown
        float left = 169f, width = 142f; // The groove inside the frame: its left edge and width in screen pixels
        var fill = M.Picture("Fill", bar, S("progress_fill"), new Vector2(left, 39f), c); // Walked part of the bar
        fill.rectTransform.pivot = new Vector2(0f, 0.5f); // It grows to the right from its left edge
        fill.rectTransform.anchoredPosition = new Vector2(left - c.x, c.y - 39f); // Left edge at the groove's start, rows 37 to 40
        fill.rectTransform.sizeDelta = new Vector2(0f, 4f); // Nothing walked yet; the controller sets the width
        float checkpointX = left + Mathf.Round(level.Checkpoint.X / level.Goal.X * width); // Checkpoint place on the bar
        controller.progressCheckpoint = M.Picture("Checkpoint", bar, S("progress_checkpoint_off"), new Vector2(checkpointX + 3f, 36f), c); // Grey flag; its pole (3 pixels left of the picture's middle) stands at the checkpoint place
        controller.progressCheckpointOn = S("progress_checkpoint_on"); // Green flag once reached
        M.Picture("Goal", bar, S("progress_goal"), new Vector2(left + width + 3f, 36f), c); // Red flag at the right end: the goal
        controller.progressMarker = M.Picture("Explorer", bar, S("progress_marker"), new Vector2(left, 39f), c).rectTransform; // The explorer marker, in front of the flags
        controller.progressFill = fill.rectTransform; // The controller widens it
        controller.progressLeft = left - c.x; // Groove start as a UI offset
        controller.progressWidth = width; // Groove width
        bar.SetAsFirstSibling(); // Draw the bar behind the pop-up and the result panel
    }

    static GameObject MenuPanel(string name, Transform root, TMPro.TMP_FontAsset font, string heading, float height, out RectTransform panel) // A dark full-screen backdrop with a cream panel and a heading
    {
        var c = K.CanvasCenter; // Middle of the screen
        var holder = M.Rect(name, root, c, c); // Holder of the whole menu, hidden at the start
        holder.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = holder.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color(0f, 0f, 0f, 0.55f); // See-through black
        dim.raycastTarget = true; // Blocks taps on the controls behind the menu
        panel = M.Panel("Panel", holder, c, new Vector2(220f, height)); // Cream panel
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

    static void BuildResult(LevelData level, LevelController controller, Transform root, TMPro.TMP_FontAsset font) // Makes the hidden Result Screen panel
    {
        var c = K.CanvasCenter; // Middle of the screen
        var panel = M.Panel("Result", root, c, new Vector2(456f, 250f)); // Big cream panel, x 12 to 468, y 10 to 260, laid out for 22-pixel text (decision D-050)
        M.Picture("Heading", panel, S("text_cleared"), new Vector2(240f, 30f), c); // "LEVEL CLEARED!"
        controller.resultStars = Enumerable.Range(0, 3).Select(i => StarPicture("Star " + (i + 1), panel, new Vector2(212f + i * 28f, 58f), c)).ToArray(); // Three stars at 2x size
        controller.starFull = Sel("Trail/star_full"); // Earned star
        controller.starEmpty = Sel("Trail/star_empty"); // Star not earned

        int n = level.Pickups.Count; // How many items the level holds
        var items = new Image[n]; // One picture per item
        for (int i = 0; i < n; i++) // Each item slot
        {
            var at = new Vector2(240f + (i - (n - 1) / 2f) * 40f, 90f); // Evenly spaced in a row
            M.Picture("Slot " + (i + 1), panel, LessonSprite("item_slot"), at, c); // Frame behind the item
            items[i] = M.Picture("Item " + (i + 1), panel, null, at, c); // The item picture; set when the result opens
            items[i].rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
        }
        controller.resultItems = items; // The controller fills them

        controller.goalIcons = new Image[2]; // A tick or cross per goal
        controller.goalTexts = new TMPro.TMP_Text[2]; // The goal lines
        for (int i = 0; i < 2; i++) // Both goals, side by side on one row to leave room for the review
        {
            float x = 70f + i * 126f; // Icon place: "Collect 5/5" on the left, "Finish before time runs out" from the middle
            controller.goalIcons[i] = M.Picture("Goal Icon " + (i + 1), panel, S("icon_tick"), new Vector2(x, 120f), c); // Tick or cross
            controller.goalTexts[i] = M.ReadingLabel("Goal " + (i + 1), panel, "", new Vector2(x + 10f + 115f, 120f), c, new Vector2(230f, 22f), K.Ink, TMPro.TextAlignmentOptions.MidlineLeft); // Goal words in the reading font, starting 10 pixels right of the icon
        }
        controller.tickSprite = S("icon_tick"); // Goal met
        controller.crossSprite = S("icon_cross"); // Goal not met

        controller.reviewLabel = M.ReadingLabel("Review", panel, "", new Vector2(240f, 170f), c, new Vector2(424f, 72f), Clay, TMPro.TextAlignmentOptions.Top); // Review sentences in the reading font (D-051), wrapped onto up to three 24-pixel lines from y 134
        controller.reviewLabel.textWrappingMode = TMPro.TextWrappingModes.Normal; // Let the sentences wrap onto lines
        controller.reviewDraftNote = M.Label("Draft Note", panel, font, "", new Vector2(240f, 218f), c, new Vector2(300f, 22f), DraftRed, TMPro.TextAlignmentOptions.Midline); // Draft marking, under the review

        controller.replayButton = SpriteButton("Replay", panel, "btn_replay", new Vector2(100f, 243f), c); // Start again from the lesson
        controller.continueButton = SpriteButton("Continue", panel, "btn_continue", new Vector2(240f, 243f), c); // Next level
        controller.mapButton = SpriteButton("Back to Map", panel, "btn_map", new Vector2(382f, 243f), c); // Back to the Landform Map
        controller.resultPanel = panel.gameObject; // The controller shows it at the goal
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
        var sr = M.Renderer(name, S(sprite), order, parent); // The renderer
        sr.drawMode = SpriteDrawMode.Tiled; // Repeat the tile instead of stretching it
        sr.tileMode = SpriteTileMode.Continuous; // Plain repeating
        sr.size = new Vector2(r.width, r.height) / LevelGeometry.PixelsPerUnit; // Area to fill, in units
        sr.transform.position = LevelGeometry.ToWorld(r.center.x, r.center.y); // Centred on the rectangle
    }

    static Sprite Item(string path) => K.Sprite(ItemSprites, path, "Tools/Art/make_lesson_art.py"); // Loads one item picture
    static Sprite LessonSprite(string path) => K.Sprite("Assets/Art/Lesson/Sprites/", path, "Tools/Art/make_lesson_art.py"); // Loads one Lesson Screen sprite
    static Sprite S(string path) => K.Sprite(SpriteFolder, path, ArtScript); // Loads one level sprite
    static Sprite Spr(string path) => K.Sprite(StartSprites, path, "the start-screen art (already in the project)"); // Loads one start-screen sprite
    static Sprite Sel(string path) => K.Sprite(SelectionSprites, path, "Tools/Art/make_selection_art.py"); // Loads one selection sprite
}
