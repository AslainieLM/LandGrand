using System.Linq; // Loads each frame in order
using LandGrand; // Gives the shared unlock effect
using UnityEditor; // Loads the existing sound clips
using UnityEngine; // Gives scene objects and sprites
using UnityEngine.UI; // Gives button and image types
using K = PixelSceneKit; // Gives shared screen helpers
using M = global::MainMenuBuilder; // Gives picture helpers

internal static class DescriptionArt // Shares the completed landform art between its screens
{ // Opens the art helpers
    internal const string Folder = "Assets/Art/Description/Sprites/"; // Holds the description and unlock pictures
    const string ArtScript = "Tools/Art/make_hard_art.py"; // Makes the supplied pictures
    internal static Sprite Sprite(string name) => K.Sprite(Folder, name, ArtScript); // Loads one sprite
    internal static AudioClip Sound(string name) // Loads one Hard sound effect
    { // Starts the sound lookup
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name + ".wav"); // Uses a supplied recording
        if (clip == null) Debug.LogError($"Missing Assets/Audio/{name}.wav. Run python Tools/Audio/make_hard_audio.py"); // Names the missing sound
        return clip; // Gives the sound to the screen
    } // Ends the sound lookup

    internal static UnlockAnimation Unlock(Transform parent, Vector2 center, Vector2 parentCenter) // Makes an effect at the lock's position
    { // Starts the effect setup
        var animation = parent.gameObject.AddComponent<UnlockAnimation>(); // Runs the shared lock animation
        animation.padlockFrames = Enumerable.Range(0, 4).Select(i => Sprite($"unlock_padlock_{i:00}")).ToArray(); // The four lock pictures
        animation.sparkleFrames = Enumerable.Range(0, 3).Select(i => Sprite($"sparkle_{i:00}")).ToArray(); // The three sparkle pictures
        var offsets = new[] { new Vector2(-14f, -6f), new Vector2(14f, -8f), new Vector2(12f, 9f) }; // Spreads the sparkles around the lock
        animation.sparkles = offsets.Select(offset => M.Picture("Unlock Sparkle", parent, animation.sparkleFrames[0], center + offset, parentCenter)).ToArray(); // Places the three sparkles
        foreach (var spark in animation.sparkles) spark.gameObject.SetActive(false); // Hidden until the animation
        return animation; // Connects the effect to the screen
    } // Ends the effect setup

    internal static Button Button(string name, Transform parent, string sprite, Vector2 center) // Makes a button from supplied idle and pressed pictures
    { // Starts the button setup
        var image = M.Picture(name, parent, Sprite(sprite), center, K.CanvasCenter); // Draws its art
        image.raycastTarget = true; // Receives taps
        var button = image.gameObject.AddComponent<Button>(); // Makes the picture tappable
        button.transition = Selectable.Transition.SpriteSwap; // Uses the matching pressed picture
        button.targetGraphic = image; // Changes this picture on tap
        button.spriteState = new SpriteState { pressedSprite = Sprite(sprite + "_pressed") }; // Loads the pressed state
        button.navigation = new Navigation { mode = Navigation.Mode.None }; // Arrow keys do not move focus
        return button; // Gives the button to the screen
    } // Ends the button setup
} // Ends the art helpers
