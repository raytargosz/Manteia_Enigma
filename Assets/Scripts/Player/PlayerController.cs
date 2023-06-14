using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    [SerializeField, Tooltip("Boost force for 'W' key")]
    private float boostForceW = 65f;
    [SerializeField, Tooltip("Boost force for 'A' key")]
    private float boostForceA = 55f;
    [SerializeField, Tooltip("Boost force for 'S' key")]
    private float boostForceS = 45f;
    [SerializeField, Tooltip("Boost force for 'D' key")]
    private float boostForceD = 55f;

    [Header("Camera Bobbing")]
    [SerializeField, Tooltip("Bobbing speed when idle")]
    private float idleBobbingSpeed = 0.5f;
    [SerializeField, Tooltip("Bobbing speed when walking")]
    private float walkBobbingSpeed = 0.8f;
    [SerializeField, Tooltip("Bobbing speed when running")]
    private float runBobbingSpeed = 1.2f;
    [SerializeField, Tooltip("Bobbing speed when boosting")]
    private float boostBobbingSpeed = 1.5f;
    [SerializeField, Tooltip("Bobbing amount")]
    private float bobbingAmount = 0.05f;

    [Header("Gravity Control")]
    [SerializeField, Tooltip("Gravity multiplier after boosting")]
    private float boostedGravityMultiplier = 2f;

    [Header("Mouse Look")]
    [SerializeField, Tooltip("Mouse sensitivity")]
    private float mouseSensitivity = 100f;

    [Header("Field of View")]
    [SerializeField, Tooltip("Default Field of View")]
    private float defaultFOV = 60f;
    [SerializeField, Tooltip("Field of View when running")]
    private float runFOV = 75f;
    [SerializeField, Tooltip("Field of View during boost")]
    private float boostFOV = 90f;
    [SerializeField, Tooltip("Transition speed between FOVs")]
    private float fovTransitionSpeed = 5f;

    [Header("Lean Effect")]
    [SerializeField, Tooltip("Max lean angle in degrees")]
    private float maxLeanAngle = 10f;
    [SerializeField, Tooltip("Speed of transitioning to max lean")]
    private float leanSpeed = 5f;

    [Header("Sounds")]
    [SerializeField, Tooltip("Right footstep sound")]
    private AudioClip rightFootstepSound;
    [SerializeField, Tooltip("Left footstep sound")]
    private AudioClip leftFootstepSound;
    [SerializeField, Tooltip("Jump sound")]
    private AudioClip jumpSound;
    [SerializeField, Tooltip("Land sound")]
    private AudioClip landSound;
    [SerializeField, Tooltip("Boost sound for 'W' key")]
    private BoostSound boostSoundW;
    [SerializeField, Tooltip("Boost sound for 'A' key")]
    private BoostSound boostSoundA;
    [SerializeField, Tooltip("Boost sound for 'S' key")]
    private BoostSound boostSoundS;
    [SerializeField, Tooltip("Boost sound for 'D' key")]
    private BoostSound boostSoundD;

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

    [Header("Player Stats")]
    [SerializeField, Tooltip("Max stamina")]
    private float maxStamina = 100f;
    private float currentStamina;

    [SerializeField, Tooltip("Max boost")]
    private float maxBoost = 100f;
    private float currentBoost;

    [SerializeField, Tooltip("Stamina depletion rate when running")]
    private float staminaDepletionRate = 10f;

    [SerializeField, Tooltip("Boost depletion rate")]
    private float boostDepletionRate = 33.3f; // Amount of boost depleted per second when boosting

    [SerializeField, Tooltip("Stamina cost for jumping")]
    private float staminaJumpCost = 20f; // Stamina cost for jumping

    [SerializeField, Tooltip("Stamina regeneration rate")]
    private float staminaRegenSpeed = 20f; // Stamina regenerated per second when not running

    [SerializeField, Tooltip("Boost regeneration rate")]
    private float boostRegenSpeed = 10f; // Boost regenerated per second when not boosting

    [SerializeField, Tooltip("Max health")]
    private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField]
    private Scrollbar healthBar;
    [SerializeField]
    private Scrollbar staminaBar;
    [SerializeField]
    private Scrollbar boostBar;
    [SerializeField]
    private Image healthHandleImage;
    [SerializeField]
    private Image staminaHandleImage;
    [SerializeField]
    private Image boostHandleImage;

    [SerializeField, Tooltip("Map of boost sounds to keys")]
    private Dictionary<KeyCode, BoostSound> boostSounds = new Dictionary<KeyCode, BoostSound>();

    // Non-Serialized Fields
    private CharacterController controller;
    private Vector3 velocity;
    private Camera playerCamera;
    private AudioSource footstepSource, actionSource;
    private float originalGravity;
    private Dictionary<KeyCode, float> boostForces;

    private bool canJump;
    private bool isGrounded;
    private bool isRunning = false;
    private bool canBoost = false;
    private bool rightFootNext = true;
    private bool wasGrounded;
    private bool hasJumped = false;
    private bool isBoosting = false;
    private Vector3 boostDirection = Vector3.zero;
    private bool isFallingAfterBoost = false;
    private float bobbingCounter = 0f;
    private float defaultCameraYPos;

    // Add a variable to track the current player state
    private PlayerState currentState = PlayerState.Idle;
    private enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Boosting,
        Grounded
    }

    private float yRotation = 0f;
    private float xRotation = 0f;
    private float footstepCounter = 0f;

    // Public getters for current health, stamina, and boost
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    public float CurrentStamina
    {
        get { return currentStamina; }
    }

    public float CurrentBoost
    {
        get { return currentBoost; }
    }

    void Start()
    {
        InitializePlayer();
        InitializeSoundAndForces();

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBoost = maxBoost;

        healthBar.size = currentHealth / maxHealth;
        staminaBar.size = currentStamina / maxStamina;
        boostBar.size = currentBoost / maxBoost;

        healthBar.value = currentHealth;
        staminaBar.value = currentStamina;
        boostBar.value = currentBoost;
    }

    private void InitializePlayer()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        footstepSource = GetComponents<AudioSource>()[0];
        actionSource = GetComponents<AudioSource>()[1];
        playerCamera.fieldOfView = defaultFOV;

        Debug.Log($"Controller: {controller != null}");
        Debug.Log($"PlayerCamera: {playerCamera != null}");
        Debug.Log($"FootstepSource: {footstepSource != null}");
        Debug.Log($"ActionSource: {actionSource != null}");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeSoundAndForces()
    {
        isGrounded = controller.isGrounded;
        originalGravity = Physics.gravity.y;
        boostSounds = new Dictionary<KeyCode, BoostSound>
    {
        {KeyCode.W, boostSoundW},
        {KeyCode.A, boostSoundA},
        {KeyCode.S, boostSoundS},
        {KeyCode.D, boostSoundD}
    };
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

        HandleBobbing();

        // Handle stamina and boost
        if (currentState == PlayerState.Running && isGrounded) // Ensure player is on the ground before draining stamina when running
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
        }
        else if (currentState == PlayerState.Boosting && currentBoost == maxBoost)
        {
            currentBoost -= boostDepletionRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRegenSpeed * Time.deltaTime;
            if (isGrounded) currentBoost += boostRegenSpeed * Time.deltaTime;
        }

        // Check if boost is fully regenerated before allowing it to be used
        canBoost = (currentBoost >= maxBoost) ? true : false;

        // Check if player can jump based on stamina
        canJump = (currentStamina > 0) ? true : false;

        // Keep values within their limits
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        currentBoost = Mathf.Clamp(currentBoost, 0, maxBoost);

        // Update UI
        staminaBar.size = currentStamina / maxStamina;
        boostBar.size = currentBoost / maxBoost;

        // Set the alpha values for the handle images
        staminaHandleImage.color = new Color(staminaHandleImage.color.r, staminaHandleImage.color.g, staminaHandleImage.color.b, staminaBar.size > 0 ? 1 : 0);
        boostHandleImage.color = new Color(boostHandleImage.color.r, boostHandleImage.color.g, boostHandleImage.color.b, boostBar.size > 0 ? 1 : 0);
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
        // Initialize a variable to hold the moveSpeed
        float moveSpeed;

        // Check the current state
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Walking:
                moveSpeed = walkSpeed;
                break;
            case PlayerState.Running:
                // Only run if there's enough stamina
                moveSpeed = (currentStamina > 0) ? runSpeed : walkSpeed;
                break;
            default:
                moveSpeed = walkSpeed;
                break;
        }

        // Apply movement based on the moveSpeed
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * moveSpeed * Time.deltaTime);

        // If player is moving, update the boost direction
        if (move != Vector3.zero)
        {
            boostDirection = move.normalized;
        }

        // Update the player's current state
        currentState = (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) ? PlayerState.Running : PlayerState.Walking;

        // If there's no movement, set the state to Idle
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) currentState = PlayerState.Idle;

        // If the player is jumping, set the state to Boosting
        if (Input.GetButtonDown("Jump") && canBoost && !isGrounded) currentState = PlayerState.Boosting;
    }

    private void ProcessPlayerFOV()
    {
        float targetFOV = defaultFOV;

        if (isBoosting)
        {
            targetFOV = boostFOV;
        }
        else if (isRunning && controller.velocity.magnitude > 0.1f)
        {
            targetFOV = runFOV;
        }

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
                if (canJump) // Check if the player has enough stamina to jump
                {
                    velocity.y += Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                    canBoost = true;
                    hasJumped = true;
                    actionSource.pitch = Random.Range(runJumpPitchRange.x, runJumpPitchRange.y);
                    actionSource.PlayOneShot(jumpSound);

                    // Decrease stamina for the jump
                    currentStamina -= staminaJumpCost;
                    currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                }
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
        float boostTimer = 0.5f;  // Adjust this value to control the duration of the boost

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
                footstepSource.pitch = (currentState == PlayerState.Running) ? Random.Range(runJumpPitchRange.x, runJumpPitchRange.y) : Random.Range(walkPitchRange.x, walkPitchRange.y);
                footstepSource.PlayOneShot(nextFootstepSound);
                rightFootNext = !rightFootNext;
                footstepCounter = (currentState == PlayerState.Running) ? runFootstepInterval : walkFootstepInterval;
            }
        }
    }

    private void HandleBobbing()
    {
        float bobbingSpeed = 0f;

        if (currentState == PlayerState.Idle || controller.velocity.magnitude > 0.1f) // Add a condition to check if player is moving for states other than idle
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                    bobbingSpeed = idleBobbingSpeed;
                    break;
                case PlayerState.Walking:
                    bobbingSpeed = walkBobbingSpeed;
                    break;
                case PlayerState.Running:
                    bobbingSpeed = runBobbingSpeed;
                    break;
                case PlayerState.Boosting:
                    bobbingSpeed = boostBobbingSpeed;
                    break;
            }

            bobbingCounter += Time.deltaTime * bobbingSpeed;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraYPos + Mathf.Sin(bobbingCounter) * bobbingAmount,
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            // Stop bobbing when not moving
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                Mathf.Lerp(playerCamera.transform.localPosition.y, defaultCameraYPos, Time.deltaTime * bobbingSpeed),
                playerCamera.transform.localPosition.z
            );
        }
    }
}