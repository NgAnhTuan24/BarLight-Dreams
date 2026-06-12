using UnityEngine;

public class FailDayUIHandler : MonoBehaviour
{
    public static FailDayUIHandler instance;

    public UIPopup failDayPopup;
    public FailDayUI failDayUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowFailDay()
    {
        failDayUI.Refresh();
        failDayPopup.Open();
    }
}
