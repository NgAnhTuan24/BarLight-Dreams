using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WineButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private WineSO wine;
    private WineCabinetUI wineCabinetUI;

    public void Setup(WineSO wineData, WineCabinetUI ui)
    {
        wine = wineData;
        wineCabinetUI = ui;

        iconImage.sprite = wine.wineIcon;
        nameText.text = wine.wineName.ToString().Replace("_", " ");

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        CounterBarUI.instance.AddIngredient(
            wine.wineIcon,
            new Vector2(40, 90),
            40
            );
    }
}
