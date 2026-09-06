using System;
using System.Collections.Generic;
using DG.Tweening;
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

    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showDuration = 0.25f;
    [SerializeField] private float hideDuration = 0.2f;
    [SerializeField] private float startScale = 0.95f;

    private Action onClosed;
    private bool isShowing;

    private Tween animationTween;

    private void Awake()
    {
        okButton.onClick.RemoveListener(OnClickOK);
        okButton.onClick.AddListener(OnClickOK);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDestroy()
    {
        if (okButton != null)
        {
            okButton.onClick.RemoveListener(OnClickOK);
        }

        animationTween?.Kill();
    }

    public bool Show(int currentDay, Action onPopupClosed)
    {
        if (isShowing)
            return false;

        if (recipeBookUI == null)
        {
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

        PlayShowAnimation();

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

    private void CreateRecipeItems(List<DrinkRecipeSO> newlyUnlockedRecipes)
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

    private void PlayShowAnimation()
    {
        animationTween?.Kill();

        transform.localScale = Vector3.one * startScale;
        canvasGroup.alpha = 0f;

        animationTween = DOTween.Sequence()
            .Join(canvasGroup.DOFade(1f, showDuration))
            .Join(transform.DOScale(1f, showDuration)
                .SetEase(Ease.OutBack));
    }

    private void OnClickOK()
    {
        if (!isShowing)
            return;

        isShowing = false;

        PlayHideAnimation();
    }

    private void PlayHideAnimation()
    {
        animationTween?.Kill();

        animationTween = DOTween.Sequence()
            .Join(canvasGroup.DOFade(0f, hideDuration))
            .Join(transform.DOScale(startScale, hideDuration)
                .SetEase(Ease.InBack))
            .OnComplete(FinishClose);
    }

    private void FinishClose()
    {
        gameObject.SetActive(false);

        UIManager.Instance.UnlockGameplayInput();
        UIManager.Instance.UnlockPauseInput();

        Action callback = onClosed;
        onClosed = null;

        callback?.Invoke();
    }
}