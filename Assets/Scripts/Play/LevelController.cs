using LandGrand.Rules; // Gives access to LevelCatalog and LevelRun
using UnityEngine; // Gives access to GameObject, Sprite and Vector2
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Runs one level (ticket 07): puts the explorer at the start, watches for the goal,
    /// then saves the clear and shows a basic result with the stars and Continue.
    /// Collecting, hazards, the timer and the pause menu come in tickets 08 to 10; the full Result Screen in ticket 11.
    /// </summary>
    public class LevelController : PixelScreen // The level brain
    {
        [Header("Level")] // Inspector heading for which level this scene plays
        public Episode episode = Episode.Mountains; // Landform of this scene
        public Difficulty difficulty = Difficulty.Easy; // Difficulty of this scene
        public int number = 1; // Level number of this scene
        public float goalReach = 8f; // How close (in pixels) the explorer must get to the goal

        [Header("Parts")] // Inspector heading for the connected parts
        public ExplorerMotor explorer; // The explorer
        public GameObject touchControls; // The on-screen arrows and jump button

        [Header("Basic result")] // Inspector heading for the result panel
        public GameObject resultPanel; // "Level cleared!" panel
        public Image[] resultStars; // Three stars on the panel
        public Sprite starFull; // Earned star
        public Sprite starEmpty; // Star not earned
        public Button continueButton; // Goes back to the level trail

        [Header("Scenes")] // Inspector heading for the scene names
        public string levelSelectionScene = "LevelSelection"; // Where Continue and Back go

        LevelRun run; // This attempt

        protected override void Awake() // Runs once when the level opens: starts the attempt
        {
            base.Awake(); // Click sound and hidden guide
            Application.targetFrameRate = 60; // Smooth movement; Android would otherwise stay at 30 frames per second
            var level = LevelCatalog.Find(episode, difficulty, number); // This scene's level data
            run = new LevelRun(level); // A fresh attempt
            explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // The attempt decides where the explorer comes back
            explorer.PlaceAt(explorer.RespawnPoint); // Put the explorer at the start
            resultPanel.SetActive(false); // No result yet
            continueButton.onClick.AddListener(() => Leave(levelSelectionScene)); // Continue goes back to the level trail
        }

        protected override void Update() // Runs every frame: checks the goal and the Back button
        {
            base.Update(); // Android Back (or Escape)
            if (Leaving || run.Outcome != RunOutcome.Playing) return; // Only while playing and not leaving
            if (explorer.Position.x >= run.Level.Goal.X - goalReach) Clear(); // Reached the cave entrance
        }

        void Clear() // The level is cleared
        {
            if (!run.ReachGoal()) return; // Count the goal once
            run.RecordResult(GameProgress.Current); // Keep the clear and its stars
            GameProgress.Save(); // Write it to the device now
            explorer.Frozen = true; // The explorer stops at the goal
            touchControls.SetActive(false); // Hide the controls
            for (int i = 0; i < resultStars.Length; i++) resultStars[i].sprite = i < run.Stars ? starFull : starEmpty; // Fill the earned stars
            resultPanel.SetActive(true); // Show the result
        }

        protected override void Back() => Leave(levelSelectionScene); // Until ticket 10 adds the pause menu, Back leaves to the level trail
    }
}
