using System.Collections.Generic; // Gives access to List
using System.IO; // Gives access to folders and files
using System.Linq; // Gives access to list helpers such as Select
using LandGrand.Play; // Gives access to the level play scripts
using LandGrand.Rules; // Gives access to LevelCatalog, Episode and Difficulty
using UnityEditor; // Gives access to editor tools such as menu items
using UnityEditor.SceneManagement; // Gives access to making and saving scenes
using UnityEngine; // Gives access to GameObject, Sprite, Rect and other basics
using UnityEngine.UI; // Gives access to Image and Button
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Builds the Mountains Easy 1 level of ticket 07 as Assets/Scenes/MountainsEasy1.unity and adds it to the build list after the Lesson Screen.
/// Run it from the menu: LandGrand > Build Mountains Easy 1 Level. Draw the art first with: python Tools/Art/make_level_art.py
/// Level positions are level pixels: x goes right from the left edge, y goes up from the ground line (the top of the grass).
/// Screen (UI) positions are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
public static class LevelBuilder // Makes the first level scene
{
    const string SpriteFolder = "Assets/Art/Level/Sprites/"; // Level PNGs
    const string ArtScript = "Tools/Art/make_level_art.py"; // Script that draws them
    const string StartSprites = "Assets/Art/StartScreen/Sprites/"; // The explorer and dust come from the start screen
    const string SelectionSprites = "Assets/Art/Selection/Sprites/"; // The result stars come from the selection art
    const string LessonPath = "Assets/Scenes/Lesson.unity"; // The level goes after this scene
    const string LevelPath = "Assets/Scenes/MountainsEasy1.unity"; // Where the level is saved
    const float Width = 4800f; // Level width: ten screens, about 65 s of walking (decision D-046)

    // Boulders: left edge, picture. They block from every side; the explorer jumps onto or over them.
    static readonly (float x, string sprite)[] Boulders = { (300f, "boulder_small"), (900f, "boulder_big"), (1300f, "boulder_small"), (2150f, "boulder_big"), (2700f, "boulder_small"), (3250f, "boulder_big"), (3850f, "boulder_big"), (4150f, "boulder_small"), (4350f, "boulder_big") }; // Nine boulders along the trail

    // Ledges: left edge, top height, width. The explorer stands on them and can jump up through them.
    static readonly (float x, float top, float w)[] Ledges = { (520f, 40f, 80f), (1050f, 28f, 80f), (1500f, 28f, 64f), (2300f, 28f, 64f), (2920f, 28f, 80f), (3560f, 28f, 48f), (3620f, 56f, 96f), (3950f, 28f, 80f), (4480f, 28f, 48f) }; // Low ledges, plus a step and a high ledge

    // Background layers from the start screen: name, share of the camera's movement, own drift in pixels per second.
    // L6 and L7 are ground strips; the level draws its own ground, so they are left out.
    static readonly (string name, float follow, float drift)[] Layers = // Far layers move least
    {
        ("L0", 0f, 0f), ("L1", 0.03f, 1.5f), ("L2", 0.1f, 0f), ("L3", 0.2f, 0f), ("L4", 0.35f, 0f), ("L5", 0.55f, 0f), // Sky, clouds, far and near mountains, plateaus, hills
    };

    [MenuItem("LandGrand/Build Mountains Easy 1 Level")] // Adds the command to Unity's LandGrand menu
    public static void Build() // Builds and saves the level scene
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // Stop if the user cancels saving the open scene
        if (!Directory.Exists("Assets/TextMesh Pro")) // TextMeshPro needs its starter files once per project
        {
            Debug.LogError("TextMeshPro is not set up yet. Choose Window > TextMeshPro > Import TMP Essential Resources, then run this builder again."); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        if (!File.Exists(SpriteFolder + "grass_top.png") || !File.Exists(SelectionSprites + "Trail/star_full.png")) // The art must be drawn first
        {
            Debug.LogError($"Missing art. Run: python {ArtScript} and python Tools/Art/make_selection_art.py"); // Tell the user what to do
            return; // Stop before building a broken scene
        }
        var font = M.EnsureFont(); // The pixel font as a TextMeshPro font
        if (font == null) return; // EnsureFont already said what is wrong
        M.ConfigurePixelSprites(SpriteFolder, _ => false); // Crisp pixel art for the level parts
        StartScreenBuilder.SetLandscapeOnly(); // Keep the game in landscape only (decision D-030)

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); // Start from an empty scene
        var ppc = K.Camera(); // The same 480x270 pixel-perfect view as the other screens
        var background = new GameObject("Background").transform; // Holds the moving mountain layers
        background.SetParent(ppc.transform, false); // Rides on the camera
        background.localPosition = new Vector3(0f, 0f, 10f); // In front of the camera's near edge
        for (int i = 0; i < Layers.Length; i++) // Each start-screen layer, far to near
        {
            var layerSprite = Spr("Background/" + Layers[i].name); // The layer art from the start screen
            var layer = new GameObject(Layers[i].name).AddComponent<BackdropLayer>(); // Slides the layer with the camera
            layer.transform.SetParent(background, false); // Keep it under Background
            layer.follow = Layers[i].follow; // How much it copies the camera
            layer.driftPixelsPerSecond = Layers[i].drift; // Clouds drift on their own
            layer.widthPixels = 960f; // Layer art is two screens wide
            layer.pixelsPerUnit = LevelGeometry.PixelsPerUnit; // Same pixel size as the sprites
            M.Renderer("Art", layerSprite, -100 + i * 10, layer.transform); // The picture; farther layers draw first
        }

