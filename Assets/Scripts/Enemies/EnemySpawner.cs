using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("Initial delay for first enemy spawn after level start.")]
    [SerializeField] private float initialSpawnDelay = 5f;
    [Tooltip("Delay between subsequent enemy spawns.")]
    [SerializeField] private float intervalSpawnDelay = 5f;
    [Tooltip("Lifetime of the spawned enemy.")]
    [SerializeField] private float enemyLifetime = 10f;
    [Tooltip("Enemy prefab to be spawned.")]
    [SerializeField] private GameObject enemyPrefab;
    [Tooltip("Initial move direction of the enemy.")]
    [SerializeField] private Vector3 initialMoveDirection = Vector3.right;

    private void Start()
    {
        // start the spawning coroutine
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        // initial spawn delay
        yield return new WaitForSeconds(initialSpawnDelay);

        while (true)
        {
            // instantiate enemy
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                // Set the initial direction and lifetime of the enemy
                enemyScript.SetInitialDirection(initialMoveDirection);
                enemyScript.SetLifetime(enemyLifetime);
            }
            else
            {
                Debug.LogError("Enemy prefab does not have an Enemy script attached.");
            }

            // wait for the interval spawn delay
            yield return new WaitForSeconds(intervalSpawnDelay);
        }
    }
}
