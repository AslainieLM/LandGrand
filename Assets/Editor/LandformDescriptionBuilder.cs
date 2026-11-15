using System; // Gives the landform list
using System.IO; // Checks the supplied panel art
using System.Linq; // Loads all the menu pictures
using LandGrand; // Gives the description controller
using LandGrand.Rules; // Gives the summary content
using TMPro; // Gives the text tools
using UnityEditor; // Adds the builder menu
using UnityEditor.SceneManagement; // Creates and saves the scene
using UnityEngine; // Gives pictures and positions
using K = PixelSceneKit; // Reuses the pixel-screen setup
using M = global::MainMenuBuilder; // Reuses menu pictures and labels

public static class LandformDescriptionBuilder // Builds the review screen from code
{ // Opens the builder
    internal const string ScenePath = "Assets/Scenes/LandformDescription.unity"; // The generated scene
    const string LessonPath = "Assets/Scenes/Lesson.unity"; // The scene just before the review in the build list
    static readonly Vector2 Panel = new Vector2(240f, 128f); // Exact centre of the 300 by 196 scroll
    static readonly Color32 DraftRed = new Color32(168, 63, 36, 255); // Matches the Lesson Screen's draft note

    [MenuItem("LandGrand/Build Landform Description Scene")] // Adds the builder to the LandGrand menu
    public static void Build() // Builds the Landform Description scene
    { // Starts the build
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Keeps any open scene work
        if (!File.Exists(DescriptionArt.Folder + "scroll_panel.png")) throw new FileNotFoundException("Missing Description scroll art. Run python Tools/Art/make_hard_art.py."); // Stops if the supplied art is missing
        M.ConfigurePixelSprites(DescriptionArt.Folder, _ => false); // Uses crisp pixel art for the buttons and panel
        var font = M.EnsureFont(); // Loads the Micro 5 font
        if (font == null) return; // Stops when the shared font is missing
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Starts with an empty scene
        var root = SelectionBuilder.Backdrop("Landform Description UI", out var music, out var group); // Reuses the menu picture, motion and canvas
        var controller = new GameObject("Landform Description").AddComponent<LandformDescriptionController>(); // Runs the screen
        var panel = M.Picture("Scroll Panel", root, DescriptionArt.Sprite("scroll_panel"), Panel, K.CanvasCenter).rectTransform; // Places the scroll at its spec position
        controller.title = M.Label("Title", panel, font, "Mountains", new Vector2(240f, 49f), Panel, new Vector2(280f, 22f), K.Ink, TextAlignmentOptions.Midline); // Micro 5 title
        BuildSummary(controller, panel); // Adds the five reading lines and draft note
        BuildTrip(controller, panel, font); // Adds the stars, kinds and badges
        BuildUnlock(controller, panel); // Adds the Hills picture and banner
        controller.toMap = DescriptionArt.Button("To the Map", root, "btn_to_map", new Vector2(240f, 245f)); // Under the scroll
        controller.backButton = K.BackIcon(root); // The controller shows it for a book visit
        controller.fanfare = DescriptionArt.Sound("sfx_fanfare"); // Plays when the summary opens
        K.Wire(controller, root, font, music, group); // Connects shared input and sound
        K.FixFonts(); // Refreshes the labels before saving
        EditorSceneManager.SaveScene(scene, ScenePath); // Saves only through the builder
        K.AddSceneAfter(ScenePath, LessonPath); // Puts the review after Lesson in the build list
        Debug.Log("Landform Description built: " + ScenePath); // Reports the generated scene
    } // Ends the build

    static void BuildSummary(LandformDescriptionController controller, Transform panel) // Builds the review text
    { // Starts the lines
        var summary = LandformSummary.For(Episode.Mountains); // Uses the actual text for the scene preview
        controller.lines = new TMP_Text[summary.Sentences.Count]; // One text label per sentence
        for (int i = 0; i < controller.lines.Length; i++) // Places the five lines
        { // Starts one line
            var label = M.ReadingLabel("Summary " + (i + 1), panel, summary.Sentences[i], new Vector2(240f, 67f + i * 17f), Panel, new Vector2(286f, 20f), K.Ink, TextAlignmentOptions.MidlineLeft); // Andika 15 inside the scroll edge
            label.textWrappingMode = TextWrappingModes.NoWrap; // Keeps each sentence on its own line
            label.characterSpacing = -2f; // Fits the longest lines inside the scroll border
            controller.lines[i] = label; // Lets the controller swap the text
        } // Ends one line
        controller.draftNote = M.ReadingLabel("Draft Note", panel, LevelData.DraftNote, new Vector2(240f, 152f), Panel, new Vector2(286f, 20f), DraftRed, TextAlignmentOptions.Midline); // Shows the teacher-review note
    } // Ends the lines

