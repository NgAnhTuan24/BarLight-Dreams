using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public static CounterManager instance;

    [SerializeField] private CounterSlot[] slots;
    [SerializeField] private int capacity = 1;

    private void Awake()
    {
        instance = this;
    }

    public CounterSlot ReserveSlot(CustomerController customer)
    {
        int startIndex = Random.Range(0, capacity);

        for (int i = 0; i < capacity; i++)
        {
            int index = (startIndex + i) % capacity;

            if (!slots[index].IsOccupied)
            {
                slots[index].Occupy(customer);
                return slots[index];
            }
        }

        return null;
    }

    public void SetCapacity(int newCapacity)
    {
        capacity = Mathf.Clamp(newCapacity, 1, slots.Length);
    }
}