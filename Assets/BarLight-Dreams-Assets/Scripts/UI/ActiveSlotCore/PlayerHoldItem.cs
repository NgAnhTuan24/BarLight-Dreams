using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public DrinkRecipeSO recipe;

    public DrinkData(DrinkRecipeSO recipe)
    {
        this.recipe = recipe;
    }
}

public class PlayerHoldItem : MonoBehaviour
{
    public static PlayerHoldItem instance;

    public HoldItemType CurrentType { get; private set; }

    private DrinkData currentDrinkData;
    private Sprite currentIcon;

    public DrinkData CurrentDrinkData => currentDrinkData;

    private void Awake()
    {
        instance = this;
    }

    public void Hold(Sprite icon, HoldItemType type)
    {
        currentIcon = icon;

        CurrentType = type;

        ActiveSlotUI.instance.Show(icon);
    }

    public void HoldDrink(DrinkRecipeSO recipe)
    {
        currentDrinkData = new DrinkData(recipe);

        currentIcon = recipe.drinkIcon;

        CurrentType = HoldItemType.Drink;

        ActiveSlotUI.instance.Show(recipe.drinkIcon);
    }

    public void Clear()
    {
        currentDrinkData = null;

        currentIcon = null;

        CurrentType = HoldItemType.None;

        ActiveSlotUI.instance.Clear();
    }

    public bool HasCup()
    {
        return CurrentType == HoldItemType.Cup;
    }

    public bool HasDrink()
    {
        return CurrentType == HoldItemType.Drink;
    }

    public bool IsEmpty()
    {
        return CurrentType == HoldItemType.None;
    }
}