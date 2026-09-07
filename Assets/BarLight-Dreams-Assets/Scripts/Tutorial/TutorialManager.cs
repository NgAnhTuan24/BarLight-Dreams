using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialState
{
    Inactive,
    Running,
    Completed
}

public enum TutorialStep
{
    None,

    Introduction,
    GoToCustomer,
    TakeOrder,
    SelectOrder,
    GetCup,
    AddIngredients,
    StartMixing,
    CompleteMixing,
    ServeDrink
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance { get; private set; }

    [SerializeField] private OrderRecipeDetailViewer orderRecipeDetailViewer;

    [SerializeField] private UIPopup wineCabinetPopup;
    [SerializeField] private UIPopup fruitBowlPopup;

    [Header("Introduction")]
    [SerializeField] private float introductionGreetingDuration = 2f;

    public TutorialState State { get; private set; } = TutorialState.Inactive;

    public bool IsTutorialActive => State == TutorialState.Running;

    public TutorialStep CurrentStep { get; private set; } = TutorialStep.None;

    private CustomerController tutorialCustomer;
    private DrinkRecipeSO tutorialRecipe;

    private bool tutorialIngredientsReady;

    private bool introductionInputReady;
    private Coroutine introductionCoroutine;

    private bool pressedW;
    private bool pressedA;
    private bool pressedS;
    private bool pressedD;

    public event Action<TutorialStep> OnStepChanged;
    public event Action OnTutorialCompleted;
    public event Action OnIntroductionInputReady;

