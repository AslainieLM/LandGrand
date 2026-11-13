using UnityEngine; // Gives access to Vector3 and Mathf

namespace LandGrand.LandformMap // Landform Map scripts live here
{
    /// <summary>Turns a pixel on the 480x270 map (y down from the top) into a world position, snapped to whole pixels.</summary>
    public static class MapPixels // Shared pixel helper for the map animations
    {
        const float PPU = 32f; // Sprite pixels per Unity unit, the same as every LandGrand screen

        /// <summary>World position of the map pixel (x, y). Rounding keeps sprites on whole pixels so they stay sharp.</summary>
        public static Vector3 ToWorld(float x, float y) => new Vector3((Mathf.Round(x) - 240f) / PPU, (135f - Mathf.Round(y)) / PPU, 0f); // Centre of the screen is world (0, 0)
    }
}
