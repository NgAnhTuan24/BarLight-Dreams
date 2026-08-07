using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderRecipeDetailViewer : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [Header("Drink")]
    [SerializeField] private Image drinkImage;
    [SerializeField] private TMP_Text drinkNameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Ingredient")]
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private IngredientItemUI ingredientPrefab;

    private readonly List<IngredientItemUI> ingredientItems = new();

    private Tween currentTween;

    public bool IsShowing => root.activeSelf;

    private void Awake()
    {
        root.SetActive(false);

        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;
    }

    public void Show(DrinkRecipeSO recipe)
    {
        bool wasHidden = !root.activeSelf;

        if (wasHidden)
        {
            root.SetActive(true);
        }

        drinkImage.sprite = recipe.drinkIcon;
        drinkImage.preserveAspect = true;
        drinkImage.rectTransform.sizeDelta = IconSizeHelper.GetOrderRecipeSize(recipe.drinkType);

        drinkNameText.text = recipe.displayName;
        priceText.text = recipe.price.ToString();

        foreach (IngredientItemUI item in ingredientItems)
        {
            Destroy(item.gameObject);
        }

        ingredientItems.Clear();

        foreach (IngredientData ingredient in recipe.ingredients)
        {
            IngredientItemUI item = Instantiate(ingredientPrefab, ingredientParent);
            item.SetupOrderIngredient(ingredient);
            ingredientItems.Add(item);
        }

        if (wasHidden)
        {
            ShowAnim();
        }
    }

    public void Hide()
    {
        if (!root.activeSelf)
            return;

        HideAnim();
    }
    
    private void ShowAnim()
    {
        currentTween?.Kill();

        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;

        currentTween = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, 0.5f))
            .Join(panel.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
    }

    private void HideAnim()
    {
        currentTween?.Kill();
        currentTween = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0f, 0.5f))
            .Join(panel.DOScale(0.8f, 0.5f).SetEase(Ease.InBack))
            .OnComplete(() => root.SetActive(false));
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
