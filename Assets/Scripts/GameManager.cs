using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager uiManager;
    public Transform playerTransform;
    public CharacterController playerController;
    public SimpleFirstPersonController firstPersonController;
    public PlayerInteractor playerInteractor;
    public Transform playerSpawnPoint;

    public bool HasKeycard { get; private set; }
    public bool MainDoorOpen { get; private set; }
    public bool ButtonPressed { get; private set; }
    public bool Escaped { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        FindMissingReferences();
    }

    private void Start()
    {
        uiManager?.SetKeycardAcquired(HasKeycard);
        uiManager?.SetPlayAgainHandler(PlayAgain);
        ShowMessage("Find the keycard and escape.", 2f);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (Escaped && keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            PlayAgain();
        }
    }

    public void AcquireKeycard()
    {
        if (HasKeycard)
        {
            return;
        }

        HasKeycard = true;
        uiManager?.SetKeycardAcquired(true);
        ShowMessage("Keycard Acquired");
    }

    public void OpenMainDoor()
    {
        if (MainDoorOpen)
        {
            return;
        }

        MainDoorOpen = true;
        ShowMessage("Door opened");
    }

    public void PressButton()
    {
        if (ButtonPressed)
        {
            return;
        }

        ButtonPressed = true;
        ShowMessage("Laser trap disabled");
    }

    public void ResetPlayer()
    {
        FindMissingReferences();

        if (playerTransform == null || playerSpawnPoint == null)
        {
            ShowMessage("Trap hit");
            return;
        }

        ShowMessage("Trap hit - reset");

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        playerTransform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        firstPersonController?.ResetView();

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    public void TryEscape()
    {
        if (Escaped)
        {
            return;
        }

        if (!HasKeycard)
        {
            ShowMessage("Need the keycard before escaping");
            return;
        }

        if (!MainDoorOpen)
        {
            ShowMessage("Open the main door before escaping");
            return;
        }

        Escaped = true;
        uiManager?.ShowEscaped();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = false;
        }
    }

    public void PlayAgain()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        if (uiManager != null)
        {
            uiManager.ShowMessage(message, duration);
        }

        Debug.Log(message);
    }

    private void FindMissingReferences()
    {
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform != null)
        {
            if (playerController == null)
            {
                playerController = playerTransform.GetComponent<CharacterController>();
            }

            if (firstPersonController == null)
            {
                firstPersonController = playerTransform.GetComponent<SimpleFirstPersonController>();
            }

            if (playerInteractor == null)
            {
                playerInteractor = playerTransform.GetComponent<PlayerInteractor>();
            }
        }
    }
}
