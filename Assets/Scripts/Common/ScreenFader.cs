using System.Collections; // Gives access to IEnumerator for timed steps
using UnityEngine; // Gives access to MonoBehaviour, Mathf and Time
using UnityEngine.SceneManagement; // Gives access to scene loading
using UnityEngine.UI; // Gives access to Canvas tools and Image

namespace LandGrand
{
    /// <summary>
    /// Screen transition used between every LandGrand screen: the screen fades to dark in a few
    /// pixel-style steps, the next scene loads, then it fades back in. The fader lives across scenes,
    /// and it blocks taps while the screen is dark so nothing is pressed by accident.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        const int Steps = 6; // Number of brightness steps; few steps look like old pixel games
        static readonly Color Dark = new Color32(31, 26, 36, 255); // Palette "Outline" colour

        static ScreenFader instance; // The one fader shared by all scenes

        Image cover; // Full-screen dark picture that fades in and out
        bool busy; // True while a transition is running

        /// <summary>True while a transition is running.</summary>
        public static bool IsBusy => instance != null && instance.busy; // Lets screens ignore taps during a transition

        /// <summary>Fades out, loads the scene, and fades back in. Does nothing if a transition is already running.</summary>
        public static void LoadScene(string sceneName, float outSeconds = 0.6f, float inSeconds = 0.5f) // Start a transition to another scene
        {
            var fader = Get(); // Find or make the fader
            if (fader.busy) return; // Only one transition at a time
            fader.StartCoroutine(fader.Run(sceneName, outSeconds, inSeconds)); // Run it on the fader, which survives the scene change
        }

        /// <summary>Fades the screen to dark and stays dark. Use it before quitting the game.</summary>
        public static IEnumerator FadeOutOnly(float seconds = 0.4f) // Darken the screen and keep it dark
        {
            var fader = Get(); // Find or make the fader
            fader.busy = true; // Block taps from now on
            yield return fader.Fade(0f, 1f, seconds); // Fade to dark
        }

        static ScreenFader Get() // Finds the fader, or makes it the first time
        {
            if (instance != null) return instance; // Reuse the fader if it already exists

            var go = new GameObject("Screen Fader"); // New object for the fader
            DontDestroyOnLoad(go); // Keep it when scenes change
            var canvas = go.AddComponent<Canvas>(); // Canvas that draws the dark cover
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Draw on top of the whole screen
            canvas.sortingOrder = short.MaxValue; // In front of every other canvas
            go.AddComponent<GraphicRaycaster>(); // Lets the cover catch taps

            var coverGo = new GameObject("Cover", typeof(RectTransform)); // Object for the dark picture
            coverGo.transform.SetParent(go.transform, false); // Put it under the canvas
            var rt = (RectTransform)coverGo.transform; // Its UI position box
            rt.anchorMin = Vector2.zero; // Stretch from the bottom-left corner...
            rt.anchorMax = Vector2.one; // ...to the top-right corner
            rt.offsetMin = Vector2.zero; // No gap at the bottom-left
            rt.offsetMax = Vector2.zero; // No gap at the top-right

            instance = go.AddComponent<ScreenFader>(); // The fader script
            instance.cover = coverGo.AddComponent<Image>(); // The dark picture
            instance.SetLevel(0f); // Start see-through
            return instance; // Give it back
        }

        IEnumerator Run(string sceneName, float outSeconds, float inSeconds) // The whole transition, step by step
        {
            busy = true; // Block taps during the whole transition
            yield return Fade(0f, 1f, outSeconds); // Fade to dark
            var loading = SceneManager.LoadSceneAsync(sceneName); // Start loading the next scene
            while (loading != null && !loading.isDone) yield return null; // Wait until it is loaded
            yield return null; // Wait one more frame so the new scene has drawn once
            yield return Fade(1f, 0f, inSeconds); // Fade back in on the new scene
            busy = false; // Taps work again
        }

        IEnumerator Fade(float from, float to, float seconds) // Changes the darkness over time
        {
            float time = 0f; // Seconds since the fade started
            while (time < seconds) // Until the fade time is over
            {
                time += Time.unscaledDeltaTime; // Add this frame's time, even if the game is paused
                SetLevel(Mathf.Lerp(from, to, Mathf.Clamp01(time / seconds))); // Darkness for this moment
                yield return null; // Wait for the next frame
            }
            SetLevel(to); // End exactly at the target
        }

        void SetLevel(float level) // Sets how dark the screen is, from 0 (clear) to 1 (dark)
        {
            float stepped = Mathf.Round(level * Steps) / Steps; // Snap to a few steps for a pixel-style fade
            cover.color = new Color(Dark.r, Dark.g, Dark.b, stepped); // Dark colour at that strength
            cover.raycastTarget = stepped > 0f; // Block taps whenever any darkness shows
            cover.enabled = stepped > 0f; // Do not draw it when fully see-through
        }
    }
}
