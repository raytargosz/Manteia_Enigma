using UnityEngine;
using System.Collections;

public class SkullEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Movement speed of the enemy.")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Hesitation time when enemy hits an obstacle.")]
    [SerializeField] private float hesitateTime = 0.5f;
    [Tooltip("Damage dealt by the enemy.")]
    [SerializeField] private int damage = 1;

    [Header("Effects")]
    [Tooltip("Sound effect on death.")]
    [SerializeField] private AudioClip deathSound;
    [Tooltip("Visual effect on death.")]
    [SerializeField] private ParticleSystem deathEffect;

    private Vector3 direction;
    private Rigidbody rb;

    public void SetInitialDirection(Vector3 initialDirection)
    {
        direction = initialDirection;
    }

    public void SetLifetime(float lifetime)
    {
        Destroy(gameObject, lifetime);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.velocity.y == 0)
        {
            // Enemy is on the ground, so it can move horizontally
            rb.velocity = direction * moveSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage player...
        }
        else if (collision.gameObject.CompareTag("Breakable"))
        {
            // Destroy breakable cube...
            direction *= -1; // Change direction
            StartCoroutine(Hesitate());
        }
        else if (collision.gameObject.CompareTag("Unbreakable"))
        {
            direction *= -1; // Change direction
            StartCoroutine(Hesitate());
        }
    }

    private void OnDestroy()
    {
        // Play sound and visual effects...
    }

    private IEnumerator Hesitate()
    {
        yield return new WaitForSeconds(hesitateTime);
    }
}
