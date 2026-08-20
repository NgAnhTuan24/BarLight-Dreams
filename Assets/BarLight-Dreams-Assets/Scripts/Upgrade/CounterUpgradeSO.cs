using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CounterLevelData : UpgradeLevelBase
{
    public int capacity;
}

[CreateAssetMenu(fileName = "Counter Upgrade",menuName = "Bar/Upgrade/Counter")]
public class CounterUpgradeSO : UpgradeSOBase
{
    public List<CounterLevelData> levels = new();
}