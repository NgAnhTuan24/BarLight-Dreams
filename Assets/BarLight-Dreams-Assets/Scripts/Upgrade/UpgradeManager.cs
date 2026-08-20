using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class UpgradeRuntimeData
{
    public UpgradeType upgradeType;

    public int currentLevel;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }

    [Header("Upgrade Data")]
    [SerializeField] private CounterUpgradeSO counterUpgrade;
    [SerializeField] private OrderSlotUpgradeSO orderSlotUpgrade;
    [SerializeField] private ChairUpgradeSO chairUpgrade;
    [SerializeField] private StaffUpgradeSO staffUpgrade;
    [SerializeField] private MixingUpgradeSO mixingUpgrade;

    [Header("Runtime")]
    [SerializeField] private List<UpgradeRuntimeData> runtimeData = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializeRuntime();
    }

    private void InitializeRuntime()
    {
        if (runtimeData.Count > 0) return;

        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            runtimeData.Add(new UpgradeRuntimeData
            {
                upgradeType = type,
                currentLevel = 0
            });
        }
    }

    public int GetLevel(UpgradeType type)
    {
        return GetRuntime(type).currentLevel;
    }

    public bool Upgrade(UpgradeType type)
    {
        UpgradeRuntimeData data = GetRuntime(type);

        if (IsMaxLevel(type)) return false;

        data.currentLevel++;

        ApplyUpgrade(type);

        return true;
    }

    public bool IsMaxLevel(UpgradeType type)
    {
        int maxLevel = GetMaxLevel(type);

        return GetLevel(type) >= maxLevel;
    }

    private int GetMaxLevel(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Counter => counterUpgrade.levels.Count - 1,
            UpgradeType.OrderSlot => orderSlotUpgrade.levels.Count - 1,
            UpgradeType.Chair => chairUpgrade.levels.Count - 1,
            UpgradeType.Staff => staffUpgrade.levels.Count - 1,
            UpgradeType.Mixing => mixingUpgrade.levels.Count - 1,
            _ => 0
        };
    }

    private UpgradeRuntimeData GetRuntime(UpgradeType type)
    {
        return runtimeData.Find(x => x.upgradeType == type);
    }

    private UpgradeSOBase GetUpgradeSO(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Counter => counterUpgrade,
            UpgradeType.OrderSlot => orderSlotUpgrade,
            UpgradeType.Chair => chairUpgrade,
            UpgradeType.Staff => staffUpgrade,
            UpgradeType.Mixing => mixingUpgrade,
            _ => null
        };
    }

    public UpgradeSOBase GetUpgradeData(UpgradeType type)
    {
        return GetUpgradeSO(type);
    }

    public UpgradeLevelBase GetCurrentLevelData(UpgradeType type)
    {
        int level = GetLevel(type);

        return type switch
        {
            UpgradeType.Counter => counterUpgrade.levels[level],
            UpgradeType.OrderSlot => orderSlotUpgrade.levels[level],
            UpgradeType.Chair => chairUpgrade.levels[level],
            UpgradeType.Staff => staffUpgrade.levels[level],
            UpgradeType.Mixing => mixingUpgrade.levels[level],
            _ => null
        };
    }

    public UpgradeLevelBase GetNextLevelData(UpgradeType type)
    {
        if (IsMaxLevel(type))
            return null;

        int nextLevel = GetLevel(type) + 1;

        return type switch
        {
            UpgradeType.Counter => counterUpgrade.levels[nextLevel],
            UpgradeType.OrderSlot => orderSlotUpgrade.levels[nextLevel],
            UpgradeType.Chair => chairUpgrade.levels[nextLevel],
            UpgradeType.Staff => staffUpgrade.levels[nextLevel],
            UpgradeType.Mixing => mixingUpgrade.levels[nextLevel],
            _ => null
        };
    }

    public int GetUpgradePrice(UpgradeType type)
    {
        UpgradeLevelBase next = GetNextLevelData(type);

        if (next == null)
            return 0;

        return next.price;
    }

    public UpgradeViewData GetViewData(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Counter => BuildCounterView(),
            UpgradeType.OrderSlot => BuildOrderSlotView(),
            UpgradeType.Chair => BuildChairView(),
            UpgradeType.Staff => BuildStaffView(),
            UpgradeType.Mixing => BuildMixingView(),
            _ => null
        };
    }

    private UpgradeViewData CreateBaseView(UpgradeSOBase so, UpgradeType type)
    {
        return new UpgradeViewData
        {
            icon = so.icon,

            title = so.title,
            description = so.description,

            currentLevel = GetLevel(type) + 1,
            maxLevel = GetMaxLevel(type) + 1,

            isMaxLevel = IsMaxLevel(type),

            price = GetUpgradePrice(type)
        };
    }

    private UpgradeViewData BuildCounterView()
    {
        CounterLevelData current = (CounterLevelData)GetCurrentLevelData(UpgradeType.Counter);

        CounterLevelData next = (CounterLevelData)GetNextLevelData(UpgradeType.Counter);

        UpgradeViewData view = CreateBaseView(counterUpgrade, UpgradeType.Counter);

        view.effect1Current = current.capacity.ToString();
        view.effect1Label = "Counter Capacity";
        view.effect1Unit = "Customers";

        if (!view.isMaxLevel)
            view.effect1Next = next.capacity.ToString();
        else
            view.effect1Next = "MAX";

        view.useEffect2 = false;

        return view;
    }

    private UpgradeViewData BuildOrderSlotView()
    {
        OrderSlotLevelData current = (OrderSlotLevelData)GetCurrentLevelData(UpgradeType.OrderSlot);

        OrderSlotLevelData next = (OrderSlotLevelData)GetNextLevelData(UpgradeType.OrderSlot);

        UpgradeViewData view = CreateBaseView(orderSlotUpgrade, UpgradeType.OrderSlot);

        view.effect1Current = current.slotCount.ToString();
        view.effect1Label = "Displayed Orders";
        view.effect1Unit = "Slots";

        if (!view.isMaxLevel)
            view.effect1Next = next.slotCount.ToString();
        else
            view.effect1Next = "MAX";

        view.useEffect2 = false;

        return view;
    }

    private UpgradeViewData BuildChairView()
    {
        ChairLevelData current = (ChairLevelData)GetCurrentLevelData(UpgradeType.Chair);

        ChairLevelData next = (ChairLevelData)GetNextLevelData(UpgradeType.Chair);

        UpgradeViewData view = CreateBaseView(chairUpgrade, UpgradeType.Chair);

        int currentPatience = Mathf.RoundToInt((current.patienceMultiplier - 1f) * 100f);
        view.effect1Current = currentPatience.ToString();
        view.effect1Label = "Patience Multiplier";
        view.effect1Unit = "%";

        int currentTip = Mathf.RoundToInt((current.tipMultiplier - 1f) * 100f);
        view.effect2Current = currentTip.ToString();
        view.effect2Label = "Tip Multiplier";
        view.effect2Unit = "%";

        if (!view.isMaxLevel)
        {
            int nextPatience = Mathf.RoundToInt((next.patienceMultiplier - 1f) * 100f);
            view.effect1Next = nextPatience.ToString();

            int nextTip = Mathf.RoundToInt((next.tipMultiplier - 1f) * 100f);
            view.effect2Next = nextTip.ToString();
        }
        else
        {
            view.effect1Next = "MAX";
            view.effect2Next = "MAX";
        }

        view.useEffect2 = true;

        return view;
    }

    private UpgradeViewData BuildStaffView()
    {
        StaffLevelData current = (StaffLevelData)GetCurrentLevelData(UpgradeType.Staff);

        StaffLevelData next = (StaffLevelData)GetNextLevelData(UpgradeType.Staff);

        UpgradeViewData view = CreateBaseView(staffUpgrade, UpgradeType.Staff);

        view.effect1Current = current.staffCount.ToString();
        view.effect1Label = "Staff Count";
        view.effect1Unit = "Staff";

        if (!view.isMaxLevel)
            view.effect1Next = next.staffCount.ToString();
        else
            view.effect1Next = "MAX";

        view.useEffect2 = false;

        return view;
    }

    private UpgradeViewData BuildMixingView()
    {
        MixingLevelData current = (MixingLevelData)GetCurrentLevelData(UpgradeType.Mixing);

        MixingLevelData next = (MixingLevelData)GetNextLevelData(UpgradeType.Mixing);

        UpgradeViewData view = CreateBaseView(mixingUpgrade, UpgradeType.Mixing);

        view.effect1Current = (current.instantChance * 100f).ToString("0");
        view.effect1Label = "Instant Mix Chance";
        view.effect1Unit = "%";

        if (!view.isMaxLevel)
            view.effect1Next = (next.instantChance * 100f).ToString("0");
        else
            view.effect1Next = "MAX";

        view.useEffect2 = false;

        return view;
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Counter:
                {
                    CounterLevelData data = (CounterLevelData)GetCurrentLevelData(type);

                    CounterManager.instance.SetCapacity(data.capacity);

                    break;
                }

            case UpgradeType.OrderSlot:
                {
                    OrderSlotLevelData data = (OrderSlotLevelData)GetCurrentLevelData(type);

                    OrderQueueManager.instance.SetSlotCount(data.slotCount);

                    break;
                }

            case UpgradeType.Chair:
                {
                    ChairLevelData data = (ChairLevelData)GetCurrentLevelData(type);

                    CustomerManager.instance.SetPatienceMultiplier(data.patienceMultiplier);

                    CustomerManager.instance.SetTipMultiplier(data.tipMultiplier);

                    break;
                }

            case UpgradeType.Staff:
                {
                    StaffManager.instance.TrySpawnStaff();

                    break;
                }

            case UpgradeType.Mixing:
                {
                    MixingLevelData data = (MixingLevelData)GetCurrentLevelData(type);

                    DrinkMixer.instance.SetInstantChance(data.instantChance);

                    break;
                }
        }
    }

    public float GetInstantMixChance()
    {
        MixingLevelData data = (MixingLevelData)GetCurrentLevelData(UpgradeType.Mixing);

        return data.instantChance;
    }
}
