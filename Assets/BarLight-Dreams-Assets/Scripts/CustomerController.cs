using Pathfinding;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    //public float moveSpeed = 3f;

    private Chair targetChair;
    private CustomerState currentState;

    private Animator animator;

    private AIPath aiPath;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    private void Start()
    {
        FindSeat();
    }

    private void Update()
    {
        UpdateMovementDirection();
        UpdateAnimator();

        CheckReachedSeat();
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
        if (currentState != CustomerState.MovingToSeat)
            return;

        if (aiPath.reachedEndOfPath)
        {
            SitDown();
        }
    }

    void SitDown()
    {
        currentState = CustomerState.Sitting;

        aiPath.canMove = false;

        moveDirection = Vector2.zero;

        transform.position = targetChair.sitPoint.position;

        animator.SetBool("IsSitting", true);

        animator.SetInteger("SitDirection", targetChair.sitDirection == SitDirection.Left ? 0 : 1);

        Debug.Log("Customer sitting");
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
    Leaving
}