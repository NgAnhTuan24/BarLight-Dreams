using UnityEngine;

[CreateAssetMenu(fileName = "Drink Name", menuName = "Bar/Customer")]
public class CustomerSO : ScriptableObject
{
    [Header("Gameplay")]
    public float waitOrderTime = 45f;
    public float waitDrinkTime = 90f;

    [Header("Tip")]
    public float tipChance = 0.5f;
    public float tipMultiplier = 1f;

    [Header("Drink Preference")]
    public DrinkRecipeSO[] favoriteDrinks; //làm sau
}
