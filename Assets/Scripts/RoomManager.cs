using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Tooltip("Add your player GameObject here.")]
    public GameObject player;

    [Tooltip("Add your rooms here.")]
    public List<Room> rooms = new List<Room>();

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
            rooms[0].SetActive(true);
        }
    }

    private void Update()
    {
        // Find the room the player is currently in.
        foreach (var room in rooms)
        {
            if (room.IsPlayerInside(player.transform.position))
            {
                // Enable room if player is inside.
                room.SetActive(true);
            }
            else
            {
                // Disable room if player is not inside.
                room.SetActive(false);
            }
        }
    }
}
