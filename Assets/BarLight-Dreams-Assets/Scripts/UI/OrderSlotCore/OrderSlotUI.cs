using UnityEngine;
using UnityEngine.UI;

public class OrderSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image drinkIcon;

    public void SetOrder(DrinkRecipeSO recipe)
    {
        if (recipe == null)
        {
            ClearSlot();
            return;
        }

        gameObject.SetActive(true);

        drinkIcon.sprite = recipe.drinkIcon;

        drinkIcon.preserveAspect = true;
        drinkIcon.rectTransform.sizeDelta = IconSizeHelper.GetDrinkOrderSize(recipe.drinkType);
    }

    public void ClearSlot()
    {
        drinkIcon.sprite = null;

        gameObject.SetActive(false);
    }
}