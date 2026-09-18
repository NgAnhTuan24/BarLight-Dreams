using DG.Tweening;
using UnityEngine;

public class DailyObjectiveUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Objective Items")]
    [SerializeField] private DailyObjectiveItemUI[] objectiveItems;

    [Header("Optional Notification")]
    [SerializeField] private DailyObjectiveNotificationUI notificationUI;

    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showDuration = 0.2f;
    [SerializeField] private float hideDuration = 0.15f;
    [SerializeField] private float startScale = 0.8f;

    private RectTransform panelRect;

    private void Awake()
    {
        if (panel != null)
        {
            panelRect = panel.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        if (DailyObjectiveManager.instance == null)
        {
            return;
        }

        DailyObjectiveManager.instance.OnObjectivesChanged += UpdateUI;

        DailyObjectiveManager.instance.OnObjectiveCompleted += HandleObjectiveCompleted;

        DailyObjectiveManager.instance.OnAllObjectivesCompleted += HandleAllObjectivesCompleted;

        UpdateUI(DailyObjectiveManager.instance.CurrentObjectives);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (panelRect != null)
        {
            panelRect.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        if (DailyObjectiveManager.instance == null)
        {
            return;
        }

        DailyObjectiveManager.instance.OnObjectivesChanged -= UpdateUI;

        DailyObjectiveManager.instance.OnObjectiveCompleted -= HandleObjectiveCompleted;

        DailyObjectiveManager.instance.OnAllObjectivesCompleted -= HandleAllObjectivesCompleted;

        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
        }

        if (panelRect != null)
        {
            panelRect.DOKill();
        }
    }

    private void UpdateUI(DailyObjective[] objectives)
    {
        if (objectiveItems == null)
        {
            return;
        }

        for (int i = 0; i < objectiveItems.Length; i++)
        {
            if (objectiveItems[i] == null)
            {
                continue;
            }

            if (objectives != null && i < objectives.Length && objectives[i] != null)
            {
                objectiveItems[i].SetObjective(objectives[i]);
            }
            else
            {
                objectiveItems[i].ClearUI();
            }
        }
    }

    private void HandleObjectiveCompleted(DailyObjective objective)
    {
        if (objective == null)
        {
            return;
        }

        if (notificationUI != null)
        {
            notificationUI.ShowObjectiveCompleted(objective);
        }
    }

    private void HandleAllObjectivesCompleted(int totalReward)
    {
        if (notificationUI != null)
        {
            notificationUI.ShowAllObjectivesCompleted(totalReward);
        }
    }

    public void ShowPanel()
    {
        if (panel == null)
        {
            return;
        }

        canvasGroup?.DOKill();
        panelRect?.DOKill();

        panel.SetActive(true);

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (panelRect != null)
        {
            panelRect.localScale = Vector3.one * startScale;
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOFade(1f, showDuration).SetEase(Ease.OutQuad);
        }

        if (panelRect != null)
        {
            panelRect.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);
        }
    }

    public void HidePanel()
    {
        if (panel == null)
        {
            return;
        }

        canvasGroup?.DOKill();
        panelRect?.DOKill();

        if (canvasGroup != null)
        {
            canvasGroup
                .DOFade(0f, hideDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    panel.SetActive(false);

                    if (panelRect != null)
                    {
                        panelRect.localScale = Vector3.one;
                    }

                    UIManager.Instance.UnlockGameplayInput();
                    UIManager.Instance.UnlockPauseInput();
                });
        }
        else
        {
            panel.SetActive(false);

            if (panelRect != null)
            {
                panelRect.localScale = Vector3.one;
            }
        }
    }
}