        var geometry = new GameObject("Level").AddComponent<LevelGeometry>(); // The level's shapes
        geometry.width = Width; // Three screens wide
        var solids = new List<Rect> { new Rect(0f, -64f, Width, 64f) }; // The ground
        Tiled("Grass", "grass_top", new Rect(0f, -8f, Width, 8f), 5, geometry.transform); // Grass along the top of the ground
        Tiled("Soil", "soil", new Rect(0f, -64f, Width, 56f), 5, geometry.transform); // Soil below the grass

        foreach (var (x, name) in Boulders) // Each boulder
        {
            var sprite = S(name); // Its picture
            float w = sprite.rect.width, h = sprite.rect.height; // Its size in pixels
            var r = M.Renderer(name, sprite, 6, geometry.transform); // Draws the boulder
            r.transform.position = LevelGeometry.ToWorld(x + w / 2f, h / 2f); // Sitting on the ground
            solids.Add(new Rect(x + 1f, 0f, w - 2f, h - 1f)); // Its solid shape, inside the dark outline
        }
        geometry.solids = solids.ToArray(); // Ground and boulders
        var ledges = new List<Rect>(); // Filled below
        foreach (var (x, top, w) in Ledges) // Each ledge
        {
            var r = new Rect(x, top - 10f, w, 10f); // Its shape; only the top holds the explorer
            Tiled("Ledge", "ledge", r, 6, geometry.transform); // Draws the ledge
            ledges.Add(r); // Keep its shape
        }
        geometry.ledges = ledges.ToArray(); // All ledges

        float goalX = LevelCatalog.Find(Episode.Mountains, Difficulty.Easy, 1).Goal.X; // The goal from the level data
        var cave = M.Renderer("Cave", S("cave"), 4, geometry.transform); // Cave entrance at the end
        cave.transform.position = LevelGeometry.ToWorld(goalX + 20f, 28f); // Its 56-pixel height sits on the ground; its opening is at the goal
        var goalFlag = M.Renderer("Goal Flag", S("flag"), 6, geometry.transform); // Flag beside the cave
        goalFlag.transform.position = LevelGeometry.ToWorld(goalX - 28f, 14f); // Just before the opening; its 28-pixel height sits on the ground

        var explorer = new GameObject("Explorer").AddComponent<ExplorerMotor>(); // The explorer
        var body = M.Renderer("Body", Spr("Explorer/explorer_08"), 20, explorer.transform); // The explorer picture, in front of the level
        body.transform.localPosition = new Vector3(0f, 15f / LevelGeometry.PixelsPerUnit, 0f); // Feet (15 pixels below the picture's middle) at the position
        explorer.body = body; // The picture the motor animates
        explorer.geometry = geometry; // What the explorer stands on
        explorer.walkFrames = Enumerable.Range(0, 8).Select(i => Spr($"Explorer/explorer_{i:00}")).ToArray(); // Walk cycle, as on the start screen
        explorer.standFrame = Spr("Explorer/explorer_08"); // Standing
        explorer.riseFrame = Spr("Explorer/explorer_03"); // Long stride for going up
        explorer.fallFrame = Spr("Explorer/explorer_07"); // Feet together for coming down
        explorer.flipFrames = Enumerable.Range(0, 4).Select(i => S($"Flip/flip_{i:00}")).ToArray(); // Somersault for the double jump
        explorer.dustFrames = Enumerable.Range(0, 3).Select(i => Spr($"Dust/dust_{i:00}")).ToArray(); // Dust puff for the double jump

        var follow = ppc.gameObject.AddComponent<CameraFollow>(); // The camera follows the explorer
        follow.target = explorer; // Follow the explorer
        follow.geometry = geometry; // Stop at the level's ends

