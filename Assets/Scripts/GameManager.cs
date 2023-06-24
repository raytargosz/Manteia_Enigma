using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;  // Reference to the player
    public PlayerController playerController;  // Reference to the player controller script
    public GameObject gameOverPanel;
    public Image panelImage;  // Reference to the panel image
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI deathByText;
    public AudioSource audioSource;  // Reference to the audio source
    public AudioClip deathSFX;  // Sound effect that plays when player dies
    public Camera mainCamera;  // Reference to the main camera
    private FireballSpawner[] spawners;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spawners = FindObjectsOfType<FireballSpawner>();
        gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        foreach (var spawner in spawners)
        {
            spawner.StopSpawning();
        }

        playerController.enabled = false;  // Disable the player controller
        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        // Pause the game
        Time.timeScale = 0f;

        // Enable the panel image and set its alpha to 0
        gameOverPanel.SetActive(true);
        Color panelColor = panelImage.color;
        panelColor.a = 0f;
        panelImage.color = panelColor;

        // Lerp the alpha of the panel image to 0.75
        float duration = 2.0f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            panelColor.a = Mathf.Lerp(0, 0.50f, elapsed / duration);
            panelImage.color = panelColor;
            yield return null;
        }

        // Fade in the game over and death by X text
        StartCoroutine(FadeInText(gameOverText, duration));
        StartCoroutine(FadeInText(deathByText, duration));

        // Play the death sound effect
        audioSource.PlayOneShot(deathSFX);

        // Make the camera fall and bounce
        StartCoroutine(CameraFallAndBounce());
    }


    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        Color textColor = text.color;
        textColor.a = 0f;
        text.color = textColor;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            textColor.a = Mathf.Lerp(0, 1, elapsed / duration);
            text.color = textColor;
            yield return null;
        }
    }

    private IEnumerator CameraFallAndBounce()
    {
        // Wait for a moment before the camera starts to fall
        yield return new WaitForSecondsRealtime(0.5f);

        // Perform the fall and bounce
        float fallTime = 0.5f; // Duration for each fall
        float bounceHeight = 1f; // Initial bounce height
        float originalY = mainCamera.transform.position.y;

        for (int i = 0; i < 4; i++) // Four bounces
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < fallTime)
            {
                float newY = Mathf.Lerp(originalY, originalY - bounceHeight, (Time.realtimeSinceStartup - startTime) / fallTime);
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, newY, mainCamera.transform.position.z);
                yield return null;
            }

            if (i == 3)
            {
                // Disable the camera movement if it has reached the floor
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, originalY - 4 * bounceHeight, mainCamera.transform.position.z);
                break;
            }

            startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < fallTime)
            {
                float newY = Mathf.Lerp(originalY - bounceHeight, originalY, (Time.realtimeSinceStartup - startTime) / fallTime);
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, newY, mainCamera.transform.position.z);
                yield return null;
            }
        }

        // Pause for a moment at the final camera position
        yield return new WaitForSecondsRealtime(2f);

        // Resume the game
        Time.timeScale = 1f;
    }
}
