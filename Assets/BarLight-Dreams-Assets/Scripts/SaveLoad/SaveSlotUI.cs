using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text saveNameText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text lastSaveText;

    [SerializeField] private int slotID;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameData data = SaveLoadSystem.LoadGame(slotID);

        if (data == null)
        {
            saveNameText.text = $"Empty Slot";
            dayText.text = "Day: ???";
            moneyText.text = "Money: ???";
            lastSaveText.text = "Last Save: ???";

            return;
        }

        saveNameText.text = $"Save Slot {slotID}";
        dayText.text = $"Day: {data.currentDay}";
        moneyText.text = $"Money: {data.currentMoney}";
        lastSaveText.text = $"Last Save: {data.saveTime}";
    }

    public void LoadSlot()
    {
        if (!SaveLoadSystem.HasSave(slotID)) return;

        SaveManager.instance.StartLoadGame(slotID);

        SceneManager.LoadScene("GamePlay");
    }

    public void DeleteSlot()
    {
        SaveManager.instance.DeleteSlot(slotID);

        Refresh();
    }
}