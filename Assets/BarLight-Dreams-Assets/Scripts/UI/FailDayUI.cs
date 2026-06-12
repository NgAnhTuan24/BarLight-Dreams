using TMPro;
using UnityEngine;

public class FailDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text customerText;

    public void Refresh()
    {
        dayText.text = $"DAY {GameClock.instance.CurrentDay} SUMMARY";
        moneyText.text = $"{DayStatsManager.instance.TotalMoneyEarned}";
        customerText.text = $"{DayStatsManager.instance.TotalCustomersServed}";
    }
}
