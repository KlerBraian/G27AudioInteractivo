using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    public GameManager gameManager;
    [TextArea]
    public string noteText = "Hint: the blue wall button disables the laser.";

    public string InteractionPrompt => "Read note";

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
        manager?.ShowMessage(noteText, 3f);
    }
}
