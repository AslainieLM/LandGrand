using System.Collections.Generic; // Gives access to HashSet
using LandGrand.Rules; // Gives access to AchievementCatalog, NextStep and LevelData
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, Sprite and Color
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The "Level cleared!" panel: stars that pop in, the items taken and missed, the two goals, the review,
    /// any new badges (D-060), and the buttons Back to Map, Replay and Next Level or Next Episode (D-059).
    /// </summary>
    public class ResultScreen : MonoBehaviour // One part of the level screen
    {
        public GameObject panel; // "Level cleared!" panel
        public Image[] stars; // Three stars on the panel
        public Sprite starFull; // Earned star
        public Sprite starEmpty; // Star not earned
        public float starPopSeconds = 0.4f; // Time between the stars popping in
        public Image[] items; // One picture per pickup of the level: taken items in colour, missed ones as grey outlines
        public Color missedColor = new Color(0.2f, 0.2f, 0.25f, 0.55f); // Tint that turns a missed item into a grey outline
        public Image[] goalIcons; // A tick or a cross for each goal
        public TMP_Text[] goalTexts; // The goal lines
        public Sprite tickSprite; // Goal met
        public Sprite crossSprite; // Goal not met
        public TMP_Text reviewLabel; // Two or three review sentences
        public TMP_Text reviewDraftNote; // Draft note under the review

        [Header("Buttons")] // Inspector heading for the three buttons
        public Button mapButton; // Goes back to the Landform Map
        public Button replayButton; // Starts the level again from its lesson
        public Button nextButton; // Goes to the next available level
        public Sprite nextLevelSprite; // "Next Level" picture for the Next button
        public Sprite nextLevelPressed; // Its pushed-in picture
        public Sprite nextEpisodeSprite; // "Next Episode" picture, after the whole landform
        public Sprite nextEpisodePressed; // Its pushed-in picture

        [Header("New badge")] // Inspector heading for the badge notice
        public GameObject badgeNotice; // "New badge!" ribbon with the badge and its name, shown after the stars
        public Image badgeNoticePicture; // The new badge
        public TMP_Text badgeNoticeName; // Its name
        public Sprite[] badgePictures; // Earned badge pictures, named "badge_" plus the achievement id
        public float badgeShowSeconds = 1.6f; // How long each new badge stays when several were earned

        LevelController level; // The level this panel belongs to
        HashSet<string> badgesBefore; // Badges the explorer had when the level began

        internal bool IsBuilt => panel != null && items != null && mapButton != null && nextButton != null; // False for a scene built before this part existed

        internal void Begin(LevelController owner) // Hides the panel, connects the buttons and remembers the badges
        {
            level = owner; // Remember the level
            badgesBefore = AchievementCatalog.EarnedIds(GameProgress.Current); // So the panel can show the badges this level earns
            if (badgeNotice != null) badgeNotice.SetActive(false); // No badge notice until the result
            panel.SetActive(false); // No result yet
            nextButton.onClick.AddListener(Next); // Next goes to the next available level
            replayButton.onClick.AddListener(() => level.GoTo(level.lessonScene)); // Replay starts again from the lesson
            mapButton.onClick.AddListener(() => level.GoTo(level.mapScene)); // Back to the Landform Map
        }

        internal void Show() // The level is cleared: fill the panel, then show it
        {
            Fill(); // Items, goals and review for this attempt
            ShowNextButton(); // "Next Level" or "Next Episode", after the clear was saved
            panel.SetActive(true); // Show the result
            StartCoroutine(PopStars()); // Stars pop in one by one with a chime, then any new badges
        }

        void Fill() // Shows what happened in the run
        {
            var run = level.Run; // The attempt
            foreach (var star in stars) star.sprite = starEmpty; // Stars start empty and pop in below
            var taken = run.TakenFlags; // Which pickups were taken
            for (int i = 0; i < items.Length; i++) // One picture per pickup
            {
                items[i].sprite = level.collector.PictureOf(run.Level.Pickups[i].Kind); // Its picture
                items[i].color = taken[i] ? Color.white : missedColor; // Taken in colour, missed as a grey outline
            }
            int g = 0; // Which goal line this is
            foreach (var goal in run.GoalResults) // Each goal with its tick
            {
                goalTexts[g].text = goal.Text; // The goal words
                goalIcons[g].sprite = goal.Met ? tickSprite : crossSprite; // Tick or cross
                g++; // Next line
            }
            reviewLabel.text = string.Join(" ", run.Level.Review); // The review sentences
            reviewDraftNote.text = run.Level.HasDraftContent ? LevelData.DraftNote : ""; // Draft marking until a teacher checks (D-043)
        }

        System.Collections.IEnumerator PopStars() // Fills the earned stars one by one, each with a chime
        {
            var chimes = level.chimes; // The shared chimes
            var sounds = level.Sounds; // The level's sound player
            for (int i = 0; i < level.Run.Stars; i++) // Each earned star
            {
                yield return new WaitForSecondsRealtime(starPopSeconds); // A short pause between stars
                stars[i].sprite = starFull; // The star pops in
                if (chimes != null && chimes.Length > 0) // A chime for each star
                {
                    sounds.pitch = 1f + 0.15f * i; // Each chime a little higher
                    sounds.PlayOneShot(chimes[0], GameSettings.SfxVolume); // Play it at the player sound volume
                }
            }
            sounds.pitch = 1f; // Back to normal pitch
            yield return ShowNewBadges(); // Then the badges this level earned, if any
        }

        System.Collections.IEnumerator ShowNewBadges() // Shows "New badge!" with each badge earned in this level, one after another
        {
            if (badgeNotice == null) yield break; // A scene built before D-060 has no notice
            var fresh = AchievementCatalog.NewlyEarned(badgesBefore, GameProgress.Current); // Badges earned since the level began
            if (fresh.Count == 0) yield break; // Nothing new
            var chimes = level.chimes; // The shared chimes
            badgeNotice.SetActive(true); // Show the ribbon
            for (int i = 0; ; i = (i + 1) % fresh.Count) // Each new badge in turn, round and round while the result shows
            {
                var badge = fresh[i]; // This badge
                badgeNoticePicture.sprite = System.Array.Find(badgePictures, s => s != null && s.name == "badge_" + badge.Id); // Its medal
                badgeNoticeName.text = badge.Name; // Its name
                if (chimes != null && chimes.Length > 0) level.PlaySfx(chimes[chimes.Length - 1]); // A chime for each badge
                if (fresh.Count == 1) yield break; // One badge stays put
                yield return new WaitForSecondsRealtime(badgeShowSeconds); // Then the next one
            }
        }

        void Next() // The Next button: the next level, the next landform, or back to the trail when nothing open follows (D-059, LG-32)
        {
            var next = GameProgress.Current.NextAfter(level.episode, level.difficulty, level.number); // The rules decide
            if (next.Step == NextStep.NextEpisode) // Reviews the landform that the explorer just finished
            { // Starts the summary route
                GameSession.Episode = level.episode; // The summary uses the finished landform
                GameSession.DescriptionFromBook = false; // Back goes to the map after a clear
                if (Application.CanStreamedLevelBeLoaded("LandformDescription")) { level.GoTo("LandformDescription"); return; } // Opens the review screen when it is built
                GameSession.Episode = next.Episode; // Without the review screen, the map opens on the next landform
                level.GoTo(level.mapScene); // The old route, so Next never breaks
                return; // Done
            } // Ends the summary route
            if (next.Step == NextStep.None) { level.GoTo(level.levelSelectionScene); return; } // Nothing open: back to the trail
            GameSession.Episode = next.Episode; // The Lesson Screen reads the landform from here
            GameSession.Difficulty = next.Difficulty; // And the difficulty, which changes after the last level of a difficulty
            GameSession.Level = next.Level; // And the level number
            level.GoTo(level.lessonScene); // The next level starts from its lesson
        }

        void ShowNextButton() // Shows "Next Level" or "Next Episode" on the Next button
        {
            if (nextLevelSprite == null || nextEpisodeSprite == null) return; // A scene built before D-059 keeps its old picture
            bool episode = GameProgress.Current.NextAfter(level.episode, level.difficulty, level.number).Step == NextStep.NextEpisode; // Is a new landform next?
            var image = (Image)nextButton.targetGraphic; // The button picture
            image.sprite = episode ? nextEpisodeSprite : nextLevelSprite; // The right words
            var state = nextButton.spriteState; // Its pressed picture
            state.pressedSprite = episode ? nextEpisodePressed : nextLevelPressed; // Matches the words
            nextButton.spriteState = state; // Use it
            image.SetNativeSize(); // Its own width; the right edge stays put
        }
    }
}
