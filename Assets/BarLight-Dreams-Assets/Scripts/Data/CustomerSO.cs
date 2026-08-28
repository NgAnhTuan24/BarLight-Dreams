using UnityEngine;

[CreateAssetMenu(fileName = "Drink Name", menuName = "Bar/Customer")]
public class CustomerSO : ScriptableObject
{
    [Header("Customer Type")]
    public CustomerType customerType = CustomerType.Normal;

    [Min(1)]
    public int unlockDay = 1;

    [Min(0f)]
    public float spawnWeight = 1f;

    [Header("Gameplay")]
    public float waitOrderTime = 45f;
    public float waitDrinkTime = 90f;

    [Header("Tip")]
    public float tipChance = 0.5f;
    public float tipMultiplier = 1f;

    [Header("Drink Preference")]
    public DrinkRecipeSO[] favoriteDrinks; //làm sau
}
