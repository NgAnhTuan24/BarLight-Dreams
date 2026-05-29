using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private RectTransform moneyRect;

    private Tween punchTween;

    void Start()
    {
        MoneyManager.instance.OnMoneyChanged += UpdateUI;

        UpdateUI(MoneyManager.instance.CurrentMoney);
    }
    
    private void UpdateUI(int money)
    {
        moneyText.text = money.ToString();

        PlayAnim();
    }

    private void PlayAnim()
    {
        if (punchTween != null && punchTween.IsActive())
            punchTween.Kill();

        moneyRect.localScale = Vector3.one;

        punchTween = moneyRect.DOPunchScale(Vector3.one * 0.3f, 0.4f, 7, 1f);
    }

    private void OnDestroy()
    {
        if (MoneyManager.instance != null)
        {
            MoneyManager.instance.OnMoneyChanged -= UpdateUI;
        }
    }
}
