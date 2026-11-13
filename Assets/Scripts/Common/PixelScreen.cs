using System.Collections; // Gives access to IEnumerator for timed steps
using LandGrand.StartScreen; // Gives access to the shared music player
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, AudioSource and Application
using UnityEngine.InputSystem; // Gives access to the keyboard, where Android's Back button shows up as Escape
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand // Game code lives here
{
    /// <summary>
    /// Shared behaviour of the menu-style screens after the Main Menu (map, difficulty cards, level trail):
    /// a click on every tap, a short guide message, Back (and Android Back) going one step back,
    /// and leaving through the screen fade while the music fades out.
    /// </summary>
    public abstract class PixelScreen : MonoBehaviour // Base for the map and selection screens
    {
        [Header("Guide")] // Inspector heading for the guide message
        public GameObject guidePanel; // Small panel that shows a guide text
        public TMP_Text guideText; // The guide words
        public float guideSeconds = 2.5f; // How long the guide stays on screen

        [Header("Sound")] // Inspector heading for the sound settings
        public StartScreenMusic music; // Background music and ambience
        public float musicFadeOutSeconds = 0.6f; // How long the music takes to fade before leaving
        public AudioClip clickSound; // Short sound played by every tap
        public CanvasGroup ui; // Whole screen UI; used to stop taps while leaving

        [Header("Back")] // Inspector heading for the Back icon
        public Button backButton; // Back icon, top-left corner

        AudioSource sfx; // Plays the click sound
        Coroutine guideRoutine; // The running "hide the guide later" timer

        /// <summary>True once the screen is loading another scene.</summary>
        protected bool Leaving { get; private set; } // Stops taps while leaving

        /// <summary>What Back does on this screen: go one step back.</summary>
        protected abstract void Back(); // Each screen names its own step back

        protected virtual void Awake() // Runs once when the screen opens: sets up the click and hides the guide
        {
            sfx = gameObject.AddComponent<AudioSource>(); // Make a sound player for the click
            sfx.playOnAwake = false; // Do not play anything by itself
            foreach (var button in ui.GetComponentsInChildren<Button>(true)) button.onClick.AddListener(PlayClick); // Every tap makes the click sound
            guidePanel.SetActive(false); // No guide at the start
            if (backButton != null) backButton.onClick.AddListener(Back); // The Back icon goes one step back
        }

        /// <summary>Shows titles[index] at its exact pixel size, if there is one.</summary>
        protected static void ShowTitle(Image title, Sprite[] titles, int index) // Picks the title picture
        {
            if (index < 0 || index >= titles.Length) return; // No title for this choice
            title.sprite = titles[index]; // The title picture
            title.SetNativeSize(); // One sprite pixel = one UI pixel
        }

        protected virtual void Update() // Runs every frame: watches for the Android Back button
        {
            if (Leaving || ScreenFader.IsBusy) return; // Ignore Back while leaving or fading
            var keyboard = Keyboard.current; // The keyboard; on Android it carries the Back button
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return; // Do nothing unless Back (or Escape) was just pressed
            PlayClick(); // Back makes the same click as a button
            Back(); // Go one step back
        }

        /// <summary>Opens the scene if it is in the build list; otherwise says it is coming soon and stays.</summary>
        protected void OpenOrComingSoon(string scene) // Used for screens that later tickets build
        {
            if (Application.CanStreamedLevelBeLoaded(scene)) Leave(scene); // Built: go there
            else ShowGuide("Coming soon"); // Not built yet: stay here
        }

        /// <summary>Fades out and opens another scene. Only the first call counts.</summary>
        protected void Leave(string scene) // Leaves this screen
        {
            if (Leaving) return; // Leave only once
            Leaving = true; // Stop more taps and Back presses
            ui.interactable = false; // Make every button stop reacting
            if (music != null) StartCoroutine(music.FadeOut(musicFadeOutSeconds)); // Music fades while the screen darkens
            ScreenFader.LoadScene(scene, musicFadeOutSeconds); // Darken, open the scene, then fade it in
        }

        /// <summary>Shows a guide message for a short time. A new message restarts the timer.</summary>
        protected void ShowGuide(string text) // Shows a short message
        {
            guideText.text = text; // The words to show
            guidePanel.SetActive(true); // Show the panel
            if (guideRoutine != null) StopCoroutine(guideRoutine); // A new message restarts the timer
            guideRoutine = StartCoroutine(HideGuideLater()); // Hide it after a while
        }

        IEnumerator HideGuideLater() // Waits, then hides the guide
        {
            yield return new WaitForSecondsRealtime(guideSeconds); // Let the player read it
            guidePanel.SetActive(false); // Hide the panel
            guideRoutine = null; // No timer is running now
        }

        void PlayClick() // Plays the tap sound
        {
            if (clickSound != null) sfx.PlayOneShot(clickSound, GameSettings.SfxVolume); // Play the click at the player's sound-effects volume
        }
    }
}
