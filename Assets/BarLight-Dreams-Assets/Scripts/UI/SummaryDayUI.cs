using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text earningsTodayText;
    [SerializeField] private TMP_Text customersSurvedText;
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

        dayText.text = $"{day}";

        canvasGroup.alpha = 0;
        panel.localScale = Vector3.zero;

        earningsTodayText.text = "0";
        customersSurvedText.text = "0";

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.25f));

        seq.Join(
            panel.DOScale(1f, 0.8f).SetEase(Ease.OutBack)
        );

        seq.AppendInterval(0.2f);

        seq.Append(
            DOTween.To(
                () => 0,
                x => earningsTodayText.text = x.ToString(),
                earnings,
                0.9f
            )
        );

        seq.Join(
            DOTween.To(
                () => 0,
                x => customersSurvedText.text = x.ToString(),
                customers,
                0.9f
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
