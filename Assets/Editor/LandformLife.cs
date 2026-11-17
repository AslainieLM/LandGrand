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
    const string HillsArt = "Tools/Art/make_hills_easy_art.py"; // Script that draws the Hills life pictures
    const string MapArt = "Tools/Art/make_landform_map_art.py"; // Script that draws the cloud

    /// <summary>One Hills life picture, such as "tree_0" or "bird_1".</summary>
    internal static Sprite HillsSprite(string name) => K.Sprite(MapSprites, "Hills/" + name, HillsArt); // Loads it from the Hills folder

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
    internal static void Bird(Transform parent, Vector2 centerPixels, Vector2 radiusPixels, float secondsPerLap) // Makes one bird
    {
        var bird = M.Renderer("Bird", HillsSprite("bird_0"), 15, parent); // Bird picture, drawn in front of the picture
        var flaps = bird.gameObject.AddComponent<SpriteFlipbook>(); // Flaps the wings
        flaps.frames = new[] { HillsSprite("bird_0"), HillsSprite("bird_1") }; // Wings up, wings down
        flaps.framesPerSecond = 3f; // Quick flaps, like a small bird
        var flight = bird.gameObject.AddComponent<CircleFlight>(); // Flies round the oval
        flight.centerPixels = centerPixels; // Middle of the oval
        flight.radiusPixels = radiusPixels; // Size of the oval
        flight.secondsPerLap = secondsPerLap; // Time for one lap
    }

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
