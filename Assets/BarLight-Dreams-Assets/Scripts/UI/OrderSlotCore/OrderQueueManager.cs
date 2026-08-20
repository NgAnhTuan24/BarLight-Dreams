using UnityEngine;

[System.Serializable]
public class OrderData
{
    public CustomerController customer;
    public DrinkRecipeSO recipe;

    public OrderData(CustomerController customer, DrinkRecipeSO recipe)
    {
        this.customer = customer;
        this.recipe = recipe;
    }
}

public class OrderQueueManager : MonoBehaviour
{
    public static OrderQueueManager instance;

    [Header("Slots")]
    [SerializeField] private OrderSlotUI[] slots;
    [SerializeField] private OrderRecipeDetailViewer orderRecipeDetailViewer;

    [SerializeField] private float stackOffset = 0f;

    private const int MAX_QUEUE = 6;
    [SerializeField] private int slotCount = 2;

    private OrderData[] activeOrders = new OrderData[MAX_QUEUE];

    private OrderSlotUI currentSelectedSlot;

    public bool IsFull
    {
        get
        {
            for (int i = 0; i < slotCount; i++)
            {
                if (activeOrders[i] == null)
                    return false;
            }

            return true;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SetupSlotPositions();   

        RefreshUI();
    }

    public void AddOrder(CustomerController customer, DrinkRecipeSO recipe)
    {
        if (recipe == null || customer == null) return;

        for (int i = 0; i < slotCount; i++)
        {
            if (activeOrders[i] == null)
            {
                activeOrders[i] = new OrderData(customer, recipe);
                slots[i].SetOrder(recipe);
                return;
            }
        }
    }

    public void RemoveOrder(CustomerController customer)
    {
        if (customer == null) return;

        for (int i = 0; i < MAX_QUEUE; i++)
        {
            if (activeOrders[i] != null && activeOrders[i].customer == customer)
            {
                if (currentSelectedSlot == slots[i])
                {
                    DeselectSlot();
                }

                activeOrders[i] = null;
                slots[i].ClearSlot();
                return;
            }
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < MAX_QUEUE; i++)
        {
            bool unlocked = i < slotCount;

            slots[i].gameObject.SetActive(unlocked);

            if (!unlocked)
                continue;

            if (activeOrders[i] != null)
                slots[i].SetOrder(activeOrders[i].recipe);
            else
                slots[i].ClearSlot();
        }
    }

    private void SetupSlotPositions()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            RectTransform rect = slots[i].GetComponent<RectTransform>();

            Vector2 pos = new Vector2(0, -165 + i * stackOffset);

            rect.anchoredPosition = pos;
            slots[i].SetOriginalPosition(pos);
        }
    }

    public void SelectSlot(OrderSlotUI slot)
    {
        if (slot == null || slot.CurrentRecipe == null) return;

        if (currentSelectedSlot == slot)
        {
            if (orderRecipeDetailViewer.IsShowing)
            {
                orderRecipeDetailViewer.Hide();
            }
            else
            {
                orderRecipeDetailViewer.Show(slot.CurrentRecipe);
            }

            return;
        }

        if (currentSelectedSlot != null) currentSelectedSlot.SetHighlight(false);

        currentSelectedSlot = slot;

        if (currentSelectedSlot != null) currentSelectedSlot.SetHighlight(true);

        orderRecipeDetailViewer.Show(slot.CurrentRecipe);
    }

    public void DeselectSlot()
    {
        if (currentSelectedSlot != null)
        {
            currentSelectedSlot.SetHighlight(false);
            currentSelectedSlot = null;
        }
        orderRecipeDetailViewer.Hide();
    }

    public void SetSlotCount(int count)
    {
        slotCount = Mathf.Clamp(count, 1, MAX_QUEUE);

        RefreshUI();
    }

    public OrderData GetSelectedOrder()
    {
        if (currentSelectedSlot == null)
            return null;

        for (int i = 0; i < MAX_QUEUE; i++)
        {
            if (activeOrders[i] == null)
                continue;

            if (slots[i] == currentSelectedSlot)
                return activeOrders[i];
        }

        return null;
    }
}