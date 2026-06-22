using System.Collections.Generic;
using UnityEngine;

public class OrderQueueManager : MonoBehaviour
{
    public static OrderQueueManager instance;

    [Header("Slots")]
    [SerializeField] private OrderSlotUI[] slots;

    [SerializeField] private float stackOffset = 0f;

    private readonly List<DrinkRecipeSO> activeOrders = new List<DrinkRecipeSO>();

    private const int MAX_QUEUE = 6;

    public bool IsFull => activeOrders.Count >= MAX_QUEUE;

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

    public void AddOrder(DrinkRecipeSO recipe)
    {
        if (recipe == null)
            return;

        activeOrders.Add(recipe);

        RefreshUI();
    }

    public void RemoveOrder(DrinkRecipeSO recipe)
    {
        if (recipe == null)
            return;

        activeOrders.Remove(recipe);

        RefreshUI();
    }

    public void ClearAll()
    {
        activeOrders.Clear();

        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        int count = Mathf.Min(MAX_QUEUE, activeOrders.Count);

        for (int i = 0; i < count; i++)
        {
            slots[i].SetOrder(activeOrders[i]);
        }
    }

    private void SetupSlotPositions()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            RectTransform rect = slots[i].GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, -165 + i * stackOffset);
        }
    }
}