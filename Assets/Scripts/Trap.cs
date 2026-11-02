
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    public GameManager gameManager;
    private AudioSource[] audioSources;

    private bool trapActive = true;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        Collider trapCollider = GetComponent<Collider>();
        trapCollider.isTrigger = true;

        audioSources = GetComponentsInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!trapActive || !other.CompareTag("Player"))
        {
            return;
        }

        gameManager?.ResetPlayer();
    }

    public void DisableTrap()
    {
        if (!trapActive)
        {
            return;
        }

        trapActive = false;

        // Detener todos los sonidos de la trampa
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }

        foreach (Renderer trapRenderer in GetComponentsInChildren<Renderer>())
        {
            trapRenderer.enabled = false;
        }

        foreach (Collider trapCollider in GetComponentsInChildren<Collider>())
        {
            trapCollider.enabled = false;
        }
    }
}

