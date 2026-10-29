using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    public GameManager gameManager;

    private bool trapActive = true;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        Collider trapCollider = GetComponent<Collider>();
        trapCollider.isTrigger = true;
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
