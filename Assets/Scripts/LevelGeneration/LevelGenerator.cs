using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Chunk[] chunks;          // The different level chunks
    public int levelSize;           // Number of chunks in the level

    public Transform player;        // Reference to the player's transform

    private List<GameObject> currentLevel; // The chunks currently in the level

    private Chunk currentChunk;     // The currently active chunk
    private Chunk previousChunk;    // The previously active chunk

    void Start()
    {
        currentLevel = new List<GameObject>();
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Clear the current level
        foreach (GameObject chunk in currentLevel)
        {
            Destroy(chunk);
        }
        currentLevel.Clear();

        // Initialize level generation
        currentChunk = Instantiate(chunks[Random.Range(0, chunks.Length)]); // Create the first chunk
        currentChunk.transform.position = Vector3.zero; // Position the chunk at the origin
        currentLevel.Add(currentChunk.gameObject); // Add the chunk to the current level
        currentChunk.ActivateChunk(player); // Activate the chunk

        // Generate subsequent chunks
        for (int i = 1; i < levelSize; i++)
        {
            Chunk newChunk = Instantiate(chunks[Random.Range(0, chunks.Length)]); // Create a new chunk
            newChunk.transform.position = GetNextChunkPosition(); // Position the chunk relative to the previous chunk
            currentLevel.Add(newChunk.gameObject); // Add the chunk to the current level
            newChunk.ActivateChunk(player); // Activate the chunk

            // Connect the new chunk to the previous chunk
            ConnectChunks(previousChunk, newChunk);

            previousChunk = newChunk; // Update the previous chunk reference
        }
    }

    Vector3 GetNextChunkPosition()
    {
        // Calculate the position for the next chunk based on the previous chunk's exit point
        return previousChunk.GetExitPoint();
    }

    void ConnectChunks(Chunk previousChunk, Chunk newChunk)
    {
        // Align the entrance and exit points of the chunks for seamless connection
        Vector3 exitPoint = previousChunk.GetExitPoint();
        Vector3 entryPoint = newChunk.GetEntryPoint();

        Vector3 displacement = entryPoint - exitPoint;
        newChunk.transform.position -= displacement;
    }
}
