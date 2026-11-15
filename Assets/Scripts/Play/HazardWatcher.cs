using LandGrand.Rules; // Gives access to Spot
using UnityEngine; // Gives access to MonoBehaviour, Rect and Vector2
using UnityEngine.UI; // Gives access to Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The checkpoint flag, the spike traps and the pits. A trap or a fall sends the explorer back with a short red flash:
    /// on Easy a short way back on safe ground (D-048, D-056), otherwise to the checkpoint or the start (D-040).
    /// The rules in LevelRun decide where; this part watches the explorer and shows it.
    /// </summary>
    public class HazardWatcher : MonoBehaviour // One part of the level screen
    {
        public Rect[] spikeAreas; // Danger areas of the spike traps, in level pixels
        public float pitDeathY = -40f; // Falling below this height means the explorer fell into the pit
        public float checkpointReach = 8f; // How close (in pixels) the explorer must get to the checkpoint flag
        public SpriteRenderer[] checkpointFlags; // One flag per checkpoint
        public Sprite checkpointOnSprite; // The raised green flag shown once it is reached
        public Image hazardFlash; // Red flash over the screen when the explorer is sent back
        public float respawnSeconds = 0.35f; // How long the explorer waits, frozen, before coming back
        public float safeBackPixels = 24f; // Easy: how far back from the last ground spot the explorer comes back
        public AudioClip hitSound; // Played when a trap or the pit sends the explorer back
        public AudioClip checkpointSound; // Played when the checkpoint flag is reached

        RollingBoulderWatcher rollingBoulders; // Optional part for boulder traps
        LevelController level; // The level these hazards belong to
        Vector2 lastGround; // Where the explorer last stood on safe ground
        int lastGroundFacing = 1; // Which way it was walking then

        /// <summary>True while the explorer is being sent back, so a hazard counts once and the menus wait.</summary>
        internal bool Respawning { get; private set; } // Set by Respawn

        internal bool IsBuilt => spikeAreas != null && checkpointFlags != null && hazardFlash != null; // False for a scene built before this part existed

        internal void Begin(LevelController owner) // Connects the hazards to their level
        {
            level = owner; // Remember the level
            rollingBoulders = GetComponent<RollingBoulderWatcher>(); // The level only has this part if boulders can roll
            lastGround = level.explorer.RespawnPoint; // Until the explorer stands somewhere else, the start is the safe spot
        }

        internal void Check() // Raises the flag, and sends the explorer back after a trap or a fall into the pit
        {
            var run = level.Run; // The attempt
            var explorer = level.explorer; // The explorer
            RememberSafeSpot(); // Keep track of the last ground spot for the Easy rule
            var p = explorer.Position; // Feet of the explorer
            for (int i = 0; i < run.Level.Checkpoints.Count; i++) // Checks flags in trail order
            { // Opens the flag check
                if (p.x < run.Level.Checkpoints[i].X - checkpointReach || !run.ReachCheckpoint(i)) continue; // Only newly reached flags count
                if (i < checkpointFlags.Length) checkpointFlags[i].sprite = checkpointOnSprite; // Raises this flag; an older scene may have fewer flags
                if (level.progressBar != null) level.progressBar.MarkCheckpoint(i); // Raises its trail marker
                explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // Updates the motor backup too
                level.Say("Checkpoint reached!"); // Announces each flag
                level.PlaySfx(checkpointSound); // Plays the ding
            } // Ends the flag check
            if (TouchesSpikes(p) || TouchesRock(p) || (rollingBoulders != null && rollingBoulders.Hits(level.ExplorerBodyAt(p)))) StartCoroutine(Respawn(false)); // A trap: on Easy a short way back, otherwise the last checkpoint
            else if (p.y < pitDeathY) StartCoroutine(Respawn(true)); // The pit
        }

        void RememberSafeSpot() // Remembers where the explorer last stood on safe ground, and which way it was walking
        {
            var explorer = level.explorer; // The explorer
            if (!explorer.Grounded || TouchesSpikes(explorer.Position)) return; // Only spots with ground under the feet and no spikes
            lastGround = explorer.Position; // The spot
            lastGroundFacing = explorer.Facing; // The walking direction
        }

        bool TouchesSpikes(Vector2 feet) // True when a body standing at these feet would touch a spike trap
        {
            var body = level.ExplorerBodyAt(feet); // The explorer body box there
            foreach (var area in spikeAreas) if (body.Overlaps(area)) return true; // Any trap counts
            return false; // Clear of every trap
        }

        bool TouchesRock(Vector2 feet) // Checks rock hit boxes at the displayed time
        { // Starts the hit check
            var body = level.ExplorerBodyAt(feet); // Current explorer body
            foreach (var rock in level.Run.Level.Layout.Rockfalls) // Checks the placed rocks
                if (rock.Hits(body.x, body.y, body.width, body.height, LevelClock.Now)) return true; // A falling or landed rock touched the body
            return false; // No rock touched the body
        } // Ends the hit check

        Spot SafeSpotNearby() // A short way back from where the explorer last stood, on the same ground and clear of spikes (Easy rule)
        {
            var spot = lastGround; // Start at the last safe ground spot
            var back = new Vector2(lastGround.x - lastGroundFacing * safeBackPixels, lastGround.y); // A short step back, away from the danger
            if (level.explorer.geometry.HasGroundAt(back.x, back.y) && !TouchesSpikes(back)) spot = back; // Use the step back only when it has ground and no spikes
            return new Spot(spot.x, spot.y); // The rules take a Spot
        }

        System.Collections.IEnumerator Respawn(bool fellInPit) // Sends the explorer back with a short, clear effect
        {
            var run = level.Run; // The attempt
            var explorer = level.explorer; // The explorer
            Respawning = true; // Count this hazard once
            level.PlaySfx(hitSound); // A short falling thud
            bool nearby = fellInPit ? run.PitReturnsNearby : run.TrapReturnsNearby; // Easy pits and traps only return a short way back (D-048, D-056)
            var back = fellInPit ? run.FallIntoPit(SafeSpotNearby()) : run.TouchTrap(SafeSpotNearby()); // The rules say where to go; items and time are untouched
            explorer.Frozen = true; // The explorer stops for a moment
            level.Say(nearby ? "Oops! Try again from here" : run.CheckpointReached ? "Oops! Back to the checkpoint" : "Oops! Back to the start"); // Say what happened
            for (float t = 0f; t < respawnSeconds; t += Time.unscaledDeltaTime) // Red flash while waiting; real time, so a later pause menu cannot stall it
            {
                hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0.5f * (1f - t / respawnSeconds)); // Red that fades out
                yield return null; // Next frame
            }
            hazardFlash.color = new Color(0.9f, 0.1f, 0.1f, 0f); // Flash gone
            explorer.RespawnPoint = new Vector2(run.RespawnPoint.X, run.RespawnPoint.Y); // The backup respawn point stays the checkpoint or the start
            explorer.PlaceAt(new Vector2(back.X, back.Y)); // The explorer comes back here, standing still
            if (run.Outcome == RunOutcome.Playing) explorer.Frozen = false; // Play on
            Respawning = false; // A new hazard may count again
        }
    }
}
