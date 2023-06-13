using UnityEngine;
using UnityEngine.UI;

public class ButtonBobbing : MonoBehaviour
{
    public float bobbingSpeed = 1f;
    public float bobbingAmount = 1f;

    private Button button;
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private float elapsedTime;
    private float randomOffset;

    private void Start()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.position;
        elapsedTime = 0f;
        randomOffset = Random.Range(-Mathf.PI, Mathf.PI); // Updated range
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime * bobbingSpeed;
        float newY = initialPosition.y + Mathf.Sin(elapsedTime + randomOffset) * bobbingAmount;
        rectTransform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
