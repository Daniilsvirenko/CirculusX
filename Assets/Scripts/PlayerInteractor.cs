using UnityEngine;
using UnityEngine.InputSystem; // Required for the new system

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f; // Max distance to interact with objects

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to the 'E' button press event
        controls.Gameplay.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    private void TryInteract()
    {
        // Shoot a ray directly from the center of the camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // DRAW RAY IN EDITOR: Now, if you switch to the Scene tab, you will see a red ray!
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red, 2f);

        // Check if the ray hits anything within interactRange
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // DIAGNOSTICS: Log to the console exactly what the ray hit
            Debug.Log($"[Raycast] Ray hit object: {hit.collider.gameObject.name}");

            // Check if the hit object implements our strict interface
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
            Debug.Log("[Raycast] Empty! The ray hit nothing. You might be standing too far away.");
        }
    }
}