using System.Collections.Generic;
using UnityEngine;

public class FruitBowlUI : MonoBehaviour
{
    [Header("Fruit List")]
    [SerializeField] private List<FruitSO> fruits;

    [SerializeField] private Transform fruitButtonParent;
    [SerializeField] private FruitButtonUI fruitButtonPrefab;

    private void Start()
    {
        CreateFruitButtons();
    }

    private void CreateFruitButtons()
    {
        foreach (FruitSO fruit in fruits)
        {
            FruitButtonUI button = Instantiate(fruitButtonPrefab, fruitButtonParent);

            button.Setup(fruit, this);
        }
    }
}
