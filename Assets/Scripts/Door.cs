using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public GameManager gameManager;
    public Transform doorPivot;
    public Vector3 openLocalEulerAngles = new Vector3(0f, 95f, 0f);
    public float openDuration = 0.75f;

    private bool isOpen;
    private bool isOpening;

    public string InteractionPrompt => isOpen ? "Door is open" : "Open door";

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (doorPivot == null && transform.parent != null)
        {
            doorPivot = transform.parent;
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        GameManager manager = gameManager != null ? gameManager : interactor.GameManager;

        if (isOpen || isOpening)
        {
            manager?.ShowMessage("Door is already open");
            return;
        }

        if (manager == null || !manager.HasKeycard)
        {
            manager?.ShowMessage("Door is locked");
            return;
        }

        manager.OpenMainDoor();
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        isOpening = true;

        Transform pivot = doorPivot != null ? doorPivot : transform;
        Quaternion startRotation = pivot.localRotation;
        Quaternion targetRotation = Quaternion.Euler(openLocalEulerAngles);

        if (openDuration <= 0f)
        {
            pivot.localRotation = targetRotation;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / openDuration);
                pivot.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }
        }

        pivot.localRotation = targetRotation;
        isOpening = false;
        isOpen = true;
    }
}
