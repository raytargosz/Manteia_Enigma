using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
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
        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Color tempColor = gameOverText.color;
        float elapsed = 0;
        float duration = 2.0f;  // fade duration

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tempColor.a = Mathf.Lerp(0, 1, elapsed / duration);
            gameOverText.color = tempColor;
            yield return null;
        }
    }
}
