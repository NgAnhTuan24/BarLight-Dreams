using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    public void SetupRecipeIngredient(IngredientData data)
    {
        iconImage.sprite = data.ingredientIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetIngredientSize(data.ingredientType);

        nameText.text = data.ingredientType.ToString().Replace("_", " ");
    }

    public void SetupOrderIngredient(IngredientData data)
    {
        iconImage.sprite = data.ingredientIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetOrderIngredientSize(data.ingredientType);

        nameText.text = data.ingredientType.ToString().Replace("_", " ");
    }
}