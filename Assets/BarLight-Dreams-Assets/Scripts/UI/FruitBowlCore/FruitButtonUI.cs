using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private FruitSO fruit;
    private FruitBowlUI fruitBowlUI;

    public void Setup(FruitSO fruitData, FruitBowlUI ui)
    {
        fruit = fruitData;
        fruitBowlUI = ui;

        iconImage.sprite = fruit.fruitIcon;
        nameText.text = fruit.fruitName.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {

    }
}
