using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [Header("Congratulations")]
    [SerializeField] private CanvasGroup congratulationsGroup;

    [Header("Rank")]
    [SerializeField] private CanvasGroup rankGroup;
    [SerializeField] private Image rankImage;

    [SerializeField] private Sprite rankSSprite;
    [SerializeField] private Sprite rankASprite;
    [SerializeField] private Sprite rankBSprite;
    [SerializeField] private Sprite rankCSprite;

    [Header("Score")]
    [SerializeField] private CanvasGroup scoreGroup;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Stats")]
    [SerializeField] private CanvasGroup moneyGroup;
    [SerializeField] private CanvasGroup customersGroup;
    [SerializeField] private CanvasGroup angryGroup;
    [SerializeField] private CanvasGroup upgradeGroup;
    [SerializeField] private CanvasGroup objectivesGroup;

    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text customersText;
    [SerializeField] private TMP_Text angryText;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private TMP_Text objectivesText;

    [Header("Button")]
    [SerializeField] private Button mainMenuButton;

    [Header("Main Menu")]
    [SerializeField] private InGame inGame;

    [Header("Animation Settings")]
    [SerializeField] private float objectDuration = 0.35f;
    [SerializeField] private float objectDelay = 0.08f;
    [SerializeField] private float scaleFrom = 0.8f;

    private Sequence showSequence;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
    }

    public void Show()
    {
        if (EndingManager.instance == null)
            return;

        Refresh();

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        gameObject.SetActive(true);

        PlayShowAnimation();


        SaveManager.instance.SaveGame();
        SaveManager.instance.SaveCompletedGame();
    }

    private void Refresh()
    {
        SetRankImage();

        finalScoreText.text = EndingManager.instance.FinalScore.ToString("0");
        
        moneyText.text = EndingManager.instance.Money.ToString();
        customersText.text = EndingManager.instance.Customers.ToString();
        angryText.text = EndingManager.instance.Angry.ToString();
        upgradeText.text = EndingManager.instance.TotalUpgradeLevel.ToString();
        objectivesText.text = EndingManager.instance.TotalObjectivesCompleted.ToString();
    }

    private void SetRankImage()
    {
        if (rankImage == null)
            return;

        switch (EndingManager.instance.FinalRank)
        {
            case "S":
                rankImage.sprite = rankSSprite;
                break;

            case "A":
                rankImage.sprite = rankASprite;
                break;

            case "B":
                rankImage.sprite = rankBSprite;
                break;

            case "C":
                rankImage.sprite = rankCSprite;
                break;

            default:
                rankImage.sprite = null;
                break;
        }
    }

    private void PlayShowAnimation()
    {
        showSequence?.Kill();

        ResetAnimationState();

        showSequence = DOTween.Sequence();

        showSequence.Append(
            ShowObject(congratulationsGroup)
        );

        showSequence.Append(
            ShowObject(rankGroup)
        );

        showSequence.Append(
            ShowObject(scoreGroup)
        );

        showSequence.Append(
            ShowObject(moneyGroup)
        );

        showSequence.AppendInterval(objectDelay);

        showSequence.Append(
            ShowObject(customersGroup)
        );

        showSequence.AppendInterval(objectDelay);

        showSequence.Append(
            ShowObject(angryGroup)
        );

        showSequence.AppendInterval(objectDelay);

        showSequence.Append(
            ShowObject(upgradeGroup)
        );

        showSequence.AppendInterval(objectDelay);

        showSequence.Append(
            ShowObject(objectivesGroup)
        );

        showSequence.AppendInterval(objectDelay);

        showSequence.Append(
            ShowButton()
        );
    }

    private Tween ShowObject(CanvasGroup group)
    {
        if (group == null)
            return DOTween.Sequence();

        group.alpha = 0f;
        group.transform.localScale = Vector3.one * scaleFrom;

        return DOTween.Sequence()
            .Join(
                group.DOFade(1f, objectDuration)
                    .SetEase(Ease.OutQuad)
            )
            .Join(
                group.transform
                    .DOScale(Vector3.one, objectDuration)
                    .SetEase(Ease.OutBack)
            );
    }

    private Tween ShowButton()
    {
        if (mainMenuButton == null)
            return DOTween.Sequence();

        Transform buttonTransform = mainMenuButton.transform;

        buttonTransform.localScale = Vector3.one * scaleFrom;

        mainMenuButton.interactable = false;

        return buttonTransform
            .DOScale(Vector3.one, objectDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                mainMenuButton.interactable = true;
            });
    }

    private void ResetAnimationState()
    {
        ResetGroup(congratulationsGroup);
        ResetGroup(rankGroup);
        ResetGroup(scoreGroup);

        ResetGroup(moneyGroup);
        ResetGroup(customersGroup);
        ResetGroup(angryGroup);
        ResetGroup(upgradeGroup);
        ResetGroup(objectivesGroup);

        if (mainMenuButton != null)
        {
            mainMenuButton.interactable = false;
            mainMenuButton.transform.localScale = Vector3.one * scaleFrom;
        }
    }

    private void ResetGroup(CanvasGroup group)
    {
        if (group == null)
            return;

        group.alpha = 0f;
        group.transform.localScale = Vector3.one * scaleFrom;
    }

    private void OnClickMainMenu()
    {
        mainMenuButton.interactable = false;

        showSequence?.Kill();

        inGame.ExitToMainMenu();
    }

    private void OnDestroy()
    {
        showSequence?.Kill();

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(OnClickMainMenu);
        }
    }
}