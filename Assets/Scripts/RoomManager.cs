using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Tooltip("Add your player GameObject here.")]
    public GameObject player;

    [Tooltip("Add your rooms here.")]
    public List<Room> rooms = new List<Room>();

    private Room currentRoom;

    private void Start()
    {
        // Disable all rooms at start.
        foreach (var room in rooms)
        {
            room.SetActive(false);
        }

        // Enable the first room.
        if (rooms.Count > 0)
        {
            currentRoom = rooms[0];
            currentRoom.SetActive(true);
        }
    }

    private void Update()
    {
        // Find the room the player is currently in.
        foreach (var room in rooms)
        {
            if (room.IsPlayerInside(player.transform.position))
            {
                // If the player has entered a new room, disable the current room and enable the new one.
                if (currentRoom != room)
                {
                    currentRoom.SetActive(false);
                    room.SetActive(true);
                    currentRoom = room;
                }

                break;
            }
        }
    }
}

[System.Serializable]
public class Room
{
    [Tooltip("Add the collider representing the room's area.")]
    public Collider roomArea;

    [Tooltip("Add the room's lights, props and other objects here.")]
    public List<GameObject> roomObjects = new List<GameObject>();

    public void SetActive(bool active)
    {
        foreach (var obj in roomObjects)
        {
            obj.SetActive(active);
        }
    }

    public bool IsPlayerInside(Vector3 playerPosition)
    {
        return roomArea.bounds.Contains(playerPosition);
    }
}
