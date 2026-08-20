using System;
using UnityEngine;

[Serializable]
public abstract class UpgradeLevelBase
{
    public int price;
}

public abstract class UpgradeSOBase : ScriptableObject
{
    public UpgradeType upgradeType;

    public Sprite icon;

    public string title;

    [TextArea]
    public string description;
}