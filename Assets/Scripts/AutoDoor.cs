using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoDoor : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("The GameObjects of the doors. 0 - Left door, 1 - Right door.")]
    public GameObject[] doorObjects;
    [Tooltip("The speed at which the doors open.")]
    public float openSpeed = 1.0f;
    [Tooltip("The angle to rotate the doors when they open.")]
    public float openAngle = 90.0f;

    [Header("Door SFX Settings")]
    [Tooltip("The sound effects that play when the door opens.")]
    public AudioClip[] openingSFX;
    [Tooltip("The sound effects that play when the door closes.")]
    public AudioClip[] closingSFX;

    [Header("Inventory and Required Items")]
    [Tooltip("The player's inventory.")]
    public Inventory playerInventory;
    [Tooltip("The key items required to unlock this door.")]
    public KeyItem[] requiredKeys;

    [Header("Lock SFX Settings")]
    [Tooltip("The sound effects that play when the door is locked.")]
    public AudioClip[] lockedSFX;
    [Tooltip("The audio source that will play the locked sound effect.")]
    public AudioSource audioSource;
    [Tooltip("The range of pitches that the locked sound effect can have.")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Auto Portcullis")]
    [Tooltip("Reference to the AutoPortcullis script that controls the portcullis for this door.")]
    public AutoPortcullis autoPortcullis;

    [Header("UI Components")]
    [Tooltip("UI text that will display 'Door is locked' message.")]
    public TMP_Text lockedDoorText;

    // Bool to track if the door is locked
    private bool doorLocked = true;
    // Bool to track if the locked sound effect has already been played during the current trigger stay
    private bool playedLockSFX = false;

    private Quaternion[] originalRotations;
    private Quaternion[] openRotations;

    private void Start()
    {
        // Store the original rotations of the doors and calculate their open rotations
        originalRotations = new Quaternion[doorObjects.Length];
        openRotations = new Quaternion[doorObjects.Length];
        for (int i = 0; i < doorObjects.Length; i++)
        {
            originalRotations[i] = doorObjects[i].transform.rotation;
            openRotations[i] = Quaternion.Euler(doorObjects[i].transform.eulerAngles + new Vector3(0, (i == 0 ? -1 : 1) * openAngle, 0));
        }

        // By default, we stop AutoPortcullis from running
        if (autoPortcullis != null)
        {
            autoPortcullis.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckDoorLockStatus();

            if (doorLocked && !playedLockSFX)
            {
                PlayRandomSFX(lockedSFX);
                lockedDoorText.gameObject.SetActive(true); // Show the text
                playedLockSFX = true;
            }
            else if (!doorLocked)
            {
                // Play opening SFX
                PlayRandomSFX(openingSFX);

                // Re-enable AutoPortcullis and open the doors when door is unlocked
                if (autoPortcullis != null)
                {
                    autoPortcullis.enabled = true;
                }
                StartCoroutine(OpenDoors());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && doorLocked)
        {
            // Reset the flag so that the SFX will play again the next time the player enters
            playedLockSFX = false;
            lockedDoorText.gameObject.SetActive(false); // Hide the text
        }
        else if (other.gameObject.CompareTag("Player") && !doorLocked)
        {
            // Play closing SFX
            PlayRandomSFX(closingSFX);

            // Start closing the doors when the player leaves the collider
            StartCoroutine(CloseDoors());
        }
    }

    // Checks if the player has all required keys to unlock the door
    private void CheckDoorLockStatus()
    {
        // Loop through all required keys and check if they're in the player's inventory
        foreach (KeyItem key in requiredKeys)
        {
            if (!playerInventory.Contains(key))
            {
                // As soon as one key is not found in the inventory, we know the door is locked
                doorLocked = true;
                return;
            }
        }

        // If we made it through the loop without returning, all keys are in the inventory and the door is unlocked
        doorLocked = false;
    }

    // Plays a random sound effect from the given array
    private void PlayRandomSFX(AudioClip[] sounds)
    {
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }

    // Opens the doors over time
    private IEnumerator OpenDoors()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            for (int i = 0; i < doorObjects.Length; i++)
            {
                doorObjects[i].transform.rotation = Quaternion.Lerp(originalRotations[i], openRotations[i], t);
            }
            yield return null;
        }

        // Disable colliders
        foreach (GameObject door in doorObjects)
        {
            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
        }
    }

    // Closes the doors over time
    private IEnumerator CloseDoors()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            for (int i = 0; i < doorObjects.Length; i++)
            {
                doorObjects[i].transform.rotation = Quaternion.Lerp(openRotations[i], originalRotations[i], t);
            }
            yield return null;
        }

        // Re-enable colliders
        foreach (GameObject door in doorObjects)
        {
            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = true;
            }
        }
    }
}
