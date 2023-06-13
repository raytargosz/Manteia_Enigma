using UnityEngine;
using TMPro;

public class TitleBobbing : MonoBehaviour
{
    public float bobbingSpeed = 1f;
    public float bobbingAmount = 1f;

    private TextMeshProUGUI titleText;
    private Vector3 initialPosition;
    private float elapsedTime;

    private void Start()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        initialPosition = titleText.transform.position;
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime * bobbingSpeed;
        float newY = initialPosition.y + Mathf.Sin(elapsedTime) * bobbingAmount;
        titleText.transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
