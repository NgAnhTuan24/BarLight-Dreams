using UnityEngine;

public class GameLoader : MonoBehaviour
{
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

        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.SetMoney(data.currentMoney);
        }

        if (GameClock.instance != null)
        {
            GameClock.instance.SetDay(data.currentDay);
        }

        if (PlayerController.instance != null && PlayerController.instance.health != null)
        {
            PlayerController.instance.health.SetHP(data.currentHP);
        }

        SaveManager.instance.ClearLoadState();
    }
}