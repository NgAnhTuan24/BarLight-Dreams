using UnityEngine;
using UnityEngine.UI;

public class CounterIngredientUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void Setup(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}