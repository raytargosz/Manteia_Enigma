using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10.0f;  // Set the speed for fireball
    public AudioSource impactAudio; // Set impact audio source for collision
    public AudioClip[] impactSFX; // Array of impact sounds for collision
    public AudioSource spawnAndMovingAudio; // Set audio source for spawn and moving
    public AudioClip[] spawnAndMovingSFX; // Array of sounds for spawn and moving
    public float fireballLifetime = 10.0f; // Fireball life time

    private Rigidbody rb;
    private Vector3 direction;
    private float lifespanTimer; // Timer for the fireball's life
    private bool canCollideWithNonPlayer; // Whether the fireball can collide with non-player objects

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lifespanTimer = 0;
        canCollideWithNonPlayer = false;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    private void Start()
    {
        rb.velocity = direction * speed;
        PlayRandomSFX(spawnAndMovingAudio, spawnAndMovingSFX);
        Invoke("AllowCollisionsWithNonPlayer", fireballLifetime * 0.5f);
    }

    private void Update()
    {
        lifespanTimer += Time.deltaTime;

        if (lifespanTimer >= fireballLifetime)
        {
            PlayRandomSFX(impactAudio, impactSFX);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by fireball!");
            GameManager.Instance.GameOver(gameObject.tag);
            PlayRandomSFX(impactAudio, impactSFX);
            Destroy(this.gameObject);
        }
        else if (canCollideWithNonPlayer)
        {
            PlayRandomSFX(impactAudio, impactSFX);
            Destroy(this.gameObject);
        }
    }

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
        spawnAndMovingAudio.Stop();
    }

    private void AllowCollisionsWithNonPlayer()
    {
        canCollideWithNonPlayer = true;
    }
}