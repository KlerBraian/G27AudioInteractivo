using UnityEngine;

public class Keycard : MonoBehaviour, IInteractable
{
    public GameManager gameManager;

    public string InteractionPrompt => "Pick up keycard";

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        GameManager manager = gameManager != null ? gameManager : interactor.GameManager;
        manager?.AcquireKeycard();
        gameObject.SetActive(false);
    }
}
