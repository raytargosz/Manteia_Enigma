using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour
{
    // Reference to the player controller
    public PlayerController playerController;

    // Define some properties of the sword
    public float swingSpeed = 5f;
    public float jabSpeed = 5f;
    public float swingAngle = 60f;  // Angle by which the sword swings
    public float jabDistance = 0.5f;  // Distance by which the sword jabs forward

    // Store the initial position and rotation of the sword for resetting after an action
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // For tracking whether an action is being performed
    private bool isSwinging = false;
    private bool isJabbing = false;

    private void Start()
    {
        // Store initial position and rotation
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // Check the player state and adjust the sword's idle animation accordingly
        PlayerController.PlayerState playerState = playerController.GetCurrentState();

        if (playerState == PlayerController.PlayerState.Idle && !isSwinging && !isJabbing)
        {
            // Here we could implement some minor bobbing to match the camera's bobbing
            // You would need to implement a similar logic as your camera's bobbing
            // You might also need to expose some variables from your PlayerController for this to work properly
        }
        else if (playerState == PlayerController.PlayerState.Walking && !isSwinging && !isJabbing)
        {
            // Adjust the sword's position and rotation to match the walking animation
        }
        // Implement similar blocks for Running, Jumping, and Boosting

        // Handle sword actions
        if (Input.GetMouseButtonDown(0) && !isSwinging && !isJabbing) // Left click
        {
            StartCoroutine(SwingLeft());
        }
        else if (Input.GetMouseButtonDown(1) && !isSwinging && !isJabbing) // Right click
        {
            StartCoroutine(SwingRight());
        }
        else if (Input.mouseScrollDelta.y > 0 && !isSwinging && !isJabbing) // Mouse scroll up
        {
            StartCoroutine(Jab());
        }
    }

    private IEnumerator SwingLeft()
    {
        isSwinging = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - swingAngle);

        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            // Swing the sword to the left
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        // Swing back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            transform.localRotation = Quaternion.Lerp(endRotation, startRotation, t);
            yield return null;
        }

        isSwinging = false;
    }

    private IEnumerator SwingRight()
    {
        isSwinging = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + swingAngle);

        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            // Swing the sword to the right
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        // Swing back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            transform.localRotation = Quaternion.Lerp(endRotation, startRotation, t);
            yield return null;
        }

        isSwinging = false;
    }

    private IEnumerator Jab()
    {
        isJabbing = true;

        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startPosition + transform.forward * jabDistance;  // Move the sword forward in the direction it's facing

        for (float t = 0; t <= 1; t += Time.deltaTime / jabSpeed)
        {
            // Jab the sword forward
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // Move back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / jabSpeed)
        {
            transform.localPosition = Vector3.Lerp(endPosition, startPosition, t);
            yield return null;
        }

        isJabbing = false;
    }
}
