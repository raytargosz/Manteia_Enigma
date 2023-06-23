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

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3F;

    private void LateUpdate()
    {
        // Smoothly transition to the target position
        Vector3 targetPosition = playerTransform.position + offsetPosition;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / transitionSpeed);

        // Apply rotation offset (consider using Quaternion.Lerp or Slerp for smoother rotation)
        Quaternion targetRotation = Quaternion.Euler(offsetRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, transitionSpeed * Time.deltaTime);
    }

}