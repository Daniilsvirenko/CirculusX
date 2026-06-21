using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public float interactRadius = 0.3f; // thickness of the interaction "feeler"

    [Header("UI")]
    public InteractionPromptUI promptUI;

    private PlayerControls controls;
    private IInteractable currentTarget;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Update()
    {
        UpdateCurrentTarget();
    }

    private void UpdateCurrentTarget()
    {
        IInteractable found = null;

        if (Physics.SphereCast(transform.position, interactRadius, transform.forward, out RaycastHit hit, interactRange))
        {
            found = hit.collider.GetComponent<IInteractable>();
        }

        if (found != currentTarget)
        {
            currentTarget = found;

            if (promptUI != null)
            {
                if (currentTarget != null)
                    promptUI.Show(currentTarget.PromptText);
                else
                    promptUI.Hide();
            }
        }
    }

    private void TryInteract()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red, 2f);

        if (Physics.SphereCast(transform.position, interactRadius, transform.forward, out RaycastHit hit, interactRange))
        {
            Debug.Log($"[SphereCast] Hit object: {hit.collider.gameObject.name}");

            IInteractable interactableObj = hit.collider.GetComponent<IInteractable>();

            if (interactableObj != null)
            {
                interactableObj.Interact();
            }
            else
            {
                Debug.LogWarning($"[Error] Object {hit.collider.gameObject.name} is missing the ElevatorButton (or IInteractable) script!");
            }
        }
        else
        {
            Debug.Log("[SphereCast] Empty! Standing too far away or not aimed close enough.");
        }
    }
}