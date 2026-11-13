using UnityEngine; // Gives access to MonoBehaviour, Sprite and Time
using UnityEngine.EventSystems; // Gives access to the touch and click events
using UnityEngine.UI; // Gives access to Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// An on-screen touch button that can be held down, unlike Unity's Button, which only reacts on release.
    /// It works with several fingers at once, so the player can walk and jump together.
    /// </summary>
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler // Reacts to fingers and the mouse
    {
        public Image image; // The button picture
        public Sprite normal; // Picture while not touched
        public Sprite pressed; // Picture while held

        int fingers; // How many fingers press the button now
        bool pressWaiting; // A press that nobody has read yet

        /// <summary>True while at least one finger holds the button.</summary>
        public bool Held => fingers > 0; // Any finger counts

        /// <summary>True once for each new press, then false until the next press. Never misses a tap, whatever runs first in a frame.</summary>
        public bool TakePress() // Reads and clears the waiting press
        {
            bool press = pressWaiting; // Was there a press?
            pressWaiting = false; // It is used now
            return press; // Tell the reader
        }

        public void OnPointerDown(PointerEventData eventData) // A finger touched the button
        {
            if (fingers == 0) pressWaiting = true; // First finger: remember the press until it is read
            fingers++; // One more finger
            Show(); // Show the pressed picture
        }

        public void OnPointerUp(PointerEventData eventData) // A finger lifted (Unity sends this even if it slid off the button)
        {
            fingers = Mathf.Max(0, fingers - 1); // One finger fewer, never below zero
            Show(); // Show the right picture
        }

        void OnDisable() { fingers = 0; pressWaiting = false; Show(); } // A hidden button is never held or pressed

        void Show() { if (image != null) image.sprite = Held ? pressed : normal; } // Pressed or normal picture
    }
}
