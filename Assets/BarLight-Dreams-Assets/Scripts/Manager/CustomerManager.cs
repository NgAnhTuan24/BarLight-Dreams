using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance { get; private set; }

    private List<CustomerController> customers = new List<CustomerController>();

    public int CurrentCustomerCount => customers.Count;

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

    public void RegisterCustomer(CustomerController customer)
    {
        if (!customers.Contains(customer))
        {
            customers.Add(customer);
        }
    }

    public void RemoveCustomer(CustomerController customer)
    {
        if (customers.Contains(customer))
        {
            customers.Remove(customer);
        }
    }
}