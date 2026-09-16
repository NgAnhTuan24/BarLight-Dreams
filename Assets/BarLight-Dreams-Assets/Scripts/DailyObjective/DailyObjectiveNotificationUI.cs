using DG.Tweening;
using TMPro;
using UnityEngine;

public class DailyObjectiveNotificationUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text rewardText;

    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float showDuration = 2f;

    private Sequence currentSequence;

    public void ShowObjectiveCompleted(DailyObjective objective)
    {
        if (objective == null)
        {
            return;
        }

        if (panel == null)
        {
            return;
        }

        currentSequence?.Kill();

        panel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = "COMPLETE!";
        }

        if (objectiveText != null)
        {
            if (DailyObjectiveManager.instance != null)
            {
                objectiveText.text = DailyObjectiveManager.instance.GetObjectiveDescription(objective);
            }
            else
            {
                objectiveText.text = "";
            }
        }

        if (rewardText != null)
        {
            rewardText.text = $"+{objective.reward} Money";
        }

        PlayNotificationAnimation();
    }

    public void ShowAllObjectivesCompleted(int totalReward)
    {
        if (panel == null)
        {
            return;
        }

        currentSequence?.Kill();

        panel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = "DAILY COMPLETE!";
        }

        if (objectiveText != null)
        {
            objectiveText.text = "All Daily Objectives Completed";
        }

        if (rewardText != null)
        {
            rewardText.text = $"Total Reward: +{totalReward} Money";
        }

        PlayNotificationAnimation();
    }

    private void PlayNotificationAnimation()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.transform.localScale = Vector3.one * 0.85f;

        currentSequence = DOTween.Sequence();

        currentSequence.Append(canvasGroup.DOFade(1f, fadeDuration));

        currentSequence.Join(
            canvasGroup.transform
                .DOScale(
                    Vector3.one,
                    fadeDuration
                )
                .SetEase(Ease.OutBack)
        );

        currentSequence.AppendInterval(showDuration);

        currentSequence.Append(canvasGroup.DOFade(0f, fadeDuration));

        currentSequence.OnComplete(() =>
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            canvasGroup.alpha = 1f;
            canvasGroup.transform.localScale = Vector3.one;
        });
    }

    public void Hide()
    {
        currentSequence?.Kill();

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.transform.localScale = Vector3.one;
        }
    }
}