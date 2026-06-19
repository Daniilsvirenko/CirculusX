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

    private CharacterController knightController;
    private bool isTriggered = false;

    // Variables to store the original starting position
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasSavedOriginalPosition = false;

    // This fires the exact frame the GameManager runs currentActiveAnomalyObject.SetActive(true);
    private void OnEnable()
    {
        if (knightController == null && knightTransform != null)
        {
            knightController = knightTransform.GetComponent<CharacterController>();
        }

        // SAFEGUARD: Saves the pedestal location the very first time this anomaly is called into action
        if (!hasSavedOriginalPosition && knightTransform != null)
        {
            if (knightController != null) knightController.enabled = false;

            originalPosition = knightTransform.position;
            originalRotation = knightTransform.rotation;
            hasSavedOriginalPosition = true;

            if (knightController != null) knightController.enabled = true;
        }

        // SHOW the moving walking knight variant mesh because the floor is active
        if (knightTransform != null) knightTransform.gameObject.SetActive(true);

        // HIDE the peaceful normal statue
        if (normalKnightStatue != null) normalKnightStatue.SetActive(false);
    }

    // This fires the exact frame the GameManager cleans up the floor via obj.SetActive(false);
    private void OnDisable()
    {
        isTriggered = false;

        // HIDE the moving walking knight variant entirely so he isn't left standing around
        if (knightTransform != null) knightTransform.gameObject.SetActive(false);

        // BRING BACK the normal statue safely
        if (normalKnightStatue != null) normalKnightStatue.SetActive(true);
    }

    // Hallway Trigger Box detection zone
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
            // 1. Aim toward the player
            Vector3 targetDirection = playerTransform.position - knightTransform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            knightTransform.rotation = Quaternion.Slerp(knightTransform.rotation, targetRotation, Time.deltaTime * 5f);

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

    private void HandlePlayerCaught()
    {
        Debug.Log("Player caught by the Knight! Resetting floor state...");
        isTriggered = false;

        // Force snap him back behind the wall frame instantly before GameManager reloads layouts
        if (knightController != null) knightController.enabled = false;
        if (knightTransform != null)
        {
            knightTransform.position = originalPosition;
            knightTransform.rotation = originalRotation;
        }
        if (knightController != null) knightController.enabled = true;

        if (knightAnimator != null)
        {
            knightAnimator.Rebind();
            knightAnimator.Update(0f);
        }

        // Call your GameManager Instance and force a wrong decision rule layout reload
        if (GameManager.Instance != null)
        {
            bool forceWrongDecision = !GameManager.Instance.isAnomalyPresentOnCurrentFloor;
            GameManager.Instance.MakeDecision(forceWrongDecision);
        }
    }
}
