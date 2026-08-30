using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [Header("Recipe List")]
    [SerializeField] private List<DrinkRecipeSO> recipes;

    [SerializeField] private Transform recipeButtonParent;
    [SerializeField] private RecipeButtonUI recipeButtonPrefab;

    [Header("Detail")]
    [SerializeField] private Image drinkIcon;
    [SerializeField] private TMP_Text drinkNameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Ingredients")]
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private IngredientItemUI ingredientPrefab;

    public IReadOnlyList<DrinkRecipeSO> Recipes => recipes;

    private void Start()
    {
        CreateRecipeButtons();
        ShowFirstUnlockedRecipe();

        GameClock.instance.OnNewDayStarted += RefreshRecipeBook;
    }

    private void OnDestroy()
    {
        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted -= RefreshRecipeBook;
        }
    }

    private void RefreshRecipeBook()
    {
        foreach (Transform child in recipeButtonParent)
        {
            Destroy(child.gameObject);
        }

        CreateRecipeButtons();
        ShowFirstUnlockedRecipe();
    }

    private void CreateRecipeButtons()
    {
        foreach (DrinkRecipeSO recipe in recipes)
        {
            //if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
            //    continue;

            RecipeButtonUI button = Instantiate(recipeButtonPrefab, recipeButtonParent);

            button.Setup(recipe, this);
        }
    }

    public void ShowRecipe(DrinkRecipeSO recipe)
    {
        drinkIcon.sprite = recipe.drinkIcon;

        drinkIcon.preserveAspect = true;
        drinkIcon.rectTransform.sizeDelta = IconSizeHelper.GetDrinkRecipeSize(recipe.drinkType);

        drinkNameText.text = recipe.displayName;
        priceText.text = recipe.price.ToString();

        foreach (Transform child in ingredientParent)
        {
            Destroy(child.gameObject);
        }

        foreach (IngredientData ingredient in recipe.ingredients)
        {
            IngredientItemUI item = Instantiate(ingredientPrefab, ingredientParent);

            item.SetupRecipeIngredient(ingredient);
        }
    }

    private void ShowFirstUnlockedRecipe()
    {
        foreach (DrinkRecipeSO recipe in recipes)
        {
            if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
                continue;

            ShowRecipe(recipe);
            break;
        }
    }
}