using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Chair targetChair;
    private CustomerState currentState;

    private Animator animator;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        FindSeat();
    }

    private void Update()
    {
        switch (currentState)
        {
            case CustomerState.MovingToSeat:
                MoveToSeat();
                break;
        }

        UpdateAnimator();
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
                return;
            }
        }

        Debug.Log("No empty chair");
    }

    void MoveToSeat()
    {
        Vector3 targetPos = targetChair.sitPoint.position;

        Vector3 direction = (targetPos - transform.position).normalized;

        moveDirection = direction;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance < 0.05f)
        {
            transform.position = targetPos;

            moveDirection = Vector2.zero;

            currentState = CustomerState.Sitting;

            animator.SetBool("IsSitting", true);

            animator.SetInteger("SitDirection", targetChair.sitDirection == SitDirection.Left ? 0 : 1);

            Debug.Log("Customer sitting");
        }
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