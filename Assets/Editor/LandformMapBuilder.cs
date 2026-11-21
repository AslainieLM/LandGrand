using System.IO; // Gives access to folders and files
using LandGrand.LandformMap; // Gives access to the Landform Map scripts
using LandGrand.Rules; // Gives access to Episode
using LandGrand.StartScreen; // Gives access to SpriteFlipbook for the swaying trees
using TMPro; // Gives access to TextMeshPro text
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
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
    const string ScenePath = "Assets/Scenes/LandformMap.unity"; // Where the scene is saved
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity"; // The map goes right after this scene in the build list

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
        M.ConfigurePixelSprites(DescriptionArt.Folder, _ => false); // Crisp unlock art
        var font = M.EnsureFont(); // The pixel font as a TextMeshPro font
        if (font == null) return; // EnsureFont already said what is wrong
        M.ConfigurePixelSprites(SpriteFolder, path => path.Contains("/Locked/")); // Crisp pixel art; the grey copies stay readable because they are the tap areas
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var ppc = K.Camera(); // The same 480x270 pixel-perfect view as the other screens

        // The map picture: all four regions in one connected 480x270 image.
        M.Renderer("Map", S("map"), 0, null).transform.position = M.Px(240f, 135f); // Fills the screen

        // Mountains life: blowing snow, a drifting cloud and a circling eagle (shown only while Mountains is unlocked).
        var life = K.MountainsLife(out _, out _, out _); // Default places fit the Mountains region of the map

        var music = K.MenuMusic(); // Calm menu music and wind-and-birds ambience
        K.Events(); // Sends taps to the buttons
        var root = K.Canvas("Map UI", ppc, out var group); // UI with the same whole-number zoom as the art

        // Regions: grey copy (tap area), then label and padlock on top.
        var regions = new LandformMapController.Region[Regions.Length]; // Filled below
        for (int i = 0; i < Regions.Length; i++) // Tap areas first, so labels draw on top of them
        {
            var r = Regions[i]; // This region's layout
            var locked = M.Picture("Region " + r.name, root, S("Locked/locked_" + r.name), K.CanvasCenter, K.CanvasCenter); // Grey copy of the region over the whole screen
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
            M.Picture("Star", plaque, S("star"), r.label + new Vector2(-14f, 7f), r.label); // Star icon, left of the middle
            regions[i].stars = M.Label("Stars", plaque, font, "0/45", r.label + new Vector2(18f, 7f), r.label, new Vector2(40f, 14f), K.Ink, TextAlignmentOptions.MidlineLeft); // Star progress, starting 4 pixels right of the star
            regions[i].padlock = M.Picture("Padlock " + r.name, root, S("padlock"), r.padlock, K.CanvasCenter).gameObject; // Padlock while locked
            if (r.episode != Episode.Mountains) regions[i].unlock = DescriptionArt.Unlock(root, r.padlock, K.CanvasCenter); // Gives newly opened regions their own sparkles
        }
        regions[0].life = life.gameObject; // Keeps the existing Mountains animations
        regions[(int)Episode.Hills].life = HillsLife().gameObject; // Trees, stream and birds, shown only while Hills is open
        regions[(int)Episode.Plains].life = PlainsLife().gameObject; // Rice, egrets and carabao while Plains is open

        // Controller: connects everything. The guide panel and Back icon are made here, last, so they draw on top.
        var controller = new GameObject("Landform Map").AddComponent<LandformMapController>(); // The map brain
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group
        controller.regions = regions; // The four regions
        controller.unlockSound = DescriptionArt.Sound("sfx_unlock"); // The map unlock sound
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); // Make the Scenes folder if it is missing
        K.FixFonts(); // Right pixel font on every text, never TextMeshPro's smooth default font
        EditorSceneManager.SaveScene(scene, ScenePath); // Save the new scene
        K.AddSceneAfter(ScenePath, MainMenuPath); // Put it in the build list so Play can open it
        Debug.Log($"Landform Map built: {ScenePath}. Open StartScreen, press Play, and tap Play on the Main Menu."); // Tell the user it worked
    }

    static Sprite S(string path) => K.Sprite(SpriteFolder, path, "Tools/Art/make_landform_map_art.py"); // Loads one Landform Map sprite

    static Transform PlainsLife() // Makes the animations inside the Plains region
    { // Opens the Plains map life
        var life = new GameObject("Plains life").transform; // Holds the Plains map life
        var rice = new (float x, float y, float framesPerSecond)[] // Bottom pixel and sway speed for each rice clump
        { // Opens the rice spots
            (30f, 170f, 1.2f), (70f, 182f, 1.5f), (140f, 168f, 1.7f), // The upper rice clumps
            (190f, 160f, 1.3f), (200f, 218f, 1.6f), (150f, 228f, 1.4f), // The eastern and lower rice clumps
        }; // Ends the rice spots
        foreach (var spot in rice) // Places all six clumps at their authored spots
            LandformLife.PlainsStandingLife(life, "Rice", "rice", new Vector2(spot.x, spot.y), spot.framesPerSecond, 2); // Sways the rice
        LandformLife.PlainsStandingLife(life, "Carabao", "carabao", new Vector2(58f, 204f), 0.5f, 3); // Changes pose every two seconds
        var center = new Vector2(120f, 152f); // Middle of the two egret paths
        var radius = new Vector2(20f, 8f); // Width and height of each oval
        LandformLife.Egret(life, center, radius, 14f); // The first egret circles in fourteen seconds
        LandformLife.Egret(life, center, radius, 17f); // The second egret circles in seventeen seconds
        return life; // The map shows this life only while Plains is open
    } // Ends the Plains map life

    // Hills trees: x across, which hill line they stand on (true = far hill), and how fast they sway.
    static readonly (float x, bool far, float framesPerSecond)[] HillsTrees = // One entry per tree
    {
        (290f, true, 1.2f), (312f, true, 1.4f), (405f, true, 1.6f), (440f, true, 1.8f), // Four on the far hill line
        (270f, false, 1.3f), (455f, false, 1.5f), // Two on the near hill line
    };
    const float TreeHalfHeight = 5.5f; // Half of the 11 pixel tree, so its bottom sits on the line
    static readonly float[] StreamRows = { 104f, 116f, 128f }; // Screen rows of the three stream ripples

    /// <summary>Hills map life: six swaying trees, three shining ripples in the stream and two birds. Only shown while Hills is open.</summary>
    static Transform HillsLife() // Makes the Hills animations
    {
        var life = new GameObject("Hills life").transform; // Parent for the Hills animations
        foreach (var t in HillsTrees) // One tree each
        {
            float line = t.far ? 72f + Mathf.Round(12f * Mathf.Sin(t.x / 27f)) : 94f + Mathf.Round(10f * Mathf.Sin(t.x / 19f + 1f)); // Hill line height at this x
            var tree = M.Renderer("Tree", LandformLife.HillsSprite("tree_0"), 2, life); // Tree picture behind the birds
            tree.transform.position = M.Px(t.x, line - TreeHalfHeight); // Bottom edge sits on the hill line
            var sway = tree.gameObject.AddComponent<SpriteFlipbook>(); // Makes the tree sway
            sway.frames = new[] { LandformLife.HillsSprite("tree_0"), LandformLife.HillsSprite("tree_1") }; // Two tree poses
            sway.framesPerSecond = t.framesPerSecond; // Each tree sways a little differently
        }
        foreach (float y in StreamRows) // One ripple per stream row
        {
            float x = 340f + Mathf.Round(9f * Mathf.Sin(y / 9f)) - 3f; // Stream shine wobbles across the water
            var ripple = M.Renderer("Ripple", LandformLife.HillsSprite("ripple_0"), 1, life); // Ripple picture, behind the trees
            ripple.transform.position = M.Px(x, y); // Put it on its row
            var shine = ripple.gameObject.AddComponent<SpriteFlipbook>(); // Makes the water shine
            shine.frames = new[] { LandformLife.HillsSprite("ripple_0"), LandformLife.HillsSprite("ripple_1") }; // Two shine poses
            shine.framesPerSecond = 2f; // Shines twice a second
        }
        LandformLife.Bird(life, new Vector2(372f, 30f), new Vector2(20f, 20f), 12f); // First bird circles high over the hills
        LandformLife.Bird(life, new Vector2(372f, 30f), new Vector2(20f, 20f), 15f); // Second bird circles at its own speed
        return life; // The map shows it only while Hills is open
    }
}
