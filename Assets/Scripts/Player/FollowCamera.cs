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
    [SerializeField, Tooltip("Speed of the camera's transition")]
    private float transitionSpeed = 5f;

    private void LateUpdate()
    {
        // Apply position and rotation offset
        Vector3 targetPosition = playerTransform.position + offsetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

        // Apply rotation offset (consider using Quaternion.Lerp or Slerp for smoother rotation)
        Quaternion targetRotation = Quaternion.Euler(offsetRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, transitionSpeed * Time.deltaTime);
    }
}