using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEndTrigger : MonoBehaviour
{
    public Image overlayImage; // Image to fade in/out
    public float fadeDuration = 2f; // Time it takes for the overlay to fade
    public AudioClip endLevelSFX; // End level sound effect
    public float sfxVolume = 1f; // Volume of the sound effect
    public float transitionDelay = 1f; // Delay before loading the next scene
    public Transform playerTransform; // Player's transform
    private PlayerController playerController; // Reference to the PlayerController script

    private void Awake()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController script not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.enabled = false; // Disable player controls
            AudioSource.PlayClipAtPoint(endLevelSFX, playerTransform.position, sfxVolume); // Play the end level sound effect
            StartCoroutine(FadeInAndLoadScene());
        }
    }

    private IEnumerator FadeInAndLoadScene()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration;
            overlayImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        // Wait for a while
        yield return new WaitForSeconds(transitionDelay);
        // Load next scene
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
}