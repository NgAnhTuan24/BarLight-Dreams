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
}

[System.Serializable]
public class IngredientData
{
    public IngredientType ingredientType;
    public Sprite ingredientIcon;
}