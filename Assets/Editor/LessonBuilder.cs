using System.Collections.Generic; // Gives access to List
using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select
using LandGrand.LessonScreen; // Gives access to the Lesson Screen script
using LandGrand.Rules; // Gives access to ItemCatalog
using TMPro; // Gives access to TextMeshPro text and fonts
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds the Lesson Screen of ticket 06 as Assets/Scenes/Lesson.unity and adds it to the build list after the level trail.
/// Run it from the menu: LandGrand > Build Lesson Scene. Draw the art first with: python Tools/Art/make_lesson_art.py
/// Screen positions below are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class LessonBuilder // Makes the Lesson Screen scene
{
    const string SpriteFolder = "Assets/Art/Lesson/Sprites/"; // Lesson Screen PNGs
    const string ItemFolder = "Assets/Art/Items/Sprites/"; // Rock and mineral PNGs, shared by every level
    const string ArtScript = "Tools/Art/make_lesson_art.py"; // Script that draws them
    const string SelectionSprites = "Assets/Art/Selection/Sprites/"; // The titles and the Mountains backdrop come from the selection art
    const string LevelPath = "Assets/Scenes/LevelSelection.unity"; // The Lesson Screen goes after this scene
    const string LessonPath = "Assets/Scenes/Lesson.unity"; // Where the Lesson Screen is saved
    static readonly string[] Landforms = { "mountains", "hills", "plains", "plateaus" }; // File names, in Episode order
    static readonly string[] Difficulties = { "easy", "normal", "hard" }; // File names, in Difficulty order
    static readonly Color32 Clay = new Color32(154, 85, 50, 255); // Clay brown from the palette, for headings
    static readonly Color32 DraftRed = new Color32(168, 63, 36, 255); // Scarf shade from the palette, so the draft note stands out
    static readonly Vector2 PanelCenter = new Vector2(240f, 152f); // Middle of the big cream panel

    [MenuItem("LandGrand/Build Lesson Scene")] // Adds the command to Unity's LandGrand menu
    public static void Build() // Builds and saves the Lesson Screen
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(SpriteFolder + "lesson_mountains.png") || !File.Exists(ItemFolder + "granite.png") || !File.Exists(SelectionSprites + "bg_mountains.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python {ArtScript} and python Tools/Art/make_selection_art.py"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        var font = M.EnsureFont(); // The pixel font as a TextMeshPro font
        if (font == null) return; // EnsureFont already said what is wrong
        M.ConfigurePixelSprites(SpriteFolder, _ => false); // Crisp pixel art for the lesson parts
        M.ConfigurePixelSprites(ItemFolder, _ => false); // Crisp pixel art for the items
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var root = SelectionBuilder.Backdrop("Lesson UI", out var music, out var group); // Mountains behind, music and UI canvas
        var controller = new GameObject("Lesson").AddComponent<LessonController>(); // The screen brain

        var titleC = new Vector2(240f, 24f); // Middle of the title plaque, top centre
        var plaque = M.Panel("Title", root, titleC, new Vector2(210f, 24f)); // Plaque background, wide enough for "MOUNTAINS: NORMAL"
        plaque.GetComponent<Image>().raycastTarget = false; // Taps pass through
        controller.title = M.Picture("Text", plaque, Sel("Titles/title_mountains_easy"), titleC, titleC); // "MOUNTAINS: EASY"
        controller.titles = Landforms.SelectMany(n => Difficulties.Select(d => Sel($"Titles/title_{n}_{d}"))).ToArray(); // Landform * 3 + difficulty

        var pc = PanelCenter; // Short name for the panel middle
        var panel = M.Panel("Lesson Panel", root, pc, new Vector2(448f, 220f)); // Big cream panel from x 16 to 464, y 42 to 262

        // Top part: the landform picture on the left, the heading and lesson sentences on the right.
        controller.picture = M.Picture("Picture", panel, S("lesson_mountains"), new Vector2(88f, 95f), pc); // 120x82 picture, x 28 to 148
        controller.heading = Spaced(M.Label("Heading", panel, font, "Level 1: Mountains", new Vector2(306f, 60f), pc, new Vector2(292f, 14f), Clay, TextAlignmentOptions.MidlineLeft)); // Heading, x 160 to 452
        var body = Spaced(M.Label("Body", panel, font, "", new Vector2(306f, 104f), pc, new Vector2(292f, 72f), K.Ink, TextAlignmentOptions.TopLeft)); // Lesson sentences under the heading: four spaced-out lines
        body.textWrappingMode = TextWrappingModes.Normal; // A long sentence breaks onto the next line
        controller.body = body; // The controller fills in the sentences
        controller.draftNote = Spaced(M.Label("Draft Note", panel, font, "", new Vector2(138f, 147f), pc, new Vector2(220f, 14f), DraftRed, TextAlignmentOptions.MidlineLeft)); // Draft note under the picture

        // Bottom-left part: "You will find:" and one box per item kind.
        controller.itemsHeading = Spaced(M.Label("Items Heading", panel, font, "You will find:", new Vector2(138f, 162f), pc, new Vector2(220f, 14f), Clay, TextAlignmentOptions.MidlineLeft)).gameObject; // Items heading
        var slots = new List<LessonController.ItemSlot>(); // Filled below
        for (int i = 0; i < 4; i++) // Four boxes: enough for granite, quartz, feldspar and mica
        {
            var c = new Vector2(50f + i * 56f, 186f); // Middle of this box; 56 apart leaves room for the goals on the right
            var box = M.Picture("Item " + (i + 1), panel, S("item_slot"), c, pc); // 30x30 tan box
            var picture = M.Picture("Picture", box.transform, I("granite"), c, c); // Item picture inside the box
            var label = Spaced(M.Label("Name", box.transform, font, "", c + new Vector2(0f, 23f), c, new Vector2(56f, 14f), K.Ink, TextAlignmentOptions.Midline)); // Item name under the box
            slots.Add(new LessonController.ItemSlot { root = box.gameObject, picture = picture, label = label }); // Everything the controller needs
        }
        controller.items = slots.ToArray(); // The four boxes

        // Bottom-right part: "Goals:" and one row per goal, each with an icon.
        controller.goalsHeading = Spaced(M.Label("Goals Heading", panel, font, "Goals:", new Vector2(354f, 162f), pc, new Vector2(196f, 14f), Clay, TextAlignmentOptions.MidlineLeft)).gameObject; // Goals heading, x 256 to 452
        string[] icons = { "icon_star", "icon_clock" }; // Star for collecting, clock for time
        var rows = new GameObject[icons.Length]; // Filled below
        var goals = new TMP_Text[icons.Length]; // Filled below
        for (int i = 0; i < icons.Length; i++) // One row per goal
        {
            var c = new Vector2(354f, 182f + i * 18f); // Middle of this row
            var row = M.Rect("Goal " + (i + 1), panel, c, pc); // Row holder
            row.sizeDelta = new Vector2(196f, 16f); // Room for the icon and the text
            M.Picture("Icon", row, S(icons[i]), new Vector2(264f, c.y), c); // Icon on the left of the row
            goals[i] = Spaced(M.Label("Text", row, font, "", new Vector2(363f, c.y), c, new Vector2(178f, 14f), K.Ink, TextAlignmentOptions.MidlineLeft)); // Goal words, x 274 to 452
            rows[i] = row.gameObject; // The controller hides rows it does not need
        }
        controller.goalRows = rows; // The goal rows
        controller.goals = goals; // The goal texts

        // Start button, bottom centre of the panel.
        var start = M.Picture("Start", panel, S("btn_start"), new Vector2(240f, 238f), pc); // Green Start button
        start.raycastTarget = true; // Buttons must catch taps
        var startButton = start.gameObject.AddComponent<Button>(); // Makes it tappable
        startButton.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        startButton.targetGraphic = start; // The picture that changes
        startButton.spriteState = new SpriteState { pressedSprite = S("btn_start_pressed") }; // Pushed-in picture
        startButton.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        controller.startButton = startButton; // The controller connects it

        var pictures = new List<LessonController.NamedSprite> { new LessonController.NamedSprite { name = "lesson_mountains", sprite = S("lesson_mountains") } }; // The Mountains lesson picture
        pictures.AddRange(ItemCatalog.All.Select(k => new LessonController.NamedSprite { name = k.Picture, sprite = I(k.Picture) })); // Every item picture
        controller.pictures = pictures.ToArray(); // The controller looks pictures up by name

        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group (made after the panel, so the guide shows on top)
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner (made last, so it is on top)

        Directory.CreateDirectory(Path.GetDirectoryName(LessonPath)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, LessonPath); // Save the scene
        K.AddSceneAfter(LessonPath, LevelPath); // Right after the level trail in the build list
        Debug.Log($"Lesson scene built: {LessonPath}. Open StartScreen, press Play, and tap Mountains, Easy, then level 1."); // Tell the user it worked
    }

    /// <summary>Gives lesson text wider gaps between words and lines, so words do not run together. 25 units = 3 pixels at the 12-pixel font size.</summary>
    static TMP_Text Spaced(TMP_Text label) // Adds the extra spacing
    {
        label.wordSpacing = 20f; // 3 more pixels between words (Micro 5 spaces are only 4 pixels)
        label.lineSpacing = 12f; // About 2 more pixels between lines
        return label; // Give the same label back
    }

    static Sprite S(string path) => K.Sprite(SpriteFolder, path, ArtScript); // Loads one Lesson Screen sprite
    static Sprite I(string path) => K.Sprite(ItemFolder, path, ArtScript); // Loads one item sprite
    static Sprite Sel(string path) => K.Sprite(SelectionSprites, path, "Tools/Art/make_selection_art.py"); // Loads one selection sprite
}
