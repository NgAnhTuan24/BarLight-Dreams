using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text numberText;

    public void SetupRecipeIngredient(IngredientData data, int index)
    {
        iconImage.sprite = data.ingredientIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetIngredientSize(data.ingredientType);

        nameText.text = data.ingredientType.ToString().Replace("_", " ");

        numberText.text = index.ToString();
    }

    public void SetupOrderIngredient(IngredientData data, int index)
    {
        iconImage.sprite = data.ingredientIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetOrderIngredientSize(data.ingredientType);

        nameText.text = data.ingredientType.ToString().Replace("_", " ");

        numberText.text = index.ToString();
    }
}