using System; // Gives access to Action
using System.Collections; // Gives access to IEnumerator for the slide
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, RectTransform and Mathf

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The level start banner (D-055): the level's name slides down on a plate, stays a moment, then slides back up.
    /// The level controller holds the clock until it has gone. It runs on real time, so nothing can stall it.
    /// </summary>
    public class LevelBanner : MonoBehaviour // Slides the level name in and out
    {
        public RectTransform plate; // The plate with the name on it
        public TMP_Text label; // The level name
        public float shownY = 0f; // The plate's height offset while shown
        public float hiddenY = 60f; // The plate's height offset while hidden above the screen
        public float slideSeconds = 0.3f; // Time to slide in, and again to slide out
        public float holdSeconds = 0.9f; // Time the name stays still (about 1.5 s in all)

        /// <summary>Shows the name, then calls done. An empty name calls done at once.</summary>
        public void Play(string levelName, Action done) // Starts the banner
        {
            if (string.IsNullOrEmpty(levelName) || plate == null) { gameObject.SetActive(false); done?.Invoke(); return; } // No banner
            label.text = levelName; // The name to show
            gameObject.SetActive(true); // Make it visible
            StartCoroutine(Run(done)); // Slide in, wait, slide out
        }

        IEnumerator Run(Action done) // The whole banner
        {
            yield return Slide(hiddenY, shownY); // Slide down into view
            for (float t = 0f; t < holdSeconds; t += Time.unscaledDeltaTime) yield return null; // Stay a moment
            yield return Slide(shownY, hiddenY); // Slide back up
            done?.Invoke(); // The level may start
            gameObject.SetActive(false); // Hide it last, since this stops the banner's own steps
        }

        IEnumerator Slide(float from, float to) // Moves the plate between two heights
        {
            for (float t = 0f; t < slideSeconds; t += Time.unscaledDeltaTime) // Each frame of the slide
            {
                float k = t / slideSeconds; // 0 to 1 through the slide
                k = 1f - (1f - k) * (1f - k); // Ease out: fast start, soft stop
                SetY(Mathf.Lerp(from, to, k)); // Move the plate
                yield return null; // Next frame
            }
            SetY(to); // End exactly at the target
        }

        void SetY(float y) // Puts the plate at a height offset, on a whole pixel
        {
            var p = plate.anchoredPosition; // Where it is
            p.y = Mathf.Round(y); // Whole pixels keep the plate sharp
            plate.anchoredPosition = p; // Move it
        }
    }
}
