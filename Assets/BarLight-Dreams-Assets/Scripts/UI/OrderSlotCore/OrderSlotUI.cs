using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OrderSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image drinkIcon;
    [SerializeField] private RectTransform visualRect;

    [SerializeField] private float hoverOffsetX = -100f;
    [SerializeField] private float hoverDuration = 0.2f;

    private Vector2 originalPos;
    private Vector2 visualOriginalPos;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        
        originalPos = rect.anchoredPosition;

        visualOriginalPos = visualRect.anchoredPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetOrder(DrinkRecipeSO recipe)
    {
        if (recipe == null)
        {
            ClearSlot();
            return;
        }

        rect.DOKill();
        canvasGroup.DOKill();

        gameObject.SetActive(true);

        drinkIcon.sprite = recipe.drinkIcon;

        drinkIcon.preserveAspect = true;
        drinkIcon.rectTransform.sizeDelta = IconSizeHelper.GetDrinkOrderSize(recipe.drinkType);

        canvasGroup.alpha = 0;
        rect.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.2f));
        seq.Join(rect.DOScale(1.2f, 0.2f));

        seq.Append(rect.DOScale(1f, 0.12f));
    }

    public void ClearSlot()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(0f, 0.15f));
        seq.Join(rect.DOScale(0f, 0.15f));

        seq.OnComplete(() =>
        {
            drinkIcon.sprite = null;
            gameObject.SetActive(false);
        });
    }

    public void SetOriginalPosition(Vector2 pos)
    {
        originalPos = pos;
    }

    public void HoverEnter()
    {
        visualRect.DOKill();

        visualRect.DOAnchorPos(new Vector2(visualOriginalPos.x + hoverOffsetX, visualOriginalPos.y), hoverDuration).SetEase(Ease.OutQuad);
    }

    public void HoverExit()
    {
        visualRect.DOKill();

        visualRect.DOAnchorPos(visualOriginalPos, hoverDuration).SetEase(Ease.OutQuad);
    }
}