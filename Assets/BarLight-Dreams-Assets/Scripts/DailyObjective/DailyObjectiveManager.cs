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

    private const int OBJECTIVE_COUNT = 3;

    [Header("Objective Target")]
    [SerializeField] private Vector2Int serveCustomersRange = new Vector2Int(5, 10);
    [SerializeField] private Vector2Int serveCustomerTypeRange = new Vector2Int(2, 4);
    [SerializeField] private Vector2Int mixDrinkRange = new Vector2Int(2, 5);
    [SerializeField] private Vector2Int serveDrinkRange = new Vector2Int(2, 5);
    [SerializeField] private Vector2Int earnMoneyRange = new Vector2Int(500, 1500);

    [Header("Objective Reward")]
    [SerializeField] private Vector2Int rewardRange = new Vector2Int(100, 300);

    [Header("Customer Source")]
    [SerializeField] private CustomerController[] customerPrefabs;

    [Header("Recipe Source")]
    [SerializeField] private RecipeBookUI recipeBookUI;

    [Header("Difficulty")]
    [SerializeField] private int serveCustomersGrowthPerDay = 1;
    [SerializeField] private int serveCustomerTypeGrowthPerDay = 1;
    [SerializeField] private int mixDrinkGrowthPerDay = 1;
    [SerializeField] private int serveDrinkGrowthPerDay = 1;
    [SerializeField] private int earnMoneyGrowthPerDay = 150;
    [SerializeField] private int rewardGrowthPerDay = 25;

    [Header("Difficulty Caps")]
    [SerializeField] private int serveCustomersTargetCap = 20;
    [SerializeField] private int serveCustomerTypeTargetCap = 8;
    [SerializeField] private int mixDrinkTargetCap = 10;
    [SerializeField] private int serveDrinkTargetCap = 10;
    [SerializeField] private int earnMoneyTargetCap = 3000;
    [SerializeField] private int rewardCap = 500;

    private DailyObjective[] currentObjectives = new DailyObjective[OBJECTIVE_COUNT];

    public DailyObjective[] CurrentObjectives => currentObjectives;

    public event Action<DailyObjective[]> OnObjectivesChanged;

    public event Action<DailyObjective> OnObjectiveCompleted;

    public event Action<int> OnAllObjectivesCompleted;

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

        bool hasObjective = false;

        for (int i = 0; i < currentObjectives.Length; i++)
        {
            if (currentObjectives[i] != null)
            {
                hasObjective = true;
                break;
            }
        }

        if (!hasObjective)
        {
            GenerateNewObjectives();
        }
        else
        {
            NotifyObjectivesChanged();
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
        GenerateNewObjectives();
    }

    public void GenerateNewObjectives()
    {
        List<DailyObjectiveType> availableTypes = GetValidObjectiveTypes();

        currentObjectives = new DailyObjective[OBJECTIVE_COUNT];

        for (int i = 0; i < OBJECTIVE_COUNT; i++)
        {
            if (availableTypes.Count == 0)
            {
                break;
            }

            int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);

            DailyObjectiveType selectedType = availableTypes[randomIndex];

            DailyObjective objective = CreateObjective(selectedType);

            availableTypes.RemoveAt(randomIndex);

            if (objective != null)
            {
                currentObjectives[i] = objective;
            }
            else
            {
                i--;
            }
        }

        NotifyObjectivesChanged();
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
        objective.progress = 0;
        objective.rewardClaimed = false;

        switch (type)
        {
            case DailyObjectiveType.ServeCustomers:
                {
                    Vector2Int range = GetScaledRange(serveCustomersRange, serveCustomersGrowthPerDay, serveCustomersTargetCap);

                    objective.target = GetRandomTarget(range);

                    break;
                }

            case DailyObjectiveType.ServeCustomerType:
                {
                    CustomerSO customer = GetRandomUnlockedCustomer();

                    if (customer == null)
                    {
                        return null;
                    }

                    objective.customerType = customer.customerType;

                    Vector2Int range = GetScaledRange(serveCustomerTypeRange, serveCustomerTypeGrowthPerDay, serveCustomerTypeTargetCap);

                    objective.target = GetRandomTarget(range);

                    break;
                }

            case DailyObjectiveType.MixDrink:
                {
                    DrinkRecipeSO recipe = GetRandomUnlockedRecipe();

                    if (recipe == null)
                    {
                        return null;
                    }

                    objective.drinkRecipe = recipe;

                    Vector2Int range = GetScaledRange(mixDrinkRange, mixDrinkGrowthPerDay, mixDrinkTargetCap);

                    objective.target = GetRandomTarget(range);

                    break;
                }

            case DailyObjectiveType.ServeDrink:
                {
                    DrinkRecipeSO recipe = GetRandomUnlockedRecipe();

                    if (recipe == null)
                    {
                        return null;
                    }

                    objective.drinkRecipe = recipe;

                    Vector2Int range = GetScaledRange(serveDrinkRange, serveDrinkGrowthPerDay, serveDrinkTargetCap);

                    objective.target = GetRandomTarget(range);

                    break;
                }

            case DailyObjectiveType.EarnMoney:
                {
                    Vector2Int range = GetScaledRange(earnMoneyRange, earnMoneyGrowthPerDay, earnMoneyTargetCap);

                    objective.target = GetRandomTarget(range);

                    break;
                }
        }

        Vector2Int rewardRangeScaled = GetScaledRewardRange();

        objective.reward = GetRandomTarget(rewardRangeScaled);

        return objective;
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
        {
            return null;
        }

        return unlockedCustomers[UnityEngine.Random.Range(0, unlockedCustomers.Count)];
    }

    private List<CustomerSO> GetUnlockedCustomers()
    {
        List<CustomerSO> result = new();

        if (customerPrefabs == null)
        {
            return result;
        }

        int currentDay = GetCurrentDay();

        foreach (CustomerController prefab in customerPrefabs)
        {
            if (prefab == null)
            {
                continue;
            }

            CustomerSO data = prefab.Data;

            if (data == null)
            {
                continue;
            }

            if (data.unlockDay > currentDay)
            {
                continue;
            }

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
        {
            return null;
        }

        return unlockedRecipes[UnityEngine.Random.Range(0, unlockedRecipes.Count)];
    }

    private List<DrinkRecipeSO> GetUnlockedRecipes()
    {
        List<DrinkRecipeSO> result = new();

        if (recipeBookUI == null)
        {
            return result;
        }

        foreach (DrinkRecipeSO recipe in recipeBookUI.Recipes)
        {
            if (recipe == null)
            {
                continue;
            }

            if (RecipeProgressionManager.instance == null)
            {
                continue;
            }

            if (!RecipeProgressionManager.instance
                .IsRecipeUnlocked(recipe))
            {
                continue;
            }

            result.Add(recipe);
        }

        return result;
    }

    public void RegisterCustomerServed(CustomerController customer, DrinkRecipeSO servedDrink)
    {
        if (currentObjectives == null)
        {
            return;
        }

        bool changed = false;

        foreach (DailyObjective objective in currentObjectives)
        {
            if (objective == null)
            {
                continue;
            }

            if (objective.IsCompleted)
            {
                continue;
            }

            switch (objective.type)
            {
                case DailyObjectiveType.ServeCustomers:

                    objective.AddProgress();
                    changed = true;

                    break;

                case DailyObjectiveType.ServeCustomerType:

                    if (customer != null && customer.Data != null && customer.Data.customerType == objective.customerType)
                    {
                        objective.AddProgress();
                        changed = true;
                    }

                    break;

                case DailyObjectiveType.ServeDrink:

                    if (servedDrink != null && servedDrink == objective.drinkRecipe)
                    {
                        objective.AddProgress();
                        changed = true;
                    }

                    break;
            }
        }

        if (changed)
        {
            NotifyObjectivesChanged();
        }
    }

    public void RegisterDrinkMixed(DrinkRecipeSO recipe)
    {
        if (currentObjectives == null)
        {
            return;
        }

        if (recipe == null)
        {
            return;
        }

        bool changed = false;

        foreach (DailyObjective objective in currentObjectives)
        {
            if (objective == null)
            {
                continue;
            }

            if (objective.IsCompleted)
            {
                continue;
            }

            if (objective.type != DailyObjectiveType.MixDrink)
            {
                continue;
            }

            if (recipe != objective.drinkRecipe)
            {
                continue;
            }

            objective.AddProgress();
            changed = true;
        }

        if (changed)
        {
            NotifyObjectivesChanged();
        }
    }

    public void RefreshEarnMoneyProgress()
    {
        if (currentObjectives == null)
        {
            return;
        }

        if (DayStatsManager.instance == null)
        {
            return;
        }

        int totalEarned = DayStatsManager.instance.MoneyEarnedToday + DayStatsManager.instance.TipsToday;

        bool changed = false;

        foreach (DailyObjective objective in currentObjectives)
        {
            if (objective == null)
            {
                continue;
            }

            if (objective.IsCompleted)
            {
                continue;
            }

            if (objective.type != DailyObjectiveType.EarnMoney)
            {
                continue;
            }

            int oldProgress = objective.progress;

            objective.SetProgress(totalEarned);

            if (oldProgress != objective.progress)
            {
                changed = true;
            }
        }

        if (changed)
        {
            NotifyObjectivesChanged();
        }
    }

    private void NotifyObjectivesChanged()
    {
        OnObjectivesChanged?.Invoke(currentObjectives);

        foreach (DailyObjective objective in currentObjectives)
        {
            CheckObjectiveCompletion(objective);
        }
    }

    private void CheckObjectiveCompletion(DailyObjective objective)
    {
        if (objective == null)
        {
            return;
        }

        if (!objective.IsCompleted)
        {
            return;
        }

        if (objective.rewardClaimed)
        {
            return;
        }

        CompleteObjective(objective);
    }

    private void CompleteObjective(DailyObjective objective)
    {
        if (objective == null)
        {
            return;
        }

        if (objective.rewardClaimed)
        {
            return;
        }

        if (MoneyManager.instance == null)
        {
            return;
        }

        if (DayStatsManager.instance == null)
        {
            return;
        }

        DayStatsManager.instance.AddObjectiveReward(objective.reward);

        objective.rewardClaimed = true;

        OnObjectiveCompleted?.Invoke(objective);

        if (AreAllObjectivesCompleted())
        {
            int totalReward = GetTotalObjectiveReward();

            OnAllObjectivesCompleted?.Invoke(totalReward);
        }
    }

    public bool AreAllObjectivesCompleted()
    {
        if (currentObjectives == null)
        {
            return false;
        }

        if (currentObjectives.Length < OBJECTIVE_COUNT)
        {
            return false;
        }

        for (int i = 0; i < OBJECTIVE_COUNT; i++)
        {
            if (currentObjectives[i] == null)
            {
                return false;
            }

            if (!currentObjectives[i].IsCompleted)
            {
                return false;
            }
        }

        return true;
    }

    public int GetTotalObjectiveReward()
    {
        int totalReward = 0;

        if (currentObjectives == null)
        {
            return 0;
        }

        foreach (DailyObjective objective in currentObjectives)
        {
            if (objective == null)
            {
                continue;
            }

            totalReward += objective.reward;
        }

        return totalReward;
    }

    public string GetObjectiveDescription(DailyObjective objective)
    {
        if (objective == null)
        {
            return string.Empty;
        }

        switch (objective.type)
        {
            case DailyObjectiveType.ServeCustomers:

                return $"Serve {objective.target} Customers";

            case DailyObjectiveType.ServeCustomerType:

                return $"Serve {objective.target} " + $"{objective.customerType} Customers";

            case DailyObjectiveType.MixDrink:

                if (objective.drinkRecipe == null)
                {
                    return string.Empty;
                }

                return $"Mix {objective.target} " + $"{objective.drinkRecipe.displayName}";

            case DailyObjectiveType.ServeDrink:

                if (objective.drinkRecipe == null)
                {
                    return string.Empty;
                }

                return $"Serve {objective.target} " + $"{objective.drinkRecipe.displayName}";

            case DailyObjectiveType.EarnMoney:

                return $"Earn {objective.target} Money";
        }

        return string.Empty;
    }

    private int GetCurrentDay()
    {
        if (GameClock.instance == null)
        {
            return 1;
        }

        return GameClock.instance.CurrentDay;
    }

    private int GetDayGrowth(int growthPerDay)
    {
        int currentDay = GetCurrentDay();

        return Mathf.Max(0, currentDay - 1) * growthPerDay;
    }

    private Vector2Int GetScaledRange(Vector2Int baseRange, int growthPerDay, int maxValue)
    {
        int growth = GetDayGrowth(growthPerDay);

        int min = baseRange.x + growth;

        int max = baseRange.y + growth;

        min = Mathf.Min(min, maxValue);

        max = Mathf.Min(max, maxValue);

        return new Vector2Int(min, max);
    }

    private Vector2Int GetScaledRewardRange()
    {
        int growth = GetDayGrowth(rewardGrowthPerDay);

        int min = rewardRange.x + growth;

        int max = rewardRange.y + growth;

        min = Mathf.Min(min, rewardCap);

        max = Mathf.Min(max, rewardCap);

        return new Vector2Int(min, max);
    }

    private int GetRandomTarget(Vector2Int range)
    {
        int min = Mathf.Max( 1, range.x);

        int max = Mathf.Max(min, range.y);

        return UnityEngine.Random.Range(min, max + 1);
    }
}