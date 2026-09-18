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

    public int totalObjectivesCompleted;

    public List<UpgradeRuntimeData> upgrades = new();

    public bool tutorialCompleted;

    public bool gameCompleted;

    public float finalScore;
    public string finalRank;

    public int finalMoney;
    public int finalCustomers;
    public int finalAngry;

    public int finalTotalUpgradeLevel;
    public int finalTotalObjectivesCompleted;

    public string saveTime;
}
