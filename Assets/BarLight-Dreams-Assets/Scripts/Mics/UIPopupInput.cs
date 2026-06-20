using UnityEngine;

public class UIPopupInput : MonoBehaviour
{
    [SerializeField] private UIPopup uiPopup;
    [SerializeField] private KeyCode keyCode = KeyCode.C;

    [SerializeField] private InputType inputType;

    void Update()
    {
        if (UIManager.Instance == null) return;

        switch (inputType)
        {
            case InputType.Gameplay:
                if (UIManager.Instance.IsGameplayInputLocked) return;
                break;

            case InputType.Pause:
                if (UIManager.Instance.IsPauseInputLocked) return;
                break;
        }

        if (Input.GetKeyDown(keyCode))
        {
            uiPopup.Toggle();
        }
    }
}

public enum InputType
{
    None,
    Gameplay,
    Pause
}
