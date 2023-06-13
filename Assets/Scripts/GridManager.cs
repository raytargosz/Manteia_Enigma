using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Size of each grid unit.")]
    [SerializeField] private Vector3 gridSize = Vector3.one;
    [Tooltip("Position of the grid.")]
    [SerializeField] private Vector3 gridPosition = Vector3.zero;
    [Tooltip("Alpha of the viewable Gizmo.")]
    [SerializeField] [Range(0f, 1f)] private float gizmoAlpha = 1f;

    // Dictionary to keep track of objects in the grid
    private Dictionary<Vector3, GameObject> grid = new Dictionary<Vector3, GameObject>();

    // In the Unity inspector, changing one dimension of the grid size 
    // will change all dimensions to keep the cube shape.
    void OnValidate()
    {
        gridSize.x = gridSize.y = gridSize.z = gridSize.x;
    }

    // Function to add object to the grid at a specific position
    public void PlaceObject(GameObject obj, Vector3 position)
    {
        Vector3 gridPos = WorldToGridPosition(position);

        if (!grid.ContainsKey(gridPos))
        {
            grid[gridPos] = obj;
            obj.transform.position = gridPos;
        }
    }

    // Function to remove an object from the grid
    public void RemoveObject(Vector3 position)
    {
        Vector3 gridPos = WorldToGridPosition(position);

        if (grid.ContainsKey(gridPos))
        {
            grid.Remove(gridPos);
        }
    }

    // Converts a world position to the corresponding grid position
    private Vector3 WorldToGridPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / gridSize.x);
        int y = Mathf.RoundToInt(position.y / gridSize.y);
        int z = Mathf.RoundToInt(position.z / gridSize.z);

        return new Vector3(x, y, z) * gridSize.x;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, gizmoAlpha);

        for (float x = 0; x <= gridSize.x; x += 1)
        {
            for (float y = 0; y <= gridSize.y; y += 1)
            {
                for (float z = 0; z <= gridSize.z; z += 1)
                {
                    var point = new Vector3(x, y, z) + gridPosition;
                    Gizmos.DrawWireCube(point, Vector3.one);
                }
            }
        }
    }
}
