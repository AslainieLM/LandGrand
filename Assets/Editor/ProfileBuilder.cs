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
    const int GridColumns = 6; // Items per row in the Collection grid (D-060)
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
        if (!File.Exists(MenuSprites + "Buttons/btn_profile.png") || !File.Exists(LessonSprites + "item_slot.png") || !File.Exists(ItemFolder + "granite.png") || !File.Exists(MenuSprites + "Profile/badge_first_steps.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python Tools/Art/make_mainmenu_art.py, python Tools/Art/make_profile_art.py and python {LessonScript}"); // Tell the user what to do
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
        BuildSections(controller, root, font); // Collection and Achievements tabs and pages (D-060)
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

    /// <summary>
    /// The two sections under the strip (D-060): Collection and Achievements tabs at y 58 to 80, and one cream page panel at y 80 to 264.
    /// Collection: four landform tabs at y 88 to 108, then a grid of 6 by 2 items with names. Achievements: a grid of 4 by 2 badges with names.
    /// </summary>
    static void BuildSections(ProfileController controller, Transform root, TMP_FontAsset font) // Makes the tabs, both pages and their grids
    {
        var page = new Vector2(240f, 172f); // Middle of the page panel
        var panel = M.Panel("Page", root, page, new Vector2(464f, 184f)); // Cream page, x 8 to 472, y 80 to 264
        panel.GetComponent<Image>().raycastTarget = false; // Taps pass through to the cells
        controller.collectionTab = Tab("Collection tab", root, "tab_collection", 8f, 69f); // Left tab, joined to the page when chosen
        float nextX = 8f + P("tab_collection_on").rect.width + 4f; // The second tab starts 4 pixels after the first
        controller.achievementsTab = Tab("Achievements tab", root, "tab_achievements", nextX, 69f); // Second tab
        controller.badgeCount = M.ReadingLabel("Badge count", root, "Badges 0/8", new Vector2(412f, 69f), K.CanvasCenter, new Vector2(112f, 22f), K.Ink, TextAlignmentOptions.MidlineRight); // "Badges 3/8", right end of the tab row, x 356 to 468
        controller.badgeCount.richText = true; // The word is clay with a colour tag

        // Collection page: landform tabs and the item grid.
        var collection = M.Rect("Collection", root, page, page); // Holder the controller shows and hides
        var lands = new ProfileController.TabView[ShelfNames.Length]; // One tab per landform
        for (int i = 0; i < lands.Length; i++) // Each landform, in episode order
        {
            string id = ShelfNames[i].ToLowerInvariant(); // "mountains", "hills", and so on
            lands[i] = Tab("Landform " + ShelfNames[i], collection, "land_" + id, 18f + i * 112f, 98f); // 108 wide, 4 apart, x 18 to 462
            lands[i].locked = P("land_" + id + "_locked"); // Grey with a padlock
        }
        controller.landformTabs = lands; // The controller switches them
        var slots = new ProfileController.SlotView[GridColumns * 2]; // Two rows of items
        for (int i = 0; i < slots.Length; i++) // Each cell
            slots[i] = Cell("Item " + (i + 1), collection, LessonSprite("item_slot"), new Vector2(60f + (i % GridColumns) * 72f, 132f + (i / GridColumns) * 68f), 24f, 70f); // Frames 72 apart; rows at y 132 and 200
        controller.slots = slots; // The item grid
        controller.comingSoon = M.ReadingLabel("Coming soon", collection, "Coming soon", new Vector2(240f, 170f), page, new Vector2(200f, 22f), Muted, TextAlignmentOptions.Midline); // For an open landform without kinds
        controller.pictures = ItemCatalog.All.Select(k => new ProfileController.NamedSprite { name = k.Picture, sprite = Item(k.Picture) }).ToArray(); // Every item picture, by name
        controller.collectionPage = collection.gameObject; // The controller shows it

        // Achievements page: the badge grid.
        var achievements = M.Rect("Achievements", root, page, page); // Holder the controller shows and hides
        var badges = new ProfileController.SlotView[AchievementCatalog.All.Count]; // One cell per badge
        for (int i = 0; i < badges.Length; i++) // Each badge
            badges[i] = Cell("Badge " + (i + 1), achievements, null, new Vector2(72f + (i % 4) * 112f, 112f + (i / 4) * 78f), 32f, 104f); // Medals 112 apart; rows at y 112 and 190
        foreach (var b in badges) // Badge names may need two lines
        {
            b.label.textWrappingMode = TextWrappingModes.Normal; // Wrap a long name such as "Mountain Collector"
            b.label.rectTransform.sizeDelta = new Vector2(104f, 44f); // Room for two 24-pixel lines
            b.label.rectTransform.anchoredPosition += new Vector2(0f, -11f); // Keep the top of the name just under the medal
            b.label.alignment = TextAlignmentOptions.Top; // Lines start at the top
        }
        controller.badges = badges; // The badge grid
        controller.badgePictures = AchievementCatalog.All.SelectMany(a => new[] { "badge_" + a.Id, "badge_" + a.Id + "_locked" }).Select(n => new ProfileController.NamedSprite { name = n, sprite = P(n) }).ToArray(); // Earned and locked pictures, by name
        controller.achievementsPage = achievements.gameObject; // The controller shows it
    }

    /// <summary>A tab button whose left edge is at x and middle at y; its pictures are Profile/name_on and Profile/name_off.</summary>
    static ProfileController.TabView Tab(string label, Transform parent, string name, float left, float y) // Makes one tab
    {
        var on = P(name + "_on"); // Chosen picture
        var image = M.Picture(label, parent, on, new Vector2(left + on.rect.width / 2f, y), K.CanvasCenter); // Placed by its left edge
        image.raycastTarget = true; // Tabs catch taps
        var button = image.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.None; // Its picture change is the feedback
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between tabs
        return new ProfileController.TabView { button = button, image = image, on = on, off = P(name + "_off") }; // Everything the controller needs
    }

    /// <summary>One grid cell: a frame (or none) with a picture of the given size and a name under it.</summary>
    static ProfileController.SlotView Cell(string label, Transform parent, Sprite frame, Vector2 at, float pictureSize, float nameWidth) // Makes one cell
    {
        var holder = M.Picture(label, parent, frame, at, K.CanvasCenter); // Tan frame, or an empty tap area for badges
        if (frame == null) { holder.color = new Color(1f, 1f, 1f, 0f); holder.rectTransform.sizeDelta = new Vector2(pictureSize + 8f, pictureSize + 8f); } // Badges: a see-through tap area around the medal
        holder.raycastTarget = true; // The cell catches taps
        var button = holder.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.None; // The card opening is the feedback
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between cells
        var picture = M.Picture("Picture", holder.transform, null, at, at); // The picture or its dark shape
        picture.rectTransform.sizeDelta = new Vector2(pictureSize, pictureSize); // 24 for items, 32 for badges
        var name = M.ReadingLabel("Name", holder.transform, "?", at + new Vector2(0f, pictureSize / 2f + 11f), at, new Vector2(nameWidth, 22f), K.Ink, TextAlignmentOptions.Midline); // Name under the picture
        name.textWrappingMode = TextWrappingModes.NoWrap; // Items keep their name on one line
        return new ProfileController.SlotView { root = holder.gameObject, button = button, picture = picture, label = name }; // Everything the controller needs
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
    static Sprite P(string path) => K.Sprite(MenuSprites + "Profile/", path, "Tools/Art/make_profile_art.py"); // Loads one Profile tab or badge sprite (D-060)
    static Sprite Item(string path) => K.Sprite(ItemFolder, path, LessonScript); // Loads one item picture
    static Sprite LessonSprite(string path) => K.Sprite(LessonSprites, path, LessonScript); // Loads one Lesson Screen sprite
}
