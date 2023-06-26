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
    public AudioClip deathMusic;  // new audio clip for death music
    public GameObject player;
    public PlayerController playerController;
    public GameObject gameOverPanel;
    public Image panelImage;
    public Image restartPanelImage;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI deathByText;
    public Button restartButton;
    public AudioSource audioSource;
    public AudioSource[] audioSourcesToKeep;  // new array for keeping some audio sources active
    public GameObject[] gameObjectsToDisable;
    public AudioClip deathSFX;
    public Camera mainCamera;
    public float restartDelay = 0.5f;  // new delay before restarting the game
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
        if (deathTriggered) return;  // If death has already been triggered, return immediately
        deathTriggered = true;

        playerController.enabled = false;
        killerObjectTag = killerTag;

        // Set game over flag to true and disable player collider
        isGameOver = true;
        player.GetComponent<Collider>().enabled = false;

        // Disable the game objects
        foreach (var go in gameObjectsToDisable)
        {
            go.SetActive(false);
        }

        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Play the death SFX once
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(deathSFX);
        }

        audioSource.clip = deathMusic;
        audioSource.loop = true;
        audioSource.Play();

        // Mute all audio sources except for the ones to keep
        foreach (var audioSource in FindObjectsOfType<AudioSource>())
        {
            if (!System.Array.Exists(audioSourcesToKeep, element => element == audioSource))
            {
                audioSource.mute = true;
            }
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
        // Unsubscribe the RestartGame from the restart button's click event
        restartButton.onClick.RemoveListener(RestartGame);

        audioSource.PlayOneShot(restartSFX);

        // Start fading in the restart panel
        StartCoroutine(FadeIn(restartPanelImage, 1, restartDelay));

        // Wait for the delay before restarting the game
        yield return new WaitForSecondsRealtime(restartDelay);

        // Fade out death music
        StartCoroutine(FadeOutAudio(audioSource, restartDelay));

        // Reset variables
        isGameOver = false;
        player.GetComponent<Collider>().enabled = true;
        deathTriggered = false;

        // Wait for the audio fade out before reloading the scene
        yield return new WaitForSecondsRealtime(restartDelay);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
