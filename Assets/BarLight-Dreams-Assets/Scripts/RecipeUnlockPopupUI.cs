using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUnlockPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Transform recipeContent;
    [SerializeField] private RecipeUnlockItem recipeUnlockItemPrefab;
    [SerializeField] private Button okButton;

    [Header("Recipe Source")]
    [SerializeField] private RecipeBookUI recipeBookUI;

    private Action onClosed;
    private bool isShowing;

    private void Awake()
    {
        okButton.onClick.RemoveListener(OnClickOK);
        okButton.onClick.AddListener(OnClickOK);
    }

    private void OnDestroy()
    {
        if (okButton != null)
        {
            okButton.onClick.RemoveListener(OnClickOK);
        }
    }

    public bool Show(int currentDay, Action onPopupClosed)
    {
        if (isShowing)
            return false;

        if (recipeBookUI == null)
        {
            Debug.LogError("RecipeUnlockPopupUI: RecipeBookUI reference is missing.");
            return false;
        }

        List<DrinkRecipeSO> newlyUnlockedRecipes = GetNewlyUnlockedRecipes(currentDay);

        if (newlyUnlockedRecipes.Count == 0)
            return false;

        isShowing = true;
        onClosed = onPopupClosed;

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        ClearRecipeItems();
        CreateRecipeItems(newlyUnlockedRecipes);
        UpdateTitle(newlyUnlockedRecipes.Count);

        gameObject.SetActive(true);

        return true;
    }

    private List<DrinkRecipeSO> GetNewlyUnlockedRecipes(int currentDay)
    {
        List<DrinkRecipeSO> newlyUnlockedRecipes = new List<DrinkRecipeSO>();

        foreach (DrinkRecipeSO recipe in recipeBookUI.Recipes)
        {
            if (recipe == null)
                continue;

            if (recipe.unlockDay == currentDay)
            {
                newlyUnlockedRecipes.Add(recipe);
            }
        }

        return newlyUnlockedRecipes;
    }

    private void ClearRecipeItems()
    {
        foreach (Transform child in recipeContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateRecipeItems(
        List<DrinkRecipeSO> newlyUnlockedRecipes)
    {
        foreach (DrinkRecipeSO recipe in newlyUnlockedRecipes)
        {
            RecipeUnlockItem item = Instantiate(recipeUnlockItemPrefab, recipeContent);

            item.Setup(recipe);
        }
    }

    private void UpdateTitle(int recipeCount)
    {
        if (recipeCount == 1)
        {
            titleText.text = "NEW RECIPE UNLOCKED!";
        }
        else
        {
            titleText.text = "NEW RECIPES UNLOCKED!";
        }
    }

    private void OnClickOK()
    {
        if (!isShowing)
            return;

        isShowing = false;

        gameObject.SetActive(false);

        UIManager.Instance.UnlockGameplayInput();
        UIManager.Instance.UnlockPauseInput();

        Action callback = onClosed;
        onClosed = null;

        callback?.Invoke();
    }
}