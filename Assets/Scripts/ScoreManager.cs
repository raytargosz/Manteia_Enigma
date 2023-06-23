using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance
    public static ScoreManager instance;

    // Player's score
    private int score;

    private void Awake()
    {
        // If an ScoreManager instance exists and it's not this one, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else // Else this instance becomes the singleton instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Makes the ScoreManager persistent across scenes
        }
    }

    // Add to score
    public void AddScore(int valueToAdd)
    {
        score += valueToAdd;
    }

    // Get the current score
    public int GetScore()
    {
        return score;
    }
}