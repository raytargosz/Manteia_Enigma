using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Transform entryPoint;   // Entry point of the chunk
    [SerializeField] private Transform exitPoint;    // Exit point of the chunk

    // Define other properties or variables specific to the Chunk class
    // ...

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateChunk(other.transform);
        }
    }

    public void ActivateChunk(Transform player)
    {
        // Implement the logic to activate the chunk, such as enabling enemies, spawning objects, etc.
        // ...

        // Teleport the player to the entry point of the chunk
        player.position = entryPoint.position;
    }

    public Vector3 GetExitPoint()
    {
        return exitPoint.position;
    }

    public Vector3 GetEntryPoint()
    {
        return entryPoint.position;
    }
}
