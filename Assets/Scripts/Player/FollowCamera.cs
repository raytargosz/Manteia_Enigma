using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Camera settings")]
    [SerializeField, Tooltip("Player game object to follow")]
    private Transform playerTransform;
    [SerializeField, Tooltip("Offset position from the player")]
    private Vector3 offsetPosition;
    [SerializeField, Tooltip("Offset rotation")]
    private Vector3 offsetRotation;

    private void Update()
    {
        // Apply position and rotation offset
        Vector3 targetPosition = playerTransform.position + offsetPosition;
        transform.position = targetPosition;

        // Apply rotation offset (consider using Quaternion.Lerp or Slerp for smoother rotation)
        transform.rotation = Quaternion.Euler(offsetRotation);
    }
}
