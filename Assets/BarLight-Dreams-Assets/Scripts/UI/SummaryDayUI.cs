using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text earningsText;
    [SerializeField] private TMP_Text customerText;
    [SerializeField] private Button nextButton;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    private event Action onNext;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnClickNext);
    }

    public void Show(int day, int earnings, int customers, Action nextCallback)
    {
        gameObject.SetActive(true);

        onNext = nextCallback;

        dayText.text = $"DAY {day}";

        canvasGroup.alpha = 0;
        panel.localScale = Vector3.zero;

        earningsText.text = "0";
        customerText.text = "0";

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.25f));

        seq.Join(
            panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack)
        );

        seq.AppendInterval(0.1f);

        seq.Append(
            DOTween.To(
                () => 0,
                x => earningsText.text = x.ToString(),
                earnings,
                0.8f
            )
        );

        seq.Join(
            DOTween.To(
                () => 0,
                x => customerText.text = x.ToString(),
                customers,
                0.8f
            )
        );

        seq.OnComplete(() =>
        {
            nextButton.transform
                .DOScale(1.08f, 0.6f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        });
    }

    void OnClickNext()
    {
        nextButton.transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(
            canvasGroup.DOFade(0f, 0.2f)
        );

        seq.Join(
            panel.DOScale(0f, 0.2f).SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onNext?.Invoke();
        });
    }
}