    static void BuildTrip(LandformDescriptionController controller, Transform panel, TMP_FontAsset font) // Builds the trip numbers and their icons
    { // Starts the totals
        M.Label("Your Trip", panel, font, "Your trip", new Vector2(138f, 177f), Panel, new Vector2(80f, 20f), K.Ink, TextAlignmentOptions.MidlineLeft); // Names the trip row
        var star = K.Sprite("Assets/Art/Selection/Sprites/Trail/", "star_full", "Tools/Art/make_selection_art.py"); // Reuses a filled star
        var gem = K.Sprite("Assets/Art/Level/Sprites/", "icon_gem", "Tools/Art/make_level_art.py"); // Reuses the pickup icon
        var medal = K.Sprite("Assets/Art/MainMenu/Sprites/Profile/", "badge_first_steps", "Tools/Art/make_profile_art.py"); // Reuses a small medal
        controller.stars = TripTotal(panel, "Stars", star, 192f, "0/45"); // Best stars
        controller.kinds = TripTotal(panel, "Kinds", gem, 260f, "0/34"); // Found kinds
        controller.badges = TripTotal(panel, "Badges", medal, 328f, "0/" + AchievementCatalog.All.Count); // Earned badges
    } // Ends the totals

    static TMP_Text TripTotal(Transform panel, string name, Sprite icon, float x, string value) // Sets one icon and its total side by side
    { // Starts the total
        M.Picture(name + " Icon", panel, icon, new Vector2(x, 177f), Panel).rectTransform.sizeDelta = new Vector2(14f, 14f); // Makes the three icons the same small size
        return M.ReadingLabel(name, panel, value, new Vector2(x + 30f, 177f), Panel, new Vector2(42f, 20f), K.Ink, TextAlignmentOptions.MidlineLeft); // Number to the right of the icon
    } // Ends the total

    static void BuildUnlock(LandformDescriptionController controller, Transform panel) // Builds the next landform banner
    { // Starts the banner
        var banner = M.Rect("Unlock Banner", panel, Panel, Panel); // Holds all the optional parts together
        var lockAt = new Vector2(160f, 204f); // Puts the lock between the picture and the text
        controller.unlockBanner = banner.gameObject; // Hides the whole banner on the last landform
        controller.landformPictures = ((Episode[])Enum.GetValues(typeof(Episode))).Select(e => K.Sprite("Assets/Art/Selection/Sprites/", LandformSummary.For(e).Picture, "Tools/Art/make_selection_art.py")).ToArray(); // Reuses the landform pictures
        controller.nextPicture = M.Picture("Next Landform", banner, controller.landformPictures[(int)Episode.Hills], new Vector2(122f, 204f), Panel); // Small Hills picture at the left
        controller.nextPicture.rectTransform.sizeDelta = new Vector2(40f, 22.5f); // Keeps the landform picture's 16 by 9 shape
        controller.padlock = M.Picture("Opening Padlock", banner, DescriptionArt.Sprite("unlock_padlock_00"), lockAt, Panel); // Opens next to the picture
        controller.unlock = DescriptionArt.Unlock(banner, lockAt, Panel); // Adds the shared lock frames and sparkles
        controller.opened = M.ReadingLabel("Opened", banner, "Hills is now open!", new Vector2(280f, 194f), Panel, new Vector2(200f, 20f), K.Ink, TextAlignmentOptions.MidlineLeft); // The first banner line
        controller.comingSoon = M.ReadingLabel("New Levels", banner, "New levels coming soon", new Vector2(280f, 211f), Panel, new Vector2(200f, 20f), K.Ink, TextAlignmentOptions.MidlineLeft); // The second banner line
    } // Ends the banner
} // Ends the builder
