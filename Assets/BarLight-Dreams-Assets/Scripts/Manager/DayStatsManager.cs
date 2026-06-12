using UnityEngine;

public class DayStatsManager : MonoBehaviour
{
    public static DayStatsManager instance { get; private set; }

    public int DayEarnings { get; private set; }
    public int CustomersServed { get; private set; }
    public int TotalCustomersServed { get; private set; }
    public int TotalMoneyEarned { get; private set; }

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
        DayEarnings += amount;
        TotalMoneyEarned += amount;
    }

    public void AddServedCustomer()
    {
        CustomersServed++;
        TotalCustomersServed++;
    }

    public void ResetDay()
    {
        DayEarnings = 0;
        CustomersServed = 0;
    }
}