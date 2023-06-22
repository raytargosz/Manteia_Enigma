using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public AudioClip[] backgroundSFX;
    public float fadeTime = 2f;
    public float volume = 1f;  // set default volume to 1, adjust this in the inspector
    public AudioSource musicSourcePrefab;

    private Queue<AudioClip> shuffledMusicTracks;
    private AudioSource currentMusic;
    private bool isTrackSwitching;

    private void Start()
    {
        foreach (var sfx in backgroundSFX)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.clip = sfx;
            sfxSource.loop = true;
            sfxSource.volume = volume;  // set the volume
            sfxSource.Play();
        }

        ShuffleMusicTracks();
        PlayNextTrack();
    }

    private void ShuffleMusicTracks()
    {
        List<AudioClip> musicList = new List<AudioClip>(musicTracks);
        shuffledMusicTracks = new Queue<AudioClip>();

        while (musicList.Count > 0)
        {
            int index = Random.Range(0, musicList.Count);
            shuffledMusicTracks.Enqueue(musicList[index]);
            musicList.RemoveAt(index);
        }
    }

    private void PlayNextTrack()
    {
        if (!isTrackSwitching)
        {
            isTrackSwitching = true;

            if (currentMusic != null)
            {
                StartCoroutine(FadeOutAndPlayNext(currentMusic, fadeTime));
            }
            else
            {
                if (shuffledMusicTracks.Count == 0)
                {
                    ShuffleMusicTracks();
                }

                currentMusic = Instantiate(musicSourcePrefab, transform);
                currentMusic.clip = shuffledMusicTracks.Dequeue();
                currentMusic.volume = volume; // set the volume
                StartCoroutine(FadeIn(currentMusic, fadeTime));
            }
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < volume) // fade to the target volume
        {
            audioSource.volume += Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = volume;
        isTrackSwitching = false;
    }

    private IEnumerator FadeOutAndPlayNext(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        Destroy(audioSource.gameObject);

        if (shuffledMusicTracks.Count == 0)
        {
            ShuffleMusicTracks();
        }

        currentMusic = Instantiate(musicSourcePrefab, transform);
        currentMusic.clip = shuffledMusicTracks.Dequeue();
        currentMusic.volume = volume; // set the volume
        StartCoroutine(FadeIn(currentMusic, fadeTime));
    }

    private void Update()
    {
        if (!isTrackSwitching && (currentMusic == null || !currentMusic.isPlaying))
        {
            PlayNextTrack();
        }
    }
}
