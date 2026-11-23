using LandGrand.Rules; // Uses the ice and soft sand speed rules
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
        public float walkSpeed = 84f; // Walking speed, a little faster than the first 72
        public float gravity = 900f; // How fast falling speeds up
        public float maxFallSpeed = 420f; // Fastest fall
        public float jumpSpeed = 290f; // First jump: about 47 pixels high
        public float doubleJumpSpeed = 230f; // Double jump: a little lower, about 29 pixels
        public float coyoteSeconds = 0.08f; // A jump still works this long after walking off an edge
        public float jumpBufferSeconds = 0.1f; // A jump pressed this long before landing still happens
        public float walkFramesPerSecond = 12f; // Walk animation speed, raised with the walking speed so the feet keep pace
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

        public float ExternalPushX { get; set; } // Wind speed added to walking
        public float UpdraftTop { get; set; } = float.NaN; // The hover height of the updraft around the feet; NaN means no updraft (D-072)
        Updraft riseRule; // The updraft rule for the current hover height, kept so no new rule is made each frame
        internal float CurrentPushX { get; set; } // River speed added only while grounded
        internal bool BatPushActive => batPushLeft > 0f; // The bat part flashes only during a push
        float batPushLeft; // Time left for the current bat push
        int batPushDirection; // The push heads away from the bat
        internal bool OnIce { get; set; } // The ice watcher checks the ground below the feet
        internal bool OnSand { get; set; } // The sand watcher says the feet stand on soft sand, so walking is slower
        internal bool SlidingOnIce { get; private set; } // Lets the ice watcher play one sound at the start
        float walkVelocity; // Walking speed before the wind is added
        public DynamicSolid StandingOn { get; private set; } // Moving or breakable support
        float lastSupportTop; // The height the rider last stood on, even across a pause
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
            StandingOn = null; // A return leaves the old support
            position = feet; // New place
            velocity = Vector2.zero; // No speed
            walkVelocity = 0f; // Starts again without a slide
            batPushLeft = 0f; // Ends a push when the explorer is placed somewhere else
            SlidingOnIce = false; // Ends the old slide
            grounded = false; // Gravity will settle it on the ground
            airJumpUsed = false; // Both jumps ready again
            jumpedFromGround = false; // Both jumps ready again
            flipClock = -1f; // No somersault
            Apply(); // Show it there
        }

        /// <summary>Throws the explorer straight up, for a steam vent puff (D-055). The double jump is ready again in the air.</summary>
        public void Launch(float speed) // Called by the level controller when a vent puffs under the feet
        {
            StandingOn = null; // A launch leaves the support
            velocity.y = speed; // Fly up
            grounded = false; // Off the ground now
            jumpedFromGround = true; // No ground jump in the air
            airJumpUsed = false; // The double jump may still be used after the launch
            jumpBufferLeft = 0f; // Forget any waiting jump press
            flipClock = -1f; // No somersault from the launch itself
        }

        internal void PushFromBat(int direction) // Starts a gentle sideways push
        { // Opens the push
            batPushDirection = direction; // Saves the way away from the bat
            batPushLeft = Bat.PushSeconds; // Starts the short push without a respawn
        } // Ends the push

        void Update() // Runs every frame: reads the controls, moves and animates
        {
            if (Frozen) { velocity = Vector2.zero; Animate(); return; } // Hold position while frozen and keep the ice speed for Resume
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f); // Never step more than a 30th of a second, so nothing passes through ground

            Carry(); // Moves a rider along with a slab or basket
            int dir = input.Direction; // Walk left, right or not at all
            float batStep = Mathf.Min(dt, batPushLeft); // Use only the time left for the bat
            if (dir != 0 && batStep < dt) facing = dir; // A bat push ignores the held walk direction
            bool startedGrounded = grounded; // Keeps the old sand footing when this step begins with a jump

            bool inUpdraft = !float.IsNaN(UpdraftTop); // The watcher found an updraft around the feet
            bool pressed = input.JumpPressed && !inUpdraft; // Jump pressed this frame; a press inside an updraft does nothing
            jumpBufferLeft = inUpdraft ? 0f : pressed ? jumpBufferSeconds : jumpBufferLeft - dt; // Remember a press for a moment; an updraft forgets it
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

            float normalTime = dt - batStep; // Any time left after a bat push uses ordinary walking
            SlidingOnIce = OnIce && IceSlide.IsSliding(walkVelocity, dir); // Releasing or turning starts a slide as before
            float target = dir * SoftSand.WalkSpeed(walkSpeed, OnSand && startedGrounded); // Keeps the sand speed on its original footing
            walkVelocity = OnIce ? IceSlide.Next(walkVelocity, target, normalTime) : target; // Only ice changes speed gradually
            velocity.x = walkVelocity + ExternalPushX; // Wind still adds on the ground or in the air
            if (grounded) velocity.x += CurrentPushX; // The river stops pushing as soon as the explorer jumps
            if (batStep > 0f) // A gentle bat took some or all of this frame
            { // Opens the push movement
                float speed = Bat.PushSpeed * batPushDirection; // Matches the push distance and time
                MoveX(speed * batStep); // Moves the explorer without using the held arrow
                batPushLeft -= batStep; // Spends time only when the motor moves
                if (normalTime == 0f) velocity.x = speed; // Shows motion during the push
            } // Ends the push movement
            velocity.y = Mathf.Max(velocity.y - gravity * dt, -maxFallSpeed); // Gravity keeps the same jump and fall
            if (inUpdraft) Ride(dt); // Inside an updraft the wind replaces gravity
            MoveX(velocity.x * normalTime); // Walks only in the rest of the frame
            MoveY(velocity.y * dt); // Move up or down, landing on solids and ledges
            float inside = Mathf.Clamp(position.x, size.x / 2f, geometry.width - size.x / 2f); // Find the point inside the level
            if (inside != position.x) walkVelocity = 0f; // The level wall stops a slide
            position.x = inside; // Stay inside the level
            if (position.y < fallOutY) PlaceAt(RespawnPoint); // Fell out of the level: back to the respawn point
            Apply(); // Show the new place
            Animate(); // Pick the picture
        }

        void Ride(float dt) // Lifts the explorer in an updraft (D-072 section 5)
        { // Opens the ride
            if (riseRule == null || riseRule.Top != UpdraftTop) riseRule = new Updraft(0f, UpdraftTop); // Makes the rule only when the hover height changes
            grounded = false; // The wind holds the explorer off the ground
            StandingOn = null; // No support carries the explorer in the wind
            jumpedFromGround = true; // No ground jump in the air
            airJumpUsed = false; // The double jump is ready on leaving, as after a launch
            velocity.y = riseRule.RiseVelocity(position.y, dt); // Rises at most 90 px/s and stops at the hover height
        } // Ends the ride

        void Carry() // Follows the dynamic top the explorer stood on last frame
        { // Opens the carry check
            var support = StandingOn; // Remembers the former support even if MoveY changes it
            if (!grounded || support == null) return; // Nothing to carry away
            var before = support.PreviousBounds; // Uses the previous top because the object already moved
            float top = support.Bounds.yMax; // Uses the current support height
            bool onPreviousTop = position.x + size.x / 2f > before.xMin && position.x - size.x / 2f < before.xMax && Mathf.Abs(position.y - lastSupportTop) < 0.02f; // Checks where the explorer last stood
            if (!onPreviousTop || (!support.IsSolid && top >= lastSupportTop)) // A gone slab only carries through its last sinking step
            { // Opens the release
                grounded = false; // Lets the explorer fall
                return; // Stops following this slab
            } // Ends the release
            MoveX(support.Movement.x); // Keeps the old horizontal carry and wall check
            if (top == lastSupportTop) return; // Sideways slabs need no vertical work
            MoveY(top - position.y); // Follows vertically while checking fixed floors and ceilings
            if (!support.IsSolid || Mathf.Abs(position.y - top) >= 0.02f) return; // A submerged raft or ceiling ends the ride
            grounded = true; // A rising platform can still hold the explorer after the move
            StandingOn = support; // Remembers the basket or raft for the next step
            lastSupportTop = top; // Saves the height the explorer was actually carried to
        } // Ends the carry check

        void MoveX(float dx) // Moves across and stops at the side of a solid
        {
            if (dx == 0f) return; // Not moving across
            float next = position.x + dx; // Where the move would end
            var box = new Rect(next - size.x / 2f, position.y + 0.01f, size.x, size.y - 0.02f); // Body at the new place, a hair shorter so floors do not block
            foreach (var s in geometry.solids) // Check each solid; slabs act like ledges and never block the sides
            {
                if (!box.Overlaps(s)) continue; // Not touching this one
                next = dx > 0f ? s.xMin - size.x / 2f : s.xMax + size.x / 2f; // Stop against its side
                box.x = next - size.x / 2f; // Move the box back too
                walkVelocity = 0f; // The solid wall stops walking or sliding
            }
            position.x = next; // Final place across
        }

        void MoveY(float dy) // Moves up or down, landing on top of solids and ledges
        {
            float next = position.y + dy; // Where the move would end
            float left = position.x - size.x / 2f, right = position.x + size.x / 2f; // Body edges across
            bool landed = false; // Did the explorer land this frame?
            DynamicSolid landedOn = null; // Records the actual moving top that caught the feet
            if (dy <= 0f) // Falling or standing
            {
                foreach (var s in geometry.solids) landed |= TryLand(s, s.yMax, ref next, left, right); // Fixed solid tops
                foreach (var ledge in geometry.ledges) landed |= TryLand(ledge, ledge.yMax, ref next, left, right); // Fixed one-way tops
            } // Ends the landing search
            else // Going up
            { // Opens the head check
                float ceiling = StopAtCeiling(next, left, right); // Only fixed solids can stop a jump from below
                if (ceiling < next) velocity.y = 0f; // Stops the jump on a head bump
                next = ceiling; // Uses the height still clear of the ceiling
            } // Ends the head check
            foreach (var slab in geometry.dynamicSolids) // Sweeps any moving one-way top against the feet
            { // Opens one slab check
                if (slab == null || !slab.IsSolid) continue; // Gone slabs cannot catch a landing
                var bounds = slab.Bounds; // The top now
                float oldTop = slab == StandingOn && lastSupportTop == bounds.yMax ? bounds.yMax : slab.PreviousBounds.yMax; // A carried rider already reached the new top
                if (dy > 0f && bounds.yMax <= oldTop) continue; // A fixed or falling slab never blocks a jump from below
                if (!TryLand(bounds, oldTop, ref next, left, right, slab.HadTop)) continue; // Tests the slab's motion only if it had a top
                landed = true; // A real slab caught the feet
                if (landedOn == null || bounds.yMax > landedOn.Bounds.yMax) landedOn = slab; // Keeps the first support at the highest top
            } // Ends one slab check
            position.y = next; // Final height
            StandingOn = landedOn; // Only a top that caught the feet can carry the explorer
            if (StandingOn != null) lastSupportTop = next; // Carries from the height this rider reached
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

        float StopAtCeiling(float next, float left, float right) // Keeps an upward move below fixed solids
        { // Opens the head check
            foreach (var solid in geometry.solids) // A one-way top never blocks the head
            { // Opens one solid check
                if (right <= solid.xMin || left >= solid.xMax) continue; // Only checks things above the body
                float head = position.y + size.y; // Top of the head before the move
                if (head <= solid.yMin + 0.01f && next + size.y > solid.yMin) next = solid.yMin - size.y; // Moves up only as far as this solid allows
            } // Ends one solid check
            return next; // Gives back the height that is clear
        } // Ends the head check

        bool TryLand(Rect r, float previousTop, ref float next, float left, float right, bool hadTop = true) // Checks whether the feet crossed a top
        { // Opens the landing check
            if (right <= r.xMin || left >= r.xMax) return false; // Not above this one
            if (!OneWayTop.Crosses(position.y, next, previousTop, r.yMax, hadTop)) return false; // Feet must meet the top from above
            if (r.yMax > previousTop && r.yMax > position.y && StopAtCeiling(r.yMax, left, right) < r.yMax) return false; // A rising top cannot force the head through a ceiling
            next = r.yMax; // Lands at the current height of the top
            return true; // The top now supports the explorer
        } // Ends the landing check

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
