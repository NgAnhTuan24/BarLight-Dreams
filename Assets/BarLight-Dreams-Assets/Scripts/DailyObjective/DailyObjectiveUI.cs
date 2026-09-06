using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyObjectiveUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("UI")]
    [SerializeField] private TMP_Text objectiveTypeText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressPercentText;

    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [SerializeField] private Ease closeEase = Ease.InBack;

    private Tween currentTween;

    private bool wasCompleted;

    private void Start()
    {
        if (DailyObjectiveManager.instance == null)
        {
            return;
        }

        DailyObjectiveManager.instance.OnObjectiveChanged += UpdateUI;

        if (DailyObjectiveManager.instance.CurrentObjective != null)
        {
            UpdateUI(DailyObjectiveManager.instance.CurrentObjective);
        }
        else
        {
            ClearUI();
        }
    }

    private void OnDestroy()
    {
        if (DailyObjectiveManager.instance != null)
        {
            DailyObjectiveManager.instance.OnObjectiveChanged -= UpdateUI;
        }
    }

    private void UpdateUI(DailyObjective objective)
    {
        if (objective == null)
        {
            ClearUI();
            wasCompleted = false;
            return;
        }

        if (objectiveTypeText != null)
        {
            objectiveTypeText.text = GetObjectiveTypeName(objective.type);
        }

        if (objectiveText != null)
        {
            objectiveText.text = DailyObjectiveManager.instance.GetObjectiveDescription();
        }

        float progress = 0f;

        if (objective.target > 0)
        {
            progress = (float)objective.progress / objective.target;
        }

        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressPercentText != null)
        {
            if (objective.IsCompleted)
            {
                progressPercentText.text = "COMPLETED!";
            }
            else
            {
                progressPercentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            }
        }

        if (objective.IsCompleted)
        {
            if (progressText != null)
            {
                progressText.gameObject.SetActive(false);
            }

            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(true);
                rewardText.text = $"REWARD: +{objective.reward} Money";
            }

            if (!wasCompleted)
            {
                wasCompleted = true;
                OpenPanel();
            }
        }
        else
        {
            if (progressText != null)
            {
                progressText.gameObject.SetActive(true);
                progressText.text = $"Progress: {objective.progress} / {objective.target}";
            }

            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(false);
            }

            wasCompleted = false;
        }
    }

    private string GetObjectiveTypeName(DailyObjectiveType type)
    {
        switch (type)
        {
            case DailyObjectiveType.ServeCustomers:
            case DailyObjectiveType.ServeCustomerType:
                return "Serve Customers";

            case DailyObjectiveType.MixDrink:
                return "Mix Drink";

            case DailyObjectiveType.ServeDrink:
                return "Serve Drink";

            case DailyObjectiveType.EarnMoney:
                return "Earn Money";
        }

        return "Daily Objective";
    }

    public void OpenPanel()
    {
        if (panel == null)
            return;

        currentTween?.Kill();

        panel.SetActive(true);

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.transform.localScale = Vector3.one * 0.8f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(canvasGroup.DOFade(1f, animationDuration));

            sequence.Join(
                canvasGroup.transform.DOScale(
                    Vector3.one,
                    animationDuration
                ).SetEase(openEase)
            );

            currentTween = sequence;
        }
    }

    public void ClosePanel()
    {
        if (panel == null)
            return;

        currentTween?.Kill();

        if (canvasGroup == null)
        {
            panel.SetActive(false);
            return;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0f, animationDuration));

        sequence.Join(
            canvasGroup.transform.DOScale(
                Vector3.one * 0.8f,
                animationDuration
            ).SetEase(closeEase)
        );

        sequence.OnComplete(() =>
        {
            panel.SetActive(false);

            UIManager.Instance.UnlockGameplayInput();
            UIManager.Instance.UnlockPauseInput();

            canvasGroup.alpha = 1f;
            canvasGroup.transform.localScale = Vector3.one;
        });

        currentTween = sequence;
    }

    private void ClearUI()
    {
        if (objectiveTypeText != null)
        {
            objectiveTypeText.text = "";
        }

        if (objectiveText != null)
        {
            objectiveText.text = "";
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "";
        }

        if (rewardText != null)
        {
            rewardText.gameObject.SetActive(false);
            rewardText.text = "";
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (progressPercentText != null)
        {
            progressPercentText.text = "0%";
        }
    }
}