using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public int CurrentSlot { get; private set; }

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

    public int GetEmptySlot()
    {
        for(int i = 1; i <=2; i++)
        {
            if (!SaveLoadSystem.HasSave(i))
            {
                return i;
            }
        }

        return -1;
    }

    public void SetCurrentSlot(int slot)
    {
        CurrentSlot = slot;
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        data.CurrentHP = PlayerController.instance.health.CurrentHP;
        data.CurrentDay = GameClock.instance.CurrentDay;
        data.CurrentMoney = MoneyManager.instance.CurrentMoney;

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
