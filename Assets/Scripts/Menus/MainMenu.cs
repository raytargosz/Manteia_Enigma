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
    public SceneTransition sceneTransition; // Reference to the SceneTransition script

    private void Start()
    {
        optionsPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        sceneTransition.StartTransition(); // Trigger the fade out and scene load in the SceneTransition script
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
