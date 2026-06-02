using UnityEngine;

public class DayStatsManager : MonoBehaviour
{
    public static DayStatsManager instance { get; private set; }

    public int DayEarnings { get; private set; }
    public int CustomersServed { get; private set; }

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
    }

    public void AddServedCustomer()
    {
        CustomersServed++;
    }

    public void ResetDay()
    {
        DayEarnings = 0;
        CustomersServed = 0;
    }
}