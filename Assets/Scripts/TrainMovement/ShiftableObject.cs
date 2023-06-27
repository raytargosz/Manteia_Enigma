using System.Collections;
using UnityEngine;

public class ShiftableObject : MonoBehaviour
{
    [Header("Shiftable Object Settings")]
    [Tooltip("Maximum distance object can move from its original position along each axis")]
    public Vector3 maxMovement;

    [Tooltip("Minimum and maximum time object can spend moving in one direction")]
    public Vector2 minMaxMoveTime;

    [Tooltip("Minimum and maximum time object can spend waiting before starting to move")]
    public Vector2 minMaxWaitTime;

    [Tooltip("Dominant axis for movement. 0 = X, 1 = Z")]
    [Range(0, 1)] public int dominantAxis;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private float moveSpeed;

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            // Wait for a random time
            yield return new WaitForSeconds(Random.Range(minMaxWaitTime.x, minMaxWaitTime.y));

            // Choose new target position
            targetPosition = originalPosition + new Vector3(
                Random.Range(-maxMovement.x, maxMovement.x) * (dominantAxis == 0 ? 1 : 0.5f),
                0,
                Random.Range(-maxMovement.z, maxMovement.z) * (dominantAxis == 1 ? 1 : 0.5f));

            // Choose new move speed
            moveSpeed = Vector3.Distance(transform.position, targetPosition) / Random.Range(minMaxMoveTime.x, minMaxMoveTime.y);

            // Move towards target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
