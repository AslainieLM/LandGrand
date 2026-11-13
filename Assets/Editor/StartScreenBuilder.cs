using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select and Where
using LandGrand.StartScreen; // Gives access to the start-screen scripts
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to Unity basics
using UnityEngine.Rendering.Universal; // Gives access to the Pixel Perfect Camera

/// <summary>
/// Builds Assets/Scenes/StartScreen.unity from the start-screen art.
/// Run it from the menu: LandGrand > Build Start Screen Scene. Running it again rebuilds the scene.
/// </summary>
public static class StartScreenBuilder
{
    const string ArtFolder = "Assets/Art/StartScreen/"; // Start-screen art folder
    const string SpriteFolder = "Assets/Art/StartScreen/Sprites/"; // Start-screen PNG folder
    const string AudioFolder = "Assets/Audio/"; // Music and sound folder
    const string ScenePath = "Assets/Scenes/StartScreen.unity"; // Where the scene is saved
    const float PPU = 32f; // Sprite pixels per Unity unit

    // Background layers: PNG name in Sprites/Background, scroll speed in canvas pixels per second.
    static readonly (string prefix, float speed)[] Layers = // One entry per background layer
    {
        ("L0", 0f), ("L1", 1.5f), ("L2", 1f), ("L3", 2.5f), // Sky (still), clouds, far and near mountains
        ("L4", 4.5f), ("L5", 8f), ("L6", 14f), ("L7", 20f), // Plateaus, hills, plains, ground (fastest)
    };

    [MenuItem("LandGrand/Build Start Screen Scene")] // Adds the command to the LandGrand menu
    public static void Build() // Builds and saves the start screen
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene

        ConfigureImporters(); // Make every start-screen PNG crisp pixel art
        SetLandscapeOnly(); // Landscape only (D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene

        // Camera: 480x270 pixel-perfect view.
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" }; // Camera object, tagged as the main camera
        var cam = camGo.AddComponent<Camera>(); // The camera itself
        cam.orthographic = true; // Flat 2D view
        cam.orthographicSize = 270f / 2f / PPU; // Shows exactly 270 pixels from top to bottom
        cam.transform.position = new Vector3(0f, 0f, -10f); // Stand back so it can see the sprites
        cam.clearFlags = CameraClearFlags.SolidColor; // Fill empty space with one colour
        cam.backgroundColor = new Color32(191, 232, 247, 255); // Light sky colour
        var ppc = camGo.AddComponent<PixelPerfectCamera>(); // Keeps pixels square and sharp
        ppc.assetsPPU = (int)PPU; // Sprites use 32 pixels per unit
        ppc.refResolutionX = 480; // Screen is 480 pixels wide
        ppc.refResolutionY = 270; // Screen is 270 pixels tall
        ppc.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture; // Draw small, then scale up evenly
        ppc.cropFrame = PixelPerfectCamera.CropFrame.None; // Fill the whole phone screen
        camGo.AddComponent<AudioListener>(); // Lets the player hear sounds

        // Background layers.
        var bgSprites = Sprites("Background"); // Layers L0 to L7
        var background = new GameObject("Background").transform; // Parent for all layers
        ParallaxLayer ground = null; // The ground layer, found in the loop
        for (int i = 0; i < Layers.Length; i++) // Make each layer
        {
            var sprite = bgSprites.FirstOrDefault(s => s.name.StartsWith(Layers[i].prefix)); // Find this layer's PNG
            if (sprite == null) { Debug.LogError($"No sprite for layer {Layers[i].prefix} in {SpriteFolder}Background"); continue; } // Skip a missing layer and say so
            var layer = new GameObject(sprite.name).AddComponent<ParallaxLayer>(); // Object that scrolls the layer
            layer.transform.SetParent(background, false); // Keep it under Background
            layer.speedPixelsPerSecond = Layers[i].speed; // How fast this layer scrolls
            layer.widthPixels = 960f; // Layer art is two screens wide
            layer.pixelsPerUnit = PPU; // Same pixel size as the sprites
            layer.followsWalk = Layers[i].prefix != "L1";   // clouds keep drifting when the explorer stops
            var art = Renderer("Art", sprite, i * 10, layer.transform); // The layer picture; farther layers draw first
            art.transform.localPosition = Vector3.zero; // Centred on its parent
            if (Layers[i].prefix == "L7") ground = layer; // Remember the ground layer
        }
        var fill = new GameObject("Below-ground fill").AddComponent<ScreenFill>(); // Fills space under the ground on tall screens
        fill.topY = -135f / PPU; // Starts at the bottom edge of the art

