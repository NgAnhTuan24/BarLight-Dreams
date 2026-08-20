using TMPro;
using UnityEngine;

public class UpgradeEffectUI : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;

    public void SetData(string label, string current, string next, string unit)
    {
        valueText.text = $"{label}: {current} → {next} {unit}";
    }

    public void SetMax(string label)
    {
        valueText.text = $"{label}: MAX";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}