using System;
using System.Collections.Generic;
using UnityEngine;

public enum DailyObjectiveType
{
    ServeCustomers,
    ServeCustomerType,
    MixDrink,
    ServeDrink,
    EarnMoney
}

[Serializable]
public class DailyObjective
{
    public DailyObjectiveType type;

    [Min(1)]
    public int target;

    [Min(0)]
    public int progress;

    [Min(0)]
    public int reward;
    public bool rewardClaimed;

    public CustomerType customerType;
    public DrinkRecipeSO drinkRecipe;

    public bool IsCompleted => progress >= target;

    public void SetProgress(int value)
    {
        progress = Mathf.Clamp(value, 0, target);
    }

    public void AddProgress(int amount = 1)
    {
        if (IsCompleted)
            return;

        progress = Mathf.Clamp(progress + amount, 0, target);
    }
}

public class DailyObjectiveManager : MonoBehaviour
{
    public static DailyObjectiveManager instance { get; private set; }

    [Header("Objective Target")]
    [SerializeField] private Vector2Int serveCustomersRange = new Vector2Int(5, 10);
    [SerializeField] private Vector2Int serveCustomerTypeRange = new Vector2Int(2, 4);
    [SerializeField] private Vector2Int mixDrinkRange = new Vector2Int(2, 5);
    [SerializeField] private Vector2Int serveDrinkRange = new Vector2Int(2, 5);
    [SerializeField] private Vector2Int earnMoneyRange = new Vector2Int(500, 1500);

    [Header("Customer Source")]
    [SerializeField] private CustomerController[] customerPrefabs;

    [Header("Recipe Source")]
    [SerializeField] private RecipeBookUI recipeBookUI;

    [Header("Objective Reward")]
    [SerializeField] private Vector2Int rewardRange = new Vector2Int(100, 300);

    private DailyObjective currentObjective;

    public DailyObjective CurrentObjective => currentObjective;

