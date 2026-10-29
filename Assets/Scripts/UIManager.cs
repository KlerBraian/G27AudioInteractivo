using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text promptText;
    public Text messageText;
    public Text inventoryText;
    public Text endHelpText;
    public Button playAgainButton;
    public float defaultMessageDuration = 2f;

    private Coroutine messageRoutine;

    private void Awake()
    {
        HidePrompt();
        SetKeycardAcquired(false);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        HideEndOptions();
    }

    public void ShowPrompt(string prompt)
    {
        if (promptText == null)
        {
            return;
        }

        promptText.text = prompt;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText != null)
        {
            promptText.text = string.Empty;
            promptText.gameObject.SetActive(false);
        }
    }

    public void SetKeycardAcquired(bool acquired)
    {
        if (inventoryText != null)
        {
            inventoryText.text = acquired ? "Keycard: Acquired" : "Keycard: Missing";
        }
    }

    public void ShowMessage(string message, float duration = -1f)
    {
        if (messageText == null)
        {
            return;
        }

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
            messageRoutine = null;
        }

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        if (Mathf.Approximately(duration, 0f))
        {
            return;
        }

        float actualDuration = duration > 0f ? duration : defaultMessageDuration;
        messageRoutine = StartCoroutine(HideMessageAfterDelay(actualDuration));
    }

    public void ShowEscaped()
    {
        HidePrompt();
        ShowMessage("Escaped!", 0f);

        if (endHelpText != null)
        {
            endHelpText.text = "Press R or click Play Again";
            endHelpText.gameObject.SetActive(true);
        }

        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(true);
        }
    }

    public void SetPlayAgainHandler(UnityAction playAgainAction)
    {
        if (playAgainButton == null)
        {
            return;
        }

        playAgainButton.onClick.RemoveAllListeners();
        playAgainButton.onClick.AddListener(playAgainAction);
    }

    private void HideEndOptions()
    {
        if (endHelpText != null)
        {
            endHelpText.gameObject.SetActive(false);
        }

        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        messageRoutine = null;
    }
}
