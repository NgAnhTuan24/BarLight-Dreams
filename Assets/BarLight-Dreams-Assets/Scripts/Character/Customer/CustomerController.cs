using Pathfinding;
using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    //public float moveSpeed = 3f;

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