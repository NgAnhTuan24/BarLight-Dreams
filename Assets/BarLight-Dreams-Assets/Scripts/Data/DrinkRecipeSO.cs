using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drink Name" ,menuName = "Bar/Drink Recipe")]
public class DrinkRecipeSO : ScriptableObject
{
    public DrinkType drinkType;
    public string displayName;

    public Sprite drinkIcon;

    public int price;

    public List<IngredientData> ingredients;

    public MixingSettings mixing = new();

    public RecipeTier recipeTier;

    [Min(1)]
    public int unlockDay = 1;
}

[System.Serializable]
public class IngredientData
{
    public IngredientType ingredientType;
    public Sprite ingredientIcon;
}