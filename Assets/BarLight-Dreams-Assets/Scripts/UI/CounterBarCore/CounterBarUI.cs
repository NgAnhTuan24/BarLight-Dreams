using System.Collections.Generic;
using UnityEngine;

public class CounterBarUI : MonoBehaviour
{
    public static CounterBarUI instance;

    [SerializeField] private Transform itemParent;
    [SerializeField] private CounterIngredientUI itemPrefab;

    private int maxItems = 4;

    private List<GameObject> spawnedItems = new();

    private void Awake()
    {
        instance = this;
    }

    public void AddIngredient(Sprite icon, Vector2 size, float posY)
    {
        if (spawnedItems.Count >= maxItems)
        {
            Debug.Log("Counter Full!");
            return;
        }

        CounterIngredientUI item = Instantiate(itemPrefab, itemParent);

        item.Setup(icon, size, posY);

        spawnedItems.Add(item.gameObject);
    }

    public void ClearCounter()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }

        spawnedItems.Clear();
    }
}