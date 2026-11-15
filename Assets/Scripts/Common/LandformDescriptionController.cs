using System.Collections; // Plays the summary entrance after the screen fade
using System.Linq; // Counts found kinds from this landform
using LandGrand.Rules; // Gives the summary and saved progress
using TMPro; // Gives the text labels
using UnityEngine; // Gives sprites and audio
using UnityEngine.UI; // Gives the screen buttons and pictures

namespace LandGrand // Holds the shared screens
{ // Opens the game code
    public sealed class LandformDescriptionController : PixelScreen // Shows the review of the completed landform
    { // Opens the screen
        public TMP_Text title; // Landform name in the pixel font
        public TMP_Text[] lines; // One label per short sentence
        public TMP_Text draftNote; // Says when the summary is still draft
        public TMP_Text stars, kinds, badges; // The three trip totals
        public GameObject unlockBanner; // Hidden at the end of the episode order
        public Image nextPicture, padlock; // Shows the next landform and its opening lock
        public Sprite[] landformPictures; // Menu pictures in landform order
        public TMP_Text opened, comingSoon; // The opening notice and future levels text
        public UnlockAnimation unlock; // Shared padlock and sparkles
        public Button toMap; // Opens the map
        public AudioClip fanfare; // Plays once when the summary opens
        Episode episode; // The landform being reviewed
        bool fromBook; // Chooses where Back goes

        protected override void Awake() // Fills the review and connects the buttons
        { // Starts the screen
            base.Awake(); // Sets up the shared screen parts
            episode = GameSession.Episode; // Uses the landform that was just finished
            fromBook = GameSession.DescriptionFromBook; // Remembers how this review opened
            GameSession.DescriptionFromBook = false; // The next visit sets its own route
            backButton.gameObject.SetActive(fromBook); // Only a book visit can return to difficulty selection
            toMap.onClick.AddListener(ToMap); // Connects the map button
            ShowSummary(LandformSummary.For(episode), GameProgress.Current); // Fills the review and totals
            ShowUnlock(); // Shows the next landform if there is one
            StartCoroutine(PlayEntrance()); // Starts the sound and animation once the screen is visible
        } // Ends the setup

        void ShowSummary(LandformSummary summary, PlayerProgress progress) // Shows the draft lines and the three totals
        { // Starts the review
            title.text = summary.Title; // Names the landform
            title.ForceMeshUpdate(); // Draws the chosen title
            for (int i = 0; i < lines.Length; i++) lines[i].text = i < summary.Sentences.Count ? summary.Sentences[i] : ""; // Gives each sentence a separate line
            draftNote.gameObject.SetActive(summary.IsDraft); // Only unreviewed summaries show this note
            var shelf = Collection.Shelves(progress)[(int)episode]; // Uses only kinds from this landform
            stars.text = progress.StarsFor(episode) + "/" + PlayerProgress.MaxStarsFor(episode); // Best stars over all its levels
            kinds.text = shelf.Slots.Count(s => s.IsFound) + "/" + shelf.Slots.Count; // Found and total kinds
            badges.text = AchievementCatalog.EarnedCount(progress) + "/" + AchievementCatalog.All.Count; // Uses the same totals as Profile
        } // Ends the review

        void ShowUnlock() // Sets up the next landform banner
        { // Starts the banner
            Episode? nextEpisode = PlayerProgress.NextEpisodeFor(episode); // Uses the same landform order as the Result Screen
            unlockBanner.SetActive(nextEpisode.HasValue); // The last landform has no next banner
            if (!nextEpisode.HasValue) return; // No text or picture is needed
            Episode next = nextEpisode.Value; // Draws the next landform
            nextPicture.sprite = landformPictures[(int)next]; // Shows the next landform's menu picture
            opened.text = LandformSummary.For(next).Title + " is now open!"; // Names the newly open landform
            comingSoon.gameObject.SetActive(!LevelCatalog.HasLevels(next)); // Shows the second line until its levels exist
        } // Ends the banner

        IEnumerator PlayEntrance() // Presents the fanfare and animation after the fade
        { // Starts the entrance
            while (ScreenFader.IsBusy) yield return null; // Waits until the player can see the screen
            if (fanfare != null) GetComponent<AudioSource>().PlayOneShot(fanfare, GameSettings.SfxVolume); // Plays the short fanfare once
            if (unlockBanner.activeSelf) yield return unlock.Play(padlock); // Opens the lock without hiding the banner
        } // Ends the entrance

        void ToMap() // Opens the map with the next landform picked
        { // Starts the exit
            if (Leaving || ScreenFader.IsBusy) return; // Ignores extra presses during a fade
            GameSession.Episode = PlayerProgress.NextEpisodeFor(episode) ?? episode; // Keeps Plateaus when there is no next landform
            Leave("LandformMap"); // Returns to the map
        } // Ends the exit

        protected override void Back() // Follows the route that opened this screen
        { // Starts the back action
            if (fromBook) Leave("DifficultySelection"); // Returns to the same landform's difficulty cards
            else ToMap(); // Finishing visits go to the map
        } // Ends the back action
    } // Ends the screen
} // Ends the game code
