using System.Collections; // Gives access to IEnumerator for timed steps
using LandGrand.StartScreen; // Gives access to the shared music player and background scrolling
using UnityEngine; // Gives access to MonoBehaviour, AudioSource and Application
using UnityEngine.InputSystem; // Gives access to the keyboard, where Android's Back button shows up as Escape
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
        public NamePanel namePanel; // Asks for the player's name the first time Play is tapped
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

        [Header("Entrance")]
        public float enterDelaySeconds = 0.2f; // Wait before the first button arrives, while the screen fades in
        public float enterStaggerSeconds = 0.08f; // Gap between one button and the next
        public float enterSeconds = 0.25f; // How long each button takes to arrive
        public float enterRisePixels = 16f; // How far below its place each button starts

        AudioSource sfx; // Plays the click sound
        bool entering; // True while the buttons are arriving
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
            namePanel.Accepted += OpenPlayScene; // A saved name carries on to the Landform Map

            var everyButton = ui.GetComponentsInChildren<Button>(true); // All menu buttons, also those in closed panels
            foreach (var button in everyButton) button.onClick.AddListener(PlayClick); // Every button makes the click sound

            Close(); // Start with no panel open
        }

        void Start() // Runs once before the first frame
        {
            StartCoroutine(Enter()); // Bring the four buttons in, one after another
        }

        IEnumerator Enter() // Slides the main buttons up into place and fades them in, as a sign the new screen has opened
        {
            entering = true; // Ignore Back while the buttons arrive
            ui.interactable = false; // No taps until every button is in place
            var buttons = new[] { playButton, settingsButton, tutorialButton, exitButton }; // Top to bottom
            var homes = new Vector2[buttons.Length]; // Each button's final place
            var fades = new CanvasGroup[buttons.Length]; // Each button's see-through control
            for (int i = 0; i < buttons.Length; i++) // Get each button ready
            {
                var rt = (RectTransform)buttons[i].transform; // Its UI position box
                homes[i] = rt.anchoredPosition; // Remember where it belongs
                fades[i] = buttons[i].gameObject.AddComponent<CanvasGroup>(); // Lets it fade in
                fades[i].alpha = 0f; // Start hidden
            }

            float start = Time.unscaledTime; // When the entrance began
            bool done = false; // True once every button has arrived
            while (!done) // Each frame until all have arrived
            {
                done = true; // Assume done until a button is still moving
                for (int i = 0; i < buttons.Length; i++) // Move each button
                {
                    float begin = enterDelaySeconds + i * enterStaggerSeconds; // When this button starts
                    float t = Mathf.Clamp01((Time.unscaledTime - start - begin) / enterSeconds); // Its progress, from 0 to 1
                    if (t < 1f) done = false; // Still moving
                    float eased = 1f - (1f - t) * (1f - t) * (1f - t); // Fast at first, gentle at the end
                    float below = Mathf.Round(enterRisePixels * (1f - eased)); // Whole pixels still to rise
                    ((RectTransform)buttons[i].transform).anchoredPosition = homes[i] - new Vector2(0f, below); // Place it that far below home
                    fades[i].alpha = Mathf.Round(t * 4f) / 4f; // Fade in, in a few pixel-style steps
                }
                yield return null; // Wait for the next frame
            }

            entering = false; // Back works again
            if (!leaving) ui.interactable = true; // Buttons can be tapped now
        }

        void Update() // Runs every frame: watches for the Back button
        {
            if (leaving || entering || ScreenFader.IsBusy) return; // Ignore the Back button while leaving, arriving or fading
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
            namePanel.gameObject.SetActive(panel == namePanel.gameObject); // Show the name panel only if it is the one asked for
            comingSoonPanel.SetActive(panel == comingSoonPanel); // Show Coming Soon only if it is the one asked for
            exitPanel.SetActive(panel == exitPanel); // Show "Exit game?" only if it is the one asked for
            dim.SetActive(panel != null); // Darken the background while a panel is open
            mainButtons.SetActive(panel == null); // Hide the main buttons while a panel is open
            openPanel = panel; // Remember which panel is open
        }

        void Close() => Open(null); // Closing means "open no panel"

        void Play() // What the Play button does
        {
            if (string.IsNullOrEmpty(GameProgress.Current.Name)) Open(namePanel.gameObject); // First time: ask for the name before anything else
            else OpenPlayScene(); // Name already saved: go straight on
        }

        void OpenPlayScene() // Opens the Landform Map, or says it is coming soon
        {
            if (Application.CanStreamedLevelBeLoaded(playScene)) // If the Landform Map is in the build
            {
                BeginLeaving(); // Stop taps and fade the music
                ScreenFader.LoadScene(playScene, musicFadeOutSeconds); // Darken, open the Landform Map, then fade it in
            }
            else Open(comingSoonPanel); // Otherwise tell the player it is coming soon
        }

        void Quit() // What the Yes button in "Exit game?" does
        {
            BeginLeaving(); // Stop taps and fade the music
            StartCoroutine(QuitAfterFade()); // Darken the screen, then close the game
        }

        void BeginLeaving() // Shared first step before leaving the menu
        {
            leaving = true; // Stop more taps and Back presses
            ui.interactable = false; // Make every button stop reacting
            if (music != null) StartCoroutine(music.FadeOut(musicFadeOutSeconds)); // Music fades while the screen darkens
        }

        IEnumerator QuitAfterFade() // Darkens the screen, then quits
        {
            yield return ScreenFader.FadeOutOnly(musicFadeOutSeconds); // Wait for the screen to go dark
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // In the Unity Editor, stop Play mode
#else
            Application.Quit(); // On Android, close the game
#endif
        }

        void PlayClick() // Plays the button click sound
        {
            if (clickSound != null) sfx.PlayOneShot(clickSound, GameSettings.SfxVolume); // Play the click at the player's sound-effects volume
        }
    }
}
