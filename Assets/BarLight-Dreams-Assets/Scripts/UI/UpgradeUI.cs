using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [SerializeField] private Button backButton;
    [SerializeField] private SummaryDayUI summaryDayUI;

    [SerializeField] private float startX = 1800f;

    [Header("Upgrade Items")]
    [SerializeField] private UpgradeItemUI counterItem;
    [SerializeField] private UpgradeItemUI orderSlotItem;
    [SerializeField] private UpgradeItemUI chairItem;
    [SerializeField] private UpgradeItemUI staffItem;
    [SerializeField] private UpgradeItemUI mixingItem;

    private void Awake()
    {
        backButton.onClick.AddListener(OnClickBack);

        counterItem.GetButton().onClick.AddListener(() => OnClickUpgrade(UpgradeType.Counter));
        orderSlotItem.GetButton().onClick.AddListener(() => OnClickUpgrade(UpgradeType.OrderSlot));
        chairItem.GetButton().onClick.AddListener(() => OnClickUpgrade(UpgradeType.Chair));
        staffItem.GetButton().onClick.AddListener(() => OnClickUpgrade(UpgradeType.Staff));
        mixingItem.GetButton().onClick.AddListener(() => OnClickUpgrade(UpgradeType.Mixing));
    }

    public void Show()
    {
        RefreshAll();

        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;

        panel.anchoredPosition = new Vector2(startX, 0);

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1f, 0.25f));

        seq.Join(
            panel.DOAnchorPos(Vector2.zero, 0.45f)
                 .SetEase(Ease.OutCubic)
        );
    }

    private void OnClickBack()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(0f, 0.2f));

        seq.Join(
            panel.DOAnchorPos(new Vector2(startX, 0), 0.35f)
                 .SetEase(Ease.InCubic)
        );

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);

            summaryDayUI.ShowFromUpgrade();
        });
    }

    private void RefreshAll()
    {
        counterItem.SetData(UpgradeManager.instance.GetViewData(UpgradeType.Counter));

        orderSlotItem.SetData(UpgradeManager.instance.GetViewData(UpgradeType.OrderSlot));

        chairItem.SetData(UpgradeManager.instance.GetViewData(UpgradeType.Chair));

        staffItem.SetData(UpgradeManager.instance.GetViewData(UpgradeType.Staff));

        mixingItem.SetData(UpgradeManager.instance.GetViewData(UpgradeType.Mixing));
    }

    private void OnClickUpgrade(UpgradeType type)
    {
        if (UpgradeManager.instance.IsMaxLevel(type))
        {
            Debug.Log($"Upgrade {type} is already at max level.");
            return;
        }

        int price = UpgradeManager.instance.GetUpgradePrice(type);

        if (!MoneyManager.instance.SpendMoney(price))
        {
            Debug.Log($"Not enough money to upgrade {type}. Required: {price}");
            return;
        }

        bool success = UpgradeManager.instance.Upgrade(type);

        if (!success) return;

        RefreshAll();
    }

    private void OnEnable()
    {
        MoneyManager.instance.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnDisable()
    {
        if (MoneyManager.instance != null)
            MoneyManager.instance.OnMoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int currentMoney)
    {
        RefreshAll();
    }
}