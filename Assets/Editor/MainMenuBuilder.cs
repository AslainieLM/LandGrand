using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Where and Select
using LandGrand.MainMenu; // Gives access to the Main Menu scripts
using LandGrand.StartScreen; // Gives access to the start-screen scripts we reuse (clouds, birds, flipbooks, music)
using LandGrand.LandformMap; // Gives access to CircleFlight, which flies the eagle and the butterflies
using TMPro; // Gives access to TextMeshPro text, input boxes and font assets
using UnityEditor; // Gives access to editor tools such as menu items and asset loading
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using LandGrand; // Gives access to the shared scripts (UI zoom)
using UnityEngine; // Gives access to GameObject, Camera, Sprite and other basics
using UnityEngine.EventSystems; // Gives access to the EventSystem that sends taps to buttons
using UnityEngine.InputSystem.UI; // Gives access to the UI input module for the new Input System
using UnityEngine.Rendering.Universal; // Gives access to the Pixel Perfect Camera
using UnityEngine.UI; // Gives access to Canvas tools, Image, Button and Slider

/// <summary>
/// Builds Assets/Scenes/MainMenu.unity (requirement LG-16) and puts it second in the build list.
/// Run it from the menu: LandGrand > Build Main Menu Scene. Running it again rebuilds the scene.
/// Screen positions below are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class MainMenuBuilder
{
    const string SpriteFolder = "Assets/Art/MainMenu/Sprites/"; // Main Menu PNGs
    const string StartSprites = "Assets/Art/StartScreen/Sprites/"; // Start-screen PNGs we reuse
    const string AudioFolder = "Assets/Audio/"; // Music and sound files
    const string ScenePath = "Assets/Scenes/MainMenu.unity"; // Where the scene is saved
    const string FontFile = "Assets/Fonts/Micro5.ttf"; // The pixel font (SIL Open Font License, see Assets/Fonts/OFL-Micro5.txt)
    // Micro 5's own pixel grid fits a font size of about 10.5 pixels. Only whole multiples stay sharp; 1x (11) is too small to read,
    // so all text uses 2x (22). Sizes in between, such as the old 15, draw some strokes 1 pixel and others 2 pixels wide, and letters look bent (decision D-050).
    const string FontAssetPath = "Assets/Fonts/Micro5 2x.asset"; // TextMeshPro font at 2x, made from the font file by this builder
    internal const int FontSize = 22; // 2x size: every stroke exactly 2 pixels wide, capital letters 10 pixels tall
    // Sentences and descriptions use Andika instead (decision D-051): a smooth font made by SIL for children learning to read,
    // whose letters are clearly different from each other. The UI canvas draws at the phone's full resolution, so it stays smooth.
    const string ReadingFontFile = "Assets/Fonts/Andika-Regular.ttf"; // The reading font (SIL Open Font License, see Assets/Fonts/OFL-Andika.txt)
    const string ReadingFontPath = "Assets/Fonts/Andika SDF.asset"; // TextMeshPro font made from it by this builder
    internal const int ReadingFontSize = 15; // UI size: the longest lesson sentence (309 pixels) fits its 312-pixel line
    internal const float ReadingLineSpacing = -6f; // Pulls lines to about 24 pixels apart, the same as the pixel font
    static readonly Color32 Ink = new Color32(31, 26, 36, 255); // Dark outline colour for text
    static readonly Color32 Warning = new Color32(160, 40, 40, 255); // Red for "name not accepted" messages
    const float PPU = 32f; // Sprite pixels per Unity unit, same as the start screen
    static readonly Vector2 CanvasCenter = new Vector2(240f, 135f); // Middle of the 480x270 screen

    [MenuItem("LandGrand/Build Main Menu Scene")]
    public static void Build() // Builds and saves the whole Main Menu scene
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene

        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        var font = EnsureFont(); // The pixel font at 2x (decision D-050)
        if (font == null) return; // EnsureFont already said what is wrong

        ConfigureImporters(); // Make every Main Menu PNG crisp pixel art
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene

        // Camera: the same 480x270 pixel-perfect view as the start screen.
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" }; // Camera object, tagged as the main camera
        var cam = camGo.AddComponent<Camera>(); // The camera itself
        cam.orthographic = true; // Flat 2D view with no perspective
        cam.orthographicSize = 270f / 2f / PPU; // Shows exactly 270 pixels from top to bottom
        cam.transform.position = new Vector3(0f, 0f, -10f); // Stand back from the sprites so it can see them
        cam.clearFlags = CameraClearFlags.SolidColor; // Fill empty space with one colour
        cam.backgroundColor = new Color32(191, 232, 247, 255); // Light sky colour
        cam.allowMSAA = false; // No anti-aliasing: it blurs pixel edges
        cam.allowHDR = false; // No HDR: pixel art needs exact palette colours
        var ppc = camGo.AddComponent<PixelPerfectCamera>(); // Keeps pixels square and sharp
        ppc.assetsPPU = (int)PPU; // Sprites use 32 pixels per unit
        ppc.refResolutionX = 480; // Screen is 480 pixels wide
        ppc.refResolutionY = 270; // Screen is 270 pixels tall
        ppc.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture; // Draw small, then scale up evenly
        ppc.cropFrame = PixelPerfectCamera.CropFrame.None; // Fill the whole phone screen
        camGo.AddComponent<AudioListener>(); // Lets the player hear sounds

        CampBackground(); // The base camp in golden afternoon light (D-054)

        // Birds that sometimes fly across the sky.
        var birdFrames = StartFrames("Birds"); // Wing frames
        var bird = Renderer("Bird", birdFrames.FirstOrDefault(), 15, null).gameObject; // Hidden bird the flock copies
        var birdBook = bird.AddComponent<SpriteFlipbook>(); // Flaps the wings
        birdBook.frames = birdFrames; // Wing frames
        birdBook.framesPerSecond = 8f; // Flap speed
        bird.SetActive(false); // Keep the template hidden
        new GameObject("Birds").AddComponent<BirdFlock>().birdTemplate = bird; // Sends flocks of birds now and then

        // No title here: the Start Screen shows the LAND GRAND title, so the menu stays clean (D-054).

        // Music: the calm menu loop (a quieter relative of Quest) and the wind-and-birds ambience.
        var music = new GameObject("Music").AddComponent<StartScreenMusic>(); // Music player
        music.music = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "bgm_mainmenu.ogg"); // Calm menu music, made by Tools/Audio/make_menu_music.py
        music.ambience = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "amb_wind_birds.ogg"); // Wind and birds
        music.musicVolume = 0.40f; // Base music loudness: about 2 dB softer than Quest on the start screen
        music.ambienceVolume = 0.30f; // Base ambience loudness, a little lower so the menu stays calm
        if (music.music == null) Debug.LogError($"Missing {AudioFolder}bgm_mainmenu.ogg. Run Tools/Audio/make_menu_music.py."); // Warn if the music file is missing
        if (music.ambience == null) Debug.LogError($"Missing {AudioFolder}amb_wind_birds.ogg"); // Warn if the ambience file is missing

        // Event system: sends taps and clicks to the buttons.
        var events = new GameObject("EventSystem"); // Object that handles UI input
        events.AddComponent<EventSystem>(); // Decides which button was tapped
        events.AddComponent<InputSystemUIInputModule>(); // Reads taps from the new Input System

        // Canvas: holds all buttons and panels, scaled from 480x270 to the phone screen.
        var canvasGo = new GameObject("Menu UI", typeof(RectTransform)); // UI root object
        var canvas = canvasGo.AddComponent<Canvas>(); // The UI canvas
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Draw on top of the game view
        canvas.pixelPerfect = true; // Snap UI pixels to screen pixels
        var scaler = canvasGo.AddComponent<CanvasScaler>(); // Sets the size of the UI on the screen
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; // Size is a whole-number zoom, set below
        scaler.scaleFactor = 4f; // 1920x1080 zoom until the game starts and the real zoom is known
        scaler.referencePixelsPerUnit = PPU; // One sprite pixel equals one UI pixel
        canvasGo.AddComponent<PixelPerfectUiScale>().pixelCamera = ppc; // UI zoom always matches the art zoom, so pixel letters stay even
        canvasGo.AddComponent<GraphicRaycaster>(); // Finds which UI picture was tapped
        var group = canvasGo.AddComponent<CanvasGroup>(); // Lets the controller turn off every button at once
        var root = canvasGo.transform; // Parent for everything below

        // The base camp hub (D-054): the player badge top-left, Settings, Tutorial and Exit as named icon buttons top-right,
        // and one big PLAY at the bottom. All four LG-16 actions stay, large for small hands (LG-18).
        // They sit in one group that hides while a panel is open; the group is made before the dim layer, so panels cover it.
        var mainButtons = Rect("Main buttons", root, CanvasCenter, CanvasCenter); // Group that holds the badge and the buttons

        // Player badge (D-052, D-054): portrait, name, stars and found items. The whole card opens the Profile.
        var badgeC = new Vector2(81f, 28f); // Middle of the badge, x 6 to 156, y 6 to 50
        var badge = Panel("Player badge", mainButtons, badgeC, new Vector2(150f, 44f)); // Cream card
        var profile = badge.gameObject.AddComponent<Button>(); // The whole card is one button
        profile.transition = Selectable.Transition.None; // The card keeps its colour; a tint would turn it grey while the menu turns taps off during its entrance
        profile.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        Picture("Portrait", badge, S("Buttons/btn_profile"), new Vector2(28f, 28f), badgeC); // Round explorer portrait, x 12 to 44
        var profileName = ReadingLabel("Name", badge, "Explorer", new Vector2(100f, 18f), badgeC, new Vector2(100f, 30f), Ink, TextAlignmentOptions.MidlineLeft); // The name, x 50 to 150; shows "Explorer" in the Scene view, and the saved name once the game runs
        profileName.textWrappingMode = TextWrappingModes.NoWrap; // One line only
        profileName.overflowMode = TextOverflowModes.Ellipsis; // A long name ends in "..." inside the card; the box is 30 tall because a line taller than its box is cut away completely
        Picture("Star", badge, PixelSceneKit.Sprite("Assets/Art/Lesson/Sprites/", "icon_star", "Tools/Art/make_lesson_art.py"), new Vector2(56f, 39f), badgeC); // Star icon
        var profileStars = ReadingLabel("Stars", badge, "0", new Vector2(80f, 39f), badgeC, new Vector2(32f, 22f), Ink, TextAlignmentOptions.MidlineLeft); // Stars earned, x 64 to 96, in the same font as the name so the card looks even
        Picture("Gem", badge, PixelSceneKit.Sprite("Assets/Art/Level/Sprites/", "icon_gem", "Tools/Art/make_level_art.py"), new Vector2(102f, 39f), badgeC); // Rock-and-mineral icon
        var profileFound = ReadingLabel("Found", badge, "0/0", new Vector2(132f, 39f), badgeC, new Vector2(44f, 22f), Ink, TextAlignmentOptions.MidlineLeft); // Item kinds found out of all, x 110 to 154, same font

        // Icon buttons top-right, each with its name under it, 62 apart so "Settings" and "Tutorial" never touch.
        var settings = IconButton("Settings", mainButtons, "settings_icon", 330f); // Gear: opens Settings
        var tutorial = IconButton("Tutorial", mainButtons, "tutorial_icon", 392f); // Open book: opens Tutorial
        var exit = IconButton("Exit", mainButtons, "exit_icon", 454f); // Door: asks "Exit game?"

        // Big PLAY at the bottom, over the grass. It sits in a holder that bobs by one pixel, so it gently calls the player.
        var playC = new Vector2(240f, 236f); // Middle of PLAY, x 172 to 308, y 218 to 254
        var playHolder = Rect("Play holder", mainButtons, playC, CanvasCenter); // Bobs; PLAY itself slides in on entry
        var bob = playHolder.gameObject.AddComponent<PixelBob>(); // Up and down by one pixel
        bob.pixelsPerUnit = 1f; // UI units are pixels
        bob.periodSeconds = 1.6f; // A little quicker than the old title bob
        var play = MakeButton("Play", playHolder, "play_big", playC, playC); // Big green PLAY with a play triangle

        // Dark layer behind open panels. It also blocks taps on what is behind it.
        var dim = Rect("Dim", root, CanvasCenter, CanvasCenter); // Full-screen dark layer
        dim.anchorMin = Vector2.zero; // Stretch from the bottom-left corner...
        dim.anchorMax = Vector2.one; // ...to the top-right corner
        dim.offsetMin = Vector2.zero; // No gap at the bottom-left
        dim.offsetMax = Vector2.zero; // No gap at the top-right
        dim.gameObject.AddComponent<Image>().color = new Color32(31, 26, 36, 140); // See-through dark outline colour

        // Settings panel: music and sound-effects volume. The name is changed in the Profile (D-052).
        var setC = new Vector2(240f, 164f); // Middle of the Settings panel
        var setPanel = Panel("Settings panel", root, setC, new Vector2(330f, 188f)); // Panel background
        Picture("Heading", setPanel, S("Text/heading_settings"), new Vector2(240f, 88f), setC); // "SETTINGS"
        Picture("Music label", setPanel, S("Text/label_music"), new Vector2(240f, 116f), setC); // "MUSIC"
        var musicSlider = MakeSlider("Music slider", setPanel, new Vector2(240f, 132f), setC); // Music volume slider
        Picture("Sound label", setPanel, S("Text/label_sfx"), new Vector2(240f, 158f), setC); // "SOUND EFFECTS"
        var sfxSlider = MakeSlider("Sound slider", setPanel, new Vector2(240f, 174f), setC); // Sound-effects volume slider
        var setBack = MakeButton("Back", setPanel, "back_icon", new Vector2(97f, 88f), setC); // Back arrow icon, top-left corner: closes Settings without saving
        var setSave = MakeButton("Save", setPanel, "save", new Vector2(369f, 240f), setC); // Save button, bottom-right corner: keeps the changes and closes Settings
        var settingsPanel = setPanel.gameObject.AddComponent<SettingsPanel>(); // Connects the sliders to the saved volumes
        settingsPanel.musicSlider = musicSlider; // Music slider
        settingsPanel.sfxSlider = sfxSlider; // Sound slider
        settingsPanel.saveButton = setSave; // Save button

        // Name panel: what Play shows the first time, before the Landform Map.
        var nameC = new Vector2(240f, 140f); // Middle of the name panel
        var namePanelRect = Panel("Name panel", root, nameC, new Vector2(300f, 130f)); // Panel background, wide enough for the longest name message at 2x
        Label("Heading", namePanelRect, font, "What is your name?", new Vector2(240f, 100f), nameC, new Vector2(260f, 16f), Ink, TextAlignmentOptions.Midline); // Question
        var typedName = InputBox("Name box", namePanelRect, font, "Type here", new Vector2(240f, 128f), nameC, new Vector2(160f, 24f)); // Where the player types
        var typedMessage = ReadingLabel("Message", namePanelRect, "", new Vector2(240f, 150f), nameC, new Vector2(284f, 22f), Warning, TextAlignmentOptions.Midline); // Why a name was refused, in the reading font
        var nameOk = MakeButton("OK", namePanelRect, "ok", new Vector2(204f, 178f), nameC); // Saves the name
        var nameBack = MakeButton("Back", namePanelRect, "back", new Vector2(276f, 178f), nameC); // Closes the name panel
        var namePanelLogic = namePanelRect.gameObject.AddComponent<NamePanel>(); // Checks and saves the typed name
        namePanelLogic.input = typedName; // Typing box
        namePanelLogic.message = typedMessage; // Message line
        namePanelLogic.okButton = nameOk; // OK button

        // Tutorial panel: four draft picture pages (waiting on open questions Q-02 and Q-06).
        var tutC = new Vector2(240f, 164f); // Middle of the Tutorial panel
        // Layout (decision D-051): it covers the logo, so nothing peeks out above it. Rows from the top:
        // Back icon, heading and page number (y 26); page title (y 54); picture (y 68 to 148); sentences (y 156 to 228);
        // Prev arrow, draft note and Next arrow (y 246).
        tutC = new Vector2(240f, 135f); // Middle of the Tutorial panel
        var tutPanel = Panel("Tutorial panel", root, tutC, new Vector2(464f, 254f)); // Panel background, x 8 to 472, y 8 to 262
        var tutClay = new Color32(154, 85, 50, 255); // Clay brown for the page title and number
        var tutRed = new Color32(168, 63, 36, 255); // Scarf red so the draft note stands out
        var tutBack = MakeButton("Back", tutPanel, "back_icon", new Vector2(30f, 26f), tutC); // Back arrow icon, top-left corner, closes Tutorial
        Label("Heading", tutPanel, font, "TUTORIAL", new Vector2(240f, 26f), tutC, new Vector2(200f, 22f), Ink, TextAlignmentOptions.Midline); // Heading, top centre, pixel font
        var pageCounter = ReadingLabel("Page number", tutPanel, "", new Vector2(410f, 26f), tutC, new Vector2(100f, 22f), tutClay, TextAlignmentOptions.MidlineRight); // "1/4", top right, ending at x 460
        var pageTitle = ReadingLabel("Page title", tutPanel, "", new Vector2(240f, 54f), tutC, new Vector2(440f, 22f), tutClay, TextAlignmentOptions.Midline); // "Move and jump", centred over the picture
        var pagePicture = Picture("Page picture", tutPanel, S("Tutorial/page_1_picture"), new Vector2(240f, 108f), tutC); // Page picture, 200x80, y 68 to 148
        var pageText = ReadingLabel("Page text", tutPanel, "", new Vector2(240f, 192f), tutC, new Vector2(440f, 72f), Ink, TextAlignmentOptions.Top); // Up to three 24-pixel lines, y 156 to 228
        var prev = MakeButton("Prev", tutPanel, "prev_icon", new Vector2(30f, 246f), tutC); // Left arrow icon, bottom-left corner
        ReadingLabel("Draft note", tutPanel, "Draft: game rules are not final yet", new Vector2(240f, 246f), tutC, new Vector2(360f, 22f), tutRed, TextAlignmentOptions.Midline); // Draft note, bottom centre between the arrows
        var next = MakeButton("Next", tutPanel, "next_icon", new Vector2(450f, 246f), tutC); // Right arrow icon, bottom-right corner
        var tutorialPanel = tutPanel.gameObject.AddComponent<TutorialPanel>(); // Flips the pages
        tutorialPanel.picture = pagePicture; // Where pictures show
        tutorialPanel.counter = pageCounter; // Where the page number shows
        tutorialPanel.title = pageTitle; // Where titles show
        tutorialPanel.text = pageText; // Where sentences show
        tutorialPanel.pictures = Enumerable.Range(1, 4).Select(i => S($"Tutorial/page_{i}_picture")).ToArray(); // Pictures for pages 1 to 4
        tutorialPanel.titles = new[] { "Move and jump", "Collect", "Earn stars", "Watch out" }; // Titles for pages 1 to 4
        tutorialPanel.texts = new[] // Sentences for pages 1 to 4 (draft wording, as before)
        {
            "Tap the arrows to walk left or right.\nTap the jump button to jump.\nTap again in the air to double jump.", // Page 1
            "Walk into a rock or mineral to pick it up.\nRead its name and what it is.", // Page 2
            "Reach the end to finish the level.\nSee what you found and your stars.", // Page 3
            "Jump over pits and traps.\nFinish before the timer runs out.", // Page 4
        }; // End of the sentences
        tutorialPanel.prevButton = prev; // Prev button
        tutorialPanel.nextButton = next; // Next button

        // Coming Soon panel: what Play shows until the Landform Map scene exists.
        var soonC = new Vector2(240f, 150f); // Middle of the Coming Soon panel
        var soonPanel = Panel("Coming soon panel", root, soonC, new Vector2(260f, 110f)); // Panel background
        Picture("Heading", soonPanel, S("Text/heading_map"), new Vector2(240f, 118f), soonC); // "LANDFORM MAP"
        Picture("Message", soonPanel, S("Text/text_coming_soon"), new Vector2(240f, 145f), soonC); // "COMING SOON!" message
        var soonBack = MakeButton("Back", soonPanel, "back", new Vector2(240f, 180f), soonC); // Closes the message

        // Exit panel: "Exit game?" with Yes and No.
        var exitC = new Vector2(240f, 150f); // Middle of the Exit panel
        var exitPanel = Panel("Exit panel", root, exitC, new Vector2(240f, 96f)); // Panel background
        Picture("Heading", exitPanel, S("Text/heading_exit"), new Vector2(240f, 128f), exitC); // "EXIT GAME?"
        var yes = MakeButton("Yes", exitPanel, "yes", new Vector2(204f, 170f), exitC); // Quits the game
        var no = MakeButton("No", exitPanel, "no", new Vector2(276f, 170f), exitC); // Stays in the game

        // Controller: connects every button to what it does.
        var controller = new GameObject("Main Menu").AddComponent<MainMenuController>(); // The menu brain
        controller.playButton = play; // Play
        controller.settingsButton = settings; // Settings
        controller.tutorialButton = tutorial; // Tutorial
        controller.exitButton = exit; // Exit
        controller.profileButton = profile; // Player badge, opens the Profile
        controller.profileName = profileName; // Name on the badge
        controller.profileStars = profileStars; // Stars on the badge
        controller.profileFound = profileFound; // Found items on the badge
        controller.mainButtons = mainButtons.gameObject; // Group of the four buttons
        controller.dim = dim.gameObject; // Dark layer
        controller.settingsPanel = setPanel.gameObject; // Settings panel
        controller.tutorialPanel = tutPanel.gameObject; // Tutorial panel
        controller.namePanel = namePanelLogic; // Name panel
        controller.comingSoonPanel = soonPanel.gameObject; // Coming Soon panel
        controller.exitPanel = exitPanel.gameObject; // Exit panel
        controller.backButtons = new[] { setBack, tutBack, soonBack, nameBack }; // All Back buttons
        controller.exitYesButton = yes; // Yes
        controller.exitNoButton = no; // No
        controller.music = music; // Music player, so it can fade out
        controller.clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "sfx_ui_click.wav"); // Button click sound
        controller.ui = group; // Whole UI, so taps can be turned off while leaving
        if (controller.clickSound == null) Debug.LogError($"Missing {AudioFolder}sfx_ui_click.wav"); // Warn if the click sound is missing

        // Panels start closed; the controller opens them.
        dim.gameObject.SetActive(false); // Dark layer hidden
        setPanel.gameObject.SetActive(false); // Settings hidden
        tutPanel.gameObject.SetActive(false); // Tutorial hidden
        namePanelRect.gameObject.SetActive(false); // Name panel hidden
        soonPanel.gameObject.SetActive(false); // Coming Soon hidden
        exitPanel.gameObject.SetActive(false); // Exit hidden

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); // Make the Scenes folder if it is missing
        PixelSceneKit.FixFonts(); // Right pixel font on every text, never TextMeshPro's smooth default font
        EditorSceneManager.SaveScene(scene, ScenePath); // Save the new scene
        AddSceneToBuildSecond(ScenePath); // Put it right after the start screen in the build list
        Debug.Log($"Main Menu built: {ScenePath}. Open StartScreen and press Play, then tap to reach the menu."); // Tell the user it worked
    }

    /// <summary>
    /// The base camp behind the Main Menu and the Profile (D-054): a still golden sky with a low sun, slowly drifting clouds,
    /// snowy peaks, a pine band and the clearing with the tent and the map board. The campfire flickers, sparks rise
    /// and the explorer breathes. Drawn by Tools/Art/make_menu_camp.py; it no longer reuses the start screen's landforms.
    /// </summary>
    internal static void CampBackground() // Makes the camp layers and its small animations
    {
        var camp = new GameObject("Base camp").transform; // Parent for everything in the camp
        Still("Sky", "C0_sky", 0, camp); // Sky and sun
        var clouds = new GameObject("Clouds").AddComponent<ParallaxLayer>(); // Drifts the clouds sideways
        clouds.transform.SetParent(camp, false); // Keep it in the camp
        clouds.speedPixelsPerSecond = 1.5f; // Slow drift
        clouds.followsWalk = false; // Clouds keep drifting on their own
        clouds.widthPixels = 960f; // The cloud strip is two screens wide and loops
        clouds.pixelsPerUnit = PPU; // Same pixel size as the sprites
        Renderer("Art", S("Camp/C1_clouds"), 10, clouds.transform); // The cloud picture
        Still("Peaks", "C2_peaks", 20, camp); // Snowy peaks
        Still("Forest", "C3_forest", 30, camp); // Pine band
        Still("Camp", "C4_camp", 40, camp); // Clearing, tent, map board, log and fire pit
        Animated("Campfire", "fire_", 4, 8f, new Vector2(172f, 188f), 50, camp); // Flames on the fire pit (logs at y 197)
        Animated("Sparks", "embers_", 6, 8f, new Vector2(172f, 162f), 51, camp); // Sparks rising above the flames
        Animated("Explorer", "explorer_stand_", 2, 1.5f, new Vector2(118f, 180f), 52, camp); // The start-screen explorer standing on the grass a little away from the fire (feet at y 202), facing it, breathing slowly
        Animated("Flag", "flag_", 3, 4f, new Vector2(405f, 146f), 45, camp); // Pennant waving on the tent pole
        Animated("Summit flag", "flag_", 3, 3f, new Vector2(166f, 98f), 21, camp); // A flag on the top of the left peak, below the icon labels: where the adventure leads
        Animated("Lantern", "lantern_", 2, 3f, new Vector2(450f, 178f), 46, camp); // A lantern hanging by the tent, flickering
        Animated("Glint", "glint_", 6, 5f, new Vector2(353f, 184f), 46, camp); // The quartz in the rock crate twinkles now and then
        Flier("Butterfly", "Camp/butterfly_", "Assets/Art/MainMenu/Sprites/", new Vector2(262f, 208f), new Vector2(26f, 6f), 9f, 53, camp); // A butterfly over the grass
        Flier("Butterfly 2", "Camp/butterfly_", "Assets/Art/MainMenu/Sprites/", new Vector2(70f, 222f), new Vector2(16f, 5f), 7f, 53, camp); // Another, near the map board
        Flier("Eagle", "Mountains/eagle_", "Assets/Art/LandformMap/Sprites/", new Vector2(300f, 68f), new Vector2(54f, 10f), 14f, 25, camp); // An eagle circling the big peak
        var fill = new GameObject("Below-ground fill").AddComponent<ScreenFill>(); // Fills space under the ground on tall screens
        fill.topY = -135f / PPU; // Starts at the bottom edge of the art
    }

    static void Still(string name, string sprite, int order, Transform parent) // One still 480x270 camp layer, centred on the screen
    {
        Renderer(name, S("Camp/" + sprite), order, parent).transform.position = Px(240f, 135f); // Its middle is the middle of the screen
    }

    static void Flier(string name, string prefix, string folder, Vector2 center, Vector2 radius, float lap, int order, Transform parent) // A two-frame flier that circles an oval
    {
        var frames = new[] { PixelSceneKit.Sprite(folder, prefix + "0", "Tools/Art/make_menu_camp.py"), PixelSceneKit.Sprite(folder, prefix + "1", "Tools/Art/make_menu_camp.py") }; // Wings up, wings down
        var sr = Renderer(name, frames[0], order, parent); // The sprite
        var book = sr.gameObject.AddComponent<SpriteFlipbook>(); // Flaps the wings
        book.frames = frames; // Both frames
        book.framesPerSecond = 4f; // Flap speed
        var flight = sr.gameObject.AddComponent<CircleFlight>(); // Moves it round an oval, on whole pixels
        flight.centerPixels = center; // Middle of the oval, in screen pixels
        flight.radiusPixels = radius; // Half width and half height
        flight.secondsPerLap = lap; // Time for one lap
    }

    static void Animated(string name, string prefix, int count, float fps, Vector2 at, int order, Transform parent) // A small looping camp animation
    {
        var frames = Enumerable.Range(0, count).Select(i => S("Camp/" + prefix + i)).ToArray(); // Its frames, in order
        var sr = Renderer(name, frames[0], order, parent); // The sprite
        sr.transform.position = Px(at.x, at.y); // Its place on the screen
        var book = sr.gameObject.AddComponent<SpriteFlipbook>(); // Plays the frames
        book.frames = frames; // All frames
        book.framesPerSecond = fps; // How fast
    }

    // Screen pixel (480x270, y down from the top) to world position.
    internal static Vector3 Px(float x, float y) => new Vector3((x - 240f) / PPU, (135f - y) / PPU, 0f); // Centre of the screen is world (0, 0)

    internal static SpriteRenderer Renderer(string name, Sprite sprite, int order, Transform parent) // Makes a world sprite (not UI)
    {
        var go = new GameObject(name); // New object for the sprite
        if (parent != null) go.transform.SetParent(parent, false); // Put it under its parent if one is given
        var sr = go.AddComponent<SpriteRenderer>(); // Draws the sprite in the world
        sr.sprite = sprite; // Which picture to draw
        sr.sortingOrder = order; // Higher numbers draw in front
        return sr;
    }

    /// <summary>Makes a UI object centred at a screen pixel, placed inside a parent whose centre is parentCenter.</summary>
    internal static RectTransform Rect(string name, Transform parent, Vector2 center, Vector2 parentCenter) // Makes a positioned UI box
    {
        var go = new GameObject(name, typeof(RectTransform)); // New UI object
        var rt = (RectTransform)go.transform; // Its UI position box
        rt.SetParent(parent, false); // Put it under its parent
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); // Measure from the parent's middle
        rt.pivot = new Vector2(0.5f, 0.5f); // Position means the object's middle
        rt.anchoredPosition = new Vector2(center.x - parentCenter.x, parentCenter.y - center.y); // Screen pixels to UI offset (UI y goes up)
        return rt;
    }

    /// <summary>A UI picture shown at its exact pixel size. It does not catch taps.</summary>
    internal static Image Picture(string name, Transform parent, Sprite sprite, Vector2 center, Vector2 parentCenter) // Makes a UI picture
    {
        var rt = Rect(name, parent, center, parentCenter); // Place it
        var img = rt.gameObject.AddComponent<Image>(); // UI picture
        img.sprite = sprite; // Which picture to show
        img.raycastTarget = false; // Taps pass through it
        if (sprite != null) rt.sizeDelta = sprite.rect.size; // One sprite pixel = one UI pixel
        if (sprite != null) rt.anchoredPosition += new Vector2(sprite.rect.width % 2f, sprite.rect.height % 2f) * 0.5f; // Odd sizes move half a pixel so their edges sit on whole pixels and stay sharp
        return img;
    }

    /// <summary>A button whose picture swaps to its "pressed" version while held down.</summary>
    internal static Button MakeButton(string name, Transform parent, string spriteName, Vector2 center, Vector2 parentCenter) // Makes a UI button
    {
        var img = Picture(name, parent, S("Buttons/btn_" + spriteName), center, parentCenter); // Normal button picture
        img.raycastTarget = true; // Buttons must catch taps
        var button = img.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        button.targetGraphic = img; // The picture that changes
        button.spriteState = new SpriteState { pressedSprite = S("Buttons/btn_" + spriteName + "_pressed") }; // Pushed-in picture
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        return button;
    }

    /// <summary>A 34x30 icon button in the top-right row with its name under it in the reading font (D-054). The name is a child, so it moves with the button.</summary>
    static Button IconButton(string name, Transform parent, string spriteName, float x) // Makes one named icon button
    {
        var c = new Vector2(x, 22f); // Middle of the icon, y 7 to 37
        var button = MakeButton(name, parent, spriteName, c, CanvasCenter); // The icon button
        var label = ReadingLabel("Label", button.transform, name, new Vector2(x, 47f), c, new Vector2(64f, 22f), Ink, TextAlignmentOptions.Midline); // Its name under it
        label.textWrappingMode = TextWrappingModes.NoWrap; // One line
        return button; // The controller connects it
    }

    /// <summary>A panel background stretched from the 24x24 panel sprite (9-slice).</summary>
    internal static RectTransform Panel(string name, Transform parent, Vector2 center, Vector2 size) // Makes a stretchable panel
    {
        var rt = Rect(name, parent, center, CanvasCenter); // Place it on the screen
        rt.sizeDelta = size; // Panel size in pixels
        var img = rt.gameObject.AddComponent<Image>(); // Panel picture
        img.sprite = S("Panels/panel"); // Cream paper with a tan frame
        img.type = Image.Type.Sliced; // Stretch the middle, keep the corners sharp
        img.raycastTarget = true; // Panels catch taps so nothing behind them is pressed
        return rt;
    }

    /// <summary>A 160x16 volume slider from 0 to 1 using the pixel track, fill and knob.</summary>
    static Slider MakeSlider(string name, Transform parent, Vector2 center, Vector2 parentCenter) // Makes a volume slider
    {
        var resources = new DefaultControls.Resources // Pictures for the slider parts
        {
            background = S("Panels/slider_track"), // Empty bar
            standard = S("Panels/slider_fill"), // Filled part of the bar
            knob = S("Panels/slider_handle"), // Knob the player drags
        };
        var go = DefaultControls.CreateSlider(resources); // Unity's standard slider layout
        go.name = name; // Name it
        var rt = (RectTransform)go.transform; // Its UI position box
        rt.SetParent(parent, false); // Put it under the panel
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); // Measure from the panel's middle
        rt.anchoredPosition = new Vector2(center.x - parentCenter.x, parentCenter.y - center.y); // Place it
        rt.sizeDelta = new Vector2(160f, 16f); // Slider size in pixels

        var fillArea = (RectTransform)go.transform.Find("Fill Area"); // Area the fill grows in
        fillArea.offsetMin = Vector2.zero; // Fill starts at the bar's left end
        fillArea.offsetMax = Vector2.zero; // Fill can reach the bar's right end
        ((RectTransform)fillArea.Find("Fill")).sizeDelta = Vector2.zero; // Fill is exactly as wide as the value
        var handleArea = (RectTransform)go.transform.Find("Handle Slide Area"); // Area the knob moves in
        handleArea.offsetMin = new Vector2(5f, 0f); // Knob stops 5 pixels from the left end
        handleArea.offsetMax = new Vector2(-5f, 0f); // Knob stops 5 pixels from the right end
        ((RectTransform)handleArea.Find("Handle")).sizeDelta = new Vector2(10f, 0f); // Knob is 10 pixels wide and as tall as the slider

        var slider = go.GetComponent<Slider>(); // The slider logic
        slider.minValue = 0f; // Silent
        slider.maxValue = 1f; // Full volume
        slider.value = 1f; // Start full; the Settings panel shows the saved value when it opens
        slider.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving
        return slider;
    }

    /// <summary>The pixel font at 2x (22 px), for all game text. Made once; later runs reuse it.</summary>
    internal static TMP_FontAsset EnsureFont() => MakeFont(FontAssetPath, FontSize, "Micro5 2x"); // Gives the pixel font

    /// <summary>The Andika reading font, for sentences and descriptions (decision D-051). Made once; later runs reuse it.</summary>
    internal static TMP_FontAsset EnsureReadingFont() // Gives the reading font
    {
        var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ReadingFontPath); // Already made on an earlier run?
        if (existing != null) return existing; // Reuse it
        var source = AssetDatabase.LoadAssetAtPath<Font>(ReadingFontFile); // The .ttf file
        if (source == null) // Unity may not have seen a newly added font file yet
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); // Look again; only here, because a refresh can make fonts loaded earlier in a build go stale
            source = AssetDatabase.LoadAssetAtPath<Font>(ReadingFontFile); // Try once more
        }
        if (source == null) { Debug.LogError($"Missing font {ReadingFontFile}. Add Andika (SIL OFL) there."); return null; } // Say what is missing
        var asset = TMP_FontAsset.CreateFontAsset(source, 90, 9, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true); // Smooth letters that stay sharp at any zoom, new letters added on demand
        asset.TryAddCharacters(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~—", out _); // Draw the basic letters now, including the long dash
        AssetDatabase.CreateAsset(asset, ReadingFontPath); // Save the font asset
        asset.material.name = "Andika SDF Material"; // Name its material
        AssetDatabase.AddObjectToAsset(asset.material, asset); // Keep the material inside the font asset
        foreach (var atlas in asset.atlasTextures) // Each letter-picture sheet
        {
            atlas.name = "Andika SDF Atlas"; // Name the sheet; it keeps smooth filtering, which distance-field letters need
            AssetDatabase.AddObjectToAsset(atlas, asset); // Keep the sheet inside the font asset
        }
        EditorUtility.SetDirty(asset); // Mark it as changed
        AssetDatabase.SaveAssets(); // Write it to disk
        return asset; // Give the font back
    }

    /// <summary>A line or paragraph of easy-to-read Andika text for sentences and descriptions (decision D-051). It does not catch taps.</summary>
    internal static TextMeshProUGUI ReadingLabel(string name, Transform parent, string text, Vector2 center, Vector2 parentCenter, Vector2 size, Color color, TextAlignmentOptions align) // Makes a reading-text label
    {
        var label = Label(name, parent, EnsureReadingFont(), text, center, parentCenter, size, color, align); // Same placing as the pixel labels
        label.fontSize = ReadingFontSize; // Reading size
        label.lineSpacing = ReadingLineSpacing; // Lines about 24 pixels apart
        return label; // Give the label back
    }

    static TMP_FontAsset MakeFont(string path, int size, string name) // Makes a TextMeshPro font from the pixel font file at one sharp size, once
    {
        var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path); // Already made on an earlier run?
        if (existing != null) return existing; // Reuse it
        var source = AssetDatabase.LoadAssetAtPath<Font>(FontFile); // The .ttf file
        if (source == null) // Unity may not have seen a newly added font file yet
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); // Look again; only here, because a refresh can make fonts loaded earlier in a build go stale
            source = AssetDatabase.LoadAssetAtPath<Font>(FontFile); // Try once more
        }
        if (source == null) { Debug.LogError($"Missing font {FontFile}. Add Micro 5 (SIL OFL) there."); return null; } // Say what is missing
        var asset = TMP_FontAsset.CreateFontAsset(source, size, 2, UnityEngine.TextCore.LowLevel.GlyphRenderMode.RASTER_HINTED, 256, 256, AtlasPopulationMode.Dynamic, false); // Hard-edged letters drawn at exactly this size, new letters added on demand
        asset.TryAddCharacters(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~—", out _); // Draw the basic letters now, including the long dash
        AssetDatabase.CreateAsset(asset, path); // Save the font asset
        asset.material.name = name + " Material"; // Name its material
        AssetDatabase.AddObjectToAsset(asset.material, asset); // Keep the material inside the font asset
        foreach (var atlas in asset.atlasTextures) // Each letter-picture sheet
        {
            atlas.name = name + " Atlas"; // Name the sheet
            atlas.filterMode = FilterMode.Point; // No blur between pixels
            AssetDatabase.AddObjectToAsset(atlas, asset); // Keep the sheet inside the font asset
        }
        EditorUtility.SetDirty(asset); // Mark it as changed
        AssetDatabase.SaveAssets(); // Write it to disk
        return asset; // Give the font back
    }

    /// <summary>A line of pixel-font text that does not catch taps.</summary>
    internal static TextMeshProUGUI Label(string name, Transform parent, TMP_FontAsset font, string text, Vector2 center, Vector2 parentCenter, Vector2 size, Color color, TextAlignmentOptions align) // Makes a text label
    {
        var rt = Rect(name, parent, center, parentCenter); // Place it
        rt.sizeDelta = size; // Room for the text
        var label = rt.gameObject.AddComponent<TextMeshProUGUI>(); // The text itself
        label.font = font != null ? font : EnsureFont(); // Pixel font; if the given one went stale after an asset refresh, load it again so TextMeshPro never falls back to its smooth default font
        label.fontSize = FontSize; // Exactly the size the font picture was drawn at, so it stays sharp
        label.text = text; // Words to show
        label.color = color; // Text colour
        label.alignment = align; // Where the words sit in the box
        label.textWrappingMode = TextWrappingModes.NoWrap; // Keep each message on one line
        label.raycastTarget = false; // Taps pass through it
        return label; // Give the label back
    }

    /// <summary>A one-line typing box. The phone's keyboard opens when it is tapped.</summary>
    internal static TMP_InputField InputBox(string name, Transform parent, TMP_FontAsset font, string hint, Vector2 center, Vector2 parentCenter, Vector2 size) // Makes a typing box
    {
        var rt = Rect(name, parent, center, parentCenter); // Place it
        rt.sizeDelta = size; // Box size in pixels
        var back = rt.gameObject.AddComponent<Image>(); // Box background
        back.color = new Color32(247, 238, 214, 255); // Light paper colour
        var field = rt.gameObject.AddComponent<TMP_InputField>(); // The typing logic
        field.targetGraphic = back; // The part that reacts to taps
        field.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving

        var area = new GameObject("Text Area", typeof(RectTransform)); // Holds the text and cuts off what does not fit
        var areaRt = (RectTransform)area.transform; // Its UI position box
        areaRt.SetParent(rt, false); // Put it inside the box
        areaRt.anchorMin = Vector2.zero; // Stretch from the bottom-left...
        areaRt.anchorMax = Vector2.one; // ...to the top-right
        areaRt.offsetMin = new Vector2(4f, 2f); // Small gap at the bottom-left
        areaRt.offsetMax = new Vector2(-4f, -2f); // Small gap at the top-right
        area.AddComponent<RectMask2D>(); // Hide text that would leave the box

        var typed = Label("Text", areaRt, font, "", Vector2.zero, Vector2.zero, Vector2.zero, Ink, TextAlignmentOptions.MidlineLeft); // The typed letters
        Stretch((RectTransform)typed.transform); // Fill the text area
        var hintText = Label("Placeholder", areaRt, font, hint, Vector2.zero, Vector2.zero, Vector2.zero, new Color32(140, 130, 110, 255), TextAlignmentOptions.MidlineLeft); // Grey hint shown while empty
        Stretch((RectTransform)hintText.transform); // Fill the text area

        field.textViewport = areaRt; // The area that clips the text
        field.textComponent = typed; // Where typed letters show
        field.placeholder = hintText; // Hint shown while empty
        field.lineType = TMP_InputField.LineType.SingleLine; // One line only
        field.contentType = TMP_InputField.ContentType.Standard; // Any text; NameRule checks it
        return field; // Give the typing box back
    }

    internal static void Stretch(RectTransform rt) // Makes a UI box fill its parent
    {
        rt.anchorMin = Vector2.zero; // Bottom-left corner of the parent
        rt.anchorMax = Vector2.one; // Top-right corner of the parent
        rt.offsetMin = Vector2.zero; // No gap at the bottom-left
        rt.offsetMax = Vector2.zero; // No gap at the top-right
    }

    /// <summary>Loads a Main Menu sprite by its path under Sprites/, without ".png".</summary>
    internal static Sprite S(string path) // Loads one Main Menu sprite
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpriteFolder + path + ".png"); // Load the PNG as a sprite
        if (sprite == null) Debug.LogError($"Missing sprite {SpriteFolder}{path}.png. Run Tools/Art/make_mainmenu_art.py."); // Say which file is missing
        return sprite;
    }

    /// <summary>All start-screen sprites in one folder, sorted by file name (frame order).</summary>
    static Sprite[] StartFrames(string folder) // Loads a folder of start-screen sprites
    {
        var dir = StartSprites + folder; // Folder to read
        if (!Directory.Exists(dir)) { Debug.LogError($"Missing folder {dir}"); return new Sprite[0]; } // Stop if it is missing
        return Directory.GetFiles(dir, "*.png").Select(p => p.Replace('\\', '/')).OrderBy(p => p) // PNG paths in name order
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p)).Where(sp => sp != null).ToArray(); // Load each as a sprite
    }

    /// <summary>Pixel-art import settings for every PNG under the Main Menu Sprites folder.</summary>
    static void ConfigureImporters() => ConfigurePixelSprites(SpriteFolder, _ => false); // Main Menu PNGs; none need to be readable

    /// <summary>Pixel-art import settings for every PNG under a folder. readable picks the PNGs whose pixels scripts must read (tap areas).</summary>
    internal static void ConfigurePixelSprites(string folder, System.Func<string, bool> readable) // Sets import options on a folder of PNGs
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); // Make Unity see newly drawn PNGs and sounds first
        foreach (var file in Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories)) // Every PNG in the folder
        {
            var path = file.Replace('\\', '/'); // Unity paths use forward slashes
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) continue; // Skip anything that is not a picture
            importer.textureType = TextureImporterType.Sprite; // Use it as a sprite
            importer.spriteImportMode = SpriteImportMode.Single; // One sprite per file
            importer.spritePixelsPerUnit = PPU; // 32 pixels per unit
            importer.filterMode = FilterMode.Point; // No blur between pixels
            importer.mipmapEnabled = false; // No smaller copies; not needed for 2D UI
            importer.alphaIsTransparency = true; // Clean see-through edges
            importer.npotScale = TextureImporterNPOTScale.None; // Keep the exact size
            importer.wrapMode = TextureWrapMode.Clamp; // Do not repeat the edges
            importer.textureCompression = TextureImporterCompression.Uncompressed; // Keep exact palette colours
            importer.isReadable = readable(path); // Only tap-area pictures keep a readable copy in memory
            var settings = new TextureImporterSettings(); // Extra sprite settings
            importer.ReadTextureSettings(settings); // Start from the current ones
            settings.spriteAlignment = (int)SpriteAlignment.Center; // Pivot in the middle
            settings.spriteMeshType = SpriteMeshType.FullRect; // Simple rectangle shape
            settings.spriteBorder = Border(path); // Edge sizes for stretchable sprites (9-slice)
            importer.SetTextureSettings(settings); // Apply the extra settings
            importer.SaveAndReimport(); // Save and re-import the PNG
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); // Wait until every import is done
    }

    /// <summary>9-slice edges (left, bottom, right, top) for the stretchable panel and slider bars.</summary>
    static Vector4 Border(string path) // Gives the 9-slice edges for a sprite
    {
        if (path.EndsWith("/panel.png") || path.EndsWith("/card_open.png") || path.EndsWith("/card_locked.png")) return new Vector4(6f, 6f, 6f, 6f); // Panel and card frames are 6 pixels wide
        if (path.EndsWith("/slider_track.png") || path.EndsWith("/slider_fill.png")) return new Vector4(3f, 3f, 3f, 3f); // Bar ends are 3 pixels wide
        return Vector4.zero; // Other sprites do not stretch
    }

    static void AddSceneToBuildSecond(string path) // Adds the scene to the build list
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != path).ToList(); // Build list without this scene
        scenes.Insert(Mathf.Min(1, scenes.Count), new EditorBuildSettingsScene(path, true)); // Put it second, right after the start screen
        EditorBuildSettings.scenes = scenes.ToArray(); // Save the new build list
    }
}
