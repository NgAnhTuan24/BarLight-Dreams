using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class MixingLevelData : UpgradeLevelBase
{
    [Range(0, 1)]
    public float instantChance;
}

[CreateAssetMenu(fileName = "Mixing Upgrade",menuName = "Bar/Upgrade/Mixing")]
public class MixingUpgradeSO : UpgradeSOBase
{
    public List<MixingLevelData> levels = new();
}