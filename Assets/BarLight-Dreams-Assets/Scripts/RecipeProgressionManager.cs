using UnityEngine;

public class RecipeProgressionManager : MonoBehaviour
{
    public static RecipeProgressionManager instance { get; private set; }

    [Header("Recipe Source")]
    [SerializeField] private RecipeBookUI recipeBookUI;

    #if UNITY_EDITOR
    [Header("Test Mode")]
    [SerializeField] private bool unlockAllRecipes = false;
    #endif

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public bool IsRecipeUnlocked(DrinkRecipeSO recipe)
    {
        if (recipe == null)
            return false;
        
        #if UNITY_EDITOR
        if (unlockAllRecipes)
            return true;
        #endif

        return GameClock.instance.CurrentDay >= recipe.unlockDay;
    }

    public RecipeTier GetHighestUnlockedTier()
    {
        if (recipeBookUI == null)
        {
            Debug.LogWarning("RecipeProgressionManager: RecipeBookUI reference is missing.");

            return RecipeTier.Tier1;
        }

        RecipeTier highestTier = RecipeTier.Tier1;

        foreach (DrinkRecipeSO recipe in recipeBookUI.Recipes)
        {
            if (recipe == null)
                continue;

            if (!IsRecipeUnlocked(recipe))
                continue;

            if (recipe.recipeTier > highestTier)
            {
                highestTier = recipe.recipeTier;
            }
        }

        return highestTier;
    }
}