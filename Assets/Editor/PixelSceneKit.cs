using System.Collections.Generic; // Gives the set of generated picture names
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
    internal const string HillsNormalArtScript = "Tools/Art/make_hills_normal_art.py"; // The generator for the new Hills Normal sprites
    static readonly HashSet<string> HillsNormalLessonAndItemPictures = new HashSet<string> // Pictures used by several different builders
    { // Opens the picture list
        "lesson_hundred_islands", "lesson_callao_cave", "lesson_pagsanjan", "lesson_banaue", "lesson_cappadocia", // Five new lesson pictures
        "aragonite", "coquina", "flowstone", "dripstone", "volcanic_breccia", "ignimbrite", "zeolite", // Seven new item pictures
    }; // Ends the picture list
    internal const string HillsHardArtScript = "Tools/Art/make_hills_hard_art.py"; // Makes the Hills Hard pictures
    static readonly HashSet<string> HillsHardLessonAndItemPictures = new HashSet<string> // Shared Hard pictures used by several builders
    { // Opens the picture list
        "lesson_tarsier_forest", "lesson_antipolo_slope", "lesson_masungi", "lesson_pililla", "lesson_clew_bay", "lesson_rome_hills", "lesson_guilin", // Seven lesson pictures
        "peridotite", "chlorite", "dacite", "quartzite", "travertine", "leucite", // Six item pictures
    }; // Ends the picture list
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
        cam.allowMSAA = false; // No anti-aliasing: it blurs pixel edges
        cam.allowHDR = false; // No HDR: pixel art needs exact palette colours
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

    /// <summary>Music for the level scenes: the landform's own tune, quieter than the menu, and its wind or stream for the difficulty (D-042, D-062, D-066).</summary>
    internal static StartScreenMusic LevelMusic(LandGrand.Rules.LevelData level) // Music for the level scene
    {
        var music = new GameObject("Music").AddComponent<StartScreenMusic>(); // Music player
        string tune = LandGrand.Rules.LandformAudio.MusicFor(level.Episode) + ".ogg"; // The tune file for this landform
        string ambience = LandGrand.Rules.LandformAudio.AmbienceFor(level) + ".ogg"; // Uses the level override or the landform's sound
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + tune); // The landform's tune
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + ambience); // Landform and difficulty choose the sound
        music.musicVolume = 0.28f; // Quieter than the menu (0.40), so the breeze and the chimes come through
        music.ambienceVolume = 0.30f; // Same as the menu ambience
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}{tune}. Run: python Tools/Audio/{LandGrand.Rules.LandformAudio.MusicScriptFor(level.Episode)}"); // Say what to do, using the script named in the audio table
        if (music.ambience == null) // An ambience file has not been made or imported
        { // Opens the missing-file message
            string script = LandGrand.Rules.LandformAudio.AmbienceScriptFor(level); // Finds the generator for this chosen sound
            string repair = script != null ? $"Run: python Tools/Audio/{script}" : "Check the level's ambience file name."; // Explains how to repair the chosen sound
            Debug.LogError($"Missing {AudioFolder}{ambience}. {repair}"); // Names the missing file and repair
        } // Ends the missing-file message
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

    /// <summary>Call just before saving a scene. Reloads both fonts and puts the right one on every text: Andika on reading text
    /// (made at the reading size), the pixel font at its sharp size on everything else. So no label is saved with a stale font
    /// reference or TextMeshPro's default font (decisions D-047, D-050, D-051).</summary>
    internal static void FixFonts() // Puts the right font on every text in the open scene
    {
        var pixel = M.EnsureFont(); // The 2x pixel font, loaded fresh after any refresh during the build
        var reading = M.EnsureReadingFont(); // The Andika reading font, loaded fresh too
        foreach (var text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include)) // Every text in the scene, hidden panels too
        {
            if (Mathf.Approximately(text.fontSize, M.ReadingFontSize)) { text.font = reading; continue; } // Reading text keeps Andika
            text.font = pixel; // Everything else: the pixel font
            text.fontSize = M.FontSize; // At its sharp 2x size
        }
    }

    /// <summary>Connects the shared parts to a screen controller: guide panel at the top, music, click and canvas group.</summary>
    internal static void Wire(PixelScreen screen, Transform root, TMP_FontAsset font, StartScreenMusic music, CanvasGroup group) // Fills the PixelScreen fields
    {
        var guideC = new Vector2(240f, 16f); // Middle of the guide panel, top centre
        var guide = M.Panel("Guide", root, guideC, new Vector2(240f, 26f)); // Panel background, wide enough for "Oops! Back to the checkpoint" at 2x
        guide.GetComponent<Image>().raycastTarget = false; // Taps pass through
        screen.guideText = M.ReadingLabel("Text", guide, "", guideC, guideC, new Vector2(228f, 22f), Ink, TextAlignmentOptions.Midline); // The guide words, in the reading font (D-051); the longest, "Oops! Back to the checkpoint", is 201 pixels
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
        if (sprite == null) // A sprite has not been made or imported
        { // Opens the missing-file message
            string script = HillsNormalLessonAndItemPictures.Contains(path) ? HillsNormalArtScript : artScript; // Resolves the shared lesson and item pictures too
            if (HillsHardLessonAndItemPictures.Contains(path)) script = HillsHardArtScript; // Names the Hard generator for shared pictures
            Debug.LogError($"Missing sprite {folder}{path}.png. Run python {script}"); // Say which file and generator to use
        } // Ends the missing-file message
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
