using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select
using LandGrand.MainMenu; // Gives access to the shared name panel
using LandGrand.Profile; // Gives access to the Profile screen script
using LandGrand.Rules; // Gives access to ItemCatalog
using TMPro; // Gives access to TextMeshPro text and alignment
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds the Profile screen (decision D-052) as Assets/Scenes/Profile.unity and adds it to the build list after the Main Menu.
/// Run it from the menu: LandGrand > Build Profile Scene. Draw the art first with: python Tools/Art/make_mainmenu_art.py
/// Screen positions below are pixels on the 480x270 screen, measured from the top-left corner.
/// Layout: the explorer strip (y 6 to 58), then four shelves of 48 pixels, one per landform (y 64 to 262).
/// </summary>
public static class ProfileBuilder // Makes the Profile scene
{
    const string MenuSprites = "Assets/Art/MainMenu/Sprites/"; // Portrait, pencil, panel and OK button
    const string ItemFolder = "Assets/Art/Items/Sprites/"; // Rock and mineral pictures
    const string LessonSprites = "Assets/Art/Lesson/Sprites/"; // The tan item frame comes from the Lesson art
    const string LessonScript = "Tools/Art/make_lesson_art.py"; // Draws the Lesson art and the item pictures
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity"; // The Profile goes after this scene
    const string ProfilePath = "Assets/Scenes/Profile.unity"; // Where the Profile is saved
    const int SlotsPerShelf = 6; // Room for six item kinds on each shelf
    static readonly Color32 Clay = new Color32(154, 85, 50, 255); // Clay brown from the palette, for small headings
    static readonly Color32 DraftRed = new Color32(168, 63, 36, 255); // Scarf shade from the palette, for the draft note
    static readonly Color32 Muted = new Color32(110, 100, 90, 255); // Soft grey-brown for "Coming soon" and the card tag
    static readonly Color32 Warning = new Color32(160, 40, 40, 255); // Red for "name not accepted" messages, as on the Main Menu
    static readonly Color32 Sky = new Color32(191, 232, 247, 255); // Light sky colour, as on the Main Menu
    static readonly string[] ShelfNames = ((Episode[])System.Enum.GetValues(typeof(Episode))).Select(e => e.ToString().ToUpperInvariant()).ToArray(); // Shelf headings, one per landform in episode order, so a new landform gets a shelf too

    [MenuItem("LandGrand/Build Profile Scene")] // Adds the command to Unity's LandGrand menu
    public static void Build() // Builds and saves the Profile
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(MenuSprites + "Buttons/btn_profile.png") || !File.Exists(LessonSprites + "item_slot.png") || !File.Exists(ItemFolder + "granite.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python Tools/Art/make_mainmenu_art.py and python {LessonScript}"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        M.ConfigurePixelSprites(MenuSprites, _ => false); // Crisp pixel art for the camp, the portrait and the icons; this refreshes the assets, so the fonts are loaded after it
        M.ConfigurePixelSprites(ItemFolder, _ => false); // Crisp pixel art for the items
        var font = M.EnsureFont(); // The pixel font at 2x, for headings and numbers (D-050)
        if (font == null) return; // EnsureFont already said what is wrong
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var ppc = K.Camera(); // The 480x270 pixel-perfect camera
        ppc.GetComponent<Camera>().backgroundColor = Sky; // Same light sky as the Main Menu
        M.CampBackground(); // The Main Menu base camp, so the Profile feels like part of the menu (D-054)
        var music = K.MenuMusic(); // The calm menu music and ambience
        K.Events(); // Taps reach the buttons
        var root = K.Canvas("Profile UI", ppc, out var group); // UI canvas with the same zoom as the art
        var controller = new GameObject("Profile").AddComponent<ProfileController>(); // The screen brain

        BuildStrip(controller, root, font); // Portrait, name, pencil and totals
        BuildShelves(controller, root, font); // The Collection
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group
        BuildCard(controller, root); // Item card, on top of the shelves
        BuildNamePanel(controller, root, font); // Name panel, on top of everything else
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner (made last, so it is on top)

        Directory.CreateDirectory(Path.GetDirectoryName(ProfilePath)); // Make the Scenes folder if it is missing
        K.FixFonts(); // Right font on every text, never TextMeshPro's smooth default font
        EditorSceneManager.SaveScene(scene, ProfilePath); // Save the scene
        K.AddSceneAfter(ProfilePath, MainMenuPath); // Right after the Main Menu in the build list
        Debug.Log($"Profile scene built: {ProfilePath}. Open StartScreen, press Play, then tap the explorer portrait in the top-right corner of the Main Menu."); // Tell the user it worked
    }

