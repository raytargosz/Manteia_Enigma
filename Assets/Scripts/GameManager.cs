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
        gameOverPanel.SetActive(true);

        // Lerp the alpha of the panel image to 0.75
        float duration = 2.0f;
        float elapsed = 0;
        Color tempColor = panelImage.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tempColor.a = Mathf.Lerp(0, 0.75f, elapsed / duration);
            panelImage.color = tempColor;
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
        float elapsed = 0;
        Color tempColor = text.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tempColor.a = Mathf.Lerp(0, 1, elapsed / duration);
            text.color = tempColor;
            yield return null;
        }
    }

    private IEnumerator CameraFallAndBounce()
    {
        // Wait for a moment before the camera starts to fall
        yield return new WaitForSeconds(0.5f);

        // Perform the fall and bounce
        float fallTime = 0.5f; // Duration for each fall
        float bounceHeight = 1f; // Initial bounce height
        float originalY = mainCamera.transform.position.y;

        for (int i = 0; i < 4; i++) // Four bounces
        {
            float startTime = Time.time;
            while (Time.time - startTime < fallTime)
            {
                float newY = Mathf.Lerp(originalY, originalY - bounceHeight, (Time.time - startTime) / fallTime);
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, newY, mainCamera.transform.position.z);
                yield return null;
            }
            startTime = Time.time;
            while (Time.time - startTime < fallTime)
            {
                float newY
