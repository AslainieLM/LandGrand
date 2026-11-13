using System.Collections; // Gives access to IEnumerator for timed steps
using LandGrand.StartScreen; // Gives access to the shared music player and background scrolling
using UnityEngine; // Gives access to MonoBehaviour, AudioSource and Application
using UnityEngine.InputSystem; // Gives access to the keyboard, where Android's Back button shows up as Escape
using UnityEngine.SceneManagement; // Gives access to scene loading
using UnityEngine.UI; // Gives access to Button

namespace LandGrand.MainMenu
{
    /// <summary>
    /// Runs the Main Menu (requirement LG-16): Play, Settings, Tutorial and Exit.
    /// Only one panel is open at a time. Android's Back button closes the open panel,
    /// or asks "Exit game?" when no panel is open.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main buttons")]
        public Button playButton; // Opens the Landform Map, or "Coming soon" until that scene exists
        public Button settingsButton; // Opens the Settings panel
        public Button tutorialButton; // Opens the Tutorial panel
        public Button exitButton; // Asks "Exit game?"
        public GameObject mainButtons; // Group that holds the four main buttons

        [Header("Panels")]
        public GameObject dim; // Dark see-through layer behind an open panel
        public GameObject settingsPanel; // Music and sound-effects volume
        public GameObject tutorialPanel; // Picture pages that explain how to play
        public GameObject comingSoonPanel; // Shown by Play while the Landform Map is not built
        public GameObject exitPanel; // "Exit game?" with Yes and No

        [Header("Panel buttons")]
        public Button[] backButtons; // Every Back button; each one closes its panel
        public Button exitYesButton; // Quits the game
        public Button exitNoButton; // Closes the "Exit game?" panel

        [Header("Play")]
        [Tooltip("Scene that Play opens. Until it is in the build list, Play shows the Coming Soon panel.")]
        public string playScene = "LandformMap"; // Name of the Landform Map scene (requirement LG-17)

        [Header("Sound")]
        public StartScreenMusic music; // Plays the Quest music and the ambience
        public float musicFadeOutSeconds = 0.6f; // How long the music takes to fade before leaving
        public AudioClip clickSound; // Short sound played by every button
        public CanvasGroup ui; // Whole menu; used to stop taps while leaving

        AudioSource sfx; // Plays the click sound
        GameObject openPanel; // The panel that is open now, or null when none is open
        bool leaving; // True once the menu is loading another scene or quitting

        void Awake() // Runs once when the menu opens: sets up sounds and buttons
        {
            ParallaxLayer.WalkSpeedScale = 1f; // The start screen may leave the background stopped; make it scroll again
            sfx = gameObject.AddComponent<AudioSource>(); // Make a sound player for the click
            sfx.playOnAwake = false; // Do not play anything by itself

            playButton.onClick.AddListener(Play); // Play button runs Play()
            settingsButton.onClick.AddListener(() => Open(settingsPanel)); // Settings button opens Settings
            tutorialButton.onClick.AddListener(() => Open(tutorialPanel)); // Tutorial button opens Tutorial
            exitButton.onClick.AddListener(() => Open(exitPanel)); // Exit button asks "Exit game?"
            foreach (var back in backButtons) back.onClick.AddListener(Close); // Every Back button closes its panel
            exitYesButton.onClick.AddListener(Quit); // Yes quits the game
            exitNoButton.onClick.AddListener(Close); // No closes the question

            var everyButton = ui.GetComponentsInChildren<Button>(true); // All menu buttons, also those in closed panels
            foreach (var button in everyButton) button.onClick.AddListener(PlayClick); // Every button makes the click sound

            Close(); // Start with no panel open
        }

        void Update() // Runs every frame: watches for the Back button
        {
            if (leaving) return; // Ignore the Back button while leaving
            var keyboard = Keyboard.current; // The keyboard; on Android it carries the Back button
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return; // Do nothing unless Back (or Escape) was just pressed
            PlayClick(); // Back makes the same click as a button
            if (openPanel != null) Close(); // Back closes the open panel
            else Open(exitPanel); // With no panel open, Back asks "Exit game?"
        }

        void Open(GameObject panel) // Shows one panel (or none) and hides the rest
        {
            settingsPanel.SetActive(panel == settingsPanel); // Show Settings only if it is the one asked for
            tutorialPanel.SetActive(panel == tutorialPanel); // Show Tutorial only if it is the one asked for
            comingSoonPanel.SetActive(panel == comingSoonPanel); // Show Coming Soon only if it is the one asked for
            exitPanel.SetActive(panel == exitPanel); // Show "Exit game?" only if it is the one asked for
            dim.SetActive(panel != null); // Darken the background while a panel is open
            mainButtons.SetActive(panel == null); // Hide the main buttons while a panel is open
            openPanel = panel; // Remember which panel is open
        }

        void Close() => Open(null); // Closing means "open no panel"

        void Play() // What the Play button does
        {
            if (Application.CanStreamedLevelBeLoaded(playScene)) StartCoroutine(Leave(() => SceneManager.LoadScene(playScene))); // Open the Landform Map if it is in the build
            else Open(comingSoonPanel); // Otherwise tell the player it is coming soon
        }

        void Quit() // What the Yes button in "Exit game?" does
        {
            StartCoroutine(Leave(() => // Fade the music first, then run the code below
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // In the Unity Editor, stop Play mode
#else
                Application.Quit(); // On Android, close the game
#endif
            }));
        }

        IEnumerator Leave(System.Action then) // Fades out, then runs the given step (load a scene or quit)
        {
            leaving = true; // Stop more taps and Back presses
            ui.interactable = false; // Make every button stop reacting
            yield return new WaitForSecondsRealtime(0.1f); // Let the click sound start before the music fades
            if (music != null) yield return music.FadeOut(musicFadeOutSeconds); // Fade the music out
            then(); // Load the next scene or quit
        }

        void PlayClick() // Plays the button click sound
        {
            if (clickSound != null) sfx.PlayOneShot(clickSound, GameSettings.SfxVolume); // Play the click at the player's sound-effects volume
        }
    }
}
