using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OrderSlotLevelData : UpgradeLevelBase
{
    public int slotCount;
}

[CreateAssetMenu(fileName = "Order Slot Upgrade",menuName = "Bar/Upgrade/Order Slot")]
public class OrderSlotUpgradeSO : UpgradeSOBase
{
    public List<OrderSlotLevelData> levels = new();
}