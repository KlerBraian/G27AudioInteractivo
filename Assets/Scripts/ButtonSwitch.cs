using UnityEngine;

public class ButtonSwitch : MonoBehaviour, IInteractable
{
    public GameManager gameManager;
    public Trap targetTrap;
    public Transform buttonVisual;
    public Renderer buttonRenderer;
    public Color pressedColor = new Color(0.08f, 0.75f, 0.32f);

    private bool pressed;
    private Vector3 originalButtonLocalPosition;

    public string InteractionPrompt => pressed ? "Button pressed" : "Press button";

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (buttonVisual == null)
        {
            Transform foundButton = transform.Find("Blue Button");
            if (foundButton != null)
            {
                buttonVisual = foundButton;
            }
        }

        if (buttonRenderer == null && buttonVisual != null)
        {
            buttonRenderer = buttonVisual.GetComponent<Renderer>();
        }

        if (buttonVisual != null)
        {
            originalButtonLocalPosition = buttonVisual.localPosition;
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        GameManager manager = gameManager != null ? gameManager : interactor.GameManager;

        if (pressed)
        {
            manager?.ShowMessage("Button already pressed");
            return;
        }

        pressed = true;
        targetTrap?.DisableTrap();
        manager?.PressButton();
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (buttonVisual != null)
        {
            buttonVisual.localPosition = originalButtonLocalPosition + Vector3.right * 0.05f;
        }

        if (buttonRenderer != null)
        {
            Material material = buttonRenderer.material;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", pressedColor);
            }
            else
            {
                material.color = pressedColor;
            }
        }
    }
}
