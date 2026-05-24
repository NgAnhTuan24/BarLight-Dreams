using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private DrinkRecipeSO[] possibleOrders;

    [SerializeField] private DrinkRecipeSO currentOrder;

    [Header("Bubble")]
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private SpriteRenderer bubbleDrinkIcon;

    private CustomerController customer;

    public DrinkRecipeSO CurrentOrder => currentOrder;
    public bool HasOrdered { get; private set; }

    private void Awake()
    {
        customer = GetComponent<CustomerController>();
    }

    public void TakeOrder()
    {
        if (customer.CurrentState != CustomerState.WaitingOrder)
            return;

        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        HasOrdered = true;

        ShowOrderBubble();

        Debug.Log("Customer ordered: " + currentOrder.drinkName);

        customer.ChangeState(CustomerState.WaitingDrink);
    }

    void ShowOrderBubble()
    {
        bubbleRoot.SetActive(true);

        bubbleDrinkIcon.sprite = currentOrder.drinkIcon;
    }

    public void TryGiveDrink()
    {
        if (customer.CurrentState != CustomerState.WaitingDrink)
            return;

        if (!PlayerHoldItem.instance.HasDrink())
            return;

        DrinkData drinkData = PlayerHoldItem.instance.CurrentDrinkData;

        if (drinkData == null)
            return;

        if (drinkData.recipe == currentOrder)
        {
            ReceiveDrink();
        }
        else
        {
            Debug.Log("Wrong drink!");
        }
    }

    void ReceiveDrink()
    {
        Debug.Log("Correct Drink!");

        PlayerHoldItem.instance.Clear();

        bubbleRoot.SetActive(false);

        customer.OnDrinkReceived();
    }
}