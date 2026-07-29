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

    [SerializeField] private UpgradeUI upgradeUI;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [SerializeField] private GameObject[] objectsToHide;

    private event Action onNext;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnClickNext);
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    public void Show(int day, int earnings, int tips, int customers, Action nextCallback)
    {
        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

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

        seq.AppendInterval(0.1f);

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
    }

    public void ShowFromUpgrade()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        panel.anchoredPosition = new Vector2(-1800f, 0);

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.2f));

        seq.Join(
            panel.DOAnchorPos(Vector2.zero, 0.35f)
                 .SetEase(Ease.OutCubic)
        );
    }

    void OnClickNext()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(0f, 0.2f));

        seq.Join(
            panel.DOScale(0f, 0.2f)
                 .SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            UIManager.Instance.UnlockGameplayInput();
            UIManager.Instance.UnlockPauseInput();

            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            gameObject.SetActive(false);
            onNext?.Invoke();
        });
    }

    private void OnClickUpgrade()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(0f, 0.2f));

        seq.Join(
            panel.DOAnchorPos(new Vector2(-1800f, 0), 0.35f)
                 .SetEase(Ease.InCubic)
        );

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);

            panel.anchoredPosition = Vector2.zero;

            upgradeUI.Show();
        });
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.UnlockGameplayInput();
    }
}
