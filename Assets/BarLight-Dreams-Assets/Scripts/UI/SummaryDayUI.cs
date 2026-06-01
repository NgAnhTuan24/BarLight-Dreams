using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryDayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text earningsText;
    [SerializeField] private TMP_Text customerText;
    [SerializeField] private Button nextButton;

    private event Action onNext;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnClickNext);
    }

    public void Show(int day, int earnings, int customers, Action nextCallback)
    {
        gameObject.SetActive(true);

        dayText.text = $"DAY {day}";
        earningsText.text = $"{earnings}";
        customerText.text = $"{customers}";

        onNext = nextCallback;
    }

    void OnClickNext()
    {
        gameObject.SetActive(false);

        onNext?.Invoke();
    }
}