        // Effect templates (disabled; scripts copy them).
        var templates = new GameObject("Effect templates").transform; // Parent for the hidden effects
        var sparkle = Effect("Sparkle", Sprites("Sparkle"), 85, 12f, templates); // Hidden sparkle
        var dust = Effect("Dust", Sprites("Dust"), 81, 10f, templates); // Hidden dust puff
        var bird = Renderer("Bird", Sprites("Birds").FirstOrDefault(), 15, templates).gameObject; // Hidden bird
        var birdBook = bird.AddComponent<SpriteFlipbook>(); // Flaps the wings
        birdBook.frames = Sprites("Birds"); // Wing frames
        birdBook.framesPerSecond = 8f; // Flap speed
        bird.SetActive(false); // Keep the bird hidden

        // Explorer, walking in place. Canvas pixel (112, 226) on the 480x270 screen.
        var explorerSprites = Sprites("Explorer"); // Explorer frames: walk then collect
        var explorer = Renderer("Explorer", explorerSprites.FirstOrDefault(), 80, null); // The explorer sprite
        explorer.transform.position = Px(112f, 226f); // Its place on the screen
        var walk = explorer.gameObject.AddComponent<SpriteFlipbook>(); // Plays the explorer's frames
        walk.frames = explorerSprites; // All explorer frames
        walk.framesPerSecond = 10f; // 10 frames per second
        walk.sequence = Enumerable.Range(0, 8).ToArray(); // Start with the 8 walk frames
        var steps = explorer.gameObject.AddComponent<ExplorerFootsteps>(); // Dust on each step
        steps.dustTemplate = dust; // Dust puff to copy
        steps.ground = ground; // Dust moves with the ground
        steps.footOffsetPixels = new Vector2(3f, -12f); // Where the front foot lands
        // Frames 0-7 walk, 8-15 collect (tags "walk" and "collect" in explorer.aseprite).
        var collector = explorer.gameObject.AddComponent<ExplorerCollector>(); // Stops and picks up minerals
        collector.handOffsetsPixels = new Vector2[16]; // One glove position per frame
        Vector2[] collectHands = { new Vector2(1, -5), new Vector2(4, -8), new Vector2(6, -11), new Vector2(6, -12), // Glove positions for the 8 collect frames
                                   new Vector2(5, -6), new Vector2(4, 7), new Vector2(-2, 0), new Vector2(1, -5) }; // (continued)
        for (int i = 0; i < collectHands.Length; i++) collector.handOffsetsPixels[8 + i] = collectHands[i]; // Copy them into frames 8 to 15
        if (explorerSprites.Length < 16) Debug.LogError("Sprites/Explorer should have 16 frames (walk + collect)."); // Warn if frames are missing

        // Title with a shine every 3.2 seconds, bobbing by one pixel.
        var titleSprites = Sprites("Title"); // Title frames: plain, then the shine
        var title = Renderer("Title", titleSprites.FirstOrDefault(), 90, null); // The LAND GRAND title
        title.transform.position = Px(240f, 40f); // Near the top, in the middle
        var shine = title.gameObject.AddComponent<SpriteFlipbook>(); // Plays the shine now and then
        shine.frames = titleSprites; // All title frames
        shine.sequence = Enumerable.Repeat(0, 20).Concat(Enumerable.Range(1, Mathf.Max(0, titleSprites.Length - 1))).ToArray(); // Wait 2 seconds, then sweep the shine
        shine.framesPerSecond = 10f; // Frame speed of the shine
        title.gameObject.AddComponent<PixelBob>(); // Moves the title up and down by one pixel

