using UnityEngine;

public class PickupDrinkSlot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer drinkIcon;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetDrink(DrinkRecipeSO recipe)
    {
        if (recipe == null)
        {
            Clear();
            return;
        }

        drinkIcon.sprite = recipe.drinkIcon;
        drinkIcon.enabled = true;

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        drinkIcon.sprite = null;
        drinkIcon.enabled = false;

        gameObject.SetActive(false);
    }
}