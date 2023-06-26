using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameOver = false;
    public AudioClip restartSFX;
    public GameObject player;
    public PlayerController playerController;
    public GameObject gameOverPanel;
    public Image panelImage;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI deathByText;
    public Button restartButton;
    public AudioSource audioSource;
    public AudioClip deathSFX;
    public Camera mainCamera;
    private string killerObjectTag;
    private bool deathTriggered = false;

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

    private void Update()
    {
        // Check if game is over and if space bar was pressed
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            // Simulate button press
            RestartGame();
        }
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false); // make sure the restart button is hidden at the start
    }

    public void GameOver(string killerTag)
    {
        playerController.enabled = false;
        killerObjectTag = killerTag;

        // Set game over flag to true and disable player collider
        isGameOver = true;
        player.GetComponent<Collider>().enabled = false;

        StartCoroutine(ShowGameOver());

        if (deathTriggered) return;
        deathTriggered = true;
    }

    private IEnumerator ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Play the death SFX once
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(deathSFX);
        }

        StartCoroutine(CameraFall());
        StartCoroutine(FadeIn(panelImage, 0.5f, 2.0f));
        StartCoroutine(FadeInText(gameOverText, 2.0f));
        deathByText.text = "You Died by " + killerObjectTag;
        StartCoroutine(FadeInText(deathByText, 2.0f));

        yield return new WaitForSecondsRealtime(2f);

        restartButton.gameObject.SetActive(true); // reveal the restart button
        restartButton.onClick.AddListener(RestartGame);
    }

    private IEnumerator FadeIn(Image image, float targetAlpha, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            image.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            color.a = Mathf.Lerp(startAlpha, 1, t);
            text.color = color;
            yield return null;
        }
    }

    private IEnumerator CameraFall()
    {
        Vector3 startPosition = mainCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(startPosition, Vector3.down, out hit))
        {
            // Modify the end position to be 25% above the hit point
            Vector3 endPosition = hit.point + new Vector3(0, hit.distance * 0.25f, 0);
            float duration = 1.0f;

            for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
            {
                mainCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            mainCamera.transform.position = endPosition;
        }
    }

    private void RestartGame()
    {
        if (!isGameOver) return;  // If game is not over, return and do nothing
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        audioSource.PlayOneShot(restartSFX);

        // Start fading in the image
        StartCoroutine(FadeIn(panelImage, 1, 0.5f));

        // Wait for 0.5 seconds
        yield return new WaitForSecondsRealtime(0.5f);

        // Reset variables
        isGameOver = false;
        player.GetComponent<Collider>().enabled = true;
        deathTriggered = false;

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
