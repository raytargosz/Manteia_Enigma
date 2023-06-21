using UnityEngine;
using TMPro;

public class HeightTracker : MonoBehaviour
{
    private float startingYPosition;
    public TMP_Text heightText; // assign in the inspector

    // Start is called before the first frame update
    void Start()
    {
        startingYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float currentHeight = transform.position.y - startingYPosition;

        // Check to ensure the height is never below 0
        if (currentHeight < 0)
            currentHeight = 0;

        heightText.text = $"{Mathf.RoundToInt(currentHeight)} M";
    }
}