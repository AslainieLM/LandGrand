using System.IO; // Checks the Plains sprite folder
using LandGrand.LandformMap; // Gives access to CloudDrift and CircleFlight
using LandGrand.StartScreen; // Gives access to SpriteFlipbook
using UnityEngine; // Gives access to GameObject, Sprite and Vector2
using K = PixelSceneKit; // Short name for the shared scene parts
using M = global::MainMenuBuilder; // Short name for the Main Menu builder, whose UI helpers we reuse

/// <summary>
/// Shared life pictures (cloud, birds and grass) for the Landform Map and the menu screens (ticket 05, spec section 9).
/// Screen positions are pixels on the 480x270 screen, measured from the top-left corner.
/// </summary>
internal static class LandformLife // Makes the small animations that belong to a landform
{
    internal const string MapSprites = "Assets/Art/LandformMap/Sprites/"; // Landform Map PNGs
    const string PlainsFolder = "Plains/"; // Holds the Plains map and menu sprites
    const string HillsArt = "Tools/Art/make_hills_easy_art.py"; // Script that draws the Hills life pictures
    const string MapArt = "Tools/Art/make_landform_map_art.py"; // Script that draws the cloud

    /// <summary>One Hills life picture, such as "tree_0" or "bird_1".</summary>
    internal static Sprite HillsSprite(string name) => K.Sprite(MapSprites, "Hills/" + name, HillsArt); // Loads it from the Hills folder

