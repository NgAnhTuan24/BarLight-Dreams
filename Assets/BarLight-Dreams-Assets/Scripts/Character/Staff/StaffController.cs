using Pathfinding;
using System.Collections;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    [Header("Drink")]
    [SerializeField] private GameObject drinkBubble;
    [SerializeField] private SpriteRenderer drinkIcon;

    [Header("Movement")]
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform spawnPoint;

    private AIPath aiPath;
    private Animator animator;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.down;

    private PickupDrinkData currentDrink;

    private StaffState currentState = StaffState.Idle;

    public StaffState CurrentState => currentState;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        if (aiPath != null)
        {
            aiPath.canMove = false;
        }

        if (drinkBubble != null)
        {
            drinkBubble.SetActive(false);
        }

        if (drinkIcon != null)
        {
            drinkIcon.sprite = null;
        }
    }

    public void Initialize(Transform pickupPoint, Transform spawnPoint)
    {
        this.pickupPoint = pickupPoint;
        this.spawnPoint = spawnPoint;
        StartCoroutine(GoToPickupAfterDelay());
    }

    private IEnumerator GoToPickupAfterDelay()
    {
        currentState = StaffState.Idle;
        yield return new WaitForSeconds(1f);

        if (!MoveToPickup())
        {
            MoveToSpawnPoint();
        }
    }

    private void Update()
    {
        if (aiPath == null)
            return;

        UpdateMovementDirection();
        UpdateAnimator();

        switch (currentState)
        {
            case StaffState.GoingToPickup:
                CheckReachedPickup();
                break;
            case StaffState.GoingToCustomer:
                CheckReachedCustomer();
                break;
            case StaffState.ReturningHome:
                CheckReachedSpawnPoint();
                break;
        }
    }

    public void MoveTo(Transform target)
    {
        if (target == null)
            return;

        aiPath.destination = target.position;
        aiPath.canMove = true;
        aiPath.SearchPath();
    }

    public bool MoveToPickup()
    {
        if (pickupPoint == null)
            return false;

        if (PickupCounter.instance == null)
            return false;

        if (PickupCounter.instance.TryReserveDrink(this, out PickupDrinkData reservedDrink))
        {
            currentState = StaffState.GoingToPickup;
            MoveTo(pickupPoint);
            return true;
        }

        return false;
    }

    public bool TryTakeDrink()
    {
        if (currentDrink != null)
            return false;

        if (PickupCounter.instance == null)
            return false;

        currentDrink = PickupCounter.instance.TakeReservedDrink(this);

        if (currentDrink == null)
        {
            MoveToSpawnPoint();
            return false;
        }

        if (drinkIcon != null)
        {
            drinkIcon.sprite = currentDrink.recipe.drinkIcon;
        }

        if (drinkBubble != null)
        {
            drinkBubble.SetActive(true);
        }

        Debug.Log($"Staff: Took {currentDrink.recipe.displayName} for {currentDrink.customer.name}");
        DeliverDrink();
        return true;
    }

    public void DeliverDrink()
    {
        MoveToCustomer();
    }

    private void CheckReachedPickup()
    {
        if (pickupPoint == null)
            return;

        if (aiPath.pathPending)
            return;

        float distance = Vector2.Distance(transform.position, pickupPoint.position);

        if (distance > 0.4f)
            return;

        aiPath.canMove = false;
        TryTakeDrink();
    }

    private void CheckReachedCustomer()
    {
        if (currentDrink == null)
            return;

        if (currentDrink.customer == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        if (currentDrink.customer.CurrentState != CustomerState.WaitingDrink)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        if (aiPath.pathPending)
            return;

        float distance = Vector2.Distance(transform.position, currentDrink.customer.transform.position);

        if (distance > 2f)
            return;

        aiPath.canMove = false;
        DeliverToCustomer();
    }

    private void DeliverToCustomer()
    {
        if (currentDrink == null)
            return;

        if (currentDrink.customer == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        if (currentDrink.customer.CurrentState != CustomerState.WaitingDrink)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        CustomerOrder customerOrder = currentDrink.customer.GetComponent<CustomerOrder>();

        if (customerOrder == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        customerOrder.ReceiveDrink();
        ClearCurrentDrink();

        if (!MoveToPickup())
        {
            MoveToSpawnPoint();
        }
    }

    private void ClearCurrentDrink()
    {
        currentDrink = null;

        if (drinkBubble != null)
        {
            drinkBubble.SetActive(false);
        }

        if (drinkIcon != null)
        {
            drinkIcon.sprite = null;
        }
    }

    private void MoveToCustomer()
    {
        if (currentDrink == null)
            return;

        if (currentDrink.customer == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        currentState = StaffState.GoingToCustomer;
        MoveTo(currentDrink.customer.transform);
    }

    private void MoveToSpawnPoint()
    {
        if (spawnPoint == null)
            return;

        if (PickupCounter.instance != null)
        {
            PickupCounter.instance.CancelReservation(this);
        }

        currentState = StaffState.ReturningHome;
        MoveTo(spawnPoint);
    }

    private void CheckReachedSpawnPoint()
    {
        if (spawnPoint == null)
            return;

        if (MoveToPickup())
        {
            return;
        }

        if (aiPath.pathPending)
            return;

        float distance = Vector2.Distance(transform.position, spawnPoint.position);

        if (distance > 0.4f)
            return;

        aiPath.canMove = false;
        HandleReachedSpawnPoint();
    }

    private void HandleReachedSpawnPoint()
    {
        currentState = StaffState.Idle;
        aiPath.canMove = false;
        gameObject.SetActive(false);
        Debug.Log("Staff: Reached spawn point.");
    }

    private void UpdateMovementDirection()
    {
        if (!aiPath.canMove)
        {
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection = aiPath.velocity;
    }

    private void UpdateAnimator()
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

public enum StaffState
{
    Idle,
    GoingToPickup,
    GoingToCustomer,
    ReturningHome
}