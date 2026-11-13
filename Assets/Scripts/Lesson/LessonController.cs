using System; // Gives access to Serializable
using System.Linq; // Gives access to ToList and FirstOrDefault
using LandGrand.Rules; // Gives access to LevelCatalog and LevelData
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject and Sprite
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.LessonScreen // Lesson Screen scripts live here
{
    /// <summary>
    /// Runs the Lesson Screen (decision D-041, ticket 06): the landform's picture, a short lesson,
    /// the level's rocks and minerals with their names, and the level's goals.
    /// Draft content shows the "needs teacher review" note (D-043). Start opens the level; Back returns to the level trail.
    /// </summary>
    public class LessonController : PixelScreen // The Lesson Screen brain
    {
        /// <summary>The parts of one item box.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class ItemSlot // One rock or mineral box
        {
            public GameObject root; // The whole box, hidden when the level has fewer kinds
            public Image picture; // The item's picture
            public TMP_Text label; // The item's name under the box
        }

        /// <summary>A picture and the name the level data uses for it.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class NamedSprite // One named picture
        {
            public string name; // Name used in the level data, for example "granite"
            public Sprite sprite; // The picture
        }

        public Image title; // "MOUNTAINS: EASY" at the top
        public Sprite[] titles; // One title per landform and difficulty: index = landform * 3 + difficulty
        public TMP_Text heading; // "Level 1: Mountains"
        public TMP_Text body; // The lesson sentences
        public Image picture; // The landform picture
        public TMP_Text draftNote; // "Draft — needs teacher review"
        public GameObject itemsHeading; // "You will find:" above the item boxes
        public ItemSlot[] items; // Boxes for the level's item kinds
        public GameObject goalsHeading; // "Goals:" above the goals
        public GameObject[] goalRows; // One row per goal: icon and text
        public TMP_Text[] goals; // The goal texts, one per row
        public Button startButton; // Starts the level
        public NamedSprite[] pictures; // Every lesson and item picture, by name

        [Header("Scenes")] // Inspector heading for the scene names
        public string levelSelectionScene = "LevelSelection"; // Where Back goes

        LevelData level; // The level this lesson belongs to; null when it has no data yet

        protected override void Awake() // Runs once when the screen opens: fills the lesson from the level data
        {
            base.Awake(); // Click sound, hidden guide and Back
            var episode = GameSession.Episode; // The landform picked on the map
            var difficulty = GameSession.Difficulty; // The difficulty picked on the cards
            ShowTitle(title, titles, (int)episode * 3 + (int)difficulty); // "MOUNTAINS: EASY" at the top
            level = LevelCatalog.Find(episode, difficulty, GameSession.Level); // The level picked on the trail
            startButton.onClick.AddListener(StartLevel); // Tapping Start runs StartLevel()
            if (level == null) { ShowNoLesson(); return; } // No data yet: say so and offer only Back

            var lesson = level.Lesson; // The lesson to show
            heading.text = "Level " + level.Number + ": " + lesson.Title; // For example "Level 1: Mountains"
            body.text = string.Join("\n", lesson.Sentences); // One sentence per line, easy to read
            ShowPicture(picture, lesson.Picture); // The landform picture
            draftNote.text = LevelData.DraftNote; // "Draft — needs teacher review"
            draftNote.gameObject.SetActive(level.HasDraftContent); // Only while a teacher has not reviewed it

            var kinds = level.Kinds.ToList(); // Each item kind once, in order
            for (int i = 0; i < items.Length; i++) // Fill each item box
            {
                bool used = i < kinds.Count; // Does the level have a kind for this box?
                items[i].root.SetActive(used); // Hide boxes the level does not need
                if (!used) continue; // Nothing more for a hidden box
                ShowPicture(items[i].picture, kinds[i].Picture); // The item's picture
                items[i].label.text = kinds[i].Name; // The item's name
            }

            var goalTexts = level.Goals.ToList(); // "Collect 5/5", "Finish before time runs out"
            for (int i = 0; i < Mathf.Min(goals.Length, goalRows.Length); i++) // Fill each goal row (both lists must have an entry)
            {
                bool used = i < goalTexts.Count; // Does the level have a goal for this row?
                goalRows[i].SetActive(used); // Hide rows it does not need
                if (used) goals[i].text = goalTexts[i]; // The goal's words
            }
        }

        void ShowNoLesson() // Shows a level that has no lesson data yet
        {
            heading.text = "Coming soon"; // Tell the player
            body.text = "This level is not ready yet."; // Short reason
            picture.gameObject.SetActive(false); // No picture
            draftNote.gameObject.SetActive(false); // No draft note
            itemsHeading.SetActive(false); // No item heading
            foreach (var slot in items) slot.root.SetActive(false); // No item boxes
            goalsHeading.SetActive(false); // No goals heading
            foreach (var row in goalRows) row.SetActive(false); // No goals
            startButton.gameObject.SetActive(false); // Nothing to start
        }

        void ShowPicture(Image image, string name) // Puts the named picture in an image at its pixel size
        {
            var found = pictures.FirstOrDefault(p => p.name == name); // Look the name up
            if (found == null || found.sprite == null) { Debug.LogWarning("Lesson Screen has no picture named " + name); image.enabled = false; return; } // Missing: hide it and say so
            image.sprite = found.sprite; // The picture
            image.rectTransform.sizeDelta = found.sprite.rect.size; // One sprite pixel = one UI pixel
        }

        void StartLevel() // What tapping Start does
        {
            if (Leaving || level == null) return; // Ignore taps while leaving or with no level
            OpenOrComingSoon(level.SceneName); // Open the level, or say it is coming soon (ticket 07 builds it)
        }

        protected override void Back() => Leave(levelSelectionScene); // One step back: the level trail
    }
}
