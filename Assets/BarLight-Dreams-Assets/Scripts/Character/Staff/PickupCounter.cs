using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupDrinkData
{
    public CustomerController customer;
    public DrinkRecipeSO recipe;

    public PickupDrinkData(CustomerController customer, DrinkRecipeSO recipe)
    {
        this.customer = customer;
        this.recipe = recipe;
    }
}

public class PickupCounter : MonoBehaviour
{
    public static PickupCounter instance;

    [Header("Pickup Settings")]
    [SerializeField] private int capacity = 3;
    [SerializeField] private PickupDrinkSlot[] drinkSlots;

    private readonly List<PickupDrinkData> drinks = new();

    private void Awake()
    {
        instance = this;
    }

    public bool HasSpace()
    {
        return drinks.Count < capacity;
    }

    public bool AddDrink(PickupDrinkData drinkData)
    {
        if (drinkData == null)
            return false;

        if (!HasSpace())
            return false;

        drinks.Add(drinkData);

        int index = drinks.Count - 1;

        if (index < drinkSlots.Length)
        {
            drinkSlots[index].SetDrink(drinkData.recipe);
        }

        Debug.Log($"PickupCounter: Added {drinkData.recipe.displayName} for {drinkData.customer.name}");

        if (StaffManager.instance != null)
        {
            StaffManager.instance.TrySpawnStaff();
        }

        return true;
    }

    public bool HasDrink()
    {
        return drinks.Count > 0;
    }

    public PickupDrinkData TakeNextDrink()
    {
        if (drinks.Count == 0)
            return null;

        PickupDrinkData drink = drinks[0];

        drinks.RemoveAt(0);

        RefreshDrinkSlots();

        return drink;
    }

    public bool TryPlaceDrink()
    {
        if (!PlayerHoldItem.instance.HasDrink())
            return false;

        DrinkData drinkData = PlayerHoldItem.instance.CurrentDrinkData;

        if (drinkData == null || drinkData.recipe == null)
            return false;

        OrderData orderData = OrderQueueManager.instance.GetSelectedOrder();

        if (orderData == null)
            return false;

        if (orderData.recipe != drinkData.recipe)
            return false;

        PickupDrinkData pickupDrink = new PickupDrinkData(orderData.customer,drinkData.recipe);

        if (!AddDrink(pickupDrink))
            return false;

        PlayerHoldItem.instance.Clear();

        return true;
    }

    private void RefreshDrinkSlots()
    {
        for (int i = 0; i < drinkSlots.Length; i++)
        {
            if (i < drinks.Count)
            {
                drinkSlots[i].SetDrink(drinks[i].recipe);
            }
            else
            {
                drinkSlots[i].Clear();
            }
        }
    }
}
