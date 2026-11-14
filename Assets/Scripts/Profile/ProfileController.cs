using System; // Gives access to Serializable
using System.Linq; // Gives access to FirstOrDefault
using LandGrand.MainMenu; // Gives access to the name panel, shared with the Main Menu
using LandGrand.Rules; // Gives access to Collection, ItemKind and LevelData
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Sprite and Color
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Profile // Profile scripts live here
{
    /// <summary>
    /// Runs the Profile screen (decision D-052): the explorer strip (name, pencil, totals) and the Collection,
    /// one shelf per landform. Found rocks and minerals show in colour with their name and open an item card;
    /// the others show as a dark shape with "?". Back closes the card or the name panel first, then returns to the Main Menu.
    /// It only shows saved progress; the only thing it changes is the name, through the shared name panel.
    /// </summary>
    public class ProfileController : PixelScreen // The Profile brain
    {
        /// <summary>One slot on a shelf: a frame with a picture and a name under it.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class SlotView // One slot
        {
            public GameObject root; // The whole slot; hidden when the shelf has fewer kinds
            public Button button; // Opens the item card when the kind is found
            public Image picture; // The rock or mineral picture, or its dark shape
            public TMP_Text label; // The name, or "?"
        }

        /// <summary>One shelf: its slots and a "Coming soon" line.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class ShelfView // One shelf
        {
            public TMP_Text comingSoon; // "Coming soon", shown when the landform has no item kinds yet
            public SlotView[] slots; // Up to six slots, filled from the left
        }

        /// <summary>A picture with the name the item catalog uses for it.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class NamedSprite // One picture
        {
            public string name; // File name without ".png", for example "granite"
            public Sprite sprite; // The picture
        }

        [Header("Explorer strip")] // Inspector heading for the strip
        public TMP_Text nameLabel; // "Explorer: Ana"
        public RectTransform editName; // The pencil icon; moved to sit just after the name
        public Button editNameButton; // The pencil: opens the name panel
        public TMP_Text starsLabel; // Stars earned, for example "7"
        public TMP_Text foundLabel; // Item kinds found, for example "2/4"
        public TMP_Text levelsLabel; // Levels cleared, for example "1"

        [Header("Collection")] // Inspector heading for the shelves
        public ShelfView[] shelves; // Four shelves, in episode order
        public NamedSprite[] pictures; // Every item picture, looked up by name
        public Color hiddenColor = new Color(0f, 0f, 0f, 0.8f); // Black times any colour is black, so the picture becomes one flat dark shape with no shading that hints at the item

        [Header("Item card")] // Inspector heading for the card
        public GameObject card; // The whole card with its dark backdrop
        public Image cardPicture; // Item picture
        public TMP_Text cardName; // Item name
        public TMP_Text cardTag; // "Rock" or "Mineral"
        public TMP_Text cardDescription; // Short description
        public TMP_Text cardDraftNote; // "Draft — needs teacher review" until a teacher checks it (Q-04)
        public Button cardOk; // Closes the card
        public Button cardOutside; // The dark backdrop: a tap outside the card closes it too

        [Header("Name")] // Inspector heading for the name panel
        public NamePanel namePanel; // The shared name panel, opened by the pencil
        public Button nameBack; // Closes the name panel without changes

        [Header("Scenes")] // Inspector heading for the scene names
        public string mainMenuScene = "MainMenu"; // Where Back goes

        const float PencilGap = 14f; // Pixels from the end of the name to the middle of the pencil

        protected override void Awake() // Runs once when the Profile opens: fills it from the saved progress and connects the buttons
        {
            base.Awake(); // Click sound, hidden guide and the Back icon
            editNameButton.onClick.AddListener(OpenNamePanel); // The pencil opens the name panel
            nameBack.onClick.AddListener(CloseNamePanel); // Back on the name panel returns to the Profile
            namePanel.Accepted += NameSaved; // A good new name updates the strip
            cardOk.onClick.AddListener(CloseCard); // OK closes the card
            cardOutside.onClick.AddListener(CloseCard); // A tap outside the card closes it too
            card.SetActive(false); // No card at the start
            namePanel.gameObject.SetActive(false); // No name panel at the start
            ShowStrip(); // Name and totals
            ShowShelves(); // The Collection, drawn once each time the Profile opens, so new finds show at once
        }

        void ShowStrip() // Fills in the name and the three totals
        {
            var progress = GameProgress.Current; // The saved progress, loaded at launch
            var explorer = progress.Name; // The saved name, empty before the first Play
            nameLabel.text = "Explorer: " + (string.IsNullOrEmpty(explorer) ? "—" : explorer); // A dash when there is no name yet
            float width = nameLabel.GetPreferredValues(nameLabel.text).x; // How wide the name is drawn
            var box = nameLabel.rectTransform; // The name's box; its pivot is in the middle
            float left = box.anchoredPosition.x - box.sizeDelta.x / 2f; // Left edge of the box (the name is left-aligned)
            editName.anchoredPosition = new Vector2(Mathf.Round(left + width + PencilGap), editName.anchoredPosition.y); // Pencil just after the name, on a whole pixel
            starsLabel.text = Total("Stars", progress.TotalStars.ToString()); // Best stars over every level
            foundLabel.text = Total("Found", Collection.FoundCount(progress) + "/" + Collection.TotalCount); // Found kinds out of all kinds
            levelsLabel.text = Total("Levels cleared", progress.ClearedLevelCount.ToString()); // Levels cleared; no total, since Q-03 is open on the level count
        }

        static string Total(string word, string value) => "<color=#9A5532>" + word + "</color> " + value; // A clay word, one space, then its number in ink

        void ShowShelves() // Fills every shelf from the Collection
        {
            var model = Collection.Shelves(GameProgress.Current); // One shelf per landform, in episode order
            for (int s = 0; s < shelves.Length && s < model.Count; s++) // Each shelf on screen
            {
                var view = shelves[s]; // The shelf on screen
                var data = model[s]; // Its data
                view.comingSoon.gameObject.SetActive(data.IsComingSoon); // "Coming soon" only for landforms without kinds
                if (data.Slots.Count > view.slots.Length) Debug.LogWarning($"{data.Landform} has {data.Slots.Count} item kinds but its shelf has {view.slots.Length} slots; add slots in ProfileBuilder."); // Never drop kinds silently
                for (int i = 0; i < view.slots.Length; i++) // Each slot on the shelf
                {
                    var slot = view.slots[i]; // The slot on screen
                    bool used = i < data.Slots.Count; // Is there a kind for this slot?
                    slot.root.SetActive(used); // Hide spare slots
                    if (!used) continue; // Nothing more for a spare slot
                    var entry = data.Slots[i]; // The kind and whether it is found
                    slot.picture.sprite = PictureOf(entry.Kind); // Its picture
                    slot.picture.color = entry.IsFound ? Color.white : hiddenColor; // Colour when found, a dark shape when not
                    slot.label.text = entry.IsFound ? entry.Kind.Name : "?"; // The name stays a surprise until it is found
                    slot.button.interactable = entry.IsFound; // Only found kinds react to a tap
                    if (entry.IsFound) { var kind = entry.Kind; slot.button.onClick.AddListener(() => OpenCard(kind)); } // Tapping it opens its card
                }
            }
        }

        Sprite PictureOf(ItemKind kind) => pictures.FirstOrDefault(p => p.name == kind.Picture)?.sprite; // Looks the picture up by name

        void OpenCard(ItemKind kind) // Shows the item card for a found kind
        {
            if (Leaving) return; // Ignore taps while leaving
            cardPicture.sprite = PictureOf(kind); // Its picture
            cardName.text = kind.Name; // Its name
            cardTag.text = kind.IsRock ? "Rock" : "Mineral"; // Rock or mineral
            cardDescription.text = kind.Description; // The same text as the pop-up in the level
            cardDraftNote.text = kind.IsDraft ? LevelData.DraftNote : ""; // Draft marking until a teacher checks it (D-043, Q-04)
            card.SetActive(true); // Show the card
        }

        void CloseCard() => card.SetActive(false); // Hides the card

        void OpenNamePanel() // Shows the name panel with the saved name typed in
        {
            if (Leaving) return; // Ignore taps while leaving
            namePanel.gameObject.SetActive(true); // Show it; it fills in the saved name and opens the keyboard
        }

        void CloseNamePanel() => namePanel.gameObject.SetActive(false); // Hides the name panel without changes

        void NameSaved() // The name panel saved a good name
        {
            CloseNamePanel(); // Back to the Profile
            ShowStrip(); // Show the new name
        }

        protected override void Back() // What Back does here: close the top-most thing first
        {
            if (card.activeSelf) { CloseCard(); return; } // A card is open: close it
            if (namePanel.gameObject.activeSelf) { CloseNamePanel(); return; } // The name panel is open: close it
            Leave(mainMenuScene); // Otherwise go back to the Main Menu
        }
    }
}
