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

    private void Awake()
    {
        rect = iconImage.rectTransform;
    }

    public void Setup(Sprite icon, Vector2 size, float posY)
    {
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;

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
        removeButton.interactable = false;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPos(rect.anchoredPosition + new Vector2(0, hiddenOffsetY), moveDuration).SetEase(Ease.InCubic));

        seq.Join(canvasGroup.DOFade(0, moveDuration));

        seq.OnComplete(() =>
        {
            CounterBarUI.instance.RemoveIngredient(gameObject);
        });
    }
}