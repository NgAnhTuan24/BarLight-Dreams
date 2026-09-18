using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FinalDayUI : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [Header("Animation")]
    [SerializeField] private float showDuration = 0.7f;
    [SerializeField] private float hideDuration = 0.5f;

    private void Awake()
    {
        okButton.onClick.AddListener(OnClickOK);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        canvasGroup.DOKill();
        panel.DOKill();

        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;

        canvasGroup
            .DOFade(1f, showDuration)
            .SetEase(Ease.OutQuad);

        panel
            .DOScale(1f, showDuration)
            .SetEase(Ease.OutBack);
    }

    private void OnClickOK()
    {
        okButton.interactable = false;

        canvasGroup.DOKill();
        panel.DOKill();

        canvasGroup
            .DOFade(0f, hideDuration)
            .SetEase(Ease.InQuad);

        panel
            .DOScale(0.9f, hideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);

                panel.localScale = Vector3.one;
                canvasGroup.alpha = 1f;

                okButton.interactable = true;

                UIManager.Instance.UnlockGameplayInput();
                UIManager.Instance.UnlockPauseInput();

                GameClock.instance.StartGameplay();
            });
    }

    private void OnDestroy()
    {
        if (okButton != null)
        {
            okButton.onClick.RemoveListener(OnClickOK);
        }

        canvasGroup.DOKill();
        panel.DOKill();
    }
}