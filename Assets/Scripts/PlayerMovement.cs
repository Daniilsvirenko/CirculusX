using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 8.0f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float mouseSensitivity = 0.3f;
    public Transform playerCamera;

    [Header("Head Bobbing")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 15f;
    public float runBobAmount = 0.1f;
    private float defaultCameraY = 0f;
    private float bobTimer = 0f;

    [Header("Audio Settings")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds; // Array of multiple sounds for variety
    public float stepDistance = 1.0f; // Distance in meters player must travel to trigger a step
    public float runStepDistance = 1.5f; // Позволяет шагам быть чуть реже при быстром беге

    // --- New Phantom Footstep Reference ---
    [Header("Anomaly Settings")]
    public FootstepEchoController echoController;
    // --------------------------------------

    private float accumulatedDistance = 0f;
    private Vector3 lastPosition; // Used to track distance moved since last frame

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    // --- Input System Variables ---
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;

    void Awake()
    {
        // Initialize New Input System
        controls = new PlayerControls();
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Gameplay.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize lastPosition to prevent instant massive distance on frame 1
        lastPosition = transform.position;

        if (playerCamera != null)
        {
            defaultCameraY = playerCamera.localPosition.y;
        }
    }

    void Update()
    {
        // --- Mouse Look ---
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- Run Input Check ---
        bool isRunning = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // --- Movement ---
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Gravity ---
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small constant downward force to stay grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check if player is actively inputting movement commands
        bool isTryingToMove = moveInput.sqrMagnitude > 0.1f;

        // --- Head Bobbing (Тряска камеры при ходьбе/беге) ---
        if (controller.isGrounded && isTryingToMove && playerCamera != null)
        {
            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;
            
            bobTimer += Time.deltaTime * bobSpeed;
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                defaultCameraY + Mathf.Sin(bobTimer) * bobAmount,
                playerCamera.localPosition.z);
        }
        else if (playerCamera != null)
        {
            bobTimer = 0f;
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                Mathf.Lerp(playerCamera.localPosition.y, defaultCameraY, Time.deltaTime * 5f),
                playerCamera.localPosition.z);
        }

        // --- DISTANCE-BASED FOOTSTEP AUDIO ---

        // 1. Calculate horizontal distance moved since the exact last frame
        Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastPosFlat = new Vector3(lastPosition.x, 0, lastPosition.z);

        float distanceMovedThisFrame = Vector3.Distance(currentPosFlat, lastPosFlat);

        // Update lastPosition for the next frame
        lastPosition = transform.position;

        // If grounded, trying to move, AND physically moved a measurable amount
        if (controller.isGrounded && isTryingToMove && distanceMovedThisFrame > 0.001f)
        {
            accumulatedDistance += distanceMovedThisFrame;
            
            float currentStepDistance = isRunning ? runStepDistance : stepDistance;

            // Trigger footstep sound when threshold is reached
            if (accumulatedDistance >= currentStepDistance)
            {
                PlayFootstep();
                accumulatedDistance = 0f; // Reset for the next step
            }
        }
        else if (!isTryingToMove)
        {
            // Pre-load the distance slightly so the very first step plays quickly when moving starts
            float targetDistance = (isRunning ? runStepDistance : stepDistance) * 0.8f;
            accumulatedDistance = targetDistance;
        }
    }

    private void PlayFootstep()
    {
        // Ensure sounds exist and AudioSource is assigned
        if (footstepSounds != null && footstepSounds.Length > 0 && footstepAudioSource != null)
        {
            // Pick random sound and slightly alter pitch for realism
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);

            footstepAudioSource.PlayOneShot(footstepSounds[randomIndex]);

            // --- Trigger the stalker echo to schedule a trailing step ---
            if (echoController != null && echoController.gameObject.activeInHierarchy)
            {
                echoController.RegisterPlayerStep();
            }
        }
    }
}