    internal static void ConfigurePlainsSprites() // Imports the shared life art at the game's pixel scale
    { // Opens the import
        string folder = MapSprites + PlainsFolder; // The prepared Plains pictures
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"Missing {folder}. Run python {K.PlainsEasyArtScript}"); // Names the generator for the missing folder
        M.ConfigurePixelSprites(folder, _ => false); // Uses crisp sprites for map and menus
    } // Ends the import

    static Sprite PlainsSprite(string name) => K.Sprite(MapSprites, PlainsFolder + name, K.PlainsEasyArtScript); // Loads one prepared Plains life picture

    const string PlateausFolder = "Plateaus/"; // Holds the Plateaus map and menu sprites (D-072)

    internal static void ConfigurePlateausSprites() // Imports the Plateaus life art at the game's pixel scale
    { // Opens the import
        string folder = MapSprites + PlateausFolder; // The prepared Plateaus pictures
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"Missing {folder}. Run python {K.PlateausEasyArtScript}"); // Names the generator for the missing folder
        M.ConfigurePixelSprites(folder, _ => false); // Uses crisp sprites for map and menus
    } // Ends the import

    static Sprite PlateausSprite(string name) => K.Sprite(MapSprites, PlateausFolder + name, K.PlateausEasyArtScript); // Loads one prepared Plateaus life picture

    internal static void Hawk(Transform parent, Vector2 centerPixels, Vector2 radiusPixels, float secondsPerLap) // Makes one Plateaus hawk
        => FlyingBird("Hawk", parent, centerPixels, radiusPixels, secondsPerLap, new[] { PlateausSprite("hawk_0"), PlateausSprite("hawk_1") }); // Flaps at the shared 3 frames per second

    internal static void PlateausStandingLife(Transform parent, string name, string file, Vector2 bottom, float framesPerSecond, int order) // Sets a swaying tuft or a shining waterfall on its spot
        => StandingLife(parent, name, new[] { PlateausSprite(file + "_0"), PlateausSprite(file + "_1") }, bottom, framesPerSecond, order); // Uses the Plateaus poses

    /// <summary>A cloud that drifts across the sky. The same slow drift on every landform.</summary>
    internal static CloudDrift Cloud(Transform parent) // Makes one drifting cloud
    {
        var cloud = M.Renderer("Cloud", K.Sprite(MapSprites, "Mountains/cloud", MapArt), 10, parent).gameObject.AddComponent<CloudDrift>(); // Cloud picture with the drift script
        ConfigureCloud(cloud); // Same speed and path for every landform
        return cloud; // Give it back so callers can tweak it
    }

    /// <summary>Sets the cloud's path: across the whole sky, slowly, 40 pixels down from the top.</summary>
    internal static void ConfigureCloud(CloudDrift cloud) // Puts one cloud on the shared path
    {
        cloud.startX = 20f; // Starts at the left of the sky
        cloud.endX = 460f; // Ends at the right of the sky
        cloud.y = 40f; // Height in the sky
        cloud.speedPixelsPerSecond = 2.5f; // Drifts slowly
    }

    /// <summary>A bird that flaps its wings and flies round on a slow oval.</summary>
    internal static void Bird(Transform parent, Vector2 centerPixels, Vector2 radiusPixels, float secondsPerLap) // Makes one Hills bird
        => FlyingBird("Bird", parent, centerPixels, radiusPixels, secondsPerLap, new[] { HillsSprite("bird_0"), HillsSprite("bird_1") }); // Keeps the Hills wings

    internal static void Egret(Transform parent, Vector2 centerPixels, Vector2 radiusPixels, float secondsPerLap) // Makes one Plains egret
        => FlyingBird("Egret", parent, centerPixels, radiusPixels, secondsPerLap, new[] { PlainsSprite("egret_0"), PlainsSprite("egret_1") }); // Uses the white wings

    static void FlyingBird(string name, Transform parent, Vector2 centerPixels, Vector2 radiusPixels, float secondsPerLap, Sprite[] frames) // Sets up the shared flight
    { // Opens one bird
        var bird = M.Renderer(name, frames[0], 15, parent); // Bird picture, drawn in front of the picture
        var flaps = bird.gameObject.AddComponent<SpriteFlipbook>(); // Flaps the wings
        flaps.frames = frames; // Wings up, wings down
        flaps.framesPerSecond = 3f; // Quick flaps, like a small bird
        var flight = bird.gameObject.AddComponent<CircleFlight>(); // Flies round the oval
        flight.centerPixels = centerPixels; // Middle of the oval
        flight.radiusPixels = radiusPixels; // Size of the oval
        flight.secondsPerLap = secondsPerLap; // Time for one lap
    }

    internal static void PlainsStandingLife(Transform parent, string name, string file, Vector2 bottom, float framesPerSecond, int order) // Sets a swaying plant or grazing animal on its spot
        => StandingLife(parent, name, new[] { PlainsSprite(file + "_0"), PlainsSprite(file + "_1") }, bottom, framesPerSecond, order); // Loads the two Plains poses

    static void StandingLife(Transform parent, string name, Sprite[] frames, Vector2 bottom, float framesPerSecond, int order) // Sets two poses on a spot, shared by Plains and Plateaus
    { // Opens the placement
        var view = M.Renderer(name, frames[0], order, parent); // Draws the first pose
        view.transform.position = M.Px(bottom.x, bottom.y - frames[0].rect.height / 2f); // Rests the bottom on the chosen row
        var animation = view.gameObject.AddComponent<SpriteFlipbook>(); // Swaps the poses
        animation.frames = frames; // Connects the two poses
        animation.framesPerSecond = framesPerSecond; // Sets the sway or graze speed
    } // Ends the placement

    /// <summary>A grass tuft that sways in the wind. Its bottom sits on screen row 268.</summary>
    internal static void Grass(Transform parent, float x) // Makes one grass tuft at a screen x
    {
        var tuft = M.Renderer("Grass", HillsSprite("grass_0"), 5, parent); // Grass picture, drawn over the sky
        tuft.transform.position = M.Px(x, 265f); // Middle of the 6 pixel tuft, so its bottom is on row 268
        var sway = tuft.gameObject.AddComponent<SpriteFlipbook>(); // Sways the grass
        sway.frames = new[] { HillsSprite("grass_0"), HillsSprite("grass_1") }; // Two blade positions
        sway.framesPerSecond = 1.5f; // Slow sway
    }
}
