using UnityEngine; // Uses pixel rectangles
namespace LandGrand.Play // Level play parts
{ // Opens the play code
    public abstract class DynamicSolid : MonoBehaviour // A solid that can move or disappear
    { // Opens the solid
        public abstract Rect Bounds { get; } // Current pixel bounds
        public abstract bool IsSolid { get; } // Whether it blocks the explorer
        internal virtual bool HadTop => true; // Most solids already had a top on the previous frame
        public Rect PreviousBounds { get; protected set; } // Bounds before this frame
        public Vector2 Movement => Bounds.position - PreviousBounds.position; // Carries a standing explorer
    } // Ends the solid
} // Ends the play code
