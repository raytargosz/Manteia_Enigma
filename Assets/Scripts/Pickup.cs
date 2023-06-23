using UnityEngine;
using TMPro;

public class Pickup : MonoBehaviour
{
    public int scoreValue = 1;       // Value to add to the score when picked up
    public AudioSource pickupSFX;    // Sound effect to play when picked up
    public GameObject pickupVFX;     // Visual effect to instantiate when picked up
    public TextMeshProUGUI scoreText; // TMP Text component to display the score

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Update score
            ScoreManager.instance.AddScore(scoreValue);
            scoreText.text = ScoreManager.instance.GetScore().ToString();

            // Play audio, if an AudioSource is provided
            if (pickupSFX != null)
            {
                pickupSFX.Play();
            }

            // Play visual effect, if a prefab is provided
            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, transform.rotation);
            }

            // Destroy the pickup object
            Destroy(gameObject);
        }
    }
}
