using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CounterIngredientUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button removeButton;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float hiddenOffsetY = -100f;

    private RectTransform rect;
    private IngredientType ingredientType;

    public IngredientType IngredientType => ingredientType;

    private void Awake()
    {
        rect = iconImage.rectTransform;
    }

    public void Setup(Sprite icon, IngredientType type, Vector2 size, float posY)
    {
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;

        ingredientType = type;

        rect.sizeDelta = size;

        Vector2 targetPos = rect.anchoredPosition;
        targetPos.y = posY;

        rect.anchoredPosition = new Vector2(targetPos.x, posY + hiddenOffsetY);

        canvasGroup.alpha = 0;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPos(targetPos, moveDuration).SetEase(Ease.OutCubic));

        seq.Join(canvasGroup.DOFade(1, moveDuration));

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (CounterBarUI.instance != null && CounterBarUI.instance.HasIngredientClickHandler)
        {
            bool accepted = CounterBarUI.instance.HandleIngredientClick(gameObject);

            if (accepted)
            {
                removeButton.interactable = false;
            }

            return;
        }

        removeButton.interactable = false;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPos(rect.anchoredPosition + new Vector2(0, hiddenOffsetY), moveDuration).SetEase(Ease.InCubic));

        seq.Join(canvasGroup.DOFade(0, moveDuration));

        seq.OnComplete(() =>
        {
            CounterBarUI.instance.RemoveIngredient(gameObject);
        });
    }

    public void MoveToTarget(Transform target, Action onComplete)
    {
        if (target == null)
        {
            onComplete?.Invoke();
            return;
        }

        removeButton.interactable = false;

        RectTransform targetRect = target as RectTransform;

        if (targetRect == null)
        {
            onComplete?.Invoke();
            return;
        }

        Vector3 targetWorldPosition = targetRect.position;

        Sequence seq = DOTween.Sequence();

        seq.Join(
            rect.DOMove(
                targetWorldPosition,
                moveDuration
            ).SetEase(Ease.InOutCubic)
        );

        seq.Join(
            rect.DOScale(
                0.6f,
                moveDuration
            ).SetEase(Ease.InOutCubic)
        );

        seq.Join(
            canvasGroup.DOFade(
                0.4f,
                moveDuration
            )
        );

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}