using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Lifetime of the enemy.")]
    [SerializeField] protected float lifetime;

    [Header("Movement")]
    [Tooltip("Initial direction of the enemy.")]
    [SerializeField] protected Vector3 initialDirection;
    [Tooltip("Movement speed of the enemy.")]
    [SerializeField] protected float moveSpeed;

    protected Vector3 direction;
    protected Rigidbody rb;

    public void SetInitialDirection(Vector3 initialDirection)
    {
        this.initialDirection = initialDirection;
    }

    public void SetLifetime(float lifetime)
    {
        this.lifetime = lifetime;
        Destroy(gameObject, lifetime);
    }

    protected virtual void Start()
    {
        direction = initialDirection;
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // Default movement logic
    }
}
