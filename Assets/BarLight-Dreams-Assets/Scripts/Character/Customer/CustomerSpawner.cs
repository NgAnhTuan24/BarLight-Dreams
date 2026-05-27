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
    [SerializeField] private float spawnInterval = 5f;

    [SerializeField] private int maxCustomers = 5;

    [SerializeField] private TextMeshProUGUI customerCountText;

    private float timer;

    private int totalSpawnedCustomers;

    private void Start()
    {
        UpdateCustomerUI();
    }

    private void Update()
    {
        if (totalSpawnedCustomers >= maxCustomers) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        Instantiate(customerPrefab, spawnPos, Quaternion.identity);

        totalSpawnedCustomers++;

        UpdateCustomerUI();
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnCenter.x - spawnRangeX, spawnCenter.x + spawnRangeX);

        float randomY = Random.Range(spawnCenter.y - spawnRangeY, spawnCenter.y + spawnRangeY);

        return new Vector3(randomX, randomY, 0f);
    }

    void UpdateCustomerUI()
    {
        CustomerController[] customers = FindObjectsOfType<CustomerController>();

        customerCountText.text = customers.Length + "/" + maxCustomers + " Customers";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnRangeX * 2, spawnRangeY * 2,0.1f));
    }
}