using Pathfinding;
using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    //public float moveSpeed = 3f;
    #region Order
    [Header("Order")]
    [SerializeField] private DrinkRecipeSO[] possibleOrders;
    [SerializeField] private DrinkRecipeSO currentOrder;

    [Header("Bubble")]
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private SpriteRenderer bubbleDrinkIcon;

    public DrinkRecipeSO CurrentOrder => currentOrder;
    public CustomerState CurrentState => currentState;
    #endregion

    public bool HasOrdered { get; private set; }

    private Chair targetChair;
    private CustomerState currentState;

    private Animator animator;

    private AIPath aiPath;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.left;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    private void Start()
    {
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
        }
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
        Chair[] chairs = FindObjectsOfType<Chair>();

        foreach (Chair chair in chairs)
        {
            if (!chair.IsOccupied)
            {
                targetChair = chair;
                chair.Occupy();

                currentState = CustomerState.MovingToSeat;

                aiPath.destination = targetChair.sitPoint.position;

                aiPath.canMove = true;

                return;
            }
        }
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
    }

    public void TakeOrder()
    {
        if (currentState != CustomerState.WaitingOrder)
            return;

        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];

        HasOrdered = true;

        Debug.Log("Customer ordered: " + currentOrder.drinkName);

        ShowOrderBubble();

        currentState = CustomerState.WaitingDrink;
    }

    void ShowOrderBubble()
    {
        bubbleRoot.SetActive(true);

        bubbleDrinkIcon.sprite = currentOrder.drinkIcon;
    }

    public void TryGiveDrink()
    {
        if (currentState != CustomerState.WaitingDrink)
            return;

        if (!PlayerHoldItem.instance.HasDrink())
            return;

        DrinkData drinkData = PlayerHoldItem.instance.CurrentDrinkData;

        if (drinkData == null)
            return;

        if (drinkData.recipe == currentOrder)
        {
            ReceiveDrink();
        }
        else
        {
            Debug.Log("Wrong drink!");
        }
    }

    void ReceiveDrink()
    {
        Debug.Log("Correct Drink!");

        PlayerHoldItem.instance.Clear();

        bubbleRoot.SetActive(false);

        currentState = CustomerState.DrinkReceived;

        StartCoroutine(DrinkRoutine());
    }

    IEnumerator DrinkRoutine()
    {
        yield return new WaitForSeconds(5f);

        //LeaveBar();
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