using UnityEngine;
using TMPro;

public class Pickup : MonoBehaviour
{
    public int scoreValue = 1;
    public AudioClip pickupSFX;
    public float pickupSFXVolume = 0.25f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public GameObject pickupVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with pickup!");

            ScoreManager.instance.AddScore(scoreValue);

            if (pickupSFX != null)
            {
                GameObject audioObject = new GameObject("PickupSFX");
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = pickupSFX;
                audioSource.volume = pickupSFXVolume;
                audioSource.pitch = Random.Range(minPitch, maxPitch); // Set a random pitch within the given range
                audioSource.spatialBlend = 0;
                audioSource.Play();

                Destroy(audioObject, pickupSFX.length); // Destroy this object once the clip has finished
            }

            if (pickupVFX != null)
            {
                var vfx = Instantiate(pickupVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 5f);
            }

            Destroy(gameObject);
        }
    }
}