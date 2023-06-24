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
    private float speed = 5.0f;
    [SerializeField, Tooltip("Walking speed of the player")]
    private float walkSpeed = 3f;
    [SerializeField, Tooltip("Running speed of the player")]
    private float runSpeed = 6f;
    [SerializeField, Tooltip("Jump force for the regular jump")]
    private float jumpForce = 2f;
    [SerializeField, Tooltip("Height of the jump")]
    private float jumpHeight = 2.0f;
    [SerializeField, Tooltip("Player's moving speed")]
    private float moveSpeed = 5f;
    [SerializeField, Tooltip("The speed at which the player falls after a jump")]
    private float fallSpeed = 2.5f;
    [SerializeField, Tooltip("Climbing speed of the player")]
    private float climbSpeed = 3f;

    private bool isJumping = false;
    private bool isFalling = false;
    private bool hasJumped = false;
    private bool isGrounded;
    private float landingTimer;
    private bool wasGrounded;
    private bool isRunning = false;
    private bool canRun = true;
    private bool canJump = true;
    private Vector3 velocity;
    private float originalGravity;
    private bool rightFootNext = true;
    private bool isClimbing = false;
    private bool jumpQueued = false;
    private bool wasJumpingLastFrame = false;
    private float jumpTimeCounter;
    private SurfaceFootstepSFX lastUsedSFXGroup = NullSFX;
    private static readonly SurfaceFootstepSFX NullSFX = new SurfaceFootstepSFX();


    [Header("Jump")]
    [SerializeField, Tooltip("Max time the jump button can be held")]
    private float jumpTime = 0.35f;
    [SerializeField, Tooltip("Allow holding the button for continuous jumping")]
    private bool holdToJump = false;
    [SerializeField, Tooltip("Jump force while running")]
    private float runJumpForce = 8.0f;
    [SerializeField, Tooltip("Jump force while walking")]
    private float walkJumpForce = 5.0f;
    [SerializeField, Tooltip("Distance after which falling starts")]
    private float fallThreshold = 10f;
    [SerializeField, Tooltip("Time taken for landing after a jump")]
    private float landingTime = 0.1f;
    [SerializeField, Tooltip("Object representing the feet of the character")]
    private Transform groundCheck;
    [SerializeField, Tooltip("Radius for ground check")]
    private float groundCheckRadius = 0.2f;
    [SerializeField, Tooltip("Layers considered as ground")]
    private LayerMask groundMask;
    private float coyoteTime;
    public float maxCoyoteTime = 0.2f;  // Maximum amount of time player can jump after falling off a platform
    [SerializeField, Tooltip("Jump force while idle")]
    private float idleJumpForce = 3.0f;

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
    private bool boostEnabled;
    public bool isBoostEnabled = true;

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
    public float mouseSmoothness = 5f;
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

    public BoostSound boostSound;
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

    public float CurrentHealth => currentHealth;
    public float CurrentStamina => currentStamina;
    public float CurrentBoost => currentBoost;

    private SoundManager soundManager; // Added reference to SoundManager

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        soundManager = FindObjectOfType<SoundManager>(); // Cache the reference to SoundManager
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = true;
            Debug.Log("Entered ladder");
        }

        if (soundManager != null)
        {
            soundManager.PlaySound(other.gameObject.tag, "landing");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = false;
            Debug.Log("Exited ladder");
        }
    }

    void Start()
    {
        InitializePlayer();
        InitializeSoundAndForces();

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBoost = maxBoost;
        jumpTimeCounter = jumpTime;

        boostSounds.Add(KeyCode.W, boostSoundW);
        boostSounds.Add(KeyCode.A, boostSoundA);
        boostSounds.Add(KeyCode.S, boostSoundS);
        boostSounds.Add(KeyCode.D, boostSoundD);

        if (footstepSFXs.Length > 0)
        {
            lastUsedSFXGroup = footstepSFXs[0];
        }
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
    }
    void Update()
    {
        // Check for ground state
        bool wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;

        // Update Coyote Time
        if (isGrounded)
        {
            coyoteTime = maxCoyoteTime;
        }
        else
        {
            coyoteTime -= Time.deltaTime;
        }

        // Process the new jump function
        ProcessJump();

        // Process the other input and state functions
        ProcessPlayerMovement();
        ProcessPlayerFOV();
        ProcessMouseLook();
        ProcessFootsteps();

        HandleBobbing();

        // If the player is on the ground and not currently jumping, forcefully keep them on the ground
        if (isGrounded && !isJumping)
        {
            velocity.y = -2f;
        }

        wasJumpingLastFrame = isJumping;
    }
    void FixedUpdate()
    {
        // Apply the velocity
        controller.Move(velocity * Time.fixedDeltaTime);

        // If we're on the ground, reset Y velocity
        if (isGrounded)
        {
            velocity.y = 0f;
        }
    }
    private void ProcessJump()
    {
        if (coyoteTime > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;

                // Modify jump force according to player's current state
                float jumpForce = GetCurrentJumpForce();

                // Increase jump force
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpForce;

                // Reset coyote time as we've used it
                coyoteTime = 0;
            }
        }

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;

                // Modify jump force according to player's current state
                float jumpForce = GetCurrentJumpForce();

                // Increase jump force
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpForce;
            }
        }

        // While the Jump button is being held AND we haven't exceeded our max jump time...
        if (isJumping && Input.GetButton("Jump") && jumpTimeCounter > 0)
        {
            // Keep applying an upward force
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpForce;
            jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            isJumping = false;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // Apply additional gravity for quicker fall
        velocity.y += gravity * 2 * Time.deltaTime;
    }
    private float GetCurrentJumpForce()
    {
        float jumpForce;

        switch (currentState)
        {
            case PlayerState.Idle:
                jumpForce = idleJumpForce; // Add this variable to your class, or substitute an appropriate value
                break;
            case PlayerState.Walking:
                jumpForce = walkJumpForce;
                break;
            case PlayerState.Running:
                jumpForce = runJumpForce;
                break;
            default:
                jumpForce = idleJumpForce;
                break;
        }

        return jumpForce;
    }

    private void ProcessGroundState()
    {
        bool wasGroundedPreviousFrame = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && !wasGroundedPreviousFrame)
        {
            velocity.y = -gravity * Time.deltaTime; // This will keep the player on the ground
        }
    }
    private void ProcessPlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift);

        bool isMoving = horizontal != 0 || vertical != 0;

        PlayerState newState = PlayerState.Idle;

        if (isMoving)
        {
            float moveSpeed = (currentStamina > 0 && isShiftHeld && canRun) ? runSpeed : walkSpeed;
            speed = moveSpeed; // Updating speed variable
            newState = (moveSpeed == runSpeed) ? PlayerState.Running : PlayerState.Walking;

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            controller.Move(move.normalized * moveSpeed * Time.deltaTime);  // Use Move instead of SimpleMove
        }
        else
        {
            // Stop player movement immediately when no input is detected
            controller.Move(Vector3.zero);  // Use Move instead of SimpleMove
        }

        currentState = newState;
        isRunning = isShiftHeld && isMoving;
    }

    private void Fall()
    {
        if (!Input.GetButton("Jump") && velocity.y > 0)
        {
            velocity.y *= 0.5f; // Reduce upward speed if jump button is released mid-air
        }

        velocity.y += gravity * Time.deltaTime;
        isJumping = false;
        isFalling = true;
    }
    private void ClimbLadder()
    {
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 climbDirection = (up * verticalInput + right * horizontalInput + forward * verticalInput).normalized;
        controller.Move(climbDirection * climbSpeed * Time.fixedDeltaTime);
    }
    private void ProcessPlayerFOV()
    {
        float targetFOV = defaultFOV;

        if (isRunning && controller.velocity.magnitude > 0.1f)
        {
            targetFOV = runFOV;
        }

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
    }
    private void ProcessMouseLook()
    {
        float mouseLookX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseLookY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseLookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // This line restricts the camera's vertical movement (up and down) so it doesn't exceed a realistic range (90 degrees)

        // rotate the player model horizontally and the camera vertically
        transform.Rotate(Vector3.up * mouseLookX);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void ProcessLand()
    {
        if (!wasGrounded && isGrounded)
        {
            SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
            footstepSource.pitch = Random.Range(sfx.landPitchRange.x, sfx.landPitchRange.y);
            footstepSource.PlayOneShot(sfx.landSounds[Random.Range(0, sfx.landSounds.Length)]);

            // Reset footstep interval when landing
            ResetFootstepCounter();
        }
    }

    private void ResetFootstepCounter()
    {
        // if the player is running, set the footstep counter to the run interval
        // otherwise, set it to the walk interval
        footstepCounter = (currentState == PlayerState.Running) ? runFootstepInterval : walkFootstepInterval;
    }


    private IEnumerator Boost(KeyCode key)
    {
        if (!boostEnabled)
        {
            yield break;  // Stop the execution of the coroutine
        }

        BoostSound boostSound;
        if (!boostSounds.TryGetValue(key, out boostSound))
        {
            Debug.LogError("No boost sound associated with this key.");
            yield break;
        }

        // Now you can use boostSound.clip as the sound clip to play
        actionSource.clip = boostSound.clip;
        actionSource.pitch = Random.Range(boostSound.pitchRange.x, boostSound.pitchRange.y);
        actionSource.Play();

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
    private void PlayBoostSound()
    {
        // Only play the sound if boost is enabled
        if (isBoostEnabled)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = boostSound.clip;
            audioSource.pitch = Random.Range(boostSound.pitchRange.x, boostSound.pitchRange.y);
            audioSource.Play();
        }
    }
    public class BoostSound
    {
        public AudioClip clip;
        public Vector2 pitchRange;
    }

    private void ProcessFootsteps()
    {
        if (!isGrounded || currentState == PlayerState.Idle)
        {
            return;
        }

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        if (move.magnitude > 0)
        {
            footstepCounter -= Time.deltaTime;
            if (footstepCounter <= 0)
            {
                SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
                if (ReferenceEquals(sfx, null))
                {
                    sfx = lastUsedSFXGroup;
                }
                else
                {
                    lastUsedSFXGroup = sfx;
                }

                AudioClip nextFootstepSound = rightFootNext ? sfx.walkSounds[Random.Range(0, sfx.walkSounds.Length)] : sfx.runSounds[Random.Range(0, sfx.runSounds.Length)];
                footstepSource.pitch = (currentState == PlayerState.Running) ? Random.Range(sfx.runPitchRange.x, sfx.runPitchRange.y) : Random.Range(sfx.walkPitchRange.x, sfx.walkPitchRange.y);
                footstepSource.PlayOneShot(nextFootstepSound);
                rightFootNext = !rightFootNext;
                footstepCounter = (currentState == PlayerState.Running) ? runFootstepInterval : walkFootstepInterval;
            }
        }
        else if (Input.GetAxis("Mouse X") != 0 && Time.time > lastTurnTime + turnCooldown)
        {
            SurfaceFootstepSFX sfx = GetFootstepSFXForCurrentSurface();
            if (ReferenceEquals(sfx, null))
            {
                sfx = lastUsedSFXGroup;
            }
            else
            {
                lastUsedSFXGroup = sfx;
            }

            AudioClip turnSound = sfx.turnSounds[Random.Range(0, sfx.turnSounds.Length)];
            turnSource.pitch = Random.Range(sfx.turnPitchRange.x, sfx.turnPitchRange.y);
            turnSource.PlayOneShot(turnSound);
            lastTurnTime = Time.time;
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
                if (sfx.tag == surfaceTag)
                {
                    Debug.Log("Matched SFX found for tag: " + sfx.tag);
                    return sfx;
                }
            }
        }
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