using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Character Controller
    private CharacterController controller;

    // Camera Settings
    private Camera playerCamera;
    private float defaultCameraYPos;
    private float bobbingCounter = 0f;

    // Player Movement Settings
    [Header("Movement")]
    [SerializeField, Tooltip("Normal speed of the player")]
    private float walkSpeed = 3f;
    [SerializeField, Tooltip("Running speed of the player")]
    private float runSpeed = 6f;
    [SerializeField, Tooltip("Jump force for the regular jump")]
    private float jumpForce = 2f;
    public float jumpHeight = 2.0f;
    public float moveSpeed = 5f;
    [Tooltip("The speed at which the player falls after a jump")]
    public float fallSpeed = 2.5f;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool hasJumped = false;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isRunning = false;
    private bool canRun = true;
    private bool canJump = true;
    private Vector3 velocity;
    private float originalGravity;
    private bool rightFootNext = true;

    // Jump Settings
    [SerializeField, Tooltip("Max time the jump button can be held")]
    private float jumpTime = 0.35f;
    private float jumpTimeCounter;

    // Boost Settings
    [Header("Boost Variables")]
    [SerializeField, Tooltip("Gravity multiplier after boosting")]
    private float boostedGravityMultiplier = 2f;
    public float gravity = -9.81f;
    private bool isBoosting = false;
    private bool canBoost = false;
    private Vector3 boostDirection = Vector3.zero;
    private bool isFallingAfterBoost = false;
    private bool hasBoosted = false;
    private bool boostUsed = false; // This flag checks whether the boost has been used or not
    private float boostCooldownTimer = 0f; // This timer keeps track of the cooldown state
    private bool boostAvailable = true; // This flag checks whether the boost is available or not
    private float boostSpeed = 20f; // Speed of the boost. You can adjust this to your needs
    private float boostYForce = 1f; // Vertical force applied during a boost. Adjust as needed
    [SerializeField, Tooltip("Boost force for 'W' key")]
    private float boostForceW = 65f;
    [SerializeField, Tooltip("Boost force for 'A' key")]
    private float boostForceA = 55f;
    [SerializeField, Tooltip("Boost force for 'S' key")]
    private float boostForceS = 45f;
    [SerializeField, Tooltip("Boost force for 'D' key")]
    private float boostForceD = 55f;
    private Dictionary<KeyCode, float> boostForces;
    [SerializeField]
    private float boostCooldown = 3f;

    // Camera Bobbing
    [Header("Camera Bobbing")]
    [SerializeField, Tooltip("Bobbing speed when idle")]
    private float idleBobbingSpeed = 0.5f;
    [SerializeField]
    private float idleBobbingAmount = 0.05f;
    [SerializeField, Tooltip("Bobbing speed when walking")]
    private float walkBobbingSpeed = 0.8f;
    [SerializeField]
    private float walkBobbingAmount = 0.1f;
    [SerializeField, Tooltip("Bobbing speed when running")]
    private float runBobbingSpeed = 1.2f;
    [SerializeField]
    private float runBobbingAmount = 0.15f;
    [SerializeField, Tooltip("Bobbing speed when boosting")]
    private float boostBobbingSpeed = 1.5f;
    [SerializeField]
    private float boostBobbingAmount = 0.2f;

    // Mouse Look
    [Header("Mouse Look")]
    [SerializeField, Tooltip("Mouse sensitivity")]
    private float mouseSensitivity = 100f;

    // Field of View
    [Header("Field of View")]
    [SerializeField, Tooltip("Default Field of View")]
    private float defaultFOV = 60f;
    [SerializeField, Tooltip("Field of View when running")]
    private float runFOV = 75f;
    [SerializeField, Tooltip("Field of View during boost")]
    private float boostFOV = 90f;
    [SerializeField, Tooltip("Transition speed between FOVs")]
    private float fovTransitionSpeed = 5f;

    // Lean Effect
    [Header("Lean Effect")]
    [SerializeField, Tooltip("Max lean angle in degrees")]
    private float maxLeanAngle = 10f;
    [SerializeField, Tooltip("Speed of transitioning to max lean")]
    private float leanSpeed = 5f;

    // Sound Settings
    [Header("Sound Settings")]

    [Tooltip("AudioSource for footstep sounds")]
    public AudioSource footstepSource;

    [Tooltip("AudioSource for action sounds")]
    public AudioSource actionSource;

    [Tooltip("AudioSource for turn sounds")]
    public AudioSource turnSource;

    [Tooltip("Array of different surface footstep sounds")]
    public SurfaceFootstepSFX[] footstepSFXs;

    [Tooltip("Indicates whether the player is currently turning")]
    private bool isTurning = false;

    [SerializeField, Tooltip("Sound clip for jump action")]
    private AudioClip jumpSound;

    [SerializeField, Tooltip("Sound clip for landing action")]
    private AudioClip landSound;

    [SerializeField, Tooltip("Boost sound clip for 'W' key")]
    private BoostSound boostSoundW;

    [SerializeField, Tooltip("Boost sound clip for 'A' key")]
    private BoostSound boostSoundA;

    [SerializeField, Tooltip("Boost sound clip for 'S' key")]
    private BoostSound boostSoundS;

    [SerializeField, Tooltip("Boost sound clip for 'D' key")]
    private BoostSound boostSoundD;

    [Tooltip("Dictionary mapping key codes to their associated boost sounds")]
    private Dictionary<KeyCode, BoostSound> boostSounds = new Dictionary<KeyCode, BoostSound>();


    // Footstep Interval
    [Header("Footstep Interval")]
    [SerializeField, Tooltip("Interval for footstep sounds when walking")]
    private float walkFootstepInterval = 0.5f;
    [SerializeField, Tooltip("Interval for footstep sounds when running")]
    private float runFootstepInterval = 0.3f;
    public float turnCooldown = 0.5f;
    private float lastTurnTime = 0f;

    // Sound Pitch Variation
    [Header("Sound Pitch Variation")]
    [SerializeField, Tooltip("Min and max pitch for walking footsteps")]
    private Vector2 walkPitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField, Tooltip("Min and max pitch for running and jumping")]
    private Vector2 runJumpPitchRange = new Vector2(0.8f, 1.2f);

    // Player Stats
    [Header("Player Stats")]
    [SerializeField, Tooltip("Max stamina")]
    private float maxStamina = 100f;
    private float currentStamina;
    [SerializeField, Tooltip("Max boost")]
    private float maxBoost = 100f;
    private float currentBoost;
    [SerializeField, Tooltip("Max health")]
    private float maxHealth = 100f;
    private float currentHealth;
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

    // Player UI
    [SerializeField]
    private PlayerUIController playerUIController;

    // Sound SFX Structure
    // This structure is used to organize the various audio clips and their pitch variations 
    // for different player actions and surface interactions.
    // Sound SFX Structure
    [System.Serializable]
    public struct SurfaceFootstepSFX
    {
        [Tooltip("Tag to identify the type of surface (e.g., 'Grass', 'Concrete')")]
        public string tag;

        [Header("Sounds")]
        [Tooltip("Array of sound clips for walking footsteps")]
        public AudioClip[] walkSounds;

        [Tooltip("Array of sound clips for running footsteps")]
        public AudioClip[] runSounds;

        [Tooltip("Array of sound clips for jumps")]
        public AudioClip[] jumpSounds;

        [Tooltip("Array of sound clips for landing from a jump")]
        public AudioClip[] landSounds;

        [Tooltip("Array of sound clips for turning")]
        public AudioClip[] turnSounds; // Turn sound is now an array

        [Header("Pitch Variation")]
        [Tooltip("Range of possible pitch adjustments for walk sounds (to add variety)")]
        public Vector2 walkPitchRange;

        [Tooltip("Range of possible pitch adjustments for run sounds (to add variety)")]
        public Vector2 runPitchRange;

        [Tooltip("Range of possible pitch adjustments for jump sounds (to add variety)")]
        public Vector2 jumpPitchRange;

        [Tooltip("Range of possible pitch adjustments for land sounds (to add variety)")]
        public Vector2 landPitchRange;

        [Tooltip("Range of possible pitch adjustments for turn sounds (to add variety)")]
        public Vector2 turnPitchRange;
    }

    // Add a variable to track the current player state
    private PlayerState currentState = PlayerState.Idle;
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Boosting,
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    private float yRotation = 0f;
    private float xRotation = 0f;
    private float footstepCounter = 0f;

    // Public getters for current health, stamina, and boost
    public float CurrentHealth => currentHealth;
    public float CurrentStamina => currentStamina;
    public float CurrentBoost => currentBoost;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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

        // Handle horizontal movement

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            Jump();
        }

        if (isFalling)
        {
            Fall();
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

        if (!isGrounded)
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
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
    private void Jump()
    {
        // Assuming you're using a Rigidbody or CharacterController for movement
        // Add force or change velocity for a jump
        velocity.y = jumpForce;
        isJumping = true;
    }

    private void Fall()
    {
        // Decrease the y velocity to simulate a fall
        // This would be done differently depending on whether you're using a Rigidbody or CharacterController
        isJumping = false;
        isFalling = true;
        velocity.y -= fallSpeed * Time.deltaTime;
    }

    private void ProcessPlayerJumpAndBoost()
    {
        if (isGrounded && Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            hasJumped = true; // Set hasJumped to true after a jump
        }

        if (isGrounded)
        {
            // Once grounded, start the cooldown timer before boost becomes available
            if (boostUsed && boostCooldownTimer <= 0f)
            {
                boostCooldownTimer = boostCooldown;
            }
            boostUsed = false; // Reset boost usage when grounded
        }

        // Once boost has been used, start counting down the cooldown timer
        if (boostCooldownTimer > 0f)
        {
            boostCooldownTimer -= Time.deltaTime;
            // Only make boost available again once cooldown timer is up
            if (boostCooldownTimer <= 0f)
            {
                boostAvailable = true;
            }
        }

        // Add the condition that boosting can only happen after jumping and when boost is available
        if (Input.GetButton("Boost") && hasJumped && boostAvailable && !boostUsed)
        {
            velocity += boostDirection * boostSpeed; // Adjust the velocity by the boost speed in the direction of movement
            isBoosting = true;
            // After using the boost, make it unavailable and mark it as used
            boostAvailable = false;
            boostUsed = true;
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
            isBoosting = false; // Reset the isBoosting flag after landing
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

    private void ProcessJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump") && canJump)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            velocity.y = jumpForce;  // Set the vertical velocity here
            SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
            footstepSource.pitch = Random.Range(sfx.jumpPitchRange.x, sfx.jumpPitchRange.y);
            footstepSource.PlayOneShot(sfx.jumpSounds[Random.Range(0, sfx.jumpSounds.Length)]);
        }
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private void ProcessLand()
    {
        if (!wasGrounded && isGrounded)
        {
            SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
            footstepSource.pitch = Random.Range(sfx.landPitchRange.x, sfx.landPitchRange.y);
            footstepSource.PlayOneShot(sfx.landSounds[Random.Range(0, sfx.landSounds.Length)]);
        }
    }

    private IEnumerator Boost(KeyCode key)
    {
        isBoosting = true;
        float boostTimer = 0.5f;  // Adjust this value to control the duration of the boost

        Vector3 boostDirectionWithY = new Vector3(boostDirection.x, boostYForce, boostDirection.z); // Creating a new vector with Y value to lift the player upwards

        while (boostTimer > 0)
        {
            velocity += boostDirectionWithY.normalized * boostForces[key] * Time.deltaTime; // Use the modified boost direction
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

        if (isGrounded)
        {
            if (move.magnitude > 0)
            {
                footstepCounter -= Time.deltaTime;
                if (footstepCounter <= 0)
                {
                    SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
                    AudioClip nextFootstepSound = rightFootNext ? sfx.walkSounds[Random.Range(0, sfx.walkSounds.Length)] : sfx.runSounds[Random.Range(0, sfx.runSounds.Length)];
                    footstepSource.pitch = (currentState == PlayerState.Running) ? Random.Range(sfx.runPitchRange.x, sfx.runPitchRange.y) : Random.Range(sfx.walkPitchRange.x, sfx.walkPitchRange.y);
                    footstepSource.PlayOneShot(nextFootstepSound);
                    rightFootNext = !rightFootNext;
                    footstepCounter = (currentState == PlayerState.Running) ? runFootstepInterval : walkFootstepInterval;
                }
            }

            // Check if player state is idle before playing the turning sound
            if (currentState == PlayerState.Idle && Input.GetAxis("Mouse X") != 0 && Time.time > lastTurnTime + turnCooldown)
            {
                SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
                AudioClip turnSound = sfx.turnSounds[Random.Range(0, sfx.turnSounds.Length)];
                turnSource.pitch = Random.Range(sfx.turnPitchRange.x, sfx.turnPitchRange.y);
                turnSource.PlayOneShot(turnSound);
                lastTurnTime = Time.time;
            }
        }
    }

    private SurfaceFootstepSFX GetFootstepSFXForCurrentSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2f))
        {
            string surfaceTag = hit.collider.gameObject.tag;
            Debug.Log("Surface tag detected: " + surfaceTag);

            foreach (SurfaceFootstepSFX sfx in footstepSFXs)
            {
                Debug.Log("Checking SFX for tag: " + sfx.tag);

                if (sfx.tag == surfaceTag)
                {
                    Debug.Log("Matched SFX found for tag: " + sfx.tag);
                    return sfx;
                }
            }
        }

        Debug.Log("No matched SFX found, defaulting to first in list");
        return footstepSFXs[0];
    }

    private void HandleBobbing()
    {
        float bobbingSpeed = 0f;
        float currentBobbingAmount = 0f; // New variable for the current bobbing amount

        if (currentState == PlayerState.Idle || controller.velocity.magnitude > 0.1f)
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                    bobbingSpeed = idleBobbingSpeed;
                    currentBobbingAmount = idleBobbingAmount;
                    break;
                case PlayerState.Walking:
                    bobbingSpeed = walkBobbingSpeed;
                    currentBobbingAmount = walkBobbingAmount;
                    break;
                case PlayerState.Running:
                    bobbingSpeed = runBobbingSpeed;
                    currentBobbingAmount = runBobbingAmount;
                    break;
                case PlayerState.Boosting:
                    bobbingSpeed = boostBobbingSpeed;
                    currentBobbingAmount = boostBobbingAmount;
                    break;
            }

            bobbingCounter += Time.deltaTime * bobbingSpeed;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraYPos + Mathf.Sin(bobbingCounter) * currentBobbingAmount, // Use current bobbing amount here
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