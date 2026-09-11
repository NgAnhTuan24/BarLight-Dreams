using System.Collections.Generic;
using UnityEngine;

public class DrinkMixer : MonoBehaviour
{
    public static DrinkMixer instance;

    [SerializeField] private List<DrinkRecipeSO> recipes;

    [SerializeField] private MixingMinigameUI minigameUI;

    [SerializeField] private FloatingPopupText popupText;

    [SerializeField] private float instantChance;

    private DrinkRecipeSO currentRecipe;

    public event System.Action<DrinkRecipeSO> OnMixingStarted;
    public event System.Action<DrinkRecipeSO> OnMixingCompleted;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetInstantChance(UpgradeManager.instance.GetInstantMixChance());
    }

    public void SetInstantChance(float chance)
    {
        instantChance = chance;
    }

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

        currentRecipe = GetCurrentRecipe();

        if (currentRecipe == null)
        {
            popupText.ShowText(PopupMessages.GetWrongRecipeMessage());

            return;
        }

        if (!RecipeProgressionManager.instance.IsRecipeUnlocked(currentRecipe))
        {
            popupText.ShowText("Recipe not unlocked yet!");

            currentRecipe = null;

            return;
        }

        OnMixingStarted?.Invoke(currentRecipe);

        if (Random.value <= instantChance)
        {
            Mix();
            return;
        }

        MixingSettings settings = currentRecipe.mixing;

        minigameUI.StartGame(currentRecipe, settings, Mix);
    }

    public void Mix()
    {
        if (currentRecipe == null)
        {
            return;
        }

        DrinkRecipeSO recipe = currentRecipe;

        currentRecipe = null;

        CounterBarUI.instance.CleanCounter();

        PlayerHoldItem.instance.HoldDrink(recipe);

        OnMixingCompleted?.Invoke(recipe);

        if (DailyObjectiveManager.instance != null)
        {
            DailyObjectiveManager.instance.RegisterDrinkMixed(recipe);
        }

        popupText.ShowText(PopupMessages.GetSuccessMessage());
    }

    private bool IsMatch(DrinkRecipeSO recipe, List<IngredientType> current)
    {
        if (recipe == null || recipe.ingredients == null || current == null)
        {
            return false;
        }

        if (recipe.ingredients.Count != current.Count)
        {
            return false;
        }

        List<IngredientType> remainingIngredients = new List<IngredientType>(current);

        foreach (IngredientData recipeIngredientData in recipe.ingredients)
        {
            IngredientType recipeIngredient = recipeIngredientData.ingredientType;

            int foundIndex = remainingIngredients.IndexOf(recipeIngredient);

            if (foundIndex == -1)
            {
                return false;
            }

            remainingIngredients.RemoveAt(foundIndex);
        }

        return remainingIngredients.Count == 0;
    }
}