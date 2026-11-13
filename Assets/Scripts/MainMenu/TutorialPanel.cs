using UnityEngine; // Gives access to MonoBehaviour and Sprite
using UnityEngine.UI; // Gives access to Image and Button

namespace LandGrand.MainMenu
{
    /// <summary>
    /// The Tutorial panel: a few picture pages the player flips through with Prev and Next.
    /// Each page has a picture and a text image. The text is draft wording until the game rules
    /// (open questions Q-02 and Q-06 in the project vault) are decided.
    /// </summary>
    public class TutorialPanel : MonoBehaviour
    {
        public Image picture; // Where the page picture is shown
        public Image text; // Where the page text is shown
        public Sprite[] pictures; // One picture per page, in order
        public Sprite[] texts; // One text image per page, in order
        public Button prevButton; // Goes back one page
        public Button nextButton; // Goes forward one page

        int page; // The page shown now, starting at 0

        void Awake() // Runs once, the first time the panel opens: connects Prev and Next
        {
            prevButton.onClick.AddListener(() => Show(page - 1)); // Prev shows the page before
            nextButton.onClick.AddListener(() => Show(page + 1)); // Next shows the page after
        }

        void OnEnable() // Runs each time the panel opens
        {
            Show(0); // Always start at the first page when the panel opens
        }

        void Show(int index) // Shows the page with this number
        {
            int count = Mathf.Min(pictures.Length, texts.Length); // Number of complete pages
            if (count == 0) return; // Nothing to show if no pages were set up
            page = Mathf.Clamp(index, 0, count - 1); // Stay between the first and last page
            picture.sprite = pictures[page]; // Put this page's picture in place
            picture.rectTransform.sizeDelta = pictures[page].rect.size; // One sprite pixel = one UI pixel
            text.sprite = texts[page]; // Put this page's text in place
            text.rectTransform.sizeDelta = texts[page].rect.size; // Text images differ in width, so resize each time
            prevButton.gameObject.SetActive(page > 0); // Hide Prev on the first page
            nextButton.gameObject.SetActive(page < count - 1); // Hide Next on the last page
        }
    }
}
