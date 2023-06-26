using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour
{
    [Tooltip("Add the collider representing the room's area.")]
    public Collider roomArea;

    [Tooltip("Add the room's lights, props and other objects here.")]
    public List<GameObject> roomObjects = new List<GameObject>();

    private RoomManager roomManager;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == roomManager.player)
        {
            roomManager.RegisterRoom(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == roomManager.player)
        {
            roomManager.UnregisterRoom(this);
        }
    }

    public void SetActive(bool active)
    {
        foreach (var obj in roomObjects)
        {
            obj.SetActive(active);
        }
    }
}
