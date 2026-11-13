using System.Linq; // Gives access to list helpers such as Where
using LandGrand; // Gives access to the shared scripts (UI zoom, PixelScreen)
using LandGrand.LandformMap; // Gives access to the Mountains animations
using LandGrand.StartScreen; // Gives access to the music player and frame animation
using TMPro; // Gives access to TextMeshPro text and fonts
using UnityEditor; // Gives access to asset loading and the build list
using UnityEngine; // Gives access to GameObject, Camera, Sprite and other basics
using UnityEngine.EventSystems; // Gives access to the EventSystem that sends taps to buttons
using UnityEngine.InputSystem.UI; // Gives access to the UI input module for the new Input System
using UnityEngine.Rendering.Universal; // Gives access to the Pixel Perfect Camera
using UnityEngine.UI; // Gives access to Canvas tools, Image and Button
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Shared scene parts for the screens after the Main Menu (map, difficulty cards, level trail):
/// the 480x270 pixel-perfect camera, the UI canvas, music, the Back icon, the guide panel and the Mountains animations.
/// Each builder calls these so every screen looks and behaves the same.
/// </summary>
internal static class PixelSceneKit // Helpers for the scene builders
{
    internal const float PPU = 32f; // Sprite pixels per Unity unit, same as every screen
    internal static readonly Vector2 CanvasCenter = new Vector2(240f, 135f); // Middle of the 480x270 screen
    internal static readonly Color32 Ink = new Color32(31, 26, 36, 255); // Dark outline colour for text and wide-screen edges
    const string AudioFolder = "Assets/Audio/"; // Music and sound files
    const string MapSprites = "Assets/Art/LandformMap/Sprites/"; // Back icon and Mountains animation pictures live here

    /// <summary>Makes the 480x270 pixel-perfect camera and gives back its Pixel Perfect Camera.</summary>
    internal static PixelPerfectCamera Camera() // The camera every screen uses
    {
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" }; // Camera object, tagged as the main camera
        var cam = camGo.AddComponent<UnityEngine.Camera>(); // The camera itself
        cam.orthographic = true; // Flat 2D view with no perspective
        cam.orthographicSize = 270f / 2f / PPU; // Shows exactly 270 pixels from top to bottom
        cam.transform.position = new Vector3(0f, 0f, -10f); // Stand back from the sprites so it can see them
        cam.clearFlags = CameraClearFlags.SolidColor; // Fill empty space with one colour
        cam.backgroundColor = Ink; // Dark outline colour around the art on wide screens
        var ppc = camGo.AddComponent<PixelPerfectCamera>(); // Keeps pixels square and sharp
        ppc.assetsPPU = (int)PPU; // Sprites use 32 pixels per unit
        ppc.refResolutionX = 480; // Screen is 480 pixels wide
        ppc.refResolutionY = 270; // Screen is 270 pixels tall
        ppc.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture; // Draw small, then scale up evenly
        ppc.cropFrame = PixelPerfectCamera.CropFrame.None; // Fill the whole phone screen
        camGo.AddComponent<AudioListener>(); // Lets the player hear sounds
        return ppc; // The canvas needs it to follow the zoom
    }

