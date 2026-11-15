using UnityEngine; // Gives access to MonoBehaviour, RectTransform and Mathf
using UnityEngine.UI; // Gives access to Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The trail bar under the countdown (D-049): the green part grows and the explorer marker rides along
    /// as the explorer walks from the start to the goal. Its checkpoint flag turns green once reached.
    /// </summary>
    public class TrailProgressBar : MonoBehaviour // One part of the level screen
    {
        public RectTransform fill; // Green part of the bar; its width shows how far the explorer has walked
        public RectTransform marker; // The explorer marker riding on the bar
        public Image checkpoint; // The small checkpoint flag on the bar
        public Sprite checkpointOn; // Its green picture once the checkpoint is reached
        public float left; // Left edge of the bar's groove, as a UI offset from the screen's middle
        public float width = 142f; // Width of the groove in pixels

        LevelController level; // The level this bar belongs to

        internal bool IsBuilt => fill != null && marker != null; // False for a scene built before this part existed

        internal void Begin(LevelController owner) // Connects the bar to its level and shows the start
        {
            level = owner; // Remember the level
            Show(); // The marker starts at the left end
        }

        internal void Show() // Moves the green part and the marker to where the explorer is now
        {
            float walked = Mathf.Round(Mathf.Clamp01(level.explorer.Position.x / level.Run.Level.Goal.X) * width); // Whole pixels along the groove
            fill.sizeDelta = new Vector2(walked, fill.sizeDelta.y); // Green up to the explorer
            var at = marker.anchoredPosition; // The marker's place
            at.x = left + walked + 0.5f; // On the end of the green part; the half pixel keeps the 9-pixel marker on whole pixels
            marker.anchoredPosition = at; // Move it
        }

        internal void MarkCheckpoint() // The checkpoint was reached: its flag on the bar turns green
        {
            if (checkpoint != null) checkpoint.sprite = checkpointOn; // The green flag
        }
    }
}
