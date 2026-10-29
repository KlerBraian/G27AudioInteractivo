using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    public Camera playerCamera;
    public float moveSpeed = 4f;
    public float lookSensitivity = 0.08f;
    public float gravity = -18f;

    private CharacterController controller;
    private float pitch;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            LockCursor();
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            UnlockCursor();
        }
    }

    private void Update()
    {
        HandleCursorState();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleLook();
        }

        HandleMovement();
    }

    public void ResetView()
    {
        pitch = 0f;

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.identity;
        }
    }

    private void HandleCursorState()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    private void HandleLook()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || playerCamera == null)
        {
            return;
        }

        Vector2 delta = mouse.delta.ReadValue();
        transform.Rotate(Vector3.up * (delta.x * lookSensitivity));

        pitch -= delta.y * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -75f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || controller == null)
        {
            return;
        }

        Vector2 moveInput = ReadMoveInput(keyboard);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = move * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private static Vector2 ReadMoveInput(Keyboard keyboard)
    {
        Vector2 input = Vector2.zero;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            input.x -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            input.x += 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            input.y -= 1f;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            input.y += 1f;
        }

        return input;
    }

    private static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
