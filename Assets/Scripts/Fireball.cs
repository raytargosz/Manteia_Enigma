using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10.0f;  // Set the speed for fireball
    public AudioSource impactAudio; // Set impact audio source for collision
    public AudioClip[] impactSFX; // Array of impact sounds for collision
    public AudioSource spawnAndMovingAudio; // Set audio source for spawn and moving
    public AudioClip[] spawnAndMovingSFX; // Array of sounds for spawn and moving

    private Rigidbody rb;
    private Vector3 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    private void Start()
    {
        // Give the fireball an initial speed in the direction it's facing
        rb.velocity = direction * speed;

        // Play a random SFX for spawn and moving
        PlayRandomSFX(spawnAndMovingAudio, spawnAndMovingSFX);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the fireball has hit the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by fireball!");
            // Implement logic here for what happens when the player is hit
            GameManager.Instance.GameOver(); // Let's assume there's a GameManager handling game states
        }

        PlayRandomSFX(impactAudio, impactSFX);
        Destroy(this.gameObject, impactAudio.clip.length);  // Destroy the fireball after impact sound has played
    }

    // Method to play a random SFX from the array
    private void PlayRandomSFX(AudioSource audioSource, AudioClip[] audioClips)
    {
        if (audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[randomIndex];
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        // Stop the moving sound effect when the fireball is destroyed
        spawnAndMovingAudio.Stop();
    }
}
