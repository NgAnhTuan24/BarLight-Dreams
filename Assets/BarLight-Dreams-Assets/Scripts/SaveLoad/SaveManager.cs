using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public int CurrentSlot { get; private set; }

    public bool IsLoadingGame { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame(int slot)
    {
        CurrentSlot = slot;
        IsLoadingGame = false;
    }

    public void StartLoadGame(int slot)
    {
        CurrentSlot = slot;
        IsLoadingGame = true;
    }

    public void ClearLoadState()
    {
        IsLoadingGame = false;
    }

    public int GetEmptySlot()
    {
        for(int i = 1; i <= 3; i++)
        {
            if (!SaveLoadSystem.HasSave(i))
            {
                return i;
            }
        }

        return -1;
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        data.currentHP = PlayerController.instance.health.CurrentHP;

        data.currentDay = GameClock.instance.CurrentDay;
        data.currentHour = GameClock.instance.CurrentHour;
        data.currentMinute = GameClock.instance.CurrentMinute;

        data.currentMoney = MoneyManager.instance.CurrentMoney;

        data.totalMoneyEarned = DayStatsManager.instance.TotalMoneyEarned;
        data.totalServedCustomers = DayStatsManager.instance.TotalServedCustomers;
        data.totalAngryCustomers = DayStatsManager.instance.TotalAngryCustomers;

        data.totalObjectivesCompleted = DailyObjectiveManager.instance.TotalObjectivesCompleted;

        data.upgrades = UpgradeManager.instance.GetSaveData();

        data.tutorialCompleted = TutorialManager.instance != null && TutorialManager.instance.IsTutorialCompleted;

        data.saveTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        SaveLoadSystem.SaveGame(data, CurrentSlot);
    }

    public void SaveCompletedGame()
    {
        if (CurrentSlot <= 0)
            return;

        if (EndingManager.instance == null)
            return;

        GameData data = new GameData();

        data.currentHP = PlayerController.instance.health.CurrentHP;

        data.currentDay = GameClock.instance.CurrentDay;
        data.currentHour = GameClock.instance.CurrentHour;
        data.currentMinute = GameClock.instance.CurrentMinute;

        data.currentMoney = MoneyManager.instance.CurrentMoney;

        data.totalMoneyEarned = DayStatsManager.instance.TotalMoneyEarned;
        data.totalServedCustomers = DayStatsManager.instance.TotalServedCustomers;
        data.totalAngryCustomers = DayStatsManager.instance.TotalAngryCustomers;

        data.totalObjectivesCompleted = DailyObjectiveManager.instance.TotalObjectivesCompleted;

        data.upgrades = UpgradeManager.instance.GetSaveData();

        data.tutorialCompleted = TutorialManager.instance != null && TutorialManager.instance.IsTutorialCompleted;

        data.gameCompleted = true;

        data.finalScore = EndingManager.instance.FinalScore;
        data.finalRank = EndingManager.instance.FinalRank;

        data.finalMoney = EndingManager.instance.Money;
        data.finalCustomers = EndingManager.instance.Customers;
        data.finalAngry = EndingManager.instance.Angry;

        data.finalTotalUpgradeLevel = EndingManager.instance.TotalUpgradeLevel;

        data.finalTotalObjectivesCompleted = EndingManager.instance.TotalObjectivesCompleted;

        data.saveTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        SaveLoadSystem.SaveGame(data, CurrentSlot);
    }

    public GameData LoadGame()
    {
        if (CurrentSlot <= 0)
        {
            return null;
        }

        return SaveLoadSystem.LoadGame(CurrentSlot);
    }

    public void DeleteSlot(int slot)
    {
        SaveLoadSystem.DeleteSave(slot);
    }
}
