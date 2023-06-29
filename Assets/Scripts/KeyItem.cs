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

    // Assume the SFX/VFX lasts for 2 seconds, adjust this to match the length of your effects
    private float effectDuration = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Disable collider to prevent further triggers
            GetComponent<Collider>().enabled = false;

            if (audioSource != null && pickupSFX != null)
            {
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
                audioSource.PlayOneShot(pickupSFX);
            }

            if (pickupVFX != null)
            {
                pickupVFX.Play();
            }

            // Wait for the effects to finish before deactivating the object
            StartCoroutine(DeactivateAfterDelay(effectDuration));
        }
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
