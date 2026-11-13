using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select
using LandGrand.LandformMap; // Gives access to the Mountains animations
using LandGrand.Rules; // Gives access to Difficulty
using LandGrand.Selection; // Gives access to the selection screen scripts
using TMPro; // Gives access to TextMeshPro text and fonts
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds the two selection screens of ticket 05 and adds them to the build list after the Landform Map:
/// Assets/Scenes/DifficultySelection.unity (Easy, Normal and Hard cards) and Assets/Scenes/LevelSelection.unity (the level trail).
/// Run it from the menu: LandGrand > Build Selection Scenes. Draw the art first with: python Tools/Art/make_selection_art.py
/// Screen positions below are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class SelectionBuilder // Makes the difficulty and level selection scenes
{
    const string SpriteFolder = "Assets/Art/Selection/Sprites/"; // Selection PNGs
    const string ArtScript = "Tools/Art/make_selection_art.py"; // Script that draws them
    const string MapSpriteFolder = "Assets/Art/LandformMap/Sprites/"; // The big padlock comes from the map art
    const string MapPath = "Assets/Scenes/LandformMap.unity"; // Difficulty selection goes after this scene
    const string DifficultyPath = "Assets/Scenes/DifficultySelection.unity"; // Where the difficulty scene is saved
    const string LevelPath = "Assets/Scenes/LevelSelection.unity"; // Where the level trail scene is saved
    static readonly string[] Landforms = { "mountains", "hills", "plains", "plateaus" }; // File names, in Episode order
    static readonly string[] Difficulties = { "easy", "normal", "hard" }; // File names, in Difficulty order

    [MenuItem("LandGrand/Build Selection Scenes")] // Adds the command to Unity's LandGrand menu
    public static void Build() // Builds and saves both selection scenes
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(SpriteFolder + "bg_mountains.png") || !File.Exists(MapSpriteFolder + "padlock.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python Tools/Art/make_landform_map_art.py and python {ArtScript}"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        var font = M.EnsureFont(); // The pixel font as a TextMeshPro font
        if (font == null) return; // EnsureFont already said what is wrong
        M.ConfigurePixelSprites(SpriteFolder, _ => false); // Crisp pixel art; nothing here needs to be readable
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        BuildDifficulty(font); // Easy, Normal and Hard cards
        BuildLevels(font); // The level trail
        Debug.Log($"Selection scenes built: {DifficultyPath} and {LevelPath}. Open StartScreen, press Play, and tap Mountains on the map."); // Tell the user it worked
    }

    static void BuildDifficulty(TMP_FontAsset font) // Makes the difficulty card scene
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var root = Backdrop("Difficulty UI", out var music, out var group); // Mountains behind, music and UI canvas
        var title = Title(root, S("Titles/title_mountains"), new Vector2(180f, 26f)); // Landform name at the top

        var cards = new DifficultySelectionController.Card[3]; // Filled below
        for (int i = 0; i < 3; i++) // One card per difficulty
        {
            var d = (Difficulty)i; // This card's difficulty
            var c = new Vector2(110f + i * 130f, 158f); // Middle of the card
            var panel = M.Panel("Card " + Difficulties[i], root, c, new Vector2(116f, 150f)); // Card background, stretched from the 24x24 panel
            var image = panel.GetComponent<Image>(); // Its picture
            image.sprite = S("Cards/card_open"); // Cream until the controller reads the progress
            var button = panel.gameObject.AddComponent<Button>(); // Makes the card tappable
            button.transition = Selectable.Transition.None; // The card's own picture shows open or locked
            button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between cards
            var heading = M.Picture("Heading", panel, S($"Cards/heading_{Difficulties[i]}"), c + new Vector2(0f, -50f), c); // "EASY", "NORMAL" or "HARD"
            M.Picture("Star", panel, S("Trail/star_full"), c + new Vector2(-16f, -24f), c); // Star icon
            var stars = M.Label("Stars", panel, font, "0/9", c + new Vector2(10f, -24f), c, new Vector2(40f, 14f), K.Ink, TextAlignmentOptions.MidlineLeft); // Star progress
            var padlock = M.Picture("Padlock", panel, K.Sprite(MapSpriteFolder, "padlock", "Tools/Art/make_landform_map_art.py"), c + new Vector2(0f, 10f), c); // Big padlock while locked
            var guide = M.Label("Guide", panel, font, "", c + new Vector2(0f, 46f), c, new Vector2(104f, 30f), K.Ink, TextAlignmentOptions.Center); // "Clear all Easy levels"
            guide.textWrappingMode = TextWrappingModes.Normal; // Long guides break onto two lines
            cards[i] = new DifficultySelectionController.Card // Everything the controller needs for this card
            {
                difficulty = d, button = button, panel = image, heading = heading, // Card, background and heading
                headingOpen = S($"Cards/heading_{Difficulties[i]}"), headingLocked = S($"Cards/heading_{Difficulties[i]}_locked"), // Heading pictures
                padlock = padlock.gameObject, stars = stars, guide = guide, // Padlock and texts
            };
        }

        var controller = new GameObject("Difficulty Selection").AddComponent<DifficultySelectionController>(); // The screen brain
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group
        controller.cards = cards; // The three cards
        controller.cardOpen = S("Cards/card_open"); // Cream card
        controller.cardLocked = S("Cards/card_locked"); // Grey card
        controller.title = title; // Title picture
        controller.titles = Landforms.Select(n => S($"Titles/title_{n}")).ToArray(); // One title per landform
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner (made last, so it is on top)
        Save(scene, DifficultyPath, MapPath); // Save and add to the build list after the map
    }

    static void BuildLevels(TMP_FontAsset font) // Makes the level trail scene
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var root = Backdrop("Level UI", out var music, out var group); // Mountains behind, music and UI canvas
        var title = Title(root, S("Titles/title_mountains_easy"), new Vector2(240f, 26f)); // "MOUNTAINS: EASY" at the top

        var dot = M.Picture("Trail dot", root, S("Trail/dot"), K.CanvasCenter, K.CanvasCenter); // One dash; the controller copies it along the trail
        var stones = new LevelSelectionController.Stone[7]; // Enough for Hard, the longest difficulty
        var c = new Vector2(240f, 150f); // Build every stone in the middle; the controller moves them into place
        for (int i = 0; i < stones.Length; i++) // One stone per level
        {
            var image = M.Picture("Level " + (i + 1), root, S("Trail/stone_open"), c, K.CanvasCenter); // Stone picture
            image.raycastTarget = true; // Stones catch taps
            var button = image.gameObject.AddComponent<Button>(); // Makes the stone tappable
            button.transition = Selectable.Transition.None; // The stone's own picture shows open or locked
            button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between stones
            var number = M.Label("Number", image.transform, font, (i + 1).ToString(), c + new Vector2(0f, -1f), c, new Vector2(26f, 14f), K.Ink, TextAlignmentOptions.Midline); // Level number
            var padlock = M.Picture("Padlock", image.transform, S("Trail/padlock_small"), c, c); // Small padlock while locked
            var stars = new Image[3]; // Three stars under the stone
            for (int s = 0; s < 3; s++) stars[s] = M.Picture("Star " + (s + 1), image.transform, S("Trail/star_empty"), c + new Vector2(-12f + s * 12f, 18f), c); // Stars side by side
            stones[i] = new LevelSelectionController.Stone { button = button, image = image, number = number, stars = stars, padlock = padlock.gameObject }; // Everything the controller needs
        }

        var controller = new GameObject("Level Selection").AddComponent<LevelSelectionController>(); // The screen brain
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group
        controller.stones = stones; // The seven stones
        controller.stoneOpen = S("Trail/stone_open"); // Light stone
        controller.stoneLocked = S("Trail/stone_locked"); // Dark stone
        controller.starFull = S("Trail/star_full"); // Earned star
        controller.starEmpty = S("Trail/star_empty"); // Star not earned yet
        controller.dotTemplate = dot; // Trail dash to copy
        controller.title = title; // Title picture
        controller.titles = Landforms.SelectMany(n => Difficulties.Select(d => S($"Titles/title_{n}_{d}"))).ToArray(); // Landform * 3 + difficulty
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner
        Save(scene, LevelPath, DifficultyPath); // Save and add to the build list after the difficulty screen
    }

    /// <summary>Camera, the full-screen Mountains picture with slow snow, cloud and eagle, the music and the UI canvas.</summary>
    static Transform Backdrop(string canvasName, out LandGrand.StartScreen.StartScreenMusic music, out CanvasGroup group) // Shared start of both scenes
    {
        var ppc = K.Camera(); // The same 480x270 pixel-perfect view as the other screens
        M.Renderer("Mountains", S("bg_mountains"), 0, null).transform.position = M.Px(240f, 135f); // Mountains fill the screen
        K.MountainsLife(out CloudDrift cloud, out CircleFlight eagle, out SnowFlurry snow); // Slow Mountains animations behind the cards
        cloud.startX = 20f; cloud.endX = 460f; cloud.y = 40f; cloud.speedPixelsPerSecond = 2.5f; // Cloud drifts slowly across the whole sky
        eagle.centerPixels = new Vector2(240f, 62f); eagle.radiusPixels = new Vector2(100f, 16f); eagle.secondsPerLap = 18f; // Eagle circles slowly above the middle peak
        snow.area = new Rect(0f, 28f, 480f, 180f); snow.count = 50; snow.windPixelsPerSecond = new Vector2(10f, 6f); // Light snow over the whole picture
        music = K.MenuMusic(); // Calm menu music and wind-and-birds ambience
        K.Events(); // Sends taps to the buttons
        return K.Canvas(canvasName, ppc, out group); // UI with the same whole-number zoom as the art
    }

    /// <summary>A cream plaque near the top with a title picture inside. Gives back the title picture.</summary>
    static Image Title(Transform root, Sprite sprite, Vector2 plaqueSize) // Makes the title plaque
    {
        var c = new Vector2(240f, 46f); // Middle of the plaque, under the guide line
        var plaque = M.Panel("Title", root, c, plaqueSize); // Plaque background
        plaque.GetComponent<Image>().raycastTarget = false; // Taps pass through
        return M.Picture("Text", plaque, sprite, c, c); // The title words
    }

    static void Save(UnityEngine.SceneManagement.Scene scene, string path, string afterPath) // Saves a scene and puts it in the build list
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, path); // Save the scene
        K.AddSceneAfter(path, afterPath); // Right after the screen before it
    }

    static Sprite S(string path) => K.Sprite(SpriteFolder, path, ArtScript); // Loads one selection sprite
}
