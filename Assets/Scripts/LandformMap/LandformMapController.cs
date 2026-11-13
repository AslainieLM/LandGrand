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
            public GameObject life; // Animations shown only while unlocked (may be empty)
            public TMP_Text stars; // Star progress, for example "0/45"
        }

        public Region[] regions; // The four regions

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
                region.locked.color = open ? Color.clear : Color.white; // Unlocked: hide the grey copy but keep it as the tap area
                region.padlock.SetActive(!open); // Padlock only while locked
                if (region.life != null) region.life.SetActive(open); // Locked regions do not move
                region.stars.text = progress.EpisodeStars(region.episode) + "/" + PlayerProgress.MaxEpisodeStars; // For example "0/45"
                var tapped = region; // Copy for the button listener below
                region.button.onClick.AddListener(() => Choose(tapped, open)); // Tapping the region runs Choose()
            }
        }

        void Choose(Region region, bool open) // What tapping a region does
        {
            if (Leaving) return; // Ignore taps while leaving
            if (!open) { ShowGuide(RegionGuide.ForLocked(region.episode)); return; } // Locked: explain and stay on the map
            GameSession.Episode = region.episode; // Remember the landform for the next screens
            OpenOrComingSoon(difficultyScene); // Open difficulty selection
        }

        protected override void Back() => Leave(mainMenuScene); // Back always returns to the Main Menu
    }
}
