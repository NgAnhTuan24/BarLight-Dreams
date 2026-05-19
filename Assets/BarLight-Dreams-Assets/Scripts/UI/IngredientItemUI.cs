using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    public void Setup(IngredientData data)
    {
        iconImage.sprite = data.ingredientIcon;
        nameText.text = data.ingredientType.ToString().Replace("_", " "); ;
    }
}