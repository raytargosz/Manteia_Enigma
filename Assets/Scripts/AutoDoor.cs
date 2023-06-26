using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> leftDoors;
    public List<GameObject> rightDoors;
    public float doorSpeed = 2f;
    public AudioSource audioSource;
    public AudioClip openSFX;
    public AudioClip closeSFX;
    private bool doorsOpen = false;
    private bool insideTrigger = false;

    void Update()
    {
        if (insideTrigger && !doorsOpen)
        {
            OpenDoors();
        }
        else if (!insideTrigger && doorsOpen)
        {
            CloseDoors();
        }
    }

    private void OpenDoors()
    {
        doorsOpen = true;
        audioSource.PlayOneShot(openSFX);
        foreach (var door in leftDoors)
        {
            StartCoroutine(RotateDoor(door, -90f));  // rotate left doors anti-clockwise
        }
        foreach (var door in rightDoors)
        {
            StartCoroutine(RotateDoor(door, 90f));  // rotate right doors clockwise
        }
    }

    private void CloseDoors()
    {
        doorsOpen = false;
        audioSource.PlayOneShot(closeSFX);
        foreach (var door in leftDoors)
        {
            StartCoroutine(RotateDoor(door, 90f));  // rotate left doors clockwise
        }
        foreach (var door in rightDoors)
        {
            StartCoroutine(RotateDoor(door, -90f));  // rotate right doors anti-clockwise
        }
    }

    IEnumerator RotateDoor(GameObject door, float angle)
    {
        var fromAngle = door.transform.rotation;
        var toAngle = Quaternion.Euler(door.transform.eulerAngles.x, door.transform.eulerAngles.y + angle, door.transform.eulerAngles.z);
        for (float t = 0f; t < 1; t += Time.deltaTime * doorSpeed)
        {
            door.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = false;
        }
    }
}