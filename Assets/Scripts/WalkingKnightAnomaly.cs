using UnityEngine;

public class WalkingKnightAnomaly : MonoBehaviour
{
    [Header("Knight Setup")]
    public Animator knightAnimator;
    public Transform knightTransform;
    public Transform playerTransform;
    public float walkSpeed = 2.0f;

    [Header("Standalone Normal Group (The Statue)")]
    public GameObject normalKnightStatue;

    [Header("Catch Settings")]
    [Tooltip("Distance at which the knight catches the player. 1.2 to 1.5 is usually ideal.")]
    public float catchDistance = 1.3f;

    [Header("Audio Settings")]
    [Tooltip("The AudioSource attached to the moving Knight prefab. Ensure Spatial Blend is set to 3D!")]
    public AudioSource movementAudioSource;
    [Tooltip("The single background sound file that should loop while the knight chases the player.")]
    public AudioClip loopingMovementClip;

    private CharacterController knightController;
    private bool isTriggered = false;
    private bool isHandlingCatch = false;

    // Variables to store the original starting position
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasSavedOriginalPosition = false;

    private void OnEnable()
    {
        if (knightController == null && knightTransform != null)
        {
            knightController = knightTransform.GetComponent<CharacterController>();
        }

        if (!hasSavedOriginalPosition && knightTransform != null)
        {
            if (knightController != null) knightController.enabled = false;

            originalPosition = knightTransform.position;
            originalRotation = knightTransform.rotation;
            hasSavedOriginalPosition = true;

            if (knightController != null) knightController.enabled = true;
        }

        if (knightTransform != null) knightTransform.gameObject.SetActive(true);
        if (normalKnightStatue != null) normalKnightStatue.SetActive(false);

        isHandlingCatch = false;
    }

    private void OnDisable()
    {
        isTriggered = false;
        isHandlingCatch = false;

        // Clean up the looping audio immediately if the floor resets out of view
        StopLoopingAudio();

        if (knightTransform != null) knightTransform.gameObject.SetActive(false);
        if (normalKnightStatue != null) normalKnightStatue.SetActive(true);
    }

    // Hallway Trigger Box detection zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered && !isHandlingCatch)
        {
            isTriggered = true;
            if (knightAnimator != null)
            {
                knightAnimator.SetTrigger("StartWalking");
            }

            // Start the looping sound sequence immediately as he starts chasing
            StartLoopingAudio();
        }
    }

    private void Update()
    {
        if (isTriggered && !isHandlingCatch && knightTransform != null && playerTransform != null)
        {
            // 1. Aim toward the player
            Vector3 targetDirection = playerTransform.position - knightTransform.position;
            targetDirection.y = 0;
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                knightTransform.rotation = Quaternion.Slerp(knightTransform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // 2. Character-Controller wall safe movement
            Vector3 moveDirection = knightTransform.forward * walkSpeed;
            if (knightController != null)
            {
                knightController.Move(moveDirection * Time.deltaTime);
            }

            // 3. Catch verification loop
            float currentDistance = Vector3.Distance(knightTransform.position, playerTransform.position);
            if (currentDistance <= catchDistance)
            {
                HandlePlayerCaught();
            }
        }
    }

    private void StartLoopingAudio()
    {
        if (movementAudioSource != null && loopingMovementClip != null)
        {
            movementAudioSource.clip = loopingMovementClip;
            movementAudioSource.loop = true; // Force-activate looping logic
            movementAudioSource.Play();
        }
    }

    private void StopLoopingAudio()
    {
        if (movementAudioSource != null)
        {
            movementAudioSource.Stop();
            movementAudioSource.loop = false;
        }
    }

    private void HandlePlayerCaught()
    {
        if (isHandlingCatch) return;
        isHandlingCatch = true;
        isTriggered = false;

        Debug.Log("Player caught by the Knight! Overriding door systems and terminating loop audio...");

        // Kill the loop immediately when you hit the player
        StopLoopingAudio();

        if (knightAnimator != null)
        {
            knightAnimator.Rebind();
            knightAnimator.Update(0f);
        }

        if (knightController != null) knightController.enabled = false;
        if (knightTransform != null)
        {
            knightTransform.position = originalPosition;
            knightTransform.rotation = originalRotation;
        }
        if (knightController != null) knightController.enabled = true;

        // Force-clear legacy elevator loops right away so the next run functions perfectly
        ElevatorDoorController[] allDoors = FindObjectsOfType<ElevatorDoorController>();
        foreach (ElevatorDoorController door in allDoors)
        {
            if (door != null)
            {
                Animation doorAnim = door.GetComponent<Animation>();
                if (doorAnim != null && doorAnim.clip != null)
                {
                    doorAnim.Stop();
                    doorAnim[doorAnim.clip.name].time = 0f;
                    doorAnim[doorAnim.clip.name].speed = 1f;
                    doorAnim.Sample();
                }
                if (door.doorAudioSource != null)
                {
                    door.doorAudioSource.Stop();
                }
            }
        }

        // Teleport player back inside the elevator box zone manually
        if (GameManager.Instance != null && GameManager.Instance.spawnPoint != null)
        {
            CharacterController playerCC = playerTransform.GetComponent<CharacterController>();
            if (playerCC != null) playerCC.enabled = false;

            playerTransform.position = GameManager.Instance.spawnPoint.position;
            playerTransform.rotation = GameManager.Instance.spawnPoint.rotation;

            if (playerCC != null) playerCC.enabled = true;

            bool forceWrongDecision = !GameManager.Instance.isAnomalyPresentOnCurrentFloor;
            GameManager.Instance.MakeDecision(forceWrongDecision);
        }
    }
}