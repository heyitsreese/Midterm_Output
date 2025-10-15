using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float playerReach = 3f;
    private Interactable currentInteractable;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.PlayerInteractions.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => controls.PlayerInteractions.Enable();
    void OnDisable() => controls.PlayerInteractions.Disable();

    void Update() => CheckInteraction();

    void TryInteract()
    {
        if (currentInteractable != null)
            currentInteractable.Interact();
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (currentInteractable && newInteractable != currentInteractable)
                    currentInteractable.DisableOutline();

                if (newInteractable != null && newInteractable.enabled)
                    SetNewCurrentInteractable(newInteractable);
                else
                    DisableCurrentInteractable();
            }
            else
                DisableCurrentInteractable();
        }
        else
            DisableCurrentInteractable();
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        if (currentInteractable == newInteractable) return;
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}
