using UnityEngine;
using UnityEngine.UI;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private DrinkRecipeSO recipe;
    private RecipeBookUI recipeBookUI;

    public void Setup(DrinkRecipeSO recipeData, RecipeBookUI ui)
    {
        recipe = recipeData;
        recipeBookUI = ui;

        iconImage.sprite = recipe.drinkIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetDrinkSize(recipe.drinkType);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        recipeBookUI.ShowRecipe(recipe);
    }
}