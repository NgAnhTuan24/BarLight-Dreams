using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class StaffLevelData : UpgradeLevelBase
{
    public int staffCount;
}

[CreateAssetMenu(fileName = "Staff Upgrade",menuName = "Bar/Upgrade/Staff")]
public class StaffUpgradeSO : UpgradeSOBase
{
    public List<StaffLevelData> levels = new();
}