using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public static CounterManager instance;

    [SerializeField] private CounterSlot[] slots;

    private void Awake()
    {
        instance = this;
    }

    public CounterSlot ReserveSlot(CustomerController customer)
    {
        int startIndex = Random.Range(0, slots.Length);

        for (int i = 0; i < slots.Length; i++)
        {
            int index = (startIndex + i) % slots.Length;

            if (!slots[index].IsOccupied)
            {
                slots[index].Occupy(customer);
                return slots[index];
            }
        }

        return null;
    }
}