using UnityEngine;

public class InteractionHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    [SerializeField] private UIPopup targetPopup;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private bool playerInRange;

    private void Start()
    {
        highlight.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (targetPopup != null && Input.GetKeyDown(interactKey))
        {
            targetPopup.Toggle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            highlight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            highlight.SetActive(false);

            if (targetPopup != null && targetPopup.IsOpen)
            {
                targetPopup.Close();
            }
        }
    }
}
