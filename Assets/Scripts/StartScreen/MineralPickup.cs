using UnityEngine;

namespace LandGrand.StartScreen
{
    /// <summary>One rock or mineral on the path. It scrolls with the ground until the explorer picks it up.</summary>
    public class MineralPickup : MonoBehaviour
    {
        enum State { OnPath, Waiting, Held, Stashed }

        public MineralSpawner spawner;
        State state = State.OnPath;
        SpriteRenderer sr;
        float stashTime;
        Vector3 stashFrom;

        void Awake() => sr = GetComponent<SpriteRenderer>();

        void Update()
        {
            if (spawner == null) { Destroy(gameObject); return; }

            switch (state)
            {
                case State.OnPath:
                    transform.position += Vector3.left * (spawner.GroundSpeedUnits * ParallaxLayer.WalkSpeedScale * Time.deltaTime);
                    if (spawner.Touches(transform.position) && spawner.Collector != null && spawner.Collector.TryCollect(this))
                        state = State.Waiting;
                    break;

                case State.Stashed:
                    // Shrink into the backpack over 0.2 seconds.
                    stashTime += Time.deltaTime;
                    float t = Mathf.Clamp01(stashTime / 0.2f);
                    transform.position = Vector3.Lerp(stashFrom, spawner.BackpackPosition, t);
                    transform.localScale = Vector3.one * (1f - t);
                    if (t >= 1f) Destroy(gameObject);
                    break;
            }
        }

        /// <summary>The explorer's hand has closed on the mineral: play the chime.</summary>
        public void OnGrabbed()
        {
            state = State.Held;
            spawner.PlayPickupSound();
        }

        /// <summary>Keeps the mineral in the explorer's hand, drawn in front of the explorer.</summary>
        public void HoldAt(Vector3 handPosition)
        {
            if (state != State.Held) return;
            transform.position = new Vector3(Mathf.Round(handPosition.x * 32f) / 32f, Mathf.Round(handPosition.y * 32f) / 32f, 0f);
            sr.sortingOrder = spawner.heldSortingOrder;
        }

        /// <summary>The explorer holds it up: sparkle.</summary>
        public void OnShown() => spawner.SparkleAt(transform.position);

        /// <summary>The explorer puts it in the backpack.</summary>
        public void OnStashed()
        {
            if (state == State.Stashed) return;
            state = State.Stashed;
            stashFrom = transform.position;
            stashTime = 0f;
        }
    }
}
