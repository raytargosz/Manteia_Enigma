using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Normal speed of the player")]
    private float walkSpeed = 3f;
    [SerializeField, Tooltip("Running speed of the player")]
    private float runSpeed = 6f;
    [SerializeField, Tooltip("Jump force for the regular jump")]
    private float jumpForce = 2f;

    [Header("Boost Forces")]
    [SerializeField, Tooltip("Boost force for 'W' key")]
    private float boostForceW = 65f;
    [SerializeField, Tooltip("Boost force for 'A' key")]
    private float boostForceA = 55f;
    [SerializeField, Tooltip("Boost force for 'S' key")]
    private float boostForceS = 45f;
    [SerializeField, Tooltip("Boost force for 'D' key")]
    private float boostForceD = 55f;

    [Header("Field of View")]
    [SerializeField, Tooltip("Default Field of View")]
    private float defaultFOV = 60f;
    [SerializeField, Tooltip("Field of View when running")]
    private float runFOV = 75f;
    [SerializeField, Tooltip("Field of View during boost")]
    private float boostFOV = 90f;
    [SerializeField, Tooltip("Transition speed between FOVs")]
    private float fovTransitionSpeed = 5f;

    [Header("Mouse Look")]
    [SerializeField, Tooltip("Mouse sensitivity")]
    private float mouseSensitivity = 100f;

    [Header("Sounds")]
    [SerializeField, Tooltip("Right footstep sound")]
    private AudioClip rightFootstepSound;
    [SerializeField, Tooltip("Left footstep sound")]
    private AudioClip leftFootstepSound;
    [SerializeField, Tooltip("Jump sound")]
    private AudioClip jumpSound;
    [SerializeField, Tooltip("Land sound")]
    private AudioClip landSound;

    [Header("Footstep Interval")]
    [SerializeField, Tooltip("Interval for footstep sounds when walking")]
    private float walkFootstepInterval = 0.5f;
    [SerializeField, Tooltip("Interval for footstep sounds when running")]
    private float runFootstepInterval = 0.3f;

    [Header("Sound Pitch Variation")]
    [SerializeField, Tooltip("Min and max pitch for walking footsteps")]
    private Vector2 walkPitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField, Tooltip("Min and max pitch for running and jumping")]
    private Vector2 runJumpPitchRange = new Vector2(0.8f, 1.2f);

    [SerializeField, Tooltip("Map of boost sounds to keys")]
    private Dictionary<KeyCode, BoostSound> boostSounds = new Dictionary<KeyCode, BoostSound>();

    [Header("Boost Sounds")]
    [SerializeField, Tooltip("Boost sound for 'W' key")]
    private BoostSound boostSoundW;
    [SerializeField, Tooltip("Boost sound for 'A' key")]
    private BoostSound boostSoundA;
    [SerializeField, Tooltip("Boost sound for 'S' key")]
    private BoostSound boostSoundS;
    [SerializeField, Tooltip("Boost sound for 'D' key")]
    private BoostSound boostSoundD;

    [Header("Lean Effect")]
    [SerializeField, Tooltip("Max lean angle in degrees")]
    private float maxLeanAngle = 10f;
    [SerializeField, Tooltip("Speed of transitioning to max lean")]
    private float leanSpeed = 5f;

    [Header("Gravity Control")]
    [SerializeField, Tooltip("Gravity multiplier after boosting")]
    private float boostedGravityMultiplier = 2f;

    // Non-Serialized Fields
    private CharacterController controller;
    private Vector3 velocity;
    private Camera playerCamera;
    private AudioSource footstepSource, actionSource;
    private float originalGravity;
    private Dictionary<KeyCode, float> boostForces;

    private bool isGrounded;
    private bool isRunning = false;
    private bool canBoost = false;
    private bool rightFootNext = true;
    private bool wasGrounded;
    private bool hasJumped = false;
    private bool isBoosting = false;
    private Vector3 boostDirection = Vector3.zero;
    private bool isFallingAfterBoost = false;

    private float yRotation = 0f;
    private float xRotation = 0f;
    private float footstepCounter = 0f;

    void Start()
    {
        InitializePlayer();
        isGrounded = controller.isGrounded;
        // Populate boostSounds
        boostSounds[KeyCode.W] = boostSoundW;
        boostSounds[KeyCode.A] = boostSoundA;
        boostSounds[KeyCode.S] = boostSoundS;
        boostSounds[KeyCode.D] = boostSoundD;

        originalGravity = Physics.gravity.y;

        // Populate boostForces
        boostForces = new Dictionary<KeyCode, float>
{
    {KeyCode.W, boostForceW},
    {KeyCode.A, boostForceA},
    {KeyCode.S, boostForceS},
    {KeyCode.D, boostForceD}
};

    }

    void Update()
    {
        ProcessGroundState();
        ProcessPlayerMovement();
        ProcessPlayerFOV();
        ProcessMouseLook();
        ProcessJumpInput();
        ProcessFootsteps();
    }

    private void InitializePlayer()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        AudioSource[] sources = GetComponents<AudioSource>();
        footstepSource = sources[0];
        actionSource = sources[1];
        playerCamera.fieldOfView = defaultFOV;

        Debug.Log("Controller: " + (controller != null));
        Debug.Log("PlayerCamera: " + (playerCamera != null));
        Debug.Log("FootstepSource: " + (footstepSource != null));
        Debug.Log("ActionSource: " + (actionSource != null));

        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ProcessGroundState()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        if (!wasGrounded && isGrounded && hasJumped) // Player has landed after a jump
        {
            actionSource.pitch = Random.Range(runJumpPitchRange.x, runJumpPitchRange.y);
            actionSource.PlayOneShot(landSound);
            hasJumped = false; // Reset the hasJumped flag after landing
        }
        wasGrounded = isGrounded;
    }

    private void ProcessPlayerMovement()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        isRunning = Input.GetKey(KeyCode.LeftShift) ? true : false;

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Store the move direction for use in boosting
        if (move != Vector3.zero)
        {
            boostDirection = move.normalized;
        }

        // Lean effect
        if (isRunning || isBoosting) // Apply lean only when running or boosting
        {
            float targetLeanAngle = 0f;
            if (move != Vector3.zero)  // If player is moving
            {
                if (Input.GetAxis("Horizontal") > 0)  // Moving right
                {
                    targetLeanAngle = -maxLeanAngle;
                }
                else if (Input.GetAxis("Horizontal") < 0)  // Moving left
                {
                    targetLeanAngle = maxLeanAngle;
                }
            }
            float currentLeanAngle = playerCamera.transform.localEulerAngles.z;
            float newLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, leanSpeed * Time.deltaTime);
            playerCamera.transform.localEulerAngles = new Vector3(playerCamera.transform.localEulerAngles.x, playerCamera.transform.localEulerAngles.y, newLeanAngle);
        }
    }

    private void ProcessPlayerFOV()
    {
        float targetFOV = isRunning ? runFOV : defaultFOV;
        if (!isGrounded && canBoost) targetFOV = boostFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
    }

    private void ProcessMouseLook()
    {
        float mouseLookX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseLookY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseLookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseLookX);

        // Debug statement:
        Debug.Log("ProcessMouseLook called. Mouse X: " + mouseLookX + ", Mouse Y: " + mouseLookY);
    }

    private void ProcessJumpInput()
    {
        KeyCode boostKey = KeyCode.None;

        foreach (var entry in boostForces)
        {
            if (Input.GetKey(entry.Key))
            {
                boostKey = entry.Key;
                break;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                canBoost = true;
                hasJumped = true;
                actionSource.pitch = Random.Range(runJumpPitchRange.x, runJumpPitchRange.y);
                actionSource.PlayOneShot(jumpSound);
            }
            else if (canBoost && !isGrounded && boostKey != KeyCode.None)
            {
                canBoost = false;

                BoostSound boostSound = boostSounds[boostKey];
                actionSource.pitch = Random.Range(boostSound.pitchRange.x, boostSound.pitchRange.y);
                actionSource.PlayOneShot(boostSound.clip);

                StartCoroutine(Boost(boostKey));
            }
        }

        if (isFallingAfterBoost)  // Only apply increased gravity when falling after boost
        {
            velocity.y += Physics.gravity.y * boostedGravityMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        // reset gravity when grounded and set the flag isFallingAfterBoost to false
        if (isGrounded)
        {
            Physics.gravity = new Vector3(0, originalGravity, 0);
            isFallingAfterBoost = false;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator Boost(KeyCode key)
    {
        isBoosting = true;
        float boostTimer = 0.33f;  // Adjust this value to control the duration of the boost

        while (boostTimer > 0)
        {
            velocity += boostDirection * boostForces[key] * Time.deltaTime;
            boostTimer -= Time.deltaTime;
            yield return null;
        }

        isBoosting = false;
        velocity = new Vector3(0, velocity.y, 0);  // Reset horizontal velocity
        isFallingAfterBoost = true;  // Set this flag to true after the boost
    }

    [System.Serializable]
    public class BoostSound
    {
        public AudioClip clip;
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    }

    private void ProcessFootsteps()
    {
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        if (isGrounded && move.magnitude > 0)
        {
            footstepCounter -= Time.deltaTime;
            if (footstepCounter <= 0)
            {
                AudioClip nextFootstepSound = rightFootNext ? rightFootstepSound : leftFootstepSound;
                footstepSource.pitch = isRunning ? Random.Range(runJumpPitchRange.x, runJumpPitchRange.y) : Random.Range(walkPitchRange.x, walkPitchRange.y);
                footstepSource.PlayOneShot(nextFootstepSound);
                rightFootNext = !rightFootNext;
                footstepCounter = isRunning ? runFootstepInterval : walkFootstepInterval;
            }
        }
    }
}