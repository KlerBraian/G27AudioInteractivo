using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Camera viewCamera;
    public UIManager uiManager;
    public GameManager gameManager;
    public float interactRange = 3f;
    public LayerMask interactMask = -1;

    private IInteractable currentInteractable;

    public GameManager GameManager => gameManager;

    public UIManager UIManager => uiManager;

    private void Awake()
    {
        if (viewCamera == null)
        {
            viewCamera = GetComponentInChildren<Camera>();
        }

        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void Update()
    {
        UpdateCurrentTarget();

        Keyboard keyboard = Keyboard.current;
        if (currentInteractable != null && keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            currentInteractable.Interact(this);
        }
    }

    private void OnDisable()
    {
        uiManager?.HidePrompt();
    }

    private void UpdateCurrentTarget()
    {
        if (viewCamera == null)
        {
            currentInteractable = null;
            uiManager?.HidePrompt();
            return;
        }

        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                uiManager?.ShowPrompt($"Press E - {interactable.InteractionPrompt}");
                return;
            }
        }

        currentInteractable = null;
        uiManager?.HidePrompt();
    }
}
