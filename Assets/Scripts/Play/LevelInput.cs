using UnityEngine; // Gives access to MonoBehaviour
using UnityEngine.InputSystem; // Gives access to the keyboard

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// Reads the player's controls: the on-screen left, right and jump buttons,
    /// and in the Editor also the keyboard (arrows or A and D to walk; Space, Up or W to jump).
    /// </summary>
    public class LevelInput : MonoBehaviour // Joins touch and keyboard into one set of controls
    {
        public HoldButton left; // Bottom-left arrow pointing left
        public HoldButton right; // Bottom-left arrow pointing right
        public HoldButton jump; // Bottom-right jump button

        /// <summary>-1 to walk left, 1 to walk right, 0 to stand still.</summary>
        public int Direction // Walking direction from the held controls
        {
            get
            {
                var k = Keyboard.current; // The keyboard, if there is one
                bool l = left.Held || (k != null && (k.leftArrowKey.isPressed || k.aKey.isPressed)); // Left held
                bool r = right.Held || (k != null && (k.rightArrowKey.isPressed || k.dKey.isPressed)); // Right held
                return (r ? 1 : 0) - (l ? 1 : 0); // Both held cancel out
            }
        }

        /// <summary>True once for each new jump press. Read it once per frame: reading uses up a touch press.</summary>
        public bool JumpPressed // A new jump press
        {
            get
            {
                var k = Keyboard.current; // The keyboard, if there is one
                bool key = k != null && (k.spaceKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame); // Keyboard jump
                return jump.TakePress() | key; // Either one counts; "|" always reads (and clears) the touch press
            }
        }
    }
}
