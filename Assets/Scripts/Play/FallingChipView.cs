using UnityEngine; // Uses sprites and movement
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    public class FallingChipView : MonoBehaviour // A short decorative falling chip
    { // Opens the effect
        Vector2 position, velocity; // Pixel position and speed
        float age; // Time since the break
        public static void Spawn(Sprite sprite, Vector2 at, Vector2 speed) // Makes one chip
        { // Starts the effect
            var go = new GameObject("Crumble Chip"); // Owns the short effect
            var art = go.AddComponent<SpriteRenderer>(); // Draws the chip
            art.sprite = sprite; art.sortingOrder = 7; // Shows it above the ground
            var chip = go.AddComponent<FallingChipView>(); // Moves the chip
            chip.position = at; chip.velocity = speed; // Starts at the slab
            go.transform.position = LevelGeometry.ToWorld(at.x, at.y); // Shows its first position
        } // Ends setup
        void Update() // Moves the chip down
        { // Starts the frame
            if (!LevelClock.Running) return; // Hangs still while the game is paused
            age += Time.deltaTime; // Counts its short life
            velocity.y -= 220f * Time.deltaTime; // Falls faster
            position += velocity * Time.deltaTime; // Moves in pixels
            transform.position = LevelGeometry.ToWorld(Mathf.Round(position.x), Mathf.Round(position.y)); // Keeps crisp pixels
            if (age >= 0.5f) Destroy(gameObject); // Removes the finished effect
        } // Ends the frame
    } // Ends the effect
} // Ends the play code
