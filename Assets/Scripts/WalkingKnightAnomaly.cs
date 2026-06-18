using UnityEngine;

public class WalkingKnightAnomaly : MonoBehaviour
{
    public Animator knightAnimator;
    public Transform knightTransform;
    public Transform playerTransform;
    public float walkSpeed = 2.0f;

    [Header("Standalone Normal Group (The Statue)")]
    public GameObject normalKnightStatue;

    [Header("Catch Settings")]
    [Tooltip("Distance at which the knight catches the player. 1.2 to 1.5 is usually ideal.")]
    public float catchDistance = 1.3f;

    private CharacterController knightController;
    private bool isTriggered = false;

    // Variables to store the original starting position
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        // Save the starting position and orientation when the scene loads
        if (knightTransform != null)
        {
            originalPosition = knightTransform.position;
            originalRotation = knightTransform.rotation;

            // Grab the Character Controller component for wall collisions
            knightController = knightTransform.GetComponent<CharacterController>();
        }
    }

    // This runs the exact frame the GameManager turns this Anomaly Object ON
    private void OnEnable()
    {
        // HIDE the normal statue because the anomaly version is active on this floor
        if (normalKnightStatue != null) normalKnightStatue.SetActive(false);
    }

    // This runs the exact frame the GameManager resets the floor and turns this OFF
    private void OnDisable()
    {
        isTriggered = false;

        // BRING BACK the normal statue when the anomaly is gone
        if (normalKnightStatue != null) normalKnightStatue.SetActive(true);
    }

    // Hallway Trigger Detection (Starts the chase)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            if (knightAnimator != null)
            {
                knightAnimator.SetTrigger("StartWalking");
            }
        }
    }

    private void Update()
    {
        if (isTriggered && knightTransform != null && playerTransform != null)
        {
            // 1. Calculate direction and smoothly rotate toward the player
            Vector3 targetDirection = playerTransform.position - knightTransform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            knightTransform.rotation = Quaternion.Slerp(knightTransform.rotation, targetRotation, Time.deltaTime * 5f);

            // 2. Physics/Wall-safe movement calculation via Character Controller
            Vector3 moveDirection = knightTransform.forward * walkSpeed;
            if (knightController != null)
            {
                knightController.Move(moveDirection * Time.deltaTime);
            }

            // 3. Distance Check (Catches the player)
            float currentDistance = Vector3.Distance(knightTransform.position, playerTransform.position);
            if (currentDistance <= catchDistance)
            {
                HandlePlayerCaught();
            }
        }
    }

    private void HandlePlayerCaught()
    {
        Debug.Log("Player caught by the Knight! Executing penalty sequence...");
        isTriggered = false; // Halt movement immediately

        if (GameManager.Instance != null)
        {
            // Send the opposite of the truth to explicitly force a failure state 
            // This resets the current floor to 10, updates displays, and teleports the player
            bool forceWrongDecision = !GameManager.Instance.isAnomalyPresentOnCurrentFloor;
            GameManager.Instance.MakeDecision(forceWrongDecision);
        }
    }

    // Call this function whenever you change floors or want to turn off the anomaly manually
    public void ResetKnight(bool shouldBePresentOnThisFloor)
    {
        isTriggered = false;

        if (knightController != null) knightController.enabled = false;
        knightTransform.position = originalPosition;
        knightTransform.rotation = originalRotation;
        if (knightController != null) knightController.enabled = true;

        if (knightAnimator != null)
        {
            knightAnimator.Rebind();
            knightAnimator.Update(0f);
        }

        if (knightTransform != null)
        {
            knightTransform.gameObject.SetActive(shouldBePresentOnThisFloor);
        }

        // Toggling this natively handles our clean OnEnable / OnDisable swapping logic!
        this.gameObject.SetActive(shouldBePresentOnThisFloor);
    }
}