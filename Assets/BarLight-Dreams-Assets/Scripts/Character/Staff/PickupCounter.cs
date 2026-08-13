using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupDrinkData
{
    public CustomerController customer;
    public DrinkRecipeSO recipe;
    public StaffController assignedStaff;

    public PickupDrinkData(CustomerController customer, DrinkRecipeSO recipe)
    {
        this.customer = customer;
        this.recipe = recipe;
        this.assignedStaff = null;
    }

    public bool IsReserved => assignedStaff != null;
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
        RefreshDrinkSlots();

        Debug.Log($"PickupCounter: Added {drinkData.recipe.displayName} for {drinkData.customer.name}");

        if (StaffManager.instance != null)
        {
            StaffManager.instance.TrySpawnStaff();
        }

        return true;
    }

    public bool HasUnclaimedDrink()
    {
        foreach (var drink in drinks)
        {
            if (!drink.IsReserved)
                return true;
        }
        return false;
    }

    public bool TryReserveDrink(StaffController staff, out PickupDrinkData reservedDrink)
    {
        reservedDrink = null;
        if (staff == null) return false;

        foreach (var drink in drinks)
        {
            if (drink.assignedStaff == staff)
            {
                reservedDrink = drink;
                return true;
            }
        }

        foreach (var drink in drinks)
        {
            if (!drink.IsReserved)
            {
                drink.assignedStaff = staff;
                reservedDrink = drink;
                return true;
            }
        }

        return false;
    }

    public PickupDrinkData TakeReservedDrink(StaffController staff)
    {
        for (int i = 0; i < drinks.Count; i++)
        {
            if (drinks[i].assignedStaff == staff)
            {
                PickupDrinkData drink = drinks[i];
                drinks.RemoveAt(i);
                RefreshDrinkSlots();
                return drink;
            }
        }
        return null;
    }

    public void CancelReservation(StaffController staff)
    {
        foreach (var drink in drinks)
        {
            if (drink.assignedStaff == staff)
            {
                drink.assignedStaff = null;
            }
        }
    }

    public bool TryPlaceDrink()
    {
        if (PlayerHoldItem.instance == null || !PlayerHoldItem.instance.HasDrink())
            return false;

        DrinkData drinkData = PlayerHoldItem.instance.CurrentDrinkData;
        if (drinkData == null || drinkData.recipe == null)
            return false;

        OrderData orderData = OrderQueueManager.instance.GetSelectedOrder();
        if (orderData == null)
            return false;

        if (orderData.recipe != drinkData.recipe)
            return false;

        PickupDrinkData pickupDrink = new PickupDrinkData(orderData.customer, drinkData.recipe);
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