    /// <summary>The explorer strip: portrait, "Explorer: name" with the pencil, and the three totals (D-052).</summary>
    static void BuildStrip(ProfileController controller, Transform root, TMP_FontAsset font) // Makes the top strip
    {
        var c = new Vector2(256f, 30f); // Middle of the strip, x 40 to 472 (the Back icon sits left of it), y 6 to 54
        var strip = M.Panel("Explorer", root, c, new Vector2(432f, 48f)); // Cream panel
        strip.GetComponent<Image>().raycastTarget = false; // Taps pass through to the pencil only
        M.Picture("Portrait", strip, S("Buttons/btn_profile"), new Vector2(64f, 30f), c); // Round explorer portrait, x 48 to 80, centred in the strip
        controller.nameLabel = M.ReadingLabel("Name", strip, "Explorer: —", new Vector2(218f, 19f), c, new Vector2(260f, 22f), K.Ink, TextAlignmentOptions.MidlineLeft); // "Explorer: Ana", from x 88, first row
        controller.nameLabel.textWrappingMode = TextWrappingModes.NoWrap; // The name stays on one line; the longest (12 wide letters) is 249 pixels in a 260-pixel box
        var pencil = M.MakeButton("Edit name", strip, "edit_icon", new Vector2(300f, 19f), c); // Pencil; the controller moves it to just after the name
        controller.editNameButton = pencil; // Opens the name panel
        controller.editName = (RectTransform)pencil.transform; // So the controller can move it

        // Totals row: a small clay word, then its number in the pixel font. No totals for stars or levels (Q-03 is open).
        controller.starsLabel = Total(strip, c, "Stars", 88f); // Stars earned, from x 88
        controller.foundLabel = Total(strip, c, "Found", 208f); // Item kinds found out of all, from x 208
        controller.levelsLabel = Total(strip, c, "Levels cleared", 328f); // Levels cleared, from x 328
    }

    /// <summary>One total in one label: the clay word and its number one space after it, so they share a baseline and a gap.</summary>
    static TMP_Text Total(Transform strip, Vector2 c, string word, float left) // Makes one total: one label, so the number always sits one space after its word
    {
        const float y = 41f; // Second row of the strip
        const float width = 116f; // Each total gets an even 120-pixel column
        var label = M.ReadingLabel(word, strip, word + " 0", new Vector2(left + width / 2f, y), c, new Vector2(width, 22f), K.Ink, TextAlignmentOptions.MidlineLeft); // The controller writes the clay word and the number
        label.richText = true; // The word is coloured clay with a colour tag
        return label; // The controller fills it
    }

