using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    [Header("Tier Visual")]
    [SerializeField] private Image visual;

    [Space(10)]

    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TMP_Text unlockDayText;

    [Header("Tier Colors")]
    [SerializeField] private Color tier1Color = new Color(0.30f, 0.70f, 0.35f);
    [SerializeField] private Color tier2Color = new Color(0.25f, 0.55f, 0.85f);
    [SerializeField] private Color tier3Color = new Color(0.65f, 0.35f, 0.80f);
    [SerializeField] private Color tier4Color = new Color(0.95f, 0.50f, 0.20f);

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

        if (isUnlocked)
        {
            ApplyTierVisual(recipe.recipeTier);
        }
        else
        {
            visual.gameObject.SetActive(false);
        }

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

    private void ApplyTierVisual(RecipeTier tier)
    {
        visual.gameObject.SetActive(true);

        switch (tier)
        {
            case RecipeTier.Tier1:
                visual.color = tier1Color;
                break;

            case RecipeTier.Tier2:
                visual.color = tier2Color;
                break;

            case RecipeTier.Tier3:
                visual.color = tier3Color;
                break;

            case RecipeTier.Tier4:
                visual.color = tier4Color;
                break;
        }
    }

    private void OnClick()
    {
        if (recipeBookUI == null)
            return;

        if (!RecipeProgressionManager.instance.IsRecipeUnlocked(recipe))
            return;

        recipeBookUI.ShowRecipe(recipe);
    }
}