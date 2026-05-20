using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Bottle Size")]
    [SerializeField] private Vector2 bottleSize = new Vector2(25, 60);

    [Header("Fruit Size")]
    [SerializeField] private Vector2 fruitSize = new Vector2(40, 50);

    [Header("Ice Size")]
    [SerializeField] private Vector2 iceSize = new Vector2(40, 50);

    public void Setup(IngredientData data)
    {
        iconImage.sprite = data.ingredientIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = GetIconSize(data.ingredientType);

        nameText.text = data.ingredientType.ToString().Replace("_", " ");
    }

    private Vector2 GetIconSize(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Ice:
                return iceSize;

            case IngredientType.Peach:
            case IngredientType.Strawberry:
            case IngredientType.Cherry:
            case IngredientType.Apple:
            case IngredientType.Lemon:
            case IngredientType.Grape:
            case IngredientType.Orange:
            case IngredientType.Pineapple:
                return fruitSize;

            case IngredientType.Mystic_Absinthe:
            case IngredientType.Verdant_Bite:
            case IngredientType.Obsidian_Gin:
            case IngredientType.Radiant_Dew:
            case IngredientType.Golden_Rum_Rush:
            case IngredientType.Golden_Gin:
            case IngredientType.Ivory_Bloom:
            case IngredientType.Vanilla_Crash:
            case IngredientType.Honey_Flame:
            case IngredientType.Azuze_Spirit:
            case IngredientType.Citrus_Ember:
            case IngredientType.Pink_Lady:
                return bottleSize;

            default:
                return new Vector2(30, 60);
        }
    }
}