using UnityEngine; // Gives the glint renderer and sprites

namespace LandGrand.Play // Holds the level object views
{ // Opens the play code
    public sealed class IceZoneView : MonoBehaviour // Holds the ice tiles and shows their glints
    { // Opens the view
        public SpriteRenderer[] glints; // Small sparks along the ice
        public Sprite[] frames; // The two glint pictures

        void Update() // Changes the glints on the shared level clock
        { // Starts the frame
            int frame = (int)(LevelClock.Now * 4f); // Changes each glint four times a second
            for (int i = 0; i < glints.Length; i++) glints[i].sprite = frames[(frame + i) % frames.Length]; // Glints take turns twinkling
        } // Ends the frame
    } // Ends the view
} // Ends the play code
