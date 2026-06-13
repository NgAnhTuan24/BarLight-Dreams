using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FailDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text totalEarningsText;
    [SerializeField] private TMP_Text totalCustomersServedText;
    [SerializeField] private TMP_Text totalCustomersAngryText;
    [SerializeField] private TMP_Text timeSurvivedText;
    [SerializeField] private Button retryDayButton;

    public void Refresh()
    {
        dayText.text = $"DAY {GameClock.instance.CurrentDay} SUMMARY";
        totalEarningsText.text = $"{DayStatsManager.instance.TotalMoneyEarned}";
        totalCustomersServedText.text = $"{DayStatsManager.instance.TotalServedCustomers}";
        totalCustomersAngryText.text = $"{DayStatsManager.instance.TotalAngryCustomers}";
        timeSurvivedText.text = $"{GameClock.instance.CurrentHour:00}:{GameClock.instance.CurrentMinute:00}";

        retryDayButton.transform.DOKill();

        retryDayButton.transform.localScale = Vector3.one;

        retryDayButton.transform
            .DOScale(1.05f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        retryDayButton.transform.DOKill();
        retryDayButton.transform.localScale = Vector3.one;
    }
}
