using UnityEngine;

public class CustomerInteraction : MonoBehaviour
{
    [SerializeField] private CustomerController customer;
    [SerializeField] private CustomerOrder customerOrder;

    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private bool playerInRange;

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
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
        }
    }
}