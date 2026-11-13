using LandGrand.Rules; // Gives access to LevelCatalog and LevelRun
using LandGrand.StartScreen; // Gives access to SpriteFlipbook for the sparkle
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Sprite and Vector2
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Runs one level (ticket 07): puts the explorer at the start, watches for the goal,
    /// then saves the clear and shows a basic result with the stars and Continue.
    /// Ticket 08 adds collecting: touching a pickup counts it, the first find of a kind opens a pop-up that pauses the game,
    /// and later finds only chime and sparkle. Hazards, the timer and the pause menu come in tickets 09 and 10; the full Result Screen in ticket 11.
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

        [Header("Collecting (ticket 08)")] // Inspector heading for the pickups, counter and pop-up
        public PickupSpot[] pickups; // The collectibles in the level, in the order of LevelData
        public TMP_Text counterLabel; // The "2/5" count, top left
        public GameObject findPanel; // The "You found an item!" pop-up with its dark backdrop
        public Image findPicture; // The item picture in the pop-up
        public TMP_Text findName; // The item name in the pop-up
        public TMP_Text findDescription; // The short description in the pop-up
        public Button findOk; // Closes the pop-up
        public Sprite[] itemPictures; // Item pictures; the one named like the kind Picture is used
        public Sprite[] sparkleFrames; // Sparkle shown where an item was taken
        public AudioClip[] chimes; // Chimes played when an item is taken

        [Header("Basic result")] // Inspector heading for the result panel
        public GameObject resultPanel; // "Level cleared!" panel
        public Image[] resultStars; // Three stars on the panel
        public Sprite starFull; // Earned star
        public Sprite starEmpty; // Star not earned
        public Button continueButton; // Goes back to the level trail

        [Header("Scenes")] // Inspector heading for the scene names
        public string levelSelectionScene = "LevelSelection"; // Where Continue and Back go

        LevelRun run; // This attempt
        AudioSource chimeSource; // Plays the pickup chimes

        protected override void Awake() // Runs once when the level opens: starts the attempt
        {
            base.Awake(); // Click sound and hidden guide
            if (pickups == null || findPanel == null || findOk == null || counterLabel == null) // The scene was built before ticket 08 added collecting
            {
                Debug.LogError("This level scene is out of date. Run LandGrand > Build Mountains Easy 1 Level, then press Play again."); // Say what to do
                enabled = false; // Stop Update so the console shows one clear message, not an error every frame
                return; // Do not start the attempt
            }
            Application.targetFrameRate = 60; // Smooth movement; Android would otherwise stay at 30 frames per second
            var level = LevelCatalog.Find(episode, difficulty, number); // This scene's level data
            run = new LevelRun(level); // A fresh attempt
            explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // The attempt decides where the explorer comes back
            explorer.PlaceAt(explorer.RespawnPoint); // Put the explorer at the start
            resultPanel.SetActive(false); // No result yet
            chimeSource = gameObject.AddComponent<AudioSource>(); // Sound player for the chimes
            chimeSource.playOnAwake = false; // Silent until an item is taken
            findPanel.SetActive(false); // No pop-up yet
            findOk.onClick.AddListener(CloseFind); // OK closes the pop-up
            ShowCount(); // Show 0/5
            continueButton.onClick.AddListener(() => Leave(levelSelectionScene)); // Continue goes back to the level trail
        }

        protected override void Update() // Runs every frame: checks the goal and the Back button
        {
            base.Update(); // Android Back (or Escape)
            if (Leaving || run.Outcome != RunOutcome.Playing || run.IsPaused) return; // Only while playing, not leaving and no pop-up open
            CheckPickups(); // Collect anything the explorer touches
            if (run.IsPaused) return; // A first find opened the pop-up: the goal waits
            if (explorer.Position.x >= run.Level.Goal.X - goalReach) Clear(); // Reached the cave entrance
        }

        void CheckPickups() // Looks for a pickup under the explorer
        {
            var p = explorer.Position; // Feet of the explorer
            var body = new Rect(p.x - explorer.size.x / 2f, p.y, explorer.size.x, explorer.size.y); // The explorer body box
            foreach (var spot in pickups) // Each pickup in the level
            {
                if (run.IsCollected(spot.index) || !body.Overlaps(spot.Area)) continue; // Taken already, or not touched
                Take(spot); // Collect it
                if (run.IsPaused) return; // A pop-up opened: stop for now
            }
        }

        void Take(PickupSpot spot) // Collects one pickup
        {
            var result = run.Collect(spot.index, GameProgress.Current); // The rules decide: first find, known kind or nothing
            if (result == CollectResult.None) return; // Nothing happened
            spot.Hide(); // The item leaves the level
            Sparkle(spot.Center); // Sparkle where it was
            if (chimes != null && chimes.Length > 0) chimeSource.PlayOneShot(chimes[Random.Range(0, chimes.Length)], GameSettings.SfxVolume); // Chime at the player sound volume
            ShowCount(); // Update the counter
            if (result != CollectResult.FirstFind) return; // A known kind: no pop-up
            GameProgress.Save(); // Keep the found kind on the device now
            OpenFind(run.PendingFind); // Show the pop-up
        }

        void ShowCount() => counterLabel.text = run.Collected + "/" + run.Level.Pickups.Count; // For example 2/5

        void OpenFind(ItemKind kind) // Opens the "You found an item!" pop-up and pauses the game
        {
            findName.text = kind.Name; // Item name
            findDescription.text = kind.Description; // Short description
            findPicture.sprite = System.Array.Find(itemPictures, s => s != null && s.name == kind.Picture); // Its picture
            findPanel.SetActive(true); // Show the pop-up; its backdrop blocks taps on the controls
            explorer.Frozen = true; // The explorer stops until OK
        }

        void CloseFind() // OK was pressed
        {
            run.DismissFind(); // The game goes on
            findPanel.SetActive(false); // Hide the pop-up
            explorer.Frozen = false; // The explorer can move again
        }

        void Sparkle(Vector2 where) // A small sparkle at a level place
        {
            if (sparkleFrames == null || sparkleFrames.Length == 0) return; // No sparkle pictures
            var go = new GameObject("Sparkle"); // New object for the sparkle
            go.transform.position = LevelGeometry.ToWorld(Mathf.Round(where.x), Mathf.Round(where.y)); // On whole pixels
            var sr = go.AddComponent<SpriteRenderer>(); // Draws it
            sr.sortingOrder = 25; // In front of the explorer
            var flip = go.AddComponent<SpriteFlipbook>(); // Plays the frames
            flip.frames = sparkleFrames; // The sparkle pictures
            flip.framesPerSecond = 10f; // A short twinkle
            flip.loop = false; // Play once
            flip.destroyWhenDone = true; // Remove it afterwards
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

        protected override void Back() // Android Back or Escape
        {
            if (run.IsPaused) return; // The pop-up blocks all other input until OK
            Leave(levelSelectionScene); // Until ticket 10 adds the pause menu, Back leaves to the level trail
        }
    }
}
