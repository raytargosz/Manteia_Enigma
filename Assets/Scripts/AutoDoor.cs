using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoDoor : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> leftDoors;
    public List<GameObject> rightDoors;
    public List<GameObject> keyItems;
    public float doorSpeed = 2f;
    public AudioSource audioSource;
    public AudioClip[] openSFX;
    public AudioClip[] closeSFX;
    public Vector2 openPitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 closePitchRange = new Vector2(0.95f, 1.05f);
    public TMP_Text lockMessage;
    private bool doorsOpen = false;
    private bool insideTrigger = false;
    private int keysCollected = 0;

    private Dictionary<GameObject, Quaternion> closedRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, Quaternion> openRotations = new Dictionary<GameObject, Quaternion>();

    void Start()
    {
        foreach (var door in leftDoors)
        {
            closedRotations[door] = door.transform.rotation;
            openRotations[door] = Quaternion.Euler(door.transform.eulerAngles + new Vector3(0, -80, 0));  // left doors rotate -X degrees when opened
        }

        foreach (var door in rightDoors)
        {
            closedRotations[door] = door.transform.rotation;
            openRotations[door] = Quaternion.Euler(door.transform.eulerAngles + new Vector3(0, 80, 0));  // right doors rotate X degrees when opened
        }
    }

    void Update()
    {
        if (insideTrigger && !doorsOpen && keysCollected == keyItems.Count)
        {
            OpenDoors();
            lockMessage.gameObject.SetActive(false);
        }
        else if (!insideTrigger && doorsOpen)
        {
            CloseDoors();
        }
        else if (insideTrigger && !doorsOpen)
        {
            lockMessage.text = "The door is locked. Keys collected: " + keysCollected + "/" + keyItems.Count;
            lockMessage.gameObject.SetActive(true);
        }
    }

    private void OpenDoors()
    {
        doorsOpen = true;
        PlaySound(openSFX, openPitchRange);
        foreach (var door in leftDoors)
        {
            StartCoroutine(RotateDoor(door, openRotations[door]));
        }
        foreach (var door in rightDoors)
        {
            StartCoroutine(RotateDoor(door, openRotations[door]));
        }
    }

    private void CloseDoors()
    {
        doorsOpen = false;
        PlaySound(closeSFX, closePitchRange);
        foreach (var door in leftDoors)
        {
            StartCoroutine(RotateDoor(door, closedRotations[door]));
        }
        foreach (var door in rightDoors)
        {
            StartCoroutine(RotateDoor(door, closedRotations[door]));
        }
    }

    IEnumerator RotateDoor(GameObject door, Quaternion toRotation)
    {
        var fromRotation = door.transform.rotation;
        for (float t = 0f; t < 1; t += Time.deltaTime * doorSpeed)
        {
            door.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
            yield return null;
        }
        door.transform.rotation = toRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = true;
        }

        if (keyItems.Contains(other.gameObject))
        {
            keysCollected++;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = false;
            lockMessage.gameObject.SetActive(false);
        }
    }

    private void PlaySound(AudioClip[] sounds, Vector2 pitchRange)
    {
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
}
