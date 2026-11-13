using System; // Gives access to Serializable
using LandGrand.Rules; // Gives access to Difficulty, PlayerProgress and RegionGuide
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Sprite and Tooltip
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Selection // Difficulty and level selection scripts live here
{
    /// <summary>
    /// Runs the difficulty screen (decision D-041): three cards, Easy, Normal and Hard, each with its stars.
    /// Locked cards are grey with a padlock and "Clear all Easy levels" or "Clear all Normal levels".
    /// An open card opens the level trail. Back returns to the Landform Map.
    /// </summary>
    public class DifficultySelectionController : PixelScreen // The difficulty screen brain
    {
        /// <summary>The parts of one difficulty card.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class Card // One difficulty card
        {
            public Difficulty difficulty; // Which difficulty this card is
            public Button button; // Reacts when the card is tapped
            public Image panel; // Card background
            public Image heading; // "EASY", "NORMAL" or "HARD"
            public Sprite headingOpen; // Dark heading for an open card
            public Sprite headingLocked; // Grey heading for a locked card
            public GameObject padlock; // Padlock shown while locked
            public TMP_Text stars; // Star progress, for example "0/9"
            public TMP_Text guide; // "Clear all Easy levels" while locked
        }

        public Card[] cards; // The three cards
        public Sprite cardOpen; // Cream card background
        public Sprite cardLocked; // Grey card background
        public Image title; // Landform name at the top
        public Sprite[] titles; // One title per landform, in Episode order

        [Header("Scenes")] // Inspector heading for the scene names
        public string mapScene = "LandformMap"; // Where Back goes
        public string levelScene = "LevelSelection"; // Where an open card goes

        protected override void Awake() // Runs once when the screen opens: shows the saved progress on each card
        {
            base.Awake(); // Click sound and hidden guide
            var progress = GameProgress.Current; // The saved player progress, loaded at launch
            var episode = GameSession.Episode; // The landform picked on the map
            ShowTitle(title, titles, (int)episode); // The landform's name at the top
            foreach (var card in cards) // Set up each card
            {
                bool open = progress.IsDifficultyUnlocked(episode, card.difficulty); // Can this difficulty be played?
                card.panel.sprite = open ? cardOpen : cardLocked; // Cream when open, grey when locked
                card.heading.sprite = open ? card.headingOpen : card.headingLocked; // Dark or grey heading
                card.padlock.SetActive(!open); // Padlock only while locked
                card.stars.text = progress.DifficultyStars(episode, card.difficulty) + "/" + PlayerProgress.MaxDifficultyStars(card.difficulty); // For example "0/9"
                card.guide.text = open ? "" : RegionGuide.ForLockedDifficulty(card.difficulty); // What unlocks it
                var tapped = card; // Copy for the button listener below
                card.button.onClick.AddListener(() => Choose(tapped, open)); // Tapping the card runs Choose()
            }
        }

        void Choose(Card card, bool open) // What tapping a card does
        {
            if (Leaving) return; // Ignore taps while leaving
            if (!open) { ShowGuide(RegionGuide.ForLockedDifficulty(card.difficulty)); return; } // Locked: explain and stay
            GameSession.Difficulty = card.difficulty; // Remember the difficulty for the trail
            Leave(levelScene); // Open the level trail
        }

        protected override void Back() => Leave(mapScene); // One step back: the Landform Map
    }
}
