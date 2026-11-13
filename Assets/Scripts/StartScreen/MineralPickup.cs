using UnityEngine; // Gives access to Unity basics such as MonoBehaviour and Vector3

namespace LandGrand.StartScreen
{
    /// <summary>One rock or mineral on the path. It scrolls with the ground until the explorer picks it up.</summary>
    public class MineralPickup : MonoBehaviour
    {
        enum State { OnPath, Waiting, Held, Stashed } // Steps in a mineral's life: on the path, waiting, held, put away

        public MineralSpawner spawner; // The spawner that made this mineral
        State state = State.OnPath; // The step it is in now; it starts on the path
        SpriteRenderer sr; // Draws the mineral
        float stashTime; // Seconds since it started going into the backpack
        Vector3 stashFrom; // Where it was when it started going into the backpack

        void Awake() => sr = GetComponent<SpriteRenderer>(); // Find the sprite once at the start

        void Update() // Runs every frame
        {
            if (spawner == null) { Destroy(gameObject); return; } // Remove it if its spawner is gone

            switch (state) // Do what fits the current step
            {
                case State.OnPath: // Lying on the path
                    transform.position += Vector3.left * (spawner.GroundSpeedUnits * ParallaxLayer.WalkSpeedScale * Time.deltaTime); // Scroll left with the ground
                    if (spawner.Touches(transform.position) && spawner.Collector != null && spawner.Collector.TryCollect(this)) // When the explorer reaches it, ask the explorer to pick it up
                        state = State.Waiting; // Wait for the explorer's hand
                    break; // Done for this frame

                case State.Stashed: // Going into the backpack
                    // Shrink into the backpack over 0.2 seconds.
                    stashTime += Time.deltaTime; // Add this frame's time
                    float t = Mathf.Clamp01(stashTime / 0.2f); // How far along, from 0 to 1
                    transform.position = Vector3.Lerp(stashFrom, spawner.BackpackPosition, t); // Move toward the backpack
                    transform.localScale = Vector3.one * (1f - t); // Shrink as it goes
                    if (t >= 1f) Destroy(gameObject); // Remove it once it is inside
                    break; // Done for this frame
            }
        }

        /// <summary>The explorer's hand has closed on the mineral: play the chime.</summary>
        public void OnGrabbed() // Called when the glove closes on it
        {
            state = State.Held; // Now held
            spawner.PlayPickupSound(); // Play a chime
        }

        /// <summary>Keeps the mineral in the explorer's hand, drawn in front of the explorer.</summary>
        public void HoldAt(Vector3 handPosition) // Called each frame while held
        {
            if (state != State.Held) return; // Only while held
            transform.position = new Vector3(Mathf.Round(handPosition.x * 32f) / 32f, Mathf.Round(handPosition.y * 32f) / 32f, 0f); // Move to the glove, snapped to whole pixels
            sr.sortingOrder = spawner.heldSortingOrder; // Draw it in front of the explorer
        }

        /// <summary>The explorer holds it up: sparkle.</summary>
        public void OnShown() => spawner.SparkleAt(transform.position); // Called when held up: sparkle

        /// <summary>The explorer puts it in the backpack.</summary>
        public void OnStashed() // Called when it goes into the backpack
        {
            if (state == State.Stashed) return; // Only start once
            state = State.Stashed; // Now going into the backpack
            stashFrom = transform.position; // Remember where it starts
            stashTime = 0f; // Start the shrink timer
        }
    }
}
