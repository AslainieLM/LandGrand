using LandGrand.Rules; // Gives access to LevelCatalog and LevelRun
using UnityEngine; // Gives access to GameObject, AudioSource and Vector2

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Runs one level. One script runs every level; the builder sets which level a scene plays.
    /// It owns the attempt (LevelRun) and the frame loop, and hands each job to its own part on the same object:
    /// TrailProgressBar (the bar under the countdown), PickupCollector (items and the first-find pop-up),
    /// HazardWatcher (checkpoint, spikes and pits), LevelTimer (the countdown), LevelMenus (pause and Time's up)
    /// and ResultScreen ("Level cleared!"). The start banner and the steam vents (D-055) stay here.
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
        public TrailProgressBar progressBar; // The bar under the countdown
        public PickupCollector collector; // Items, the counter and the first-find pop-up
        public HazardWatcher hazards; // Checkpoint, spike traps and pits
        public LevelTimer timer; // The countdown
        public LevelMenus menus; // Pause button, pause menu and Time's up panel
        public ResultScreen result; // "Level cleared!" panel
        public WindWatcher wind; // Wind gust push on the explorer
        public LevelBanner banner; // Slides the level name in before the clock starts (D-055)

        [Header("Sounds")] // Inspector heading for the shared sounds
        public AudioClip[] chimes; // Chimes for items, stars and badges
        public AudioClip steamSound; // Played when the vents puff near the explorer (D-055)
        public float steamHearingPixels = 260f; // How near (in pixels) a puffing vent must be to be heard

        [Header("Scenes")] // Inspector heading for the scene names
        public string levelSelectionScene = "LevelSelection"; // Where Next goes when nothing open follows, Levels in the pause and Time's up menus, and Back after the goal or time up (D-064)
        public string lessonScene = "Lesson"; // Where Retry and Replay go (D-040)
        public string mapScene = "LandformMap"; // Where Back to Map goes

        LevelRun run; // This attempt
        AudioSource sounds; // Plays the chimes and the sound effects
        VentPhase lastVentPhase; // The vents' phase last frame, to hear the start of each puff

        /// <summary>This attempt: time, items, checkpoint and outcome. The parts read and change it.</summary>
        internal LevelRun Run => run; // Shared with the parts

        /// <summary>The level's sound player, shared by the parts.</summary>
        internal AudioSource Sounds => sounds; // Shared with the parts

        protected override void Awake() // Runs once when the level opens: starts the attempt and its parts
        {
            base.Awake(); // Click sound and hidden guide
            LevelClock.Reset(); // Moving objects start their cycles at time 0
            if (!PartsBuilt()) // An old scene, built before the parts were split
            {
                Debug.LogError("This level scene is out of date. Run LandGrand > Build Levels, then press Play again."); // Say what to do
                enabled = false; // Stop Update so the console shows one clear message, not an error every frame
                return; // Do not start the attempt
            }
            Application.targetFrameRate = 60; // Smooth movement; Android would otherwise stay at 30 frames per second
            run = new LevelRun(LevelCatalog.Find(episode, difficulty, number)); // A fresh attempt at this scene's level
            explorer.GroundWalkSpeed = run.Level.Layout.GroundWalkSpeed; // Gives the motor the optional ground speed from this trail
            sounds = gameObject.AddComponent<AudioSource>(); // Sound player for the chimes and effects
            sounds.playOnAwake = false; // Silent until something happens
            explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // The attempt decides where the explorer comes back
            explorer.PlaceAt(explorer.RespawnPoint); // Put the explorer at the start
            result.Begin(this); // Hidden panel; remembers the badges before the level
            collector.Begin(this); // Hidden pop-up and the 0/5 count
            hazards.Begin(this); // The start is the first safe spot
            progressBar.Begin(this); // The marker at the left end
            menus.Begin(this); // Hidden menus and their buttons
            timer.Begin(this); // Shows 2:00
            wind.Begin(this); // Calm until a gust zone pushes
            StartBanner(); // The level name slides in; the clock waits for it (D-055)
        }

        bool PartsBuilt() // True when the scene has every part and each part has its pieces
        {
            return explorer != null && touchControls != null // The explorer and its controls
                && progressBar != null && progressBar.IsBuilt // The trail bar
                && collector != null && collector.IsBuilt // Items and pop-up
                && hazards != null && hazards.IsBuilt // Checkpoint, spikes and pits
                && timer != null && timer.IsBuilt // The countdown
                && menus != null && menus.IsBuilt // Pause and Time's up
                && result != null && result.IsBuilt // Result Screen
                && wind != null && wind.IsBuilt; // Wind gusts
        }

        protected override void Update() // Runs every frame: the clock, then hazards, vents, pickups and the goal
        {
            base.Update(); // Android Back (or Escape)
            LevelClock.Step(!Leaving && run.Outcome == RunOutcome.Playing && !run.IsPaused, Time.deltaTime); // Moving objects wait during the banner, the pause menu and the find pop-up
            if (Leaving) return; // Nothing more while leaving
            progressBar.Show(); // Move the marker along the bar
            if (run.Outcome == RunOutcome.Playing) // The clock runs only while playing; the rules stop it for a pause or a pop-up
            {
                run.Tick(Time.deltaTime); // Time passes, also while the explorer is sent back (D-040)
                timer.Show(); // Update the countdown
                if (run.IsTimeUp) { TimeUp(); return; } // The countdown reached zero
            }
            if (run.Outcome != RunOutcome.Playing || run.IsPaused) return; // Only while playing, not leaving and no pop-up open
            if (hazards.Respawning) return; // Wait while the explorer is being sent back
            hazards.Check(); // The flag, the spikes and the pit
            if (hazards.Respawning) return; // A hazard just sent the explorer back
            CheckVents(); // A puffing vent under the feet throws the explorer up
            collector.Check(); // Collect anything the explorer touches
            if (run.IsPaused) return; // A first find opened the pop-up: the goal waits
            if (explorer.Position.x >= run.Level.Goal.X - goalReach) Clear(); // Reached the goal
        }

        void StartBanner() // Holds the attempt while the level start banner shows
        {
            if (banner == null) return; // A scene without a banner starts at once
            run.BeginIntro(); // The clock waits
            explorer.Frozen = true; // The explorer waits too
            banner.Play(run.Level.BannerName, EndBanner); // Show the name; EndBanner runs when it has gone
        }

        void EndBanner() // The banner has gone: the attempt really starts
        {
            run.EndIntro(); // The clock starts
            if (run.Outcome == RunOutcome.Playing && !run.IsPaused) explorer.Frozen = false; // The explorer may move
        }

        void CheckVents() // Launches the explorer from a puffing vent, and plays the hiss for puffs nearby (D-055)
        {
            var vents = run.Level.Layout.Vents; // The level's steam vents
            if (vents.Count == 0) return; // Most levels have none
            float t = LevelClock.Now; // The vents' shared clock, as SteamVentView uses; it stops while paused
            var phase = SteamVent.PhaseAt(t); // Resting, warning or puffing
            bool puffStarted = phase == VentPhase.Puff && lastVentPhase != VentPhase.Puff; // A new puff began this frame
            lastVentPhase = phase; // Remember for next frame
            var p = explorer.Position; // Feet of the explorer
            foreach (var vent in vents) // Each vent
            {
                if (puffStarted && Mathf.Abs(vent.CenterX - p.x) < steamHearingPixels) PlaySfx(steamSound); // Hear the puff when near it
                if (run.VentLaunches(vent, p.x, explorer.Grounded, t)) explorer.Launch(SteamVent.LaunchSpeed); // The rules say the puff throws the explorer up
            }
        }

        void Clear() // The level is cleared
        {
            if (!run.ReachGoal()) return; // Count the goal once
            run.RecordResult(GameProgress.Current); // Keep the clear and its stars
            GameProgress.Save(); // Write it to the device now
            StopPlay(); // The explorer stops at the goal
            result.Show(); // Stars, items, goals, review, badges and the buttons
        }

        void TimeUp() // Time ran out: stop everything and offer Retry or Back to Main Menu
        {
            StopPlay(); // The explorer stops
            collector.HidePopup(); // The pop-up cannot stay open
            menus.ShowTimeUp(); // The Time's up panel
        }

        void StopPlay() // The attempt is over: freeze the explorer and hide the controls
        {
            explorer.Frozen = true; // The explorer stops
            touchControls.SetActive(false); // Hide the on-screen controls
            menus.HidePauseButton(); // No pause button now
        }

        protected override void Back() // Android Back or Escape
        {
            if (run.Outcome == RunOutcome.TimeUp || run.Outcome == RunOutcome.Cleared) { Leave(levelSelectionScene); return; } // Time up or after the goal: back to the level trail (D-064)
            if (run.PendingFind != null || hazards.Respawning) return; // The pop-up blocks all other input until OK
            if (run.IsMenuOpen) menus.ClosePause(); // Back again closes the pause menu
            else menus.OpenPause(); // Android Back opens the pause menu during play
        }

        /// <summary>The explorer's body box if its feet stood here, in level pixels.</summary>
        internal Rect ExplorerBodyAt(Vector2 feet) => new Rect(feet.x - explorer.size.x / 2f, feet.y, explorer.size.x, explorer.size.y); // Shared by pickups and hazards

        internal void PlaySfx(AudioClip clip, float volume = 1f) // Plays a sound effect at the player sound-effects volume, scaled by an optional part volume
        {
            if (clip != null) sounds.PlayOneShot(clip, volume * GameSettings.SfxVolume); // Quiet or silent when the player lowered the slider; volume 1 keeps the old loudness
        }

        internal void Say(string text) => ShowGuide(text); // Shows a short guide message, for the parts

        internal void GoTo(string scene) => Leave(scene); // Fades out and opens another scene, for the parts
    }
}
