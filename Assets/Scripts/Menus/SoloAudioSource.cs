using UnityEngine;
using System.Collections.Generic;

public class SoloAudioSource : MonoBehaviour
{
    public List<AudioSource> soloAudioSources;
    private List<AudioSource> mutedAudioSources;

    void Start()
    {
        if (soloAudioSources == null || soloAudioSources.Count == 0)
        {
            Debug.LogError("No solo audio sources assigned!");
            return;
        }

        mutedAudioSources = new List<AudioSource>();
        MuteAllExceptSolo();
    }

    void OnDestroy()
    {
        UnmuteAll();
    }

    void MuteAllExceptSolo()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            if (!soloAudioSources.Contains(audioSource))
            {
                audioSource.mute = true;
                mutedAudioSources.Add(audioSource);
            }
        }
    }

    void UnmuteAll()
    {
        foreach (AudioSource audioSource in mutedAudioSources)
        {
            audioSource.mute = false;
        }
    }
}
