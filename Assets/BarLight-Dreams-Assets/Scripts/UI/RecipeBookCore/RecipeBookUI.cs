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

    private void Start()
    {
        CreateRecipeButtons();

        if (recipes.Count > 0)
        {
            ShowRecipe(recipes[0]);
        }
    }

    private void CreateRecipeButtons()
    {
        foreach (DrinkRecipeSO recipe in recipes)
        {
            RecipeButtonUI button = Instantiate(recipeButtonPrefab, recipeButtonParent);

            button.Setup(recipe, this);
        }
    }

    public void ShowRecipe(DrinkRecipeSO recipe)
    {
        drinkIcon.sprite = recipe.drinkIcon;
        drinkNameText.text = recipe.drinkName;
        priceText.text = recipe.price.ToString();

        foreach (Transform child in ingredientParent)
        {
            Destroy(child.gameObject);
        }

        foreach (IngredientData ingredient in recipe.ingredients)
        {
            IngredientItemUI item = Instantiate(ingredientPrefab, ingredientParent);

            item.Setup(ingredient);
        }
    }
}