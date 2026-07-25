using System.Collections.Generic;
using UnityEngine;

public class DrinkMixer : MonoBehaviour
{
    [SerializeField] private List<DrinkRecipeSO> recipes;

    [SerializeField] private MixingMinigameUI minigameUI;

    [SerializeField] private FloatingPopupText popupText;

    public bool CanMix()
    {
        return PlayerHoldItem.instance.HasCup() && CounterBarUI.instance.GetIngredients().Count > 0;
    }

    private DrinkRecipeSO GetCurrentRecipe()
    {
        List<IngredientType> current = CounterBarUI.instance.GetIngredients();

        foreach (DrinkRecipeSO recipe in recipes)
        {
            if (IsMatch(recipe, current))
                return recipe;
        }

        return null;
    }

    public void StartMixing()
    {
        if (!CanMix())
        {
            return;
        }

        DrinkRecipeSO recipe = GetCurrentRecipe();

        MixingSettings settings = recipe != null ? recipe.mixing : new MixingSettings();

        minigameUI.StartGame(settings, Mix);
    }

    public void Mix()
    {
        if (!CanMix())
        {
            return;
        }

        List<IngredientType> current = CounterBarUI.instance.GetIngredients();

        foreach (DrinkRecipeSO recipe in recipes)
        {
            if (IsMatch(recipe, current))
            {
                CounterBarUI.instance.CleanCounter();

                PlayerHoldItem.instance.HoldDrink(recipe);

                popupText.ShowText(PopupMessages.GetSuccessMessage());

                return;
            }
        }

        CounterBarUI.instance.CleanCounter();
        PlayerHoldItem.instance.Clear();
        popupText.ShowText(PopupMessages.GetFailMessage());
    }

    private bool IsMatch(DrinkRecipeSO recipe, List<IngredientType> current)
    {
        if (recipe.ingredients.Count != current.Count)
        {
            return false;
        }

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            IngredientType recipeIngredient = recipe.ingredients[i].ingredientType;

            IngredientType currentIngredient = current[i];

            if (recipeIngredient != currentIngredient)
            {
                return false;
            }
        }

        return true;
    }
}