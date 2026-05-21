using UnityEngine;
using UnityEngine.UI;

public class CounterIngredientUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void Setup(Sprite icon, Vector2 size, float posY)
    {
        iconImage.sprite = icon;

        iconImage.preserveAspect = true;

        RectTransform rect = iconImage.rectTransform;
        rect.sizeDelta = size;

        Vector2 pos = rect.anchoredPosition;
        pos.y = posY;
        
        rect.anchoredPosition = pos;
    }
}