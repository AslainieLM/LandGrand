using System; // Gives access to Serializable
using System.Collections; // Gives access to IEnumerator for timed steps
using LandGrand.Rules; // Gives access to Episode, PlayerProgress and RegionGuide
using LandGrand.StartScreen; // Gives access to the shared music player
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, AudioSource and Application
using UnityEngine.InputSystem; // Gives access to the keyboard, where Android's Back button shows up as Escape
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>
    /// Runs the Landform Map (requirement LG-17, decision D-041). Each region follows the saved progress:
    /// unlocked regions show colour and animate; locked ones are grey with a padlock, stay still,
    /// and show a guide such as "Finish Mountains first" when tapped. Back returns to the Main Menu.
    /// </summary>
    public class LandformMapController : MonoBehaviour // The map brain
    {
        /// <summary>The parts of one region on the map.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class Region // One landform's region
        {
            public Episode episode; // Which landform this region is
            public Image locked; // Grey copy of the region; also the region's tap area
            public Button button; // Reacts when the region is tapped
            public GameObject padlock; // Padlock picture shown while locked
            public GameObject life; // Animations shown only while unlocked (may be empty)
            public TMP_Text stars; // Star progress, for example "0/45"
        }

        public Region[] regions; // The four regions
        public TMP_Text playerName; // The player's name at the top
        public Button backButton; // Back, top-left corner
        public GameObject guidePanel; // Small panel that shows a guide text
        public TMP_Text guideText; // The guide words
        public float guideSeconds = 2.5f; // How long the guide stays on screen

        [Header("Scenes")] // Inspector heading for the scene names
        public string mainMenuScene = "MainMenu"; // Where Back goes
        [Tooltip("Scene that an unlocked region opens. Until it is in the build list, the region says it is coming soon.")]
        public string difficultyScene = "DifficultySelection"; // Difficulty selection (ticket 05)

        [Header("Sound")] // Inspector heading for the sound settings
        public StartScreenMusic music; // Background music and ambience
        public float musicFadeOutSeconds = 0.6f; // How long the music takes to fade before leaving
        public AudioClip clickSound; // Short sound played by every tap
        public CanvasGroup ui; // Whole map UI; used to stop taps while leaving

        AudioSource sfx; // Plays the click sound
        bool leaving; // True once the map is loading another scene
        Coroutine guideRoutine; // The running "hide the guide later" timer

        void Awake() // Runs once when the map opens: shows the saved progress and connects the buttons
        {
            sfx = gameObject.AddComponent<AudioSource>(); // Make a sound player for the click
            sfx.playOnAwake = false; // Do not play anything by itself

            var progress = GameProgress.Current; // The saved player progress, loaded at launch
            playerName.text = progress.Name; // Show the player's name at the top
            foreach (var region in regions) // Set up each region
            {
                bool open = progress.IsEpisodeUnlocked(region.episode); // Can this landform be played?
                region.locked.alphaHitTestMinimumThreshold = 0.5f; // Only the region's own pixels catch taps, not its see-through rest
                region.locked.canvasRenderer.cullTransparentMesh = false; // Keep a hidden (unlocked) region tappable
                region.locked.color = open ? Color.clear : Color.white; // Unlocked: hide the grey copy but keep it as the tap area
                region.padlock.SetActive(!open); // Padlock only while locked
                if (region.life != null) region.life.SetActive(open); // Locked regions do not move
                region.stars.text = progress.EpisodeStars(region.episode) + "/" + PlayerProgress.MaxEpisodeStars; // For example "0/45"
                var tapped = region; // Copy for the button listener below
                region.button.onClick.AddListener(() => Choose(tapped, open)); // Tapping the region runs Choose()
            }
            backButton.onClick.AddListener(Back); // Back returns to the Main Menu
            foreach (var button in ui.GetComponentsInChildren<Button>(true)) button.onClick.AddListener(PlayClick); // Every tap makes the click sound
            guidePanel.SetActive(false); // No guide at the start
        }

        void Update() // Runs every frame: watches for the Android Back button
        {
            if (leaving || ScreenFader.IsBusy) return; // Ignore Back while leaving or fading
            var keyboard = Keyboard.current; // The keyboard; on Android it carries the Back button
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return; // Do nothing unless Back (or Escape) was just pressed
            PlayClick(); // Back makes the same click as a button
            Back(); // Go back to the Main Menu
        }

        void Choose(Region region, bool open) // What tapping a region does
        {
            if (leaving) return; // Ignore taps while leaving
            if (!open) { ShowGuide(RegionGuide.ForLocked(region.episode)); return; } // Locked: explain and stay on the map
            if (Application.CanStreamedLevelBeLoaded(difficultyScene)) Leave(difficultyScene); // Open difficulty selection
            else ShowGuide("Coming soon"); // Difficulty selection is not built yet
        }

        void Back() => Leave(mainMenuScene); // Back always returns to the Main Menu

        void Leave(string scene) // Fades out and opens another scene
        {
            if (leaving) return; // Leave only once
            leaving = true; // Stop more taps and Back presses
            ui.interactable = false; // Make every button stop reacting
            if (music != null) StartCoroutine(music.FadeOut(musicFadeOutSeconds)); // Music fades while the screen darkens
            ScreenFader.LoadScene(scene, musicFadeOutSeconds); // Darken, open the scene, then fade it in
        }

        void ShowGuide(string text) // Shows a guide for a short time
        {
            guideText.text = text; // The words to show
            guidePanel.SetActive(true); // Show the panel
            if (guideRoutine != null) StopCoroutine(guideRoutine); // A new tap restarts the timer
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
