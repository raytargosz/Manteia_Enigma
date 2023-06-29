using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ScrollTMP : MonoBehaviour, IScrollHandler
{
    public TMP_Text textComponent;
    public ScrollRect scrollRect;

    private void Start()
    {
        // Ensure ScrollRect and TMP_Text are not null
        if (textComponent == null || scrollRect == null)
        {
            Debug.LogError("ScrollRect or TMP_Text is not assigned in the inspector");
            return;
        }

        // Enable vertical scrolling only
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
    }

    public void OnScroll(PointerEventData eventData)
    {
        // Get the scroll delta
        float delta = eventData.scrollDelta.y;

        // Change the normalized position of the ScrollRect based on the scroll delta
        scrollRect.verticalNormalizedPosition += delta * Time.deltaTime;
    }
}
