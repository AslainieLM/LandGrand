using System.Collections; // Gives access to IEnumerator for the fade-out
using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Application
using UnityEngine.InputSystem; // Gives access to taps and clicks
using UnityEngine.SceneManagement; // Gives access to scene loading

namespace LandGrand.StartScreen
{
    /// <summary>Waits for a tap anywhere, fades the music out, then opens the Main Menu scene.</summary>
    public class StartScreenController : MonoBehaviour
    {
        public string nextScene = "MainMenu"; // Scene a tap opens
        [Tooltip("Ignore taps for this long after the screen opens.")]
        public float ignoreTapsSeconds = 0.5f; // Short wait so an early tap is ignored
        public StartScreenMusic music; // Music player, so it can fade out
        public float musicFadeOutSeconds = 0.6f; // How long the fade-out takes

        bool leaving; // True once a tap has started leaving

        void Update() // Runs every frame
        {
            if (leaving || Time.timeSinceLevelLoad < ignoreTapsSeconds) return; // Ignore taps while leaving or right after opening
            var pointer = Pointer.current; // The touch screen or mouse
            if (pointer == null || !pointer.press.wasPressedThisFrame) return; // Do nothing unless it was just pressed

            if (Application.CanStreamedLevelBeLoaded(nextScene)) // If the next scene is in the build list
            {
                leaving = true; // Start leaving
                StartCoroutine(Leave()); // Fade out, then load
            }
            else // If the next scene is not in the build list
            {
                Debug.Log($"Tap received. Add a scene named '{nextScene}' to the build to continue."); // Tell the developer what is missing
            }
        }

        IEnumerator Leave() // Fades the music, then loads the next scene
        {
            if (music != null) yield return music.FadeOut(musicFadeOutSeconds); // Wait for the music to fade
            SceneManager.LoadScene(nextScene); // Open the next scene
        }
    }
}
