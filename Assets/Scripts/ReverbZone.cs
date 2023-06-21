using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ReverbZone : MonoBehaviour
{
    [System.Serializable]
    public class AudioSourceSettings
    {
        [Header("General Settings")]
        [Tooltip("A descriptive name for this AudioSource.")]
        public string label;

        [Tooltip("The AudioSource to which these settings should be applied.")]
        public AudioSource audioSource;

        [Header("Audio Settings")]
        [Tooltip("The AudioClip to be played by the AudioSource.")]
        public AudioClip audioClip;

        [Tooltip("Whether or not the AudioSource should loop.")]
        public bool loop;

        [Tooltip("Whether or not the AudioSource should be muted.")]
        public bool mute;

        [Header("Effects Settings")]
        [Tooltip("Intensity of the reverb effect. Larger values mean more reverb.")]
        public float reverbIntensity;

        [Tooltip("The volume of the AudioSource.")]
        public float volume;

        [Header("Advanced Audio Settings")]
        [Tooltip("Changes the pitch of the AudioSource. Values less than 1 lower the pitch, and values greater than 1 raise it.")]
        public float pitch = 1.0f;

        [Tooltip("Controls how much of the AudioSource's signal is treated as 3D positional audio. 0 means all audio is played in 2D; 1 means all audio is played in 3D.")]
        public float spatialBlend;

        [Header("Reverb Settings")]
        [Tooltip("The decay time of the reverb effect.")]
        public float reverbDecayTime = 1.0f;

        [Tooltip("The room effect level.")]
        public float room = 0.0f;

        [Tooltip("The room effect high-frequency level.")]
        public float roomHF = 0.0f;

    }

    public AudioSourceSettings[] audioSourceSettings; // Array of audio sources and their settings

    private void Start()
    {
        if (audioSourceSettings.Length == 0)
        {
            // Try to automatically fetch AudioSource components on this GameObject and its children
            AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
            if (audioSources.Length > 0)
            {
                audioSourceSettings = new AudioSourceSettings[audioSources.Length];
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSourceSettings[i] = new AudioSourceSettings
                    {
                        audioSource = audioSources[i],
                        reverbIntensity = 0.5f, // Default reverbIntensity
                        volume = audioSources[i].volume // Default volume
                    };
                }
            }
            else
            {
                Debug.LogError("No Audio Sources added to ReverbZone script");
                return;
            }
        }

        foreach (var settings in audioSourceSettings)
        {
            if (settings.audioSource.GetComponent<AudioReverbFilter>() == null)
            {
                settings.audioSource.gameObject.AddComponent<AudioReverbFilter>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") // If the player enters the zone
        {
            foreach (var settings in audioSourceSettings)
            {
                StartCoroutine(ChangeReverb(settings, settings.reverbIntensity, 1.0f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") // If the player leaves the zone
        {
            foreach (var settings in audioSourceSettings)
            {
                StartCoroutine(ChangeReverb(settings, 0f, 1.0f));
            }
        }
    }

    private IEnumerator ChangeReverb(AudioSourceSettings settings, float targetReverb, float duration)
    {
        float startTime = Time.time;

        AudioReverbFilter filter = settings.audioSource.GetComponent<AudioReverbFilter>();

        // Set the reverb settings
        filter.decayTime = settings.reverbDecayTime;
        filter.room = settings.room;
        filter.roomHF = settings.roomHF;

        if (filter == null)
        {
            Debug.LogError("No AudioReverbFilter attached to the AudioSource: " + settings.audioSource.name);
            yield break;
        }

        float startReverb = filter.reverbLevel;
        float startVolume = settings.audioSource.volume;

        // Apply the new settings to the AudioSource
        settings.audioSource.clip = settings.audioClip;
        settings.audioSource.pitch = settings.pitch;
        settings.audioSource.spatialBlend = settings.spatialBlend;
        settings.audioSource.loop = settings.loop;
        settings.audioSource.mute = settings.mute;

        while (Time.time < startTime + duration)
        {
            filter.reverbLevel = Mathf.Lerp(startReverb, targetReverb, (Time.time - startTime) / duration);
            settings.audioSource.volume = Mathf.Lerp(startVolume, settings.volume, (Time.time - startTime) / duration);
            yield return null;
        }

        filter.reverbLevel = targetReverb;
        settings.audioSource.volume = settings.volume;
    }
}
