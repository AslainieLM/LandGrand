using WindZone = LandGrand.Rules.WindZone; // Uses the game gust rule instead of Unity wind
using LandGrand.Rules; // Uses gust timing
using UnityEngine; // Uses sprites and time
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    public class WindZoneView : MonoBehaviour // Shows one gust area
    { // Opens the view
        public float x, width, offset; // Saved gust placement
        public SpriteRenderer[] streaks; // Six streaks across the area
        public Sprite[] frames; // Four gust pictures
        public ExplorerMotor explorer; // Distance for hearing
        public LevelController level; // Plays the warning sound
        public AudioClip gustSound; // Warning gust sound
        WindZone rule; // Plain gust rule
        int lastWarning; // Counts warnings once
        void Start() // Reads the placement
        { // Starts setup
            rule = new WindZone(x, width, offset); // Makes the rule
            lastWarning = Mathf.FloorToInt((LevelClock.Now + offset - WindZone.WarnAt) / WindZone.CycleSeconds); // Avoids a false opening sound
        } // Ends setup
        void Update() // Draws streaks during warning and push
        { // Starts the frame
            float now = LevelClock.Now; // Shared level clock
            bool visible = rule.PhaseAt(now) != WindPhase.Calm; // Calm has no streaks
            for (int i = 0; i < streaks.Length; i++) // Each streak
            { // Opens the streak update
                streaks[i].sprite = visible ? frames[((int)(now * 10f) + i) % frames.Length] : null; // Runs the gust frames
                float along = Mathf.Repeat(i * width / streaks.Length - now * 50f, width); // Blows left across the area
                streaks[i].transform.position = LevelGeometry.ToWorld(Mathf.Round(x + along), 24f + (i % 3) * 28f); // Spreads streaks above the ground
            } // Ends the streak update
            int warning = Mathf.FloorToInt((now + offset - WindZone.WarnAt) / WindZone.CycleSeconds); // Current warning count
            if (warning > lastWarning && explorer.Position.x >= x - 260f && explorer.Position.x <= x + width + 260f && !explorer.Frozen) level.PlaySfx(gustSound); // Hears one nearby warning
            lastWarning = warning; // Remembers the warning
        } // Ends the frame
    } // Ends the view
} // Ends the play code
