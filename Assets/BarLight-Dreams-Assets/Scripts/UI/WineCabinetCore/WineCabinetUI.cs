using System.Collections.Generic;
using UnityEngine;

public class WineCabinetUI : MonoBehaviour
{
    [Header("Wine List")]
    [SerializeField] private List<WineSO> wines;

    [SerializeField] private Transform wineButtonParent;
    [SerializeField] private WineButtonUI wineButtonPrefab;

    private void Start()
    {
        CreateWineButtons();
    }

    private void CreateWineButtons()
    {
        foreach (WineSO wine in wines)
        {
            WineButtonUI button = Instantiate(wineButtonPrefab, wineButtonParent);

            button.Setup(wine, this);
        }
    }
}
