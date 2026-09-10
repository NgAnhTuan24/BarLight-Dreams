using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private CustomerController[] customerPrefabs;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnCenter;

    [SerializeField] private float spawnRangeX = 2f;
    [SerializeField] private float spawnRangeY = 7f;

    [Header("Time")]
    [SerializeField] private float[] spawnIntervals;
    [SerializeField, Range(0f, 1f)] private float intervalReductionPerDay = 0.0375f;
    [SerializeField] private float minimumSpawnInterval = 8f;

    private float currentSpawnInterval;

    [SerializeField] private int maxCustomers = 12;

    [Header("UI")]
    [SerializeField] private TMP_Text customerCountText;

    [Header("Test Mode")]
    [SerializeField] private bool testSpawnEverySecond = false;
    [SerializeField] private float testSpawnInterval = 1f;

    private float timer;

    private readonly HashSet<CustomerType> appearedTypesToday = new();

    private bool tutorialSpawnLocked;
    private bool tutorialFirstCustomerSpawned;

    private void Start()
    {
        ResetSpawnInterval();

        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted += ResetSpawnInterval;
        }

        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnStepChanged += HandleTutorialStepChanged;
            TutorialManager.instance.OnTutorialCompleted += HandleTutorialCompleted;
        }
    }

    private void Update()
    {
        UpdateCustomerUI();

        if (GameClock.instance == null) return;

        if (!GameClock.instance.IsRunning) return;

        if (tutorialSpawnLocked) return;

        timer += Time.deltaTime;

        while (timer >= currentSpawnInterval)
        {
            timer = 0f;

            TrySpawnCustomer();

            SetRandomSpawnInterval();
        }
    }

    void ResetSpawnInterval()
    {
        timer = 0f;

        appearedTypesToday.Clear();

        tutorialSpawnLocked = false;
        tutorialFirstCustomerSpawned = false;

        if (TutorialManager.instance != null && GameClock.instance != null && GameClock.instance.CurrentDay == 1 && TutorialManager.instance.IsTutorialActive)
        {
            tutorialSpawnLocked = true;
        }

        if (testSpawnEverySecond)
        {
            currentSpawnInterval = testSpawnInterval;
        }
        else
        {
            currentSpawnInterval = 2f;
        }
    }

    void SetRandomSpawnInterval()
    {
        if (testSpawnEverySecond)
        {
            currentSpawnInterval = testSpawnInterval;
            return;
        }

        if (GameClock.instance.IsRushHour)
        {
            currentSpawnInterval = 10f;
            return;
        }

        if (spawnIntervals == null || spawnIntervals.Length == 0)
        {
            currentSpawnInterval = minimumSpawnInterval;

            return;
        }

        int randomIndex = Random.Range(0, spawnIntervals.Length);

        float baseInterval = spawnIntervals[randomIndex];

        currentSpawnInterval = GetScaledSpawnInterval(baseInterval);
    }

    float GetScaledSpawnInterval(float baseInterval)
    {
        int currentDay = GameClock.instance.CurrentDay;

        float dayMultiplier = 1f - intervalReductionPerDay * (currentDay - 1);

        dayMultiplier = Mathf.Max(dayMultiplier, 0f);

        float scaledInterval = baseInterval * dayMultiplier;

        return Mathf.Max(scaledInterval, minimumSpawnInterval);
    }

    void TrySpawnCustomer()
    {
        if (PlayerController.instance.health.CurrentHP == 0) return;

        if (!GameClock.instance.CanReceiveCustomers) return;

        if (CustomerManager.instance.CurrentCustomerCount >= maxCustomers) return;

        if (tutorialSpawnLocked) return;

        CustomerController customerPrefab = SelectCustomerPrefab();

        if (customerPrefab == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        CustomerSO customerData = customerPrefab.Data;

        Instantiate(customerPrefab, spawnPos, Quaternion.identity);

        if (customerData != null)
        {
            appearedTypesToday.Add(customerData.customerType);
        }

        if (GameClock.instance.CurrentDay == 1 && TutorialManager.instance != null && TutorialManager.instance.IsTutorialActive)
        {
            tutorialFirstCustomerSpawned = true;
            tutorialSpawnLocked = true;
        }
    }

    CustomerController SelectCustomerPrefab()
    {
        int currentDay = GameClock.instance.CurrentDay;

        Dictionary<CustomerType, List<CustomerController>> customerGroups = BuildCustomerGroups();

        List<CustomerType> unlockedTypes = new();
        List<CustomerType> newlyUnlockedTypes = new();

        foreach (KeyValuePair<CustomerType, List<CustomerController>> group in customerGroups)
        {
            CustomerType customerType = group.Key;
            List<CustomerController> prefabs = group.Value;

            if (prefabs.Count == 0)
                continue;

            CustomerSO customerData = prefabs[0].Data;

            if (customerData == null)
                continue;

            if (currentDay < customerData.unlockDay)
                continue;

            unlockedTypes.Add(customerType);

            if (customerData.unlockDay == currentDay && !appearedTypesToday.Contains(customerType))
            {
                newlyUnlockedTypes.Add(customerType);
            }
        }

        if (newlyUnlockedTypes.Count > 0)
        {
            CustomerType selectedType = SelectRandomType(newlyUnlockedTypes);

            return SelectRandomPrefab(customerGroups[selectedType]);
        }

        CustomerType weightedType = SelectWeightedType(unlockedTypes, customerGroups);

        if (!customerGroups.ContainsKey(weightedType))
        {
            return null;
        }

        return SelectRandomPrefab(customerGroups[weightedType]);
    }

    Dictionary<CustomerType, List<CustomerController>> BuildCustomerGroups()
    {
        Dictionary<CustomerType, List<CustomerController>> groups = new();

        foreach (CustomerController customerPrefab in customerPrefabs)
        {
            if (customerPrefab == null)
                continue;

            CustomerSO customerData = customerPrefab.Data;

            if (customerData == null)
                continue;

            CustomerType customerType = customerData.customerType;

            if (!groups.ContainsKey(customerType))
            {
                groups.Add(customerType, new List<CustomerController>());
            }

            groups[customerType].Add(customerPrefab);
        }

        return groups;
    }

    CustomerType SelectRandomType(List<CustomerType> customerTypes)
    {
        if (customerTypes == null || customerTypes.Count == 0)
        {
            return default;
        }

        int randomIndex = Random.Range(0, customerTypes.Count);

        return customerTypes[randomIndex];
    }

    CustomerType SelectWeightedType(List<CustomerType> customerTypes, Dictionary<CustomerType, List<CustomerController>> customerGroups)
    {
        if (customerTypes == null || customerTypes.Count == 0)
        {
            return default;
        }

        float totalWeight = 0f;

        foreach (CustomerType customerType in customerTypes)
        {
            List<CustomerController> prefabs = customerGroups[customerType];

            if (prefabs == null || prefabs.Count == 0)
                continue;

            CustomerSO customerData = prefabs[0].Data;

            if (customerData == null)
                continue;

            if (customerData.spawnWeight <= 0f)
                continue;

            totalWeight += customerData.spawnWeight;
        }

        if (totalWeight <= 0f)
        {
            return SelectRandomType(customerTypes);
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (CustomerType customerType in customerTypes)
        {
            List<CustomerController> prefabs = customerGroups[customerType];

            if (prefabs == null || prefabs.Count == 0)
                continue;

            CustomerSO customerData = prefabs[0].Data;

            if (customerData == null)
                continue;

            if (customerData.spawnWeight <= 0f)
                continue;

            randomValue -= customerData.spawnWeight;

            if (randomValue <= 0f)
            {
                return customerType;
            }
        }

        return customerTypes[customerTypes.Count - 1];
    }

    CustomerController SelectRandomPrefab(List<CustomerController> prefabs)
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, prefabs.Count);

        return prefabs[randomIndex];
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnCenter.x - spawnRangeX, spawnCenter.x + spawnRangeX);

        float randomY = Random.Range(spawnCenter.y - spawnRangeY, spawnCenter.y + spawnRangeY);

        return new Vector3(randomX, randomY, 0f);
    }

    void UpdateCustomerUI()
    {
        int currentCustomers = CustomerManager.instance.CurrentCustomerCount;

        int totalChairs = ChairManager.instance.GetChairCount();

        customerCountText.text = currentCustomers + "/" + totalChairs + " Customer";
    }

    private void HandleTutorialStepChanged(TutorialStep step)
    {
        if (GameClock.instance == null)
            return;

        if (GameClock.instance.CurrentDay != 1)
            return;

        if (step == TutorialStep.Introduction)
        {
            tutorialSpawnLocked = true;
            return;
        }

        if (step != TutorialStep.GoToCustomer)
            return;

        if (tutorialFirstCustomerSpawned)
            return;

        timer = 0f;
        currentSpawnInterval = 2f;
        tutorialSpawnLocked = false;
    }

    private void HandleTutorialCompleted()
    {
        if (GameClock.instance == null)
            return;

        if (GameClock.instance.CurrentDay != 1)
            return;

        tutorialSpawnLocked = false;

        timer = 0f;

        TrySpawnCustomer();

        SetRandomSpawnInterval();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnRangeX * 2, spawnRangeY * 2, 0.1f));
    }

    private void OnDestroy()
    {
        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted -= ResetSpawnInterval;
        }

        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnStepChanged -= HandleTutorialStepChanged;
            TutorialManager.instance.OnTutorialCompleted -= HandleTutorialCompleted;
        }
    }
}