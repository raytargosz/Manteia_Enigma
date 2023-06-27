using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrainSoundCategory
{
    [Header("Sound Category Settings")]
    [Tooltip("Category name, for clarity")]
    public string categoryName;

    [Tooltip("Audio clips in this category")]
    public AudioClip[] soundEffects;

    [Tooltip("Minimum interval (in seconds) at which sounds from this category should play")]
    public float minInterval;

    [Tooltip("Maximum interval (in seconds) at which sounds from this category should play")]
    public float maxInterval;
}

public class TrainSoundManager : MonoBehaviour
{
    [Header("Train Sound Manager Settings")]
    [Tooltip("Sound categories to play")]
    public TrainSoundCategory[] soundCategories;

    [Tooltip("Audio source to play the sounds")]
    public AudioSource audioSource;

    private void Start()
    {
        foreach (var soundCategory in soundCategories)
        {
            StartCoroutine(PlayRandomSoundFromCategory(soundCategory));
        }
    }

    private IEnumerator PlayRandomSoundFromCategory(TrainSoundCategory soundCategory)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(soundCategory.minInterval, soundCategory.maxInterval));

            int clipIndex = Random.Range(0, soundCategory.soundEffects.Length);
            audioSource.PlayOneShot(soundCategory.soundEffects[clipIndex]);
        }
    }
}