    /// <summary>The calm menu music and the wind-and-birds ambience, at Main Menu loudness.</summary>
    internal static StartScreenMusic MenuMusic() // Music for the menu-style screens
    {
        var music = new GameObject("Music").AddComponent<StartScreenMusic>(); // Music player
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "bgm_mainmenu.ogg"); // Calm menu music
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "amb_wind_birds.ogg"); // Wind and birds
        music.musicVolume = 0.40f; // Same loudness as the Main Menu
        music.ambienceVolume = 0.30f; // Same loudness as the Main Menu
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}bgm_mainmenu.ogg"); // Warn if the music file is missing
        if (music.ambience == null) Debug.LogError($"Missing {AudioFolder}amb_wind_birds.ogg"); // Warn if the ambience file is missing
        return music; // The controller fades it out when leaving
    }

    /// <summary>The event system that sends taps to the buttons.</summary>
    internal static void Events() // Makes the UI input objects
    {
        var events = new GameObject("EventSystem"); // Object that handles UI input
        events.AddComponent<EventSystem>(); // Decides which button was tapped
        events.AddComponent<InputSystemUIInputModule>(); // Reads taps from the new Input System
    }

    /// <summary>A UI canvas with the same whole-number zoom as the art. Gives back its root and its CanvasGroup.</summary>
    internal static Transform Canvas(string name, PixelPerfectCamera ppc, out CanvasGroup group) // Makes the UI canvas
    {
        var canvasGo = new GameObject(name, typeof(RectTransform)); // UI root object
        var canvas = canvasGo.AddComponent<UnityEngine.Canvas>(); // The UI canvas
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Draw on top of the game view
        canvas.pixelPerfect = true; // Snap UI pixels to screen pixels
        var scaler = canvasGo.AddComponent<CanvasScaler>(); // Sets the size of the UI on the screen
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; // Size is a whole-number zoom
        scaler.scaleFactor = 4f; // 1920x1080 zoom until the game starts and the real zoom is known
        scaler.referencePixelsPerUnit = PPU; // One sprite pixel equals one UI pixel
        canvasGo.AddComponent<PixelPerfectUiScale>().pixelCamera = ppc; // UI zoom always matches the art zoom
        canvasGo.AddComponent<GraphicRaycaster>(); // Finds which UI picture was tapped
        group = canvasGo.AddComponent<CanvasGroup>(); // Lets the controller turn off every button at once
        return canvasGo.transform; // Parent for the screen's UI
    }

    /// <summary>The brown Back icon (an arrow, no word) in the top-left corner.</summary>
    internal static Button BackIcon(Transform root) // Makes the Back button
    {
        var image = M.Picture("Back", root, MapSprite("Buttons/btn_back_icon"), new Vector2(18f, 16f), CanvasCenter); // Brown button with a white arrow
        image.raycastTarget = true; // Buttons must catch taps
        var back = image.gameObject.AddComponent<Button>(); // Makes it tappable
        back.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        back.targetGraphic = image; // The picture that changes
        back.spriteState = new SpriteState { pressedSprite = MapSprite("Buttons/btn_back_icon_pressed") }; // Pushed-in picture
        back.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        return back; // The controller connects it
    }

    /// <summary>Connects the shared parts to a screen controller: guide panel at the top, music, click and canvas group.</summary>
    internal static void Wire(PixelScreen screen, Transform root, TMP_FontAsset font, StartScreenMusic music, CanvasGroup group) // Fills the PixelScreen fields
    {
        var guideC = new Vector2(240f, 16f); // Middle of the guide panel, top centre
        var guide = M.Panel("Guide", root, guideC, new Vector2(200f, 24f)); // Panel background
        guide.GetComponent<Image>().raycastTarget = false; // Taps pass through
        screen.guideText = M.Label("Text", guide, font, "", guideC, guideC, new Vector2(188f, 14f), Ink, TextAlignmentOptions.Midline); // The guide words
        screen.guidePanel = guide.gameObject; // Guide panel
        screen.music = music; // Music player, so it can fade out
        screen.clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "sfx_ui_click.wav"); // Tap sound
        screen.ui = group; // Whole UI, so taps can be turned off while leaving
        if (screen.clickSound == null) Debug.LogError($"Missing {AudioFolder}sfx_ui_click.wav"); // Warn if the click sound is missing
        guide.gameObject.SetActive(false); // Guide starts hidden
    }

    /// <summary>Mountains animations: a drifting cloud, a circling eagle and blowing snow. Callers set where they move.</summary>
    internal static Transform MountainsLife(out CloudDrift cloud, out CircleFlight eagle, out SnowFlurry snow) // Makes the Mountains animations
    {
        var life = new GameObject("Mountains life").transform; // Parent for the Mountains animations
        cloud = M.Renderer("Cloud", MapSprite("Mountains/cloud"), 10, life).gameObject.AddComponent<CloudDrift>(); // Cloud that drifts across the sky
        var eagleSprite = M.Renderer("Eagle", MapSprite("Mountains/eagle_0"), 15, life); // Eagle picture
        var wings = eagleSprite.gameObject.AddComponent<SpriteFlipbook>(); // Flaps the wings
        wings.frames = new[] { MapSprite("Mountains/eagle_0"), MapSprite("Mountains/eagle_1") }; // Wings up, wings down
        wings.framesPerSecond = 3f; // Slow flaps, like a gliding bird
        eagle = eagleSprite.gameObject.AddComponent<CircleFlight>(); // Circles above the peaks
        snow = new GameObject("Snow").AddComponent<SnowFlurry>(); // Blowing snow
        snow.transform.SetParent(life, false); // Keep it with the other Mountains animations
        snow.flakeSprites = new[] { MapSprite("Mountains/snow_0"), MapSprite("Mountains/snow_1") }; // Small and big flakes
        return life; // The map shows it only while Mountains is unlocked
    }

    /// <summary>Loads a sprite by folder and path (without ".png") and says which art script makes it if it is missing.</summary>
    internal static Sprite Sprite(string folder, string path, string artScript) // Loads one sprite
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(folder + path + ".png"); // Load the PNG as a sprite
        if (sprite == null) Debug.LogError($"Missing sprite {folder}{path}.png. Run python {artScript}"); // Say which file is missing
        return sprite; // Give it back
    }

    static Sprite MapSprite(string path) => Sprite(MapSprites, path, "Tools/Art/make_landform_map_art.py"); // Loads a Landform Map sprite

    /// <summary>Puts a scene in the build list right after another one (or at the end if that one is missing).</summary>
    internal static void AddSceneAfter(string scenePath, string afterPath) // Updates the build list
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != scenePath).ToList(); // Build list without this scene
        int after = scenes.FindIndex(s => s.path == afterPath); // Where the earlier scene is
        scenes.Insert(after >= 0 ? after + 1 : scenes.Count, new EditorBuildSettingsScene(scenePath, true)); // Right after it, or at the end
        EditorBuildSettings.scenes = scenes.ToArray(); // Save the new build list
    }
}
