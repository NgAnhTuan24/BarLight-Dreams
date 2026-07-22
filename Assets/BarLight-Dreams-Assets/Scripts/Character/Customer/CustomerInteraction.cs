using UnityEngine;

public class CustomerInteraction : MonoBehaviour
{
    [SerializeField] private CustomerController customer;
    [SerializeField] private CustomerOrder customerOrder;

    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [SerializeField] private InteractionUIText interactionUIPrefab;
    [SerializeField] private Transform textAnchor;

    private InteractionUIText interactionUI;

    private bool playerInRange;

    private void Start()
    {
        interactionUI = Instantiate(interactionUIPrefab);
    }

    private void Update()
    {
        if (!playerInRange || !CanInteract())
        {
            interactionUI?.Hide();
            return;
        }

        interactionUI?.Show(textAnchor, interactKey);

        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private bool CanInteract()
    {
        if (customer.CurrentState == CustomerState.WaitingOrder)
        {
            return !OrderQueueManager.instance.IsFull;
        }

        if (customer.CurrentState == CustomerState.WaitingDrink)
        {
            return PlayerHoldItem.instance.HasDrink();
        }

        return false;
    }

    void Interact()
    {
        if (customer.CurrentState == CustomerState.WaitingOrder)
        {
            customerOrder.TakeOrder();
        }
        else if (customer.CurrentState == CustomerState.WaitingDrink)
        {
            customerOrder.TryGiveDrink();
        }

        if (!CanInteract())
        {
            interactionUI.Hide();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI?.Hide();
        }
    }

    private void OnDestroy()
    {
        if (interactionUI != null)
            Destroy(interactionUI.gameObject);
    }
}