        // "Tap to Start".
        var tap = Renderer("Tap to Start", Sprites("UI").FirstOrDefault(), 90, null); // The Tap to Start prompt
        tap.transform.position = Px(240f, 257.5f); // Near the bottom, in the middle
        tap.gameObject.AddComponent<Blink>(); // Makes it blink

        // Birds, ground sparkles, and minerals on the path.
        var flock = new GameObject("Birds").AddComponent<BirdFlock>(); // Sends flocks of birds
        flock.birdTemplate = bird; // Bird to copy

        var sparkles = new GameObject("Ground sparkles").AddComponent<GroundSparkles>(); // Twinkles on the ground minerals
        sparkles.pointsJson = AssetDatabase.LoadAssetAtPath<TextAsset>(ArtFolder + "startup-background-points.json"); // Mineral spots in the ground art
        sparkles.groundLayer = ground; // Sparkles stay on the ground
        sparkles.sparkleTemplate = sparkle; // Sparkle to copy

        var minerals = new GameObject("Minerals on path").AddComponent<MineralSpawner>(); // Puts minerals on the path
        minerals.gameObject.AddComponent<AudioSource>().playOnAwake = false; // Sound player for the chimes; silent at first
        minerals.mineralSprites = Sprites("Minerals"); // Mineral pictures
        minerals.pickupSounds = Directory.GetFiles(AudioFolder, "sfx_mineral_chime_*.wav") // Find every chime file...
            .Select(p => AssetDatabase.LoadAssetAtPath<AudioClip>(p.Replace('\\', '/'))) // ...load each one...
            .Where(c => c != null).ToArray(); // ...and keep the ones that loaded
        minerals.explorer = collector; // The explorer who picks them up
        minerals.sparkleTemplate = sparkle; // Sparkle to copy
        minerals.groundLayer = ground; // Minerals scroll with the ground
        minerals.pathY = Px(0f, 236f).y; // Height of the path
        minerals.secondsBetweenMinerals = new Vector2(1.5f, 3f); // Wait between minerals
        minerals.firstMineralAheadUnits = 3f; // First mineral comes soon

