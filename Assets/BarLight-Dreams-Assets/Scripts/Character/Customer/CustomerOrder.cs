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

        if (OrderQueueManager.instance.IsFull)
            return;

        alertBubble.SetActive(false);

        patience.StopPatience();

        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        OrderQueueManager.instance.AddOrder(currentOrder);

        ShowOrderBubble();

        PlayOrderVoice();

        customer.ReleaseCounterSlot();

        customer.ChangeState(CustomerState.FindSeat);
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

    void TryGiveTip()
    {
        float finalTipChance = customer.Data.tipChance;

        float patienceUsed = patience.PatiencePercentUsed;

        if (patienceUsed > 0.8f)
        {
            finalTipChance *= 0.5f;
        }

        if (Random.value > finalTipChance) return;

        int tipAmount = Mathf.RoundToInt(currentOrder.price * Random.Range(0.1f, 0.5f) * customer.Data.tipMultiplier);

        DayStatsManager.instance.AddTips(tipAmount);

        popupText.ShowText($"+{tipAmount} Tip!");
    }

    void ReceiveDrink()
    {
        OrderQueueManager.instance.RemoveOrder(currentOrder);

        PlayerHoldItem.instance.Clear();

        drinkBubble.SetActive(false);

        patience.StopPatience();

        popupText.ShowText("Thanks!");

        ShowHappyBubble();

        TryGiveTip();

        DayStatsManager.instance.AddEarnings(currentOrder.price);
        DayStatsManager.instance.AddCustomersServed();

        AudioManager.instance.PlaySFX(collectionSFX);

        customer.OnDrinkReceived();
    }
}