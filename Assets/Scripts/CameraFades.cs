using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraFades : MonoBehaviour
{
    public TMP_Text[] texts;
    public Button[] buttons;
    public Button specialButton;
    public Image transitionImage;
    public Camera mainCamera;
    public Camera secondaryCamera;
    public GameObject uiElement;
    public float fadeDuration = 1.0f;

    void Start()
    {
        // Ensure mainCamera is enabled and secondaryCamera is disabled at the start
        mainCamera.enabled = true;
        secondaryCamera.enabled = false;
    }

    public void OnButtonClick()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        // Fade out texts and buttons
        foreach (var text in texts)
        {
            StartCoroutine(FadeOutText(text, fadeDuration));
        }
        foreach (var button in buttons)
        {
            StartCoroutine(FadeOutButton(button, fadeDuration));
        }

        // Fade in image
        yield return StartCoroutine(FadeInImage(transitionImage, fadeDuration));

        // Disable texts, buttons and main camera
        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        mainCamera.enabled = false;

        // Enable secondary camera, UI element, and special button
        secondaryCamera.enabled = true;
        uiElement.SetActive(true);
        specialButton.gameObject.SetActive(true);

        // Fade out image
        yield return StartCoroutine(FadeOutImage(transitionImage, fadeDuration));
    }

    IEnumerator FadeOutText(TMP_Text text, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var color = text.color;
            color.a = Mathf.Lerp(1, 0, t / duration);
            text.color = color;
            yield return null;
        }
    }

    IEnumerator FadeOutButton(Button button, float duration)
    {
        var colors = button.colors;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var color = colors.normalColor;
            color.a = Mathf.Lerp(1, 0, t / duration);
            colors.normalColor = color;
            button.colors = colors;
            yield return null;
        }
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var color = image.color;
            color.a = Mathf.Lerp(0, 1, t / duration);
            image.color = color;
            yield return null;
        }
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var color = image.color;
            color.a = Mathf.Lerp(1, 0, t / duration);
            image.color = color;
            yield return null;
        }
    }
}