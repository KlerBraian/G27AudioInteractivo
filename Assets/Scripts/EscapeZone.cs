
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EscapeZone : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource audioSource;
    public AudioSource musicBackground;

    private bool escaped;

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

        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (escaped || !other.CompareTag("Player"))
        {
            return;
        }

        escaped = true;

        // Reproducir sonido de victoria inmediatamente.
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
            musicBackground.Stop();
        }

        gameManager?.TryEscape();
    }
}
