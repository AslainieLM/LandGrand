using LandGrand.StartScreen; // Gives access to SpriteFlipbook for the dust puff
using UnityEngine; // Gives access to MonoBehaviour, Rect, Sprite and Time

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Moves the Cave Spelunker explorer (decision D-040): walk while an arrow is held, jump,
    /// and one lower double jump in the air with a dust puff. Positions are whole level pixels so the art stays sharp.
    /// The explorer's position is the middle of its feet.
    /// </summary>
    public class ExplorerMotor : MonoBehaviour // The explorer's movement and animation
    {
        [Header("Parts")] // Inspector heading for the connected parts
        public LevelInput input; // The player's controls
        public LevelGeometry geometry; // Ground, boulders and ledges
        public SpriteRenderer body; // The explorer picture, a child placed so its feet touch the position
        public Sprite[] walkFrames; // Walk cycle
        public Sprite standFrame; // Standing still
        public Sprite riseFrame; // Going up in a jump
        public Sprite fallFrame; // Coming down
        public Sprite[] dustFrames; // Dust puff for the double jump
        public Sprite[] flipFrames; // Curled-up somersault pictures for the double jump (decision D-046)

        [Header("Feel (pixels and seconds)")] // Inspector heading for the movement numbers
        public float walkSpeed = 72f; // Walking speed
        public float gravity = 900f; // How fast falling speeds up
        public float maxFallSpeed = 420f; // Fastest fall
        public float jumpSpeed = 290f; // First jump: about 47 pixels high
        public float doubleJumpSpeed = 230f; // Double jump: a little lower, about 29 pixels
        public float coyoteSeconds = 0.08f; // A jump still works this long after walking off an edge
        public float jumpBufferSeconds = 0.1f; // A jump pressed this long before landing still happens
        public float walkFramesPerSecond = 10f; // Walk animation speed
        public float flipSeconds = 0.3f; // How long the somersault lasts
        public Vector2 size = new Vector2(12f, 26f); // Body box: width and height
        public float fallOutY = -120f; // Falling below this puts the explorer back at the respawn point

        /// <summary>True stops all movement, for example after the goal.</summary>
        public bool Frozen { get; set; } // Stops the explorer

        /// <summary>Where the explorer appears after falling out. The level controller sets it.</summary>
        public Vector2 RespawnPoint { get; set; } // Start of the level for now

        /// <summary>The middle of the explorer's feet, in level pixels.</summary>
        public Vector2 Position => position; // Read by the camera and the goal check

        /// <summary>True while the explorer stands on something.</summary>
        public bool Grounded => grounded; // Read by the level controller to remember the last safe spot

        /// <summary>1 when the explorer faces right, -1 when it faces left.</summary>
        public int Facing => facing; // Tells which way the explorer was walking

        Vector2 position; // Feet position in level pixels
        Vector2 velocity; // Speed in pixels per second
        bool grounded; // Standing on something
        float lastGroundedTime = -1f; // When the explorer last stood on something
        bool jumpedFromGround; // A ground jump was used since the last landing
        bool airJumpUsed; // The double jump was used since the last landing
        float jumpBufferLeft; // Seconds left for a waiting jump press
        float walkClock; // Time spent walking, for the animation
        int facing = 1; // 1 faces right, -1 faces left
        float flipClock = -1f; // Seconds into the somersault; below 0 means no somersault

        /// <summary>Puts the explorer at a place, standing still.</summary>
        public void PlaceAt(Vector2 feet) // Moves the explorer at once
        {
            position = feet; // New place
            velocity = Vector2.zero; // No speed
            grounded = false; // Gravity will settle it on the ground
            airJumpUsed = false; // Both jumps ready again
            jumpedFromGround = false; // Both jumps ready again
            flipClock = -1f; // No somersault
            Apply(); // Show it there
        }

        void Update() // Runs every frame: reads the controls, moves and animates
        {
            if (Frozen) { velocity = Vector2.zero; Animate(); return; } // Stand still while frozen
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f); // Never step more than a 30th of a second, so nothing passes through ground

            int dir = input.Direction; // Walk left, right or not at all
            velocity.x = dir * walkSpeed; // Walking starts and stops at once, which feels responsive
            if (dir != 0) facing = dir; // Face the way of walking

            bool pressed = input.JumpPressed; // Jump pressed this frame
            jumpBufferLeft = pressed ? jumpBufferSeconds : jumpBufferLeft - dt; // Remember a press for a moment
            bool canGroundJump = grounded || (Time.time - lastGroundedTime <= coyoteSeconds && !jumpedFromGround); // On the ground or just off an edge
            if (jumpBufferLeft > 0f && canGroundJump) // A ground jump
            {
                velocity.y = jumpSpeed; // Jump up
                jumpedFromGround = true; // Used the ground jump
                grounded = false; // Leaving the ground
                jumpBufferLeft = 0f; // The press is used
            }
            else if (pressed && !grounded && !airJumpUsed) // A double jump in the air
            {
                velocity.y = doubleJumpSpeed; // A little lower than the first jump
                airJumpUsed = true; // Only one double jump per landing
                flipClock = 0f; // Start the somersault
                jumpBufferLeft = 0f; // The press is used
                Dust(); // Small dust puff under the feet
            }

            velocity.y = Mathf.Max(velocity.y - gravity * dt, -maxFallSpeed); // Gravity pulls down, up to the fastest fall
            MoveX(velocity.x * dt); // Move across, stopping at solids
            MoveY(velocity.y * dt); // Move up or down, landing on solids and ledges
            position.x = Mathf.Clamp(position.x, size.x / 2f, geometry.width - size.x / 2f); // Stay inside the level
            if (position.y < fallOutY) PlaceAt(RespawnPoint); // Fell out of the level: back to the respawn point
            Apply(); // Show the new place
            Animate(); // Pick the picture
        }

        void MoveX(float dx) // Moves across and stops at the side of a solid
        {
            if (dx == 0f) return; // Not moving across
            float next = position.x + dx; // Where the move would end
            var box = new Rect(next - size.x / 2f, position.y + 0.01f, size.x, size.y - 0.02f); // Body at the new place, a hair shorter so floors do not block
            foreach (var s in geometry.solids) // Check each solid
            {
                if (!box.Overlaps(s)) continue; // Not touching this one
                next = dx > 0f ? s.xMin - size.x / 2f : s.xMax + size.x / 2f; // Stop against its side
                box.x = next - size.x / 2f; // Move the box back too
            }
            position.x = next; // Final place across
        }

        void MoveY(float dy) // Moves up or down, landing on top of solids and ledges
        {
            float next = position.y + dy; // Where the move would end
            float left = position.x - size.x / 2f, right = position.x + size.x / 2f; // Body edges across
            bool landed = false; // Did the explorer land this frame?
            if (dy <= 0f) // Falling or standing
            {
                foreach (var s in geometry.solids) Land(s, ref next, ref landed, left, right); // Solid tops
                foreach (var l in geometry.ledges) Land(l, ref next, ref landed, left, right); // Ledge tops (only from above)
            }
            else // Going up
            {
                foreach (var s in geometry.solids) // Only solids stop a jump; ledges let the explorer through
                {
                    if (right <= s.xMin || left >= s.xMax) continue; // Not above or below this one
                    float head = position.y + size.y; // Top of the head now
                    if (head <= s.yMin + 0.01f && next + size.y > s.yMin) { next = s.yMin - size.y; velocity.y = 0f; } // Bump the head
                }
            }
            position.y = next; // Final height
            grounded = landed; // Standing only if something is under the feet
            if (landed) // On the ground
            {
                velocity.y = 0f; // Stop falling
                lastGroundedTime = Time.time; // Remember for the edge grace time
                jumpedFromGround = false; // Ground jump ready again
                airJumpUsed = false; // Double jump ready again
                flipClock = -1f; // A landing ends any somersault
            }
        }

        void Land(Rect r, ref float next, ref bool landed, float left, float right) // Lands on the top of r if the feet cross it
        {
            if (right <= r.xMin || left >= r.xMax) return; // Not above this one
            if (position.y >= r.yMax - 0.01f && next <= r.yMax) { next = r.yMax; landed = true; } // Feet were above the top and now reach it
        }

        void Apply() // Puts the object at the feet position, on whole pixels
        {
            transform.position = LevelGeometry.ToWorld(Mathf.Round(position.x), Mathf.Round(position.y)); // Whole pixels keep the art sharp
        }

        void Animate() // Picks the picture for what the explorer is doing
        {
            body.flipX = facing < 0; // The art faces right; flip it to face left
            if (!grounded && flipClock >= 0f && flipFrames != null && flipFrames.Length > 0) // Somersault after a double jump
            {
                flipClock += Time.deltaTime; // Somersault time goes on
                if (flipClock < flipSeconds) { body.sprite = flipFrames[(int)(flipClock / flipSeconds * flipFrames.Length)]; return; } // One full turn over flipSeconds
                flipClock = -1f; // Turn finished; the normal falling picture takes over
            }
            if (!grounded) { body.sprite = velocity.y > 0f ? riseFrame : fallFrame; return; } // In the air
            if (velocity.x == 0f) { body.sprite = standFrame; walkClock = 0f; return; } // Standing still
            walkClock += Time.deltaTime; // Walking time goes on
            body.sprite = walkFrames[(int)(walkClock * walkFramesPerSecond) % walkFrames.Length]; // The walk frame for now
        }

        void Dust() // Makes a small dust puff under the feet
        {
            if (dustFrames == null || dustFrames.Length == 0) return; // No dust pictures
            var go = new GameObject("Dust"); // New object for the puff
            go.transform.position = LevelGeometry.ToWorld(Mathf.Round(position.x), Mathf.Round(position.y) + 2f); // At the feet: the 5-pixel puff's middle sits 2 pixels up, so it rests on the feet line
            var sr = go.AddComponent<SpriteRenderer>(); // Draws the puff
            sr.sortingOrder = body.sortingOrder - 1; // Behind the explorer
            var flip = go.AddComponent<SpriteFlipbook>(); // Plays the puff frames
            flip.frames = dustFrames; // The dust pictures
            flip.framesPerSecond = 14f; // Quick puff
            flip.loop = false; // Play once
            flip.destroyWhenDone = true; // Remove it afterwards
        }
    }
}
