using System.Collections.Generic;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    public static StaffManager instance;

    [Header("Staff")]
    [SerializeField] private GameObject staffPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform pickupPoint;

    private readonly List<StaffController> staffs = new();

    public bool HasActiveStaff
    {
        get
        {
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i] == null)
                    continue;

                if (staffs[i].gameObject.activeSelf)
                    return true;
            }

            return false;
        }
    }

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
        if (PickupCounter.instance == null)
            return;

        if (!PickupCounter.instance.HasUnclaimedDrink())
            return;

        StaffController availableStaff = GetAvailableStaff();

        if (availableStaff != null)
        {
            availableStaff.gameObject.SetActive(true);
            if (!availableStaff.MoveToPickup())
            {
                availableStaff.gameObject.SetActive(false);
            }
            return;
        }

        int maxStaffCount = GetMaxStaffCount();

        if (staffs.Count >= maxStaffCount)
            return;

        SpawnStaff();
    }

    public void SpawnStaff()
    {
        if (staffPrefab == null || spawnPoint == null)
            return;

        int maxStaffCount = GetMaxStaffCount();

        if (staffs.Count >= maxStaffCount)
            return;

        GameObject staffObject = Instantiate(staffPrefab, spawnPoint.position, spawnPoint.rotation);

        StaffController staffController = staffObject.GetComponent<StaffController>();

        if (staffController == null)
        {
            Debug.LogWarning("StaffManager: Staff prefab doesn't have StaffController!");
            Destroy(staffObject);
            return;
        }

        staffs.Add(staffController);

        staffController.Initialize(pickupPoint, spawnPoint);
    }

    private int GetMaxStaffCount()
    {
        if (UpgradeManager.instance == null)
            return 1;

        StaffLevelData data =
            (StaffLevelData)UpgradeManager.instance.GetCurrentLevelData(
                UpgradeType.Staff
            );

        if (data == null)
            return 1;

        return data.staffCount;
    }

    private StaffController GetAvailableStaff()
    {
        for (int i = 0; i < staffs.Count; i++)
        {
            if (staffs[i] == null)
                continue;

            if (!staffs[i].gameObject.activeSelf)
                return staffs[i];

            if (staffs[i].CurrentState == StaffState.ReturningHome)
                return staffs[i];
        }

        return null;
    }
}