    /// <summary>The Collection: four shelves, one per landform, each with up to six slots (D-052).</summary>
    static void BuildShelves(ProfileController controller, Transform root, TMP_FontAsset font) // Makes the shelves
    {
        var shelves = new ProfileController.ShelfView[ShelfNames.Length]; // Filled below
        for (int s = 0; s < ShelfNames.Length; s++) // One shelf per landform
        {
            var c = new Vector2(240f, 83f + s * 51f); // Middle of this shelf: y 58 to 108, 109 to 159, 160 to 210, 211 to 261
            var shelf = M.Panel("Shelf " + ShelfNames[s], root, c, new Vector2(464f, 50f)); // Cream shelf, x 8 to 472, 50 tall so the frame and the name both have room
            shelf.GetComponent<Image>().raycastTarget = false; // Taps pass through to the slots
            M.Label("Landform", shelf, font, ShelfNames[s], new Vector2(70f, c.y), c, new Vector2(104f, 22f), Clay, TextAlignmentOptions.MidlineLeft); // Landform name, x 18 to 122
            var view = new ProfileController.ShelfView // The parts the controller fills
            {
                comingSoon = M.ReadingLabel("Coming soon", shelf, "Coming soon", new Vector2(230f, c.y), c, new Vector2(200f, 22f), Muted, TextAlignmentOptions.MidlineLeft), // Shown when the landform has no kinds yet, from x 130
                slots = new ProfileController.SlotView[SlotsPerShelf], // Filled below
            };
            for (int i = 0; i < SlotsPerShelf; i++) // Each slot
            {
                var sc = new Vector2(146f + i * 62f, c.y - 7f); // Middle of the frame: 62 apart (x 131 to 471 for six), 3 pixels below the shelf top
                var frame = M.Picture("Slot " + (i + 1), shelf, LessonSprite("item_slot"), sc, c); // Tan frame, also the tap area
                frame.raycastTarget = true; // The frame catches taps
                var button = frame.gameObject.AddComponent<Button>(); // Makes it tappable
                button.transition = Selectable.Transition.None; // The card opening is the feedback
                button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between slots
                var picture = M.Picture("Picture", frame.transform, null, sc, sc); // The item picture or its dark shape
                picture.rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
                var label = M.ReadingLabel("Name", frame.transform, "?", sc + new Vector2(0f, 23f), sc, new Vector2(60f, 22f), K.Ink, TextAlignmentOptions.Midline); // Name or "?", 8 pixels under the frame and clear of the shelf bottom; 60 wide for "Feldspar" (60 pixels)
                label.textWrappingMode = TextWrappingModes.NoWrap; // A name stays on one line
                view.slots[i] = new ProfileController.SlotView { root = frame.gameObject, button = button, picture = picture, label = label }; // Everything the controller needs
            }
            shelves[s] = view; // Keep it
        }
        controller.shelves = shelves; // The four shelves
        controller.pictures = ItemCatalog.All.Select(k => new ProfileController.NamedSprite { name = k.Picture, sprite = Item(k.Picture) }).ToArray(); // Every item picture, by name
    }

