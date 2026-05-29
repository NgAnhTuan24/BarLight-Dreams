using TMPro;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private CustomerController customerPrefab;

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

    private float timer;

    private void Start()
    {
        SetRandomSpawnInterval();
        TrySpawnCustomer();
        UpdateCustomerUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            timer = 0f;

            TrySpawnCustomer();

            SetRandomSpawnInterval();
        }

        UpdateCustomerUI();
    }

    void SetRandomSpawnInterval()
    {
        int hour = GameClock.Instance.CurrentHour;
        int minute = GameClock.Instance.CurrentMinute;

        bool isRushHour = (hour == 22) || (hour == 23 && minute <= 30);

        if (isRushHour)
        {
            currentSpawnInterval = 10f;
        }
        else
        {
            int randomIndex = Random.Range(0, spawnIntervals.Length);

            currentSpawnInterval = spawnIntervals[randomIndex];
        }

        Debug.Log("Next Spawn In: " + currentSpawnInterval);
    }

    void TrySpawnCustomer()
    {
        int hour = GameClock.Instance.CurrentHour;
        int minute = GameClock.Instance.CurrentMinute;

        bool stopReceivingCustomers = (hour == 23 && minute >= 30);

        if (stopReceivingCustomers)
        {
            return;
        }

        if (CustomerManager.instance.CurrentCustomerCount >= maxCustomers)
        {
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();

        Instantiate(customerPrefab, spawnPos, Quaternion.identity);
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
}