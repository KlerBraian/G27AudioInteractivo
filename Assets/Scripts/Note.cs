
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    public GameManager gameManager;
    public AudioSource audioSource;

    [TextArea]
    public string noteText = "Hint: the blue wall button disables the laser.";

    public string InteractionPrompt => "Read note";

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        GameManager manager = gameManager != null ? gameManager : interactor.GameManager;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        manager?.ShowMessage(noteText, 3f);
    }
}
