using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    public Button optionsButton;
    public GameObject optionsPanel;
    public Button startButton;
    public Button exitButton;
    public List<Graphic> fadeOutElements;
    public List<Graphic> fadeInElements;
    public float fadeDuration = 1f;

    void Start()
    {
        optionsButton.onClick.AddListener(ShowOptions);
    }

    void ShowOptions()
    {
        StartCoroutine(FadeOutElements());
    }

    IEnumerator FadeOutElements()
    {
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            foreach (var element in fadeOutElements)
            {
                element.color = new Color(element.color.r, element.color.g, element.color.b, 1 - t);
            }
            yield return null;
        }

        foreach (var element in fadeOutElements)
        {
            element.gameObject.SetActive(false);
        }

        StartCoroutine(FadeInElements());
    }

    IEnumerator FadeInElements()
    {
        optionsPanel.SetActive(true);

        foreach (var element in fadeInElements)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, 0);
        }

        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            foreach (var element in fadeInElements)
            {
                element.color = new Color(element.color.r, element.color.g, element.color.b, t);
            }
            yield return null;
        }
    }

    public void HideOptions()
    {
        StartCoroutine(FadeOutOptions());
    }

    IEnumerator FadeOutOptions()
    {
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            foreach (var element in fadeInElements)
            {
                element.color = new Color(element.color.r, element.color.g, element.color.b, 1 - t);
            }
            yield return null;
        }

        optionsPanel.SetActive(false);

        foreach (var element in fadeOutElements)
        {
            element.gameObject.SetActive(true);
        }

        StartCoroutine(FadeInMainMenu());
    }

    IEnumerator FadeInMainMenu()
    {
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            foreach (var element in fadeOutElements)
            {
                element.color = new Color(element.color.r, element.color.g, element.color.b, t);
            }
            yield return null;
        }
    }
}
