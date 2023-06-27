using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AudioFadeInManager : MonoBehaviour
{
    public float fadeInDuration = 1f;
    public AudioSource[] audioSources;

    private float[] targetVolumes;

    private void Start()
    {
        targetVolumes = new float[audioSources.Length];

        for (int i = 0; i < audioSources.Length; i++)
        {
            targetVolumes[i] = audioSources[i].volume;
            audioSources[i].volume = 0;
        }

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float startTime = Time.time;

        while (Time.time - startTime < fadeInDuration)
        {
            float t = (Time.time - startTime) / fadeInDuration;

            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].volume = Mathf.Lerp(0, targetVolumes[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = targetVolumes[i];
        }
    }
}
