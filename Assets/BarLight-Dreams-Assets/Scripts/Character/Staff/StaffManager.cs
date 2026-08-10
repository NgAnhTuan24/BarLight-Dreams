using UnityEngine;

public class StaffManager : MonoBehaviour
{
    public static StaffManager instance;

    [Header("Staff")]
    [SerializeField] private GameObject staffPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform pickupPoint;

    private GameObject currentStaff;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void TrySpawnStaff()
    {
        if (currentStaff != null)
            return;

        if (PickupCounter.instance == null)
            return;

        if (!PickupCounter.instance.HasDrink())
            return;

        SpawnStaff();
    }

    public void SpawnStaff()
    {
        if (currentStaff != null)
            return;

        if (staffPrefab == null || spawnPoint == null)
            return;

        currentStaff = Instantiate(staffPrefab, spawnPoint.position, spawnPoint.rotation);

        StaffController staffController = currentStaff.GetComponent<StaffController>();

        if (staffController == null)
        {
            Debug.LogWarning("StaffManager: Staff prefab doesn't have StaffController!");
            return;
        }

        staffController.Initialize(pickupPoint, spawnPoint);
    }
}