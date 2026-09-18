using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager instance { get; private set; }

    [Header("Score Weights")]
    [SerializeField] private float moneyWeight = 1f;
    [SerializeField] private float customerWeight = 10f;
    [SerializeField] private float upgradeWeight = 100f;
    [SerializeField] private float objectiveWeight = 50f;
    [SerializeField] private float angryPenalty = 25f;

    [Header("Rank Thresholds")]
    [SerializeField] private float sThreshold = 10000f;
    [SerializeField] private float aThreshold = 7500f;
    [SerializeField] private float bThreshold = 5000f;

    public float FinalScore { get; private set; }
    public string FinalRank { get; private set; }

    public int Money { get; private set; }
    public int Customers { get; private set; }
    public int Angry { get; private set; }
    public int TotalUpgradeLevel { get; private set; }
    public int TotalObjectivesCompleted { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void CollectFinalData()
    {
        CollectEconomyData();
        CollectUpgradeData();
        CollectObjectiveData();
    }

    private void CollectEconomyData()
    {
        if (DayStatsManager.instance == null)
            return;

        Money = DayStatsManager.instance.TotalMoneyEarned;
        Customers = DayStatsManager.instance.TotalServedCustomers;
        Angry = DayStatsManager.instance.TotalAngryCustomers;
    }

    private void CollectUpgradeData()
    {
        if (UpgradeManager.instance == null)
            return;

        TotalUpgradeLevel = 0;

        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            TotalUpgradeLevel += UpgradeManager.instance.GetLevel(type);
        }
    }

    private void CollectObjectiveData()
    {
        if (DailyObjectiveManager.instance == null)
            return;

        TotalObjectivesCompleted = DailyObjectiveManager.instance.TotalObjectivesCompleted;
    }

    public void CalculateScore()
    {
        float moneyScore = Money * moneyWeight;
        float customerScore = Customers * customerWeight;
        float upgradeScore = TotalUpgradeLevel * upgradeWeight;
        float objectiveScore = TotalObjectivesCompleted * objectiveWeight;
        float angryPenaltyScore = Angry * angryPenalty;

        FinalScore = moneyScore + customerScore + upgradeScore + objectiveScore - angryPenaltyScore;
    }

    public void CalculateRank()
    {
        if (FinalScore >= sThreshold)
        {
            FinalRank = "S";
        }
        else if (FinalScore >= aThreshold)
        {
            FinalRank = "A";
        }
        else if (FinalScore >= bThreshold)
        {
            FinalRank = "B";
        }
        else
        {
            FinalRank = "C";
        }
    }

    public void CalculateFinalResult()
    {
        CalculateScore();
        CalculateRank();
    }

    public void LoadFinalData(GameData data)
    {
        Money = data.finalMoney;
        Customers = data.finalCustomers;
        Angry = data.finalAngry;

        TotalUpgradeLevel = data.finalTotalUpgradeLevel;
        TotalObjectivesCompleted = data.finalTotalObjectivesCompleted;

        FinalScore = data.finalScore;
        FinalRank = data.finalRank;
    }
}