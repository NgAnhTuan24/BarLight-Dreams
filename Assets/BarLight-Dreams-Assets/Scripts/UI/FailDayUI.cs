using TMPro;
using UnityEngine;

public class FailDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text totalEarningsText;
    [SerializeField] private TMP_Text totalCustomersServedText;
    [SerializeField] private TMP_Text totalCustomersAngryText;
    [SerializeField] private TMP_Text timeSurvivedText;

    public void Refresh()
    {
        dayText.text = $"DAY {GameClock.instance.CurrentDay} SUMMARY";
        totalEarningsText.text = $"{DayStatsManager.instance.TotalMoneyEarned}";
        totalCustomersServedText.text = $"{DayStatsManager.instance.TotalCustomersServed}";
        totalCustomersAngryText.text = $"{DayStatsManager.instance.TotalCustomersAngry}";
        timeSurvivedText.text = $"{GameClock.instance.CurrentHour:00}:{GameClock.instance.CurrentMinute:00}";
    }
}
