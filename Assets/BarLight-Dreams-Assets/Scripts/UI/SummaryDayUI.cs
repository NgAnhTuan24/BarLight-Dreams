using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text earningsTodayText;
    [SerializeField] private TMP_Text tipsTodayText;
    [SerializeField] private TMP_Text customersServedText;
    [SerializeField] private TMP_Text totalIncomeText;
    [SerializeField] private Button nextButton;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [SerializeField] private GameObject[] objectsToHide;

    private event Action onNext;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnClickNext);
    }

    public void Show(int day, int earnings, int tips, int customers, Action nextCallback)
    {
        gameObject.SetActive(true);

        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        onNext = nextCallback;

        dayText.text = $"{day}";

        canvasGroup.alpha = 0;
        panel.localScale = Vector3.zero;

        earningsTodayText.text = "0";
        tipsTodayText.text = "0";
        customersServedText.text = "0";
        totalIncomeText.text = "0";

        int totalIncome = earnings + tips;

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
                x => tipsTodayText.text = x.ToString(),
                tips,
                0.9f
            )
        );

        seq.Join(
            DOTween.To(
                () => 0,
                x => customersServedText.text = x.ToString(),
                customers,
                0.9f
            )
        );

        seq.Join(
            DOTween.To(
                () => 0,
                x => totalIncomeText.text = x.ToString(),
                totalIncome,
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
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            gameObject.SetActive(false);
            onNext?.Invoke();
        });
    }
}
