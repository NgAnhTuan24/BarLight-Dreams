using UnityEngine;

public class DayStatsManager : MonoBehaviour
{
    public static DayStatsManager instance { get; private set; }

    public int EarningsToDay { get; private set; }
    public int CustomersServed { get; private set; }
    public int CustomersAngry { get; private set; }

    public int TotalCustomersServed { get; private set; }
    public int TotalMoneyEarned { get; private set; }
    public int TotalCustomersAngry { get; private set; }

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
        EarningsToDay += amount;
        TotalMoneyEarned += amount;
    }

    public void AddCustomersServed()
    {
        CustomersServed++;
        TotalCustomersServed++;
    }

    public void AddCustomersAngry(int amount)
    {
        CustomersAngry += amount;
        TotalCustomersAngry++;
    }

    public void ResetDay()
    {
        EarningsToDay = 0;
        CustomersServed = 0;
        CustomersAngry = 0;
    }
}