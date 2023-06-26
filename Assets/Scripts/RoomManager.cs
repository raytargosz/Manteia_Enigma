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
