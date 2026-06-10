using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterIngredientData
{
    public IngredientType type;
    public GameObject obj;

    public CounterIngredientData(IngredientType type, GameObject obj)
    {
        this.type = type;
        this.obj = obj;
    }
}

public class CounterBarUI : MonoBehaviour
{
    public static CounterBarUI instance;

    [SerializeField] private Transform itemParent;
    [SerializeField] private CounterIngredientUI itemPrefab;

    private int maxItems = 4;

    private List<CounterIngredientData> spawnedItems = new();

    private void Awake()
    {
        instance = this;
    }

    public void AddIngredient(IngredientType type, Sprite icon, Vector2 size, float posY)
    {
        if (spawnedItems.Count >= maxItems)
        {
            return;
        }

        CounterIngredientUI item = Instantiate(itemPrefab, itemParent);

        item.Setup(icon, size, posY);

        spawnedItems.Add(new CounterIngredientData(type, item.gameObject));
    }

    public void RemoveIngredient(GameObject item)
    {
        CounterIngredientData target = null;

        foreach (CounterIngredientData data in spawnedItems)
        {
            if (data.obj == item)
            {
                target = data;
                break;
            }
        }

        if (target != null)
        {
            spawnedItems.Remove(target);

            Destroy(item);
        }
    }

    public void CleanCounter()
    {
        foreach (CounterIngredientData data in spawnedItems)
        {
            Destroy(data.obj);
        }

        spawnedItems.Clear();
    }

    public List<IngredientType> GetIngredients()
    {
        List<IngredientType> list = new();

        foreach (CounterIngredientData data in spawnedItems)
        {
            list.Add(data.type);
        }

        return list;
    }
}