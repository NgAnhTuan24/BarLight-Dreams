using UnityEngine;

public class PlayerHoldItem : MonoBehaviour
{
    public static PlayerHoldItem instance;

    public bool HasItem { get; private set; }

    private Sprite currentIcon;

    private void Awake()
    {
        instance = this;
    }

    public void Hold(Sprite icon)
    {
        HasItem = true;
        currentIcon = icon;

        ActiveSlotUI.instance.Show(icon);
    }

    public void Clear()
    {
        HasItem = false;
        currentIcon = null;

        ActiveSlotUI.instance.Clear();
    }
}