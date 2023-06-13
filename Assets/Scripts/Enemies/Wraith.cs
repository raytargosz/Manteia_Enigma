using UnityEngine;

public class Wraith : MonoBehaviour
{
    [Header("Wraith Settings")]
    [Tooltip("Movement speed of the Wraith.")]
    [SerializeField] private float speed = 5f;
    [Tooltip("Vertical distance the Wraith can move up and down from its starting position.")]
    [SerializeField] private float moveDistance = 10f;
    [Tooltip("Direction the Wraith should initially move in.")]
    [SerializeField] private bool moveUpFirst = true;

    private float startingY;
    private bool isMovingUp;

    private void Start()
    {
        startingY = transform.position.y;
        isMovingUp = moveUpFirst;
    }

    private void Update()
    {
        float newY = transform.position.y + (isMovingUp ? speed : -speed) * Time.deltaTime;

        if (newY > startingY + moveDistance / 2f)
        {
            newY = startingY + moveDistance / 2f;
            isMovingUp = false;
        }
        else if (newY < startingY - moveDistance / 2f)
        {
            newY = startingY - moveDistance / 2f;
            isMovingUp = true;
        }

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
