using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour
{
    [Tooltip("Add the collider representing the room's area.")]
    public Collider roomArea;

    [Tooltip("Add the room's lights, props and other objects here.")]
    public List<GameObject> roomObjects = new List<GameObject>();

    public void SetActive(bool active)
    {
        foreach (var obj in roomObjects)
        {
            if (obj.GetComponent<Collider>() == null || obj.tag != "RoomCollider")
            {
                obj.SetActive(active);
            }
        }
    }

    public bool IsPlayerInside(Vector3 playerPosition)
    {
        return roomArea.bounds.Contains(playerPosition);
    }
}

