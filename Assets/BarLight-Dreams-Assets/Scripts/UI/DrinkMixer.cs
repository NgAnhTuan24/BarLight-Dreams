using System.Collections.Generic;
using UnityEngine;

public class DrinkMixer : MonoBehaviour
{
    [SerializeField] private List<DrinkRecipeSO> recipes;

    [Header("Result")]
    [SerializeField] private Sprite resultDrinkSprite;

    public void Mix()
    {
        if (!PlayerHoldItem.instance.HasCup())
        {
            Debug.Log("Need Cup!");
            return;
        }

        List<IngredientType> current = CounterBarUI.instance.GetIngredients();

        foreach (DrinkRecipeSO recipe in recipes)
        {
            if (IsMatch(recipe, current))
            {
                Debug.Log("Created: " + recipe.drinkName);

                CounterBarUI.instance.CleanCounter();

                PlayerHoldItem.instance.HoldDrink(recipe);

                return;
            }
        }

        Debug.Log("Wrong Recipe!");
    }

    private bool IsMatch(DrinkRecipeSO recipe, List<IngredientType> current)
    {
        if (recipe.ingredients.Count != current.Count)
        {
            return false;
        }

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            IngredientType recipeIngredient =
                recipe.ingredients[i].ingredientType;

            IngredientType currentIngredient =
                current[i];

            if (recipeIngredient != currentIngredient)
            {
                return false;
            }
        }

        return true;
    }
}