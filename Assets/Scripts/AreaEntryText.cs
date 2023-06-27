using UnityEngine;
using TMPro;
using System.Collections;

public class AreaEntryText : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Text element to display when the player enters the area")]
    public TextMeshProUGUI areaEntryText;

    [Header("Player Settings")]
    [Tooltip("The player's GameObject")]
    public GameObject player;

    [Header("Fade Settings")]
    [Tooltip("Time it takes for the text to fade in/out")]
    public float fadeDuration = 1.5f;

    [Tooltip("Whether to fade in the text when the player enters the area or start at full alpha")]
    public bool fadeIn = true;

    private bool hasEntered = false;

    private void Start()
    {
        if (!fadeIn)
        {
            areaEntryText.color = new Color(areaEntryText.color.r, areaEntryText.color.g, areaEntryText.color.b, 1f);
        }
        else
        {
            areaEntryText.color = new Color(areaEntryText.color.r, areaEntryText.color.g, areaEntryText.color.b, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.gameObject == player && !hasEntered)
        {
            hasEntered = true;
            if (fadeIn)
            {
                StartCoroutine(FadeTextToFullAlpha(fadeDuration, areaEntryText));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has exited the trigger zone
        if (other.gameObject == player)
        {
            StartCoroutine(FadeTextToZeroAlpha(fadeDuration, areaEntryText));
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI text)
    {
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI text)
    {
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}