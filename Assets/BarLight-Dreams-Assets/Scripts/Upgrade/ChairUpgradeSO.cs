using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChairLevelData : UpgradeLevelBase
{
    public float patienceMultiplier;

    public float tipMultiplier;
}

[CreateAssetMenu(fileName = "Chair Upgrade",menuName = "Bar/Upgrade/Chair")]
public class ChairUpgradeSO : UpgradeSOBase
{
    public List<ChairLevelData> levels = new();
}