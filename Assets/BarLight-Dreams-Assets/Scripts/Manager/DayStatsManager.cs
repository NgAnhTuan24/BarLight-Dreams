using UnityEngine;

public class DayStatsManager : MonoBehaviour
{
    public static DayStatsManager instance { get; private set; }

    public int MoneyEarnedToday { get; private set; }
    public int ServedCustomersToday { get; private set; }
    public int AngryCustomersToday { get; private set; }

    public int TotalMoneyEarned { get; private set; }
    public int TotalServedCustomers { get; private set; }
    public int TotalAngryCustomers { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEarnings(int amount)
    {
        MoneyEarnedToday += amount;
        TotalMoneyEarned += amount;
    }

    public void AddCustomersServed()
    {
        ServedCustomersToday++;
        TotalServedCustomers++;
    }

    public void AddCustomersAngry(int amount)
    {
        AngryCustomersToday += amount;
        TotalAngryCustomers += amount;
    }

    public void ResetDay()
    {
        MoneyEarnedToday = 0;
        ServedCustomersToday = 0;
        AngryCustomersToday = 0;
    }

    public void LoadTotalStats(int money, int served, int angry)
    {
        TotalMoneyEarned = money;
        TotalServedCustomers = served;
        TotalAngryCustomers = angry;
    }
}