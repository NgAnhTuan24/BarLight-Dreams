using UnityEngine;

public class UIPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject panelToShow;
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShowWhenClose;

    public void Open()
    {
        panelToShow.SetActive(true);

        foreach (var obj in objectsToHide)
            obj.SetActive(false);
    }

    public void Close()
    {
        panelToShow.SetActive(false);

        foreach (var obj in objectsToShowWhenClose)
            obj.SetActive(true);
    }
}