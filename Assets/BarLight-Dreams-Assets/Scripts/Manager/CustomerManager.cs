using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance { get; private set; }

    private List<CustomerController> customers = new List<CustomerController>();

    [SerializeField] private float patienceMultiplier = 1f;
    [SerializeField] private float tipMultiplier = 1f;

    public int CurrentCustomerCount => customers.Count;

    public event System.Action<CustomerController> OnCustomerRegistered;

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

    private void Start()
    {
        GameClock.instance.OnBarClosed += ForceAllCustomersLeave;
    }

    public void SetPatienceMultiplier(float multiplier)
    {
        patienceMultiplier = multiplier;
    }

    public float GetPatienceMultiplier()
    {
        return patienceMultiplier;
    }

    public void SetTipMultiplier(float multiplier)
    {
        tipMultiplier = multiplier;
    }

    public float GetTipMultiplier()
    {
        return tipMultiplier;
    }

    public void RegisterCustomer(CustomerController customer)
    {
        if (customer == null)
            return;

        if (customers.Contains(customer))
            return;

        customers.Add(customer);

        OnCustomerRegistered?.Invoke(customer);
    }

    public void RemoveCustomer(CustomerController customer)
    {
        if (customers.Contains(customer))
        {
            customers.Remove(customer);
        }
    }

    public void ForceAllCustomersLeave()
    {
        foreach (CustomerController customer in customers)
        {
            customer.ForceLeave();
        }
    }

    private void OnDestroy()
    {
        GameClock.instance.OnBarClosed -= ForceAllCustomersLeave;
    }
}