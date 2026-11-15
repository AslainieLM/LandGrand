using LandGrand.Rules; // Gives access to SteamVent and VentPhase
using UnityEngine; // Gives access to MonoBehaviour, Sprite and SpriteRenderer

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Shows one steam vent's cycle (D-055): nothing while resting, a small wisp as a warning, then a tall puff.
    /// It only draws; the level controller asks the rules whether a puff launches the explorer.
    /// </summary>
    public class SteamVentView : MonoBehaviour // Draws the steam above one vent
    {
        public SpriteRenderer warn; // The small wisp; its bottom sits on the vent
        public SpriteRenderer steam; // The tall puff; its bottom sits on the vent
        public Sprite[] warnFrames; // Small wisp frames for the warning
        public Sprite[] puffFrames; // Tall steam column frames for the puff
        public float warnFramesPerSecond = 6f; // How fast the wisp flickers

        void Update() // Runs every frame: picks the picture for the current phase
        {
            float t = LevelClock.Now; // Same paused-aware clock the controller uses, so the picture and the launch agree
            var phase = SteamVent.PhaseAt(t); // Resting, warning or puffing
            bool hasWarn = phase == VentPhase.Warning && warnFrames != null && warnFrames.Length > 0; // Warning, and wisp pictures to show
            warn.sprite = hasWarn ? warnFrames[(int)(t * warnFramesPerSecond) % warnFrames.Length] : null; // The flickering wisp, only as a warning
            if (phase != VentPhase.Puff || puffFrames == null || puffFrames.Length == 0) { steam.sprite = null; return; } // No puff now, or no puff pictures
            steam.sprite = Frame(puffFrames, (int)(SteamVent.PuffProgress(t) * puffFrames.Length)); // The puff grows through its frames
        }

        static Sprite Frame(Sprite[] frames, int i) => frames == null || frames.Length == 0 ? null : frames[Mathf.Clamp(i, 0, frames.Length - 1)]; // One frame, never past the last
    }
}
