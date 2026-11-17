using System; // Gives access to Serializable
using System.Linq; // Gives access to FirstOrDefault
using LandGrand.MainMenu; // Gives access to the name panel, shared with the Main Menu
using LandGrand.Rules; // Gives access to Collection, AchievementCatalog, ItemKind and LevelData
using TMPro; // Gives access to TMP_Text
using UnityEngine; // Gives access to GameObject, Sprite and Color
using UnityEngine.UI; // Gives access to Button and Image

namespace LandGrand.Profile // Profile scripts live here
{
    /// <summary>
    /// Runs the Profile screen (D-052, D-060): the explorer strip (name, pencil, totals), then two section tabs.
    /// Collection has one tab per landform and shows that landform's rocks and minerals in a grid; landforms not yet open
    /// on the map have a grey tab with a padlock. Achievements has the same landform tabs, and each landform has its own 5 by 3 badge page;
    /// locked badges are dark but keep their name, and a landform with no badges yet says "Coming soon".
    /// Tapping a found item or any badge opens a card. Back closes the card or the name panel first, then returns to the Main Menu.
    /// It only shows saved progress; the only thing it changes is the name, through the shared name panel.
    /// </summary>
    public class ProfileController : PixelScreen // The Profile brain
    {
        /// <summary>One cell of a grid: a frame with a picture and a name under it.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class SlotView // One slot
        {
            public GameObject root; // The whole slot; hidden when there is nothing to show in it
            public Button button; // Opens a card
            public Image picture; // The picture, or its dark shape
            public TMP_Text label; // The name, or "?"
        }

        /// <summary>A tab with its three pictures: chosen, not chosen, and locked.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class TabView // One tab
        {
            public Button button; // Taps
            public Image image; // Its picture
            public Sprite on; // Chosen
            public Sprite off; // Not chosen
            public Sprite locked; // Grey with a padlock (landform tabs only)
        }

        /// <summary>A picture with the name the catalogs use for it.</summary>
        [Serializable] // Lets Unity save this in the scene
        public class NamedSprite // One picture
        {
            public string name; // File name without ".png", for example "granite" or "badge_first_steps"
            public Sprite sprite; // The picture
        }

        [Header("Explorer strip")] // Inspector heading for the strip
        public TMP_Text nameLabel; // "Explorer: Ana"
        public RectTransform editName; // The pencil icon; moved to sit just after the name
        public Button editNameButton; // The pencil: opens the name panel
        public TMP_Text starsLabel; // Stars earned, for example "7"
        public TMP_Text foundLabel; // Item kinds found, for example "2/4"
        public TMP_Text levelsLabel; // Levels cleared, for example "1"

        [Header("Sections (D-060)")] // Inspector heading for the two big tabs
        public TabView collectionTab; // "Collection"
        public TabView achievementsTab; // "Achievements"
        public GameObject collectionPage; // Landform tabs and the item grid
        public GameObject achievementsPage; // The badge grid
        public TMP_Text badgeCount; // "Badges 3/15"

        [Header("Collection")] // Inspector heading for the item grid
        public TabView[] landformTabs; // Mountains, Hills, Plains, Plateaus, in episode order
        public UnityEngine.UI.Button previousPage, nextPage; // Collection page arrows
        public TMP_Text pageNumber; // Current page and total
        int page; // Zero-based page inside this landform
        public SlotView[] slots; // The item grid, filled from the top-left
        public TMP_Text comingSoon; // Shown when an open landform has no item kinds yet
        public NamedSprite[] pictures; // Every item picture, looked up by name
        public Color hiddenColor = new Color(0f, 0f, 0f, 0.8f); // Black times any colour is black, so the picture becomes one flat dark shape with no shading that hints at the item

