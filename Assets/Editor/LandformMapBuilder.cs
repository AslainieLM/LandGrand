using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Where
using LandGrand; // Gives access to the shared scripts (UI zoom)
using LandGrand.LandformMap; // Gives access to the Landform Map scripts
using LandGrand.Rules; // Gives access to Episode
using LandGrand.StartScreen; // Gives access to the music player and frame animation we reuse
using TMPro; // Gives access to TextMeshPro text
using UnityEditor; // Gives access to editor tools such as menu items and asset loading
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Camera, Sprite and other basics
using UnityEngine.EventSystems; // Gives access to the EventSystem that sends taps to buttons
using UnityEngine.InputSystem.UI; // Gives access to the UI input module for the new Input System
using UnityEngine.Rendering.Universal; // Gives access to the Pixel Perfect Camera
using UnityEngine.UI; // Gives access to Canvas tools, Image and Button
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds Assets/Scenes/LandformMap.unity (requirement LG-17, ticket 04) and puts it after the Main Menu in the build list.
/// Run it from the menu: LandGrand > Build Landform Map Scene. Running it again rebuilds the scene.
/// Draw the art first with: python Tools/Art/make_landform_map_art.py
/// Screen positions below are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class LandformMapBuilder // Makes the Landform Map scene
{
    const string SpriteFolder = "Assets/Art/LandformMap/Sprites/"; // Landform Map PNGs
    const string AudioFolder = "Assets/Audio/"; // Music and sound files
    const string ScenePath = "Assets/Scenes/LandformMap.unity"; // Where the scene is saved
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity"; // The map goes right after this scene in the build list
    const float PPU = 32f; // Sprite pixels per Unity unit, same as the other screens
    static readonly Vector2 CanvasCenter = new Vector2(240f, 135f); // Middle of the 480x270 screen
    static readonly Color32 Ink = new Color32(31, 26, 36, 255); // Dark outline colour for text

    // Each region: landform, file name, label middle and padlock middle (map pixels).
    static readonly (Episode episode, string name, Vector2 label, Vector2 padlock)[] Regions = // One entry per region
    {
        (Episode.Mountains, "mountains", new Vector2(60f, 114f), new Vector2(150f, 60f)), // Top left
        (Episode.Hills, "hills", new Vector2(430f, 118f), new Vector2(360f, 60f)), // Top right
        (Episode.Plains, "plains", new Vector2(60f, 248f), new Vector2(170f, 236f)), // Bottom left
        (Episode.Plateaus, "plateaus", new Vector2(424f, 248f), new Vector2(330f, 150f)), // Bottom right
    };

    [MenuItem("LandGrand/Build Landform Map Scene")] // Adds the command to Unity's LandGrand menu
    public static void Build() // Builds and saves the whole Landform Map scene
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(SpriteFolder + "map.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing {SpriteFolder}map.png. Run: python Tools/Art/make_landform_map_art.py"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        var font = M.EnsureFont(); // The pixel font as a TextMeshPro font
        if (font == null) return; // EnsureFont already said what is wrong
        M.ConfigurePixelSprites(SpriteFolder, path => path.Contains("/Locked/")); // Crisp pixel art; the grey copies stay readable because they are the tap areas
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene

        // Camera: the same 480x270 pixel-perfect view as the other screens.
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" }; // Camera object, tagged as the main camera
        var cam = camGo.AddComponent<Camera>(); // The camera itself
        cam.orthographic = true; // Flat 2D view with no perspective
        cam.orthographicSize = 270f / 2f / PPU; // Shows exactly 270 pixels from top to bottom
        cam.transform.position = new Vector3(0f, 0f, -10f); // Stand back from the sprites so it can see them
        cam.clearFlags = CameraClearFlags.SolidColor; // Fill empty space with one colour
        cam.backgroundColor = Ink; // Dark outline colour around the map on wide screens
        var ppc = camGo.AddComponent<PixelPerfectCamera>(); // Keeps pixels square and sharp
        ppc.assetsPPU = (int)PPU; // Sprites use 32 pixels per unit
        ppc.refResolutionX = 480; // Screen is 480 pixels wide
        ppc.refResolutionY = 270; // Screen is 270 pixels tall
        ppc.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture; // Draw small, then scale up evenly
        ppc.cropFrame = PixelPerfectCamera.CropFrame.None; // Fill the whole phone screen
        camGo.AddComponent<AudioListener>(); // Lets the player hear sounds

        // The map picture: all four regions in one connected 480x270 image.
        M.Renderer("Map", S("map"), 0, null).transform.position = M.Px(240f, 135f); // Fills the screen

        // Mountains life: blowing snow, a drifting cloud and a circling eagle (shown only while Mountains is unlocked).
        var life = new GameObject("Mountains life").transform; // Parent for the Mountains animations
        var cloud = M.Renderer("Cloud", S("Mountains/cloud"), 10, life); // Cloud picture
        cloud.gameObject.AddComponent<CloudDrift>(); // Drifts it across the sky
        var eagle = M.Renderer("Eagle", S("Mountains/eagle_0"), 15, life); // Eagle picture
        var wings = eagle.gameObject.AddComponent<SpriteFlipbook>(); // Flaps the wings
        wings.frames = new[] { S("Mountains/eagle_0"), S("Mountains/eagle_1") }; // Wings up, wings down
        wings.framesPerSecond = 3f; // Slow flaps, like a gliding bird
        eagle.gameObject.AddComponent<CircleFlight>(); // Circles above the peaks
        var snow = new GameObject("Snow").AddComponent<SnowFlurry>(); // Blowing snow
        snow.transform.SetParent(life, false); // Keep it with the other Mountains animations
        snow.flakeSprites = new[] { S("Mountains/snow_0"), S("Mountains/snow_1") }; // Small and big flakes

        // Music: the calm menu loop and the wind-and-birds ambience, the same as the Main Menu.
        var music = new GameObject("Music").AddComponent<StartScreenMusic>(); // Music player
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "bgm_mainmenu.ogg"); // Calm menu music
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "amb_wind_birds.ogg"); // Wind and birds
        music.musicVolume = 0.40f; // Same loudness as the Main Menu
        music.ambienceVolume = 0.30f; // Same loudness as the Main Menu
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}bgm_mainmenu.ogg"); // Warn if the music file is missing
        if (music.ambience == null) Debug.LogError($"Missing {AudioFolder}amb_wind_birds.ogg"); // Warn if the ambience file is missing

        // Event system: sends taps and clicks to the buttons.
        var events = new GameObject("EventSystem"); // Object that handles UI input
        events.AddComponent<EventSystem>(); // Decides which button was tapped
        events.AddComponent<InputSystemUIInputModule>(); // Reads taps from the new Input System

        // Canvas: holds the region tap areas, labels and buttons, with the same whole-number zoom as the art.
        var canvasGo = new GameObject("Map UI", typeof(RectTransform)); // UI root object
        var canvas = canvasGo.AddComponent<Canvas>(); // The UI canvas
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Draw on top of the game view
        canvas.pixelPerfect = true; // Snap UI pixels to screen pixels
        var scaler = canvasGo.AddComponent<CanvasScaler>(); // Sets the size of the UI on the screen
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; // Size is a whole-number zoom, set below
        scaler.scaleFactor = 4f; // 1920x1080 zoom until the game starts and the real zoom is known
        scaler.referencePixelsPerUnit = PPU; // One sprite pixel equals one UI pixel
        canvasGo.AddComponent<PixelPerfectUiScale>().pixelCamera = ppc; // UI zoom always matches the art zoom
        canvasGo.AddComponent<GraphicRaycaster>(); // Finds which UI picture was tapped
        var group = canvasGo.AddComponent<CanvasGroup>(); // Lets the controller turn off every button at once
        var root = canvasGo.transform; // Parent for everything below

        // Regions: grey copy (tap area), then label and padlock on top.
        var regions = new LandformMapController.Region[Regions.Length]; // Filled below
        for (int i = 0; i < Regions.Length; i++) // Tap areas first, so labels draw on top of them
        {
            var r = Regions[i]; // This region's layout
            var locked = M.Picture("Region " + r.name, root, S("Locked/locked_" + r.name), CanvasCenter, CanvasCenter); // Grey copy of the region over the whole screen
            locked.raycastTarget = true; // It catches taps; the controller limits that to the region's own pixels
            var button = locked.gameObject.AddComponent<Button>(); // Makes the region tappable
            button.transition = Selectable.Transition.None; // The colour shows locked or unlocked, so taps must not change it
            button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between regions
            regions[i] = new LandformMapController.Region { episode = r.episode, locked = locked, button = button }; // Start the region record
        }
        for (int i = 0; i < Regions.Length; i++) // Labels and padlocks
        {
            var r = Regions[i]; // This region's layout
            var plaque = M.Panel("Label " + r.name, root, r.label, new Vector2(88f, 38f)); // Small cream plaque
            plaque.GetComponent<Image>().raycastTarget = false; // Taps pass through to the region
            M.Picture("Name", plaque, S("Labels/label_" + r.name), r.label + new Vector2(0f, -8f), r.label); // Region name
            M.Picture("Star", plaque, S("star"), r.label + new Vector2(-18f, 7f), r.label); // Star icon
            regions[i].stars = M.Label("Stars", plaque, font, "0/45", r.label + new Vector2(10f, 7f), r.label, new Vector2(40f, 14f), Ink, TextAlignmentOptions.MidlineLeft); // Star progress
            regions[i].padlock = M.Picture("Padlock " + r.name, root, S("padlock"), r.padlock, CanvasCenter).gameObject; // Padlock while locked
        }
        regions[0].life = life.gameObject; // Only Mountains has animations so far

        // Player name at the top.
        var nameC = new Vector2(240f, 14f); // Middle of the name plaque
        var namePlaque = M.Panel("Player name", root, nameC, new Vector2(140f, 22f)); // Small plaque
        namePlaque.GetComponent<Image>().raycastTarget = false; // Taps pass through
        var playerName = M.Label("Name", namePlaque, font, "", nameC, nameC, new Vector2(128f, 14f), Ink, TextAlignmentOptions.Midline); // The player's name

        // Guide panel: "Finish Mountains first" and similar, shown for a moment.
        var guideC = new Vector2(240f, 40f); // Middle of the guide panel, under the name
        var guide = M.Panel("Guide", root, guideC, new Vector2(200f, 24f)); // Panel background
        guide.GetComponent<Image>().raycastTarget = false; // Taps pass through
        var guideText = M.Label("Text", guide, font, "", guideC, guideC, new Vector2(188f, 14f), Ink, TextAlignmentOptions.Midline); // The guide words

        // Back button in the top-left corner (made last, so it is on top).
        var back = M.MakeButton("Back", root, "back", new Vector2(34f, 16f), CanvasCenter); // Returns to the Main Menu

        // Controller: connects everything.
        var controller = new GameObject("Landform Map").AddComponent<LandformMapController>(); // The map brain
        controller.regions = regions; // The four regions
        controller.playerName = playerName; // Name at the top
        controller.backButton = back; // Back button
        controller.guidePanel = guide.gameObject; // Guide panel
        controller.guideText = guideText; // Guide words
        controller.music = music; // Music player, so it can fade out
        controller.clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "sfx_ui_click.wav"); // Tap sound
        controller.ui = group; // Whole UI, so taps can be turned off while leaving
        if (controller.clickSound == null) Debug.LogError($"Missing {AudioFolder}sfx_ui_click.wav"); // Warn if the click sound is missing
        guide.gameObject.SetActive(false); // Guide starts hidden

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, ScenePath); // Save the new scene
        AddSceneAfterMainMenu(); // Put it in the build list so Play can open it
        Debug.Log($"Landform Map built: {ScenePath}. Open StartScreen, press Play, and tap Play on the Main Menu."); // Tell the user it worked
    }

    /// <summary>Loads a Landform Map sprite by its path under Sprites/, without ".png".</summary>
    static Sprite S(string path) // Loads one Landform Map sprite
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpriteFolder + path + ".png"); // Load the PNG as a sprite
        if (sprite == null) Debug.LogError($"Missing sprite {SpriteFolder}{path}.png. Run Tools/Art/make_landform_map_art.py."); // Say which file is missing
        return sprite; // Give it back
    }

    static void AddSceneAfterMainMenu() // Adds the scene to the build list, right after the Main Menu
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != ScenePath).ToList(); // Build list without this scene
        int menu = scenes.FindIndex(s => s.path == MainMenuPath); // Where the Main Menu is
        scenes.Insert(menu >= 0 ? menu + 1 : scenes.Count, new EditorBuildSettingsScene(ScenePath, true)); // After the menu, or at the end
        EditorBuildSettings.scenes = scenes.ToArray(); // Save the new build list
    }
}
