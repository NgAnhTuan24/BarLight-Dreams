using System.Collections;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private DrinkRecipeSO[] possibleOrders;
    [SerializeField] private AudioClip[] orderVoices;

    [SerializeField] private DrinkRecipeSO currentOrder;

    [Header("Alert Bubble")]
    [SerializeField] private GameObject alertBubble;

    [Header("Order Bubble")]
    [SerializeField] private GameObject drinkBubble;
    [SerializeField] private SpriteRenderer drinkIcon;

    [Header("Emotes Bubble")]
    [SerializeField] private GameObject happyBubble;
    [SerializeField] private GameObject angryBubble;

    [Header("Audio")]
    [SerializeField] private AudioClip collectionSFX;

    private CustomerController customer;
    private CustomerPatience patience;
    private CustomerPopupText popupText;

    public DrinkRecipeSO CurrentOrder => currentOrder;
    public bool HasOrdered { get; private set; }
    public GameObject AlertBubble => alertBubble;
    public GameObject DrinkBubble => drinkBubble;

    private void Awake()
    {
        customer = GetComponent<CustomerController>();
        patience = GetComponent<CustomerPatience>();
        popupText = GetComponentInChildren<CustomerPopupText>();

        alertBubble.SetActive(false);
        drinkBubble.SetActive(false);

        happyBubble.SetActive(false);
        angryBubble.SetActive(false);
    }

    public void ShowAlertBubble()
    {
        alertBubble.SetActive(true);
    }

    public void ShowHappyBubble()
    {
        happyBubble.SetActive(true);

        StartCoroutine(HideBubbleRoutine(happyBubble));
    }

    public void ShowAngryBubble()
    {
        angryBubble.SetActive(true);

        StartCoroutine(HideBubbleRoutine(angryBubble));
    }

    IEnumerator HideBubbleRoutine(GameObject target)
    {
        yield return new WaitForSeconds(3f);

        target.SetActive(false);
    }

    void PlayOrderVoice()
    {
        if (orderVoices.Length == 0) return;

        AudioClip clip = orderVoices[Random.Range(0, orderVoices.Length)];

        AudioManager.instance.PlaySFX(clip);
    }

    public void TakeOrder()
    {
        if (customer.CurrentState != CustomerState.WaitingOrder)
            return;

        alertBubble.SetActive(false);

        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        HasOrdered = true;

        ShowOrderBubble();

        PlayOrderVoice();

        Debug.Log("Customer ordered: " + currentOrder.drinkName);

        customer.ChangeState(CustomerState.WaitingDrink);

        patience.StartWaitingDrink();
    }

    void ShowOrderBubble()
    {
        drinkBubble.SetActive(true);

        drinkIcon.sprite = currentOrder.drinkIcon;
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

        drinkBubble.SetActive(false);

        patience.StopPatience();

        popupText.ShowText("Thanks!");

        ShowHappyBubble();

        MoneyManager.instance.AddMoney(currentOrder.price);

        DayStatsManager.instance.AddEarnings(currentOrder.price);
        DayStatsManager.instance.AddServedCustomer();

        AudioManager.instance.PlaySFX(collectionSFX);

        customer.OnDrinkReceived();
    }
}