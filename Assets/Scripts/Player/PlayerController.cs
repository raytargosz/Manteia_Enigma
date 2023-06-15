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

    [SerializeField, Tooltip("Map of boost sounds to keys")]
    private Dictionary<KeyCode, BoostSound> boostSounds = new Dictionary<KeyCode, BoostSound>();

    [SerializeField]
    private PlayerUIController playerUIController;

    // Non-Serialized Fields
    private CharacterController controller;
    private Vector3 velocity;
    private Camera playerCamera;
    private AudioSource footstepSource, actionSource;
    private float originalGravity;
    private Dictionary<KeyCode, float> boostForces;

    private bool hasBoosted = false;
    private bool canRun = true;
    private bool canJump = true;
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

    public float jumpHeight = 2.0f;
    // Variables for gravity
    public float gravity = -9.81f; // Standard gravity value

    // Variables for boosting
    public float boostSpeed = 2.0f; // Adjust as needed
    public bool boostAvailable = true; // Adjust as needed


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

        // Ground check
        bool isGrounded = IsPlayerGrounded();

        // Handle stamina
        if (currentState == PlayerState.Running && isGrounded && currentStamina > 0) // Ensure player is on the ground before draining stamina when running
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenSpeed * Time.deltaTime;
            }
        }

        playerUIController.UpdateStaminaBar(currentStamina, maxStamina);

        // If stamina hits zero, player can't run or jump until stamina is full
        if (currentStamina <= 0)
        {
            canRun = false;
            canJump = false;
        }
        else if (currentStamina >= maxStamina)
        {
            canRun = true;
            canJump = true;
        }

        // Handle boost
        if (currentState == PlayerState.Boosting && currentBoost >= 0)
        {
            currentBoost -= boostDepletionRate * Time.deltaTime;
        }
        else if (isGrounded) // boost regenerates only when player is on the ground
        {
            currentBoost += boostRegenSpeed * Time.deltaTime;
        }

        // Check if boost is fully regenerated before allowing it to be used
        canBoost = (currentBoost >= maxBoost) ? true : false;

        // Check if player can jump based on stamina
        canJump = (currentStamina > 0) ? true : false;

        // Keep values within their limits
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        currentBoost = Mathf.Clamp(currentBoost, 0, maxBoost);

        // Update UI
        playerUIController.UpdateBoostBar(currentBoost, maxBoost);

        // Adjust Stamina bar
        float staminaAlpha = currentStamina > 0 ? 1f : 0f;
        playerUIController.SetStaminaHandleAlpha(staminaAlpha);

        // Adjust Boost bar
        float boostAlpha = currentBoost > 0 ? 1f : 0f;
        playerUIController.SetBoostHandleAlpha(boostAlpha);

        // If boost is regenerating, make the boost bar fill gradually
        if (currentBoost < maxBoost && isGrounded)
        {
            // You will need to add a method in PlayerUIController to handle this
            playerUIController.LerpBoostBarSize(currentBoost / maxBoost, boostRegenSpeed * Time.deltaTime);
        }
        else
        {
            // The boost bar size is already updated at the top of the function
        }

        // Handle Health
        float healthRatio = currentHealth / maxHealth;
        playerUIController.UpdateHealthBar(currentHealth, maxHealth);

        // Handle health alpha
        float healthAlpha = healthRatio > 0 ? 1f : 0f;
        playerUIController.SetHealthHandleAlpha(healthAlpha);
    }
    bool IsPlayerGrounded()
    {
        // The length of the ray
        float distanceToGround = 0.1f;

        // Raycast down from the player's position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToGround))
        {
            // Check if the GameObject hit by the ray has the "Ground" tag
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    private void ProcessPlayerJumpAndBoost()
    {
        if (isGrounded && Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            hasJumped = true; // Set hasJumped to true after a jump
        }

        // Add the condition that boosting can only happen after jumping
        if (Input.GetKey(KeyCode.Space) && hasJumped && !isBoosting && boostAvailable)
        {
            velocity += boostDirection * boostSpeed; // Adjust the velocity by the boost speed in the direction of movement
            isBoosting = true;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
                               // As the player has landed, boosting is also reset
            isBoosting = false;
        }
        wasGrounded = isGrounded;
    }

    private void ProcessPlayerMovement()
    {
        float moveSpeed;

        // Only allow running if stamina is not exhausted
        if ((currentStamina > 0 || !Input.GetKey(KeyCode.LeftShift)) && canRun)
        {
            moveSpeed = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) ? runSpeed : walkSpeed;
            currentState = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) ? PlayerState.Running : PlayerState.Walking;
        }
        else
        {
            moveSpeed = walkSpeed;
            currentState = PlayerState.Walking;
        }

        // Change the state to Idle if there's no movement
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) currentState = PlayerState.Idle;

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

                    // If stamina is zero, disallow running
                    if (currentStamina <= 0)
                    {
                        canRun = false;
                    }
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