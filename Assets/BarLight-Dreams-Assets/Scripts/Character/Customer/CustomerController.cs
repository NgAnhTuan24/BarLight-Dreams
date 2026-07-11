using Pathfinding;
using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private CustomerSO customerData;

    //public float moveSpeed = 3f;
    [Header("Leave")]
    [SerializeField] private Transform leavePoint;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtSFX;

    float counterCheckTimer;

    private CounterSlot targetCounterSlot;

    private Chair targetChair;
    private CustomerState currentState;

    private Animator animator;
    private AIPath aiPath;
    private CustomerOrder order;
    private CustomerPatience patience;
    private FloatingPopupText popupText;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.left;
    
    public CustomerState CurrentState => currentState;

    public CustomerSO Data => customerData;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        order = GetComponent<CustomerOrder>();
        patience = GetComponent<CustomerPatience>();
        popupText = GetComponentInChildren<FloatingPopupText>();
    }

    private void Start()
    {
        leavePoint = GameObject.FindGameObjectWithTag("ExitPoint").transform;

        EnterBar();
    }

    private void Update()
    {
        UpdateState();

        UpdateMovementDirection();
        UpdateAnimator();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case CustomerState.WaitingForCounter:
                TryFindCounterAgain();
                break;

            case CustomerState.FindCounter:
                MoveToCounter();
                break;

            case CustomerState.MovingToCounter:
                CheckReachedCounter();
                break;

            case CustomerState.FindSeat:
                FindSeat();
                break;

            case CustomerState.MovingToSeat:
                CheckReachedSeat();
                break;

            case CustomerState.Leaving:
                CheckReachedExit();
                break;
        }
    }

    public void ChangeState(CustomerState newState)
    {
        currentState = newState;
    }

    void EnterBar()
    {
        currentState = CustomerState.Enter;

        aiPath.canMove = false;

        StartCoroutine(EnterDelay());
    }

    IEnumerator EnterDelay()
    {
        yield return new WaitForSeconds(2f);

        currentState = CustomerState.FindCounter;
    }

    void TryFindCounterAgain()
    {
        counterCheckTimer += Time.deltaTime;

        if (counterCheckTimer < 2f) return;

        counterCheckTimer = 0f;

        currentState = CustomerState.FindCounter;
    }

    void MoveToCounter()
    {
        CounterSlot slot = CounterManager.instance.ReserveSlot(this);

        if (slot == null)
        {
            currentState = CustomerState.WaitingForCounter;
            return;
        }

        targetCounterSlot = slot;

        currentState = CustomerState.MovingToCounter;

        aiPath.destination = slot.transform.position;
        aiPath.canMove = true;
    }

    void CheckReachedCounter()
    {
        if (targetCounterSlot == null) return;
        
        if (aiPath.pathPending) return;

        if (Vector2.Distance(transform.position, targetCounterSlot.transform.position) > 0.2f) return;

        aiPath.canMove = false;

        currentState = CustomerState.WaitingOrder;

        order.ShowAlertBubble();

        patience.StartWaitingOrder();
    }

    public void ReleaseCounterSlot()
    {
        if (targetCounterSlot == null) return;

        targetCounterSlot.Release();
        targetCounterSlot = null;
    }

    void FindSeat()
    {
        Chair chair = ChairManager.instance.GetAvailableChair();

        if (chair == null)
            return;

        targetChair = chair;

        chair.Occupy();

        currentState = CustomerState.MovingToSeat;

        aiPath.destination = targetChair.sitPoint.position;

        aiPath.canMove = true;
    }

    void CheckReachedSeat()
    {
        if (targetChair == null)
        {
            LeaveBar();
            return;
        }

        float dist = Vector2.Distance(transform.position, targetChair.sitPoint.position);

        if (aiPath.pathPending) return;
        if (dist > 1.5f) return;

        SitDown();
    }

    void SitDown()
    {
        currentState = CustomerState.Sitting;

        aiPath.canMove = false;

        moveDirection = Vector2.zero;

        transform.position = targetChair.sitPoint.position;

        animator.SetBool("IsSitting", true);

        animator.SetInteger("SitDirection", targetChair.sitDirection == SitDirection.Left ? 0 : 1);

        StartCoroutine(SitRoutine());
    }

    IEnumerator SitRoutine()
    {
        yield return new WaitForSeconds(1f);

        currentState = CustomerState.WaitingDrink;

        patience.StartWaitingDrink();
    }

    public void OnDrinkReceived()
    {
        currentState = CustomerState.DrinkReceived;

        StartCoroutine(DrinkRoutine());
    }

    IEnumerator DrinkRoutine()
    {
        yield return new WaitForSeconds(6f);

        LeaveBar();
    }

    #region Rời quán với tâm trạng tức giận
    public void LeaveAngry()
    {
        if (order.CurrentOrder != null)
        {
            OrderQueueManager.instance.RemoveOrder(this);
        }

        order.AlertBubble.SetActive(false);
        order.DrinkBubble.SetActive(false);

        StartCoroutine(LeaveAngryRoutine());
    }

    IEnumerator LeaveAngryRoutine()
    {
        popupText.ShowText("Too slow!");

        order.ShowAngryBubble();

        DayStatsManager.instance.AddCustomersAngry(1);

        if (PlayerController.instance.health.CurrentHP > 0)
        {
            PlayerController.instance.health.TakeDamage(1);
            
            AudioManager.instance.PlaySFX(hurtSFX);
        }

        yield return new WaitForSeconds(1f);

        LeaveBar();
    }
    #endregion

    #region End Day - Khách Buộc phải rời quán
    public void ForceLeave()
    {
        StopAllCoroutines();

        if (order.CurrentOrder != null)
        {
            OrderQueueManager.instance.RemoveOrder(this);
        }

        order.AlertBubble.SetActive(false);
        order.DrinkBubble.SetActive(false);
        patience.StopPatience();

        LeaveBar();
    }
    #endregion

    void LeaveBar()
    {
        ReleaseCounterSlot();

        currentState = CustomerState.Leaving;

        animator.SetBool("IsSitting", false);

        if (targetChair != null)
        {
            targetChair.Leave();
        }

        aiPath.canMove = true;
        aiPath.destination = leavePoint.position;
        aiPath.SearchPath();
    }

    void CheckReachedExit()
    {
        if (aiPath.pathPending) return;

        if (Vector2.Distance(transform.position, leavePoint.position) > 0.2f) return;

        CustomerManager.instance.RemoveCustomer(this);

        Destroy(gameObject);
    }

    void UpdateMovementDirection()
    {
        if (!aiPath.canMove)
        {
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection = aiPath.velocity;
    }

    void UpdateAnimator()
    {
        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
        }

        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }

    private void OnEnable()
    {
        if (CustomerManager.instance != null)
        {
            CustomerManager.instance.RegisterCustomer(this);
        }
    }
}

public enum CustomerState
{
    Enter,

    WaitingForCounter,

    FindCounter,
    MovingToCounter,

    WaitingOrder,

    FindSeat,
    MovingToSeat,

    Sitting,
    WaitingDrink,
    DrinkReceived,

    Leaving
}