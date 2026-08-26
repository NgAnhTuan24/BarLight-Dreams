using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TMP_Text unlockDayText;

    private DrinkRecipeSO recipe;
    private RecipeBookUI recipeBookUI;

    public void Setup(DrinkRecipeSO recipeData, RecipeBookUI ui)
    {
        recipe = recipeData;
        recipeBookUI = ui;

        iconImage.sprite = recipe.drinkIcon;

        iconImage.preserveAspect = true;

        iconImage.rectTransform.sizeDelta = IconSizeHelper.GetDrinkSize(recipe.drinkType);

        bool isUnlocked = RecipeProgressionManager.instance.IsRecipeUnlocked(recipe);

        SetLockedState(!isUnlocked);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void SetLockedState(bool locked)
    {
        lockedOverlay.SetActive(locked);

        unlockDayText.gameObject.SetActive(locked);

        if (locked)
        {
            unlockDayText.text = $"Unlock Day {recipe.unlockDay}";
        }

        button.interactable = !locked;
    }

    private void OnClick()
    {
        if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
            return;

        recipeBookUI.ShowRecipe(recipe);
    }
}