        [Header("Achievements")] // Inspector heading for the badges
        public TabView[] badgeLandformTabs; // Mountains, Hills, Plains, Plateaus, in episode order; each landform has its own badge page (D-066)
        public SlotView[] badges; // One 5 by 3 page of cells; a landform's badges fill from the top-left
        public TMP_Text badgeComingSoon; // Shown when an open landform has no badges yet
        public NamedSprite[] badgePictures; // Every badge picture, earned and locked, by name

        [Header("Card")] // Inspector heading for the card
        public GameObject card; // The whole card with its dark backdrop
        public Image cardPicture; // Item or badge picture
        public TMP_Text cardName; // Name
        public TMP_Text cardTag; // "Rock", "Mineral", or the badge colour and state
        public TMP_Text cardDescription; // Description, or how to earn the badge
        public TMP_Text cardDraftNote; // "Draft — needs teacher review" until it is checked (Q-04)
        public Button cardOk; // Closes the card
        public Button cardOutside; // The dark backdrop: a tap outside the card closes it too

        [Header("Name")] // Inspector heading for the name panel
        public NamePanel namePanel; // The shared name panel, opened by the pencil
        public Button nameBack; // Closes the name panel without changes

        [Header("Scenes")] // Inspector heading for the scene names
        public string mainMenuScene = "MainMenu"; // Where Back goes

        const float PencilGap = 14f; // Pixels from the end of the name to the middle of the pencil
        Episode landform = Episode.Mountains; // Which landform's items are shown
        Episode badgeLandform = Episode.Mountains; // Which landform's badges are shown

