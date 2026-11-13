using System.IO; // Gives access to folders and files
using LandGrand.LandformMap; // Gives access to the Landform Map scripts
using LandGrand.Rules; // Gives access to Episode
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
        }
        regions[0].life = life.gameObject; // Only Mountains has animations so far

        // Controller: connects everything. The guide panel and Back icon are made here, last, so they draw on top.
        var controller = new GameObject("Landform Map").AddComponent<LandformMapController>(); // The map brain
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group
        controller.regions = regions; // The four regions
        controller.backButton = K.BackIcon(root); // Back icon, top-left corner

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath)); // Make the Scenes folder if it is missing
        K.FixFonts(); // Right pixel font on every text, never TextMeshPro's smooth default font
        EditorSceneManager.SaveScene(scene, ScenePath); // Save the new scene
        K.AddSceneAfter(ScenePath, MainMenuPath); // Put it in the build list so Play can open it
        Debug.Log($"Landform Map built: {ScenePath}. Open StartScreen, press Play, and tap Play on the Main Menu."); // Tell the user it worked
    }

    static Sprite S(string path) => K.Sprite(SpriteFolder, path, "Tools/Art/make_landform_map_art.py"); // Loads one Landform Map sprite
}
