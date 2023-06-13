using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreakableBlock : MonoBehaviour
{
    [Header("Visual Effects")]
    [Tooltip("Particle effect when the block spawns.")]
    public ParticleSystem spawnEffect;

    [Tooltip("Particle effect when the block is destroyed.")]
    public ParticleSystem destroyEffect;

    [Tooltip("Visual effect when the block is hit once.")]
    public GameObject crackVisualEffect;

    [Header("Audio Effects")]
    [Tooltip("Audio effect when the block spawns.")]
    public AudioClip spawnClip;

    [Tooltip("Audio effect when the block is hit.")]
    public AudioClip hitClip;

    [Tooltip("Audio effect when the block is destroyed.")]
    public AudioClip destroyClip;

    [Tooltip("Range of random pitch adjustment for sound effects.")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private bool isCracked = false;
    private AudioSource audioSource;

    // Called when the script instance is being loaded
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Called when the block is instantiated
    void Start()
    {
        spawnEffect.Play();
        PlaySound(spawnClip);
    }

    // The method called when the block is hit
    public void Interact()
    {
        if (isCracked)
        {
            // If the block is already cracked, destroy it
            DestroyBlock();
        }
        else
        {
            // Otherwise, display the crack visual and mark the block as cracked
            crackVisualEffect.SetActive(true);
            isCracked = true;
            PlaySound(hitClip);
        }
    }

    private void DestroyBlock()
    {
        destroyEffect.Play();
        PlaySound(destroyClip);
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the collision is from above, treat it as an interaction
        if (collision.relativeVelocity.y > 0)
        {
            Interact();
        }
    }
}
