using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [SerializeField] private Button backButton;
    [SerializeField] private SummaryDayUI summaryDayUI;

    [SerializeField] private float startX = 1800f;

    private void Awake()
    {
        backButton.onClick.AddListener(OnClickBack);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;

        panel.anchoredPosition = new Vector2(startX, 0);

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.25f));

        seq.Join(
            panel.DOAnchorPos(Vector2.zero, 0.45f)
                 .SetEase(Ease.OutCubic)
        );
    }

    private void OnClickBack()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(0f, 0.2f));

        seq.Join(
            panel.DOAnchorPos(new Vector2(startX, 0), 0.35f)
                 .SetEase(Ease.InCubic)
        );

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);

            summaryDayUI.ShowFromUpgrade();
        });
    }
}