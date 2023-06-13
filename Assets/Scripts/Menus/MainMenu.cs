using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button exitButton;
    public GameObject optionsPanel;

    private void Start()
    {
        optionsPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    private void ExitGame()
    {
        Debug.Log("Game is Quitting...");
        Application.Quit();
    }
}
