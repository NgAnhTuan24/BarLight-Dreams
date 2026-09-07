using System.Collections;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private DrinkRecipeSO[] possibleOrders;
    [SerializeField] private AudioClip[] orderVoices;

    [SerializeField] private DrinkRecipeSO currentOrder;

    [Header("Favorite Drink")]
    [SerializeField, Range(0f, 1f)] private float favoriteDrinkChance = 0.3f;
    private bool isFavoriteOrder;

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
    private FloatingPopupText popupText;

    public event System.Action<DrinkRecipeSO> OnOrderTaken;
    public event System.Action OnDrinkServed;

    public DrinkRecipeSO CurrentOrder => currentOrder;
    public GameObject AlertBubble => alertBubble;
    public GameObject DrinkBubble => drinkBubble;

    private void Awake()
    {
        customer = GetComponent<CustomerController>();
        patience = GetComponent<CustomerPatience>();
        popupText = GetComponentInChildren<FloatingPopupText>();

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

        isFavoriteOrder = false;
        currentOrder = null;

        bool isVIP = customer.Data != null && customer.Data.customerType == CustomerType.VIP;

        RecipeTier highestTier = RecipeTier.Tier1;

        if (isVIP)
        {
            if (RecipeProgressionManager.instance == null)
                return;

            highestTier = RecipeProgressionManager.instance.GetHighestUnlockedTier();
        }

        int candidateOrderCount = 0;

        foreach (DrinkRecipeSO recipe in possibleOrders)
        {
            if (recipe == null)
                continue;

            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
                continue;

            if (isVIP && recipe.recipeTier != highestTier)
                continue;

            candidateOrderCount++;
        }

        if (candidateOrderCount == 0)
            return;

        alertBubble.SetActive(false);
        patience.StopPatience();

        if (Random.value < favoriteDrinkChance)
        {
            DrinkRecipeSO favoriteRecipe;

            if (isVIP)
            {
                favoriteRecipe = GetUnlockedFavoriteDrinkForVIP(highestTier);
            }
            else
            {
                favoriteRecipe = GetUnlockedFavoriteDrink();
            }

            if (favoriteRecipe != null)
            {
                currentOrder = favoriteRecipe;
                isFavoriteOrder = true;
            }
        }

        if (currentOrder == null || !isFavoriteOrder)
        {
            int randomIndex = Random.Range(0, candidateOrderCount);

            foreach (DrinkRecipeSO recipe in possibleOrders)
            {
                if (recipe == null)
                    continue;

                if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
                    continue;

                if (isVIP && recipe.recipeTier != highestTier)
                    continue;

                if (randomIndex == 0)
                {
                    currentOrder = recipe;
                    break;
                }

                randomIndex--;
            }

            isFavoriteOrder = false;
        }

        if (currentOrder == null)
            return;

        OrderQueueManager.instance.AddOrder(customer, currentOrder);

        OnOrderTaken?.Invoke(currentOrder);

        ShowOrderBubble();

        PlayOrderVoice();

        customer.ReleaseCounterSlot();

        customer.ChangeState(CustomerState.FindSeat);
    }

    private DrinkRecipeSO GetUnlockedFavoriteDrink()
    {
        if (customer.Data.favoriteDrinks == null || customer.Data.favoriteDrinks.Length == 0)
        {
            return null;
        }

        int unlockedFavoriteCount = 0;

        foreach (DrinkRecipeSO recipe in customer.Data.favoriteDrinks)
        {
            if (recipe == null)
                continue;

            if (RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
            {
                unlockedFavoriteCount++;
            }
        }

        if (unlockedFavoriteCount == 0)
            return null;

        int randomIndex = Random.Range(0, unlockedFavoriteCount);

        foreach (DrinkRecipeSO recipe in customer.Data.favoriteDrinks)
        {
            if (recipe == null)
                continue;

            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
                continue;

            if (randomIndex == 0)
                return recipe;

            randomIndex--;
        }

        return null;
    }

    private DrinkRecipeSO GetUnlockedFavoriteDrinkForVIP(RecipeTier highestTier)
    {
        if (customer == null || customer.Data == null)
            return null;

        if (customer.Data.favoriteDrinks == null || customer.Data.favoriteDrinks.Length == 0)
            return null;

        if (possibleOrders == null || possibleOrders.Length == 0)
            return null;

        int validFavoriteCount = 0;

        foreach (DrinkRecipeSO favoriteRecipe in customer.Data.favoriteDrinks)
        {
            if (favoriteRecipe == null)
                continue;

            if (favoriteRecipe.recipeTier != highestTier)
                continue;

            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(favoriteRecipe))
                continue;

            if (!IsRecipeInPossibleOrders(favoriteRecipe))
                continue;

            validFavoriteCount++;
        }

        if (validFavoriteCount == 0)
            return null;

        int randomIndex = Random.Range(0, validFavoriteCount);

        foreach (DrinkRecipeSO favoriteRecipe in customer.Data.favoriteDrinks)
        {
            if (favoriteRecipe == null)
                continue;

            if (favoriteRecipe.recipeTier != highestTier)
                continue;

            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(favoriteRecipe))
                continue;

            if (!IsRecipeInPossibleOrders(favoriteRecipe))
                continue;

            if (randomIndex == 0)
                return favoriteRecipe;

            randomIndex--;
        }

        return null;
    }

    private bool IsRecipeInPossibleOrders(DrinkRecipeSO targetRecipe)
    {
        if (targetRecipe == null || possibleOrders == null)
            return false;

        foreach (DrinkRecipeSO recipe in possibleOrders)
        {
            if (recipe == targetRecipe)
                return true;
        }

        return false;
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
            PlayerHoldItem.instance.Clear();
        }
        else
        {
            popupText.ShowText(PopupMessages.GetWrongDrinkMessage());
        }
    }

    int TryGiveTip()
    {
        float finalTipChance = customer.Data.tipChance;

        float patienceUsed = patience.PatiencePercentUsed;

        if (patienceUsed > 0.8f)
        {
            finalTipChance *= 0.5f;
        }

        if (Random.value > finalTipChance) return 0;

        float finalTipMultiplier = customer.Data.tipMultiplier;

        if (isFavoriteOrder)
        {
            finalTipMultiplier *= 1.5f;
        }

        int tipAmount = Mathf.RoundToInt(currentOrder.price * Random.Range(0.1f, 0.5f) * finalTipMultiplier * CustomerManager.instance.GetTipMultiplier());

        DayStatsManager.instance.AddTips(tipAmount);

        return tipAmount;
    }

    public void ReceiveDrink()
    {
        OrderQueueManager.instance.RemoveOrder(customer);

        drinkBubble.SetActive(false);

        patience.StopPatience();

        ShowHappyBubble();

        int tipAmount = TryGiveTip();

        if (tipAmount > 0)
        {
            popupText.ShowText(PopupMessages.GetTipMessage(tipAmount));
        }
        else if (patience.PatiencePercentUsed > 0.8f)
        {
            popupText.ShowText(PopupMessages.GetSlowMessage());
        }
        else
        {
            popupText.ShowText(PopupMessages.GetThanksMessage());
        }

        DayStatsManager.instance.AddEarnings(currentOrder.price);
        DayStatsManager.instance.AddCustomersServed();

        if (DailyObjectiveManager.instance != null)
        {
            DailyObjectiveManager.instance.RegisterCustomerServed(customer, currentOrder);

            DailyObjectiveManager.instance.RefreshEarnMoneyProgress();
        }

        AudioManager.instance.PlaySFX(collectionSFX);

        OnDrinkServed?.Invoke();

        customer.OnDrinkReceived();
    }
}