using System.IO;
using System.Linq;
using LandGrand.StartScreen;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Builds Assets/Scenes/StartScreen.unity from the start-screen art.
/// Run it from the menu: LandGrand > Build Start Screen Scene. Running it again rebuilds the scene.
/// </summary>
public static class StartScreenBuilder
{
    const string ArtFolder = "Assets/Art/StartScreen/";
    const string SpriteFolder = "Assets/Art/StartScreen/Sprites/";
    const string AudioFolder = "Assets/Audio/";
    const string ScenePath = "Assets/Scenes/StartScreen.unity";
    const float PPU = 32f;

    // Background layers: PNG name in Sprites/Background, scroll speed in canvas pixels per second.
    static readonly (string prefix, float speed)[] Layers =
    {
        ("L0", 0f), ("L1", 1.5f), ("L2", 1f), ("L3", 2.5f),
        ("L4", 4.5f), ("L5", 8f), ("L6", 14f), ("L7", 20f),
    };

    [MenuItem("LandGrand/Build Start Screen Scene")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        ConfigureImporters();
        SetLandscapeOnly();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera: 480x270 pixel-perfect view.
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 270f / 2f / PPU;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(191, 232, 247, 255);
        var ppc = camGo.AddComponent<PixelPerfectCamera>();
        ppc.assetsPPU = (int)PPU;
        ppc.refResolutionX = 480;
        ppc.refResolutionY = 270;
        ppc.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture;
        ppc.cropFrame = PixelPerfectCamera.CropFrame.None;
        camGo.AddComponent<AudioListener>();

        // Background layers.
        var bgSprites = Sprites("Background");
        var background = new GameObject("Background").transform;
        ParallaxLayer ground = null;
        for (int i = 0; i < Layers.Length; i++)
        {
            var sprite = bgSprites.FirstOrDefault(s => s.name.StartsWith(Layers[i].prefix));
            if (sprite == null) { Debug.LogError($"No sprite for layer {Layers[i].prefix} in {SpriteFolder}Background"); continue; }
            var layer = new GameObject(sprite.name).AddComponent<ParallaxLayer>();
            layer.transform.SetParent(background, false);
            layer.speedPixelsPerSecond = Layers[i].speed;
            layer.widthPixels = 960f;
            layer.pixelsPerUnit = PPU;
            layer.followsWalk = Layers[i].prefix != "L1";   // clouds keep drifting when the explorer stops
            var art = Renderer("Art", sprite, i * 10, layer.transform);
            art.transform.localPosition = Vector3.zero;
            if (Layers[i].prefix == "L7") ground = layer;
        }
        var fill = new GameObject("Below-ground fill").AddComponent<ScreenFill>();
        fill.topY = -135f / PPU;

        // Effect templates (disabled; scripts copy them).
        var templates = new GameObject("Effect templates").transform;
        var sparkle = Effect("Sparkle", Sprites("Sparkle"), 85, 12f, templates);
        var dust = Effect("Dust", Sprites("Dust"), 81, 10f, templates);
        var bird = Renderer("Bird", Sprites("Birds").FirstOrDefault(), 15, templates).gameObject;
        var birdBook = bird.AddComponent<SpriteFlipbook>();
        birdBook.frames = Sprites("Birds");
        birdBook.framesPerSecond = 8f;
        bird.SetActive(false);

        // Explorer, walking in place. Canvas pixel (112, 226) on the 480x270 screen.
        var explorerSprites = Sprites("Explorer");
        var explorer = Renderer("Explorer", explorerSprites.FirstOrDefault(), 80, null);
        explorer.transform.position = Px(112f, 226f);
        var walk = explorer.gameObject.AddComponent<SpriteFlipbook>();
        walk.frames = explorerSprites;
        walk.framesPerSecond = 10f;
        walk.sequence = Enumerable.Range(0, 8).ToArray();
        var steps = explorer.gameObject.AddComponent<ExplorerFootsteps>();
        steps.dustTemplate = dust;
        steps.ground = ground;
        steps.footOffsetPixels = new Vector2(3f, -12f);
        // Frames 0-7 walk, 8-15 collect (tags "walk" and "collect" in explorer.aseprite).
        var collector = explorer.gameObject.AddComponent<ExplorerCollector>();
        collector.handOffsetsPixels = new Vector2[16];
        Vector2[] collectHands = { new Vector2(1, -5), new Vector2(4, -8), new Vector2(6, -11), new Vector2(6, -12),
                                   new Vector2(5, -6), new Vector2(4, 7), new Vector2(-2, 0), new Vector2(1, -5) };
        for (int i = 0; i < collectHands.Length; i++) collector.handOffsetsPixels[8 + i] = collectHands[i];
        if (explorerSprites.Length < 16) Debug.LogError("Sprites/Explorer should have 16 frames (walk + collect).");

        // Title with a shine every 3.2 seconds, bobbing by one pixel.
        var titleSprites = Sprites("Title");
        var title = Renderer("Title", titleSprites.FirstOrDefault(), 90, null);
        title.transform.position = Px(240f, 40f);
        var shine = title.gameObject.AddComponent<SpriteFlipbook>();
        shine.frames = titleSprites;
        shine.sequence = Enumerable.Repeat(0, 20).Concat(Enumerable.Range(1, Mathf.Max(0, titleSprites.Length - 1))).ToArray();
        shine.framesPerSecond = 10f;
        title.gameObject.AddComponent<PixelBob>();

        // "Tap to Start".
        var tap = Renderer("Tap to Start", Sprites("UI").FirstOrDefault(), 90, null);
        tap.transform.position = Px(240f, 257.5f);
        tap.gameObject.AddComponent<Blink>();

        // Birds, ground sparkles, and minerals on the path.
        var flock = new GameObject("Birds").AddComponent<BirdFlock>();
        flock.birdTemplate = bird;

        var sparkles = new GameObject("Ground sparkles").AddComponent<GroundSparkles>();
        sparkles.pointsJson = AssetDatabase.LoadAssetAtPath<TextAsset>(ArtFolder + "startup-background-points.json");
        sparkles.groundLayer = ground;
        sparkles.sparkleTemplate = sparkle;

        var minerals = new GameObject("Minerals on path").AddComponent<MineralSpawner>();
        minerals.gameObject.AddComponent<AudioSource>().playOnAwake = false;
        minerals.mineralSprites = Sprites("Minerals");
        minerals.pickupSounds = Directory.GetFiles(AudioFolder, "sfx_mineral_chime_*.wav")
            .Select(p => AssetDatabase.LoadAssetAtPath<AudioClip>(p.Replace('\\', '/')))
            .Where(c => c != null).ToArray();
        minerals.explorer = collector;
        minerals.sparkleTemplate = sparkle;
        minerals.groundLayer = ground;
        minerals.pathY = Px(0f, 236f).y;
        minerals.secondsBetweenMinerals = new Vector2(1.5f, 3f);
        minerals.firstMineralAheadUnits = 3f;

        // Background music and ambience (seamless loops, fade in on start, fade out on tap).
        var music = new GameObject("Music").AddComponent<StartScreenMusic>();
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "bgm_startscreen.ogg");
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "amb_wind_birds.ogg");
        music.musicVolume = 0.55f;
        music.ambienceVolume = 0.35f;
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}bgm_startscreen.ogg");
        if (music.ambience == null) Debug.LogError($"Missing {AudioFolder}amb_wind_birds.ogg");

        var controller = new GameObject("Start Screen").AddComponent<StartScreenController>();
        controller.music = music;

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildFirst(ScenePath);
        Debug.Log($"Start screen built: {ScenePath}. Press Play to try it.");
    }

    // Screen pixel (480x270, y down from the top) to world position.
    static Vector3 Px(float x, float y) => new Vector3((x - 240f) / PPU, (135f - y) / PPU, 0f);

    static SpriteRenderer Renderer(string name, Sprite sprite, int order, Transform parent)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        return sr;
    }

    static GameObject Effect(string name, Sprite[] frames, int order, float fps, Transform parent)
    {
        var sr = Renderer(name, frames.FirstOrDefault(), order, parent);
        var book = sr.gameObject.AddComponent<SpriteFlipbook>();
        book.frames = frames;
        book.framesPerSecond = fps;
        book.loop = false;
        book.destroyWhenDone = true;
        sr.gameObject.SetActive(false);
        return sr.gameObject;
    }

    /// <summary>All sprites in Sprites/&lt;folder&gt;, sorted by file name (frame order).</summary>
    static Sprite[] Sprites(string folder)
    {
        var dir = SpriteFolder + folder;
        if (!Directory.Exists(dir)) { Debug.LogError($"Missing folder {dir}"); return new Sprite[0]; }
        var sprites = Directory.GetFiles(dir, "*.png").Select(p => p.Replace('\\', '/')).OrderBy(p => p)
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p)).Where(sp => sp != null).ToArray();
        if (sprites.Length == 0) Debug.LogError($"No sprites found in {dir}");
        return sprites;
    }

    /// <summary>Pixel-art import settings for every PNG under Sprites/.</summary>
    static void ConfigureImporters()
    {
        foreach (var file in Directory.GetFiles(SpriteFolder, "*.png", SearchOption.AllDirectories))
        {
            var path = file.Replace('\\', '/');
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PPU;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    static void SetLandscapeOnly()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
    }

    static void AddSceneToBuildFirst(string path)
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != path).ToList();
        scenes.Insert(0, new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
