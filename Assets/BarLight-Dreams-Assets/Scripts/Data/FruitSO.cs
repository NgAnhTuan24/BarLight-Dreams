using UnityEngine;

[CreateAssetMenu(fileName = "Fruit Name", menuName = "Bar/Fruit")]
public class FruitSO : ScriptableObject
{
    public IngredientType fruitName;
    public Sprite fruitIcon;
}
