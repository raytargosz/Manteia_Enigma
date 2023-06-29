using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("The GameObject of the door.")]
    public GameObject doorObject;

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

    // Bool to track if the door is locked
    private bool doorLocked = true;
    // Bool to track if the locked sound effect has already been played during the current trigger stay
    private bool playedLockSFX = false;

    private void Start()
    {
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
                playedLockSFX = true;
            }
            else if (!doorLocked)
            {
                // Re-enable AutoPortcullis when door is unlocked
                if (autoPortcullis != null)
                {
                    autoPortcullis.enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && doorLocked)
        {
            // Reset the flag so that the SFX will play again the next time the player enters
            playedLockSFX = false;
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
}
