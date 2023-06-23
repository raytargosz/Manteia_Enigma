using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public SurfaceAudio[] surfaceAudio; // An array of all surface audio data
    public Transform playerTransform; // Player transform to track

    private Dictionary<string, SurfaceAudio> audioDictionary; // A dictionary for quick look up of surface audio data
    private AudioSource audioSource; // The AudioSource component
    private float lastSoundTime; // The time when the last sound was played

    private void Awake()
    {
        // Initialize the audio dictionary
        audioDictionary = new Dictionary<string, SurfaceAudio>();
        foreach (SurfaceAudio audio in surfaceAudio)
        {
            audioDictionary.Add(audio.tag, audio);
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found!");
        }
    }

    public void PlaySound(string tag, string action)
    {
        if (Time.time - lastSoundTime < audioDictionary[tag].cooldown)
        {
            return;
        }

        AudioClip clip = null;
        switch (action)
        {
            case "walking":
                clip = audioDictionary[tag].walkingSounds[Random.Range(0, audioDictionary[tag].walkingSounds.Length)];
                break;
            case "landing":
                clip = audioDictionary[tag].landingSounds[Random.Range(0, audioDictionary[tag].landingSounds.Length)];
                break;
            case "jumping":
                clip = audioDictionary[tag].jumpingSounds[Random.Range(0, audioDictionary[tag].jumpingSounds.Length)];
                break;
        }

        if (clip != null)
        {
            audioSource.pitch = 1f + Random.Range(-audioDictionary[tag].pitchRange, audioDictionary[tag].pitchRange);
            audioSource.PlayOneShot(clip);
            lastSoundTime = Time.time;
        }
    }
}
