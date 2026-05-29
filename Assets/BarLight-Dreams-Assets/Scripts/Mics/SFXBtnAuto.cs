using UnityEngine;
using UnityEngine.UI;

public class SFXBtnAuto : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayClickSound);
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(clickSound);
        }
    }
}