        var music = K.MenuMusic(); // Menu music for now; ticket 12 adds the Mountains music and breeze
        K.Events(); // Sends taps to the buttons
        var root = K.Canvas("Level UI", ppc, out var group); // UI with the same whole-number zoom as the art

        var controls = M.Rect("Touch Controls", root, K.CanvasCenter, K.CanvasCenter); // Holder for the on-screen buttons
        var input = new GameObject("Controls").AddComponent<LevelInput>(); // Joins touch and keyboard
        input.left = Hold("Left", controls, "btn_left", new Vector2(34f, 240f)); // Bottom-left: walk left
        input.right = Hold("Right", controls, "btn_right", new Vector2(80f, 240f)); // Next to it: walk right
        input.jump = Hold("Jump", controls, "btn_jump", new Vector2(444f, 238f)); // Bottom-right: jump
        explorer.input = input; // The explorer reads these controls

        var controller = new GameObject("Level Controller").AddComponent<LevelController>(); // The level brain
        controller.explorer = explorer; // The explorer
        controller.touchControls = controls.gameObject; // Hidden after the goal
        BuildResult(controller, root); // The basic result panel
        K.Wire(controller, root, font, music, group); // Guide panel, music, click and canvas group

        Directory.CreateDirectory(Path.GetDirectoryName(LevelPath)); // Make the Scenes folder if it is missing
        EditorSceneManager.SaveScene(scene, LevelPath); // Save the scene
        K.AddSceneAfter(LevelPath, LessonPath); // Right after the Lesson Screen in the build list
        Debug.Log($"Level built: {LevelPath}. Open StartScreen, press Play, and go Mountains > Easy > 1 > Start. Arrows or A/D walk; Space jumps."); // Tell the user it worked
    }

    static void BuildResult(LevelController controller, Transform root) // Makes the hidden "Level cleared!" panel
    {
        var c = K.CanvasCenter; // Middle of the screen
        var panel = M.Panel("Result", root, c, new Vector2(220f, 128f)); // Cream panel
        M.Picture("Heading", panel, S("text_cleared"), new Vector2(240f, 94f), c); // "LEVEL CLEARED!"
        controller.resultStars = Enumerable.Range(0, 3).Select(i => M.Picture("Star " + (i + 1), panel, Sel("Trail/star_empty"), new Vector2(220f + i * 20f, 124f), c)).ToArray(); // Three stars in a row
        controller.starFull = Sel("Trail/star_full"); // Earned star
        controller.starEmpty = Sel("Trail/star_empty"); // Star not earned
        var image = M.Picture("Continue", panel, S("btn_continue"), new Vector2(240f, 166f), c); // Continue button
        image.raycastTarget = true; // Buttons must catch taps
        var button = image.gameObject.AddComponent<Button>(); // Makes it tappable
        button.transition = Selectable.Transition.SpriteSwap; // Change picture instead of colour when pressed
        button.targetGraphic = image; // The picture that changes
        button.spriteState = new SpriteState { pressedSprite = S("btn_continue_pressed") }; // Pushed-in picture
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // No arrow-key moving between buttons
        controller.continueButton = button; // The controller connects it
        controller.resultPanel = panel.gameObject; // The controller shows it at the goal
    }

    static HoldButton Hold(string name, Transform parent, string sprite, Vector2 center) // Makes one on-screen touch button
    {
        var image = M.Picture(name, parent, S(sprite), center, K.CanvasCenter); // Button picture
        image.raycastTarget = true; // Catches touches
        var hold = image.gameObject.AddComponent<HoldButton>(); // Can be held down
        hold.image = image; // The picture it swaps
        hold.normal = S(sprite); // Normal picture
        hold.pressed = S(sprite + "_pressed"); // Held picture
        return hold; // The input reads it
    }

    static void Tiled(string name, string sprite, Rect r, int order, Transform parent) // Draws a repeated tile over a level-pixel rectangle
    {
        var sr = M.Renderer(name, S(sprite), order, parent); // The renderer
        sr.drawMode = SpriteDrawMode.Tiled; // Repeat the tile instead of stretching it
        sr.tileMode = SpriteTileMode.Continuous; // Plain repeating
        sr.size = new Vector2(r.width, r.height) / LevelGeometry.PixelsPerUnit; // Area to fill, in units
        sr.transform.position = LevelGeometry.ToWorld(r.center.x, r.center.y); // Centred on the rectangle
    }

    static Sprite S(string path) => K.Sprite(SpriteFolder, path, ArtScript); // Loads one level sprite
    static Sprite Spr(string path) => K.Sprite(StartSprites, path, "the start-screen art (already in the project)"); // Loads one start-screen sprite
    static Sprite Sel(string path) => K.Sprite(SelectionSprites, path, "Tools/Art/make_selection_art.py"); // Loads one selection sprite
}
