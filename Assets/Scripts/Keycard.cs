
using UnityEngine;
using System.Collections;

public class Keycard : MonoBehaviour, IInteractable
{
    public GameManager gameManager;
    public AudioSource audioSource;

    public string InteractionPrompt => "Pick up keycard";

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (audioSource != null)
        {
            audioSource.Play();
            StartCoroutine(DisableAfterSound());
        }
        else
        {
            gameObject.SetActive(false);
        }

        GameManager manager = gameManager != null ? gameManager : interactor.GameManager;
        manager?.AcquireKeycard();
    }

    private IEnumerator DisableAfterSound()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        gameObject.SetActive(false);
    }
}
