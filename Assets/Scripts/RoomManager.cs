using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Tooltip("Add your player GameObject here.")]
    public GameObject player;

    [Tooltip("Add your rooms here.")]
    public List<Room> rooms = new List<Room>();

    private Room currentRoom;
    private List<Room> roomsPlayerIsInside = new List<Room>();

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
        // If player is not inside any room, then don't do anything
        if (roomsPlayerIsInside.Count == 0)
            return;

        Room lastEnteredRoom = roomsPlayerIsInside[roomsPlayerIsInside.Count - 1];
        // If the player has entered a new room, disable the current room and enable the new one.
        if (currentRoom != lastEnteredRoom)
        {
            currentRoom.SetActive(false);
            lastEnteredRoom.SetActive(true);
            currentRoom = lastEnteredRoom;
        }
    }

    public void RegisterRoom(Room room)
    {
        roomsPlayerIsInside.Add(room);
    }

    public void UnregisterRoom(Room room)
    {
        roomsPlayerIsInside.Remove(room);
    }
}
