using UnityEngine;

public class PlayerHoldItem : MonoBehaviour
{
    public static PlayerHoldItem instance;

    public HoldItemType CurrentType { get; private set; }

    private Sprite currentIcon;

    private void Awake()
    {
        instance = this;
    }

    public void Hold(Sprite icon, HoldItemType type)
    {
        currentIcon = icon;

        CurrentType = type;

        ActiveSlotUI.instance.Show(icon);
    }

    public void Clear()
    {
        currentIcon = null;

        CurrentType = HoldItemType.None;

        ActiveSlotUI.instance.Clear();
    }

    public bool HasCup()
    {
        return CurrentType == HoldItemType.Cup;
    }

    public bool HasDrink()
    {
        return CurrentType == HoldItemType.Drink;
    }

    public bool IsEmpty()
    {
        return CurrentType == HoldItemType.None;
    }
}