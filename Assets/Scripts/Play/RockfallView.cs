using LandGrand.Rules; // Uses rock timing
using LandGrand.StartScreen; // Uses the dust animation
using UnityEngine; // Uses sprites and time
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    public class RockfallView : MonoBehaviour // Draws one rock cycle
    { // Opens the view
        public float x, offset; // Saved layout placement
        public SpriteRenderer shadow, rock, pebbles; // Three parts of the warning and fall
        public Sprite[] warningFrames, pebbleFrames, dustFrames; // Warning and landing pictures
        public Sprite rockSprite; // Falling rock picture
        public ExplorerMotor explorer; // Distance for hearing
        public LevelController level; // Plays the landing sound
        public AudioClip landSound; // Rock landing sound
        Rockfall rule; // Plain rock rule
        RockfallPhase previous; // Detects the start of dust
        int lastLanding; // Counts landing cycles
        void Start() // Reads the placement
        { // Starts setup
            rule = new Rockfall(x, offset); // Makes the rule
            lastLanding = Mathf.FloorToInt((LevelClock.Now + offset - Rockfall.Land) / Rockfall.CycleSeconds); // Avoids a false sound on opening
        } // Ends setup
        void Update() // Draws the current phase
        { // Starts the frame
            float now = LevelClock.Now; // Shared level clock
            var phase = rule.PhaseAt(now); // Reads the rule
            bool warning = phase == RockfallPhase.Warning; // Shadow and trickling pebbles
            shadow.sprite = warning ? warningFrames[rule.CycleTime(now) < 0.6f ? 0 : 1] : null; // Warning grows darker
            pebbles.sprite = warning ? pebbleFrames[(int)(now * 8f) % pebbleFrames.Length] : null; // Pebble frames
            pebbles.transform.position = LevelGeometry.ToWorld(x, Mathf.Round(160f - (now * 90f % 140f))); // Pebbles trickle down
            float? bottom = rule.RockBottomAt(now); // Null when the rock is absent
            rock.sprite = bottom.HasValue ? rockSprite : null; // Shows only the active rock
            if (bottom.HasValue) rock.transform.position = LevelGeometry.ToWorld(x, Mathf.Round(bottom.Value + 8f)); // Centres the 16-pixel picture above its bottom
            int landing = Mathf.FloorToInt((now + offset - Rockfall.Land) / Rockfall.CycleSeconds); // Current landing count
            if (landing > lastLanding && Mathf.Abs(explorer.Position.x - x) <= 260f && !explorer.Frozen) level.PlaySfx(landSound); // Hears one nearby impact
            lastLanding = landing; // Remembers this cycle
            if (phase == RockfallPhase.Clear && previous != RockfallPhase.Clear) Dust(); // Dust when the landed rock disappears
            previous = phase; // Remembers the phase
        } // Ends the frame
        void Dust() // Reuses the start-screen dust
        { // Starts the effect
            var go = new GameObject("Rock Dust"); // Owns the short puff
            go.transform.position = LevelGeometry.ToWorld(x, 2f); // Sits on the ground
            var art = go.AddComponent<SpriteRenderer>(); // Draws the puff
            art.sortingOrder = 19; // Behind the explorer
            var flip = go.AddComponent<SpriteFlipbook>(); // Plays the dust frames
            flip.frames = dustFrames; flip.framesPerSecond = 14f; // Quick landing puff
            flip.loop = false; flip.destroyWhenDone = true; // Removes the finished puff
        } // Ends the effect
    } // Ends the view
} // Ends the play code
