using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false); // make sure the restart button is hidden at the start
    }

    public void GameOver(string killerTag)
    {
        playerController.enabled = false;
        killerObjectTag = killerTag;
        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        audioSource.PlayOneShot(deathSFX);

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
        Vector3 endPosition = new Vector3(startPosition.x, 0f, startPosition.z);

        float duration = 1.0f;

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // This restarts the current scene
    }
}