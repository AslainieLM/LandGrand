using System; // Gives access to Serializable
using LandGrand.Rules; // Gives access to PlayerProgress
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Sprite, Vector2 and Mathf
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Selection // Difficulty and level selection scripts live here
{
    /// <summary>
    /// Runs the level trail (decision D-041): one stone per level of the chosen difficulty, each with its best stars.
    /// Only the first uncleared level and the ones before it can be tapped; later stones show a padlock.
    /// A stone opens the Lesson Screen. Back returns to the difficulty cards.
    /// </summary>
    public class LevelSelectionController : PixelScreen // The level trail brain
    {
        /// <summary>The parts of one level stone.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class Stone // One level stone
        {
            public Button button; // Reacts when the stone is tapped
            public Image image; // Stone picture
            public TMP_Text number; // Level number on the stone
            public Image[] stars; // Three stars under the stone
            public GameObject padlock; // Padlock shown while locked
        }

        public Stone[] stones; // Enough stones for the longest difficulty (7)
        public Sprite stoneOpen; // Light stone that can be tapped
        public Sprite stoneLocked; // Dark stone that is still locked
        public Sprite starFull; // Earned star
        public Sprite starEmpty; // Star not earned yet
        public Image dotTemplate; // One dash of the trail, copied between stones
        public Image title; // "MOUNTAINS: EASY" at the top
        public Sprite[] titles; // One title per landform and difficulty: index = landform * 3 + difficulty

        [Header("Layout")] // Inspector heading for the stone layout (map pixels, y down from the top)
        public float trailY = 150f; // Middle height of the trail
        public float zigzag = 18f; // How far stones go up and down from the middle
        public float maxSpacing = 120f; // Widest gap between two stones
        public float trailWidth = 400f; // Widest the whole trail may be
        public float dotGap = 6f; // Pixels between two dashes of the trail

        [Header("Scenes")] // Inspector heading for the scene names
        public string difficultyScene = "DifficultySelection"; // Where Back goes
        public string lessonScene = "Lesson"; // Where a stone goes (ticket 06)

        protected override void Awake() // Runs once when the screen opens: lays out the stones from the saved progress
        {
            base.Awake(); // Click sound and hidden guide
            var progress = GameProgress.Current; // The saved player progress, loaded at launch
            var episode = GameSession.Episode; // The landform picked on the map
            var difficulty = GameSession.Difficulty; // The difficulty picked on the cards
            ShowTitle(title, titles, (int)episode * 3 + (int)difficulty); // "MOUNTAINS: EASY" at the top

            int count = Mathf.Min(PlayerProgress.LevelCount(difficulty), stones.Length); // Stones this difficulty needs
            float spacing = count > 1 ? Mathf.Min(maxSpacing, trailWidth / (count - 1)) : 0f; // Gap that fits the trail on the screen
            var places = new Vector2[count]; // Middle of each stone, in map pixels
            for (int i = 0; i < count; i++) // Work out each place
                places[i] = new Vector2(Mathf.Round(240f + (i - (count - 1) / 2f) * spacing), trailY + (i % 2 == 0 ? -zigzag : zigzag)); // Left to right, up and down

            DrawTrail(places); // Dashes between the stones, drawn first so stones sit on top
            for (int i = 0; i < stones.Length; i++) // Set up each stone
            {
                var stone = stones[i]; // This stone
                bool used = i < count; // Does this difficulty have this level?
                stone.button.gameObject.SetActive(used); // Hide the stones it does not need
                if (!used) continue; // Nothing more to do for a hidden stone
                stone.button.transform.SetAsLastSibling(); // Draw it on top of the trail dashes
                ((RectTransform)stone.button.transform).anchoredPosition = ToUi(places[i]); // Put it in its place
                int level = i + 1; // Level number, starting at 1
                bool open = progress.IsLevelAvailable(episode, difficulty, level); // Can this level be played now?
                int best = progress.BestStars(episode, difficulty, level); // Best stars so far (0 when not cleared)
                stone.image.sprite = open ? stoneOpen : stoneLocked; // Light or dark stone
                stone.number.text = open ? level.ToString() : ""; // The number shows on open stones; locked ones show the padlock
                stone.padlock.SetActive(!open); // Padlock only while locked
                for (int s = 0; s < stone.stars.Length; s++) stone.stars[s].sprite = s < best ? starFull : starEmpty; // Fill the earned stars
                stone.button.interactable = open; // Only open levels can be tapped
                stone.button.onClick.AddListener(() => Choose(level)); // Tapping the stone runs Choose()
            }
            backButton.transform.SetAsLastSibling(); // Back stays on top of everything
            guidePanel.transform.SetAsLastSibling(); // The guide shows above the stones
        }

        void DrawTrail(Vector2[] places) // Places dashes along the lines between stones
        {
            dotTemplate.gameObject.SetActive(false); // The template itself stays hidden
            for (int i = 0; i + 1 < places.Length; i++) // Each gap between two stones
            {
                Vector2 from = places[i], to = places[i + 1]; // Start and end of this piece
                int dots = Mathf.FloorToInt(Vector2.Distance(from, to) / dotGap); // How many dashes fit
                for (int d = 2; d < dots - 1; d++) // Skip the dashes under the stones at both ends
                {
                    var dot = Instantiate(dotTemplate, dotTemplate.transform.parent); // Copy the dash
                    dot.gameObject.SetActive(true); // Show the copy
                    var p = Vector2.Lerp(from, to, (float)d / dots); // Place along the line
                    ((RectTransform)dot.transform).anchoredPosition = ToUi(new Vector2(Mathf.Round(p.x), Mathf.Round(p.y))); // On a whole pixel
                }
            }
        }

        static Vector2 ToUi(Vector2 pixel) => new Vector2(pixel.x - 240f, 135f - pixel.y); // Map pixel (y down) to UI offset from the middle (y up)

        void Choose(int level) // What tapping an open stone does
        {
            if (Leaving) return; // Ignore taps while leaving
            GameSession.Level = level; // Remember the level for the Lesson Screen
            OpenOrComingSoon(lessonScene); // Open the lesson, or say it is coming soon
        }

        protected override void Back() => Leave(difficultyScene); // One step back: the difficulty cards
    }
}
