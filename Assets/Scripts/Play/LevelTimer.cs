using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, Color and Coroutine

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The countdown at the top centre. In the last 20 seconds it turns red, hops and flashes each second,
    /// and in the last 10 seconds a soft tick plays (D-058). The rules in LevelRun keep the time; this only shows it.
    /// </summary>
    public class LevelTimer : MonoBehaviour // One part of the level screen
    {
        public TMP_Text label; // The countdown text; its parent is the small cream panel that hops
        public Color normal = new Color32(31, 26, 36, 255); // Countdown colour
        public Color low = new Color32(168, 63, 36, 255); // Countdown colour in the last 20 seconds
        public Color flash = new Color32(230, 92, 50, 255); // Brighter red for a moment each second while time is low
        public float hopPixels = 2f; // How far the countdown hops up each second while time is low
        public float hopSeconds = 0.12f; // How long each hop and flash lasts
        public AudioClip tickSound; // Soft tick each second in the last 10 seconds

        LevelController level; // The level this countdown belongs to
        int lastShownSecond = -1; // The second shown last, to hop once per new second
        Vector2 home; // Where the countdown panel rests
        Coroutine hop; // The hop running now, if any

        internal bool IsBuilt => label != null; // False for a scene built before this part existed

        internal void Begin(LevelController owner) // Connects the countdown to its level and shows the start time
        {
            level = owner; // Remember the level
            home = ((RectTransform)label.transform.parent).anchoredPosition; // The panel's resting place, for the hop
            Show(); // Show 2:00
        }

        internal void Show() // Shows the time left as minutes and seconds
        {
            var run = level.Run; // The attempt that keeps the time
            int seconds = Mathf.CeilToInt(run.RemainingSeconds); // Whole seconds left, rounded up so 0:00 only shows at the end
            label.text = (seconds / 60) + ":" + (seconds % 60).ToString("00"); // For example 1:05
            if (seconds != lastShownSecond) // A new second began
            {
                if (lastShownSecond >= 0 && run.IsTimeLow) // In the last 20 seconds
                {
                    if (hop != null) StopCoroutine(hop); // Finish any earlier hop at once
                    hop = StartCoroutine(Hop()); // Hop and flash
                    if (run.IsTicking) level.PlaySfx(tickSound); // A soft tick in the last 10 seconds
                }
                lastShownSecond = seconds; // Remember it
            }
            if (hop == null) label.color = run.IsTimeLow || run.IsTimeUp ? low : normal; // Red in the last 20 seconds, and at 0:00
        }

        System.Collections.IEnumerator Hop() // The countdown jumps up a little and flashes bright red, then settles
        {
            var panel = (RectTransform)label.transform.parent; // The small cream countdown panel
            panel.anchoredPosition = home + new Vector2(0f, hopPixels); // Up by whole pixels, so it stays sharp
            label.color = flash; // Bright red
            for (float t = 0f; t < hopSeconds; t += Time.unscaledDeltaTime) yield return null; // A short moment
            panel.anchoredPosition = home; // Back down
            label.color = level.Run.IsTimeLow ? low : normal; // Steady red again
            hop = null; // Done
        }
    }
}
