using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private EndingUI endingUI;

    private void Start()
    {
        if (!SaveManager.instance.IsLoadingGame) return;

        LoadGameData();
    }

    private void LoadGameData()
    {
        if (SaveManager.instance == null) return;

        GameData data = SaveManager.instance.LoadGame();

        if (data == null) return;

        if (data.gameCompleted)
        {
            if (GameClock.instance != null)
            {
                GameClock.instance.SetDay(data.currentDay);
            }

            if (MoneyManager.instance != null)
            {
                MoneyManager.instance.SetMoney(data.currentMoney);
            }

            if (TutorialManager.instance != null)
            {
                TutorialManager.instance.LoadTutorialState(data.tutorialCompleted);
            }

            if (EndingManager.instance != null)
            {
                EndingManager.instance.LoadFinalData(data);
            }

            SaveManager.instance.ClearLoadState();

            if (endingUI != null)
            {
                endingUI.Show();
            }

            return;
        }

        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.SetMoney(data.currentMoney);
        }

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.LoadSaveData(data.upgrades);
        }

        if (DayStatsManager.instance != null)
        {
            DayStatsManager.instance.LoadTotalStats(data.totalMoneyEarned, data.totalServedCustomers, data.totalAngryCustomers);
        }

        if (DailyObjectiveManager.instance != null)
        {
            DailyObjectiveManager.instance.LoadTotalObjectivesCompleted(data.totalObjectivesCompleted);
        }

        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.LoadTutorialState(data.tutorialCompleted);
        }

        if (GameClock.instance != null)
        {
            GameClock.instance.SetDay(data.currentDay);
            GameClock.instance.SetTime(data.currentHour, data.currentMinute);
        }

        if (PlayerController.instance != null && PlayerController.instance.health != null)
        {
            PlayerController.instance.health.SetHP(data.currentHP);
        }

        SaveManager.instance.ClearLoadState();
    }
}