    public event Action OnTutorialUIHideRequested;
    public event Action OnTutorialUIShowRequested;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.Introduction)
            return;

        if (!introductionInputReady)
            return;

        CheckMovementKeys();
    }

    private void CheckMovementKeys()
    {
        if (Input.GetKeyDown(KeyCode.W))
            pressedW = true;

        if (Input.GetKeyDown(KeyCode.A))
            pressedA = true;

        if (Input.GetKeyDown(KeyCode.S))
            pressedS = true;

        if (Input.GetKeyDown(KeyCode.D))
            pressedD = true;

        if (pressedW && pressedA && pressedS && pressedD)
        {
            CompleteIntroduction();
        }
    }

    private void OnEnable()
    {
        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted += HandleNewDayStarted;
        }

        if (CustomerManager.instance != null)
        {
            CustomerManager.instance.OnCustomerRegistered += HandleCustomerRegistered;
        }

        if (OrderQueueManager.instance != null)
        {
            OrderQueueManager.instance.OnOrderSelected += HandleOrderSelected;
        }

        if (orderRecipeDetailViewer != null)
        {
            orderRecipeDetailViewer.OnOrderDetailOpened += HandleOrderDetailOpened;
            orderRecipeDetailViewer.OnOrderDetailClosed += HandleOrderDetailClosed;
        }

        if (PlayerHoldItem.instance != null)
        {
            PlayerHoldItem.instance.OnCupHeld += HandleCupHeld;
        }

        if (CounterBarUI.instance != null)
        {
            CounterBarUI.instance.OnIngredientAdded += HandleIngredientAdded;
        }

        if (wineCabinetPopup != null)
        {
            wineCabinetPopup.OnOpened += HandleIngredientPopupOpened;
            wineCabinetPopup.OnClosed += HandleIngredientPopupClosed;
        }

        if (fruitBowlPopup != null)
        {
            fruitBowlPopup.OnOpened += HandleIngredientPopupOpened;
            fruitBowlPopup.OnClosed += HandleIngredientPopupClosed;
        }

        if (DrinkMixer.instance != null)
        {
            DrinkMixer.instance.OnMixingStarted += HandleMixingStarted;
            DrinkMixer.instance.OnMixingCompleted += HandleMixingCompleted;
        }
    }

    private void OnDisable()
    {
        if (introductionCoroutine != null)
        {
            StopCoroutine(introductionCoroutine);
            introductionCoroutine = null;
        }

        if (GameClock.instance != null)
        {
            GameClock.instance.OnNewDayStarted -= HandleNewDayStarted;
        }

        if (CustomerManager.instance != null)
        {
            CustomerManager.instance.OnCustomerRegistered -= HandleCustomerRegistered;
        }

        if (OrderQueueManager.instance != null)
        {
            OrderQueueManager.instance.OnOrderSelected -= HandleOrderSelected;
        }

        if (orderRecipeDetailViewer != null)
        {
            orderRecipeDetailViewer.OnOrderDetailOpened -= HandleOrderDetailOpened;
            orderRecipeDetailViewer.OnOrderDetailClosed -= HandleOrderDetailClosed;
        }

        if (PlayerHoldItem.instance != null)
        {
            PlayerHoldItem.instance.OnCupHeld -= HandleCupHeld;
        }

        if (CounterBarUI.instance != null)
        {
            CounterBarUI.instance.OnIngredientAdded -= HandleIngredientAdded;
        }

        if (wineCabinetPopup != null)
        {
            wineCabinetPopup.OnOpened -= HandleIngredientPopupOpened;
            wineCabinetPopup.OnClosed -= HandleIngredientPopupClosed;
        }

        if (fruitBowlPopup != null)
        {
            fruitBowlPopup.OnOpened -= HandleIngredientPopupOpened;
            fruitBowlPopup.OnClosed -= HandleIngredientPopupClosed;
        }

        if (DrinkMixer.instance != null)
        {
            DrinkMixer.instance.OnMixingStarted -= HandleMixingStarted;
            DrinkMixer.instance.OnMixingCompleted -= HandleMixingCompleted;
        }
    }

    private void HandleNewDayStarted()
    {
        if (!CanStartTutorial())
            return;

        StartTutorial();
    }

    private void SetStep(TutorialStep step)
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep == step)
            return;

        CurrentStep = step;

        OnStepChanged?.Invoke(CurrentStep);
    }

    private bool CanStartTutorial()
    {
        if (GameClock.instance == null)
            return false;

        if (SaveManager.instance == null)
            return false;

        if (GameClock.instance.CurrentDay != 1)
            return false;

        if (SaveManager.instance.IsLoadingGame)
            return false;

        if (State != TutorialState.Inactive)
            return false;

        return true;
    }

    private void StartTutorial()
    {
        State = TutorialState.Running;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LockGameplayInput();
        }

        introductionInputReady = false;

        pressedW = false;
        pressedA = false;
        pressedS = false;
        pressedD = false;

        tutorialIngredientsReady = false;
        tutorialCustomer = null;
        tutorialRecipe = null;

        SetStep(TutorialStep.Introduction);

        if (introductionCoroutine != null)
        {
            StopCoroutine(introductionCoroutine);
        }

        introductionCoroutine = StartCoroutine(IntroductionRoutine());
    }

    private IEnumerator IntroductionRoutine()
    {
        yield return new WaitForSeconds(introductionGreetingDuration);

        if (!IsTutorialActive)
            yield break;

        if (CurrentStep != TutorialStep.Introduction)
            yield break;

        introductionInputReady = true;

        OnIntroductionInputReady?.Invoke();
    }

    private void CompleteIntroduction()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.Introduction)
            return;

        introductionInputReady = false;

        if (introductionCoroutine != null)
        {
            StopCoroutine(introductionCoroutine);
            introductionCoroutine = null;
        }

        SetStep(TutorialStep.GoToCustomer);

        if (GameClock.instance != null)
        {
            GameClock.instance.StartClock();
        }
    }

    private void HandleCustomerRegistered(CustomerController customer)
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.GoToCustomer)
            return;

        if (tutorialCustomer != null)
            return;

        StartCoroutine(WaitForCustomerReady(customer));
    }

    private IEnumerator WaitForCustomerReady(CustomerController customer)
    {
        if (customer == null)
            yield break;

        yield return new WaitUntil(() => customer == null || customer.CurrentState == CustomerState.WaitingOrder);

        if (customer == null)
            yield break;

        if (customer.CurrentState != CustomerState.WaitingOrder)
            yield break;

        tutorialCustomer = customer;

        CustomerOrder customerOrder = customer.GetComponent<CustomerOrder>();

        if (customerOrder != null)
        {
            customerOrder.OnOrderTaken += HandleTutorialOrderTaken;
            customerOrder.OnDrinkServed += HandleDrinkServed;
        }

        SetStep(TutorialStep.TakeOrder);
    }

    private void HandleTutorialOrderTaken(DrinkRecipeSO recipe)
    {
        if (!IsTutorialActive)
            return;

        CustomerOrder customerOrder = tutorialCustomer?.GetComponent<CustomerOrder>();

        if (customerOrder != null)
        {
            customerOrder.OnOrderTaken -= HandleTutorialOrderTaken;
        }

        SetStep(TutorialStep.SelectOrder);
    }

    private void HandleOrderSelected(OrderData order)
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.SelectOrder)
            return;

        if (order == null)
            return;

        if (tutorialCustomer == null)
            return;

        if (order.customer != tutorialCustomer)
            return;

        tutorialRecipe = order.recipe;
    }

    private void HandleOrderDetailOpened()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.SelectOrder && CurrentStep != TutorialStep.GetCup && CurrentStep != TutorialStep.AddIngredients)
            return;

        OnTutorialUIHideRequested?.Invoke();
    }

    private void HandleOrderDetailClosed()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.SelectOrder && CurrentStep != TutorialStep.GetCup && CurrentStep != TutorialStep.AddIngredients)
            return;

        if (CurrentStep == TutorialStep.SelectOrder)
        {
            if (tutorialRecipe == null)
            {
                OnTutorialUIShowRequested?.Invoke();
                return;
            }

            SetStep(TutorialStep.GetCup);
        }

        OnTutorialUIShowRequested?.Invoke();
    }

    private void HandleCupHeld()
    {
        if (!IsTutorialActive)
            return;

        if (tutorialCustomer == null)
            return;

        SetStep(TutorialStep.AddIngredients);
    }

    private void HandleIngredientAdded(IngredientType ingredient)
    {
        if (!IsTutorialActive)
            return;

        if (tutorialCustomer == null)
            return;

        if (CurrentStep != TutorialStep.AddIngredients)
            return;

        CheckTutorialIngredients();
    }

    private void CheckTutorialIngredients()
    {
        if (tutorialRecipe == null)
            return;

        if (CounterBarUI.instance == null)
            return;

        List<IngredientType> currentIngredients = CounterBarUI.instance.GetIngredients();

        if (currentIngredients.Count != tutorialRecipe.ingredients.Count)
            return;

        List<IngredientType> remainingIngredients = new List<IngredientType>(currentIngredients);

        foreach (IngredientData ingredientData in tutorialRecipe.ingredients)
        {
            int index = remainingIngredients.IndexOf(ingredientData.ingredientType);

            if (index == -1)
                return;

            remainingIngredients.RemoveAt(index);
        }

        if (remainingIngredients.Count != 0)
            return;

        tutorialIngredientsReady = true;
    }

    private void HandleIngredientPopupOpened()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.AddIngredients)
            return;

        OnTutorialUIHideRequested?.Invoke();
    }

    private void HandleIngredientPopupClosed()
    {
        if (!IsTutorialActive)
            return;

        if (CurrentStep != TutorialStep.AddIngredients)
            return;

        CheckTutorialIngredients();

        if (tutorialIngredientsReady)
        {
            SetStep(TutorialStep.StartMixing);
        }

        OnTutorialUIShowRequested?.Invoke();
    }

    private void HandleMixingStarted(DrinkRecipeSO recipe)
    {
        if (!IsTutorialActive)
            return;

        if (tutorialRecipe == null)
            return;

        if (recipe != tutorialRecipe)
            return;

        SetStep(TutorialStep.CompleteMixing);
    }

    private void HandleMixingCompleted(DrinkRecipeSO recipe)
    {
        if (!IsTutorialActive)
            return;

        if (tutorialRecipe == null)
            return;

        if (recipe != tutorialRecipe)
            return;

        SetStep(TutorialStep.ServeDrink);
    }

    private void HandleDrinkServed()
    {
        if (!IsTutorialActive)
            return;

        if (tutorialCustomer == null)
            return;

        CustomerOrder customerOrder = tutorialCustomer.GetComponent<CustomerOrder>();

        if (customerOrder != null)
        {
            customerOrder.OnDrinkServed -= HandleDrinkServed;
        }

        CompleteTutorial();
    }

    public void CompleteTutorial()
    {
        if (State != TutorialState.Running)
            return;

        if (introductionCoroutine != null)
        {
            StopCoroutine(introductionCoroutine);
            introductionCoroutine = null;
        }

        State = TutorialState.Completed;

        introductionInputReady = false;
        tutorialIngredientsReady = false;

        pressedW = false;
        pressedA = false;
        pressedS = false;
        pressedD = false;

        tutorialCustomer = null;
        tutorialRecipe = null;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnlockGameplayInput();
        }

        OnTutorialCompleted?.Invoke();
    }
}