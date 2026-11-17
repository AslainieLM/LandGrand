using System; // Gives access to Enum
using System.Collections.Generic; // Gives IReadOnlyList
using System.Linq; // Gives Where, Select and Count

namespace LandGrand.Rules // Plain C# rules with no Unity types
{
    /// <summary>One item kind on a shelf, and whether the explorer has found it.</summary>
    public sealed class ShelfSlot // One slot of a shelf
    {
        public ItemKind Kind { get; } // The rock or mineral in this slot
        public bool IsFound { get; } // True once any pickup of it was collected

        public ShelfSlot(ItemKind kind, bool isFound) { Kind = kind; IsFound = isFound; } // Makes a slot
    }

    /// <summary>One row of the Collection: the item kinds on this landform's shelf (D-052, D-066).</summary>
    public sealed class Shelf // One landform's row
    {
        public Episode Landform { get; } // The landform of this shelf
        public IReadOnlyList<ShelfSlot> Slots { get; } // Its item kinds, in lesson order
        public bool IsComingSoon => Slots.Count == 0; // No item kinds yet: the shelf says "Coming soon"
        public int FoundCount => Slots.Count(slot => slot.IsFound); // How many kinds on this shelf are found, for "Found here 3/10"

        public Shelf(Episode landform, IReadOnlyList<ShelfSlot> slots) { Landform = landform; Slots = slots; } // Makes a shelf
        public int PageCount(int perPage) => perPage <= 0 ? 1 : Math.Max(1, (Slots.Count + perPage - 1) / perPage); // How many pages the shelf needs, rounded up, at least 1
        public IReadOnlyList<ShelfSlot> SlotsOnPage(int page, int perPage) // The kinds shown on one page; a page past the end shows the last page
        { // Opens the page slice
            if (perPage <= 0) return Slots; // No paging requested
            int last = PageCount(perPage) - 1; // Last valid page
            int p = page < 0 ? 0 : page > last ? last : page; // Keeps the page in range
            return Slots.Skip(p * perPage).Take(perPage).ToArray(); // Keeps lesson order
        } // Ends the page slice
    }

    /// <summary>
    /// The Collection shown in the Profile (D-052): every item kind, grouped by landform shelf (a shared kind is on more than one), marked found or not.
    /// The future Completion Screen (LG-15) uses the same numbers, so both screens always agree.
    /// </summary>
    public static class Collection // Reads the Collection from the player's progress
    {
        /// <summary>Number of item kinds in the game so far.</summary>
        public static int TotalCount => ItemCatalog.All.Count; // Grows when new kinds are added

        /// <summary>One shelf per landform, in episode order; each lists its item kinds in lesson order.</summary>
        public static IReadOnlyList<Shelf> Shelves(PlayerProgress progress) // Builds the shelves
        {
            return ((Episode[])Enum.GetValues(typeof(Episode))) // Every landform, in episode order
                .Select(landform => new Shelf(landform, ItemCatalog.All // For each landform, look through every kind
                    .Where(kind => kind.BelongsTo(landform)) // Only the kinds on this landform's shelf (a shared kind can be on two), keeping the catalog's lesson order
                    .Select(kind => new ShelfSlot(kind, progress.HasFoundKind(kind.Id))) // Mark each one found or not
                    .ToArray())) // Fix the slots
                .ToArray(); // Fix the shelves
        }

        /// <summary>Number of item kinds the explorer has found. Ids in the save that are not in the catalog are not counted.</summary>
        public static int FoundCount(PlayerProgress progress) => ItemCatalog.All.Count(kind => progress.HasFoundKind(kind.Id)); // Count the found kinds
    }
}
