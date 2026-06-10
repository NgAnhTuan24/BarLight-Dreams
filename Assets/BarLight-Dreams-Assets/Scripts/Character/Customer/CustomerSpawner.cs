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

    private float timer;

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
        currentSpawnInterval = 2f;
        timer = 0f;
    }

    void SetRandomSpawnInterval()
    {
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

        Vector3 spawnPos = GetRandomSpawnPosition();

        int randomIndex = Random.Range(0, customerPrefabs.Length);

        CustomerController randomCustomer = customerPrefabs[randomIndex];

        Instantiate(randomCustomer, spawnPos, Quaternion.identity);
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