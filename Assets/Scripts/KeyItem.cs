using System.Collections;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Key Pickup Audio and Visuals")]
    public AudioSource audioSource;
    public AudioClip pickupSFX;
    public ParticleSystem pickupVFX;
    public float increasedSpinSpeed = 20f; // The spin speed during pickup

    [Header("Audio Settings")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private BobbingAndSpinning bobbingAndSpinning;
    private float effectDuration = 2.0f;
    private float originalSpinSpeed;

    private void Start()
    {
        bobbingAndSpinning = GetComponent<BobbingAndSpinning>();
        if (bobbingAndSpinning != null)
        {
            originalSpinSpeed = bobbingAndSpinning.spinningSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            if (bobbingAndSpinning != null)
            {
                bobbingAndSpinning.spinningSpeed = increasedSpinSpeed;
            }

            if (audioSource != null && pickupSFX != null)
            {
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
                audioSource.PlayOneShot(pickupSFX);
            }

            if (pickupVFX != null)
            {
                pickupVFX.Play();
            }

            StartCoroutine(DeactivateAfterDelay(effectDuration));
        }
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bobbingAndSpinning != null)
        {
            bobbingAndSpinning.spinningSpeed = originalSpinSpeed;
        }

        gameObject.SetActive(false);
    }
}
