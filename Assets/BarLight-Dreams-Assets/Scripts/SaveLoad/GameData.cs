using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int currentHP;

    public int currentDay;
    public int currentHour;
    public int currentMinute;

    public int currentMoney;

    public int totalMoneyEarned;
    public int totalServedCustomers;
    public int totalAngryCustomers;

    public List<UpgradeRuntimeData> upgrades = new();

    public bool tutorialCompleted;

    public string saveTime;
}
