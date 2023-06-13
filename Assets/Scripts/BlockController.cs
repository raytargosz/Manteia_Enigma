using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Tooltip("Prefab of the breakable block that can be spawned by the player.")]
    public GameObject breakableBlockPrefab;

    [Tooltip("Distance from player to spawn the block.")]
    public float spawnDistance = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Change this input as needed
        {
            // Define a raycast in the direction the player is facing
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, spawnDistance))
            {
                // If a block is hit by the raycast
                BreakableBlock block = hit.collider.GetComponent<BreakableBlock>();
                if (block)
                {
                    // Destroy the block
                    block.Interact();
                    return;
                }
            }

            Vector3 spawnPosition;

            if (Input.GetKey(KeyCode.C)) // Change this input as needed
            {
                // Spawn block in front and below if crouching
                spawnPosition = transform.position + transform.forward - transform.up;
            }
            else
            {
                // Otherwise spawn block directly in front
                spawnPosition = transform.position + transform.forward;
            }

            // Create the new block
            Instantiate(breakableBlockPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
