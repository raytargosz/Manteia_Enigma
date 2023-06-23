using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public Image overlayImage; // Image to fade in/out
    public float fadeDuration = 2f; // Time it takes for the overlay to fade
    public float playerControlDelay = 0.25f; // Delay before player can move after fade
    public AudioClip introSFX; // Intro sound effect
    public float sfxVolume = 1f; // Volume of the sound effect
    public Transform playerTransform; // Player's transform to set the initial position and orientation
    private PlayerController playerController; // Reference to the PlayerController script

    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private void Awake()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController script not found!");
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerController.enabled = false;
        overlayImage.color = Color.black;
        AudioSource.PlayClipAtPoint(introSFX, playerTransform.position, sfxVolume);

        // Store the initial player position and rotation
        initialPlayerPosition = playerTransform.position;
        initialPlayerRotation = playerTransform.rotation;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            overlayImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(playerControlDelay);

        // Reset the player's position and rotation
        playerTransform.position = initialPlayerPosition;

        // Assuming Y rotation is what we need to preserve, modify according to your needs
        playerTransform.rotation = Quaternion.Euler(playerTransform.eulerAngles.x, initialPlayerRotation.eulerAngles.y, playerTransform.eulerAngles.z);

        playerController.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
