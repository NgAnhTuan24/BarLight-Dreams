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
    private float currentSpawnInterval;

    [SerializeField] private int maxCustomers = 12;

    [Header("UI")]
    [SerializeField] private TMP_Text customerCountText;

    [Header("Test Mode")]
    [SerializeField] private bool testSpawnEverySecond = false;
    [SerializeField] private float testSpawnInterval = 1f;

    private float timer;

    private readonly HashSet<CustomerType> appearedTypesToday = new();

    private void Start()
    {
        ResetSpawnInterval();

        GameClock.instance.OnNewDayStarted += ResetSpawnInterval;
    }

    private void Update()
    {
        UpdateCustomerUI();

        if (!GameClock.instance.IsRunning) return;

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
        }
        else
        {
            int randomIndex = Random.Range(0, spawnIntervals.Length);

            currentSpawnInterval = spawnIntervals[randomIndex];
        }
    }

    void TrySpawnCustomer()
    {
        if (PlayerController.instance.health.CurrentHP == 0) return;

        if (!GameClock.instance.CanReceiveCustomers)
        {
            return;
        }

        if (CustomerManager.instance.CurrentCustomerCount >= maxCustomers)
        {
            return;
        }

        CustomerController customerPrefab = SelectCustomerPrefab();

        if (customerPrefab == null)
        {
            Debug.LogWarning("CustomerSpawner: No valid Customer prefab is available for the current day.");
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();

        CustomerSO customerData = customerPrefab.Data;

        Instantiate(customerPrefab, spawnPos, Quaternion.identity);

        if (customerData != null)
        {
            appearedTypesToday.Add(customerData.customerType);
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
    }
}