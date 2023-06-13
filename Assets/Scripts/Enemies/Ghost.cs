using UnityEngine;

public class Ghost : MonoBehaviour
{
    [Header("Ghost Settings")]
    [Tooltip("Movement speed of the Ghost.")]
    [SerializeField] private float speed = 5f;
    [Tooltip("Horizontal distance the Ghost can move right and left from its starting position.")]
    [SerializeField] private float moveDistance = 10f;
    [Tooltip("Direction the Ghost should initially move in.")]
    [SerializeField] private bool moveRightFirst = true;

    private float startingX;
    private bool isMovingRight;

    private void Start()
    {
        startingX = transform.position.x;
        isMovingRight = moveRightFirst;
    }

    private void Update()
    {
        float newX = transform.position.x + (isMovingRight ? speed : -speed) * Time.deltaTime;

        if (newX > startingX + moveDistance / 2f)
        {
            newX = startingX + moveDistance / 2f;
            isMovingRight = false;
        }
        else if (newX < startingX - moveDistance / 2f)
        {
            newX = startingX - moveDistance / 2f;
            isMovingRight = true;
        }

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
