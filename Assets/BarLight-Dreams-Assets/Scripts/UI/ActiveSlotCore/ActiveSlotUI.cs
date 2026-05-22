using UnityEngine;
using UnityEngine.UI;

public class ActiveSlotUI : MonoBehaviour
{
    public static ActiveSlotUI instance;

    [SerializeField] private Image itemIcon;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Clear();
    }

    public void Show(Sprite icon)
    {
        itemIcon.sprite = icon;
        itemIcon.gameObject.SetActive(true);
    }

    public void Clear()
    {
        itemIcon.gameObject.SetActive(false);
    }
}
