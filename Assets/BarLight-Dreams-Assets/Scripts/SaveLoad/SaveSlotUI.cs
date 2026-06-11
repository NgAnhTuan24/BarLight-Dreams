using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text saveNameText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text lastSaveText;

    [Header("Button")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Button deleteButton;

    [SerializeField] private int slotID;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameData data = SaveLoadSystem.LoadGame(slotID);

        bool hasSave = data != null;

        slotButton.interactable = hasSave;
        deleteButton.interactable = hasSave;

        if (!hasSave)
        {
            saveNameText.text = $"Empty Slot";
            dayText.text = "Day: ???";
            timeText.text = "Time: ???";
            moneyText.text = "Money: ???";
            lastSaveText.text = "Last Save: ???";

            return;
        }

        saveNameText.text = $"Save Slot {slotID}";
        dayText.text = $"Day: {data.currentDay}";
        timeText.text = $"Time: {data.currentHour:00}:{data.currentMinute:00}";
        moneyText.text = $"Money: {data.currentMoney}";
        lastSaveText.text = $"Last Save: {data.saveTime}";
    }

    public void LoadSlot()
    {
        if (!SaveLoadSystem.HasSave(slotID)) return;

        SaveManager.instance.StartLoadGame(slotID);

        SceneTransition.instance.FadeOut(() =>
        {
            SceneManager.LoadScene("GamePlay");
        });
    }

    public void DeleteSlot()
    {
        SaveManager.instance.DeleteSlot(slotID);

        Refresh();
    }
}