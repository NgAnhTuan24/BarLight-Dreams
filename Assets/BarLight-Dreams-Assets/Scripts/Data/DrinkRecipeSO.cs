using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drink Name" ,menuName = "Bar/Drink Recipe")]
public class DrinkRecipeSO : ScriptableObject
{
    public string drinkName;
    public Sprite drinkIcon;
    public int price;

    public List<IngredientData> ingredients;
}

[System.Serializable]
public class IngredientData
{
    public IngredientType ingredientType;
    public Sprite ingredientIcon;
}

public enum IngredientType
{
    None,

    Ice,

    Peach,
    Strawberry,
    Cherry,
    Apple,
    Lemon,
    Grape,
    Orange,
    Pineapple,

    Mystic_Absinthe,
    Verdant_Bite,
    Obsidian_Gin,
    Radiant_Dew,
    Golden_Rum_Rush,
    Golden_Gin,
    Ivory_Bloom,
    Vanilla_Crash,
    Honey_Flame,
    Azuze_Spirit,
    Citrus_Ember,
    Pink_Lady,
}