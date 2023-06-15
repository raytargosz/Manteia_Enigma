using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;

public class SwordController : MonoBehaviour
{
    [Header("Player Controller")]
    [Tooltip("Reference to the player controller")]
    public PlayerController playerController;

    [Header("Sword Properties")]
    [Tooltip("Speed of the sword swing")]
    public float swingSpeed = 5f;
    [Range(0, 10)]
    [Tooltip("Speed of the sword jab")]
    public float jabSpeed = 5f;
    [Tooltip("Angle by which the sword swings")]
    public float swingAngle = 60f;
    [Tooltip("Distance by which the sword jabs forward")]
    public float jabDistance = 0.5f;

    [Header("Sword Bobbing")]
    [Tooltip("Speed of bobbing while Idle")]
    public float idleBobSpeed = 0.05f;
    [Tooltip("Speed of bobbing while Walking")]
    public float walkBobSpeed = 0.1f;
    [Tooltip("Speed of bobbing while Running")]
    public float runBobSpeed = 0.2f;
    [Tooltip("Speed of bobbing while Jumping")]
    public float jumpBobSpeed = 0.3f;
    [Tooltip("Amount of bobbing (up and down movement)")]
    [Range(0, 1)]
    public float bobAmount = 0.05f;

    [Header("Attack Settings")]
    [Tooltip("Cool down time between each attack")]
    public float attackCooldown = 1.0f;
    private float lastAttackTime = 0f;

    [Header("Animation Curves")]
    [Tooltip("Animation curve for the sword swing")]
    public AnimationCurve swingCurve;
    [Tooltip("Animation curve for the sword jab")]
    public AnimationCurve jabCurve;
    [Tooltip("Animation curve for the sword bob")]
    public AnimationCurve bobCurve;

    [Header("Events")]
    public UnityEvent OnSwingComplete;
    public UnityEvent OnJabComplete;

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
            Bob(idleBobSpeed);
        }
        else if (playerState == PlayerController.PlayerState.Walking && !isSwinging && !isJabbing)
        {
            Bob(walkBobSpeed);
        }
        else if (playerState == PlayerController.PlayerState.Running && !isSwinging && !isJabbing)
        {
            Bob(runBobSpeed);
        }
        else if (playerState == PlayerController.PlayerState.Jumping && !isSwinging && !isJabbing)
        {
            Bob(jumpBobSpeed);
        }

        if (Time.time > lastAttackTime + attackCooldown)
        {
            if (Input.GetMouseButtonDown(0) && !isSwinging && !isJabbing) // Left click
            {
                StartCoroutine(SwingLeft());
                lastAttackTime = Time.time;
            }
            else if (Input.GetMouseButtonDown(1) && !isSwinging && !isJabbing) // Right click
            {
                StartCoroutine(SwingRight());
                lastAttackTime = Time.time;
            }
            else if (Input.mouseScrollDelta.y > 0 && !isSwinging && !isJabbing) // Mouse scroll up
            {
                StartCoroutine(Jab());
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator SwingLeft()
    {
        isSwinging = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - swingAngle);

        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            // Swing the sword to the left using the animation curve
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, swingCurve.Evaluate(t));
            yield return null;
        }

        // Swing back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            transform.localRotation = Quaternion.Slerp(endRotation, startRotation, swingCurve.Evaluate(t));
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
            // Swing the sword to the right using the animation curve
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, swingCurve.Evaluate(t));
            yield return null;
        }

        // Swing back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / swingSpeed)
        {
            transform.localRotation = Quaternion.Slerp(endRotation, startRotation, swingCurve.Evaluate(t));
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
            // Jab the sword forward using the animation curve
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, jabCurve.Evaluate(t));
            yield return null;
        }

        // Move back to initial position
        for (float t = 0; t <= 1; t += Time.deltaTime / jabSpeed)
        {
            transform.localPosition = Vector3.Lerp(endPosition, startPosition, jabCurve.Evaluate(t));
            yield return null;
        }

        isJabbing = false;
    }

    private void Bob(float bobSpeed)
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    // Swing and Jab methods remain the same as the previous version, but you may want to modify them to use the animation curves.
}
