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
        public TMP_Text levelNameLabel; // "Mountains Easy 1", above the count (decision D-049)

        [Header("Trail progress (D-049)")] // Inspector heading for the bar under the countdown
        public RectTransform progressFill; // Green part of the bar; its width shows how far the explorer has walked
        public RectTransform progressMarker; // The explorer marker riding on the bar
        public Image progressCheckpoint; // The small checkpoint flag on the bar
        public Sprite progressCheckpointOn; // Its green picture once the checkpoint is reached
        public float progressLeft; // Left edge of the bar's groove, as a UI offset from the screen's middle
        public float progressWidth = 142f; // Width of the groove in pixels
        public GameObject findPanel; // The "You found an item!" pop-up with its dark backdrop
        public Image findPicture; // The item picture in the pop-up
        public TMP_Text findName; // The item name in the pop-up
        public TMP_Text findDescription; // The short description in the pop-up
        public Button findOk; // Closes the pop-up
        public Sprite[] itemPictures; // Item pictures; the one named like the kind Picture is used
        public Sprite[] sparkleFrames; // Sparkle shown where an item was taken
        public AudioClip[] chimes; // Chimes played when an item is taken
        public AudioClip hitSound; // Played when a trap or the pit sends the explorer back
        public AudioClip checkpointSound; // Played when the checkpoint flag is reached

        [Header("Hazards and checkpoint (ticket 09)")] // Inspector heading for the traps, pit and flag
        public Rect[] spikeAreas; // Danger areas of the spike traps, in level pixels
        public float pitDeathY = -40f; // Falling below this height means the explorer fell into the pit
        public float checkpointReach = 8f; // How close (in pixels) the explorer must get to the checkpoint flag
        public SpriteRenderer checkpointFlag; // The flag picture
        public Sprite checkpointOnSprite; // The raised green flag shown once it is reached
        public Image hazardFlash; // Red flash over the screen when the explorer is sent back
        public float respawnSeconds = 0.35f; // How long the explorer waits, frozen, before coming back
        public float safeBackPixels = 24f; // Easy: how far back from the last ground spot the explorer comes back after a fall into the pit

        [Header("Result Screen (ticket 11)")] // Inspector heading for the result panel
        public GameObject resultPanel; // "Level cleared!" panel
        public Image[] resultStars; // Three stars on the panel
        public Sprite starFull; // Earned star
        public Sprite starEmpty; // Star not earned
        public Button continueButton; // Goes to the next available level
        public Button replayButton; // Starts the level again from its lesson
        public Button mapButton; // Goes back to the Landform Map
        public Image[] resultItems; // One picture per pickup of the level: taken items in colour, missed ones as grey outlines
        public Image[] goalIcons; // A tick or a cross for each goal
        public TMP_Text[] goalTexts; // The goal lines
        public TMP_Text reviewLabel; // Two or three review sentences
        public TMP_Text reviewDraftNote; // Draft note under the review
        public Sprite tickSprite; // Goal met
        public Sprite crossSprite; // Goal not met
        public Color missedColor = new Color(0.2f, 0.2f, 0.25f, 0.55f); // Tint that turns a missed item into a grey outline
        public float starPopSeconds = 0.4f; // Time between the stars popping in

        [Header("Timer, pause and time up (ticket 10)")] // Inspector heading for the countdown and its menus
        public TMP_Text timerLabel; // The countdown, top centre
        public Button pauseButton; // Pause button, top right
        public GameObject pausePanel; // Pause menu with its dark backdrop
        public Button resumeButton; // Pause menu: back to the game
        public Button pauseRetryButton; // Pause menu: start again from the Lesson Screen
        public Button pauseMenuButton; // Pause menu: leave to the Main Menu
        public GameObject timeUpPanel; // Time up panel with its dark backdrop
        public Button timeUpRetryButton; // Time up: start again from the Lesson Screen
        public Button timeUpMenuButton; // Time up: leave to the Main Menu
        public Color timerNormal = new Color32(31, 26, 36, 255); // Countdown colour
        public Color timerLow = new Color32(168, 63, 36, 255); // Countdown colour in the last 10 seconds

        [Header("Scenes")] // Inspector heading for the scene names
        public string levelSelectionScene = "LevelSelection"; // Where Continue and Back go
        public string lessonScene = "Lesson"; // Where Retry and Replay go (D-040)
        public string mapScene = "LandformMap"; // Where Back to Map goes
        public string mainMenuScene = "MainMenu"; // Where Back to Main Menu goes

        LevelRun run; // This attempt
        AudioSource chimeSource; // Plays the pickup chimes
        bool respawning; // True while the explorer is being sent back, so a hazard counts once
        Vector2 lastGround; // Where the explorer last stood on the ground
        int lastGroundFacing = 1; // Which way it was walking then

        protected override void Awake() // Runs once when the level opens: starts the attempt
        {
            base.Awake(); // Click sound and hidden guide
            if (pickups == null || findPanel == null || findOk == null || counterLabel == null || hazardFlash == null || checkpointFlag == null || timerLabel == null || pausePanel == null || timeUpPanel == null || resultItems == null || mapButton == null) // The scene was built before tickets 08 to 11
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
            lastGround = explorer.RespawnPoint; // Until the explorer stands somewhere else, this is the safe spot
            resultPanel.SetActive(false); // No result yet
            chimeSource = gameObject.AddComponent<AudioSource>(); // Sound player for the chimes
            chimeSource.playOnAwake = false; // Silent until an item is taken
            findPanel.SetActive(false); // No pop-up yet
            findOk.onClick.AddListener(CloseFind); // OK closes the pop-up
            ShowCount(); // Show 0/5
            if (levelNameLabel != null) levelNameLabel.text = episode + " " + difficulty + " " + number; // For example "Mountains Easy 1"
            ShowProgress(); // The marker starts at the left end of the bar
            pausePanel.SetActive(false); // Menus start hidden
            timeUpPanel.SetActive(false); // Menus start hidden
            pauseButton.onClick.AddListener(OpenPause); // The pause button opens the menu
            resumeButton.onClick.AddListener(ClosePause); // Resume closes it
            pauseRetryButton.onClick.AddListener(() => Leave(lessonScene)); // Retry starts again from the Lesson Screen
            pauseMenuButton.onClick.AddListener(() => Leave(mainMenuScene)); // Back to Main Menu
            timeUpRetryButton.onClick.AddListener(() => Leave(lessonScene)); // Retry starts again from the Lesson Screen
            timeUpMenuButton.onClick.AddListener(() => Leave(mainMenuScene)); // Back to Main Menu
            ShowTimer(); // Show 2:00
            continueButton.onClick.AddListener(Continue); // Continue goes to the next available level
            replayButton.onClick.AddListener(() => Leave(lessonScene)); // Replay starts again from the lesson
            mapButton.onClick.AddListener(() => Leave(mapScene)); // Back to the Landform Map
        }

        protected override void Update() // Runs every frame: checks the goal and the Back button
        {
            base.Update(); // Android Back (or Escape)
            if (Leaving) return; // Nothing more while leaving
            ShowProgress(); // Move the marker along the bar
            if (run.Outcome == RunOutcome.Playing) // The clock runs only while playing; the rules stop it for a pause or a pop-up
            {
                run.Tick(Time.deltaTime); // Time passes, also while the explorer is sent back (D-040)
                ShowTimer(); // Update the countdown
                if (run.IsTimeUp) { ShowTimeUp(); return; } // The countdown reached zero
            }
            if (run.Outcome != RunOutcome.Playing || run.IsPaused) return; // Only while playing, not leaving and no pop-up open
            if (respawning) return; // Wait while the explorer is being sent back
            CheckCheckpointAndHazards(); // The flag, the spikes and the pit
            if (respawning) return; // A hazard just sent the explorer back
            CheckPickups(); // Collect anything the explorer touches
            if (run.IsPaused) return; // A first find opened the pop-up: the goal waits
            if (explorer.Position.x >= run.Level.Goal.X - goalReach) Clear(); // Reached the cave entrance
        }

        void CheckCheckpointAndHazards() // Raises the flag, and sends the explorer back after a trap or a fall into the pit
        {
            RememberSafeSpot(); // Keep track of the last ground spot for the Easy pit rule
            var p = explorer.Position; // Feet of the explorer
            if (!run.CheckpointReached && p.x >= run.Level.Checkpoint.X - checkpointReach && run.ReachCheckpoint()) // First touch of the flag
            {
                checkpointFlag.sprite = checkpointOnSprite; // Show the raised green flag
                if (progressCheckpoint != null) progressCheckpoint.sprite = progressCheckpointOn; // The flag on the bar turns green too
                ShowGuide("Checkpoint reached!"); // Tell the player
                PlaySfx(checkpointSound); // A short ding
            }
            var body = new Rect(p.x - explorer.size.x / 2f, p.y, explorer.size.x, explorer.size.y); // The explorer body box
            bool onSpikes = false; // Did the body touch a spike trap?
            foreach (var area in spikeAreas) if (body.Overlaps(area)) onSpikes = true; // Check each trap
            if (onSpikes) StartCoroutine(Respawn(false)); // A trap: back to the last checkpoint
            else if (p.y < pitDeathY) StartCoroutine(Respawn(true)); // The pit
        }

        void RememberSafeSpot() // Remembers where the explorer last stood on the ground, and which way it was walking
        {
            if (!explorer.Grounded) return; // Only spots with ground under the feet
            lastGround = explorer.Position; // The spot
            lastGroundFacing = explorer.Facing; // The walking direction
        }

        Spot SafeSpotNearThePit() // A short way back from where the explorer last stood, on the same ground (Easy rule)
        {
            var spot = lastGround; // Start at the last ground spot
            var back = new Vector2(lastGround.x - lastGroundFacing * safeBackPixels, lastGround.y); // A short step back, away from the pit
            if (explorer.geometry.HasGroundAt(back.x, back.y)) spot = back; // Use the step back only when it has ground under it
            return new Spot(spot.x, spot.y); // The rules take a Spot
        }

        System.Collections.IEnumerator Respawn(bool fellInPit) // Sends the explorer back with a short, clear effect
        {
            respawning = true; // Count this hazard once
            PlaySfx(hitSound); // A short falling thud
            bool nearby = fellInPit && run.PitReturnsNearby; // Easy pits only return a short way back
            var back = fellInPit ? run.FallIntoPit(SafeSpotNearThePit()) : run.HitHazard(); // The rules say where to go; items and time are untouched
            explorer.Frozen = true; // The explorer stops for a moment
            ShowGuide(nearby ? "Oops! Try again from here" : run.CheckpointReached ? "Oops! Back to the checkpoint" : "Oops! Back to the start"); // Say what happened
            for (float t = 0f; t < respawnSeconds; t += Time.unscaledDeltaTime) // Red flash while waiting; real time, so a later pause menu cannot stall it
            {
                hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0.5f * (1f - t / respawnSeconds)); // Red that fades out
                yield return null; // Next frame
            }
            hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0f); // Flash gone
            explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // The backup respawn point stays the checkpoint or the start
            explorer.PlaceAt(new Vector2(back.X, back.Y)); // The explorer comes back here, standing still
            if (run.Outcome == RunOutcome.Playing) explorer.Frozen = false; // Play on
            respawning = false; // A new hazard may count again
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

        void ShowProgress() // Shows on the trail bar how far the explorer is from the start to the goal
        {
            if (progressFill == null || progressMarker == null) return; // A scene built before D-049 has no bar
            float walked = Mathf.Round(Mathf.Clamp01(explorer.Position.x / run.Level.Goal.X) * progressWidth); // Whole pixels along the groove
            progressFill.sizeDelta = new Vector2(walked, progressFill.sizeDelta.y); // Green up to the explorer
            var at = progressMarker.anchoredPosition; // The marker's place
            at.x = progressLeft + walked + 0.5f; // On the end of the green part; the half pixel keeps the 9-pixel marker on whole pixels
            progressMarker.anchoredPosition = at; // Move it
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
            pauseButton.gameObject.SetActive(false); // No pause button on the result
            FillResult(); // Items, goals and review for this attempt
            resultPanel.SetActive(true); // Show the result
            StartCoroutine(PopStars()); // Stars pop in one by one with a chime
        }

        void FillResult() // Shows what happened in the run on the Result Screen
        {
            foreach (var star in resultStars) star.sprite = starEmpty; // Stars start empty and pop in below
            var taken = run.TakenFlags; // Which pickups were taken
            for (int i = 0; i < resultItems.Length; i++) // One picture per pickup
            {
                var kind = run.Level.Pickups[i].Kind; // What it is
                resultItems[i].sprite = System.Array.Find(itemPictures, sp => sp != null && sp.name == kind.Picture); // Its picture
                resultItems[i].color = taken[i] ? Color.white : missedColor; // Taken in colour, missed as a grey outline
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
            for (int i = 0; i < run.Stars; i++) // Each earned star
            {
                yield return new WaitForSecondsRealtime(starPopSeconds); // A short pause between stars
                resultStars[i].sprite = starFull; // The star pops in
                if (chimes != null && chimes.Length > 0) // A chime for each star
                {
                    chimeSource.pitch = 1f + 0.15f * i; // Each chime a little higher
                    chimeSource.PlayOneShot(chimes[0], GameSettings.SfxVolume); // Play it at the player sound volume
                }
            }
            chimeSource.pitch = 1f; // Back to normal pitch
        }

        void Continue() // Goes to the next available level, or back to the trail when there is none
        {
            int next = GameProgress.Current.NextLevelAfter(episode, difficulty, number); // 0 when there is no next level
            if (next <= 0) { Leave(levelSelectionScene); return; } // Last level of the difficulty: back to the trail
            GameSession.Level = next; // The Lesson Screen reads the level number from here
            Leave(lessonScene); // Next level starts from its lesson (Easy 2 shows Coming soon until it is built)
        }

        protected override void Back() // Android Back or Escape
        {
            if (run.Outcome == RunOutcome.TimeUp) { Leave(mainMenuScene); return; } // Time up: Back means Back to Main Menu
            if (run.Outcome == RunOutcome.Cleared) { Leave(levelSelectionScene); return; } // After the goal: back to the level trail
            if (run.PendingFind != null || respawning) return; // The pop-up blocks all other input until OK
            if (run.IsMenuOpen) ClosePause(); // Back again closes the pause menu
            else OpenPause(); // Android Back opens the pause menu during play
        }

        void PlaySfx(AudioClip clip) // Plays a sound effect at the player sound-effects volume
        {
            if (clip != null) chimeSource.PlayOneShot(clip, GameSettings.SfxVolume); // Quiet or silent when the player lowered the slider
        }

        void OpenPause() // Opens the pause menu
        {
            if (respawning || !run.Pause()) return; // Not while being sent back, behind the pop-up or after the end
            if (music != null) music.DuckTo(0.3f); // The music and the breeze play on, but quietly, while paused
            explorer.Frozen = true; // The explorer stops
            pausePanel.SetActive(true); // Show the menu; its backdrop blocks the controls
        }

        void ClosePause() // Closes the pause menu
        {
            run.Resume(); // The game clock runs again
            if (music != null) music.DuckTo(1f); // Back to normal loudness
            pausePanel.SetActive(false); // Hide the menu
            explorer.Frozen = false; // The explorer can move again
        }

        void ShowTimer() // Shows the countdown as minutes and seconds
        {
            int seconds = Mathf.CeilToInt(run.RemainingSeconds); // Whole seconds left, rounded up so 0:00 only shows at the end
            timerLabel.text = (seconds / 60) + ":" + (seconds % 60).ToString("00"); // For example 1:05
            timerLabel.color = seconds <= 10 ? timerLow : timerNormal; // Warn in the last 10 seconds
        }

        void ShowTimeUp() // Time ran out: stop everything and offer Retry or Back to Main Menu
        {
            explorer.Frozen = true; // The explorer stops
            touchControls.SetActive(false); // Hide the controls
            pauseButton.gameObject.SetActive(false); // No pause button now
            pausePanel.SetActive(false); // The pause menu cannot stay open
            findPanel.SetActive(false); // Nor the pop-up
            if (music != null) music.DuckTo(0.3f); // Quiet music under the Time up panel
            timeUpPanel.SetActive(true); // Show the Time up panel
        }
    }
}
