using UnityEngine;
using UnityEngine.EventSystems;

public class OrderSlotHoverArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private OrderSlotUI slotUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotUI.HoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotUI.HoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        slotUI.OnClick();
    }
}