        // Background music and ambience (seamless loops, fade in on start, fade out on tap).
        var music = new GameObject("Music").AddComponent<StartScreenMusic>(); // Music player
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "bgm_startscreen.ogg"); // Quest music
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "amb_wind_birds.ogg"); // Wind and birds
        music.musicVolume = 0.55f; // Base music loudness
        music.ambienceVolume = 0.35f; // Base ambience loudness
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}bgm_startscreen.ogg"); // Warn if the music file is missing
        if (music.ambience == null) Debug.LogError($"Missing {AudioFolder}amb_wind_birds.ogg"); // Warn if the ambience file is missing

        var controller = new GameObject("Start Screen").AddComponent<StartScreenController>(); // Waits for the tap
        controller.music = music; // So it can fade the music out

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, ScenePath); // Save the scene
        AddSceneToBuildFirst(ScenePath); // Put it first in the build list
        Debug.Log($"Start screen built: {ScenePath}. Press Play to try it."); // Tell the user it worked
    }

    // Screen pixel (480x270, y down from the top) to world position.
    static Vector3 Px(float x, float y) => new Vector3((x - 240f) / PPU, (135f - y) / PPU, 0f); // Centre of the screen is world (0, 0)

    static SpriteRenderer Renderer(string name, Sprite sprite, int order, Transform parent) // Makes a sprite object
    {
        var go = new GameObject(name); // New object
        if (parent != null) go.transform.SetParent(parent, false); // Put it under its parent if one is given
        var sr = go.AddComponent<SpriteRenderer>(); // Draws the sprite
        sr.sprite = sprite; // Which picture
        sr.sortingOrder = order; // Higher numbers draw in front
        return sr; // Give it back
    }

    static GameObject Effect(string name, Sprite[] frames, int order, float fps, Transform parent) // Makes a hidden one-time effect to copy
    {
        var sr = Renderer(name, frames.FirstOrDefault(), order, parent); // The effect sprite
        var book = sr.gameObject.AddComponent<SpriteFlipbook>(); // Plays its frames
        book.frames = frames; // Effect frames
        book.framesPerSecond = fps; // Effect speed
        book.loop = false; // Play once
        book.destroyWhenDone = true; // Remove each copy when it ends
        sr.gameObject.SetActive(false); // Keep the template hidden
        return sr.gameObject; // Give it back
    }

    /// <summary>All sprites in Sprites/&lt;folder&gt;, sorted by file name (frame order).</summary>
    static Sprite[] Sprites(string folder) // Loads one folder of sprites
    {
        var dir = SpriteFolder + folder; // Folder to read
        if (!Directory.Exists(dir)) { Debug.LogError($"Missing folder {dir}"); return new Sprite[0]; } // Stop if it is missing
        var sprites = Directory.GetFiles(dir, "*.png").Select(p => p.Replace('\\', '/')).OrderBy(p => p) // PNG paths in name order
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p)).Where(sp => sp != null).ToArray(); // Load each as a sprite
        if (sprites.Length == 0) Debug.LogError($"No sprites found in {dir}"); // Warn if none loaded
        return sprites; // Give them back
    }

    /// <summary>Pixel-art import settings for every PNG under Sprites/.</summary>
    static void ConfigureImporters() // Sets import options on the start-screen PNGs
    {
        foreach (var file in Directory.GetFiles(SpriteFolder, "*.png", SearchOption.AllDirectories)) // Every start-screen PNG
        {
            var path = file.Replace('\\', '/'); // Unity paths use forward slashes
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) continue; // Skip anything that is not a picture
            importer.textureType = TextureImporterType.Sprite; // Use it as a sprite
            importer.spriteImportMode = SpriteImportMode.Single; // One sprite per file
            importer.spritePixelsPerUnit = PPU; // 32 pixels per unit
            importer.filterMode = FilterMode.Point; // No blur between pixels
            importer.mipmapEnabled = false; // No smaller copies
            importer.alphaIsTransparency = true; // Clean see-through edges
            importer.npotScale = TextureImporterNPOTScale.None; // Keep the exact size
            importer.wrapMode = TextureWrapMode.Clamp; // Do not repeat the edges
            importer.textureCompression = TextureImporterCompression.Uncompressed; // Keep exact palette colours
            var settings = new TextureImporterSettings(); // Extra sprite settings
            importer.ReadTextureSettings(settings); // Start from the current ones
            settings.spriteAlignment = (int)SpriteAlignment.Center; // Pivot in the middle
            settings.spriteMeshType = SpriteMeshType.FullRect; // Simple rectangle shape
            importer.SetTextureSettings(settings); // Apply the extra settings
            importer.SaveAndReimport(); // Save and re-import the PNG
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); // Wait until every import is done
    }

    internal static void SetLandscapeOnly() // Internal so the Main Menu builder can use it too
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation; // Let the phone turn...
        PlayerSettings.allowedAutorotateToPortrait = false; // ...but never upright...
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false; // ...or upside down...
        PlayerSettings.allowedAutorotateToLandscapeLeft = true; // ...only sideways one way...
        PlayerSettings.allowedAutorotateToLandscapeRight = true; // ...or the other
    }

    static void AddSceneToBuildFirst(string path) // Puts the scene first in the build list
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != path).ToList(); // Build list without this scene
        scenes.Insert(0, new EditorBuildSettingsScene(path, true)); // Add it at the front
        EditorBuildSettings.scenes = scenes.ToArray(); // Save the new build list
    }
}
