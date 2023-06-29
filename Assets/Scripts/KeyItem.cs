using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Key Pickup Audio and Visuals")]
    [Tooltip("The audio source that will play the pickup sound effect")]
    public AudioSource audioSource;
    [Tooltip("The sound effect that will play when the key is picked up")]
    public AudioClip pickupSFX;
    [Tooltip("The visual effect that will play when the key is picked up")]
    public ParticleSystem pickupVFX;

    [Header("Audio Settings")]
    [Tooltip("The range of pitches that the sound effect can have when the key is picked up")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    /// <summary>
    /// Method called when another collider enters this object's trigger area.
    /// If the other object is tagged as "Player", it will play the pickup sound effect,
    /// play the pickup visual effect (if it exists), and deactivate this object.
    /// </summary>
    /// <param name="other">The other collider that entered this object's trigger area</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is the player
        if (other.gameObject.CompareTag("Player")) // assuming the player object has a tag of "Player"
        {
            // Play sound effect
            if (audioSource != null && pickupSFX != null)
            {
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
                audioSource.PlayOneShot(pickupSFX);
            }

            // Play visual effect
            if (pickupVFX != null)
            {
                pickupVFX.Play();
            }

            // Disable the object
            gameObject.SetActive(false);
        }
    }
}
