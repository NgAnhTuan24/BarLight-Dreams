using UnityEngine;

public class CounterSlot : MonoBehaviour
{
    public CustomerController Occupier { get; private set; }

    public bool IsOccupied => Occupier != null;

    public void Occupy(CustomerController customer)
    {
        Occupier = customer;
    }

    public void Release()
    {
        Occupier = null;
    }
}