        protected override void Awake() // Runs once when the Profile opens: fills it from the saved progress and connects the buttons
        {
            base.Awake(); // Click sound, hidden guide and the Back icon
            editNameButton.onClick.AddListener(OpenNamePanel); // The pencil opens the name panel
            nameBack.onClick.AddListener(CloseNamePanel); // Back on the name panel returns to the Profile
            namePanel.Accepted += NameSaved; // A good new name updates the strip
            cardOk.onClick.AddListener(CloseCard); // OK closes the card
            cardOutside.onClick.AddListener(CloseCard); // A tap outside the card closes it too
            collectionTab.button.onClick.AddListener(() => ShowSection(false)); // Collection tab
            achievementsTab.button.onClick.AddListener(() => ShowSection(true)); // Achievements tab
            for (int i = 0; i < landformTabs.Length; i++) { var e = (Episode)i; landformTabs[i].button.onClick.AddListener(() => ChooseLandform(e)); } // One listener per landform tab
            for (int i = 0; i < badgeLandformTabs.Length; i++) { var e = (Episode)i; badgeLandformTabs[i].button.onClick.AddListener(() => ChooseBadgeLandform(e)); } // One listener per badge landform tab
            previousPage.onClick.AddListener(() => ChangePage(-1)); // Previous twelve kinds
            nextPage.onClick.AddListener(() => ChangePage(1)); // Next twelve kinds
            card.SetActive(false); // No card at the start
            namePanel.gameObject.SetActive(false); // No name panel at the start
            ShowStrip(); // Name and totals
            ChooseBadgeLandform(Episode.Mountains); // The Mountains badge page and its count
            ShowSection(false); // Open on the Collection, Mountains first
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

        void ShowSection(bool achievements) // Opens the Collection or the Achievements section
        {
            if (Leaving) return; // Ignore taps while leaving
            collectionPage.SetActive(!achievements); // One page at a time
            achievementsPage.SetActive(achievements); // The other page
            collectionTab.image.sprite = achievements ? collectionTab.off : collectionTab.on; // The chosen tab looks joined to the page
            achievementsTab.image.sprite = achievements ? achievementsTab.on : achievementsTab.off; // The other tab looks behind
            if (!achievements) ChooseLandform(landform); // Draw the chosen landform's items
            else ChooseBadgeLandform(badgeLandform); // Draw the chosen landform's badges
        }

        void ChooseLandform(Episode e) // Shows one landform's rocks and minerals, or says to finish the earlier landform first
        {
            if (!CanShowLandform(e)) return; // Ignore taps while leaving, and stay on the shown landform when locked
            landform = e; // Remember it
            page = 0; // A landform choice starts on page one
            ShowLandformTabs(landformTabs, e); // Marks the chosen tab
            ShowCollectionPage(); // Fills the selected page
        } // Ends the tab change

        /// <summary>False while leaving or when the landform is locked (the lock guide is shown then). Shared by both landform tab rows.</summary>
        bool CanShowLandform(Episode e) // The lock guard for both tab rows
        { // Opens the block
            if (Leaving) return false; // Ignore taps while leaving
            if (!GameProgress.Current.IsEpisodeUnlocked(e)) { ShowGuide(RegionGuide.ForLocked(e)); return false; } // Locked like on the map (D-041); the same text as the map
            return true; // Open, so the tab can change
        } // Ends the block

        /// <summary>Sets each landform tab picture: locked, chosen or not chosen. Shared by both tab rows.</summary>
        static void ShowLandformTabs(TabView[] tabs, Episode chosen) // The tab pictures for one row
        { // Opens the block
            var progress = GameProgress.Current; // Saved progress
            for (int i = 0; i < tabs.Length; i++) // Each landform tab
            { // Opens the block
                var tab = tabs[i]; // The tab
                bool open = progress.IsEpisodeUnlocked((Episode)i); // Is that landform open on the map?
                tab.image.sprite = !open ? tab.locked : i == (int)chosen ? tab.on : tab.off; // Locked, chosen, or not chosen
            } // Ends the block
        } // Ends the block

        void ChangePage(int step) // Moves within the chosen landform
        { // Opens the page change
            if (Leaving) return; // Ignores taps while leaving
            int count = Collection.Shelves(GameProgress.Current)[(int)landform].PageCount(slots.Length); // Available pages
            page = Mathf.Clamp(page + step, 0, count - 1); // Stays within the shelf
            ShowCollectionPage(); // Redraws the page
        } // Ends the page change

        void ShowCollectionPage() // Fills twelve kinds and updates the arrows
        { // Opens the page display
            var shelf = Collection.Shelves(GameProgress.Current)[(int)landform]; // Chosen shelf
            int count = shelf.PageCount(slots.Length); // Number of pages
            var entries = shelf.SlotsOnPage(page, slots.Length); // Only this page's kinds
            previousPage.gameObject.SetActive(count > 1); // Hides paging for a short shelf
            nextPage.gameObject.SetActive(count > 1); // Hides paging for a short shelf
            pageNumber.gameObject.SetActive(count > 1); // Hides the single-page count
            previousPage.interactable = page > 0; // Stops at the first page
            nextPage.interactable = page + 1 < count; // Stops at the last page
            pageNumber.text = (page + 1) + "/" + count; // Shows one-based page numbers
            comingSoon.gameObject.SetActive(shelf.IsComingSoon); // "Coming soon" when it has none yet
            for (int i = 0; i < slots.Length; i++) // Each cell of the grid
            {
                var slot = slots[i]; // The cell
                bool used = i < entries.Count; // Is there a kind for it?
                slot.root.SetActive(used); // Hide spare cells
                slot.button.onClick.RemoveAllListeners(); // Forget what the cell showed before
                if (!used) continue; // Nothing more for a spare cell
                var entry = entries[i]; // The kind and whether it is found
                slot.picture.sprite = PictureOf(entry.Kind); // Its picture
                slot.picture.color = entry.IsFound ? Color.white : hiddenColor; // Colour when found, a dark shape when not
                slot.label.text = entry.IsFound ? entry.Kind.Name : "?"; // The name stays a surprise until it is found
                slot.button.interactable = entry.IsFound; // Only found kinds react to a tap
                if (entry.IsFound) { var kind = entry.Kind; slot.button.onClick.AddListener(() => OpenItemCard(kind)); } // Tapping it opens its card
            }
        }

        void ChooseBadgeLandform(Episode e) // Shows one landform's badges, or says to finish the earlier landform first
        {
            if (!CanShowLandform(e)) return; // Same lock guard as the Collection tabs; stay on the shown landform
            badgeLandform = e; // Remember it
            ShowLandformTabs(badgeLandformTabs, e); // Marks the chosen badge tab
            ShowBadges(); // Fills the chosen page
        }

        void ShowBadges() // Fills the badge page of the chosen landform and its count
        {
            var progress = GameProgress.Current; // Saved progress
            var all = AchievementCatalog.ForLandform(badgeLandform); // This landform's badges, in order
            badgeCount.text = Total("Badges", all.Count(a => a.IsEarned(progress)) + "/" + all.Count); // For example "Badges 3/15" on Mountains, "Badges 0/10" on Hills
            badgeComingSoon.gameObject.SetActive(all.Count == 0); // "Coming soon" for an open landform with no badges yet
            for (int i = 0; i < badges.Length; i++) // Each badge cell on the page
            {
                var cell = badges[i]; // The cell
                bool used = i < all.Count; // Is there a badge for it?
                cell.root.SetActive(used); // Hide spare cells
                cell.button.onClick.RemoveAllListeners(); // Forget any earlier listener
                if (!used) continue; // Nothing more for a spare cell
                var badge = all[i]; // The badge
                bool earned = badge.IsEarned(progress); // Earned or locked
                cell.picture.sprite = BadgeOf(badge, earned); // Medal, or its dark locked shape
                cell.label.text = badge.Name; // The name shows even while locked, so the player has a goal
                cell.label.color = earned ? Ink : LockedInk; // Locked names are softer
                cell.button.interactable = true; // Every badge opens its card, to show how to earn it
                cell.button.onClick.AddListener(() => OpenBadgeCard(badge)); // Tapping it opens its card
            }
        }

        static readonly Color Ink = new Color32(31, 26, 36, 255); // The game's ink colour
        static readonly Color LockedInk = new Color32(110, 100, 90, 255); // Soft grey-brown, as for "Coming soon"

        Sprite PictureOf(ItemKind kind) => pictures.FirstOrDefault(p => p.name == kind.Picture)?.sprite; // Looks an item picture up by name

        Sprite BadgeOf(Achievement badge, bool earned) => badgePictures.FirstOrDefault(p => p.name == "badge_" + badge.Id + (earned ? "" : "_locked"))?.sprite; // Looks a badge picture up by name

        void OpenItemCard(ItemKind kind) // Shows the card for a found rock or mineral
        {
            if (Leaving) return; // Ignore taps while leaving
            ShowCard(PictureOf(kind), kind.Name, kind.IsRock ? "Rock" : "Mineral", kind.Description, kind.IsDraft); // The same text as the pop-up in the level
        }

        void OpenBadgeCard(Achievement badge) // Shows the card for a badge: its colour, whether it is earned, and how to earn it
        {
            if (Leaving) return; // Ignore taps while leaving
            bool earned = badge.IsEarned(GameProgress.Current); // Earned or locked
            string state = badge.Tier + " badge" + (earned ? " — earned!" : " — locked"); // For example "Gold badge — locked"
            ShowCard(BadgeOf(badge, earned), badge.Name, state, badge.HowToEarn, badge.IsDraft); // Name, state and hint
        }

        void ShowCard(Sprite picture, string title, string tag, string text, bool draft) // Fills the card and shows it
        {
            cardPicture.sprite = picture; // The picture
            cardPicture.SetNativeSize(); // 24x24 for items, 32x32 for badges
            cardName.text = title; // The name
            cardTag.text = tag; // Rock, Mineral, or the badge state
            cardDescription.text = text; // Description or how to earn it
            cardDraftNote.text = draft ? LevelData.DraftNote : ""; // Draft marking until it is checked (D-043, Q-04)
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
