using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ScrollTMP : MonoBehaviour, IScrollHandler
{
    [Tooltip("Reference to the TextMeshPro component")]
    public TMP_Text textComponent;

    [Tooltip("Reference to the ScrollRect component")]
    public ScrollRect scrollRect;

    private void Start()
    {
        // Check if ScrollRect and TMP_Text are assigned in the inspector
        if (textComponent == null || scrollRect == null)
        {
            Debug.LogError("ScrollRect or TMP_Text is not assigned in the inspector");
            return;
        }

        // Set the ScrollRect to only allow vertical scrolling
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
    }

    // This method is called when a scroll event is detected
    public void OnScroll(PointerEventData eventData)
    {
        // Calculate the amount of scrolling in the y-axis
        float delta = eventData.scrollDelta.y;

        // Adjust the vertical normalized position of the ScrollRect based on the scroll delta
        // Normalized position is a value between 0 and 1 representing the position of the viewport in the content
        scrollRect.verticalNormalizedPosition += delta * Time.deltaTime;
    }
}
