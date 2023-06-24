using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab; // Drag your fireball prefab here in Inspector
    public float spawnInterval = 2.0f; // Time between each spawn
    public float fireballLifetime = 10.0f; // Fireball life time
    private bool gameIsOver = false;

    private void Start()
    {
        InvokeRepeating("SpawnFireball", spawnInterval, spawnInterval);
    }

    private void SpawnFireball()
    {
        if (!gameIsOver)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);
            Fireball fireballScript = fireball.GetComponent<Fireball>();

            if (fireballScript != null)
            {
                fireballScript.SetDirection(transform.forward);
            }

            Destroy(fireball, fireballLifetime);
        }
    }

    // Draw a gizmo in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }

    public void StopSpawning()
    {
        gameIsOver = true;
    }
}
