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

    private PickupDrinkData currentDrink;

    private StaffState currentState = StaffState.Idle;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();

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

        MoveToPickup();
    }

    private void Update()
    {
        if (aiPath == null)
            return;

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

    public bool TryTakeDrink()
    {
        if (currentDrink != null)
            return false;

        if (PickupCounter.instance == null)
            return false;

        if (!PickupCounter.instance.HasDrink())
        {
            MoveToSpawnPoint();
            return false;
        }

        currentDrink = PickupCounter.instance.TakeNextDrink();

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

        float distance = Vector2.Distance(
            transform.position,
            pickupPoint.position
        );

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

        if (aiPath.pathPending)
            return;

        float distance = Vector2.Distance(transform.position, currentDrink.customer.transform.position);

        if (distance > 1.2f)
            return;

        aiPath.canMove = false;

        DeliverToCustomer();
    }

    private void DeliverToCustomer()
    {
        if (currentDrink == null)
            return;

        CustomerController customer = currentDrink.customer;

        if (customer == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        CustomerOrder customerOrder = customer.GetComponent<CustomerOrder>();

        if (customerOrder == null)
        {
            ClearCurrentDrink();
            MoveToSpawnPoint();
            return;
        }

        customerOrder.ReceiveDrink();

        ClearCurrentDrink();

        if (PickupCounter.instance != null && PickupCounter.instance.HasDrink())
        {
            MoveToPickup();
        }
        else
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

    private void MoveToPickup()
    {
        if (pickupPoint == null)
            return;

        currentState = StaffState.GoingToPickup;

        MoveTo(pickupPoint);
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

        currentState = StaffState.ReturningHome;

        MoveTo(spawnPoint);
    }

    private void CheckReachedSpawnPoint()
    {
        if (spawnPoint == null)
            return;

        if (PickupCounter.instance != null && PickupCounter.instance.HasDrink())
        {
            MoveToPickup();
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

        Debug.Log("Staff: Reached spawn point.");
    }
}

public enum StaffState
{
    Idle,
    GoingToPickup,
    GoingToCustomer,
    ReturningHome
}