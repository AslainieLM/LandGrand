using System.Collections; // Gives timed steps for the unlock animation
using System; // Gives access to Serializable
using LandGrand.Rules; // Gives access to Episode, PlayerProgress and RegionGuide
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Color and Tooltip
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>
    /// Runs the Landform Map (requirement LG-17, decision D-041). Each region follows the saved progress:
    /// unlocked regions show colour and animate; locked ones are grey with a padlock, stay still,
    /// and show a guide such as "Finish Mountains first" when tapped. Back returns to the Main Menu.
    /// </summary>
    public class LandformMapController : PixelScreen // The map brain
    {
        /// <summary>The parts of one region on the map.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class Region // One landform's region
        {
            public Episode episode; // Which landform this region is
            public Image locked; // Grey copy of the region; also the region's tap area
            public Button button; // Reacts when the region is tapped
            public GameObject padlock; // Padlock picture shown while locked
            public UnlockAnimation unlock; // Opening lock and sparkles for this region
            public GameObject life; // Animations shown only while unlocked (may be empty)
            public TMP_Text stars; // Star progress, for example "0/45"
        }

        public Region[] regions; // The four regions
        public AudioClip unlockSound; // Plays once when a region opens
        bool unlocking; // Blocks taps while an unlock animation plays
        const string SeenPrefix = "LandGrand.UnlockSeen."; // Device setting for each landform

        [Header("Scenes")] // Inspector heading for the scene names
        public string mainMenuScene = "MainMenu"; // Where Back goes
        [Tooltip("Scene that an unlocked region opens. Until it is in the build list, the region says it is coming soon.")]
        public string difficultyScene = "DifficultySelection"; // Difficulty selection (ticket 05)

        protected override void Awake() // Runs once when the map opens: shows the saved progress and connects the buttons
        {
            base.Awake(); // Click sound and hidden guide
            var progress = GameProgress.Current; // The saved player progress, loaded at launch
            foreach (var region in regions) // Set up each region
            {
                bool open = progress.IsEpisodeUnlocked(region.episode); // Can this landform be played?
                region.locked.alphaHitTestMinimumThreshold = 0.5f; // Only the region's own pixels catch taps, not its see-through rest
                region.locked.canvasRenderer.cullTransparentMesh = false; // Keep a hidden (unlocked) region tappable
                bool unseen = region.unlock != null && progress.ShouldAnimateUnlock(region.episode, PlayerPrefs.GetInt(SeenPrefix + region.episode) != 0); // Saves the animation for an unseen unlock
                region.locked.color = open && !unseen ? Color.clear : Color.white; // Holds the grey until an unseen unlock plays
                region.padlock.SetActive(!open || unseen); // Shows the lock when the region still looks locked
                if (region.life != null) region.life.SetActive(open); // Locked regions do not move
                region.stars.text = progress.StarsFor(region.episode) + "/" + PlayerProgress.MaxStarsFor(region.episode); // For example "0/45"
                var tapped = region; // Copy for the button listener below
                region.button.onClick.AddListener(() => Choose(tapped, open)); // Tapping the region runs Choose()
            }
            StartCoroutine(PlayUnseenUnlocks(progress)); // Plays any just opened regions before accepting taps
        }

        IEnumerator PlayUnseenUnlocks(PlayerProgress progress) // Plays unseen opens in map order
        { // Starts the unlock check
            foreach (var region in regions) // Looks at each landform
            { // Checks one region
                string key = SeenPrefix + region.episode; // Its own device flag
                if (region.unlock == null || !progress.ShouldAnimateUnlock(region.episode, PlayerPrefs.GetInt(key) != 0)) continue; // Already open before, or still locked
                unlocking = true; // Holds input while the lock opens
                ui.interactable = false; // Blocks taps on all map buttons
                while (ScreenFader.IsBusy) yield return null; // Shows the full animation after the map fades in
                ShowGuide(region.episode + " is now open!", 2f); // Tells the player what opened
                if (unlockSound != null) GetComponent<AudioSource>().PlayOneShot(unlockSound, GameSettings.SfxVolume); // Plays one unlock sound
                yield return region.unlock.Play(region.padlock.GetComponent<Image>(), region.locked); // Opens the lock and fades the grey
                region.padlock.SetActive(false); // The open map has no lock
                PlayerPrefs.SetInt(key, 1); // Records that this landform's animation played
                PlayerPrefs.Save(); // Keeps the flag when the game closes
            } // Ends the region checks
            if (unlocking) ui.interactable = true; // Gives the map its input back
            unlocking = false; // Back and taps work again
        } // Ends the unlock check

        void Choose(Region region, bool open) // What tapping a region does
        {
            if (Leaving || unlocking) return; // Ignore taps during the unlock and the exit
            if (!open) { ShowGuide(RegionGuide.ForLocked(region.episode)); return; } // Locked: explain and stay on the map
            if (!LevelCatalog.HasLevels(region.episode)) { ShowGuide("Coming soon"); return; } // Open regions with no levels stay on the map
            GameSession.Episode = region.episode; // Remember the landform for the next screens
            OpenOrComingSoon(difficultyScene); // Open difficulty selection
        }

        protected override void Back() { if (!unlocking) Leave(mainMenuScene); } // Back waits until the lock opens
    }
}