    /// <summary>The item card: the in-level pop-up layout plus a Rock or Mineral tag and the draft note (D-052, Q-04).</summary>
    static void BuildCard(ProfileController controller, Transform root) // Makes the card
    {
        var c = K.CanvasCenter; // The card is centred on the screen
        var holder = M.Rect("Item card", root, c, c); // Holder: dark backdrop and the card
        holder.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = holder.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color(0f, 0f, 0f, 0.55f); // See-through black, as in the level
        dim.raycastTarget = true; // Catches taps outside the card
        var outside = holder.gameObject.AddComponent<Button>(); // A tap outside the card closes it
        outside.transition = Selectable.Transition.None; // No colour change on the backdrop
        outside.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving
        var panel = M.Panel("Card", holder, c, new Vector2(360f, 196f)); // Cream card, x 60 to 420, y 37 to 233; it catches its own taps, so only the backdrop closes it
        M.Picture("Slot", panel, LessonSprite("item_slot"), new Vector2(96f, 76f), c); // Frame behind the picture
        controller.cardPicture = M.Picture("Picture", panel, null, new Vector2(96f, 76f), c); // The item picture
        controller.cardPicture.rectTransform.sizeDelta = new Vector2(24f, 24f); // Item pictures are 24x24
        controller.cardName = M.ReadingLabel("Name", panel, "", new Vector2(222f, 66f), c, new Vector2(204f, 22f), Clay, TextAlignmentOptions.MidlineLeft); // Item name, from x 120
        controller.cardTag = M.ReadingLabel("Tag", panel, "", new Vector2(222f, 86f), c, new Vector2(204f, 22f), Muted, TextAlignmentOptions.MidlineLeft); // "Rock" or "Mineral", under the name
        controller.cardDescription = M.ReadingLabel("Description", panel, "", new Vector2(240f, 140f), c, new Vector2(336f, 72f), K.Ink, TextAlignmentOptions.TopLeft); // Up to three 24-pixel lines, x 72 to 408, y 104 to 176
        controller.cardDescription.textWrappingMode = TextWrappingModes.Normal; // Let the sentence wrap
        controller.cardDraftNote = M.ReadingLabel("Draft note", panel, "", new Vector2(240f, 188f), c, new Vector2(336f, 22f), DraftRed, TextAlignmentOptions.Midline); // "Draft — needs teacher review"
        controller.cardOk = M.MakeButton("OK", panel, "ok", new Vector2(240f, 216f), c); // OK, bottom of the card
        controller.cardOutside = outside; // The backdrop
        controller.card = holder.gameObject; // The controller shows and hides it
        holder.gameObject.SetActive(false); // Hidden at the start
    }

    /// <summary>The name panel, the same as the Main Menu one: question, typing box, message, OK and Back (D-052).</summary>
    static void BuildNamePanel(ProfileController controller, Transform root, TMP_FontAsset font) // Makes the name panel
    {
        var c = K.CanvasCenter; // Full-screen holder centre
        var holder = M.Rect("Name", root, c, c); // Holder: dark backdrop and the panel
        holder.sizeDelta = new Vector2(480f, 270f); // Covers the screen
        var dim = holder.gameObject.AddComponent<Image>(); // Dark backdrop
        dim.color = new Color32(31, 26, 36, 140); // Same see-through dark as the Main Menu
        dim.raycastTarget = true; // Blocks taps on the Profile while the panel is open
        var nameC = new Vector2(240f, 140f); // Middle of the name panel, as on the Main Menu
        var panel = M.Panel("Name panel", holder, nameC, new Vector2(300f, 130f)); // Panel background
        M.Label("Heading", panel, font, "What is your name?", new Vector2(240f, 100f), nameC, new Vector2(260f, 16f), K.Ink, TextAlignmentOptions.Midline); // Question
        var typed = M.InputBox("Name box", panel, font, "Type here", new Vector2(240f, 128f), nameC, new Vector2(160f, 24f)); // Where the player types
        var message = M.ReadingLabel("Message", panel, "", new Vector2(240f, 150f), nameC, new Vector2(284f, 22f), Warning, TextAlignmentOptions.Midline); // Why a name was refused
        var ok = M.MakeButton("OK", panel, "ok", new Vector2(204f, 178f), nameC); // Saves the name
        var back = M.MakeButton("Back", panel, "back", new Vector2(276f, 178f), nameC); // Closes the panel without changes
        var logic = holder.gameObject.AddComponent<NamePanel>(); // Checks and saves the typed name, like on the Main Menu
        logic.input = typed; // Typing box
        logic.message = message; // Message line
        logic.okButton = ok; // OK button
        controller.namePanel = logic; // The pencil opens it
        controller.nameBack = back; // Back closes it
        holder.gameObject.SetActive(false); // Hidden at the start
    }

    static Sprite S(string path) => K.Sprite(MenuSprites, path, "Tools/Art/make_mainmenu_art.py"); // Loads one Main Menu sprite
    static Sprite Item(string path) => K.Sprite(ItemFolder, path, LessonScript); // Loads one item picture
    static Sprite LessonSprite(string path) => K.Sprite(LessonSprites, path, LessonScript); // Loads one Lesson Screen sprite
}
