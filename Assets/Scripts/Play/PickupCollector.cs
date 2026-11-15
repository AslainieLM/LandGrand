using LandGrand.Rules; // Gives access to ItemKind and CollectResult
using LandGrand.StartScreen; // Gives access to SpriteFlipbook for the sparkle
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to MonoBehaviour, Sprite and Rect
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Play // Level play scripts live here
{
    /// <summary>
    /// The rocks and minerals in a level: touching one collects it. The first find of a kind opens
    /// the "You found an item!" pop-up, which pauses the game; later finds only chime and sparkle.
    /// Also shows the level name and the "2/5" count at the top left (D-049).
    /// </summary>
    public class PickupCollector : MonoBehaviour // One part of the level screen
    {
        public PickupSpot[] pickups; // The collectibles in the level, in the order of LevelData
        public TMP_Text counterLabel; // The "2/5" count, top left
        public TMP_Text levelNameLabel; // "Mountains Easy 1", above the count
        public GameObject findPanel; // The "You found an item!" pop-up with its dark backdrop
        public Image findPicture; // The item picture in the pop-up
        public TMP_Text findName; // The item name in the pop-up
        public TMP_Text findDescription; // The short description in the pop-up
        public Button findOk; // Closes the pop-up
        public Sprite[] itemPictures; // Item pictures; the one named like the kind's Picture is used
        public Sprite[] sparkleFrames; // Sparkle shown where an item was taken

        LevelController level; // The level these pickups belong to

        internal bool IsBuilt => pickups != null && counterLabel != null && findPanel != null && findOk != null; // False for a scene built before this part existed

        internal void Begin(LevelController owner) // Hides the pop-up and shows the start count
        {
            level = owner; // Remember the level
            findPanel.SetActive(false); // No pop-up yet
            findOk.onClick.AddListener(CloseFind); // OK closes the pop-up
            ShowCount(); // Show 0/5
            if (levelNameLabel != null) levelNameLabel.text = level.episode + " " + level.difficulty + " " + level.number; // For example "Mountains Easy 1"
        }

        internal Sprite PictureOf(ItemKind kind) => System.Array.Find(itemPictures, s => s != null && s.name == kind.Picture); // The picture of a kind

        internal void Check() // Looks for a pickup under the explorer
        {
            var run = level.Run; // The attempt
            var body = level.ExplorerBodyAt(level.explorer.Position); // The explorer body box
            foreach (var spot in pickups) // Each pickup in the level
            {
                if (run.IsCollected(spot.index) || !body.Overlaps(spot.Area)) continue; // Taken already, or not touched
                Take(spot); // Collect it
                if (run.IsPaused) return; // A pop-up opened: stop for now
            }
        }

        internal void HidePopup() => findPanel.SetActive(false); // Time ran out: the pop-up cannot stay open

        void Take(PickupSpot spot) // Collects one pickup
        {
            var run = level.Run; // The attempt
            var result = run.Collect(spot.index, GameProgress.Current); // The rules decide: first find, known kind or nothing
            if (result == CollectResult.None) return; // Nothing happened
            spot.Hide(); // The item leaves the level
            Sparkle(spot.Center); // Sparkle where it was
            if (level.chimes != null && level.chimes.Length > 0) level.PlaySfx(level.chimes[Random.Range(0, level.chimes.Length)]); // One of the pickup chimes, at the player sound volume
            ShowCount(); // Update the counter
            if (result != CollectResult.FirstFind) return; // A known kind: no pop-up
            GameProgress.Save(); // Keep the found kind on the device now
            OpenFind(run.PendingFind); // Show the pop-up
        }

        void ShowCount() => counterLabel.text = level.Run.Collected + "/" + level.Run.Level.Pickups.Count; // For example 2/5

        void OpenFind(ItemKind kind) // Opens the pop-up and pauses the game
        {
            findName.text = kind.Name; // Item name
            findDescription.text = kind.Description; // Short description
            findPicture.sprite = PictureOf(kind); // Its picture
            findPanel.SetActive(true); // Show the pop-up; its backdrop blocks taps on the controls
            level.explorer.Frozen = true; // The explorer stops until OK
        }

        void CloseFind() // OK was pressed
        {
            level.Run.DismissFind(); // The game goes on
            findPanel.SetActive(false); // Hide the pop-up
            level.explorer.Frozen = false; // The explorer can move again
        }

        void Sparkle(Vector2 where) // A small sparkle at a level place
        {
            if (sparkleFrames == null || sparkleFrames.Length == 0) return; // No sparkle pictures
            var go = new GameObject("Sparkle"); // New object for the sparkle
            go.transform.position = LevelGeometry.ToWorld(Mathf.Round(where.x), Mathf.Round(where.y)); // On whole pixels
            var sr = go.AddComponent<SpriteRenderer>(); // Draws it
            sr.sortingOrder = 25; // In front of the explorer
            var flip = go.AddComponent<SpriteFlipbook>(); // Plays the frames
            flip.frames = sparkleFrames; // The sparkle pictures
            flip.framesPerSecond = 10f; // A short twinkle
            flip.loop = false; // Play once
            flip.destroyWhenDone = true; // Remove it afterwards
        }
    }
}
