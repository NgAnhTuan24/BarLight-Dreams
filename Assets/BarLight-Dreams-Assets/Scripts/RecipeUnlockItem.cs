using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUnlockItem : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Image drinkIcon;
    [SerializeField] private TMP_Text recipeNameText;

    public void Setup(DrinkRecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("RecipeUnlockItem.Setup received a null recipe.");
            return;
        }

        drinkIcon.sprite = recipe.drinkIcon;
        drinkIcon.preserveAspect = true;

        recipeNameText.text = recipe.displayName;
    }
}