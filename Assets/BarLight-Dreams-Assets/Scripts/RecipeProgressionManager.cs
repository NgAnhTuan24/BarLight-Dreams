using UnityEngine;

public class RecipeProgressionManager : MonoBehaviour
{
    public static RecipeProgressionManager instance { get; private set; }

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

        return GameClock.instance.CurrentDay >= recipe.unlockDay;
    }
}