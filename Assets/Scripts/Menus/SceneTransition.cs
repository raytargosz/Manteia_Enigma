using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Button startButton;
    public string nextSceneName;
    public Image fadeOutImage;
    public float fadeDuration = 3f;
    public float waitAfterFade = 1f;

    private List<AudioSource> audioSources;
    private List<float> initialAudioVolumes;

    void Start()
    {
        audioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        initialAudioVolumes = new List<float>();

        // Store initial audio volumes
        foreach (AudioSource audioSource in audioSources)
        {
            initialAudioVolumes.Add(audioSource.volume);
        }

        startButton.onClick.AddListener(StartTransition);
    }

    void StartTransition()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        Color imageColor = fadeOutImage.color;
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            fadeOutImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, t);

            // Fade out audio sources
            for (int i = 0; i < audioSources.Count; i++)
            {
                AudioSource audioSource = audioSources[i];
                if (audioSource != null)
                {
                    // Calculate audio fade out value using initial volumes
                    float audioT = Mathf.Clamp01(1 - t);
                    audioSource.volume = initialAudioVolumes[i] * audioT;
                }
            }

            yield return null;
        }

        fadeOutImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1);
        yield return new WaitForSeconds(waitAfterFade);
        SceneManager.LoadScene(nextSceneName);
    }
}