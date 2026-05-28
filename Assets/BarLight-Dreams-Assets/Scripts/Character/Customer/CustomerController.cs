using Pathfinding;
using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    //public float moveSpeed = 3f;
    [Header("Leave")]
    [SerializeField] private Transform leavePoint;

    private Chair targetChair;
    private CustomerState currentState;

    private Animator animator;
    private AIPath aiPath;
    private CustomerOrder order;
    private CustomerPatience patience;
    private CustomerPopupText popupText;


    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.left;
    
    public CustomerState CurrentState => currentState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        order = GetComponent<CustomerOrder>();
        patience = GetComponent<CustomerPatience>();
        popupText = GetComponentInChildren<CustomerPopupText>();
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

        currentState = CustomerState.FindSeat;
    }

    void FindSeat()
    {
        Chair chair = ChairManager.Instance.GetAvailableChair();

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
        if (!aiPath.reachedEndOfPath)
            return;

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

        currentState = CustomerState.WaitingOrder;

        order.ShowAlertBubble();

        patience.StartWaitingOrder();
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

    public void LeaveAngry()
    {
        order.AlertBubble.SetActive(false);
        order.DrinkBubble.SetActive(false);

        StartCoroutine(LeaveAngryRoutine());
    }

    IEnumerator LeaveAngryRoutine()
    {
        popupText.ShowText("Too slow!");

        order.ShowAngryBubble();

        yield return new WaitForSeconds(1f);

        LeaveBar();
    }

    void LeaveBar()
    {
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
        if (aiPath.pathPending)
            return;

        if (Vector2.Distance(transform.position, leavePoint.position) > 0.2f)
            return;

        Destroy(gameObject);
    }

    void UpdateMovementDirection()
    {
        moveDirection = aiPath.desiredVelocity.normalized;
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
}

public enum CustomerState
{
    Enter,
    FindSeat,
    MovingToSeat,

    Sitting,
    WaitingOrder,
    WaitingDrink,
    DrinkReceived,

    Leaving
}