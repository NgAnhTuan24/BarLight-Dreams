using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private Image icon;

    [SerializeField] private TMP_Text titleText;

    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private UpgradeEffectUI effect1;

    [SerializeField] private UpgradeEffectUI effect2;

    [SerializeField] private TMP_Text priceText;

    [SerializeField] private Button upgradeButton;

    public void SetData(UpgradeViewData data)
    {
        icon.sprite = data.icon;

        titleText.text = data.title;

        descriptionText.text = data.description;

        levelText.text = data.isMaxLevel ? "MAX" : $"Lv {data.currentLevel}/{data.maxLevel}";

        if (data.isMaxLevel)
        {
            priceText.text = "MAX";
        }
        else
        {
            priceText.text = data.price.ToString();
        }

        if (data.isMaxLevel)
        {
            effect1.SetMax(data.effect1Label);
        }
        else
        {
            effect1.SetData(
                data.effect1Label,
                data.effect1Current,
                data.effect1Next,
                data.effect1Unit);
        }

        if (data.useEffect2)
        {
            if (effect2 != null)
            {
                effect2.Show();

                if (data.isMaxLevel)
                {
                    effect2.SetMax(data.effect2Label);
                }
                else
                {
                    effect2.SetData(
                        data.effect2Label,
                        data.effect2Current,
                        data.effect2Next,
                        data.effect2Unit);
                }
            }
        }
        else
        {
            if (effect2 != null)
            {
                effect2.Hide();
            }
        }

        bool canUpgrade = !data.isMaxLevel;

        if (canUpgrade)
        {
            canUpgrade = MoneyManager.instance.CurrentMoney >= data.price;
        }

        upgradeButton.interactable = canUpgrade;

        if (data.isMaxLevel)
        {
            priceText.color = Color.white;
        }
        else
        {
            priceText.color = canUpgrade ? Color.white : Color.red;
        }
    }

    public Button GetButton()
    {
        return upgradeButton;
    }
}