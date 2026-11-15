using UnityEngine; // Gives access to MonoBehaviour and GameObject
using UnityEngine.UI; // Gives access to Button

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The pause button, the pause menu (Resume, Retry, Main Menu) and the Time's up panel (Retry, Main Menu) (D-057).
    /// Retry starts again from the Lesson Screen (D-040).
    /// </summary>
    public class LevelMenus : MonoBehaviour // One part of the level screen
    {
        public Button pauseButton; // Pause button, top right
        public GameObject pausePanel; // Pause menu with its dark backdrop
        public Button resumeButton; // Pause menu: back to the game
        public Button pauseRetryButton; // Pause menu: start again from the Lesson Screen
        public Button pauseLevelsButton; // Pause menu: leave to the level selection (D-064)
        public GameObject timeUpPanel; // Time's up panel with its dark backdrop
        public Button timeUpRetryButton; // Time's up: start again from the Lesson Screen
        public Button timeUpLevelsButton; // Time's up: leave to the level selection (D-064)

        LevelController level; // The level these menus belong to

        internal bool IsBuilt => pauseButton != null && pausePanel != null && timeUpPanel != null && pauseLevelsButton != null && timeUpLevelsButton != null; // False for a scene built before this part existed, or before the Levels button (D-064)

        internal void Begin(LevelController owner) // Connects the buttons and hides the menus
        {
            level = owner; // Remember the level
            pausePanel.SetActive(false); // Menus start hidden
            timeUpPanel.SetActive(false); // Menus start hidden
            pauseButton.onClick.AddListener(OpenPause); // The pause button opens the menu
            resumeButton.onClick.AddListener(ClosePause); // Resume closes it
            pauseRetryButton.onClick.AddListener(() => level.GoTo(level.lessonScene)); // Retry starts again from the Lesson Screen
            pauseLevelsButton.onClick.AddListener(() => level.GoTo(level.levelSelectionScene)); // Back to the level selection, to pick another level
            timeUpRetryButton.onClick.AddListener(() => level.GoTo(level.lessonScene)); // Retry starts again from the Lesson Screen
            timeUpLevelsButton.onClick.AddListener(() => level.GoTo(level.levelSelectionScene)); // Back to the level selection, to pick another level
        }

        internal void OpenPause() // Opens the pause menu
        {
            if (level.hazards.Respawning || !level.Run.Pause()) return; // Not while being sent back, behind the pop-up or after the end
            if (level.music != null) level.music.DuckTo(0.3f); // The music and the breeze play on, but quietly, while paused
            level.explorer.Frozen = true; // The explorer stops
            pausePanel.SetActive(true); // Show the menu; its backdrop blocks the controls
        }

        internal void ClosePause() // Closes the pause menu
        {
            level.Run.Resume(); // The game clock runs again
            if (level.music != null) level.music.DuckTo(1f); // Back to normal loudness
            pausePanel.SetActive(false); // Hide the menu
            level.explorer.Frozen = false; // The explorer can move again
        }

        internal void HidePauseButton() => pauseButton.gameObject.SetActive(false); // No pause button on the Result Screen or after time is up

        internal void ShowTimeUp() // Time ran out: close the pause menu and offer Retry or Back to Main Menu
        {
            HidePauseButton(); // No pause button now
            pausePanel.SetActive(false); // The pause menu cannot stay open
            if (level.music != null) level.music.DuckTo(0.3f); // Quiet music under the Time's up panel
            timeUpPanel.SetActive(true); // Show the Time's up panel
        }
    }
}
