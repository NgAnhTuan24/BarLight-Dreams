using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private RectTransform tutorialPanelRect;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text hintText;

    private bool temporarilyHidden;

    private void OnEnable()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnStepChanged += HandleStepChanged;
            TutorialManager.instance.OnTutorialCompleted += HandleTutorialCompleted;
            TutorialManager.instance.OnIntroductionInputReady += HandleIntroductionInputReady;

            TutorialManager.instance.OnTutorialUIHideRequested += HandleTemporaryHide;
            TutorialManager.instance.OnTutorialUIShowRequested += HandleTemporaryShow;
        }

        temporarilyHidden = false;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnStepChanged -= HandleStepChanged;
            TutorialManager.instance.OnTutorialCompleted -= HandleTutorialCompleted;
            TutorialManager.instance.OnIntroductionInputReady -= HandleIntroductionInputReady;

            TutorialManager.instance.OnTutorialUIHideRequested -= HandleTemporaryHide;
            TutorialManager.instance.OnTutorialUIShowRequested -= HandleTemporaryShow;
        }

        temporarilyHidden = false;
    }

    private void HandleTutorialCompleted()
    {
        temporarilyHidden = false;
        HideTutorial();
    }

    private void HandleStepChanged(TutorialStep step)
    {
        if (TutorialManager.instance == null)
        {
            HideTutorial();
            return;
        }

        if (!TutorialManager.instance.IsTutorialActive)
        {
            HideTutorial();
            return;
        }

        if (step == TutorialStep.CompleteMixing)
        {
            temporarilyHidden = true;
            HideTutorial();
            return;
        }

        temporarilyHidden = false;

        tutorialPanel.SetActive(true);

        UpdateStepContent(step);
    }

    private void HandleIntroductionInputReady()
    {
        if (TutorialManager.instance == null)
            return;

        if (!TutorialManager.instance.IsTutorialActive)
            return;

        if (TutorialManager.instance.CurrentStep != TutorialStep.Introduction)
            return;

        if (temporarilyHidden)
            return;

        instructionText.text = "Press all four WASD keys to move.";
        hintText.text = "W / A / S / D";
    }

    private void HandleTemporaryHide()
    {
        if (TutorialManager.instance == null)
            return;

        if (!TutorialManager.instance.IsTutorialActive)
            return;

        temporarilyHidden = true;

        tutorialPanel.SetActive(false);
    }

    private void HandleTemporaryShow()
    {
        if (TutorialManager.instance == null)
            return;

        if (!TutorialManager.instance.IsTutorialActive)
            return;

        temporarilyHidden = false;

        ShowTutorial();
    }

    public void RefreshUI()
    {
        if (TutorialManager.instance == null)
        {
            HideTutorial();
            return;
        }

        if (!TutorialManager.instance.IsTutorialActive)
        {
            HideTutorial();
            return;
        }

        if (TutorialManager.instance.CurrentStep == TutorialStep.CompleteMixing)
        {
            temporarilyHidden = true;
            HideTutorial();
            return;
        }

        ShowTutorial();
    }

    public void ShowTutorial()
    {
        if (TutorialManager.instance == null)
            return;

        if (!TutorialManager.instance.IsTutorialActive)
        {
            HideTutorial();
            return;
        }

        if (TutorialManager.instance.CurrentStep == TutorialStep.CompleteMixing)
        {
            temporarilyHidden = true;
            HideTutorial();
            return;
        }

        tutorialPanel.SetActive(true);

        UpdateStepContent(TutorialManager.instance.CurrentStep);
    }

    public void HideTutorial()
    {
        temporarilyHidden = false;
        tutorialPanel.SetActive(false);
    }

    private void SetPanelY(float y)
    {
        Vector2 position = tutorialPanelRect.anchoredPosition;
        position.y = y;
        tutorialPanelRect.anchoredPosition = position;
    }

    private void UpdateStepContent(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.Introduction:
                titleText.text = "TUTORIAL";
                instructionText.text = "Welcome! This tutorial will guide you through the basics of the game. Are you ready?";
                hintText.text = "";

                SetPanelY(328.8f);
                break;

            case TutorialStep.GoToCustomer:
                titleText.text = "FIRST CUSTOMER";
                instructionText.text = "Go to the bar counter and wait for the customer.";
                hintText.text = "";

                SetPanelY(328.8f);
                break;

            case TutorialStep.TakeOrder:
                titleText.text = "TAKE THE ORDER";
                instructionText.text = "Take the customer's order.";
                hintText.text = "Press F";

                SetPanelY(328.8f);
                break;

            case TutorialStep.SelectOrder:
                titleText.text = "CHECK THE ORDER";
                instructionText.text = "Select the customer's order.";
                hintText.text = "Click the order";

                SetPanelY(0f);
                break;

            case TutorialStep.GetCup:
                titleText.text = "GET A CUP";
                instructionText.text = "Pick up a cup to prepare the drink.";
                hintText.text = "";

                SetPanelY(328.8f);
                break;

            case TutorialStep.AddIngredients:
                titleText.text = "GET INGREDIENTS";
                instructionText.text = "Get the ingredients required by the recipe.";
                hintText.text = "";

                SetPanelY(328.8f);
                break;

            case TutorialStep.StartMixing:
                titleText.text = "MIX THE DRINK";
                instructionText.text = "Go to the bar counter and start mixing the drink.";
                hintText.text = "Press E";

                SetPanelY(328.8f);
                break;

            case TutorialStep.CompleteMixing:
                HideTutorial();
                break;

            case TutorialStep.ServeDrink:
                titleText.text = "SERVE THE DRINK";
                instructionText.text = "Bring the finished drink to the customer.";
                hintText.text = "Press F";

                SetPanelY(328.8f);
                break;

            default:
                HideTutorial();
                break;
        }
    }
}