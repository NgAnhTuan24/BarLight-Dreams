using UnityEngine;

[CreateAssetMenu(fileName = "Wine Name", menuName = "Bar/Wine")]
public class WineSO : ScriptableObject
{
    public IngredientType wineName;
    public Sprite wineIcon;
}