    public event Action<DailyObjective> OnObjectiveChanged;

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
        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted += HandleNewDayStarted;
        }
    }

    private void OnDestroy()
    {
        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted -= HandleNewDayStarted;
        }
    }

    private void HandleNewDayStarted()
    {
        GenerateNewObjective();
    }

    public void GenerateNewObjective()
    {
        List<DailyObjectiveType> validTypes = GetValidObjectiveTypes();

        if (validTypes.Count == 0)
        {

            currentObjective = null;
            return;
        }

        DailyObjectiveType selectedType = validTypes[UnityEngine.Random.Range(0, validTypes.Count)];

        currentObjective = CreateObjective(selectedType);

        if (currentObjective == null)
        {
            return;
        }

        OnObjectiveChanged?.Invoke(currentObjective);
    }

    private List<DailyObjectiveType> GetValidObjectiveTypes()
    {
        List<DailyObjectiveType> validTypes = new();

        validTypes.Add(DailyObjectiveType.ServeCustomers);

        if (HasUnlockedCustomerType())
        {
            validTypes.Add(DailyObjectiveType.ServeCustomerType);
        }

        if (HasUnlockedRecipe())
        {
            validTypes.Add(DailyObjectiveType.MixDrink);
            validTypes.Add(DailyObjectiveType.ServeDrink);
        }

        validTypes.Add(DailyObjectiveType.EarnMoney);

        return validTypes;
    }

    private DailyObjective CreateObjective(DailyObjectiveType type)
    {
        DailyObjective objective = new DailyObjective();
        objective.type = type;

        switch (type)
        {
            case DailyObjectiveType.ServeCustomers:
                objective.target = GetRandomTarget(serveCustomersRange);
                break;

            case DailyObjectiveType.ServeCustomerType:
                {
                    CustomerSO customer = GetRandomUnlockedCustomer();

                    if (customer == null)
                        return null;

                    objective.customerType = customer.customerType;
                    objective.target = GetRandomTarget(serveCustomerTypeRange);
                    break;
                }

            case DailyObjectiveType.MixDrink:
                {
                    DrinkRecipeSO recipe = GetRandomUnlockedRecipe();

                    if (recipe == null)
                        return null;

                    objective.drinkRecipe = recipe;
                    objective.target = GetRandomTarget(mixDrinkRange);
                    break;
                }

            case DailyObjectiveType.ServeDrink:
                {
                    DrinkRecipeSO recipe = GetRandomUnlockedRecipe();

                    if (recipe == null)
                        return null;

                    objective.drinkRecipe = recipe;
                    objective.target = GetRandomTarget(serveDrinkRange);
                    break;
                }

            case DailyObjectiveType.EarnMoney:
                objective.target = GetRandomTarget(earnMoneyRange);
                break;
        }

        objective.reward = GetRandomTarget(rewardRange);

        return objective;
    }

    private int GetRandomTarget(Vector2Int range)
    {
        int min = Mathf.Max(1, range.x);
        int max = Mathf.Max(min, range.y);

        return UnityEngine.Random.Range(min, max + 1);
    }

    private bool HasUnlockedCustomerType()
    {
        return GetUnlockedCustomers().Count > 0;
    }

    private bool HasUnlockedRecipe()
    {
        return GetUnlockedRecipes().Count > 0;
    }

    private CustomerSO GetRandomUnlockedCustomer()
    {
        List<CustomerSO> unlockedCustomers = GetUnlockedCustomers();

        if (unlockedCustomers.Count == 0)
            return null;

        return unlockedCustomers[UnityEngine.Random.Range(0, unlockedCustomers.Count)];
    }

    private List<CustomerSO> GetUnlockedCustomers()
    {
        List<CustomerSO> result = new();

        if (customerPrefabs == null)
            return result;

        int currentDay = GetCurrentDay();

        foreach (CustomerController prefab in customerPrefabs)
        {
            if (prefab == null)
                continue;

            CustomerSO data = prefab.Data;

            if (data == null)
                continue;

            if (data.unlockDay > currentDay)
                continue;

            bool alreadyAdded = false;

            foreach (CustomerSO existing in result)
            {
                if (existing.customerType == data.customerType)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                result.Add(data);
            }
        }

        return result;
    }

    private DrinkRecipeSO GetRandomUnlockedRecipe()
    {
        List<DrinkRecipeSO> unlockedRecipes = GetUnlockedRecipes();

        if (unlockedRecipes.Count == 0)
            return null;

        return unlockedRecipes[UnityEngine.Random.Range(0, unlockedRecipes.Count)];
    }

    private List<DrinkRecipeSO> GetUnlockedRecipes()
    {
        List<DrinkRecipeSO> result = new();

        if (recipeBookUI == null)
            return result;

        foreach (DrinkRecipeSO recipe in recipeBookUI.Recipes)
        {
            if (recipe == null)
                continue;

            if (RecipeProgressionManager.instance == null)
                continue;

            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
                continue;

            result.Add(recipe);
        }

        return result;
    }

    private int GetCurrentDay()
    {
        if (GameClock.instance == null)
            return 1;

        return GameClock.instance.CurrentDay;
    }

    public void RegisterCustomerServed(CustomerController customer, DrinkRecipeSO servedDrink)
    {
        if (currentObjective == null)
            return;

        switch (currentObjective.type)
        {
            case DailyObjectiveType.ServeCustomers:
                currentObjective.AddProgress();
                break;

            case DailyObjectiveType.ServeCustomerType:
                if (customer != null && customer.Data != null && customer.Data.customerType == currentObjective.customerType)
                {
                    currentObjective.AddProgress();
                }
                break;

            case DailyObjectiveType.ServeDrink:
                if (servedDrink == currentObjective.drinkRecipe)
                {
                    currentObjective.AddProgress();
                }
                break;
        }

        NotifyObjectiveChanged();
    }

    public void RegisterDrinkMixed(DrinkRecipeSO recipe)
    {
        if (currentObjective == null)
            return;

        if (currentObjective.type != DailyObjectiveType.MixDrink)
            return;

        if (recipe != currentObjective.drinkRecipe)
            return;

        currentObjective.AddProgress();

        NotifyObjectiveChanged();
    }

    public void RefreshEarnMoneyProgress()
    {
        if (currentObjective == null)
            return;

        if (currentObjective.type != DailyObjectiveType.EarnMoney)
            return;

        if (DayStatsManager.instance == null)
            return;

        int totalEarned = DayStatsManager.instance.MoneyEarnedToday + DayStatsManager.instance.TipsToday;

        currentObjective.SetProgress(totalEarned);

        NotifyObjectiveChanged();
    }

    private void NotifyObjectiveChanged()
    {
        OnObjectiveChanged?.Invoke(currentObjective);

        if (currentObjective.IsCompleted)
        {
            CompleteObjective();
        }
    }

    public string GetObjectiveDescription()
    {
        if (currentObjective == null)
            return string.Empty;

        switch (currentObjective.type)
        {
            case DailyObjectiveType.ServeCustomers:
                return $"Serve {currentObjective.target} Customers";

            case DailyObjectiveType.ServeCustomerType:
                return $"Serve {currentObjective.target} {currentObjective.customerType} Customers";

            case DailyObjectiveType.MixDrink:
                return $"Mix {currentObjective.target} {currentObjective.drinkRecipe.displayName}";

            case DailyObjectiveType.ServeDrink:
                return $"Serve {currentObjective.target} {currentObjective.drinkRecipe.displayName}";

            case DailyObjectiveType.EarnMoney:
                return $"Earn {currentObjective.target} Money";
        }

        return string.Empty;
    }

    private void CompleteObjective()
    {
        if (currentObjective == null)
            return;

        if (currentObjective.rewardClaimed)
            return;

        if (MoneyManager.instance == null)
            return;

        DayStatsManager.instance.AddObjectiveReward(currentObjective.reward);

        currentObjective.rewardClaimed = true;
    }
}