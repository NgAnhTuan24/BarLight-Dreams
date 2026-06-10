using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void Start()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        if (SaveManager.instance == null) return;

        GameData data = SaveManager.instance.LoadGame();

        if (data == null) return;

        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.SetMoney(data.CurrentMoney);
        }

        if (GameClock.instance != null)
        {
            GameClock.instance.SetDay(data.CurrentDay);
        }

        if (PlayerController.instance != null && PlayerController.instance.health != null)
        {
            PlayerController.instance.health.SetHP(data.CurrentHP);
        }

    }
}