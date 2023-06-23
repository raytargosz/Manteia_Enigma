using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int scoreValue = 1;
    public AudioClip pickupSFX;
    public GameObject pickupVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with pickup!");

            ScoreManager.instance.AddScore(scoreValue);

            if (pickupSFX != null)
                AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

            if (pickupVFX != null)
            {
                var vfx = Instantiate(pickupVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 5f);
            }

            Destroy(gameObject);
        }
    }
}
